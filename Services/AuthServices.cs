using JWTRefreshTokenInDotNet6.Helpers;
using JWTRefreshTokenInDotNet6.Models;
using JWTRefreshTokenInDotNet6.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JWTRefreshTokenInDotNet6.Services
{
    public class AuthServices : IAuthService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly JWT jWT;
        public AuthServices(UserManager<ApplicationUser> userManager, IOptions<JWT> jwt, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.jWT = jwt.Value;
            this.roleManager = roleManager;
        }

        public async Task<AuthModel> RegisterProfessor(RegisterModel model)
        {
            if (await userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = "Email is already registered!" };

            if (await userManager.FindByNameAsync(model.Username) is not null)
                return new AuthModel { Message = "UserName is already registerd!" };

            var user = new ApplicationUser
            {
                FirstName = model.Username,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Username,
            };
            var res = await userManager.CreateAsync(user, model.Password);
            if (!res.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in res.Errors)
                    errors += $"{error.Description},";

                return new AuthModel { Message = errors };
            }

            await userManager.AddToRoleAsync(user, "Professor");
            var jwtSecurityToken = await CreateJwtToken(user);

            return new AuthModel
            {
                Email = user.Email,
                //ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "Professor" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName
            };
        }
        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await userManager.GetClaimsAsync(user);
            var roles = await userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jWT.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: jWT.Issuer,
                audience: jWT.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(jWT.DurationInMinutes),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
        public async Task<string> AddRoleAsync(AddRoleModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user is null || !await roleManager.RoleExistsAsync(model.Role))
                return "Invalid user ID or Role";

            if (await userManager.IsInRoleAsync(user, model.Role))
                return "User already assigned to this role";

            var result = await userManager.AddToRoleAsync(user, model.Role);

            return result.Succeeded ? string.Empty : "Sonething went wrong";
        }

        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using var generator = new RNGCryptoServiceProvider();

            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow
            };
        }

        public async Task<AuthModel> RefreshTokenAsync(string token)
        {
            var authmodel = new AuthModel();
            var user = await userManager.Users.SingleOrDefaultAsync(u => u.refreshTokens.Any(t => t.Token == token));
            if (user == null)
            {
                authmodel.IsAuthenticated = false;
                authmodel.Message = "InValid token";
                return authmodel;
            }
            var refreshToken = user.refreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
            {
                authmodel.Message = "Inactive token";
                return authmodel;
            }
            refreshToken.RevokedOn = DateTime.UtcNow;
            var newRefreshToken = GenerateRefreshToken();
            user.refreshTokens.Add(newRefreshToken);
            await userManager.UpdateAsync(user);

            var jwtToken = await CreateJwtToken(user);
            authmodel.IsAuthenticated = true;
            authmodel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authmodel.Email = user.Email;
            authmodel.Username = user.UserName;
            var roles = await userManager.GetRolesAsync(user);
            authmodel.Roles = roles.ToList();
            authmodel.RefreshToken = newRefreshToken.Token;
            authmodel.RefreshTokenExpiration = newRefreshToken.ExpiresOn;
            return authmodel;
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            var user = await userManager.Users.SingleOrDefaultAsync(u => u.refreshTokens.Any(t => t.Token == token));
            if (user == null)
                return false;

            var refreshToken = user.refreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
                return false;

            refreshToken.RevokedOn = DateTime.UtcNow;

            await userManager.UpdateAsync(user);

            return true;
        }

        public async Task<AuthModel> LoginAsync( LoginModel model)
        {
            var authmodel = new AuthModel();
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user !=null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var authclaims = new List<Claim>
                {
                     
                      new Claim(ClaimTypes.Email ,user.Email ),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                var userroles = await userManager.GetRolesAsync(user);
                foreach (var userRole in userroles)
                {
                    authclaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                var token = await CreateJwtToken(user);
                var refreshtoken = GenerateRefreshToken();

                authmodel.IsAuthenticated = true;
                authmodel.Username = user.UserName;
                authmodel.Email = user.Email;
                authmodel.Roles = userroles.ToList();
                authmodel.Token = new JwtSecurityTokenHandler().WriteToken(token);
                
                if(user.refreshTokens.Any(s=>s.IsActive))
                {
                    var activerefreshtoken = user.refreshTokens.FirstOrDefault(t => t.IsActive);
                    authmodel.RefreshToken = activerefreshtoken.Token;
                    authmodel.RefreshTokenExpiration = activerefreshtoken.ExpiresOn;
                }
                else
                {
                    var newrefreshtoken = GenerateRefreshToken();
                    authmodel.RefreshToken = newrefreshtoken.Token;
                    authmodel.RefreshTokenExpiration = newrefreshtoken.ExpiresOn;
                    user.refreshTokens.Add(newrefreshtoken);
                    await userManager.UpdateAsync(user);
                }
            }
            return authmodel;    
        }
        public async Task<string> ForgetPassword(ForgetPasswordModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if(user != null && await userManager.IsEmailConfirmedAsync(user))
             {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var res = await userManager.ResetPasswordAsync(user, token, model.Password);
                if (res != null)
                {
                    return "Password Reset Success !";
                }
                return "Something Went Wrong";
            }
            return "Invalid email";
        }

        public async Task<AuthModel> RegisterStudent(RegisterModel model)
        {
            if (await userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = "Email is already registered!" };

            if (await userManager.FindByNameAsync(model.Username) is not null)
                return new AuthModel { Message = "UserName is already registerd!" };

            var user = new ApplicationUser
            {
                FirstName = model.Username,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Username,
            };
            var res = await userManager.CreateAsync(user, model.Password);
            if (!res.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in res.Errors)
                    errors += $"{error.Description},";

                return new AuthModel { Message = errors };
            }

            await userManager.AddToRoleAsync(user, "Student");
            var jwtSecurityToken = await CreateJwtToken(user);

            return new AuthModel
            {
                Email = user.Email,
                //ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "Student" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName
            };

        }

    }
 }

