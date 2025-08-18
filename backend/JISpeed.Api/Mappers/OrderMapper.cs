using AutoMapper;
using JISpeed.Api.DTOs;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Entities.Junctions;
using JISpeed.Core.Entities.Order;
using OrderEntity = JISpeed.Core.Entities.Order.Order;
namespace JISpeed.Api.Mappers
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<OrderEntity,OrderDto>();
            CreateMap<Payment,PaymentDto>();
            CreateMap<Coupon,CouponResponseDto>(); 
            CreateMap<Order, OrderDetailDto>()
                 .ForMember(dest => dest.OrderLogIds,
                    opt => opt.MapFrom(src => src.OrderLogs.Select(l => l.LogId)))
                .ForMember(dest => dest.PaymentIds,
                    opt => opt.MapFrom(src => src.Payments.Select(p => p.PayId)))
                .ForMember(dest => dest.RefundIds,
                    opt => opt.MapFrom(src => src.Refunds.Select(r => r.RefundId)))
                .ForMember(dest => dest.ComplaintIds,
                    opt => opt.MapFrom(src => src.Complaints.Select(c => c.ComplaintId)))
                .ForMember(dest => dest.ReviewIds,
                    opt => opt.MapFrom(src => src.Reviews.Select(rv => rv.ReviewId)));

            CreateMap<OrderDish, OrderDishDto>();
            CreateMap<OrderLog, OrderLogResponseDto>();
            CreateMap<Payment, PaymentResponseDto>();
            CreateMap<Refund, RefundResponseDto>();
        }

    }
}