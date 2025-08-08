using JISpeed.Core.Constants;
using JISpeed.Core.Entities.Order;
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
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            IPaymentRepository paymentRepository,
            ILogger<OrderService> logger
        )
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _paymentRepository = paymentRepository;
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

        public async Task<bool> UpdateOrderAsync(string orderId, int? orderStatus)
        {
            try
            {
                _logger.LogInformation("开始更新订单信息");
        
                var entity = await _orderRepository.GetByIdAsync(orderId);
                if (entity == null)
                {
                    _logger.LogWarning("无相关数据, OrderId: {OrderId}", orderId);
                    throw new NotFoundException(ErrorCodes.UserNotFound, $"无相关数据, OrderId: {orderId}");
                }

                if (orderStatus.HasValue && orderStatus.Value == entity.OrderStatus)
                {
                    throw new BusinessException("无法重复操作！");
                }
        
                entity.OrderStatus = orderStatus ?? entity.OrderStatus;
                await _orderRepository.SaveChangesAsync();
                _logger.LogInformation("更新订单信息成功, OrderId: {OrderId}", orderId);
                return true;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException ||ex is BusinessException))
            {
                _logger.LogError(ex, "更新订单信息时发生异常, OrderId: {OrderId}", orderId);
                throw new BusinessException("更新订单信息失败");
            }
        }

        public async Task<bool> UpdatePaymentAsync(string payId, int payStatus,int? amount)
        {
            try
            {
                _logger.LogInformation("开始更新支付信息");
        
                var entity = await _paymentRepository.GetWithDetailsAsync(payId);
                if (entity == null)
                {
                    _logger.LogWarning("无相关数据，PayId: {PayId}", payId);
                    throw new NotFoundException(ErrorCodes.UserNotFound, $"无相关数据, PayId: {payId}");
                }
                // 只有order是未支付状态，才能支付
                if (entity.Order.OrderStatus != (int)PayStatus.Unpaid &&
                    payStatus == (int)PayStatus.Paid)
                {
                    throw new BusinessException(ErrorCodes.DuplicatePayment, "重复支付！");
                }
                if (payStatus == (int)PayStatus.Paid)
                {
                    entity.PayStatus = payStatus;
                    entity.Order.OrderStatus = (int)OrderStatus.Paid;
                    entity.PayTime=DateTime.Now;
                }
                else
                    entity.PayStatus = payStatus;
                entity.PayAmount = amount??entity.PayAmount;
                await _paymentRepository.SaveChangesAsync();
                await _orderRepository.SaveChangesAsync();
                _logger.LogInformation("更新支付信息成功，PayId: {PayId}", payId);
        
                return true;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException||ex is BusinessException))
            {
                _logger.LogError(ex, "更新支付信息时发生异常，PayId: {PayId}", payId);
                throw new BusinessException("更新支付信息失败");
            }
        }

        public async Task<Payment> CreatePaymentByorderIdAsync(string orderId, string channel)
        {
            try
            {
                _logger.LogInformation("开始创建用户支付订单实体");

                var entity = await _orderRepository.GetByIdAsync(orderId);
                if (entity==null)
                {
                    _logger.LogWarning("无相关数据,OrderId: {OrderId}", orderId);
                    throw new NotFoundException(ErrorCodes.OrderNotFound, $"无相关数据, OrderId: {orderId}");
                }

                var data = new Payment()
                {
                    PayId = Guid.NewGuid().ToString("N"),
                    Channel = channel,
                    OrderId = orderId,
                    PayStatus = (int)PayStatus.Unpaid,
                    Order = entity,
                    PayAmount = entity.OrderAmount
                };
                var res = await _paymentRepository.CreateAsync(data);
                await _paymentRepository.SaveChangesAsync();
                _logger.LogInformation("成功创建订单实体，PayId: {PayId}", data.PayId);

                return data;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "创建订单实体时发生异常,OrderId: {OrderId}", orderId);
                throw new BusinessException("创建订单实体失败");
            }
        }



    }
}