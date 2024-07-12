using System.ComponentModel.DataAnnotations;

namespace JWTRefreshTokenInDotNet6.Models
{
    public class ChangePasswordModel
    {
        [Required]
        [MaxLength(200)]
        public string OldPassword { get; set; }
        [Required]
        [MaxLength(200)]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }

    }
}
