using JISpeed.Core.Entities.Common;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Interfaces.IRepositories.Admin;
using JISpeed.Core.Interfaces.IRepositories.Merchant;
using JISpeed.Core.Interfaces.IRepositories.Rider;
using JISpeed.Core.Interfaces.IRepositories.User;
using JISpeed.Core.Interfaces.IServices;
using Microsoft.Extensions.Logging;

namespace JISpeed.Application.Services.Common
{
    public class LoginService :  ILoginService
    {
        private readonly IMerchantRepository _merchantRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRiderRepository _riderRepository;
        private readonly ILogger<LoginService> _logger;
        
        public LoginService(
            ILogger<LoginService> logger,
            IRiderRepository riderRepository,
            IMerchantRepository merchantRepository,
            IAdminRepository adminRepository,
            IUserRepository userRepository
        )
        {
            _logger = logger;
            _riderRepository = riderRepository;
            _merchantRepository = merchantRepository;
            _adminRepository = adminRepository;
            _userRepository = userRepository;
        }

        public async Task<string> GetBusinessEntityId(string applicationUserId,int userType)
        {
            try
            {
                _logger.LogInformation("开始获取实体ID, ApplicationUserId: {ApplicationUserId}", applicationUserId);

                string? id = null;
                if (userType == 1)
                {
                    var data = await _userRepository.GetByApplicationUserIdAsync(applicationUserId);
                    if (data != null)
                        id = data.UserId;
                }
                else if (userType == 2)
                {
                    var data = await _merchantRepository.GetByApplicationUserIdAsync(applicationUserId);
                    if (data != null) 
                        id = data.MerchantId;
                }
                else if (userType == 3)
                {
                    var data = await _riderRepository.GetByApplicationUserIdAsync(applicationUserId);
                    if (data != null)
                        id = data.RiderId;
                }
                else if (userType == 4)
                {
                    var data = await _adminRepository.GetByApplicationUserIdAsync(applicationUserId);
                    if (data != null)
                        id = data.AdminId;
                }

                if (id == null)
                    throw new Exception();
                _logger.LogInformation("成功获取实体ID, Id: {Id}", id);

                return id;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取实体ID时发生异常, ApplicationUserId: {ApplicationUserId}", applicationUserId);
                throw new BusinessException("获取实体ID失败");
            }
        }

        public bool IsLocked(ApplicationUser user)
        {
            if (user.Status == 1)
            {
                return true;
            }
            return false;
        }
    }
}