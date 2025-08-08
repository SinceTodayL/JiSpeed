using AutoMapper;
using JISpeed.Api.Common;
using JISpeed.Api.DTOs;
using JISpeed.Core.Constants;
using JISpeed.Core.Entities.Junctions;
using JISpeed.Core.Entities.Order;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace JISpeed.Api.Controllers
{
    [ApiController]
    [Route("api/")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public OrderController(
            ILogger<OrderController> logger,
            IOrderService orderService,
            IUserService userService,
            IMapper mapper
        )
        {
            _logger = logger;
            _orderService = orderService;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet("orders/{orderId}")]
        public async Task<ActionResult<ApiResponse<OrderDetailDto>>> GetOrderByOrderId(string orderId)
        {
            try
            {
                _logger.LogInformation("收到查看订单详情的请求, OrderId: {OrderId}", orderId);
                var order = await _orderService.GetOrderDetailByOrderIdAsync(orderId);

                var merchantGroups = order.OrderDishes
                    .GroupBy(od => od.Dish.MerchantId) // 按商家ID分组
                    .ToDictionary<IGrouping<string, OrderDish>, string, MerchantWithDishesDto>(
                        group => group.Key, // 键：MerchantId
                        group => new MerchantWithDishesDto // 值：商家信息+菜品列表
                        {
                            MerchantId = group.Key,
                            MerchantName = group.First().Dish.Merchant.MerchantName, // 商家名称（同组内一致）
                            Dishes = group.Select(od => new DishItemDto // 转换菜品信息
                            {
                                DishId = od.Dish.DishId,
                                DishName = od.Dish.DishName,
                                Quantity = od.Quantity, // 数量来自 OrderDish
                                Price = od.Dish.Price,
                                UnitPrice = od.Dish.Price, // 单价来自 OrderDish,后续实现cuponId折扣？？TODO
                                CoverUrl = od.Dish.CoverUrl ?? ""
                            }).ToList()
                        }
                    );
                var response = new OrderDetailDto
                {
                    OrderId = order.OrderId,
                    OrderAmount = order.OrderAmount,
                    CreateAt = order.CreateAt,
                    OrderStatus = order.OrderStatus,
                    UserId = order.UserId,
                    AddressId = order.AddressId,
                    CouponId = order.CouponId,
                    ReconId = order.ReconId,
                    AssignId = order.AssignId,
                    MerchantDishes = merchantGroups
                };
                _logger.LogInformation("成功获取用户详细信息, OrderId: {OrderId}", orderId);
                return Ok(ApiResponse<OrderDetailDto>.Success(response, "商家菜品信息获取成功"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败, OrderId: {OrderId}", orderId);
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "商家不存在, OrderId: {OrderId}", orderId);
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.MerchantNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异常, OrderId: {OrderId}", orderId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取商家数据统计信息时发生未知异常, OrderId: {OrderId}", orderId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }

        }

        // 获取用户的历史order
        [HttpGet("users/{userId}/orders")]
        public async Task<ActionResult<ApiResponse<List<OrderDto>>>> GetOrderListByUserId(
            string userId,
            [FromQuery] int? orderStatus,
            [FromQuery] int? size, [FromQuery] int? page)
        {
            try
            {
                _logger.LogInformation("收到获取用户订单列表的请求, UserId: {UserId}", userId);
                var data = await _orderService.GetOrderIdByUserIdAsync(userId, orderStatus, size, page);
                _logger.LogInformation("成功获取用户订单列表, UserId: {UserId}", userId);
                var response = _mapper.Map<List<OrderDto>>(data);
                return Ok(ApiResponse<List<OrderDto>>.Success(response, "用户订单信息获取成功"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败, UserId: {UserId}", userId);
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "用户不存在, UserId: {UserId}", userId);
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.UserNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异常, UserId: {UserId}", userId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取商家数据统计信息时发生未知异常, UserId: {UserId}", userId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }

        }

        // 用户发起支付请求
        [HttpPost("orders/{orderId}/createPayment")]
        public async Task<ActionResult<ApiResponse<PaymentDto>>> CreatePaymentAsync(
            string orderId,
            [FromQuery] string channel)
        {
            try
            {
                _logger.LogInformation("收到用发起支付订单的请求, OrderId: {OrderId}", orderId);
                var data = await _orderService.CreatePaymentByorderIdAsync(orderId, channel);
                _logger.LogInformation("成功创建订单实体,PayId: {PayId}", data.PayId);
                var response = _mapper.Map<PaymentDto>(data);
                return Ok(ApiResponse<PaymentDto>.Success(response, "用发起支付订单的请求成功"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败, OrderId: {OrderId}", orderId);
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "订单不存在, OrderId: {OrderId}", orderId);
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.OrderNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异常, OrderId: {OrderId}", orderId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "支付时发生未知异常, OrderId: {OrderId}", orderId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }

        }


        [HttpPatch("payments/{payid}/pay")]
        public async Task<ActionResult<ApiResponse<bool>>> PayPaymentAsync(
            string payId,
            [FromQuery] int? payAmount)
        {
            try
            {
                _logger.LogInformation("收到用支付订单的请求, PayId: {payId}", payId);
                var data = await _orderService.UpdatePaymentAsync(payId,(int)PayStatus.Paid,payAmount);
                _logger.LogInformation("成功支付订单, PayId: {payId}", payId);
              
                return Ok(ApiResponse<bool>.Success(true, "用户支付订单成功"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败, PayId: {payId}", payId);
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "订单不存在, PayId: {payId}", payId);
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.OrderNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    ex.ErrorCode,  // 对应 ErrorCodes.DuplicatePayment
                    ex.Message       // 对应 "重复支付！"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "支付时发生未知异常, PayId: {payId}", payId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
        [HttpPatch("payments/{payid}/cancel")]
        public async Task<ActionResult<ApiResponse<bool>>> CancelPaymentAsync(string payId)
        {
            try
            {
                _logger.LogInformation("收到用户取消订单的请求, PayId: {payId}", payId);
                var data = await _orderService.UpdatePaymentAsync(payId,(int)PayStatus.Cancelled,null);
                _logger.LogInformation("成功取消订单, PayId: {payId}", payId);
              
                return Ok(ApiResponse<bool>.Success(true, "用户取消订单成功"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败, PayId: {payId}", payId);
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "订单不存在, PayId: {payId}", payId);
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.OrderNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异常, PayId: {payId}", payId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "支付时发生未知异常, PayId: {payId}", payId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
        
        [HttpPatch("orders/{orderId}/confirm")]
        public async Task<ActionResult<ApiResponse<Order>>> ConfirmedOrderByOrderId(string orderId)
        {
            try
            {
                _logger.LogInformation("收到用户确认收货的请求, OrderId: {OrderId}", orderId);
                var data = await _orderService.UpdateOrderAsync(orderId,(int)OrderStatus.Confirmed);
                _logger.LogInformation("成功确认收货, OrderId: {OrderId}", orderId);
              
                return Ok(ApiResponse<bool>.Success(true, "用户确认收货成功"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败, OrderId: {OrderId}", orderId);
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "订单不存在, OrderId: {OrderId}", orderId);
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.OrderNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异常, OrderId: {OrderId}", orderId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "确认收货时发生未知异常, OrderId: {OrderId}", orderId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
        
        [HttpPatch("orders/{orderId}/cancel")]
        public async Task<ActionResult<ApiResponse<Order>>> CancelOrderByOrderId(string orderId)
        {
            try
            {
                _logger.LogInformation("收到用户取消订单的请求, OrderId: {OrderId}", orderId);
                var data = await _orderService.UpdateOrderAsync(orderId,(int)OrderStatus.Cancelled);
                _logger.LogInformation("成功取消订单, OrderId: {OrderId}", orderId);
              
                return Ok(ApiResponse<bool>.Success(true, "用户取消订单成功"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败, OrderId: {OrderId}", orderId);
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "订单不存在, OrderId: {OrderId}", orderId);
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.OrderNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异常, OrderId: {OrderId}", orderId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "确认收货时发生未知异常, OrderId: {OrderId}", orderId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }

    }
}