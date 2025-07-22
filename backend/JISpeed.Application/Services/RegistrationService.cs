using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Constants;

namespace JISpeed.Application.Services
{
    
    /// 注册服务实现
    
    public class RegistrationService : IRegistrationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        private readonly ILogger<RegistrationService> _logger;

        public RegistrationService(
            UserManager<ApplicationUser> userManager,
            IUserService userService,
            ILogger<RegistrationService> logger)
        {
            _userManager = userManager;
            _userService = userService;
            _logger = logger;
        }

        
        /// 注册用户并创建对应的实体
        
        public async Task<RegistrationResult> RegisterUserAsync(string userName, string password, int userType,
            string? email = null, string? nickname = null)
        {
            try
            {
                _logger.LogInformation("开始注册用户, UserName: {UserName}, UserType: {UserType}", userName, userType);

                // 参数验证
                if (string.IsNullOrWhiteSpace(userName))
                {
                    return RegistrationResult.Failure("用户名不能为空");
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    return RegistrationResult.Failure("密码不能为空");
                }

                if (userType < 1 || userType > 4)
                {
                    return RegistrationResult.Failure("用户类型无效");
                }

                // 检查用户名是否已存在
                var existingUser = await _userManager.FindByNameAsync(userName);
                if (existingUser != null)
                {
                    return RegistrationResult.Failure("用户名已存在");
                }

                // 检查邮箱是否已存在（如果提供了邮箱）
                if (!string.IsNullOrWhiteSpace(email))
                {
                    var existingEmailUser = await _userManager.FindByEmailAsync(email);
                    if (existingEmailUser != null)
                    {
                        return RegistrationResult.Failure("邮箱已被使用");
                    }
                }

                // 创建ApplicationUser
                var applicationUser = new ApplicationUser(userName, userType, email);
                var result = await _userManager.CreateAsync(applicationUser, password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToArray();
                    _logger.LogWarning("ApplicationUser创建失败, UserName: {UserName}, Errors: {Errors}",
                        userName, string.Join(", ", errors));
                    return RegistrationResult.Failure(errors);
                }

                _logger.LogInformation("ApplicationUser创建成功, UserId: {UserId}, UserName: {UserName}",
                    applicationUser.Id, userName);

                // 根据UserType创建对应的业务实体
                var additionalData = new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(nickname))
                {
                    additionalData["nickname"] = nickname;
                }

                var businessEntityCreated = await CreateBusinessEntityAsync(applicationUser, additionalData);
                if (!businessEntityCreated)
                {
                    // 如果业务实体创建失败，删除已创建的ApplicationUser
                    await _userManager.DeleteAsync(applicationUser);
                    return RegistrationResult.Failure("创建用户信息失败");
                }

                // 获取业务实体ID
                var businessEntityId = await GetBusinessEntityIdAsync(applicationUser);

                _logger.LogInformation("用户注册成功, UserName: {UserName}, ApplicationUserId: {ApplicationUserId}, BusinessEntityId: {BusinessEntityId}",
                    userName, applicationUser.Id, businessEntityId);

                return RegistrationResult.Success(applicationUser.Id, businessEntityId ?? "");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "用户注册时发生异常, UserName: {UserName}", userName);
                return RegistrationResult.Failure("注册失败，请稍后重试");
            }
        }

        
        /// 根据UserType创建对应的业务实体
        
        public async Task<bool> CreateBusinessEntityAsync(ApplicationUser applicationUser, Dictionary<string, object>? additionalData = null)
        {
            try
            {
                _logger.LogInformation("开始创建业务实体, UserType: {UserType}, ApplicationUserId: {ApplicationUserId}",
                    applicationUser.UserType, applicationUser.Id);

                switch (applicationUser.UserType)
                {
                    case 1: // 普通用户
                        var nickname = additionalData?.ContainsKey("nickname") == true
                            ? additionalData["nickname"].ToString()
                            : null;
                        await _userService.CreateUserEntityAsync(applicationUser, nickname);
                        break;

                    case 2: // 商家
                        // TODO: 实现商家实体创建
                        _logger.LogInformation("商家实体创建功能待实现");
                        break;

                    case 3: // 骑手
                        // TODO: 实现骑手实体创建
                        _logger.LogInformation("骑手实体创建功能待实现");
                        break;

                    case 4: // 管理员
                        // TODO: 实现管理员实体创建
                        _logger.LogInformation("管理员实体创建功能待实现");
                        break;

                    default:
                        _logger.LogWarning("未知的用户类型: {UserType}", applicationUser.UserType);
                        return false;
                }

                _logger.LogInformation("业务实体创建成功, UserType: {UserType}, ApplicationUserId: {ApplicationUserId}",
                    applicationUser.UserType, applicationUser.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建业务实体时发生异常, UserType: {UserType}, ApplicationUserId: {ApplicationUserId}",
                    applicationUser.UserType, applicationUser.Id);
                return false;
            }
        }

        
        /// 获取业务实体ID
        
        private async Task<string?> GetBusinessEntityIdAsync(ApplicationUser applicationUser)
        {
            try
            {
                switch (applicationUser.UserType)
                {
                    case 1: // 普通用户
                        var user = await _userService.GetUserByApplicationUserIdAsync(applicationUser.Id);
                        return user?.UserId;

                    case 2: // 商家
                        // TODO: 实现获取商家ID
                        break;

                    case 3: // 骑手
                        // TODO: 实现获取骑手ID
                        break;

                    case 4: // 管理员
                        // TODO: 实现获取管理员ID
                        break;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取业务实体ID时发生异常, ApplicationUserId: {ApplicationUserId}", applicationUser.Id);
                return null;
            }
        }
    }
}
