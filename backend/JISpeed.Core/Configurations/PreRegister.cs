using JISpeed.Core.Entities.Common;

// 预注册信息结构
namespace JISpeed.Core.Configurations
{
    public class PreRegistrationInfo
    {
        public required ApplicationUser User { get; set; }
        public required string PasswordHash { get; set; } // 存储加密后的密码哈希
        public required string Token { get; set; }
    }
}
