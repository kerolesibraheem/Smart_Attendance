using System.ComponentModel.DataAnnotations;

namespace JWTRefreshTokenInDotNet6.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Enter a valid Email!")]
        public string Email { get; set; }

        [MaxLength(60), MinLength(8)]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d).{8,}$", ErrorMessage = "Password must be at least 8 characters.")]
        [Required]
        public string Password { get; set; }
    }
}
