using JISpeed.Core.Constants;
using JISpeed.Core.Entities.Order;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Interfaces.IRepositories.Admin;
using JISpeed.Core.Interfaces.IRepositories.Common;
using JISpeed.Core.Interfaces.IRepositories.Dish;
using JISpeed.Core.Interfaces.IRepositories.Junctions;
using JISpeed.Core.Interfaces.IRepositories.Merchant;
using JISpeed.Core.Interfaces.IRepositories.Order;
using JISpeed.Core.Interfaces.IRepositories.User;
using JISpeed.Core.Interfaces.IServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrderEntity = JISpeed.Core.Entities.Order.Order;
using OrderDishEntity = JISpeed.Core.Entities.Junctions.OrderDish;

namespace JISpeed.Application.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMerchantRepository _merchantRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IOrderDishRepository _orderDishRepository;
        private readonly IOrderLogRepository _orderLogRepository;
        private readonly IDishRepository _dishRepository;
        private readonly ISalesStatRepository _salesStatRepository;
        private readonly IRefundRepository _refundRepository;
        private readonly ICouponRepository _couponRepository;

        private readonly IComplaintRepository _complaintRepository;
        private readonly IUserService _userService;
        private readonly ILogger<OrderService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public OrderService(
            IOrderRepository orderRepository,
            IAdminRepository adminRepository,
            IUserRepository userRepository,
            IPaymentRepository paymentRepository,
            IMerchantRepository merchantRepository,
            IAddressRepository addressRepository,
            IOrderLogRepository orderLogRepository,
            IOrderDishRepository orderDishRepository,
            IDishRepository dishRepository,
            IRefundRepository refundRepository,
            IComplaintRepository complaintRepository,
            ISalesStatRepository salesStatRepository,
            ICouponRepository couponRepository,
            IUserService userService,
            IServiceProvider serviceProvider,
            ILogger<OrderService> logger
        )
        {
            _orderRepository = orderRepository;
            _adminRepository = adminRepository;
            _userRepository = userRepository;
            _paymentRepository = paymentRepository;
            _merchantRepository = merchantRepository;
            _addressRepository = addressRepository;
            _orderDishRepository = orderDishRepository;
            _orderLogRepository = orderLogRepository;
            _dishRepository = dishRepository;
            _refundRepository = refundRepository;
            _complaintRepository = complaintRepository;
            _salesStatRepository = salesStatRepository;
            _couponRepository = couponRepository;
            _userService = userService;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }


        public async Task<OrderEntity> GetOrderDetailByOrderIdAsync(string orderId)
        {
            try
            {
                _logger.LogInformation("开始获取订单信息, OrderId: {OrderId}", orderId);

                var entity = await _orderRepository.GetOrderWithDishesAndMerchantsAsync(orderId);
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
            string merchantId, List<DishQuantityDto> dishQuantities)
        {
            try
            {
                _logger.LogInformation("开始创建订单实体");

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw new NotFoundException(ErrorCodes.UserNotFound, "用户不存在");
                }
                var merchant = await _merchantRepository.GetByIdAsync(merchantId);
                if (merchant == null)
                {
                    throw new NotFoundException(ErrorCodes.MerchantNotFound, "商家不存在");
                }
                var address = await _addressRepository.GetByIdAsync(addressId);
                if (address == null)
                {
                    throw new NotFoundException(ErrorCodes.UserAddressNotFound, "地址不存在");
                }

                if (couponId != null)
                {
                    var coupon = await _couponRepository.GetByIdAsync(couponId);
                    if (coupon == null)
                        throw new NotFoundException(ErrorCodes.ResourceNotFound, "优惠券不存在");
                    if (coupon.IsUsed)
                        throw new BusinessException(ErrorCodes.GeneralError, "优惠券已经被使用");
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
                    OrderLogs = new List<OrderLog> { }
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
                    // 减去dish中对应的数量
                    dish.StockQuantity -= dishQuantity.Quantity;
                    await _dishRepository.SaveChangesAsync();
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

                // 安排订单自动取消任务
                var autoOrderService = _serviceProvider.GetService(typeof(IAutoOrderService)) as IAutoOrderService;
                if (autoOrderService != null)
                {
                    autoOrderService.ScheduleOrderCancellation(orderEntity.OrderId, orderEntity.CreateAt);
                    _logger.LogInformation("已为订单 {OrderId} 安排15分钟自动取消任务", orderEntity.OrderId);
                }
                else
                {
                    _logger.LogWarning("未能获取AutoOrderService服务实例，订单 {OrderId} 将不会自动取消", orderEntity.OrderId);
                }

                // 清理购物车中对应的商品
                try
                {
                    var clearedItemsCount = await _userService.ReduceCartItemsByDishQuantitiesAsync(userId, dishQuantities);
                    if (clearedItemsCount > 0)
                    {
                        _logger.LogInformation("成功清理购物车商品，OrderId: {OrderId}, UserId: {UserId}, 清理数量: {ClearedCount}",
                            orderEntity.OrderId, userId, clearedItemsCount);
                    }
                }
                catch (Exception ex)
                {
                    // 购物车清理失败不应该影响订单创建，只记录日志
                    _logger.LogWarning(ex, "订单创建成功但清理购物车失败，OrderId: {OrderId}, UserId: {UserId}",
                        orderEntity.OrderId, userId);
                }

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
            string userId, int? orderStatus,
            int? size, int? page)
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
                    data = await _orderRepository.GetByUserIdAndStatusAsync(userId, orderStatus.Value, size, page);
                else
                    data = await _orderRepository.GetByUserIdAsync(userId, size, page);
                _logger.LogInformation("成功用户获取订单列表信息, UserId: {UserId}", userId);

                return data.Select(x => x.OrderId).ToList();
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

                string remark;
                int statusCode;
                if (orderStatus == (int)OrderStatus.Confirmed)
                {
                    remark = "用户确认收货";
                    statusCode = (int)OrderLogStatus.Delivered;
                }
                else
                {
                    remark = "用户取消订单";
                    statusCode = (int)OrderLogStatus.Cancelled;
                    // 将库存返还
                    foreach (var dish in entity.OrderDishes)
                    {
                        dish.Dish.StockQuantity += dish.Quantity;
                    }
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
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException || ex is BusinessException))
            {
                _logger.LogError(ex, "更新订单信息时发生异常, OrderId: {OrderId}", orderId);
                throw new BusinessException("更新订单信息失败");
            }
        }

        public async Task<bool> UpdatePaymentAsync(string payId, int payStatus, int? amount)
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
                    entity.PayTime = DateTime.Now;
                }
                else
                    entity.PayStatus = payStatus;

                entity.PayAmount = amount ?? entity.PayAmount;
                await _paymentRepository.SaveChangesAsync();
                await _orderRepository.SaveChangesAsync();
                var remark = (payStatus == (int)PayStatus.Paid) ? "用户支付订单" : "用户取消支付";
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
                if (payStatus == (int)PayStatus.Cancelled && entity.Order.Coupon != null)
                {
                    entity.Order.Coupon.IsUsed = false;
                    await _couponRepository.SaveChangesAsync();
                }
                _logger.LogInformation("更新支付信息成功，PayId: {PayId}", payId);

                var salesStat =
                    await _salesStatRepository.GetByIdAsync(DateTime.Now.Date, entity.Order.MerchantId);
                if (salesStat == null)
                {
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, "无法找到该商家今日销售清单实体");
                }
                salesStat.SalesQty += 1;
                salesStat.SalesAmount += entity.PayAmount;
                await _salesStatRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException || ex is BusinessException))
            {
                _logger.LogError(ex, "更新支付信息时发生异常，PayId: {PayId}", payId);
                throw new BusinessException("更新支付信息失败");
            }
        }

        public async Task<Payment> CreatePaymentByOrderIdAsync(string orderId, string channel)
        {
            try
            {
                _logger.LogInformation("开始创建用户支付订单实体");

                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning("无相关数据,OrderId: {OrderId}", orderId);
                    throw new NotFoundException(ErrorCodes.OrderNotFound, $"无相关数据, OrderId: {orderId}");
                }

                decimal faceValue = 0;
                if (order.Coupon != null)
                {
                    faceValue = order.Coupon.FaceValue;
                    order.Coupon.IsUsed = true;
                }
                var data = new Payment()
                {
                    PayId = Guid.NewGuid().ToString("N"),
                    Channel = channel,
                    OrderId = orderId,
                    PayStatus = (int)PayStatus.Unpaid,
                    Order = order,
                    PayAmount = order.OrderAmount - faceValue,
                };
                await _paymentRepository.CreateAsync(data);
                await _paymentRepository.SaveChangesAsync();
                await _couponRepository.SaveChangesAsync();
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
                    data = await _orderRepository.GetByMerchantIdAndStatusAsync(merchantId, orderStatus.Value, size, page);
                else if (startDate.HasValue || endDate.HasValue)
                {
                    var start = startDate.HasValue ? startDate.Value : DateTime.MinValue;
                    var end = endDate.HasValue ? endDate.Value : DateTime.Now;
                    data = await _orderRepository.GetByMerchantIdAndTimeRangeAsync(merchantId, start,
                        end, size, page);
                }
                else
                    data = await _orderRepository.GetByMerchantIdAsync(merchantId, size, page);

                return data.Select(x => x.OrderId).ToList();
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取商家订单列表信息时发生异常");
                throw new BusinessException("获取商家订单列表信息失败");
            }
        }

        public async Task<OrderLog> GetOrderLogDetailByLogIdAsync(string logId)
        {
            try
            {
                _logger.LogInformation("开始获取订单日志信息");

                var entity = await _orderLogRepository.GetByIdAsync(logId);
                if (entity == null)
                {
                    _logger.LogWarning("无相关数据");
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"无相关数据，ID: {logId}");
                }
                _logger.LogInformation("成功获取订单日志信息");

                return entity;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取订单日志信息时发生异常");
                throw new BusinessException("获取订单日志信息失败");
            }
        }

        public async Task<Payment> GetPaymentDetailByPayIdAsync(string payId)
        {
            try
            {
                _logger.LogInformation("开始获取支付实体信息");

                var entity = await _paymentRepository.GetByIdAsync(payId);
                if (entity == null)
                {
                    _logger.LogWarning("无相关数据");
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"无相关数据，ID: {payId}");
                }
                _logger.LogInformation("成功获取支付实体信息");

                return entity;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取支付实体信息时发生异常");
                throw new BusinessException("获取支付实体信息失败");
            }
        }

        public async Task<string> CreateRefundByOrderIdAndUserIdAsync(
            string userId,
            string orderId,
            string reason,
            decimal amount)
        {
            try
            {
                _logger.LogInformation("开始创建用户退款实体");

                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning("无相关数据,OrderId: {OrderId}", orderId);
                    throw new NotFoundException(ErrorCodes.OrderNotFound, $"无相关数据, OrderId: {orderId}");
                }
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw new NotFoundException(ErrorCodes.UserNotFound, $"无相关数据, UserID: {userId}");
                }

                var paidPayment = await _paymentRepository.GetByOrderIdAndStatusAsync(orderId, (int)PayStatus.Paid);
                if (paidPayment == null)
                {
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, "无相关支付实体");
                }
                var refund = new Refund()
                {
                    RefundId = Guid.NewGuid().ToString("N"),
                    OrderId = orderId,
                    Reason = reason,
                    ApplicationId = userId,
                    ApplyAt = DateTime.Now,
                    Order = order,
                    Applicant = user,
                    RefundAmount = amount,
                    AuditStatus = (int)RefundStatus.Default
                };
                await _refundRepository.CreateAsync(refund);
                await _refundRepository.SaveChangesAsync();
                order.OrderStatus = (int)OrderStatus.Aftersales;
                await _orderRepository.SaveChangesAsync();
                _logger.LogInformation("成功创建退款实体，RefundId: {RefundId}", refund.RefundId);

                var orderLog = new OrderLog
                {
                    Actor = "user",
                    LoggedAt = DateTime.Now,
                    OrderId = refund.OrderId,
                    LogId = Guid.NewGuid().ToString("N"),
                    Remark = "用户请求退款",
                    StatusCode = (int)OrderLogStatus.Aftersales,
                    Order = refund.Order
                };
                await _orderLogRepository.CreateAsync(orderLog);
                await _orderLogRepository.SaveChangesAsync();
                _logger.LogInformation("成功更新退款实体，RefundId: {RefundId}", refund.RefundId);

                return refund.RefundId;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "创建订单退款时发生异常,OrderId: {OrderId}", orderId);
                throw new BusinessException("创建订单退款实体失败");
            }
        }

        public async Task<string> UpdateRefundForMerchantAsync(string merchantId, string refundId, int refundStatus)
        {
            try
            {
                _logger.LogInformation("开始更新退款实体");

                var merchant = await _merchantRepository.ExistsAsync(merchantId);
                if (!merchant)
                {
                    _logger.LogWarning("无相关数据,Merchant: {merchant}", merchant);
                    throw new NotFoundException(ErrorCodes.MerchantNotFound, $"无相关数据, merchant: {merchant}");
                }
                var refund = await _refundRepository.GetByIdAsync(refundId);
                if (refund == null)
                {
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"无相关数据, refundId: {refundId}");
                }

                if (refund.AuditStatus != (int)RefundStatus.Default)
                {
                    _logger.LogInformation("该申请已经被处理过！");
                    throw new BusinessException(ErrorCodes.GeneralError, "该申请已经被处理过！");
                }
                refund.AuditStatus = refundStatus;
                refund.FinishAt = DateTime.Now;
                await _refundRepository.SaveChangesAsync();
                refund.Order.OrderStatus = (int)OrderStatus.AftersalesCompleted;
                await _orderRepository.SaveChangesAsync();

                var remark = (refundStatus == (int)RefundStatus.Refunded) ? "商家同意退款" : "商家拒绝退款";

                var orderLog = new OrderLog
                {
                    Actor = "merchant",
                    LoggedAt = DateTime.Now,
                    OrderId = refund.OrderId,
                    LogId = Guid.NewGuid().ToString("N"),
                    Remark = remark,
                    StatusCode = (int)OrderLogStatus.AftersalesCompleted,
                    Order = refund.Order
                };
                await _orderLogRepository.CreateAsync(orderLog);
                await _orderLogRepository.SaveChangesAsync();
                _logger.LogInformation("成功更新退款实体，RefundId: {RefundId}", refund.RefundId);

                return orderLog.LogId;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "更新订单退款时发生异常");
                throw new BusinessException("更新订单退款实体失败");
            }
        }
        public async Task<string> UpdateRefundForAdminAsync(string adminId, string refundId, int refundStatus)
        {
            try
            {
                _logger.LogInformation("开始更新退款实体");

                var admin = await _adminRepository.ExistsAsync(adminId);
                if (!admin)
                {
                    _logger.LogWarning("无相关数据,Admin: {admin}", admin);
                    throw new NotFoundException(ErrorCodes.GeneralError, $"无相关数据, admin: {admin}");
                }
                var refund = await _refundRepository.GetByIdAsync(refundId);
                if (refund == null)
                {
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"无相关数据, refundId: {refundId}");
                }

                if (refund.AuditStatus != (int)RefundStatus.DefaultForAdmin && refund.AuditStatus != (int)RefundStatus.Default)
                {
                    _logger.LogInformation("该申请已经被处理过！");
                    throw new BusinessException(ErrorCodes.GeneralError, "该申请已经被处理过！");

                }
                refund.AuditStatus = refundStatus;
                refund.FinishAt = DateTime.Now;
                await _refundRepository.SaveChangesAsync();
                refund.Order.OrderStatus = (int)OrderStatus.AftersalesCompleted;
                await _orderRepository.SaveChangesAsync();
                _logger.LogInformation("成功更新退款实体，RefundId: {RefundId}", refund.RefundId);

                var remark = (refundStatus == (int)RefundStatus.Refunded) ? "管理员同意退款" : "管理员拒绝退款";

                var orderLog = new OrderLog
                {
                    Actor = "admin",
                    LoggedAt = DateTime.Now,
                    OrderId = refund.OrderId,
                    LogId = Guid.NewGuid().ToString("N"),
                    Remark = remark,
                    StatusCode = (int)OrderLogStatus.AftersalesCompleted,
                    Order = refund.Order
                };
                await _orderLogRepository.CreateAsync(orderLog);
                await _orderLogRepository.SaveChangesAsync();

                return orderLog.LogId;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "更新订单退款时发生异常");
                throw new BusinessException("更新订单退款实体失败");
            }
        }

        public async Task<Refund> GetRefundDetailByRefundIdAsync(string refundId)
        {
            try
            {
                _logger.LogInformation("开始获取退款实体信息");

                var entity = await _refundRepository.GetByIdAsync(refundId);
                if (entity == null)
                {
                    _logger.LogWarning("无相关数据");
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"无相关数据，ID: {refundId}");
                }
                _logger.LogInformation("成功获取退款实体信息");

                return entity;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取退款实体信息时发生异常");
                throw new BusinessException("获取退款实体信息失败");
            }
        }

        public async Task<List<string>> GetRefundListByFilterAsync(
            string? userId,
            string? merchantId,
            string? adminId,
            int? auditStatus,
            int? size, int? page)
        {
            try
            {
                _logger.LogInformation("开始获取退款列表信息");

                List<Refund>? refundList;
                if (userId != null)
                    refundList = await _refundRepository.GetByUserIdAndStatusAsync(userId, auditStatus, size, page);
                else if (merchantId != null)
                    refundList = await _refundRepository.GetByMerchantIdAndStatusAsync(merchantId, auditStatus, size, page);
                else if (adminId != null)
                {
                    var admin = await _adminRepository.ExistsAsync(adminId);
                    if (!admin)
                        throw new NotFoundException(ErrorCodes.ResourceNotFound, "管理员不存在");
                    refundList = await _refundRepository.GetAllByStatusForAdminAsync(auditStatus, size, page);
                }
                else
                    refundList = await _refundRepository.GetAllAsync(size, page);

                if (refundList == null)
                {
                    _logger.LogWarning("无相关数据");
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, "无相关数据");
                }
                _logger.LogInformation("成功获取退款实体信息");

                return refundList.Select(r => r.RefundId).ToList();
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取退款实体列表时发生异常");
                throw new BusinessException("获取退款实体列表失败");
            }
        }

        public async Task<Complaint> GetComplaintDetailByComplainantIdAsync(string complaintId)
        {
            try
            {
                _logger.LogInformation("开始获取投诉实体信息");

                var entity = await _complaintRepository.GetByIdAsync(complaintId);
                if (entity == null)
                {
                    _logger.LogWarning("无相关数据");
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"无相关数据，ID: {complaintId}");
                }
                _logger.LogInformation("成功获取投诉实体信息");

                return entity;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取投诉实体信息时发生异常");
                throw new BusinessException("获取投诉实体信息失败");
            }
        }

        public async Task<string> CreateComplaintDetailAsync(
            string orderId, string userId,
            int cmplRole, string? cmplDescription)
        {
            try
            {

                _logger.LogInformation("开始创建投诉实体信息");

                // 参数检查
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("无相关数据,userId: {userId}", userId);
                    throw new NotFoundException(ErrorCodes.UserNotFound, $"无相关数据, userId: {userId}");
                }
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning("无相关数据,OrderId: {OrderId}", orderId);
                    throw new NotFoundException(ErrorCodes.OrderNotFound, $"无相关数据, OrderId: {orderId}");
                }

                var complaint = new Complaint
                {
                    OrderId = orderId,
                    ComplaintId = Guid.NewGuid().ToString("N"),
                    CmplRole = cmplRole,
                    CmplDescription = cmplDescription,
                    CmplStatus = (int)ComplaintStatus.Default, //默认状态为待处理
                    CreatedAt = DateTime.Now, //设置创建时间为当前时间
                    ComplainantId = userId,
                    Order = order,
                    Complainant = user
                };



                var orderLog = new OrderLog
                {
                    Actor = "user",
                    LoggedAt = DateTime.Now,
                    OrderId = orderId,
                    LogId = Guid.NewGuid().ToString("N"),
                    Remark = "用户投诉订单",
                    StatusCode = (int)OrderLogStatus.Aftersales,
                    Order = order
                };
                await _complaintRepository.CreateAsync(complaint);
                await _complaintRepository.SaveChangesAsync();
                await _orderLogRepository.CreateAsync(orderLog);
                await _orderLogRepository.SaveChangesAsync();

                _logger.LogInformation("成功创建投诉实体信息");

                return complaint.ComplaintId;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取投诉实体信息时发生异常");
                throw new BusinessException("获取投诉实体信息失败");
            }
        }

        public async Task<bool> AuditComplaintAsync(string adminId, string complaintId)
        {
            try
            {

                _logger.LogInformation("开始更新投诉实体信息");

                // 参数检查
                var admin = await _adminRepository.ExistsAsync(adminId);
                if (!admin)
                {
                    _logger.LogWarning("无相关数据,adminId: {adminId}", adminId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"无相关数据, adminId: {adminId}");
                }
                var complaint = await _complaintRepository.GetByIdAsync(complaintId);
                if (complaint == null)
                {
                    _logger.LogWarning("无相关数据,complaintId: {complaintId}", complaintId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"无相关数据, complaintId: {complaintId}");
                }

                complaint.CmplStatus = (int)ComplaintStatus.Resolved;
                await _complaintRepository.SaveChangesAsync();
                _logger.LogInformation("成功更新投诉实体信息");
                var orderLog = new OrderLog
                {
                    Actor = "admin",
                    LoggedAt = DateTime.Now,
                    OrderId = complaint.OrderId,
                    LogId = Guid.NewGuid().ToString("N"),
                    Remark = "管理员审核投诉",
                    StatusCode = (int)OrderLogStatus.AftersalesCompleted,
                    Order = complaint.Order
                };
                complaint.Order.OrderStatus = (int)OrderStatus.AftersalesCompleted;
                await _orderRepository.SaveChangesAsync();
                await _orderLogRepository.CreateAsync(orderLog);
                await _orderLogRepository.SaveChangesAsync();

                return true;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "更新投诉实体信息时发生异常");
                throw new BusinessException("更新投诉实体信息失败");
            }
        }

        public async Task<bool> CancelComplaintAsync(string userId, string complaintId)
        {
            try
            {

                _logger.LogInformation("开始关闭投诉实体信息");

                // 参数检查
                var user = await _userRepository.ExistsAsync(userId);
                if (!user)
                {
                    _logger.LogWarning("无相关数据,userId: {userId}", userId);
                    throw new NotFoundException(ErrorCodes.UserNotFound, $"无相关数据, userId: {userId}");
                }
                var complaint = await _complaintRepository.GetByIdAsync(complaintId);
                if (complaint == null)
                {
                    _logger.LogWarning("无相关数据,complaintId: {complaintId}", complaintId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"无相关数据, complaintId: {complaintId}");
                }

                complaint.CmplStatus = (int)ComplaintStatus.Cancelled;
                await _complaintRepository.SaveChangesAsync();
                _logger.LogInformation("成功关闭投诉实体信息");
                var orderLog = new OrderLog
                {
                    Actor = "user",
                    LoggedAt = DateTime.Now,
                    OrderId = complaint.OrderId,
                    LogId = Guid.NewGuid().ToString("N"),
                    Remark = "用户撤销投诉",
                    StatusCode = (int)OrderLogStatus.AftersalesCompleted,
                    Order = complaint.Order
                };
                complaint.Order.OrderStatus = (int)OrderStatus.AftersalesCompleted;
                await _orderRepository.SaveChangesAsync();
                await _orderLogRepository.CreateAsync(orderLog);
                await _orderLogRepository.SaveChangesAsync();

                return true;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "关闭投诉实体信息时发生异常");
                throw new BusinessException("更新投诉实体信息失败");
            }
        }

        public async Task<List<string>> GetComplaintListByFilterAsync(
            string? userId,
            string? merchantId,
            int? status,
            string? adminId,
            int? size, int? page)
        {
            try
            {
                _logger.LogInformation("开始获取投诉列表信息");
                List<Complaint>? complaints;
                if (merchantId != null)
                {
                    var merchant = await _merchantRepository.ExistsAsync(merchantId);
                    if (!merchant)
                    {
                        throw new NotFoundException(ErrorCodes.MerchantNotFound, "商家不存在");
                    }
                    complaints = await _complaintRepository.GetByMerchantIdAndStatusAsync(merchantId, status, size, page);
                }
                else if (userId != null)
                {
                    var user = await _userRepository.ExistsAsync(userId);
                    if (!user)
                    {
                        throw new NotFoundException(ErrorCodes.UserNotFound, "用户不存在");
                    }
                    complaints = await _complaintRepository.GetByUserIdAndStatusAsync(userId, status, size, page);
                }
                else if (adminId != null)
                {
                    var admin = await _adminRepository.ExistsAsync(adminId);
                    if (!admin)
                    {
                        _logger.LogWarning("无相关数据,adminId: {adminId}", adminId);
                        throw new NotFoundException(ErrorCodes.ResourceNotFound, $"无相关数据, adminId: {adminId}");
                    }
                    complaints = await _complaintRepository.GetAllByFilterAsync(status, size, page);
                }
                else
                {
                    complaints = await _complaintRepository.GetAllAsync(size, page);
                }

                if (complaints == null)
                {
                    _logger.LogWarning("无相关数据");
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, "无相关数据");
                }
                _logger.LogInformation("成功获取投诉实体信息");

                return complaints.Select(c => c.ComplaintId).ToList();
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取投诉实体列表时发生异常");
                throw new BusinessException("获取投诉实体列表失败");
            }
        }



    }
}