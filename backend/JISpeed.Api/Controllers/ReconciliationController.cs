using AutoMapper;
using JISpeed.Api.Common;
using JISpeed.Api.DTOs;
using JISpeed.Core.Constants;
using JISpeed.Core.Entities.Reconciliation;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace JISpeed.Api.Controllers
{
    [ApiController]
    [Route("api/")]
    public class ReconciliationController : ControllerBase
    {
        private readonly ILogger<ReconciliationController> _logger;
        private readonly IAdminService _adminService;
        private readonly IMapper _mapper;

        public ReconciliationController(
            ILogger<ReconciliationController> logger,
            IAdminService adminService,
            IMapper mapper
        )
        {
            _logger = logger;
            _adminService = adminService;
            _mapper = mapper;
        }

        [HttpPost("reconciliations")]
        public async Task<ActionResult<ApiResponse<bool>>> AddNewReconciliation( 
            [FromBody]ReconciliationRequestDto  dto)
        {
            try
            {
                _logger.LogInformation("收到新增对账异常主体的请求");
                // 参数验证
                var res = await _adminService.CreateReconciliationEntityAsync(dto.PeriodStart, dto.PeriodEnd,dto.ReconType,dto.OrderIds);
                _logger.LogInformation("成功新增对账异常主体");
                return Ok(ApiResponse<bool>.Success(res));
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
                _logger.LogWarning(ex, "资源不存在");
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.ResourceNotFound,
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
                _logger.LogError(ex, "新增对账异常主体时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
        
        [HttpGet("reconciliations/{reconId}")]
        public async Task<ActionResult<ApiResponse<ReconciliationResponseDto>>> GetReconciliationDetail(string reconId)
        {
            try
            {
                _logger.LogInformation("收到获取对账异常主体的请求");
                // 参数验证
                var entity = await _adminService.GetReconciliationDetailAsync(reconId);
                var response = _mapper.Map<ReconciliationResponseDto>(entity);
                _logger.LogInformation("成功获取对账异常主体");
                return Ok(ApiResponse<ReconciliationResponseDto>.Success(response));
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
                _logger.LogWarning(ex, "资源不存在");
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.ResourceNotFound,
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
                _logger.LogError(ex, "获取对账异常主体时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
        
        [HttpGet("reconciliations")]
        public async Task<ActionResult<ApiResponse<List<ReconciliationResponseDto>>>> GetReconciliationByFilter(
            [FromQuery]bool? isResolved,
            [FromQuery]int? reconType,
            [FromQuery]int? size, [FromQuery]int? page)
        {
            try
            {
                _logger.LogInformation("收到获取对账异常主体的请求");
                var entityList = await _adminService.GetReconciliationByFilterAsync(isResolved, reconType, size, page);
                var response = _mapper.Map<List<ReconciliationResponseDto>>(entityList);
                _logger.LogInformation("成功获取对账异常主体");
                return Ok(ApiResponse<List<ReconciliationResponseDto>>.Success(response));
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
                _logger.LogWarning(ex, "资源不存在");
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.ResourceNotFound,
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
                _logger.LogError(ex, "获取对账异常主体时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
        [HttpPatch("reconciliations/{reconId}/resolve")]
        public async Task<ActionResult<ApiResponse<bool>>> ResolveReconciliation(string reconId)
        {
            try
            {
                _logger.LogInformation("收到解决对账异常主体的请求");
                var entityList = await _adminService.ResolveReconciliationAsync(reconId);
                _logger.LogInformation("成功解决对账异常主体");
                return Ok(ApiResponse<bool>.Success(true));
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
                _logger.LogWarning(ex, "资源不存在");
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.ResourceNotFound,
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
                _logger.LogError(ex, "解决对账异常主体时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }

    }
}