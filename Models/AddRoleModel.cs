using System.ComponentModel.DataAnnotations;

namespace JWTRefreshTokenInDotNet6.Models
{
    public class AddRoleModel
    {
        [Required]
        public string UserId { get; set; }
        public string Role { get; set; }
    }
}