using JISpeed.Core.Interfaces.IRepositories.Admin;
using JISpeed.Infrastructure.Data;
using JISpeed.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using AdminEntity = JISpeed.Core.Entities.Admin.Admin;

namespace JISpeed.Infrastructure.Repositories.Admin
{
    public class AdminRepository : BaseRepository<AdminEntity, string>, IAdminRepository
    {
        public AdminRepository(OracleDbContext context) : base(context)
        {
        }

        // 重写GetWithDetailsAsync方法以包含关联数据
        // <param name="adminId">管理员ID</param>
        // <returns>包含关联数据的管理员实体，如果不存在则返回null</returns>
        public override async Task<AdminEntity?> GetWithDetailsAsync(string adminId)
        {
            return await _context.Admins
                .Include(a => a.ApplicationUser)
                .Include(a => a.Announcements)
                .FirstOrDefaultAsync(a => a.AdminId == adminId);
        }

        // 重写GetAllAsync方法以包含关联数据
        // <returns>管理员列表</returns>
        public override async Task<List<AdminEntity>> GetAllAsync()
        {
            return await _context.Admins
                .Include(a => a.ApplicationUser)
                .ToListAsync();
        }

        // === 业务专用查询方法 ===

        // 根据ApplicationUserId获取管理员信息
        // <param name="applicationUserId">应用用户ID</param>
        // <returns>管理员实体，如果不存在则返回null</returns>
        public async Task<AdminEntity?> GetByApplicationUserIdAsync(string applicationUserId)
        {
            return await _context.Admins
                .FirstOrDefaultAsync(a => a.ApplicationUserId == applicationUserId);
        }

        // 根据权限级别获取管理员列表
        // <param name="permissionLevel">权限级别</param>
        // <returns>管理员列表</returns>
        public async Task<List<AdminEntity>> GetByPermissionLevelAsync(int permissionLevel)
        {
            return await _context.Admins
                .Include(a => a.ApplicationUser)
                .Where(a => a.PermissionLevel == permissionLevel.ToString())
                .ToListAsync();
        }

        // 根据状态获取管理员列表
        // <param name="status">状态</param>
        // <returns>管理员列表</returns>
        public async Task<List<AdminEntity>> GetByStatusAsync(int status)
        {
            return await _context.Admins
                .Include(a => a.ApplicationUser)
                .Where(a => a.ApplicationUser != null && a.ApplicationUser.Status == status)
                .ToListAsync();
        }

        // 根据姓名搜索管理员
        // <param name="name">姓名</param>
        // <returns>管理员列表</returns>
        public async Task<List<AdminEntity>> SearchByNameAsync(string name)
        {
            return await _context.Admins
                .Include(a => a.ApplicationUser)
                .Where(a => a.ApplicationUser != null &&
                           !string.IsNullOrEmpty(a.ApplicationUser.UserName) &&
                           a.ApplicationUser.UserName.Contains(name))
                .ToListAsync();
        }
    }
}
