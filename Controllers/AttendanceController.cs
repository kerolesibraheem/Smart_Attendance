using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Drawing.Imaging;
using System.Drawing;
using JWTRefreshTokenInDotNet6.Models;
using Microsoft.EntityFrameworkCore;
using JWTRefreshTokenInDotNet6.DTOS;
using Microsoft.AspNetCore.Identity;
using JWTRefreshTokenInDotNet6.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AutoMapper.Features;
using AutoMapper;

namespace JWTRefreshTokenInDotNet6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;
        public AttendanceController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }
        [HttpGet("GetAllAttendanceForStudents")]
        public async Task<IActionResult> GetAllAttendances(string LectureName, string Coursename)
        {
            var lectures = await _context.Lectures
               .Where(l => l.Name == LectureName)
               .ToListAsync();

            if (lectures.Count == 0)
            {
                return NotFound("Lecture Not Found!");
            }
            var lectureIds = lectures.Select(l => l.Id).ToList();
            var attendance = await _context.Attendances
                .Include(a => a.ApplicationUser)
                .Include(a => a.Course)
                .Where(a => lectureIds.Contains(a.LecId) && a.Course.Name == Coursename)
                .Select(a => new ViewAtten
                   {
                    Username = a.ApplicationUser.UserName,
                    CourseName = a.Course.Name,
                    LectureName = a.Lecture.Name,
                    Email = a.ApplicationUser.Email,
                    Attend = a.Status,
                    Date = a.AttendDate
                })
            .ToListAsync();

            return Ok(attendance);
        }

        [HttpDelete("{Email}")]
        public async Task<IActionResult> DeleteAttendance(string Email)
        {
            var attend = await _context.Attendances.Where(s=>s.ApplicationUser.Email == Email).ToListAsync();
            if (attend.Count == 0)
            {
                return NotFound("There are no attendance records for this Email.");
            }
            _context.Attendances.RemoveRange(attend);
            await _context.SaveChangesAsync();
            return Ok("Deleted!");
        }
        [HttpGet("{LectureId}")]
        public IActionResult GetQrCode(int LectureId)
        {
            var qrCodeImage = _context.QrCodeImages.FirstOrDefault(q => q.lecId == LectureId);

            if (qrCodeImage == null)
            {
                return NotFound(); 
            }
            using (MemoryStream ms = new MemoryStream(qrCodeImage.Image))
            {
                Bitmap bitmap = new Bitmap(ms);
                // Convert the bitmap to a file and return it
                using (MemoryStream stream = new MemoryStream())
                {
                    bitmap.Save(stream, ImageFormat.Bmp);
                    return File(stream.ToArray(), "image/bmp");
                }
            }
        }

        [HttpGet("AttendenceView")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> ViewAttendance()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
            var attenddetails = await _context.Attendances.Include(x => x.ApplicationUser).Include(x => x.Course).
                Where(x => x.UserId == userId )
                .Select(a => new ScanResult
                {
                    CourseName = a.Course.Name,
                    Date = a.AttendDate,
                    LectureName = a.Lecture.Name,
                    Attend = a.Status
                }).ToListAsync();
            return Ok(attenddetails);
        }
        [HttpPost("{LectureId}")]
        public async Task<IActionResult> GenerateQrCode(int LectureId)
        {
            QRCodeGenerator qRCode = new QRCodeGenerator();
            QRCodeData codeData = qRCode.CreateQrCode(LectureId.ToString(), QRCodeGenerator.ECCLevel.Q);
            QRCode qR = new QRCode(codeData);
            Bitmap bitmap = qR.GetGraphic(20);
            byte[] bitmaparray = bitmap.ImageByteToArray();

            _context.QrCodeImages.Add(new QrCodeImages
            {
                lecId = LectureId,
                Image = bitmaparray
            });
            _context.SaveChanges();

            return File(bitmaparray, "image/bmp");
        }

        [HttpPost("MarkAttendance")]
        [Authorize(Roles = "Student")]
        public IActionResult MarkAttendance(MarkAttendRequest request )
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
            var existingattend = _context.Attendances.FirstOrDefault(l=>l.LecId == request.LectureId && l.UserId == userId);
            if (existingattend != null)
            {
                return BadRequest("Attendance already marked for this lecture.");
            }
            if (IsvalidQrcodeData(request.LectureId))
            {
                var lectur = _context.Lectures.FirstOrDefault(l=>l.Id == request.LectureId);
                var course = lectur.CourseId;
                var attendrec = new Attendance
                {
                    UserId = userId,
                    AttendDate = DateTime.Now,
                    Status = true,
                    LecId = request.LectureId,
                    CourseId = course
                };
                _context.Attendances.Add(attendrec);
                _context.SaveChanges();
                return Ok("Attendance Marked Successfully !");
            }
            return BadRequest();
        }
        private bool IsvalidQrcodeData(int lectureId)
        {
            var lecture = _context.QrCodeImages.FirstOrDefault(l=>l.lecId  == lectureId);
            if (lecture != null)
            {
                return true;
            }
            return false;
        }
    }
    public static class BitMapToArray
    {
        public static byte[] ImageByteToArray(this Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }

            
        }
    }
}
