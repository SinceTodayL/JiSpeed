using Microsoft.AspNetCore.Mvc;
using JISpeed.Core.Entities.Common;
using Microsoft.AspNetCore.Identity;
using JISpeed.Api.DTOS;
using JISpeed.Core.Interfaces.IServices;

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
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Emil);
            if (user == null)
            {
                _logger.LogWarning($"登录接口：失败，用户邮箱={request.Emil}");
                return Unauthorized("登录失败，用户不存在");
            }

            var result = await _userManager.CheckPasswordAsync(user, request.PassWord);
            if (!result)
            {
                _logger.LogWarning($"登录接口：失败，用户邮箱={request.Emil}");
                return Unauthorized("登录失败，密码错误");
            }

            _logger.LogInformation($"登录接口：成功，用户邮箱={request.Emil}");
            return Ok("登录成功");
        }

        // 用户前端填入预注册表单，后端发送验证邮件
        [HttpPost("register")]
        public async Task<IActionResult> Register(int userType,[FromBody] UserRegisterRequest request)
        {
            // 1. 验证请求模型有效性
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                _logger.LogWarning("注册请求参数无效: {Errors}", string.Join(", ", errors));
                return BadRequest(new { message = "输入信息有误", errors });
            }

            try
            {
                // 2. 检查邮箱格式
                var res = await _emailService.IsValidEmail(request.Email);
                if (!res)
                {
                    return BadRequest(new { message = "邮箱格式无效" });
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
                    return BadRequest(new { message = "注册失败，请检查邮箱是否已被使用" });
                }

                _logger.LogInformation("预注册成功，已发送验证邮件，用户名: {UserName}", request.UserName);
                //newUser.Id = "";
                return Ok(new
                {
                    message = "验证邮件已发送，请在1小时内点击链接完成注册",
                    email = request.Email // 提示用户查看的邮箱
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "注册接口异常，用户名: {UserName}", request.UserName);
                return StatusCode(500, new { message = "服务器处理失败，请稍后重试" });
            }
        }



        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string userId, string token)
        {
            _logger.LogInformation($"收到验证: {userId}");
            var result = await _registerService.RegisterUserAsync(userId, token);
            if (!result.IsSuccess)
            {
                _logger.LogInformation($"创建失败，用户userId: {userId}");
                return BadRequest("创建失败");
            }

            return Ok("注册成功");
        }
    }
}
    