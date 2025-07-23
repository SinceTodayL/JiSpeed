using JISpeed.Core.Configurations;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using JISpeed.Infrastructure.Redis;
using Newtonsoft.Json;


namespace JISpeed.Application.Services.Common
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        private readonly ILogger<RegistrationService> _logger;
        private readonly RedisService _redisService;
        private readonly IRiderService _riderService;
        private readonly IMerchantService _merchantService;
        private readonly IAdminService _adminService;

        public RegistrationService(
            IEmailService emailService,
            ILogger<RegistrationService> logger,
            IUserService userService,
            UserManager<ApplicationUser> userManager,
            RedisService redisService,
            IRiderService riderService,
            IMerchantService merchantService,
            IAdminService adminService
            )
        {
            _emailService = emailService;
            _logger = logger;
            _userManager = userManager;
            _redisService = redisService;
            _userService = userService;
            _riderService = riderService;
            _merchantService = merchantService;
            _adminService = adminService;
        }

        // 预注册
        public async Task<PreRegistrationResult> PreRegisterUserAsync(ApplicationUser newUser, string passwordHash)
        {
            try
            {
                // 1. 参数验证
                var existingUser = await _userManager.FindByEmailAsync(newUser.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("用户已存在: {Email}", newUser.Email);
                    return PreRegistrationResult.Failure("用户存在");
                }

                // 2. 发送验证邮箱
                var token = await _emailService.SendVerificationEmailAsync(newUser);
                if (token == null)
                {
                    _logger.LogError("验证邮件发送失败: {Email}", newUser.Email);
                    return PreRegistrationResult.Failure("邮件发送失败");
                }

                // 4. 构建预注册信息对象（包含加密后的密码）
                var preRegData = new PreRegistrationInfo
                {
                    User = newUser,
                    PasswordHash = passwordHash, // 存储哈希而非明文
                    token = token
                };

                // 5. 存入Redis，准备验证
                await _redisService.SetStringAsync(
                    key: $"pre_reg:{newUser.Id}",
                    value: JsonConvert.SerializeObject(preRegData),
                    expiry: TimeSpan.FromHours(1));
                return PreRegistrationResult.Success(newUser.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "邮件发送失败");
                return PreRegistrationResult.Failure("邮件发送失败,稍后再试");
            }
        }

        // 正式注册：验证令牌并创建用户
        public async Task<RegistrationResult> RegisterUserAsync(string userId, string token)
        {
            try
            {
                // 1. 参数验证
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("用户ID或令牌为空");
                    return RegistrationResult.Failure("用户名不能为空");
                }

                // 2. 检查Redis中是否存在预注册信息
                string redisKey = $"pre_reg:{userId}";
                bool keyExists = await _redisService.KeyExistsAsync(redisKey);
                if (!keyExists)
                {
                    _logger.LogWarning("预注册信息不存在或已过期: {UserId}", userId);
                    return RegistrationResult.Failure("验证超时");
                }

                // 3. 从Redis读取并反序列化数据
                string preRegJson = await _redisService.GetStringAsync(redisKey);
                var preRegData = JsonConvert.DeserializeObject<PreRegistrationInfo>(preRegJson);
                if (preRegData == null)
                {
                    _logger.LogError("预注册信息解析失败: {UserId}", userId);
                    return RegistrationResult.Failure("验证失败");
                }


                // 4. 解码令牌并验证（使用数据库用户对象）
                //token = WebUtility.UrlDecode(token);
                if (token != preRegData.token)
                {
                    // 输出具体错误原因（关键调试信息）
                    _logger.LogWarning("令牌验证失败，用户ID: {UserId}，token：{Token}", userId, token);
                    return RegistrationResult.Failure("令牌验证失败");
                }
                // 5. 完善用户信息并保存到数据库
                _logger.LogInformation("查询用户");
               
                
                var user = new ApplicationUser
                {
                    Id = userId, 
                    Email = preRegData.User.Email,
                    UserName = preRegData.User.UserName,
                    UserType = preRegData.User.UserType,
                    CreatedAt = preRegData.User.CreatedAt,
                    PhoneNumber = preRegData.User.PhoneNumber
                };
                
                user.EmailConfirmed = true;
                user.PasswordHash = preRegData.PasswordHash; // 设置预注册时的密码哈希

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    var createErrors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                    _logger.LogError("创建用户失败: {Errors}", createErrors);
                    return RegistrationResult.Failure("创建用户失败");
                }

                // 6.在关联表中创建对应的用户
                var businessEntityCreated = await CreateBusinessEntityAsync(user);
                if (!businessEntityCreated)
                {
                    // 如果业务实体创建失败，删除已创建的ApplicationUser
                    await _userManager.DeleteAsync(user);                    
                    return RegistrationResult.Failure("创建用户信息失败");
                }

                // 7. 清理Redis中的临时数据
                await _redisService.KeyDeleteAsync(redisKey);
                _logger.LogInformation("用户注册成功");
                return RegistrationResult.Success(user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "用户注册失败: {UserId}", userId);
                return RegistrationResult.Failure("注册失败，请稍后重试");
            }
        }


        public async Task<bool> CreateBusinessEntityAsync(ApplicationUser user)
        {
            try
            {
                _logger.LogInformation("开始创建业务实体, UserType: {UserType}, ApplicationUserId: {UserId}",
                    user.UserType, user.Id);
                // 顾客
                if (user.UserType == 1)
                {
                    await _userService.CreateUserEntityAsync(user);
                }
                // 商家
                else if (user.UserType == 2)
                {
                    await _merchantService.CreateUserEntityAsync(user);
                }
                // 骑手
                else if (user.UserType == 3)
                {
                    await _riderService.CreateUserEntityAsync(user);
                }
                // 管理员
                else if (user.UserType == 4)
                {
                    await _adminService.CreateUserEntityAsync(user);
                }
                else
                {
                    _logger.LogWarning("未知的用户类型: {UserType}", user.UserType);
                    return false;
                }

                _logger.LogInformation("业务实体创建成功, UserType: {UserType}, ApplicationUserId: {UserId}",
                    user.UserType, user.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建业务实体时发生异常, UserType: {UserType}, ApplicationUserId: {UserId}",
                    user.UserType, user.Id);
                return false;
            }
            
        }
    }

}