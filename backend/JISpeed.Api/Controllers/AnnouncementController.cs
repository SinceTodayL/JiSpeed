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
    public class AnnouncementController : ControllerBase
    {
        private readonly ILogger<AnnouncementController> _logger;
        private readonly IMapper _mapper;
        private readonly IAdminService _adminService;

        public AnnouncementController(
            ILogger<AnnouncementController> logger,
            IMapper mapper,
            IAdminService adminService
        )
        {
            _logger = logger;
            _mapper = mapper;
            _adminService = adminService;
        }

        [HttpPost("admin/{adminId}/announcements")]
        public async Task<ActionResult<ApiResponse<bool>>> AddNewAnnouncement(string adminId,
            [FromBody] CreateAnnouncementDto request)
        {
            try
            {
                _logger.LogInformation("收到管理员新增公告的请求, AdminId: {AdminId}", adminId);
                // 参数验证
                if (string.IsNullOrWhiteSpace(adminId))
                {
                    _logger.LogWarning("管理员ID参数为空");
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.MissingParameter,
                        "管理员ID不能为空"));
                }

                var res = await _adminService.CreateAnnouncementEntityAsync(adminId, request.Title, request.Content,request.TargetRole,request.StartAt,request.EndAt);
                _logger.LogInformation("成功新增公告, AdminId: {AdminId}", adminId);
                return Ok(ApiResponse<bool>.Success(res));
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
                _logger.LogWarning(ex, "管理员不存在, AdminId: {AdminId}", adminId);
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.ResourceNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异常, AdminId: {AdminId}", adminId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "新增公告时发生未知异常, AdminId: {AdminId}", adminId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
        
        
        [HttpGet("announcements/activate/{TargetRole}")]
        public async Task<ActionResult<ApiResponse<List<AnnouncementResponseDto>>>> GetActivateAnnouncementsByUserType(
            string targetRole,
            [FromQuery]int? size,[FromQuery]int? page)
        {
            try
            {
                _logger.LogInformation("收到获取有效公告的请求");
                var res = await _adminService.GetActiveAnnouncementByUserTypeAsync(targetRole,size,page);
                _logger.LogInformation("成功获取有效公告");
                var response = _mapper.Map<List<AnnouncementResponseDto>>(res);
                return Ok(ApiResponse<List<AnnouncementResponseDto>>.Success(response));
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
                _logger.LogError(ex, "业务处理异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取公告时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }

        [HttpGet("announcements/all/{TargetRole}")]
        public async Task<ActionResult<ApiResponse<List<AnnouncementResponseDto>>>> GetAllAnnouncementsByDateAndUserType(
            string targetRole,
            [FromQuery]int? size,[FromQuery]int? page)
        {
            try
            {
                _logger.LogInformation("收到获取公告的请求");
                var res = await _adminService.GetAllAnnouncementByUserTypeAsync(targetRole,size,page);
                _logger.LogInformation("成功获取公告");
                var response = _mapper.Map<List<AnnouncementResponseDto>>(res);
                return Ok(ApiResponse<List<AnnouncementResponseDto>>.Success(response));
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
                _logger.LogError(ex, "业务处理异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取公告时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }

        [HttpPatch("announcements/{announcementId}")]
        public async Task<ActionResult<ApiResponse<bool>>> ModifyAnnouncement(
            string announcementId,
            [FromQuery]string? title,[FromQuery] string? content, [FromQuery]string? targetRole,
            [FromQuery]DateTime? startAt,[FromQuery]DateTime? endAt)
        {
            try
            {
                _logger.LogInformation("收到修改公告的请求");
                var res = await _adminService.ModifyAnnouncementAsync(announcementId,title,content,targetRole,startAt,endAt);
                _logger.LogInformation("成功修改公告");
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
                _logger.LogError(ex, "业务处理异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取公告时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
        [HttpGet("announcements/{announcementId}")]
        public async Task<ActionResult<ApiResponse<AnnouncementResponseDto>>> GetAnnouncementDetailByIdAsync(string announcementId)
        {
            try
            {
                _logger.LogInformation("收到获取公告详情的请求");
                var res = await _adminService.GetAnnouncementDetailAsync(announcementId);
                _logger.LogInformation("成功获取公告详情");
                var response = _mapper.Map<AnnouncementResponseDto>(res);
                return Ok(ApiResponse<AnnouncementResponseDto>.Success(response));
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
                _logger.LogError(ex, "业务处理异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取公告时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
    }
}