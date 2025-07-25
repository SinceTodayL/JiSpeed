using JISpeed.Core.Entities.User;

namespace JISpeed.Core.Interfaces.IRepositories.User
{
    // 用户仓储接口
    public interface IUserRepository
    {
        // 根据用户ID获取用户信息
        // <param name="userId">用户ID</param>
        // <returns>用户实体，如果不存在则返回null</returns>
        Task<JISpeed.Core.Entities.User.User?> GetByIdAsync(string userId);

        // 根据用户ID获取用户详细信息（包含关联数据）
        // <param name="userId">用户ID</param>
        // <returns>包含关联数据的用户实体，如果不存在则返回null</returns>
        Task<JISpeed.Core.Entities.User.User?> GetWithDetailsAsync(string userId);

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

        // 获取所有用户列表
        // <returns>用户列表</returns>
        Task<List<JISpeed.Core.Entities.User.User>> GetAllAsync();

        // 检查用户是否存在
        // <param name="userId">用户ID</param>
        // <returns>用户是否存在</returns>
        Task<bool> ExistsAsync(string userId);

        // 创建新用户
        // <param name="user">用户实体</param>
        // <returns>创建的用户实体</returns>
        Task<JISpeed.Core.Entities.User.User> CreateAsync(JISpeed.Core.Entities.User.User user);

        // 更新用户信息
        // <param name="user">用户实体</param>
        // <returns>更新的用户实体</returns>
        Task<JISpeed.Core.Entities.User.User> UpdateAsync(JISpeed.Core.Entities.User.User user);

        // 删除用户
        // <param name="userId">用户ID</param>
        // <returns>是否删除成功</returns>
        Task<bool> DeleteAsync(string userId);

        // 保存更改
        // <returns>保存的记录数</returns>
        Task<int> SaveChangesAsync();
    }
}
