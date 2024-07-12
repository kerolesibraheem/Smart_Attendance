using JWTRefreshTokenInDotNet6.DTOS;
using JWTRefreshTokenInDotNet6.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace JWTRefreshTokenInDotNet6.Services
{
    public class CourseServices : ICourseServices
    {
        private readonly ApplicationDbContext dbContext;

        public CourseServices(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Course GetDetailsById(int Id)
        {
            return dbContext.Courses.SingleOrDefault(s => s.Id == Id);
           
        }
        public async Task<Course> AddCourse(Course course)
        {
            await dbContext.Courses.AddAsync(course);
            dbContext.SaveChanges();
            return course;
        }

        public Course DeleteCourse(Course course)
        {
            dbContext.Courses.Remove(course);
            dbContext.SaveChanges();
            return course;
        }
        public async Task<IEnumerable<object>> SelectCourses()
        {
            return await dbContext.Courses.Select(s=>new
            {
                Id = s.Id,
                Name = s.Name,
            }).ToListAsync();
        }
        public async Task<IEnumerable<object>> GetAllCourses()
        {
            return await dbContext.Courses
                .Select(course => new
                {
                    Id = course.Id,
                    Name = course.Name,
                    Level = course.Level
                })
                .ToListAsync();
        }

        public async Task<Course> GetCourseById(int Id)
        {
            return await dbContext.Courses.Include(l=>l.Lectures).SingleOrDefaultAsync(g=>g.Id == Id);
        }
       
        public Course UpdateCourse(Course course)
        {
            dbContext.Courses.Update(course);
            dbContext.SaveChanges();

            return course;
        }

        public Lecture DeleteLecture(Lecture lecture)
        {
            if (lecture.QrCodeImages != null)
            {
                dbContext.QrCodeImages.Remove(lecture.QrCodeImages);
            }
            dbContext.Lectures.Remove(lecture);
            dbContext.SaveChanges();
            return lecture;
        }

        public QrCodeImages DeleteQrcode(QrCodeImages qrcode)
        {
            dbContext.QrCodeImages.Remove(qrcode);
            dbContext.SaveChanges();
            return qrcode;
        }

        public async Task<IEnumerable<Course>> GetCourses()
        {
            return await dbContext.Courses
              .Include(c => c.Lectures)
           .ThenInclude(l => l.QrCodeImages)
            .Include(c => c.attendances) 
             .ToListAsync();
        }

        public async Task<bool> CourseExist(CourseDTO course)
        {
           var res = await dbContext.Courses.FirstOrDefaultAsync(
                c => c.Name == course.Name &&
                c.CreditHours == course.CreditHours &&
                c.Departement == course.Departement &&
                c.Level == course.Level &&
                c.Semester == course.Semester &&
                c.LowestAttendanceRate == course.LowestAttendanceRate &&
                c.AttendanceInstruction == course.AttendanceInstruction);
            if (res == null)
            {
                return false;
            }
            return true;
        }

        public Attendance DeleteAttendance(Attendance attendance)
        {

            dbContext.Attendances.RemoveRange(attendance);
            dbContext.SaveChanges();
            return attendance;
        }
    }
}

