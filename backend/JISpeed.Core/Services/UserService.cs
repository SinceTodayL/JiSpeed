using System.Threading.Tasks;
using JISpeed.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace JISpeed.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<bool> LoginAsync(string id, string password)
        {
            _logger.LogInformation($"尝试登录，用户ID: {id}");
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning($"登录失败，用户不存在，ID: {id}");
                return false;
            }
            if (user.Password != password)
            {
                _logger.LogWarning($"登录失败，密码错误，ID: {id}");
                return false;
            }

            _logger.LogInformation($"登录成功，用户ID: {id}");
            return true;
        }
    }
}