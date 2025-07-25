using JISpeed.Core.Entities.Admin;

namespace JISpeed.Core.Interfaces.IRepositories.Admin
{
    // 公告仓储接口
    public interface IAnnouncementRepository
    {
        // 根据公告ID获取公告信息
        // <param name="announceId">公告ID</param>
        // <returns>公告实体，如果不存在则返回null</returns>
        Task<Announcement?> GetByIdAsync(string announceId);

        // 根据公告ID获取公告详细信息（包含关联数据）
        // <param name="announceId">公告ID</param>
        // <returns>包含关联数据的公告实体，如果不存在则返回null</returns>
        Task<Announcement?> GetWithDetailsAsync(string announceId);

        // 根据管理员ID获取公告列表
        // <param name="adminId">管理员ID</param>
        // <returns>公告列表</returns>
        Task<List<Announcement>> GetByAdminIdAsync(string adminId);

        // 根据目标角色获取有效公告列表
        // <param name="targetRole">目标角色</param>
        // <returns>有效公告列表</returns>
        Task<List<Announcement>> GetActiveByTargetRoleAsync(string? targetRole);

        // 获取所有有效公告列表
        // <returns>有效公告列表</returns>
        Task<List<Announcement>> GetActiveAnnouncementsAsync();

        // 获取所有公告列表
        // <returns>公告列表</returns>
        Task<List<Announcement>> GetAllAsync();

        // 检查公告是否存在
        // <param name="announceId">公告ID</param>
        // <returns>公告是否存在</returns>
        Task<bool> ExistsAsync(string announceId);

        // 创建新公告
        // <param name="announcement">公告实体</param>
        // <returns>创建的公告实体</returns>
        Task<Announcement> CreateAsync(Announcement announcement);

        // 更新公告信息
        // <param name="announcement">公告实体</param>
        // <returns>更新的公告实体</returns>
        Task<Announcement> UpdateAsync(Announcement announcement);

        // 删除公告
        // <param name="announceId">公告ID</param>
        // <returns>是否删除成功</returns>
        Task<bool> DeleteAsync(string announceId);

        // 保存更改
        // <returns>保存的记录数</returns>
        Task<int> SaveChangesAsync();
    }
}
