using JISpeed.Core.Entities.Common;
using JISpeed.Core.Interfaces.IRepositories.Common;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories.Common
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly OracleDbContext _context;

        public ApplicationUserRepository(OracleDbContext context)
        {
            _context = context;
        }

        // 根据用户ID获取应用用户信息
        // <param name="id">用户ID</param>
        // <returns>应用用户实体，如果不存在则返回null</returns>
        public async Task<ApplicationUser?> GetByIdAsync(string id)
        {
            return await _context.ApplicationUsers
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        // 根据用户名获取应用用户信息
        // <param name="userName">用户名</param>
        // <returns>应用用户实体，如果不存在则返回null</returns>
        public async Task<ApplicationUser?> GetByUserNameAsync(string userName)
        {
            return await _context.ApplicationUsers
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }

        // 根据邮箱获取应用用户信息
        // <param name="email">邮箱</param>
        // <returns>应用用户实体，如果不存在则返回null</returns>
        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            return await _context.ApplicationUsers
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        // 根据用户类型获取应用用户列表
        // <param name="userType">用户类型</param>
        // <returns>应用用户列表</returns>
        public async Task<List<ApplicationUser>> GetByUserTypeAsync(int userType)
        {
            return await _context.ApplicationUsers
                .Where(u => u.UserType == userType)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        // 根据状态获取应用用户列表
        // <param name="status">状态</param>
        // <returns>应用用户列表</returns>
        public async Task<List<ApplicationUser>> GetByStatusAsync(int status)
        {
            return await _context.ApplicationUsers
                .Where(u => u.Status == status)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        // 获取所有应用用户列表
        // <returns>应用用户列表</returns>
        public async Task<List<ApplicationUser>> GetAllAsync()
        {
            return await _context.ApplicationUsers
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        // 检查应用用户是否存在
        // <param name="id">用户ID</param>
        // <returns>应用用户是否存在</returns>
        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.ApplicationUsers
                .AnyAsync(u => u.Id == id);
        }

        // 创建新应用用户
        // <param name="user">应用用户实体</param>
        // <returns>创建的应用用户实体</returns>
        public async Task<ApplicationUser> CreateAsync(ApplicationUser user)
        {
            var entity = await _context.ApplicationUsers.AddAsync(user);
            return entity.Entity;
        }

        // 更新应用用户信息
        // <param name="user">应用用户实体</param>
        // <returns>更新的应用用户实体</returns>
        public async Task<ApplicationUser> UpdateAsync(ApplicationUser user)
        {
            var entity = _context.ApplicationUsers.Update(user);
            await Task.CompletedTask; // 修复CS1998警告
            return entity.Entity;
        }

        // 删除应用用户
        // <param name="id">用户ID</param>
        // <returns>是否删除成功</returns>
        public async Task<bool> DeleteAsync(string id)
        {
            var user = await GetByIdAsync(id);
            if (user == null)
                return false;

            _context.ApplicationUsers.Remove(user);
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
