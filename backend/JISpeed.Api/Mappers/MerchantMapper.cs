using AutoMapper;
using JISpeed.Api.DTOs;
using JISpeed.Core.Entities.Dish;
using JISpeed.Core.Entities.Merchant;

namespace JISpeed.Api.Mappers
{
    public class MerchantProfile : Profile
    {
        public MerchantProfile()
        {
            // 配置 Merchant → MerchantDetailDto 的映射
            CreateMap<Merchant, MerchantDto>()
                // 处理空值：如果 ContactInfo 为 null 则映射为 ""
                .ForMember(dest => dest.ContactInfo, 
                    opt => opt.MapFrom(src => src.ContactInfo ?? ""))
                // 处理空值：如果 Location 为 null 则映射为 ""
                .ForMember(dest => dest.Location, 
                    opt => opt.MapFrom(src => src.Location ?? ""));

            CreateMap<UpdateMerchantDto, Merchant>();
            CreateMap<UpdateDishesDto, Dish>();
            CreateMap<CreateDishesDto, Dish>();
            // 配置 SalesStat → SalesStatDto 的映射
            CreateMap<SalesStat, SalesStatDto>();
            // 配置 Dish → DishesDto 的映射
            CreateMap<Dish, DishesDto>() 
                .ForMember(dest => dest.CategoryName, 
                opt => opt.MapFrom(src => src.Category.CategoryName)); 
            
            CreateMap<AuditRequest, Core.Entities.Merchant.Application>()
                .ForMember(dest => dest.AuditStatus, opt => opt.MapFrom(src => src.AuditStatus))
                .ForMember(dest => dest.RejectReason, opt => opt.MapFrom(src => src.RejectReason))
                .ForAllMembers(opt => opt.Ignore());
            
            CreateMap<ApplicationRequest, Core.Entities.Merchant.Application>()
                .ForMember(dest => dest.CompanyName, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Ignore());

            CreateMap<Category, CategoryDto>();

            CreateMap<Core.Entities.Merchant.Application, ApplicationResponse>();

        }
    }
}
