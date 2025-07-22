using Microsoft.EntityFrameworkCore;
using JISpeed.Core.Entities.User;
using JISpeed.Core.Interfaces.IRepositories;
using JISpeed.Infrastructure.Data;

namespace JISpeed.Infrastructure.Repositories
{

    // 用户仓储实现

    public class UserRepository : IUserRepository
    {
        private readonly OracleDbContext _context;

        public UserRepository(OracleDbContext context)
        {
            _context = context;
        }


        // 根据用户ID获取用户基本信息

        // <param name="userId">用户ID</param>
        // <returns>用户实体</returns>
        public async Task<User?> GetUserByIdAsync(string userId)
        {
            return await _context.CustomUsers
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }


        // 根据用户ID获取用户详细信息（包含关联数据）

        // <param name="userId">用户ID</param>
        // <returns>包含关联数据的用户实体</returns>
        public async Task<User?> GetUserWithDetailsAsync(string userId)
        {
            return await _context.CustomUsers
                .Include(u => u.DefaultAddress) // 包含默认地址
                .Include(u => u.ApplicationUser) // 包含身份验证信息
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }


        // 检查用户是否存在

        // <param name="userId">用户ID</param>
        // <returns>用户是否存在</returns>
        public async Task<bool> ExistsAsync(string userId)
        {
            return await _context.CustomUsers
                .AnyAsync(u => u.UserId == userId);
        }


        // 获取用户的统计信息

        // <param name="userId">用户ID</param>
        // <returns>用户统计信息</returns>
        public async Task<UserStats> GetUserStatsAsync(string userId)
        {
            // 使用LINQ查询获取各种统计数据
            var stats = new UserStats();

            // 获取订单总数
            stats.TotalOrders = await _context.Orders
                .CountAsync(o => o.UserId == userId);

            // 获取收藏数量
            stats.FavoriteCount = await _context.Favorites
                .CountAsync(f => f.UserId == userId);

            // 获取购物车商品数量
            stats.CartItemCount = await _context.CartItems
                .CountAsync(c => c.UserId == userId);

            // 获取可用优惠券数量（假设Coupon表有状态字段，这里简化为所有优惠券）
            stats.AvailableCouponCount = await _context.Coupons
                .CountAsync(c => c.UserId == userId);

            // 获取地址数量
            stats.AddressCount = await _context.Addresses
                .CountAsync(a => a.UserId == userId);

            return stats;
        }

        
        /// 创建新用户
        /// </summary>
        /// <param name="user">用户实体</param>
        /// <returns>创建的用户实体</returns>
        public async Task<User> CreateAsync(User user)
        {
            var entity = await _context.CustomUsers.AddAsync(user);
            return entity.Entity;
        }

        
        /// 根据ApplicationUser ID获取关联的User实体
        /// </summary>
        /// <param name="applicationUserId">ApplicationUser的ID</param>
        /// <returns>关联的User实体</returns>
        public async Task<User?> GetUserByApplicationUserIdAsync(string applicationUserId)
        {
            return await _context.CustomUsers
                .Include(u => u.DefaultAddress)
                .Include(u => u.ApplicationUser)
                .FirstOrDefaultAsync(u => u.ApplicationUserId == applicationUserId);
        }

        
        /// 保存更改
        /// </summary>
        /// <returns>保存的记录数</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
