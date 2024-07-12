using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWTRefreshTokenInDotNet6.Models
{
    public class Reports
    {
        public int Id { get; set; }
        [Required]
        public int TotalAbsant { get; set; }
        [Required]
        public DateTime Date { get; set; }
       
        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public Course Course { get; set; }

        [ForeignKey("ApplicationUser")]
        public string? UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

    }
}
