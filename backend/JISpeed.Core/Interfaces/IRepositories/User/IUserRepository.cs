using JISpeed.Core.Entities.Common;

namespace JISpeed.Core.Interfaces.IRepositories.User
{
    // 用户仓储接口
    public interface IUserRepository : IBaseRepository<JISpeed.Core.Entities.User.User, string>
    {
        // === 业务专用查询方法 ===

        // 根据ApplicationUserId获取用户信息
        // <param name="applicationUserId">应用用户ID</param>
        // <returns>用户实体，如果不存在则返回null</returns>
        Task<JISpeed.Core.Entities.User.User?> GetByApplicationUserIdAsync(string applicationUserId);

        // 根据昵称搜索用户
        // <param name="nickname">昵称</param>
        // <returns>用户列表</returns>
        Task<List<JISpeed.Core.Entities.User.User>> SearchByNicknameAsync(string nickname);

        // 根据等级获取用户列表
        // <param name="level">等级</param>
        // <returns>用户列表</returns>
        Task<List<JISpeed.Core.Entities.User.User>> GetByLevelAsync(int level);
        
        // 使用ApplicationUser进行用户的创建
        // <returns>用户实体，如果不存在则返回null</returns>
        Task<Core.Entities.User.User?> CreateFromApplicationUserAsync(ApplicationUser applicationUser);

    }
}
