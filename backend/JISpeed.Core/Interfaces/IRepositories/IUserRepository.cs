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

        
        /// 获取用户的统计信息
        
        /// <param name="userId">用户ID</param>
        /// <returns>用户统计信息</returns>
        Task<UserStats> GetUserStatsAsync(string userId);

        
        /// 创建新用户
        
        /// <param name="user">用户实体</param>
        /// <returns>创建的用户实体</returns>
        Task<User> CreateAsync(User user);

        
        /// 根据ApplicationUser ID获取关联的User实体
        
        /// <param name="applicationUserId">ApplicationUser的ID</param>
        /// <returns>关联的User实体</returns>
        Task<User?> GetUserByApplicationUserIdAsync(string applicationUserId);

        
        /// 保存更改
        
        /// <returns>保存的记录数</returns>
        Task<int> SaveChangesAsync();
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