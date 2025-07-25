using JISpeed.Core.Entities.Common;

namespace JISpeed.Core.Interfaces.IRepositories.Common
{
    // 应用角色仓储接口
    public interface IApplicationRoleRepository : IBaseRepository<ApplicationRole, string>
    {
        // === 业务专用查询方法 ===

        // 根据角色名获取应用角色信息
        // <param name="roleName">角色名</param>
        // <returns>应用角色实体，如果不存在则返回null</returns>
        Task<ApplicationRole?> GetByNameAsync(string roleName);

        // 根据角色类型获取应用角色列表
        // <param name="roleType">角色类型</param>
        // <returns>应用角色列表</returns>
        Task<List<ApplicationRole>> GetByRoleTypeAsync(int roleType);

        // 根据角色名检查应用角色是否存在
        // <param name="roleName">角色名</param>
        // <returns>应用角色是否存在</returns>
        Task<bool> ExistsByNameAsync(string roleName);
    }
}
