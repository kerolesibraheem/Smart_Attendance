using JWTRefreshTokenInDotNet6.DTOS;
using JWTRefreshTokenInDotNet6.Models;

namespace JWTRefreshTokenInDotNet6.Services
{
    public interface ILectureServices
    {
        Task<Lecture> AddLecture(Lecture lecture);
        Task<IEnumerable<Lecture>> GetAllLectures();
        Task<Lecture> GetLectureById(int Id);
        Lecture DeleteLecture(Lecture lecture);
        Lecture UpdateLecture(Lecture Lecture);
    }
}
