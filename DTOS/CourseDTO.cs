using System.ComponentModel.DataAnnotations;

namespace JWTRefreshTokenInDotNet6.DTOS
{
    public class CourseDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        public int Semester { get; set; }
        public int CreditHours { get; set; }
        public string Departement { get; set; }
        public int LowestAttendanceRate { get; set; }
        [Required]
        public int Level { get; set; }
        public string AttendanceInstruction { get; set; }
    }
}
