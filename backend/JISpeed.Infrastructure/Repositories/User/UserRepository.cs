using JISpeed.Core.Entities.User;
using JISpeed.Core.Interfaces.IRepositories.User;
using JISpeed.Infrastructure.Data;
using JISpeed.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories.User
{
    public class UserRepository : BaseRepository<JISpeed.Core.Entities.User.User, string>, IUserRepository
    {
        public UserRepository(OracleDbContext context) : base(context)
        {
        }

        // 重写GetWithDetailsAsync方法以包含关联数据
        // <param name="userId">用户ID</param>
        // <returns>包含关联数据的用户实体，如果不存在则返回null</returns>
        public override async Task<JISpeed.Core.Entities.User.User?> GetWithDetailsAsync(string userId)
        {
            return await _context.CustomUsers
                .Include(u => u.ApplicationUser)
                .Include(u => u.Addresses)
                .Include(u => u.CartItems)
                .Include(u => u.Favorites)
                .Include(u => u.Orders)
                .Include(u => u.Coupons)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        // === 业务专用查询方法 ===

        // 根据ApplicationUserId获取用户信息
        // <param name="applicationUserId">应用用户ID</param>
        // <returns>用户实体，如果不存在则返回null</returns>
        public async Task<JISpeed.Core.Entities.User.User?> GetByApplicationUserIdAsync(string applicationUserId)
        {
            return await _context.CustomUsers
                .FirstOrDefaultAsync(u => u.ApplicationUserId == applicationUserId);
        }

        // 根据昵称搜索用户
        // <param name="nickname">昵称</param>
        // <returns>用户列表</returns>
        public async Task<List<JISpeed.Core.Entities.User.User>> SearchByNicknameAsync(string nickname)
        {
            return await _context.CustomUsers
                .Where(u => u.Nickname.Contains(nickname))
                .OrderBy(u => u.Nickname)
                .ToListAsync();
        }

        // 根据等级获取用户列表
        // <param name="level">等级</param>
        // <returns>用户列表</returns>
        public async Task<List<JISpeed.Core.Entities.User.User>> GetByLevelAsync(int level)
        {
            return await _context.CustomUsers
                .Where(u => u.Level == level)
                .OrderBy(u => u.Nickname)
                .ToListAsync();
        }

        // 重写GetAllAsync方法以包含关联数据和排序
        // <returns>用户列表</returns>
        public override async Task<List<JISpeed.Core.Entities.User.User>> GetAllAsync()
        {
            return await _context.CustomUsers
                .Include(u => u.ApplicationUser)
                .OrderByDescending(u => u.UserId)
                .ToListAsync();
        }
    }
}
