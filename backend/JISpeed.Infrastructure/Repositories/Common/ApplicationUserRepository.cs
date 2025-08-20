using JISpeed.Core.Entities.Common;
using JISpeed.Core.Interfaces.IRepositories.Common;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories.Common
{
    public class ApplicationUserRepository : BaseRepository<ApplicationUser, string>, IApplicationUserRepository
    {
        public ApplicationUserRepository(OracleDbContext context) : base(context)
        {
        }

        // 重写：获取所有应用用户列表
        public override async Task<List<ApplicationUser>> GetAllAsync()
        {
            return await _context.ApplicationUsers
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        // === 业务专用查询方法 ===

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
        // <returns>应用用户实体集合，如果不存在则返回空集合</returns>
        public async Task<List<ApplicationUser>> GetByEmailAsync(string email)
        {
            return await _context.ApplicationUsers
                .Where(u => u.Email == email).ToListAsync();;
        }
        
        // 根据邮箱和用户属性获取应用用户信息
        // <param name="email">邮箱</param>
        // <returns>应用用户实体，如果不存在则返回null</returns>
        public async Task<ApplicationUser?> GetByEmailAndUserTypeAsync(string email, int userType)
        {
            return await _context.ApplicationUsers
                .FirstOrDefaultAsync(u => u.Email == email && u.UserType == userType);
        }
        // 根据用户类型获取应用用户列表
        // <param name="userType">用户类型</param>
        // <returns>应用用户列表</returns>
        public async Task<List<ApplicationUser>> GetByUserTypeAndTimeRangeAsync(  int userType,
            DateTime start,
            DateTime end)
        {
            return await _context.ApplicationUsers
                .Where(u => u.UserType == userType&&u.CreatedAt >= start && u.CreatedAt <= end)
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
    }
}
