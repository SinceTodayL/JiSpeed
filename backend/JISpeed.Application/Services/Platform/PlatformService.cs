using System.Runtime.InteropServices.JavaScript;
using JISpeed.Core.Constants;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Interfaces.IRepositories.Common;
using JISpeed.Core.Interfaces.IRepositories.Merchant;
using JISpeed.Core.Interfaces.IRepositories.Order;
using JISpeed.Core.Interfaces.IRepositories.User;
using JISpeed.Core.Interfaces.IServices;

namespace JISpeed.Application.Services.Platform
{
    public class PlatformService: IPlatformService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMerchantRepository _merchantRepository;
        private readonly IApplicationUserRepository _applicationUserRepository;
        public PlatformService(
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            IMerchantRepository merchantRepository,
            IApplicationUserRepository applicationUserRepository
            )
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _merchantRepository = merchantRepository;
            _applicationUserRepository = applicationUserRepository;
        }

        public async Task<OperationalDataDto> GetOperationalDataByTimeRangeAsync(
            DateTime? startTime,
            DateTime? endTime)
        {
            var start =startTime?? DateTime.MinValue;
            var end = endTime?? DateTime.Now;
            if (start > end)
            {
                throw new BusinessException(ErrorCodes.UnsupportedOperation,"Start time must be before end time");
            }
            // 总单数
            var ordrQuantity = (await _orderRepository.GetByTimeRangeAsync(start, end)).Count;
            // 取消单数
            var cancelledOrderQuantity = (await _orderRepository.GetByTimeRangeAndStatusAsync(start, end,(int)OrderStatus.Cancelled)).Count;
            // 售后完成的单数
            var aftersalesCompletedOrderQuantity = (await _orderRepository.GetByTimeRangeAndStatusAsync(start, end,(int)OrderStatus.AftersalesCompleted)).Count;
            // 用户数量
            var userQuantity = (await _applicationUserRepository.GetByUserTypeAndTimeRangeAsync((int)UserType.User,start, end)).Count;
            // 商家数量
            var merchantQuantity = (await _applicationUserRepository.GetByUserTypeAndTimeRangeAsync((int)UserType.Merchant,start, end)).Count;
            // 骑手数量
            var riderQuantity = (await _applicationUserRepository.GetByUserTypeAndTimeRangeAsync((int)UserType.Rider,start, end)).Count;

            var response = new OperationalDataDto()
            {
                OrderQuantity = ordrQuantity,
                CancelledOrderQuantity = cancelledOrderQuantity,
                AftersalesCompletedOrderQuantity = aftersalesCompletedOrderQuantity,
                MerchantQuantity = merchantQuantity,
                RiderQuantity = riderQuantity,
                UserQuantity = userQuantity,
            };
            return response;
        }
    }
}