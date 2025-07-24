
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Interfaces.IRepositories;
using JISpeed.Core.Interfaces.IServices;
using Microsoft.Extensions.Logging;

namespace JISpeed.Application.Services.Common
{
    public class AppUserService : IAppUserService
    {
        private readonly IAppUserRepository _appUserRepository;
        private readonly ILogger<AppUserService> _logger;
        
        public AppUserService(IAppUserRepository repository, ILogger<AppUserService> logger)
        {
            _appUserRepository = repository;
            _logger = logger;
        }

        public async Task<ApplicationUser?> FindUserAsync(string email, int userType)
        {
            try
            {
                _logger.LogInformation("开始查找目标用户");
                var res = await _appUserRepository.FindByEmailAndTypeAsync(email, userType);
                if (res == null)
                    _logger.LogInformation("无该用户");
                else
                    _logger.LogInformation("该用户已经创建");
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查询业务实体时发生异常, UserType: {UserType}, Email: {Email}",
                    userType, email);
                return null;
            }
        }


    }
}