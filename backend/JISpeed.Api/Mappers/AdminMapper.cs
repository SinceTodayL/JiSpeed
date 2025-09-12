using AutoMapper;
using JISpeed.Api.DTOs;
using JISpeed.Core.Entities.Admin;
using JISpeed.Core.Entities.Reconciliation;

namespace JISpeed.Api.Mappers
{
    public class AdminProfile : Profile
    {
        public AdminProfile()
        {
            CreateMap<Announcement, AnnouncementResponseDto>();
            CreateMap<Reconciliation,ReconciliationResponseDto>()  
                .ForMember(
                dest => dest.OrderIdList,
                // 从关联的RelatedOrders中提取OrderId并转换为列表
                opt => opt
                    .MapFrom(src => src.Orders.Select(o => o.OrderId).ToList())
            );
        }
    }
}