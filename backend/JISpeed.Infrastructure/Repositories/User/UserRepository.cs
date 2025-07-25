using JISpeed.Core.Entities.User;
using JISpeed.Core.Interfaces.IRepositories.User;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories.User
{
    public class UserRepository : IUserRepository
    {
        private readonly OracleDbContext _context;

        public UserRepository(OracleDbContext context)
        {
            _context = context;
        }

        // 根据用户ID获取用户信息
        // <param name="userId">用户ID</param>
        // <returns>用户实体，如果不存在则返回null</returns>
        public async Task<JISpeed.Core.Entities.User.User?> GetByIdAsync(string userId)
        {
            return await _context.CustomUsers
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        // 根据用户ID获取用户详细信息（包含关联数据）
        // <param name="userId">用户ID</param>
        // <returns>包含关联数据的用户实体，如果不存在则返回null</returns>
        public async Task<JISpeed.Core.Entities.User.User?> GetWithDetailsAsync(string userId)
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

        // 获取所有用户列表
        // <returns>用户列表</returns>
        public async Task<List<JISpeed.Core.Entities.User.User>> GetAllAsync()
        {
            return await _context.CustomUsers
                .Include(u => u.ApplicationUser)
                .OrderByDescending(u => u.UserId)
                .ToListAsync();
        }

        // 检查用户是否存在
        // <param name="userId">用户ID</param>
        // <returns>用户是否存在</returns>
        public async Task<bool> ExistsAsync(string userId)
        {
            return await _context.CustomUsers
                .AnyAsync(u => u.UserId == userId);
        }

        // 创建新用户
        // <param name="user">用户实体</param>
        // <returns>创建的用户实体</returns>
        public async Task<JISpeed.Core.Entities.User.User> CreateAsync(JISpeed.Core.Entities.User.User user)
        {
            var entity = await _context.CustomUsers.AddAsync(user);
            return entity.Entity;
        }

        // 更新用户信息
        // <param name="user">用户实体</param>
        // <returns>更新的用户实体</returns>
        public async Task<JISpeed.Core.Entities.User.User> UpdateAsync(JISpeed.Core.Entities.User.User user)
        {
            var entity = _context.CustomUsers.Update(user);
            await Task.CompletedTask; // 解决异步警告
            return entity.Entity;
        }

        // 删除用户
        // <param name="userId">用户ID</param>
        // <returns>是否删除成功</returns>
        public async Task<bool> DeleteAsync(string userId)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
                return false;

            _context.CustomUsers.Remove(user);
            return true;
        }

        // 保存更改
        // <returns>保存的记录数</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
