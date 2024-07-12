using AutoMapper;
using JWTRefreshTokenInDotNet6.DTOS;
using JWTRefreshTokenInDotNet6.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JWTRefreshTokenInDotNet6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel change)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var checkuser = await userManager.FindByNameAsync(user);
            if (!await userManager.CheckPasswordAsync(checkuser, change.OldPassword)){
                return BadRequest("Invalid Password!");
            }
            var res = await userManager.ChangePasswordAsync(checkuser,change.OldPassword,change.NewPassword);
            if (res.Succeeded)
            {
                return Ok("Password Changed successfully!");
            }
            else
            {
                return BadRequest("Failed To Change!");
            }
        }
        [HttpGet("ViewAccount")]
        public async Task<IActionResult> ViewProfile()
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var users = await userManager.FindByNameAsync(user);
            ViewAccount view = new ViewAccount();
            view.FirstName = users.FirstName;
            view.LastName = users.LastName;
            view.Email = users.Email;
            view.UserName = users.UserName;

            return Ok(view);
        }

        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(UpdateAccountModel accountModel)
        {
            var user = await userManager.FindByNameAsync(accountModel.Username);
            if(user is null)
            {
                return NotFound("Account Not found!");
            }

            user.FirstName = accountModel.FirstName;
            user.LastName = accountModel.LastName;
            user.Email = accountModel.Email;

            var res = await userManager.UpdateAsync(user);
            if (!res.Succeeded)
            {
                return BadRequest("Account with the same username or email already exists.");
            }
            return Ok("Profile Updated Successfully!");
        }

        //[Authorize]
        [HttpDelete("DeleteAccount")]
        public async Task<IActionResult> DeleteAccount()
        {
           var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var del = await userManager.FindByNameAsync(user);
            if(user is null)
            {
                return NotFound("User Not Found");
            }

            var res = await userManager.DeleteAsync(del);
            if(res.Succeeded)
            {
                return Ok("Account Deleted Successfully!");
            } else
            {
                return BadRequest();
            }
        }
      
    }
}
