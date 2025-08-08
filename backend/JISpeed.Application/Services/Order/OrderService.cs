using JISpeed.Core.Constants;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Interfaces.IRepositories.Junctions;
using JISpeed.Core.Interfaces.IRepositories.Order;
using JISpeed.Core.Interfaces.IRepositories.User;
using JISpeed.Core.Interfaces.IServices;
using Microsoft.Extensions.Logging;
using OrderEntity = JISpeed.Core.Entities.Order.Order;
using OrderDishEntity = JISpeed.Core.Entities.Junctions.OrderDish;

namespace JISpeed.Application.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;

        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            ILogger<OrderService> logger
        )
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _logger = logger;
        }
        
        
        public async Task<OrderEntity> GetOrderDetailByOrderIdAsync(string orderId)
        {
            try
            {
                _logger.LogInformation("开始获取订单信息, OrderId: {OrderId}", orderId);

                var entity= await _orderRepository.GetOrderWithDishesAndMerchantsAsync(orderId);
                if (entity == null)
                {
                    _logger.LogWarning("无相关数据, OrderId: {OrderId}", orderId);
                    throw new NotFoundException(ErrorCodes.OrderNotFound, $"无相关数据，ID: {orderId}");
                }
                _logger.LogInformation("成功获取订单信息, OrderId: {OrderId}", orderId);

                return entity;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取订单信息时发生异常, OrderId: {OrderId}", orderId);
                throw new BusinessException("获取订单信息失败");
            }
        }

        public async Task<List<OrderEntity>> GetOrderIdByUserIdAsync(
            string userId,int? orderStatus,
            int?size,int? page)
        {
            try
            {
                _logger.LogInformation("开始获取用户订单列表信息");

                var res = await _userRepository.ExistsAsync(userId);
                if (!res)
                {
                    _logger.LogWarning("无相关数据, UserId: {UserId}", userId);
                    throw new NotFoundException(ErrorCodes.UserNotFound, $"无相关数据, UserId: {userId}");
                }

                List<OrderEntity> ?data;
                if (orderStatus.HasValue)
                    data= await _orderRepository.GetByUserIdAndStatusAsync(userId,orderStatus.Value,size,page);
                else
                    data= await _orderRepository.GetByUserIdAsync(userId,size,page);
                if (data == null)
                {
                    _logger.LogWarning("无相关数据, UserId: {UserId}", userId);
                    throw new NotFoundException(ErrorCodes.OrderNotFound, $"无相关数据，ID: {userId}");
                }
                _logger.LogInformation("成功用户获取订单列表信息, UserId: {UserId}", userId);

                return data;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取用户订单列表信息时发生异常, UserId: {UserId}", userId);
                throw new BusinessException("获取用户订单列表信息失败");
            }
        }

    }
}