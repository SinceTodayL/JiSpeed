using JISpeed.Core.Entities.Admin;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Interfaces.IRepositories;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Core.Constants;
using JISpeed.Core.Exceptions;
using Microsoft.Extensions.Logging;

namespace JISpeed.Application.Services.Admin
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ILogger<AdminService> _logger;

        public AdminService(IAdminRepository adminRepository, ILogger<AdminService> logger)
        {
            _adminRepository = adminRepository;
            _logger = logger;
        }
        /// 创建用户实体（当ApplicationUser的UserType=4时调用）
        
        /// <param name="applicationUser">已创建的ApplicationUser</param>
        /// <param name="nickname">用户昵称，默认使用用户名</param>
        /// <returns>创建的Admin实体</returns>
        public async Task<JISpeed.Core.Entities.Admin.Admin> CreateUserEntityAsync(ApplicationUser applicationUser, string? nickname = null)
        {
            try
            {
                _logger.LogInformation("开始创建用户实体, ApplicationUserId: {ApplicationUserId}", applicationUser.Id);
            
                // 参数验证
                if (applicationUser == null)
                {
                    throw new ValidationException("ApplicationUser不能为空");
                }
            
                if (applicationUser.UserType != 4)
                {
                    throw new ValidationException($"UserType必须为4，当前值: {applicationUser.UserType}");
                }
            
                // 检查是否已存在关联的Admin实体
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
                var user = new Core.Entities.Admin.Admin
                {
                    AdminId = userId,
                    PermissionLevel = "",
                    ApplicationUserId = applicationUser.Id
                };
                
                // 保存到数据库
                await _adminRepository.CreateAsync(user);
                await _adminRepository.SaveChangesAsync(); _logger.LogInformation("用户实体创建成功, UserId: {UserId}, ApplicationUserId: {ApplicationUserId}",
                    user.AdminId, applicationUser.Id);
            
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