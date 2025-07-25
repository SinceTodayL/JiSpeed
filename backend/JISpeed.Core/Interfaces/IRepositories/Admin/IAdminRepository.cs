using JISpeed.Core.Entities.Admin;

namespace JISpeed.Core.Interfaces.IRepositories.Admin
{
    // 管理员仓储接口
    public interface IAdminRepository
    {
        // 根据管理员ID获取管理员信息
        // <param name="adminId">管理员ID</param>
        // <returns>管理员实体，如果不存在则返回null</returns>
        Task<Entities.Admin.Admin?> GetByIdAsync(string adminId);

        // 根据管理员ID获取管理员详细信息（包含关联数据）
        // <param name="adminId">管理员ID</param>
        // <returns>包含关联数据的管理员实体，如果不存在则返回null</returns>
        Task<Entities.Admin.Admin?> GetWithDetailsAsync(string adminId);

        // 根据ApplicationUserId获取管理员信息
        // <param name="applicationUserId">应用用户ID</param>
        // <returns>管理员实体，如果不存在则返回null</returns>
        Task<Entities.Admin.Admin?> GetByApplicationUserIdAsync(string applicationUserId);

        // 获取所有管理员列表
        // <returns>管理员列表</returns>
        Task<List<Entities.Admin.Admin>> GetAllAsync();

        // 检查管理员是否存在
        // <param name="adminId">管理员ID</param>
        // <returns>管理员是否存在</returns>
        Task<bool> ExistsAsync(string adminId);

        // 创建新管理员
        // <param name="admin">管理员实体</param>
        // <returns>创建的管理员实体</returns>
        Task<Entities.Admin.Admin> CreateAsync(Entities.Admin.Admin admin);

        // 更新管理员信息
        // <param name="admin">管理员实体</param>
        // <returns>更新的管理员实体</returns>
        Task<Entities.Admin.Admin> UpdateAsync(Entities.Admin.Admin admin);

        // 删除管理员
        // <param name="adminId">管理员ID</param>
        // <returns>是否删除成功</returns>
        Task<bool> DeleteAsync(string adminId);

        // 保存更改
        // <returns>保存的记录数</returns>
        Task<int> SaveChangesAsync();
    }
}
