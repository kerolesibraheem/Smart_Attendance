using System.ComponentModel.DataAnnotations;

namespace JWTRefreshTokenInDotNet6.DTOS
{
    public class ViewAccount
    {
        [MaxLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [MaxLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string Email { get; set; }
        
        public string UserName {  get; set; }
    }
}
