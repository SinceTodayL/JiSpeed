/*using Microsoft.AspNetCore.Mvc;
using JISpeed.Api.Common;
using JISpeed.Api.DTOs;
using JISpeed.Api.Mappers;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Constants;

namespace JISpeed.Api.Controllers
{
    
    // 用户相关API控制器
    
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        
        // 根据用户ID获取用户详细信息
        
        // <param name="userId">用户ID</param>
        // <returns>用户详细信息</returns>
        // <response code="200">成功获取用户信息</response>
        // <response code="404">用户不存在</response>
        // <response code="400">请求参数错误</response>
        // <response code="500">服务器内部错误</response>
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(ApiResponse<UserDetailDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<UserDetailDto>>> GetUserDetail(string userId)
        {
            try
            {
                _logger.LogInformation("收到获取用户详细信息请求, UserId: {UserId}", userId);

                // 参数验证
                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogWarning("用户ID参数为空");
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.MissingParameter,
                        "用户ID不能为空"));
                }

                // 获取用户详细信息
                var user = await _userService.GetUserDetailAsync(userId);
                if (user == null)
                {
                    return NotFound(ApiResponse<object>.Fail(
                        ErrorCodes.ResourceNotFound,
                        "用户不存在"));
                }

                // 获取用户统计信息
                var stats = await _userService.GetUserStatsAsync(userId);

                // 转换为DTO
                var userDetailDto = UserMapper.ToUserDetailDto(user, stats);

                _logger.LogInformation("成功获取用户详细信息, UserId: {UserId}, Nickname: {Nickname}",
                    userId, user.Nickname);

                return Ok(ApiResponse<UserDetailDto>.Success(userDetailDto, "用户信息获取成功"));
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
                    ErrorCodes.ResourceNotFound,
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
                _logger.LogError(ex, "获取用户详细信息时发生未知异常, UserId: {UserId}", userId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }

        
        // 检查用户是否存在
        
        // <param name="userId">用户ID</param>
        // <returns>用户是否存在</returns>
        // <response code="200">检查成功</response>
        // <response code="400">请求参数错误</response>
        [HttpGet("{userId}/exists")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<ActionResult<ApiResponse<bool>>> CheckUserExists(string userId)
        {
            try
            {
                _logger.LogInformation("收到检查用户存在性请求, UserId: {UserId}", userId);

                if (string.IsNullOrWhiteSpace(userId))
                {
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.MissingParameter,
                        "用户ID不能为空"));
                }

                var exists = await _userService.ValidateUserExistsAsync(userId);

                _logger.LogInformation("用户存在性检查完成, UserId: {UserId}, Exists: {Exists}", userId, exists);

                return Ok(ApiResponse<bool>.Success(exists, "检查完成"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检查用户存在性时发生异常, UserId: {UserId}", userId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
    }
}*/
