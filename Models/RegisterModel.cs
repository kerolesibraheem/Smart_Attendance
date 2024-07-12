using System.ComponentModel.DataAnnotations;

namespace JWTRefreshTokenInDotNet6.Models
{
    public class RegisterModel
    {
        [MaxLength(100)]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }

        [MaxLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [MaxLength(100)]
        public string Username { get; set; }

        [MaxLength(128)]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(60), MinLength(8)]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d).{8,}$", ErrorMessage = "Password must be at least 8 characters.")]
        public string Password { get; set; }

        [MaxLength(60) , MinLength(8)]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d).{8,}$", ErrorMessage = "Password must be at least 8 characters.")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}