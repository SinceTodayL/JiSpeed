using JISpeed.Core.Entities.Admin;
using JISpeed.Core.Interfaces.IRepositories.Admin;
using JISpeed.Infrastructure.Data;
using JISpeed.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories.Admin
{
    public class AnnouncementRepository : BaseRepository<Announcement, string>, IAnnouncementRepository
    {
        public AnnouncementRepository(OracleDbContext context) : base(context)
        {
        }

        // 重写GetWithDetailsAsync方法以包含关联数据
        // <param name="announceId">公告ID</param>
        // <returns>包含关联数据的公告实体，如果不存在则返回null</returns>
        public override async Task<Announcement?> GetWithDetailsAsync(string announceId)
        {
            return await _context.Announcements
                .Include(a => a.Admin)
                .ThenInclude(admin => admin.ApplicationUser)
                .FirstOrDefaultAsync(a => a.AnnounceId == announceId);
        }

        // 根据管理员ID获取公告列表
        // <param name="adminId">管理员ID</param>
        // <returns>公告列表</returns>
        public async Task<List<Announcement>> GetByAdminIdAsync(string adminId)
        {
            return await _context.Announcements
                .Where(a => a.AdminId == adminId)
                .OrderByDescending(a => a.StartAt)
                .ToListAsync();
        }

        // 根据目标角色获取有效公告列表
        // <param name="targetRole">目标角色</param>
        // <returns>有效公告列表</returns>
        public async Task<List<Announcement>> GetActiveByTargetRoleAsync(string? targetRole)
        {
            var now = DateTime.UtcNow;
            return await _context.Announcements
                .Where(a => a.StartAt <= now && a.EndAt >= now &&
                           (a.TargetRole == null || a.TargetRole == targetRole))
                .OrderByDescending(a => a.StartAt)
                .ToListAsync();
        }

        // 重写GetAllAsync方法以包含关联数据和排序
        // <returns>公告列表</returns>
        public override async Task<List<Announcement>> GetAllAsync()
        {
            return await _context.Announcements
                .Include(a => a.Admin)
                .OrderByDescending(a => a.StartAt)
                .ToListAsync();
        }

        // === 业务专用查询方法 ===

        // 根据类型获取公告列表
        // <param name="type">公告类型</param>
        // <returns>公告列表</returns>
        public async Task<List<Announcement>> GetByTypeAsync(int type)
        {
            return await _context.Announcements
                .Where(a => a.TargetRole == type.ToString())
                .OrderByDescending(a => a.StartAt)
                .ToListAsync();
        }

        // 根据状态获取公告列表
        // <param name="status">状态</param>
        // <returns>公告列表</returns>
        public async Task<List<Announcement>> GetByStatusAsync(int status)
        {
            var now = DateTime.UtcNow;
            return status switch
            {
                1 => await _context.Announcements.Where(a => a.StartAt <= now && a.EndAt >= now).ToListAsync(), // 有效
                0 => await _context.Announcements.Where(a => a.EndAt < now).ToListAsync(), // 过期
                _ => await _context.Announcements.ToListAsync()
            };
        }

        // 获取有效公告（未过期且状态为活跃）
        // <returns>有效公告列表</returns>
        public async Task<List<Announcement>> GetActiveAnnouncementsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Announcements
                .Where(a => a.StartAt <= now && a.EndAt >= now)
                .OrderByDescending(a => a.StartAt)
                .ToListAsync();
        }

        // 根据时间范围获取公告
        // <param name="startTime">开始时间</param>
        // <param name="endTime">结束时间</param>
        // <returns>公告列表</returns>
        public async Task<List<Announcement>> GetByTimeRangeAsync(DateTime startTime, DateTime endTime)
        {
            return await _context.Announcements
                .Where(a => a.StartAt >= startTime && a.EndAt <= endTime)
                .OrderByDescending(a => a.StartAt)
                .ToListAsync();
        }

        // 根据标题搜索公告
        // <param name="title">标题</param>
        // <returns>公告列表</returns>
        public async Task<List<Announcement>> SearchByTitleAsync(string title)
        {
            return await _context.Announcements
                .Where(a => a.Title.Contains(title))
                .OrderByDescending(a => a.StartAt)
                .ToListAsync();
        }
    }
}
