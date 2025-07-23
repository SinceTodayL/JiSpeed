using Microsoft.AspNetCore.Mvc;
using JISpeed.Core.Entities.Common;
using Microsoft.AspNetCore.Identity;
using JISpeed.Api.DTOS;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Api.Common;

namespace JISpeed.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRegistrationService _registerService;
        private readonly IEmailService _emailService;

        public AuthController(ILogger<AuthController> logger,
            UserManager<ApplicationUser> userManager,
            IRegistrationService registerService,
            IEmailService emailService)
        {
            _logger = logger;
            _userManager = userManager;
            _registerService = registerService;
            _emailService = emailService;
        }


        [HttpPost("login")]
        public async Task<ApiResponse> Login([FromBody] UserLoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning($"登录接口：失败，用户邮箱={request.Email}");
                return ApiResponse.Fail(3002,"登录失败，用户不存在");
            }

            var result = await _userManager.CheckPasswordAsync(user, request.PassWord);
            if (!result)
            {
                _logger.LogWarning($"登录接口：失败，用户邮箱={request.Email}");
                return ApiResponse.Fail(3007,"登录失败，密码错误");
            }

            _logger.LogInformation($"登录接口：成功，用户邮箱={request.Email}");
            return ApiResponse.Success("登录成功！");
        }

        // 用户前端填入预注册表单，后端发送验证邮件
        [HttpPost("register")]
        public async Task<ApiResponse> Register(int userType,[FromBody] UserRegisterRequest request)
        {
            // 1. 验证请求模型有效性
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                _logger.LogWarning("注册请求参数无效: {Errors}", string.Join(", ", errors));
                return ApiResponse.Fail(1003,"输入信息有误");
            }

            try
            {
                // 2. 检查邮箱格式
                var res = await _emailService.IsValidEmailAsync(request.Email);
                if (!res)
                {
                    return ApiResponse.Fail(3008,"邮箱格式无效");
                }

                // 3. 创建用户对象
                var newUser = new ApplicationUser(
                    id: Guid.NewGuid().ToString("N"),
                    userName: request.UserName,
                    userType: userType,
                    email: request.Email,
                    createdAt: DateTime.UtcNow,
                    phoneNumber: request.PhoneNumber);
                // 4. 生成密码哈希（在控制器层处理，避免服务层接触明文）
                string passwordHash = _userManager.PasswordHasher.HashPassword(newUser, request.PassWord);

                // 5. 调用预注册服务
                var result = await _registerService.PreRegisterUserAsync(newUser, passwordHash);


                if (!result.IsSuccess)
                {
                    _logger.LogWarning("预注册失败，用户名: {UserName}", request.UserName);
                    return ApiResponse.Fail(3001,"注册失败，请检查邮箱是否已被使用");
                }

                _logger.LogInformation("预注册成功，已发送验证邮件，用户名: {UserName}", request.UserName);
                return ApiResponse.Success("验证邮件已发送，请在1小时内点击链接完成注册");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "注册接口异常，用户名: {UserName}", request.UserName);
                return ApiResponse.Fail(5000,"服务器处理失败，请稍后重试" );
            }
        }



        [HttpGet("verify-email")]
        public async Task<ApiResponse> VerifyEmail(string userId, string token)
        {
            _logger.LogInformation($"收到验证: {userId}");
            var result = await _registerService.RegisterUserAsync(userId, token);
            if (!result.IsSuccess)
            {
                _logger.LogInformation($"创建失败，用户userId: {userId}");
                return ApiResponse.Fail(5001,"服务器处理失败，请稍后重试" );
            }
            return ApiResponse.Success("注册成功!");
        }
    }
}
    