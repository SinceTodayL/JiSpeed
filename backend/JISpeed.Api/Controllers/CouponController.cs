using AutoMapper;
using JISpeed.Api.Common;
using JISpeed.Api.DTOs;
using JISpeed.Core.Constants;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace JISpeed.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class CouponController : ControllerBase
    {
       
        private readonly IMapper _mapper;
        private readonly ILogger<CouponController> _logger;
        private readonly ICouponService _couponService;

        public CouponController(
            ILogger<CouponController> logger,
            IMapper mapper,
            ICouponService couponService
        )
        {
            _logger = logger;
            _mapper = mapper;
            _couponService = couponService;
        }
        
        
        [HttpGet("users/{userId}/coupons")]
        public async Task<ActionResult<ApiResponse<List<string>>>> GetAllCoupons(
            string userId,
            [FromQuery]bool? isActive,
            [FromQuery]decimal? amount,
            [FromQuery]int? size, [FromQuery]int? page)
        {
            try
            {
                _logger.LogInformation("收到获取顾客优惠券统计请求");
                var couponIds = await _couponService.GetCouponIdByFilterAsync(userId, isActive,amount, size, page);
                _logger.LogInformation("成功获取顾客优惠券统计信息");
                return Ok(ApiResponse<List<string>>.Success(couponIds,"顾客优惠券信息获取成功"));
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
                _logger.LogWarning(ex, "用户不存在");
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.UserNotFound,
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
                _logger.LogError(ex, "获取优惠券信息时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
        
        [HttpGet("coupons/{couponId}")]
        public async Task<ActionResult<ApiResponse<CouponResponseDto>>> GetCouponDetail(string couponId)
        {
            try
            {
                _logger.LogInformation("收到获取优惠券详情请求");
                var coupon = await _couponService.GetCouponDetailByCouponIdAsync(couponId);
                _logger.LogInformation("成功获取优惠券详情信息");
                var response = _mapper.Map<CouponResponseDto>(coupon);
                return Ok(ApiResponse<CouponResponseDto>.Success(response,"优惠券信息获取成功"));
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
                    ErrorCodes.ResourceNotFound,
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
                _logger.LogError(ex, "获取优惠券信息时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
        [HttpPost("admin/{adminId}/coupons")]
        public async Task<ActionResult<ApiResponse<bool>>> IssueCouponsForAllUsers(
            string adminId,
            [FromBody]CouponRequestDto dto)
        {
            try
            {
                _logger.LogInformation("收到发放优惠券请求");
                var coupon = await _couponService.IssueCouponsAsync(adminId, dto.StartTime, dto.EndTime,dto.UserId,dto.FaceValue, dto.Threshold);
                _logger.LogInformation("成功发放优惠券");
                return Ok(ApiResponse<bool>.Success(true,"优惠券发放成功"));
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
                    ErrorCodes.ResourceNotFound,
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
                _logger.LogError(ex, "发放优惠券时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
    }
}