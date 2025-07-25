using JISpeed.Core.Interfaces.IRepositories.Admin;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using AdminEntity = JISpeed.Core.Entities.Admin.Admin;

namespace JISpeed.Infrastructure.Repositories.Admin
{
    public class AdminRepository : IAdminRepository
    {
        private readonly OracleDbContext _context;

        public AdminRepository(OracleDbContext context)
        {
            _context = context;
        }

        // 根据管理员ID获取管理员信息
        // <param name="adminId">管理员ID</param>
        // <returns>管理员实体，如果不存在则返回null</returns>
        public async Task<AdminEntity?> GetByIdAsync(string adminId)
        {
            return await _context.Admins
                .FirstOrDefaultAsync(a => a.AdminId == adminId);
        }

        // 根据管理员ID获取管理员详细信息（包含关联数据）
        // <param name="adminId">管理员ID</param>
        // <returns>包含关联数据的管理员实体，如果不存在则返回null</returns>
        public async Task<AdminEntity?> GetWithDetailsAsync(string adminId)
        {
            return await _context.Admins
                .Include(a => a.ApplicationUser)
                .Include(a => a.Announcements)
                .FirstOrDefaultAsync(a => a.AdminId == adminId);
        }

        // 根据ApplicationUserId获取管理员信息
        // <param name="applicationUserId">应用用户ID</param>
        // <returns>管理员实体，如果不存在则返回null</returns>
        public async Task<AdminEntity?> GetByApplicationUserIdAsync(string applicationUserId)
        {
            return await _context.Admins
                .FirstOrDefaultAsync(a => a.ApplicationUserId == applicationUserId);
        }

        // 获取所有管理员列表
        // <returns>管理员列表</returns>
        public async Task<List<AdminEntity>> GetAllAsync()
        {
            return await _context.Admins
                .Include(a => a.ApplicationUser)
                .ToListAsync();
        }

        // 检查管理员是否存在
        // <param name="adminId">管理员ID</param>
        // <returns>管理员是否存在</returns>
        public async Task<bool> ExistsAsync(string adminId)
        {
            return await _context.Admins
                .AnyAsync(a => a.AdminId == adminId);
        }

        // 创建新管理员
        // <param name="admin">管理员实体</param>
        // <returns>创建的管理员实体</returns>
        public async Task<AdminEntity> CreateAsync(AdminEntity admin)
        {
            var entity = await _context.Admins.AddAsync(admin);
            return entity.Entity;
        }

        // 更新管理员信息
        // <param name="admin">管理员实体</param>
        // <returns>更新的管理员实体</returns>
        public async Task<AdminEntity> UpdateAsync(AdminEntity admin)
        {
            var entity = _context.Admins.Update(admin);
            await Task.CompletedTask; // 修复CS1998警告
            return entity.Entity;
        }

        // 删除管理员
        // <param name="adminId">管理员ID</param>
        // <returns>是否删除成功</returns>
        public async Task<bool> DeleteAsync(string adminId)
        {
            var admin = await GetByIdAsync(adminId);
            if (admin == null)
                return false;

            _context.Admins.Remove(admin);
            return true;
        }

        // 保存更改
        // <returns>保存的记录数</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
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
