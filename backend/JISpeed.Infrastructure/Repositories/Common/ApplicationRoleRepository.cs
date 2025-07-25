using JISpeed.Core.Entities.Common;
using JISpeed.Core.Interfaces.IRepositories.Common;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories.Common
{
    public class ApplicationRoleRepository : BaseRepository<ApplicationRole, string>, IApplicationRoleRepository
    {
        public ApplicationRoleRepository(OracleDbContext context) : base(context)
        {
        }

        // 重写：获取所有应用角色列表
        public override async Task<List<ApplicationRole>> GetAllAsync()
        {
            return await _context.ApplicationRoles
                .OrderBy(r => r.RoleType)
                .ThenBy(r => r.Name)
                .ToListAsync();
        }

        // === 业务专用查询方法 ===

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

        // 根据角色名检查应用角色是否存在
        // <param name="roleName">角色名</param>
        // <returns>应用角色是否存在</returns>
        public async Task<bool> ExistsByNameAsync(string roleName)
        {
            return await _context.ApplicationRoles
                .AnyAsync(r => r.Name == roleName);
        }
    }
}
