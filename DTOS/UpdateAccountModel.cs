using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace JWTRefreshTokenInDotNet6.DTOS
{
    public class UpdateAccountModel
    {
        [MaxLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [MaxLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [MaxLength(100)]
        public string Username { get; set; }

        [MaxLength(128)]
        [EmailAddress]
        public string Email { get; set; }
    }
}
