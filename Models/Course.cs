using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Composition;

namespace JWTRefreshTokenInDotNet6.Models
{
    public class Course
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        public int Semester { get; set; }
        public int CreditHours {  get; set; }
        public string Departement { get; set; }
        public int LowestAttendanceRate { get; set; }
        [Required]
        public int Level { get; set; }
        public string AttendanceInstruction { get; set; }
        public virtual ICollection<Lecture> Lectures { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
        public ICollection<Attendance> attendances { get; set; }
        public ICollection<Reports> reports { get; set; }

    }
}
