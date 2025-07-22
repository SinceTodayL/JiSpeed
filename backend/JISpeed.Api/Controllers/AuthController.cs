using Microsoft.AspNetCore.Mvc;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Api.Common;
using JISpeed.Api.Models;
using JISpeed.Core.Constants;
using Microsoft.AspNetCore.Identity;

namespace JISpeed.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRegistrationService _registrationService;

        public AuthController(
            ILogger<AuthController> logger,
            UserManager<ApplicationUser> userManager,
            IRegistrationService registrationService)
        {
            _logger = logger;
            _userManager = userManager;
            _registrationService = registrationService;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                _logger.LogWarning($"登录接口：失败，用户ID={request.UserName}");
                return Unauthorized("登录失败，ID不存在");
            }
            var result = await _userManager.CheckPasswordAsync(user, request.PassWord);
            if (!result)
            {
                _logger.LogWarning($"登录接口：失败，用户ID={request.UserName}");
                return Unauthorized("登录失败，密码错误");
            }

            _logger.LogInformation($"登录接口：成功，用户ID={request.UserName}");
            return Ok("登录成功");
        }


        
        /// 用户注册
        
        /// <param name="request">注册请求</param>
        /// <returns>注册结果</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<object>>> Register([FromBody] UserRegisterRequest request)
        {
            try
            {
                _logger.LogInformation("收到用户注册请求, UserName: {UserName}, UserType: {UserType}",
                    request.UserName, request.UserType);

                // 使用注册服务进行注册
                var result = await _registrationService.RegisterUserAsync(
                    request.UserName,
                    request.PassWord,
                    request.UserType,
                    request.Email,
                    request.Nickname);

                if (result.IsSuccess)
                {
                    _logger.LogInformation("用户注册成功, UserName: {UserName}, ApplicationUserId: {ApplicationUserId}, BusinessEntityId: {BusinessEntityId}",
                        request.UserName, result.ApplicationUserId, result.BusinessEntityId);

                    var responseData = new
                    {
                        ApplicationUserId = result.ApplicationUserId,
                        BusinessEntityId = result.BusinessEntityId,
                        UserType = request.UserType
                    };

                    return Ok(ApiResponse<object>.Success(responseData, "注册成功"));
                }
                else
                {
                    _logger.LogWarning("用户注册失败, UserName: {UserName}, Errors: {Errors}",
                        request.UserName, string.Join(", ", result.Errors));

                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.ValidationFailed,
                        string.Join("; ", result.Errors)));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "用户注册时发生异常, UserName: {UserName}", request.UserName);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "注册失败，请稍后重试"));
            }
        }
    }

}