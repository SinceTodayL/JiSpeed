using Microsoft.AspNetCore.Mvc;
using JISpeed.Api.Models;
using JISpeed.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace JISpeed.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly UserManager<UUser> _userManager;
        public AuthController(ILogger<AuthController> logger, UserManager<UUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                _logger.LogWarning($"登录接口：失败，用户ID={request.UserName}");
                return Unauthorized("登录失败，ID不存在");
            }
            var result = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!result)
            {
                _logger.LogWarning($"登录接口：失败，用户ID={request.UserName}");
                return Unauthorized("登录失败，密码错误");
            }

            _logger.LogInformation($"登录接口：成功，用户ID={request.UserName}");
            return Ok("登录成功");
        }
        
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var newUser = new UUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = request.UserName
                
            };
            var result = await _userManager.CreateAsync(newUser, request.Password);
            _logger.LogInformation($"注册成功，用户userName: {request.UserName}");
            if (result.Succeeded)
            {
                // 可选：设置角色、邮箱确认等
                return Ok("注册成功");
            }
            
            return BadRequest(result.Errors);
        }
    }
    
}