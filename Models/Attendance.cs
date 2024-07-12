using System.ComponentModel.DataAnnotations.Schema;

namespace JWTRefreshTokenInDotNet6.Models
{
    public class Attendance
    {
        public int Id { get; set; }

        [ForeignKey("ApplicationUser")]
        public string? UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public DateTime AttendDate { get; set; }

        [ForeignKey("Lecture")]
        public int LecId { get; set; }
        public Lecture Lecture { get; set; }
        public bool Status { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public Course Course { get; set; }

    }
}
