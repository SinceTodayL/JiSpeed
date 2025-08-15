using JISpeed.Core.Constants;
using JISpeed.Core.Entities.Order;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Interfaces.IRepositories.Dish;
using JISpeed.Core.Interfaces.IRepositories.Junctions;
using JISpeed.Core.Interfaces.IRepositories.Merchant;
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
        private readonly IMerchantRepository _merchantRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IOrderDishRepository _orderDishRepository;
        private readonly IOrderLogRepository _orderLogRepository;
        private readonly IDishRepository _dishRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            IPaymentRepository paymentRepository,
            IMerchantRepository merchantRepository,
            IAddressRepository addressRepository,
            IOrderLogRepository orderLogRepository,
            IOrderDishRepository orderDishRepository,
            IDishRepository dishRepository,
            ILogger<OrderService> logger
        )
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _paymentRepository = paymentRepository;
            _merchantRepository = merchantRepository;
            _addressRepository = addressRepository;
            _orderDishRepository = orderDishRepository;
            _orderLogRepository = orderLogRepository;
            _dishRepository = dishRepository;
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

        public async Task<string> CreateOrderByUserIdAsync(
            string userId, decimal orderAmount, 
            string? couponId, string addressId,
            string merchantId,List<DishQuantityDto> dishQuantities)
        {
            try
            {
                _logger.LogInformation("开始创建订单实体");

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw new NotFoundException(ErrorCodes.UserNotFound,"用户不存在");
                }
                var merchant = await _merchantRepository.GetByIdAsync(merchantId);
                if (merchant == null)
                {
                    throw new NotFoundException(ErrorCodes.MerchantNotFound,"商家不存在");
                }
                var address = await _addressRepository.GetByIdAsync(addressId);
                if (address == null)
                {
                    throw new NotFoundException(ErrorCodes.UserAddressNotFound,"地址不存在");
                }

                var orderEntity = new OrderEntity()
                {
                    OrderId = Guid.NewGuid().ToString("N"),
                    UserId = userId,
                    OrderAmount = orderAmount,
                    CouponId = couponId,
                    AddressId = addressId,
                    CreateAt = DateTime.Now,
                    OrderStatus = (int)OrderStatus.Unpaid,
                    MerchantId = merchantId,
                    Merchant = merchant,
                    User = user,
                    Address = address,
                    OrderLogs = new List<OrderLog> {  } 
                };
                await _orderRepository.CreateAsync(orderEntity);
                await _orderRepository.SaveChangesAsync();
                foreach (var dishQuantity in dishQuantities)
                {
                    var dish = await _dishRepository.GetByIdAsync(dishQuantity.DishId);
                    if (dish == null)
                    {
                        _logger.LogInformation($"ID:{dishQuantity}");
                        throw new NotFoundException(ErrorCodes.ResourceNotFound, "菜品不存在");
                    }
                    var orderdish = new OrderDishEntity
                    {
                        Order = orderEntity,
                        OrderId = orderEntity.OrderId,
                        DishId = dishQuantity.DishId,
                        Dish = dish,
                        Quantity = dishQuantity.Quantity,
                    };
                    await _orderDishRepository.CreateAsync(orderdish);
                }
                await _orderDishRepository.SaveChangesAsync();
                var orderLog = new OrderLog
                {
                    Actor = "user",
                    LoggedAt = DateTime.Now,
                    OrderId = orderEntity.OrderId,
                    LogId = Guid.NewGuid().ToString("N"),
                    Remark = "用户创建订单",
                    StatusCode = (int)OrderLogStatus.Created,
                    Order = orderEntity
                };
                
                await _orderLogRepository.CreateAsync(orderLog);
                await _orderLogRepository.SaveChangesAsync();
             
                
                _logger.LogInformation("成功创建订单信息, OrderId: {OrderId}", orderEntity.OrderId);

                return orderLog.LogId;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "创建订单信息时发生异常");
                throw new BusinessException("创建订单失败");
            }
        }

        public async Task<List<string>> GetOrderIdByUserIdAsync(
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

                List<OrderEntity> data;
                if (orderStatus.HasValue)
                    data= await _orderRepository.GetByUserIdAndStatusAsync(userId,orderStatus.Value,size,page);
                else
                    data= await _orderRepository.GetByUserIdAsync(userId,size,page);
                _logger.LogInformation("成功用户获取订单列表信息, UserId: {UserId}", userId);

                return data.Select(x => x.OrderId).ToList()??new List<string>();
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
                var remark = "";
                var statusCode =0;
                if (orderStatus == (int)OrderStatus.Confirmed)
                {
                    remark = "用户确认收货";
                    statusCode = (int)OrderLogStatus.Delivered;
                }
                else
                {
                    remark = "用户取消订单";
                    statusCode = (int)OrderLogStatus.Cancelled;
                }
               
                var orderLog = new OrderLog
                {
                    Actor = "user",
                    LoggedAt = DateTime.Now,
                    OrderId = entity.OrderId,
                    LogId = Guid.NewGuid().ToString("N"),
                    Remark = remark,
                    StatusCode = statusCode,
                    Order = entity
                };
                await _orderLogRepository.CreateAsync(orderLog);
                await _orderLogRepository.SaveChangesAsync();
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
                var remark =(payStatus==(int)PayStatus.Paid)?"用户支付订单":"用户取消支付";
                var orderLog = new OrderLog
                {
                    Actor = "user",
                    LoggedAt = DateTime.Now,
                    OrderId = entity.Order.OrderId,
                    LogId = Guid.NewGuid().ToString("N"),
                    Remark = remark,
                    StatusCode = (int)OrderLogStatus.Paid,
                    Order = entity.Order
                };
                await _orderLogRepository.CreateAsync(orderLog);
                await _orderLogRepository.SaveChangesAsync();
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

        public async Task<List<string>> GetOrderIdByMerchantIdAsync(
            string merchantId, int? orderStatus,
            DateTime? startDate, DateTime? endDate,
            int? size, int? page)
        {
            try
            {
                _logger.LogInformation("开始获取商家订单列表信息");

                var res = await _merchantRepository.ExistsAsync(merchantId);
                if (!res)
                {
                    _logger.LogWarning("无相关数据");
                    throw new NotFoundException(ErrorCodes.MerchantNotFound, $"无相关数据, merchantId: {merchantId}");
                }

                List<OrderEntity> data;
                if (orderStatus.HasValue)
                    data= await _orderRepository.GetByMerchantIdAndStatusAsync(merchantId,orderStatus.Value,size,page);
                else if (startDate.HasValue || endDate.HasValue)
                {
                    var start = startDate.HasValue ? startDate.Value : DateTime.MinValue;
                    var end = endDate.HasValue ? endDate.Value : DateTime.Now;
                    data = await _orderRepository.GetByMerchantIdAndTimeRangeAsync(merchantId, start,
                        end, size, page);
                }
                else
                    data = await _orderRepository.GetByMerchantIdAsync(merchantId, size, page);
                return data.Select(x => x.OrderId).ToList()??new List<string>();
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取商家订单列表信息时发生异常");
                throw new BusinessException("获取商家订单列表信息失败");
            }
        }



    }
}