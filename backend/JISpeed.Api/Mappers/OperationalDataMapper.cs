using AutoMapper;
using JISpeed.Api.DTOs;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Entities.Junctions;
using JISpeed.Core.Entities.Order;
using JISpeed.Core.Interfaces.IServices;
using OrderEntity = JISpeed.Core.Entities.Order.Order;

namespace JISpeed.Api.Mappers
{
    public class OperationalDataProfile : Profile
    {
        public OperationalDataProfile()
        {
            CreateMap<OperationalDataDto,RecentOperationsDto>();
        }
    }
}