using JWTRefreshTokenInDotNet6.DTOS;
using JWTRefreshTokenInDotNet6.Models;

namespace JWTRefreshTokenInDotNet6.Services
{
    public interface ICourseServices
    {
        Task<IEnumerable<object>> GetAllCourses();
        Task<IEnumerable<Course>> GetCourses();
        Task<IEnumerable<object>> SelectCourses();
        Task<Course> AddCourse(Course course);
        Task<Course> GetCourseById(int Id);
        Course GetDetailsById(int Id);
        Course UpdateCourse(Course course);
        Task<bool> CourseExist(CourseDTO course);
        Lecture DeleteLecture(Lecture lecture);
        Attendance DeleteAttendance(Attendance attendance);
        Course DeleteCourse(Course course);
        QrCodeImages DeleteQrcode(QrCodeImages qrcode);
        
    }
}
