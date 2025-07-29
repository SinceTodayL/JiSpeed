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
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly IMerchantService _merchantService;
        private readonly ILogger<ApplicationController> _logger;
        private readonly IMapper _mapper;

        public ApplicationController(
            IApplicationService applicationService,
            IMerchantService merchantService,
            ILogger<ApplicationController> logger,
            IMapper mapper)
        {
            _applicationService = applicationService;
            _merchantService = merchantService;
            _logger = logger;
            _mapper = mapper;
        }

        // 管理员通过/拒绝申请
        [HttpPatch("admin/{adminId}/audit/{applyId}")]
        public async Task<ActionResult<ApiResponse<bool>>> Audit(string adminId, string applyId,[FromBody] AuditRequest request)
        {
            try
            { 
                // 参数验证
                if (string.IsNullOrWhiteSpace(adminId)||string.IsNullOrWhiteSpace(applyId))
                {
                    _logger.LogWarning("ID参数为空");
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.MissingParameter,
                        "ID不能为空"));
                }
                _logger.LogInformation("收到管理员处理申请请求, AdminId: {AdminId}", adminId);
                var entity = _mapper.Map<AuditRequest>(request);
                if (entity.AuditStatus == 1)
                {
                    _logger.LogInformation("收到管理员通过申请请求, AdminId: {AdminId}", adminId);
                    await _applicationService.ApproveApplicationAsync(applyId, adminId);
                }
                else if (entity.AuditStatus == 2)
                {   
                    _logger.LogInformation("收到管理员拒绝申请请求, AdminId: {AdminId}", adminId);
                    await _applicationService.RejectApplicationAsync(applyId, adminId, entity.RejectReason);
                }
                else
                {
                    _logger.LogInformation("未知申请");
                    throw new ValidationException("未知申请");
                }
                _logger.LogInformation("管理员处理申请成功, AdminId: {AdminId}", adminId);
                return Ok(ApiResponse<bool>.Success(true));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败, AdminId: {AdminId}", adminId);
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "实体不存在, AdminId: {AdminId}，ApplyId：{ApplyId}", adminId, applyId);
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.ResourceNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异常, AdminId: {AdminId}，ApplyId：{ApplyId}", adminId, applyId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "管理员处理申请时发生未知异常, AdminId: {AdminId}，ApplyId：{ApplyId}", adminId, applyId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
        
        [HttpPost("merchant/{merchantId}/applications")]
        public async Task<ActionResult<ApiResponse<bool>>> SubmitApplication(string merchantId, [FromBody] ApplicationRequest request)
        {
            try
            { 
                // 参数验证
                if (string.IsNullOrWhiteSpace(merchantId))
                {
                    _logger.LogWarning("ID参数为空");
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.MissingParameter,
                        "ID不能为空"));
                }
                _logger.LogInformation("收到商家发起申请请求, MerchantId: {MerchantId}", merchantId);
                var entity = _mapper.Map<ApplicationRequest>(request);
                var application = new Core.Entities.Merchant.Application
                {
                    AuditStatus = 0,
                    MerchantId = merchantId,
                    ApplyId = Guid.NewGuid().ToString("N"),
                    SubmittedAt = DateTime.Now,
                    CompanyName = entity.CompanyName,
                    Merchant = await _merchantService.GetMerchantDetailAsync(merchantId)
                };
                await _applicationService.CreateApplicationEntityAsync(merchantId, application);
                _logger.LogInformation("创建申请成功,  MerchantId: {MerchantId}", merchantId);
                return Ok(ApiResponse<bool>.Success(true));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败,  MerchantId: {MerchantId}", merchantId);
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "实体不存在, MerchantId: {MerchantId}", merchantId);
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.ResourceNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异常,MerchantId: {MerchantId}", merchantId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建申请时发生未知异常, MerchantId: {MerchantId}", merchantId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
        
        
        [HttpGet("admin/{adminId}/applications")]
        public async Task<ActionResult<ApiResponse<List<ApplicationResponse>>>> GetPendingApplications
        (
            string adminId,
            [FromBody]AuditRequest request
            )
        {
            try
            { 
                // 参数验证
                if (string.IsNullOrWhiteSpace(adminId))
                {
                    _logger.LogWarning("ID参数为空");
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.MissingParameter,
                        "ID不能为空"));
                }
                _logger.LogInformation("收到获取目标申请状态的申请列表请求, AdminId: {AdminId}", adminId);
                var entity = _mapper.Map<AuditRequest>(request);
                var data = await _applicationService.GetApplicationsByAuditStatusAsync(entity.AuditStatus);
                var response = _mapper.Map<List<ApplicationResponse>>(data);
                _logger.LogInformation("获取目标申请状态的申请列表请求成功,  AdminId: {AdminId}", adminId);
                return Ok(ApiResponse<List<ApplicationResponse>>.Success(response));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败, AdminId: {AdminId}", adminId);
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "实体不存在, AdminId: {AdminId}", adminId);
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.ResourceNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异常,AdminId: {AdminId}", adminId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取目标申请状态的申请列表时发生未知异常, AdminId: {AdminId}", adminId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
        
        
        [HttpGet("merchant/{merchantId}/applications")]
        public async Task<ActionResult<ApiResponse<List<ApplicationResponse>>>> GetApplicationListByMerchantId(string merchantId)
        {
            try
            { 
                // 参数验证
                if (string.IsNullOrWhiteSpace(merchantId))
                {
                    _logger.LogWarning("ID参数为空");
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.MissingParameter,
                        "ID不能为空"));
                }
                _logger.LogInformation("收到商家获取申请请求, MerchantId: {MerchantId}", merchantId);
                
                var data = await _applicationService.GetApplicationsByMerchantAsync(merchantId);
                var response = _mapper.Map<List<ApplicationResponse>>(data);
                _logger.LogInformation("商家获取申请请求成功, MerchantId: {MerchantId}", merchantId);
                return Ok(ApiResponse<List<ApplicationResponse>>.Success(response));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败,MerchantId: {MerchantId}", merchantId);
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "实体不存在, MerchantId: {MerchantId}", merchantId);
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.ResourceNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异常,MerchantId: {MerchantId}", merchantId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "商家获取申请请求时发生未知异常, MerchantId: {MerchantId}", merchantId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
        
        
        [HttpGet("applications/{applicationId}")]
        public async Task<ActionResult<ApiResponse<ApplicationResponse>>> GetApplicationByApplicationId(string applicationId)
        {
            try
            { 
                // 参数验证
                if (string.IsNullOrWhiteSpace(applicationId))
                {
                    _logger.LogWarning("ID参数为空");
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.MissingParameter,
                        "ID不能为空"));
                }
                _logger.LogInformation("收到获取目标申请请求, ApplicationId: {ApplicationId}", applicationId);
                
                var data = await _applicationService.GetDetailsAsync(applicationId);
                var response = _mapper.Map<ApplicationResponse>(data);
                _logger.LogInformation("获取目标申请请求成功,  ApplicationId: {ApplicationId}", applicationId);
                return Ok(ApiResponse<ApplicationResponse>.Success(response));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败, ApplicationId: {ApplicationId}", applicationId);
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "实体不存在, ApplicationId: {ApplicationId}", applicationId);
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.ResourceNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异常, ApplicationId: {ApplicationId}", applicationId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取目标申请时发生未知异常,  ApplicationId: {ApplicationId}", applicationId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
        
        [HttpGet("applications")]
        public async Task<ActionResult<ApiResponse<List<ApplicationResponse>>>> GetApplicationByDateRange(DateTime? startDate, DateTime? endDate)
        {
            try
            { 
                var start = startDate ?? DateTime.MinValue; 
                var end = endDate ?? DateTime.Now;
                _logger.LogInformation("收到获取时间段内的申请请求, StartDate: {StartDate}，EndDate：{EndDate}", start, end);

                
                var data = await _applicationService.GetApplicationsByTimeRangeAsync(start, end);
                var response = _mapper.Map<List<ApplicationResponse>>(data);
                _logger.LogInformation("获取时间段内的申请请求成功, StartDate: {StartDate}，EndDate：{EndDate}", start, end);
                return Ok(ApiResponse<List<ApplicationResponse>>.Success(response));
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
                _logger.LogWarning(ex, "实体不存在");
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
                _logger.LogError(ex, "获取时间段内的申请请求时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
    }
}