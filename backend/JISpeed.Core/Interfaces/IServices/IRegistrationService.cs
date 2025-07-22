using JISpeed.Core.Entities.Common;

namespace JISpeed.Core.Interfaces.IServices
{
    
    /// 注册服务接口 - 处理用户注册相关的业务逻辑
    
    public interface IRegistrationService
    {
        
        /// 注册用户并创建对应的实体
        
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="userType">用户类型：1-普通用户, 2-商家, 3-骑手, 4-管理员</param>
        /// <param name="email">邮箱（可选）</param>
        /// <param name="nickname">昵称（可选，仅对普通用户有效）</param>
        /// <returns>注册结果</returns>
        Task<RegistrationResult> RegisterUserAsync(string userName, string password, int userType,
            string? email = null, string? nickname = null);

        
        /// 根据UserType创建对应的业务实体
        
        /// <param name="applicationUser">已创建的ApplicationUser</param>
        /// <param name="additionalData">额外数据（如昵称等）</param>
        /// <returns>创建结果</returns>
        Task<bool> CreateBusinessEntityAsync(ApplicationUser applicationUser, Dictionary<string, object>? additionalData = null);
    }

    
    /// 注册结果
    
    public class RegistrationResult
    {
        
        /// 是否成功
        
        public bool IsSuccess { get; set; }

        
        /// 错误信息
        
        public List<string> Errors { get; set; } = new List<string>();

        
        /// ApplicationUser ID
        
        public string? ApplicationUserId { get; set; }

        
        /// 业务实体ID（如User的UserId）
        
        public string? BusinessEntityId { get; set; }

        
        /// 成功的注册结果
        
        public static RegistrationResult Success(string applicationUserId, string businessEntityId)
        {
            return new RegistrationResult
            {
                IsSuccess = true,
                ApplicationUserId = applicationUserId,
                BusinessEntityId = businessEntityId
            };
        }

        
        /// 失败的注册结果
        
        public static RegistrationResult Failure(params string[] errors)
        {
            return new RegistrationResult
            {
                IsSuccess = false,
                Errors = errors.ToList()
            };
        }
    }
}
