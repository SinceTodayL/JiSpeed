using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using JISpeed.Core.Entities.Common;
using Microsoft.AspNetCore.Identity;
using JISpeed.Api.DTOS;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Api.Common;
using JISpeed.Core.Constants;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Interfaces.IRepositories.Common;

namespace JISpeed.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRegistrationService _registerService;
        private readonly ILoginService _loginService;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public AuthController(ILogger<AuthController> logger,
            UserManager<ApplicationUser> userManager,
            IRegistrationService registerService,
            ILoginService loginService,
            IApplicationUserRepository applicationUserRepository,
            IEmailService emailService,
            IMapper mapper)
        {
            _logger = logger;
            _userManager = userManager;
            _registerService = registerService;
            _emailService = emailService;
            _loginService = loginService;
            _applicationUserRepository = applicationUserRepository;
            _mapper = mapper;
        }


        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<string>>> Login(int userType,[FromBody] UserLoginRequest request)
        {
            try
            {
                _logger.LogInformation("收到登录请求");
                if (request == null)
                {
                    _logger.LogWarning("请求体不能为空");
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.MissingParameter, 
                        "请求体不能为空，请提供登录信息"));
                }

                if (userType < 1 || userType > 4)
                {
                    _logger.LogWarning("userType 无效: {UserType}", userType);
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.ValidationFailed, 
                        "userType 必须为1-4"));
                }
                if ((string.IsNullOrEmpty(request.UserName) && string.IsNullOrEmpty(request.Email))||string.IsNullOrEmpty(request.PassWord))
                {
                    _logger.LogWarning("用户名或密码为空");
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.InvalidRequestFormat, 
                        "用户名和密码不能为空"));
                }

                ApplicationUser? user;
                string? key;
                if (request.Email != null)
                {
                    user = await _applicationUserRepository.GetByEmailAndUserTypeAsync(request.Email, userType);
                    key = request.Email;
                }
                else if (request.UserName != null)
                {
                    user = await _userManager.FindByNameAsync(request.UserName);
                    key = request.UserName;
                }
                else
                {
                    _logger.LogWarning("登录失败，缺少参数");
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.MissingParameter,
                        "ID不能为空"));
                }

                if (user == null)
                {
                    _logger.LogWarning("登录失败，用户不存在");
                    return Unauthorized(ApiResponse<object>.Fail(
                        ErrorCodes.InvalidCredentials,
                        "用户不存在"));
                }

                var result = await _userManager.CheckPasswordAsync(user, request.PassWord);
                if (!result)
                {
                    _logger.LogWarning($"登录接口：失败，登录名{key}");
                    return Unauthorized(ApiResponse<object>.Fail(
                        ErrorCodes.InvalidCredentials,
                        "登录失败，密码错误"));
                }
                _logger.LogInformation("成功获取对象，尝试登录");
                var isLockedOut =  _loginService.IsLocked(user);
                if (!isLockedOut)
                {
                    _logger.LogWarning("登录失败，账号被封禁");
                    return StatusCode(403,ApiResponse<object>.Fail(
                        ErrorCodes.Forbidden,
                        "用户被封禁"
                    ));
                }
                var id = await _loginService.GetBusinessEntityId(user.Id, userType);
                _logger.LogInformation($"登录接口：成功，登录名{key}");

                return Ok(ApiResponse<string>.Success(id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }

        // 用户前端填入预注册表单，后端发送验证邮件
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<bool>>> Register(int userType,[FromBody] UserRegisterRequest request)
        {

            try
            {
                // 2. 检查邮箱格式
                var res = await _emailService.IsValidEmailAsync(request.Email);
                if (!res)
                {
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.ValidationFailed, 
                        "邮箱格式无效"));
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
                    throw new Exception();
                }

                _logger.LogInformation("预注册成功，已发送验证邮件，用户名: {UserName}", request.UserName);
                return Ok(ApiResponse<bool>.Success(true,"验证邮件已发送，请在1小时内点击链接完成注册"));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }



        [HttpGet("verify-email")]
        public async Task<ActionResult<ApiResponse<bool>>> VerifyEmail(string userId, string token)
        {
            try
            {
                _logger.LogInformation($"收到验证请求: {userId}");
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userId))
                {
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.MissingParameter, 
                        "userId或token为空"));
                }
                var result = await _registerService.RegisterUserAsync(userId, token);
                if (!result.IsSuccess)
                {
                    _logger.LogInformation($"创建失败，用户userId: {userId}");
                    return Unauthorized(ApiResponse<object>.Fail(
                        ErrorCodes.InvalidCredentials,
                        "token不存在，或已过期"));
                }
                return Ok(ApiResponse<bool>.Success(true,"注册成功!"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
    }
}
    