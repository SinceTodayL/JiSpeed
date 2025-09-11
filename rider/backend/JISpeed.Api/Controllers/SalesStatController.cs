using AutoMapper;
using JISpeed.Api.Common;
using JISpeed.Api.DTOs;
using JISpeed.Core.Constants;
using JISpeed.Core.Entities.Merchant;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace JISpeed.Api.Controllers
{
    [ApiController]
    [Route("api/")]
    public class SalesStatController : ControllerBase
    {
        private readonly IMerchantService _merchantService;
        private readonly ILogger<SalesStatController> _logger;
        private readonly IMapper _mapper;

        public SalesStatController(
            IMerchantService merchantService,
            ILogger<SalesStatController> logger,
            IMapper mapper)
        {
            _merchantService = merchantService;
            _logger = logger;
            _mapper = mapper;
        }
        
        // 根据商家ID和日期获取商家统计数据
        [HttpGet("merchants/{merchantId}/SalesStat/{statDate}")]
        public async Task<ActionResult<ApiResponse<SalesStatDto>>> GetMerchantSalesStatBystatDateAndMerchantId(string merchantId, DateTime statDate)
        {
            try
            {
                _logger.LogInformation("收到获取商家数据统计请求, MerchantID: {MerchantID}", merchantId);
                // 参数验证
                if (string.IsNullOrWhiteSpace(merchantId))
                {
                    _logger.LogWarning("商家ID参数为空");
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.MissingParameter,
                        "商家ID不能为空"));
                }

                await _merchantService.GetMerchantDetailAsync(merchantId);

                var data = await _merchantService.GetSalesStateByMerchantIdAndDateTimeAsync(merchantId, statDate);
                var response = _mapper.Map<SalesStatDto>(data);
                _logger.LogInformation("成功获取用户详细信息, MerchantId: {MerchantId}", merchantId);

                return Ok(ApiResponse<SalesStatDto>.Success(response, "商家信息获取成功"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败, MerchantId: {MerchantId}", merchantId);
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "无相关数据, MerchantId: {MerchantId}", merchantId);
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.ResourceNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异常, MerchantId: {MerchantId}", merchantId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取商家数据统计信息时发生未知异常, MerchantId: {MerchantId}", merchantId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
        
        
        // 根据商家ID和自定义筛选获取商家统计数据
        [HttpGet("merchants/{merchantId}/SalesStat")]
        public async Task<ActionResult<ApiResponse<List<SalesStatDto>>>> GetMerchantSalesStatBystatDateAndStatType(
            string merchantId,
            [FromQuery]DateTime? startTime, 
            [FromQuery]DateTime? endTime,
            [FromQuery] decimal? minAmount,
            [FromQuery] decimal? maxAmount,
            [FromQuery]int? statType,
            [FromQuery]int? size,
            [FromQuery]int? page)
        {
            try
            {
                _logger.LogInformation("收到获取商家数据统计请求, MerchantID: {MerchantID}", merchantId);
                // 参数验证
                if (string.IsNullOrWhiteSpace(merchantId))
                {
                    _logger.LogWarning("商家ID参数为空");
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.MissingParameter,
                        "商家ID不能为空"));
                }

                await _merchantService.GetMerchantDetailAsync(merchantId);
                var data = await _merchantService.GetByFiltersAsync(merchantId, statType, size, page, startTime, endTime, minAmount, maxAmount);
                var response = _mapper.Map<List<SalesStatDto>>(data);
                _logger.LogInformation("成功获取用户详细信息, MerchantId: {MerchantId}", merchantId);

                return Ok(ApiResponse<List<SalesStatDto>>.Success(response, "商家信息获取成功"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败, MerchantId: {MerchantId}", merchantId);
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "无相关数据, MerchantId: {MerchantId}", merchantId);
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.ResourceNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异常, MerchantId: {MerchantId}", merchantId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取商家数据统计信息时发生未知异常, MerchantId: {MerchantId}", merchantId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
    }
}