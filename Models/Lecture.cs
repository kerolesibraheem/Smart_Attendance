using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWTRefreshTokenInDotNet6.Models
{
    public class Lecture
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Time { get; set; }
        [Required]
        public string Location { get; set; }
        public string LectureNotes { get; set; }
        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public QrCodeImages QrCodeImages { get; set; }
        public ICollection<ApplicationUser> Users { get; set;}
        public ICollection<Attendance> Attendances { get; set; }

    }
}
