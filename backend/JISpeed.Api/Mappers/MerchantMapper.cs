using AutoMapper;
using JISpeed.Api.DTOs;
using JISpeed.Core.Entities.Dish;
using JISpeed.Core.Entities.Merchant;
using JISpeed.Core.Interfaces.IRepositories;

namespace JISpeed.Api.Mappers
{
    // 用户信息映射器 - 负责将实体对象转换为DTO对象
    public class MerchantProfile : Profile
    {
        public MerchantProfile()
        {
            // 配置 Merchant → MerchantDetailDto 的映射
            CreateMap<Merchant, MerchantDetailDto>()
                // 处理空值：如果 ContactInfo 为 null 则映射为 ""
                .ForMember(dest => dest.ContactInfo, 
                    opt => opt.MapFrom(src => src.ContactInfo ?? ""))
                // 处理空值：如果 Location 为 null 则映射为 ""
                .ForMember(dest => dest.Location, 
                    opt => opt.MapFrom(src => src.Location ?? ""));

            // 配置 SalesStat → SalesStatDto 的映射
            CreateMap<SalesStat, SalesStatDto>();
            // 配置 Dish → DishesDto 的映射
            CreateMap<Dish, DishesDto>();
        }
    }
}
