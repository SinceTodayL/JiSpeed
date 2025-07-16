using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JISpeed.Core.Services;
using JISpeed.Api.Models;
using Microsoft.Extensions.Logging;

namespace JISpeed.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Id) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("用户ID和密码不能为空");
            }

            var success = await _userService.LoginAsync(request.Id, request.Password);
            if (!success)
            {
                _logger.LogWarning($"登录接口：失败，用户ID={request.Id}");
                return Unauthorized("登录失败，ID或密码错误");
            }

            _logger.LogInformation($"登录接口：成功，用户ID={request.Id}");
            return Ok("登录成功");
        }
    }
}