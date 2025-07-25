using JISpeed.Core.Entities.Admin;
using JISpeed.Core.Interfaces.IRepositories.Admin;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories.Admin
{
    public class AnnouncementRepository : IAnnouncementRepository
    {
        private readonly OracleDbContext _context;

        public AnnouncementRepository(OracleDbContext context)
        {
            _context = context;
        }

        // 根据公告ID获取公告信息
        // <param name="announceId">公告ID</param>
        // <returns>公告实体，如果不存在则返回null</returns>
        public async Task<Announcement?> GetByIdAsync(string announceId)
        {
            return await _context.Announcements
                .FirstOrDefaultAsync(a => a.AnnounceId == announceId);
        }

        // 根据公告ID获取公告详细信息（包含关联数据）
        // <param name="announceId">公告ID</param>
        // <returns>包含关联数据的公告实体，如果不存在则返回null</returns>
        public async Task<Announcement?> GetWithDetailsAsync(string announceId)
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

        // 获取所有有效公告列表
        // <returns>有效公告列表</returns>
        public async Task<List<Announcement>> GetActiveAnnouncementsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Announcements
                .Where(a => a.StartAt <= now && a.EndAt >= now)
                .OrderByDescending(a => a.StartAt)
                .ToListAsync();
        }

        // 获取所有公告列表
        // <returns>公告列表</returns>
        public async Task<List<Announcement>> GetAllAsync()
        {
            return await _context.Announcements
                .Include(a => a.Admin)
                .OrderByDescending(a => a.StartAt)
                .ToListAsync();
        }

        // 检查公告是否存在
        // <param name="announceId">公告ID</param>
        // <returns>公告是否存在</returns>
        public async Task<bool> ExistsAsync(string announceId)
        {
            return await _context.Announcements
                .AnyAsync(a => a.AnnounceId == announceId);
        }

        // 创建新公告
        // <param name="announcement">公告实体</param>
        // <returns>创建的公告实体</returns>
        public async Task<Announcement> CreateAsync(Announcement announcement)
        {
            var entity = await _context.Announcements.AddAsync(announcement);
            return entity.Entity;
        }

        // 更新公告信息
        // <param name="announcement">公告实体</param>
        // <returns>更新的公告实体</returns>
        public async Task<Announcement> UpdateAsync(Announcement announcement)
        {
            var entity = _context.Announcements.Update(announcement);
            await Task.CompletedTask; // 修复CS1998警告
            return entity.Entity;
        }

        // 删除公告
        // <param name="announceId">公告ID</param>
        // <returns>是否删除成功</returns>
        public async Task<bool> DeleteAsync(string announceId)
        {
            var announcement = await GetByIdAsync(announceId);
            if (announcement == null)
                return false;

            _context.Announcements.Remove(announcement);
            return true;
        }

        // 保存更改
        // <returns>保存的记录数</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
