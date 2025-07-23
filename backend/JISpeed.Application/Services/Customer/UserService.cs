using JISpeed.Core.Entities.User;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Interfaces.IRepositories;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Constants;
using Microsoft.Extensions.Logging;

namespace JISpeed.Application.Services.Customer
{

    // 用户服务实现

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }


        // 根据用户ID获取用户详细信息

        // <param name="userId">用户ID</param>
        // <returns>包含完整信息的用户实体</returns>
        public async Task<User?> GetUserDetailAsync(string userId)
        {
            try
            {
                _logger.LogInformation("开始获取用户详细信息, UserId: {UserId}", userId);

                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogWarning("用户ID为空");
                    throw new ValidationException("用户ID不能为空");
                }

                var user = await _userRepository.GetUserWithDetailsAsync(userId);

                if (user == null)
                {
                    _logger.LogWarning("用户不存在, UserId: {UserId}", userId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"用户不存在，ID: {userId}");
                }

                _logger.LogInformation("成功获取用户详细信息, UserId: {UserId}, Nickname: {Nickname}",
                    userId, user.Nickname);

                return user;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取用户详细信息时发生异常, UserId: {UserId}", userId);
                throw new BusinessException("获取用户信息失败");
            }
        }


        // 根据用户ID获取用户统计信息

        // <param name="userId">用户ID</param>
        // <returns>用户统计信息</returns>
        public async Task<UserStats> GetUserStatsAsync(string userId)
        {
            try
            {
                _logger.LogInformation("开始获取用户统计信息, UserId: {UserId}", userId);

                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogWarning("用户ID为空");
                    throw new ValidationException("用户ID不能为空");
                }

                // 先验证用户是否存在
                var userExists = await _userRepository.ExistsAsync(userId);
                if (!userExists)
                {
                    _logger.LogWarning("用户不存在, UserId: {UserId}", userId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"用户不存在，ID: {userId}");
                }

                var stats = await _userRepository.GetUserStatsAsync(userId);

                _logger.LogInformation("成功获取用户统计信息, UserId: {UserId}", userId);

                return stats;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取用户统计信息时发生异常, UserId: {UserId}", userId);
                throw new BusinessException("获取用户统计信息失败");
            }
        }


        // 验证用户是否存在

        // <param name="userId">用户ID</param>
        // <returns>用户是否存在</returns>
        public async Task<bool> ValidateUserExistsAsync(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return false;
                }

                return await _userRepository.ExistsAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "验证用户存在性时发生异常, UserId: {UserId}", userId);
                return false;
            }
        }

        
        /// 创建用户实体（当ApplicationUser的UserType=1时调用）
        
        /// <param name="applicationUser">已创建的ApplicationUser</param>
        /// <param name="nickname">用户昵称，默认使用用户名</param>
        /// <returns>创建的User实体</returns>
        public async Task<User> CreateUserEntityAsync(ApplicationUser applicationUser, string? nickname = null)
        {
            try
            {
                _logger.LogInformation("开始创建用户实体, ApplicationUserId: {ApplicationUserId}", applicationUser.Id);

                // 参数验证
                if (applicationUser == null)
                {
                    throw new ValidationException("ApplicationUser不能为空");
                }

                if (applicationUser.UserType != 1)
                {
                    throw new ValidationException($"UserType必须为1，当前值: {applicationUser.UserType}");
                }

                // 检查是否已存在关联的User实体
                var existingUser = await _userRepository.GetUserByApplicationUserIdAsync(applicationUser.Id);
                if (existingUser != null)
                {
                    _logger.LogWarning("用户实体已存在, ApplicationUserId: {ApplicationUserId}", applicationUser.Id);
                    throw new BusinessException("用户实体已存在");
                }

                // 生成用户ID和昵称
                var userId = Guid.NewGuid().ToString("N");
                var userNickname = nickname ?? applicationUser.UserName ?? "用户" + userId.Substring(0, 8);
 
                // 创建User实体
                var user = new User
                {
                    UserId = userId,
                    Nickname = userNickname,     
                    ApplicationUserId = applicationUser.Id
                };
                
                // 保存到数据库
                await _userRepository.CreateAsync(user);
                await _userRepository.SaveChangesAsync(); _logger.LogInformation("用户实体创建成功, UserId: {UserId}, ApplicationUserId: {ApplicationUserId}",
                    user.UserId, applicationUser.Id);

                return user;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is BusinessException))
            {
                _logger.LogError(ex, "创建用户实体时发生异常, ApplicationUserId: {ApplicationUserId}",
                    applicationUser?.Id);
                throw new BusinessException("创建用户实体失败");
            }
        }

        
        /// 根据ApplicationUser ID获取关联的User实体
        
        /// <param name="applicationUserId">ApplicationUser的ID</param>
        /// <returns>关联的User实体</returns>
        public async Task<User?> GetUserByApplicationUserIdAsync(string applicationUserId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(applicationUserId))
                {
                    return null;
                }

                return await _userRepository.GetUserByApplicationUserIdAsync(applicationUserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "根据ApplicationUser ID获取用户实体时发生异常, ApplicationUserId: {ApplicationUserId}",
                    applicationUserId);
                return null;
            }
        }
    }
}