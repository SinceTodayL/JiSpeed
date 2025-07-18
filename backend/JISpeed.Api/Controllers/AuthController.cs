using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JISpeed.Core.Services;
using JISpeed.Api.Models;
using Microsoft.Extensions.Logging;
using JISpeed.Infrastructure;
using JISpeed.Core.Entities;
using JISpeed.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace JISpeed.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly UserManager<UUser> _userManager;
        private readonly AppDbContext _db;
        public AuthController(ILogger<AuthController> logger, UserManager<UUser> userManager, AppDbContext db)
        {
            _logger = logger;
            _userManager = userManager;
            _db = db;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            var result = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!result)
            {
                _logger.LogWarning($"登录接口：失败，用户ID={request.UserName}");
                return Unauthorized("登录失败，ID或密码错误");
            }

            _logger.LogInformation($"登录接口：成功，用户ID={request.UserName}");
            return Ok("登录成功");
        }
        
        [HttpGet("login/test")]
        public async Task<IActionResult> GetAllUsers()
        {

            _logger.LogInformation("返回所有用户");
             var users = await _db.Users.ToListAsync();
             //var users = await _db.Users.Take(1).ToListAsync();

             return Ok(users);
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