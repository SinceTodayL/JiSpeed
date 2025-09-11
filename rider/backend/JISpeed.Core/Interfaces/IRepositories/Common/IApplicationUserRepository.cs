using JISpeed.Core.Entities.Common;

namespace JISpeed.Core.Interfaces.IRepositories.Common
{
    // 应用用户仓储接口
    public interface IApplicationUserRepository : IBaseRepository<ApplicationUser, string>
    {
        // === 业务专用查询方法 ===

        // 根据用户名获取应用用户信息
        // <param name="userName">用户名</param>
        // <returns>应用用户实体，如果不存在则返回null</returns>
        Task<ApplicationUser?> GetByUserNameAsync(string userName);

        // 根据邮箱获取应用用户信息
        // <param name="email">邮箱</param>
        // <returns>应用用户实体，如果不存在则返回null</returns>
        Task<List<ApplicationUser>> GetByEmailAsync(string email);
        
        // 根据邮箱获取应用用户信息
        // <param name="email">邮箱</param>
        // <returns>应用用户实体，如果不存在则返回null</returns>
        Task<ApplicationUser?> GetByEmailAndUserTypeAsync(string email,int userType);

        // 根据用户类型获取应用用户列表
        // <param name="userType">用户类型</param>
        // <returns>应用用户列表</returns>
        Task<List<ApplicationUser>> GetByUserTypeAndTimeRangeAsync(
            int userType,
            DateTime start,
            DateTime end);

        // 根据状态获取应用用户列表
        // <param name="status">状态</param>
        // <returns>应用用户列表</returns>
        Task<List<ApplicationUser>> GetByStatusAsync(int status);
        
    }
}
