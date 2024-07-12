using AutoMapper;
using JWTRefreshTokenInDotNet6.DTOS;
using JWTRefreshTokenInDotNet6.Models;

namespace JWTRefreshTokenInDotNet6.Helpers
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Course, CourseDTO>();

            CreateMap<CourseDTO, Course>();
            CreateMap<LectureDto, Lecture>();
            CreateMap<Lecture, LectureDto>();

            CreateMap<Lecture, ViewLecture>();
            CreateMap<ViewLecture, Lecture>();
              
        }
    }
}
