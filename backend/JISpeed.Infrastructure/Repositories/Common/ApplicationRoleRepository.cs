using JISpeed.Core.Entities.Common;
using JISpeed.Core.Interfaces.IRepositories.Common;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories.Common
{
    public class ApplicationRoleRepository : IApplicationRoleRepository
    {
        private readonly OracleDbContext _context;

        public ApplicationRoleRepository(OracleDbContext context)
        {
            _context = context;
        }

        // 根据角色ID获取应用角色信息
        // <param name="id">角色ID</param>
        // <returns>应用角色实体，如果不存在则返回null</returns>
        public async Task<ApplicationRole?> GetByIdAsync(string id)
        {
            return await _context.ApplicationRoles
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        // 根据角色名获取应用角色信息
        // <param name="roleName">角色名</param>
        // <returns>应用角色实体，如果不存在则返回null</returns>
        public async Task<ApplicationRole?> GetByNameAsync(string roleName)
        {
            return await _context.ApplicationRoles
                .FirstOrDefaultAsync(r => r.Name == roleName);
        }

        // 根据角色类型获取应用角色列表
        // <param name="roleType">角色类型</param>
        // <returns>应用角色列表</returns>
        public async Task<List<ApplicationRole>> GetByRoleTypeAsync(int roleType)
        {
            return await _context.ApplicationRoles
                .Where(r => r.RoleType == roleType)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        // 获取所有应用角色列表
        // <returns>应用角色列表</returns>
        public async Task<List<ApplicationRole>> GetAllAsync()
        {
            return await _context.ApplicationRoles
                .OrderBy(r => r.RoleType)
                .ThenBy(r => r.Name)
                .ToListAsync();
        }

        // 检查应用角色是否存在
        // <param name="id">角色ID</param>
        // <returns>应用角色是否存在</returns>
        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.ApplicationRoles
                .AnyAsync(r => r.Id == id);
        }

        // 根据角色名检查应用角色是否存在
        // <param name="roleName">角色名</param>
        // <returns>应用角色是否存在</returns>
        public async Task<bool> ExistsByNameAsync(string roleName)
        {
            return await _context.ApplicationRoles
                .AnyAsync(r => r.Name == roleName);
        }

        // 创建新应用角色
        // <param name="role">应用角色实体</param>
        // <returns>创建的应用角色实体</returns>
        public async Task<ApplicationRole> CreateAsync(ApplicationRole role)
        {
            var entity = await _context.ApplicationRoles.AddAsync(role);
            return entity.Entity;
        }

        // 更新应用角色信息
        // <param name="role">应用角色实体</param>
        // <returns>更新的应用角色实体</returns>
        public async Task<ApplicationRole> UpdateAsync(ApplicationRole role)
        {
            var entity = _context.ApplicationRoles.Update(role);
            await Task.CompletedTask; // 修复CS1998警告
            return entity.Entity;
        }

        // 删除应用角色
        // <param name="id">角色ID</param>
        // <returns>是否删除成功</returns>
        public async Task<bool> DeleteAsync(string id)
        {
            var role = await GetByIdAsync(id);
            if (role == null)
                return false;

            _context.ApplicationRoles.Remove(role);
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
