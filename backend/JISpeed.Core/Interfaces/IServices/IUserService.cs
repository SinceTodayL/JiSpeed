using JISpeed.Core.Entities.User;
using JISpeed.Core.Interfaces.IRepositories;

namespace JISpeed.Core.Interfaces.IServices
{
    
    // 用户服务接口
    
    public interface IUserService
    {
        
        // 根据用户ID获取用户详细信息
        
        // <param name="userId">用户ID</param>
        // <returns>包含完整信息的用户实体</returns>
        Task<User?> GetUserDetailAsync(string userId);

        
        // 根据用户ID获取用户统计信息
        
        // <param name="userId">用户ID</param>
        // <returns>用户统计信息</returns>
        Task<UserStats> GetUserStatsAsync(string userId);

        
        // 验证用户是否存在
        
        // <param name="userId">用户ID</param>
        // <returns>用户是否存在</returns>
        Task<bool> ValidateUserExistsAsync(string userId);
    }
}
