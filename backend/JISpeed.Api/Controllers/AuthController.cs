using Microsoft.AspNetCore.Mvc;
using JISpeed.Core.Entities;
using JISpeed.Core.Entities.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using JISpeed.Api.Models;

namespace JISpeed.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        public AuthController(ILogger<AuthController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
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
        
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
        {
            var newUser = new ApplicationUser(request.UserName, request.UserType);
            var result = await _userManager.CreateAsync(newUser, request.PassWord);
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