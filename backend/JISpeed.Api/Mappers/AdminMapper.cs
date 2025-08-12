using AutoMapper;
using JISpeed.Api.DTOs;
using JISpeed.Core.Entities.Admin;

namespace JISpeed.Api.Mappers
{
    public class AdminProfile : Profile
    {
        public AdminProfile()
        {
            CreateMap<Announcement, AnnouncementResponseDto>();
        }
    }
}