using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWTRefreshTokenInDotNet6.DTOS
{
    public class LectureDto
    {
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
    }
}
