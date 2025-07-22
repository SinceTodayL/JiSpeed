using System.Net;
using JISpeed.Core.Configurations;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Interfaces.IRepositories;
using JISpeed.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using JISpeed.Infrastructure.Redis;
using Newtonsoft.Json;



namespace JISpeed.Application.Services.Common
{
    public class RegisterService : IRegisterService
    {
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterService> _logger;
        private readonly RedisService _redisService;

        public RegisterService(
            IEmailService emailService,
            ILogger<RegisterService> logger,
            UserManager<ApplicationUser> userManager,
            RedisService redisService)
        {
            _emailService = emailService;
            _logger = logger;
            _userManager = userManager;
            _redisService = redisService;
        }
        // 预注册
        public async Task<bool> PreRegisterUserAsync(ApplicationUser newUser, string passwordHash)
        {
            try
            {
                // 1. 参数验证
                var existingUser = await _userManager.FindByEmailAsync(newUser.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("用户已存在: {Email}", newUser.Email);
                    return false; // 避免抛出异常，用返回值表示失败
                }
                // 2. 发送验证邮箱
                var token =await _emailService.SendVerificationEmailAsync(newUser);
                if (token==null)
                {
                    _logger.LogError("验证邮件发送失败: {Email}", newUser.Email);
                    return false;
                }
                
                // 4. 构建预注册信息对象（包含加密后的密码）
                var preRegData = new PreRegistrationInfo
                {
                    User = newUser,
                    PasswordHash = passwordHash, // 存储哈希而非明文
                    token=token
                };
                
                // 5. 存入Redis，准备验证
                await _redisService.SetStringAsync(
                    key: $"pre_reg:{newUser.Id}",
                    value: JsonConvert.SerializeObject(preRegData),
                    expiry: TimeSpan.FromHours(1));
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "邮件发送失败");
                return false;
            }
        }

        // 正式注册：验证令牌并创建用户
        public async Task<bool> RegisterUserAsync(string userId, string token)
        {
            try
            {
                // 1. 参数验证
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("用户ID或令牌为空");
                    return false;
                }

                // 2. 检查Redis中是否存在预注册信息
                string redisKey = $"pre_reg:{userId}";
                bool keyExists = await _redisService.KeyExistsAsync(redisKey);
                if (!keyExists)
                {
                    _logger.LogWarning("预注册信息不存在或已过期: {UserId}", userId);
                    return false;
                }

                // 3. 从Redis读取并反序列化数据
                string preRegJson = await _redisService.GetStringAsync(redisKey);
                var preRegData = JsonConvert.DeserializeObject<PreRegistrationInfo>(preRegJson);
                if (preRegData == null)
                {
                    _logger.LogError("预注册信息解析失败: {UserId}", userId);
                    return false;
                }


                // 4. 解码令牌并验证（使用数据库用户对象）
                //token = WebUtility.UrlDecode(token);
                if(token != preRegData.token)
                {
                    // 输出具体错误原因（关键调试信息）
                    _logger.LogWarning("令牌验证失败，用户ID: {UserId}，token：{Token}",  userId, token);
                    return false;
                }

                // 5. 完善用户信息并保存到数据库
                var user = new ApplicationUser(
                    preRegData.User.Id,
                    preRegData.User.UserName,
                    preRegData.User.UserType,
                    preRegData.User.Email,
                    preRegData.User.CreatedAt,
                    preRegData.User.PhoneNumber);
                user.EmailConfirmed = true;
                user.PasswordHash = preRegData.PasswordHash; // 设置预注册时的密码哈希
                
                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    var createErrors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                    _logger.LogError("创建用户失败: {Errors}", createErrors);
                    return false;
                }
                
                // 7. 清理Redis中的临时数据
                await _redisService.KeyDeleteAsync(redisKey);
                _logger.LogInformation("用户注册成功");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "用户注册失败: {UserId}", userId);
                return false;
            }
        }
    }
}