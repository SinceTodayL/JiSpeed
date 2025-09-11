using JISpeed.Core.Entities.Admin;

namespace JISpeed.Core.Interfaces.IRepositories.Admin
{
    // 公告仓储接口
    public interface IAnnouncementRepository : IBaseRepository<Announcement, string>
    {
        // === 业务专用查询方法 ===
        Task<List<Announcement>> GetAllAnnouncementsAsync(
            int? size, int? page);
        Task<List<Announcement>> GetAllAnnouncementsByUserTypeAsync(string  targetRole,int? size,int?page);

        // 获取有效公告（未过期且状态为活跃）
        // <returns>有效公告列表</returns>
        Task<List<Announcement>> GetActiveAnnouncementsByUserTypeAsync(string  targetRole,int? size,int?page);
        Task<List<Announcement>> GetAllActiveAnnouncementsAsync(int? size,int?page);

        // 根据时间范围获取公告
        // <param name="startTime">开始时间</param>
        // <param name="endTime">结束时间</param>
        // <returns>公告列表</returns>
        Task<List<Announcement>> GetByTimeRangeAsync(DateTime startTime, DateTime endTime);

        // 根据标题搜索公告
        // <param name="title">标题</param>
        // <returns>公告列表</returns>
        Task<List<Announcement>> SearchByTitleAsync(string title);
    }
}
