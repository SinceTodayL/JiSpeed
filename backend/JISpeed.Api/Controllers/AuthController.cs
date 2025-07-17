using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JISpeed.Core.Services;
using JISpeed.Api.Models;
using Microsoft.Extensions.Logging;
using JISpeed.Infrastructure;
using JISpeed.Core.Entities;
using JISpeed.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace JISpeed.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;
        private readonly AppDbContext _db;
        public AuthController(IUserService userService, ILogger<AuthController> logger, AppDbContext db)
        {
            _userService = userService;
            _logger = logger;
            _db = db;
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
        
        [HttpGet("login/test")]
        public async Task<IActionResult> GetAllUsers()
        {
             var users = await _userService.GetAllUsersAsync();
             return Ok(users);
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            //
            _logger.LogWarning($"失败");
            
            // 检查用户是否已存在
            var success = await _userService.RegiAsync(request.Id, request.Password, request.Role);
            if (!success)
            {
                _logger.LogWarning($"注册接口：失败，用户ID={request.Id}");
                return Unauthorized("注册失败，ID存在");
            }
            
            // 创建新的用户
            var newUser = new UUser
            {
                Id = request.Id,
                Password = request.Password, // 明文保存（后续加密）
                Role = request.Role
            };
            
            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();

            return Ok("注册成功");
        }
    }
    
}