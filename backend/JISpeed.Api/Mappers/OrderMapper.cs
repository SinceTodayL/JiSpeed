using AutoMapper;
using JISpeed.Api.DTOs;
using JISpeed.Core.Entities.Order;
using OrderEntity = JISpeed.Core.Entities.Order.Order;
namespace JISpeed.Api.Mappers
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<OrderEntity,OrderDto>();
        }

    }
}