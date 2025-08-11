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
    [Route("api/")]

    public class SettlementController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<SettlementController> _logger;
        private readonly ISettlementService _settlementService;

        public SettlementController(
            ILogger<SettlementController> logger,
            ISettlementService settlementService,
            IMapper mapper
        )
        {
            _settlementService = settlementService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("settlements/{settlementId}")]
        public async Task<ActionResult<ApiResponse<SettlementDetailDto>>> GetSettlementDetail(string settlementId)
        {
            try
            {
                _logger.LogInformation("收到获取结算单请求, SettlementId: {settlementId}", settlementId);
                // 参数验证
                if (string.IsNullOrWhiteSpace(settlementId))
                {
                    _logger.LogWarning("结算单ID参数为空");
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.MissingParameter,
                        "结算单ID不能为空"));
                }
                var entity = await _settlementService.GetDetailBySettlementIdAsync(settlementId);
                _logger.LogInformation("成功获取结算单详细信息, SettlementId: {settlementId}", settlementId);
                var response = _mapper.Map<SettlementDetailDto>(entity);
                return Ok(ApiResponse<SettlementDetailDto>.Success(response,"获取结算单详细信息成功"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败, SettlementId: {settlementId}", settlementId);
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "结算单不存在, SettlementId: {settlementId}", settlementId);
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.ResourceNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异常, SettlementId: {settlementId}", settlementId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取结算单信息时发生未知异常, SettlementId: {settlementId}", settlementId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }

        
        [HttpGet("settlements/merchants/{merchantId}")]
        public async Task<ActionResult<ApiResponse<List<SettlementDto>>>> GetSettlementByMerchantId(
            string merchantId,
            [FromQuery]DateTime? startDate, [FromQuery]DateTime? endDate,
            [FromQuery]bool? isSettled,
            [FromQuery]int? size,[FromQuery]int? page)
        {
            try
            {
                _logger.LogInformation("收到获取结算单请求, MerchantId: {MerchantId}", merchantId);
                // 参数验证
                if (string.IsNullOrWhiteSpace(merchantId))
                {
                    _logger.LogWarning("结算单ID参数为空");
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.MissingParameter,
                        "结算单ID不能为空"));
                }
                var entity = await _settlementService.GetSettlementListByFiltersAsync(merchantId, startDate, endDate,isSettled, size, page);
                _logger.LogInformation("成功获取结算单详细信息, MerchantId: {MerchantId}", merchantId);
                var response = _mapper.Map<List<SettlementDto>>(entity);
                return Ok(ApiResponse<List<SettlementDto>>.Success(response,"获取结算单详细信息成功"));
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
                _logger.LogWarning(ex, "商家不存在, MerchantId: {MerchantId}", merchantId);
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.MerchantNotFound,
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
                _logger.LogError(ex, "获取结算单信息时发生未知异常, MerchantId: {MerchantId}", merchantId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }

        [HttpGet("settlements")]
        public async Task<ActionResult<ApiResponse<List<SettlementDto>>>> GetSettlement(
            [FromQuery]DateTime? startDate, [FromQuery]DateTime? endDate,
            [FromQuery]bool? isSettled,
            [FromQuery]int? size,[FromQuery]int? page)
        {
            try
            {
                _logger.LogInformation("收到获取结算单请求");
                var entity = await _settlementService.GetSettlementListByFiltersAsync(null, startDate, endDate,isSettled, size, page);
                _logger.LogInformation("成功获取结算单详细信息");
                var response = _mapper.Map<List<SettlementDto>>(entity);
                return Ok(ApiResponse<List<SettlementDto>>.Success(response,"获取结算单详细信息成功"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败");
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
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
                _logger.LogError(ex, "获取结算单信息时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
    }
}
