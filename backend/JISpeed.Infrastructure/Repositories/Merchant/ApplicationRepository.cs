using JISpeed.Core.Entities.Merchant;
using JISpeed.Core.Interfaces.IRepositories.Merchant;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories.Merchant
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly OracleDbContext _context;

        public ApplicationRepository(OracleDbContext context)
        {
            _context = context;
        }

        // 根据申请ID获取申请信息
        // <param name="applyId">申请ID</param>
        // <returns>申请实体，如果不存在则返回null</returns>
        public async Task<Application?> GetByIdAsync(string applyId)
        {
            return await _context.Applications
                .FirstOrDefaultAsync(a => a.ApplyId == applyId);
        }

        // 根据申请ID获取申请详细信息（包含关联数据）
        // <param name="applyId">申请ID</param>
        // <returns>包含关联数据的申请实体，如果不存在则返回null</returns>
        public async Task<Application?> GetWithDetailsAsync(string applyId)
        {
            return await _context.Applications
                .Include(a => a.Admin)
                    .ThenInclude(admin => admin!.ApplicationUser)
                .Include(a => a.Merchant)
                    .ThenInclude(m => m!.ApplicationUser)
                .FirstOrDefaultAsync(a => a.ApplyId == applyId);
        }

        // 根据商家ID获取申请列表
        // <param name="merchantId">商家ID</param>
        // <returns>申请列表</returns>
        public async Task<List<Application>> GetByMerchantIdAsync(string merchantId)
        {
            return await _context.Applications
                .Where(a => a.MerchantId == merchantId)
                .OrderByDescending(a => a.SubmittedAt)
                .ToListAsync();
        }

        // 根据管理员ID获取申请列表
        // <param name="adminId">管理员ID</param>
        // <returns>申请列表</returns>
        public async Task<List<Application>> GetByAdminIdAsync(string adminId)
        {
            return await _context.Applications
                .Where(a => a.AdminId == adminId)
                .Include(a => a.Merchant)
                .OrderByDescending(a => a.AuditAt)
                .ToListAsync();
        }

        // 根据审核状态获取申请列表
        // <param name="auditStatus">审核状态</param>
        // <returns>申请列表</returns>
        public async Task<List<Application>> GetByAuditStatusAsync(int auditStatus)
        {
            return await _context.Applications
                .Where(a => a.AuditStatus == auditStatus)
                .Include(a => a.Merchant)
                .Include(a => a.Admin)
                .OrderByDescending(a => a.SubmittedAt)
                .ToListAsync();
        }

        // 根据时间范围获取申请列表
        // <param name="startTime">开始时间</param>
        // <param name="endTime">结束时间</param>
        // <returns>申请列表</returns>
        public async Task<List<Application>> GetByTimeRangeAsync(DateTime startTime, DateTime endTime)
        {
            return await _context.Applications
                .Where(a => a.SubmittedAt >= startTime && a.SubmittedAt <= endTime)
                .Include(a => a.Merchant)
                .Include(a => a.Admin)
                .OrderByDescending(a => a.SubmittedAt)
                .ToListAsync();
        }

        // 根据公司名称搜索申请
        // <param name="companyName">公司名称</param>
        // <returns>申请列表</returns>
        public async Task<List<Application>> SearchByCompanyNameAsync(string companyName)
        {
            return await _context.Applications
                .Where(a => a.CompanyName.Contains(companyName))
                .Include(a => a.Merchant)
                .OrderByDescending(a => a.SubmittedAt)
                .ToListAsync();
        }

        // 获取所有申请列表
        // <returns>申请列表</returns>
        public async Task<List<Application>> GetAllAsync()
        {
            return await _context.Applications
                .Include(a => a.Merchant)
                .Include(a => a.Admin)
                .OrderByDescending(a => a.SubmittedAt)
                .ToListAsync();
        }

        // 检查申请是否存在
        // <param name="applyId">申请ID</param>
        // <returns>申请是否存在</returns>
        public async Task<bool> ExistsAsync(string applyId)
        {
            return await _context.Applications
                .AnyAsync(a => a.ApplyId == applyId);
        }

        // 创建新申请
        // <param name="application">申请实体</param>
        // <returns>创建的申请实体</returns>
        public async Task<Application> CreateAsync(Application application)
        {
            var entity = await _context.Applications.AddAsync(application);
            return entity.Entity;
        }

        // 更新申请信息
        // <param name="application">申请实体</param>
        // <returns>更新的申请实体</returns>
        public async Task<Application> UpdateAsync(Application application)
        {
            var entity = _context.Applications.Update(application);
            await Task.CompletedTask; // 解决异步警告
            return entity.Entity;
        }

        // 删除申请
        // <param name="applyId">申请ID</param>
        // <returns>是否删除成功</returns>
        public async Task<bool> DeleteAsync(string applyId)
        {
            var application = await GetByIdAsync(applyId);
            if (application == null)
                return false;

            _context.Applications.Remove(application);
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
