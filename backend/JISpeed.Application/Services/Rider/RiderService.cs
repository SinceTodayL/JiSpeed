using JISpeed.Core.Entities.Rider;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Interfaces.IRepositories;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Core.Constants;
using JISpeed.Core.Exceptions;
using Microsoft.Extensions.Logging;

namespace JISpeed.Application.Services.Rider
{
    public class RiderService : IRiderService
    {
        private readonly IRiderRepository _riderRepository;
        private readonly ILogger<RiderService> _logger;

        public RiderService(IRiderRepository riderRepository, ILogger<RiderService> logger)
        {
            _riderRepository = riderRepository;
            _logger = logger;
        }
        /// 创建用户实体（当ApplicationUser的UserType=1时调用）
        
        /// <param name="applicationUser">已创建的ApplicationUser</param>
        /// <param name="nickname">用户昵称，默认使用用户名</param>
        /// <returns>创建的Rider实体</returns>
        public async Task<JISpeed.Core.Entities.Rider.Rider> CreateUserEntityAsync(ApplicationUser applicationUser, string? nickname = null)
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
            
                // 检查是否已存在关联的Rider实体
                //var existingUser = await _riderRepository.GetUserByApplicationUserIdAsync(applicationUser.Id);
                // if (existingUser != null)
                // {
                //     _logger.LogWarning("用户实体已存在, ApplicationUserId: {ApplicationUserId}", applicationUser.Id);
                //     throw new BusinessException("用户实体已存在");
                // }
            
                // 生成用户ID和昵称
                var userId = Guid.NewGuid().ToString("N");
                var userNickname = nickname ?? applicationUser.UserName ?? "用户" + userId.Substring(0, 8);
            
                // 创建User实体
                var user = new Core.Entities.Rider.Rider
                {
                    RiderId = userId,
                    Name = userNickname,    
                    PhoneNumber = "1234567890",
                    ApplicationUserId = applicationUser.Id
                };
                
                // 保存到数据库
                await _riderRepository.CreateAsync(user);
                await _riderRepository.SaveChangesAsync(); _logger.LogInformation("用户实体创建成功, UserId: {UserId}, ApplicationUserId: {ApplicationUserId}",
                    user.RiderId, applicationUser.Id);
            
                return user;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is BusinessException))
            {
                _logger.LogError(ex, "创建用户实体时发生异常, ApplicationUserId: {ApplicationUserId}",
                    applicationUser?.Id);
                throw new BusinessException("创建用户实体失败");
            }
        }

    }

}