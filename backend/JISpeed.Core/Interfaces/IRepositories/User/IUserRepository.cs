using JISpeed.Core.Entities.Common;

namespace JISpeed.Core.Interfaces.IRepositories.User
{
    // 用户仓储接口
    public interface IUserRepository : IBaseRepository<JISpeed.Core.Entities.User.User, string>
    {
        // === 业务专用查询方法 ===

        // 根据ApplicationUserId获取用户信息
        // <param name="applicationUserId">应用用户ID</param>
        // <returns>用户实体，如果不存在则返回null</returns>
        Task<JISpeed.Core.Entities.User.User?> GetByApplicationUserIdAsync(string applicationUserId);

        /// 返回所有用户信息
        /// <param name="size">每页大小</param>
        /// <param name="page">页码</param>
        /// <returns>用户列表</returns>
        Task<List<JISpeed.Core.Entities.User.User>> GetAllUsersAsync(int size, int page);

        // 根据昵称搜索用户
        // <param name="nickname">昵称</param>
        // <returns>用户列表</returns>
        Task<List<JISpeed.Core.Entities.User.User>> SearchByNicknameAsync(string nickname, int size, int page);

        // 根据等级获取用户列表
        // <param name="level">等级</param>
        // <returns>用户列表</returns>
        Task<List<JISpeed.Core.Entities.User.User>> GetByLevelAsync(int level);

        // 使用ApplicationUser进行用户的创建
        // <returns>用户实体，如果不存在则返回null</returns>
        Task<Core.Entities.User.User?> CreateFromApplicationUserAsync(ApplicationUser applicationUser);


        /// 获取用户收藏列表（分页）

        /// <param name="userId">用户ID</param>
        /// <param name="page">页码</param>
        /// <param name="size">每页大小</param>
        /// <returns>收藏列表</returns>
        Task<List<JISpeed.Core.Entities.User.Favorite>> GetUserFavoritesAsync(string userId, int page = 1, int size = 10);


        /// 获取用户收藏数量

        /// <param name="userId">用户ID</param>
        /// <returns>收藏数量</returns>
        Task<int> GetUserFavoritesCountAsync(string userId);


        /// 获取用户统计信息（订单数、收藏数、地址数等）

        /// <param name="userId">用户ID</param>
        /// <returns>统计信息</returns>
        Task<(int OrderCount, int FavoriteCount, int AddressCount, int CartItemCount)> GetUserStatsAsync(string userId);


        /// 获取用户详细信息（包含ApplicationUser和DefaultAddress）

        /// <param name="userId">用户ID</param>
        /// <returns>用户详细信息</returns>
        Task<Core.Entities.User.User?> GetUserDetailAsync(string userId);

    }
}
