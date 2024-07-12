using System.ComponentModel.DataAnnotations.Schema;

namespace JWTRefreshTokenInDotNet6.DTOS
{
    public class AttendanceDTO
    {
        [ForeignKey("ApplicationUser")]
        public string? UserId { get; set; }
        public DateTime AttendDate { get; set; }

        [ForeignKey("Lecture")]
        public int LecId { get; set; }
        public bool Status { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }
    }
}
