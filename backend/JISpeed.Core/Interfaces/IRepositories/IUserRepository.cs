using JISpeed.Core.Entities.User;

namespace JISpeed.Core.Interfaces.IRepositories
{
    
    // 用户仓储接口
    
    public interface IUserRepository
    {
        
        // 根据用户ID获取用户详细信息
        
        // <param name="userId">用户ID</param>
        // <returns>用户实体，如果不存在则返回null</returns>
        Task<User?> GetUserByIdAsync(string userId);

        
        // 根据用户ID获取用户详细信息（包含关联数据）
        
        // <param name="userId">用户ID</param>
        // <returns>包含关联数据的用户实体，如果不存在则返回null</returns>
        Task<User?> GetUserWithDetailsAsync(string userId);

        
        // 检查用户是否存在
        
        // <param name="userId">用户ID</param>
        // <returns>用户是否存在</returns>
        Task<bool> ExistsAsync(string userId);

        
        // 获取用户的统计信息
        
        // <param name="userId">用户ID</param>
        // <returns>用户统计信息</returns>
        Task<UserStats> GetUserStatsAsync(string userId);
    }

    
    // 用户统计信息
    
    public class UserStats
    {
        public int TotalOrders { get; set; }
        public int FavoriteCount { get; set; }
        public int CartItemCount { get; set; }
        public int AvailableCouponCount { get; set; }
        public int AddressCount { get; set; }
    }
}
