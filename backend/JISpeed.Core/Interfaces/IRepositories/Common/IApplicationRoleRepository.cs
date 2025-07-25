using JISpeed.Core.Entities.Common;

namespace JISpeed.Core.Interfaces.IRepositories.Common
{
    // 应用角色仓储接口
    public interface IApplicationRoleRepository
    {
        // 根据角色ID获取应用角色信息
        // <param name="id">角色ID</param>
        // <returns>应用角色实体，如果不存在则返回null</returns>
        Task<ApplicationRole?> GetByIdAsync(string id);

        // 根据角色名获取应用角色信息
        // <param name="roleName">角色名</param>
        // <returns>应用角色实体，如果不存在则返回null</returns>
        Task<ApplicationRole?> GetByNameAsync(string roleName);

        // 根据角色类型获取应用角色列表
        // <param name="roleType">角色类型</param>
        // <returns>应用角色列表</returns>
        Task<List<ApplicationRole>> GetByRoleTypeAsync(int roleType);

        // 获取所有应用角色列表
        // <returns>应用角色列表</returns>
        Task<List<ApplicationRole>> GetAllAsync();

        // 检查应用角色是否存在
        // <param name="id">角色ID</param>
        // <returns>应用角色是否存在</returns>
        Task<bool> ExistsAsync(string id);

        // 根据角色名检查应用角色是否存在
        // <param name="roleName">角色名</param>
        // <returns>应用角色是否存在</returns>
        Task<bool> ExistsByNameAsync(string roleName);

        // 创建新应用角色
        // <param name="role">应用角色实体</param>
        // <returns>创建的应用角色实体</returns>
        Task<ApplicationRole> CreateAsync(ApplicationRole role);

        // 更新应用角色信息
        // <param name="role">应用角色实体</param>
        // <returns>更新的应用角色实体</returns>
        Task<ApplicationRole> UpdateAsync(ApplicationRole role);

        // 删除应用角色
        // <param name="id">角色ID</param>
        // <returns>是否删除成功</returns>
        Task<bool> DeleteAsync(string id);

        // 保存更改
        // <returns>保存的记录数</returns>
        Task<int> SaveChangesAsync();
    }
}
