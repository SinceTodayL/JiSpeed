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
        private readonly IMapper _mapper;

        public OrderController(
            ILogger<OrderController> logger,
            IOrderService orderService,
            IMapper mapper
        )
        {
            _logger = logger;
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpGet("orders/{orderId}")]
        public async Task<ActionResult<ApiResponse<OrderDetailDto>>> GetOrderByOrderId(string orderId)
        {
            try
            {
                _logger.LogInformation("收到查看订单详情的请求, OrderId: {OrderId}", orderId);
                var order = await _orderService.GetOrderDetailByOrderIdAsync(orderId);
                var response = _mapper.Map<OrderDetailDto>(order);
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
        public async Task<ActionResult<ApiResponse<string>>> GetOrderIdListByUserId(
            string userId,
            [FromQuery] int? orderStatus,
            [FromQuery] int? size, [FromQuery] int? page)
        {
            try
            {
                _logger.LogInformation("收到获取用户订单列表的请求, UserId: {UserId}", userId);
                var orderIds = await _orderService.GetOrderIdByUserIdAsync(userId, orderStatus, size, page);
                _logger.LogInformation("成功获取用户订单列表, UserId: {UserId}", userId);
                return Ok(ApiResponse<List<string>>.Success(orderIds, "用户订单信息获取成功"));
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

        // 用户创建订单，返回对应的订单日志
        [HttpPost("users/{userId}/createOrder")]
        public async Task<ActionResult<ApiResponse<string>>> CreateOrderAsync(
            string userId,
            [FromBody] OrderRequestDto dto)
        {
            try
            {
                _logger.LogInformation("收到用户创建订单的请求, UserId: {UserId}", userId);
                var orderLogId = await _orderService.CreateOrderByUserIdAsync(userId, dto.OrderAmount, dto.CouponId,
                    dto.AddressId, dto.MerchantId, dto.DishQuantities);
                _logger.LogInformation("成功创建订单实体");
                return Ok(ApiResponse<string>.Success(orderLogId, "用户创建订单的请求成功"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败");
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.OrderNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建订单时发生未知异常");
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
                var data = await _orderService.CreatePaymentByOrderIdAsync(orderId, channel);
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
                var data = await _orderService.UpdatePaymentAsync(payId, (int)PayStatus.Paid, payAmount);
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
                    ex.ErrorCode, // 对应 ErrorCodes.DuplicatePayment
                    ex.Message // 对应 "重复支付！"
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
                var data = await _orderService.UpdatePaymentAsync(payId, (int)PayStatus.Cancelled, null);
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
                var data = await _orderService.UpdateOrderAsync(orderId, (int)OrderStatus.Confirmed);
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
                var data = await _orderService.UpdateOrderAsync(orderId, (int)OrderStatus.Cancelled);
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

        [HttpGet("merchants/{merchantId}/orders")]
        public async Task<ActionResult<ApiResponse<List<string>>>> GetOrderListByMerchantId(
            string merchantId,
            [FromQuery] int? orderStatus,
            [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate,
            [FromQuery] int? size, [FromQuery] int? page)
        {
            try
            {
                _logger.LogInformation("收到获取商家订单列表的请求");
                var orderIds =
                    await _orderService.GetOrderIdByMerchantIdAsync(merchantId, orderStatus, startDate, endDate, size,
                        page);
                _logger.LogInformation("成功获取商家订单列表");
                return Ok(ApiResponse<List<string>>.Success(orderIds, "商家订单信息获取成功"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败");
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.MerchantNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取商家订单信息时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }

        }

        [HttpGet("orderLogs/{logId}")]
        public async Task<ActionResult<ApiResponse<OrderLogResponseDto>>> GetOrderLogByLogId(string logId)
        {
            try
            {
                _logger.LogInformation("收到查看订单日志详情的请求");
                var orderlog = await _orderService.GetOrderLogDetailByLogIdAsync(logId);
                var response = _mapper.Map<OrderLogResponseDto>(orderlog);
                _logger.LogInformation("成功获取订单日志详细信息");
                return Ok(ApiResponse<OrderLogResponseDto>.Success(response, "订单日志详细信息获取成功"));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.ResourceNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取日志信息时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }

        }

        [HttpGet("payments/{payId}")]
        public async Task<ActionResult<ApiResponse<PaymentResponseDto>>> GetPaymentByPayId(string payId)
        {
            try
            {
                _logger.LogInformation("收到查看订单日志详情的请求");
                var payment = await _orderService.GetPaymentDetailByPayIdAsync(payId);
                var response = _mapper.Map<PaymentResponseDto>(payment);
                _logger.LogInformation("成功获取订单日志详细信息");
                return Ok(ApiResponse<PaymentResponseDto>.Success(response, "订单日志详细信息获取成功"));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.ResourceNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取日志信息时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }

        }


        // 用户创建退款申请，返回对应的订单日志ID
        [HttpPost("users/{userId}/orders/{orderId}/refunds")]
        public async Task<ActionResult<ApiResponse<string>>> CreateRefundAsync(
            string userId,
            string orderId,
            [FromBody] RefundRequestDto dto)
        {
            try
            {
                _logger.LogInformation("收到用户退款的请求, UserId: {UserId}", userId);
                var orderLogId =
                    await _orderService.CreateRefundByOrderIdAndUserIdAsync(userId, orderId, dto.Reason,
                        dto.RefundAmount);
                _logger.LogInformation("成功创建退款申请");
                return Ok(ApiResponse<string>.Success(orderLogId, "用户创建订单的请求成功"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败");
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(
                    ex.ErrorCode,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建退款实体时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }

        }

        // 商家同意退款
        [HttpPatch("merchants/{merchantId}/refunds/{refundId}/approve")]
        public async Task<ActionResult<ApiResponse<bool>>> ApproveRefundAsync(
            string merchantId,
            string refundId
        )
        {
            try
            {
                _logger.LogInformation("收到商家同意的请求");
                var orderLogId =
                    await _orderService.UpdateRefundForMerchantAsync(merchantId, refundId, (int)RefundStatus.Refunded);
                _logger.LogInformation("成功创建退款申请");
                return Ok(ApiResponse<bool>.Success(true, "商家退款成功"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败");
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(
                    ex.ErrorCode,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新退款实体时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }

        }

        // 商家拒绝退款
        [HttpPatch("merchants/{merchantId}/refunds/{refundId}/reject")]
        public async Task<ActionResult<ApiResponse<bool>>> RejectRefundAsync(
            string merchantId,
            string refundId
        )
        {
            try
            {
                _logger.LogInformation("收到商家拒绝退款的请求");
                await _orderService.UpdateRefundForMerchantAsync(merchantId, refundId, (int)RefundStatus.Rejected);
                _logger.LogInformation("成功拒绝退款申请");
                return Ok(ApiResponse<bool>.Success(true, "商家拒绝退款成功"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败");
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(
                    ex.ErrorCode,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新退款实体时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }

        }

        // 管理员操作退款
        [HttpPatch("admins/{adminId}/refunds/{refundId}/audit")]
        public async Task<ActionResult<ApiResponse<bool>>> AuditRefundByAdminAsync(
            string adminId,
            string refundId,
            [FromBody] RefundUpdateDto dto
        )
        {
            try
            {
                _logger.LogInformation("收到管理员处理退款的请求");
                await _orderService.UpdateRefundForAdminAsync(adminId, refundId, dto.RefundStatus);
                _logger.LogInformation("成功更新退款申请");
                return Ok(ApiResponse<bool>.Success(true, "管理员更新退款实体成功"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败");
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(
                    ex.ErrorCode,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新退款实体时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }

        }



        [HttpGet("refunds/{refundId}")]
        public async Task<ActionResult<ApiResponse<RefundResponseDto>>> GetPRefundByPayId(string refundId)
        {
            try
            {
                _logger.LogInformation("收到查看退款详情的请求");
                var payment = await _orderService.GetRefundDetailByRefundIdAsync(refundId);
                var response = _mapper.Map<RefundResponseDto>(payment);
                _logger.LogInformation("成功获取退款详细信息");
                return Ok(ApiResponse<RefundResponseDto>.Success(response, "退款详细信息获取成功"));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.ResourceNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取退款信息时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }

        }
        
        [HttpGet("users/{userId}/refunds")]
        public async Task<ActionResult<ApiResponse<List<string>>>> GetRefundListByUserId(
            string userId,
            [FromQuery] int? auditStatus,
            [FromQuery] int? size, [FromQuery] int? page)
        {
            try
            {
                _logger.LogInformation("收到查看用户退款记录的请求");
                var refundList = await _orderService.GetRefundListByFilterAsync(userId, null,null,auditStatus, size,page);
                _logger.LogInformation("成功获取用户退款记录信息");
                return Ok(ApiResponse<List<string>>.Success(refundList, "用户退款记录获取成功"));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.ResourceNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户退款列表时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }

        }
        
        [HttpGet("merchants/{merchantId}/refunds")]
        public async Task<ActionResult<ApiResponse<List<string>>>> GetRefundListByMerchantId(
            string merchantId,
            [FromQuery] int? auditStatus,
            [FromQuery] int? size, [FromQuery] int? page)
        {
            try
            {
                _logger.LogInformation("收到查看商家关联的退款记录的请求");
                var refundList = await _orderService.GetRefundListByFilterAsync(null, merchantId, null,auditStatus, size, page);
                _logger.LogInformation("成功获取商家关联退款记录信息");
                return Ok(ApiResponse<List<string>>.Success(refundList, "商家关联退款记录获取成功"));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.ResourceNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取商家关联退款列表时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }

        }
        
        [HttpGet("admins/{adminId}/refunds")]
        public async Task<ActionResult<ApiResponse<List<string>>>> GetRefundListForAdmin(
            string adminId,
            [FromQuery] int? auditStatus,
            [FromQuery] int? size, [FromQuery] int? page)
        {
            try
            {
                _logger.LogInformation("收到管理员查看退款记录的请求");
                var refundList = await _orderService.GetRefundListByFilterAsync(null, null, adminId,auditStatus, size, page);
                _logger.LogInformation("成功获管理员查看退款记录信息");
                return Ok(ApiResponse<List<string>>.Success(refundList, "管理员查看退款记录获取成功"));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.ResourceNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取管理员查看退款记录时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }

        }
        
        [HttpGet("complaints/{complaintId}")]
        public async Task<ActionResult<ApiResponse<ComplaintDetailDto>>> GetComplaintDetailAsync(string complaintId)
        {
            try
            {
                _logger.LogInformation("收到查看投诉详情的请求");
                var complaint = await _orderService.GetComplaintDetailByComplainantIdAsync(complaintId);
                _logger.LogInformation("成功查看投诉详情");
                var response = _mapper.Map<ComplaintDetailDto>(complaint);
                return Ok(ApiResponse<ComplaintDetailDto>.Success(response, "查看投诉详情成功"));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.ResourceNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取投诉详情时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }

        }
        
        [HttpPost("complaints")]
        public async Task<ActionResult<ApiResponse<string>>> CreateComplaintAsync(
            [FromBody]ComplaintRequestDto dto)
        {
            try
            {
                _logger.LogInformation("收到创建投诉的请求");
                var complaintId = await _orderService.CreateComplaintDetailAsync(dto.OrderId,dto.ComplainantId,dto.CmplRole,dto.CmplDescription);
                _logger.LogInformation("成功创建投诉"); 
                return Ok(ApiResponse<string>.Success(complaintId, "查看投诉详情成功"));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.ResourceNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建投诉详情时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }

        }
        
        [HttpPatch("admin/{adminId}complaints/{complaintId}/audit")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateComplaintAsync(string adminId,string complaintId)
        {
            try
            {
                _logger.LogInformation("收到更新投诉的请求");
                await _orderService.AuditComplaintAsync(adminId, complaintId);
                _logger.LogInformation("成功更新投诉"); 
                return Ok(ApiResponse<bool>.Success(true,"更新投诉详情成功"));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.ResourceNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新投诉时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }

        }
        
        [HttpPatch("users/{userId}complaints/{complaintId}/cancel")]
        public async Task<ActionResult<ApiResponse<bool>>> CancelComplaintAsync(string userId,string complaintId)
        {
            try
            {
                _logger.LogInformation("收到用户关闭投诉的请求");
                await _orderService.CancelComplaintAsync(userId, complaintId);
                _logger.LogInformation("成功关闭投诉"); 
                return Ok(ApiResponse<bool>.Success(true,"关闭投诉详情成功"));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.ResourceNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "关闭投诉时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }

        }
        
        [HttpGet("complaints")]
        public async Task<ActionResult<ApiResponse<List<string>>>> GetComplaintListByFilter(
            [FromQuery]string? userId,
            [FromQuery]string? merchantId,
            [FromQuery]int? status,
            [FromQuery]string? adminId,
            [FromQuery]int? size,[FromQuery]int? page)
        {
            try
            {
                _logger.LogInformation("收到获取投诉的请求");
                var complaintIds = await _orderService.GetComplaintListByFilterAsync(userId, merchantId, status,adminId, size, page);
                _logger.LogInformation("成功获取投诉"); 
                return Ok(ApiResponse<List<string>>.Success(complaintIds,"获取列表成功"));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.ResourceNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取投诉时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }

        }
    }
}