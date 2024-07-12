using JWTRefreshTokenInDotNet6.Models;
using JWTRefreshTokenInDotNet6.Tokens;
using Microsoft.AspNetCore.Mvc;

namespace JWTRefreshTokenInDotNet6.Services
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterProfessor(RegisterModel model);
        Task<AuthModel> RegisterStudent(RegisterModel model);
        Task<AuthModel> LoginAsync(LoginModel model);
        Task<string> AddRoleAsync(AddRoleModel model);
        Task<AuthModel> RefreshTokenAsync(string token);
        Task<bool> RevokeTokenAsync(string token);
        Task<string> ForgetPassword(ForgetPasswordModel model);

        
    }
}