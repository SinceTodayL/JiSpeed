using JISpeed.Core.Entities.User;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Interfaces.IRepositories;

namespace JISpeed.Core.Interfaces.IServices
{
    
    /// 用户服务接口
    
    public interface IUserService
    {
        
        /// 根据用户ID获取用户详细信息
        
        /// <param name="userId">用户ID</param>
        /// <returns>包含完整信息的用户实体</returns>
        Task<User?> GetUserDetailAsync(string userId);

        
        /// 根据用户ID获取用户统计信息
        
        /// <param name="userId">用户ID</param>
        /// <returns>用户统计信息</returns>
        Task<UserStats> GetUserStatsAsync(string userId);

        
        /// 验证用户是否存在
        
        /// <param name="userId">用户ID</param>
        /// <returns>用户是否存在</returns>
        Task<bool> ValidateUserExistsAsync(string userId);

        
        /// 创建用户实体（当ApplicationUser的UserType=1时调用）
        
        /// <param name="applicationUser">已创建的ApplicationUser</param>
        /// <param name="nickname">用户昵称，默认使用用户名</param>
        /// <returns>创建的User实体</returns>
        Task<User> CreateUserEntityAsync(ApplicationUser applicationUser, string? nickname = null);

        
        /// 根据ApplicationUser ID获取关联的User实体
        
        /// <param name="applicationUserId">ApplicationUser的ID</param>
        /// <returns>关联的User实体</returns>
        Task<User?> GetUserByApplicationUserIdAsync(string applicationUserId);
    }
}
