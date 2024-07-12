using AutoMapper;
using JWTRefreshTokenInDotNet6.DTOS;
using JWTRefreshTokenInDotNet6.Models;
using JWTRefreshTokenInDotNet6.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JWTRefreshTokenInDotNet6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseServices courseServices;
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly ApplicationDbContext context;
        public CourseController(ICourseServices courseServices, IMapper mapper, ApplicationDbContext context )
        {
            this.courseServices = courseServices;
            this.mapper = mapper;
            this.context = context;
        }

        //[Authorize(Roles = "Professor")]
        [HttpGet("GetAllCourses")]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await courseServices.GetAllCourses();
            return Ok(courses);
        }


        [HttpGet("SelectCourse")]
        public async Task<IActionResult> SelectCourses()
        {
            var course = await courseServices.SelectCourses();
            return Ok(course);
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> ViewCourseDetails(int Id)
        {
            var details = courseServices.GetDetailsById(Id);
            if(details is null)
            {
                return NotFound("Not Found !");
            }
            var course = mapper.Map<CourseDTO>(details);
            return Ok(course);
        }
      
        [HttpPost("AddCourse")]
        public async Task<IActionResult> AddCourse( CourseDTO courseDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            bool exist = await courseServices.CourseExist(courseDto);
            if (exist)
            {
                return Conflict("Course already exists.");
            }
            var course = mapper.Map<Course>(courseDto);
            courseServices.AddCourse(course);
            return Ok();
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateCourse(int Id , [FromBody] CourseDTO courseDTO)
        {
           var course = await courseServices.GetCourseById(Id);
            if (course == null)
                return NotFound("No Course Found With this Id");

            course.Name = courseDTO.Name;
            course.Level = courseDTO.Level;
            course.Semester = courseDTO.Semester;
            course.AttendanceInstruction = courseDTO.AttendanceInstruction;
            course.Departement = courseDTO.Departement;
            course.LowestAttendanceRate= courseDTO.LowestAttendanceRate;
            course.CreditHours= courseDTO.CreditHours;
            courseServices.UpdateCourse(course);
            return Ok(course);
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteCourse(int Id)
        {
            var course = await courseServices.GetCourseById(Id);
            if (course == null)
                return NotFound("No Course With this Id");
            foreach (var lecture in course.Lectures.ToList())
            {
                var attendances = context.Attendances.Where(a => a.LecId == lecture.Id);
                context.Attendances.RemoveRange(attendances);
                courseServices.DeleteLecture(lecture);
            }

            courseServices.DeleteCourse(course);
            return Ok("Done");
        }
        [HttpDelete("DeleteAllCourses")]
        public async Task<IActionResult> DeleteCourses()
        {
            var courses = await courseServices.GetCourses();
            if (courses == null)
                return NotFound("No courses found!");

            foreach(var course in courses)
            {
                foreach (var lecture in course.Lectures.ToList())
                {
                    foreach (var attendance in lecture.Attendances.ToList())
                    {
                        courseServices.DeleteAttendance(attendance);
                    }
                    courseServices.DeleteLecture(lecture);
                }
                courseServices.DeleteCourse(course);
            }
            return Ok("All courses Deleted !");
        }

    }
}
