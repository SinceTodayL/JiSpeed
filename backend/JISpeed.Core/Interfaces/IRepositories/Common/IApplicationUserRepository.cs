using JISpeed.Core.Entities.Common;

namespace JISpeed.Core.Interfaces.IRepositories.Common
{
    // 应用用户仓储接口
    public interface IApplicationUserRepository
    {
        // 根据用户ID获取应用用户信息
        // <param name="id">用户ID</param>
        // <returns>应用用户实体，如果不存在则返回null</returns>
        Task<ApplicationUser?> GetByIdAsync(string id);

        // 根据用户名获取应用用户信息
        // <param name="userName">用户名</param>
        // <returns>应用用户实体，如果不存在则返回null</returns>
        Task<ApplicationUser?> GetByUserNameAsync(string userName);

        // 根据邮箱获取应用用户信息
        // <param name="email">邮箱</param>
        // <returns>应用用户实体，如果不存在则返回null</returns>
        Task<ApplicationUser?> GetByEmailAsync(string email);

        // 根据用户类型获取应用用户列表
        // <param name="userType">用户类型</param>
        // <returns>应用用户列表</returns>
        Task<List<ApplicationUser>> GetByUserTypeAsync(int userType);

        // 根据状态获取应用用户列表
        // <param name="status">状态</param>
        // <returns>应用用户列表</returns>
        Task<List<ApplicationUser>> GetByStatusAsync(int status);

        // 获取所有应用用户列表
        // <returns>应用用户列表</returns>
        Task<List<ApplicationUser>> GetAllAsync();

        // 检查应用用户是否存在
        // <param name="id">用户ID</param>
        // <returns>应用用户是否存在</returns>
        Task<bool> ExistsAsync(string id);

        // 创建新应用用户
        // <param name="user">应用用户实体</param>
        // <returns>创建的应用用户实体</returns>
        Task<ApplicationUser> CreateAsync(ApplicationUser user);

        // 更新应用用户信息
        // <param name="user">应用用户实体</param>
        // <returns>更新的应用用户实体</returns>
        Task<ApplicationUser> UpdateAsync(ApplicationUser user);

        // 删除应用用户
        // <param name="id">用户ID</param>
        // <returns>是否删除成功</returns>
        Task<bool> DeleteAsync(string id);

        // 保存更改
        // <returns>保存的记录数</returns>
        Task<int> SaveChangesAsync();
    }
}
