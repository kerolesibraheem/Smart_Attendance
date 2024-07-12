using JWTRefreshTokenInDotNet6.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace JWTRefreshTokenInDotNet6.Services
{
    public class LectureServices : ILectureServices
    {
        private readonly ApplicationDbContext dbContext;

        public LectureServices(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IEnumerable<Lecture>> GetAllLectures()
        {
            return await dbContext.Lectures.ToListAsync();
        }

        //[Authorize(Roles = "Professor")]
        public async Task<Lecture> AddLecture(Lecture lecture)
        {
            await dbContext.Lectures.AddAsync(lecture);
            dbContext.SaveChanges();
            return lecture;
        }

        //[Authorize(Roles = "Professor")]
        public Lecture DeleteLecture(Lecture lecture)
        {
            dbContext.Lectures.Remove(lecture);
            dbContext.SaveChanges();
            return lecture;
        }

        //[Authorize(Roles = "Professor")]
        public async Task<Lecture> GetLectureById(int Id)
        {
            return await dbContext.Lectures.SingleOrDefaultAsync(g => g.Id == Id);
        }

        //[Authorize(Roles = "Professor")]
        public Lecture UpdateLecture(Lecture Lecture)
        {
           dbContext.Lectures.Update(Lecture);
            dbContext.SaveChanges();
            return Lecture;
        }

       
    }
}

