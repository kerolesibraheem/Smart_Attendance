using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace JWTRefreshTokenInDotNet6.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [MaxLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public List<RefreshToken>? refreshTokens { get; set; }
        public ICollection<Course>? Courses { get; set; }
        public ICollection<Lecture>? lectures { get; set; }

        public ICollection<Attendance>? Attendances { get; set; }

        public ICollection<Reports>? Reports { get; set; }
    }
}
