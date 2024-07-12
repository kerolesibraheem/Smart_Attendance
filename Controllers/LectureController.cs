using AutoMapper;
using JWTRefreshTokenInDotNet6.DTOS;
using JWTRefreshTokenInDotNet6.Models;
using JWTRefreshTokenInDotNet6.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace JWTRefreshTokenInDotNet6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LectureController : ControllerBase
    {
        private readonly ICourseServices courseServices;
        private readonly ILectureServices lectureServices;
        private readonly IMapper mapper;
        public LectureController(ILectureServices lectureServices, IMapper mapper, ICourseServices courseServices)
        {
            this.lectureServices = lectureServices;
            this.mapper = mapper;
            this.courseServices = courseServices;
        }
       
        [HttpGet("GetAllLectures")]
        public async Task<IActionResult> GetAllLectures()
        {
            var data = await lectureServices.GetAllLectures();
            var lecture = mapper.Map<IEnumerable<ViewLecture>>(data);
            return Ok(lecture);
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetLectureById(int Id)
        {
            var lec = await lectureServices.GetLectureById(Id);
            if (lec == null)
            {
                return NotFound();
            }
            var data = mapper.Map<LectureDto>(lec);
            return Ok(data);
        }
        //[Authorize(Roles = "Professor")]
        [HttpPost("AddLecture")]
        public async Task<IActionResult> AddLecture( LectureDto lectureDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var course = await courseServices.GetCourseById(lectureDto.CourseId);
            if(course is null)
            {
                return NotFound("Course Id is not found!");
            }

            var lec = mapper.Map<Lecture>(lectureDto);
            lectureServices.AddLecture(lec);
            var addedLectureDto = mapper.Map<LectureDto>(lec);

            return Ok(addedLectureDto);
        }

        //[Authorize(Roles = "Professor")]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteLecture(int Id)
        {
            var lecture = await lectureServices.GetLectureById(Id);
            if (lecture == null)
                return NotFound("No Course With this Id");
       
            lectureServices.DeleteLecture(lecture);
            return Ok("Done");
        }

        //[Authorize(Roles = "Professor")]
        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateLecture(int Id , LectureDto lectureDto)
        {
            var lec = await lectureServices.GetLectureById(Id);
            if (lec == null)
                return NotFound("No Lecture Found With this Id");
            lec.Name = lectureDto.Name;
            lec.Time = lectureDto.Time;
            lec.Date = lectureDto.Date;
            lec.LectureNotes = lectureDto.LectureNotes;
            lec.Location = lectureDto.Location;
            lec.CourseId = lectureDto.CourseId;

            var updatedlec = new ViewLecture
            {
                Id = Id,
                Name = lec.Name,
                Date = lec.Date,
                Time = lec.Time,
                Location = lec.Location,
                LectureNotes = lec.LectureNotes,
                CourseId = lec.CourseId
            };
            lectureServices.UpdateLecture(lec);
            return Ok(updatedlec);
        }
    }
}
