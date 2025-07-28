using JISpeed.Core.Entities.Common;

namespace JISpeed.Core.Interfaces.IRepositories.Admin
{
    // 管理员仓储接口
    public interface IAdminRepository : IBaseRepository<JISpeed.Core.Entities.Admin.Admin, string>
    {
        // === 业务专用查询方法 ===

        // 根据ApplicationUserId获取管理员信息
        // <param name="applicationUserId">应用用户ID</param>
        // <returns>管理员实体，如果不存在则返回null</returns>
        Task<JISpeed.Core.Entities.Admin.Admin?> GetByApplicationUserIdAsync(string applicationUserId);

        // 根据权限级别获取管理员列表
        // <param name="permissionLevel">权限级别</param>
        // <returns>管理员列表</returns>
        Task<List<JISpeed.Core.Entities.Admin.Admin>> GetByPermissionLevelAsync(int permissionLevel);

        // 根据状态获取管理员列表
        // <param name="status">状态</param>
        // <returns>管理员列表</returns>
        Task<List<JISpeed.Core.Entities.Admin.Admin>> GetByStatusAsync(int status);

        // 根据姓名搜索管理员
        // <param name="name">姓名</param>
        // <returns>管理员列表</returns>
        Task<List<JISpeed.Core.Entities.Admin.Admin>> SearchByNameAsync(string name);
        
        // 使用ApplicationUser进行用户的创建
        // <returns>用户实体，如果不存在则返回null</returns>
        Task<Core.Entities.Admin.Admin?> CreateFromApplicationUserAsync(ApplicationUser applicationUser);
    }
}
