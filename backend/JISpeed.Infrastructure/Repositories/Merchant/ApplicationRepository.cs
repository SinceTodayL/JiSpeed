using JISpeed.Core.Entities.Merchant;
using JISpeed.Core.Interfaces.IRepositories.Common;
using JISpeed.Core.Interfaces.IRepositories.Merchant;
using JISpeed.Infrastructure.Data;
using JISpeed.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories.Merchant
{
    public class ApplicationRepository : BaseRepository<Application, string>, IApplicationRepository
    {
        public ApplicationRepository(OracleDbContext context) : base(context)
        {
        }
        

        // 重写GetAllAsync以包含关联数据和排序
        // <returns>申请列表</returns>
        public async Task<List<Application>> GetAllAsync(int? size,int ? page)
        {
            int currentPage = page ?? 1;
            int pageSize = size ?? 20;
            return await _context.Applications
                .Include(a => a.Merchant)
                .Include(a => a.Admin)
                .OrderByDescending(a => a.SubmittedAt)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // === 业务专用查询方法 ===

        // 根据商家ID获取申请列表
        // <param name="merchantId">商家ID</param>
        // <returns>申请列表</returns>
        public async Task<List<Application>> GetByMerchantIdAsync(string merchantId,int?size,int? page)
        {
            int currentPage = page ?? 1;
            int pageSize = size ?? 20;
            return await _context.Applications
                .Where(a => a.MerchantId == merchantId)
                .OrderByDescending(a => a.SubmittedAt)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // 根据管理员ID获取申请列表
        // <param name="adminId">管理员ID</param>
        // <returns>申请列表</returns>
        public async Task<List<Application>> GetByAdminIdAsync(string adminId,int? size,int? page)
        {
            int currentPage = page ?? 1;
            int pageSize = size ?? 20;
            return await _context.Applications
                .Where(a => a.AdminId == adminId)
                .Include(a => a.Merchant)
                .OrderByDescending(a => a.AuditAt)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // 根据审核状态获取申请列表
        // <param name="auditStatus">审核状态</param>
        // <returns>申请列表</returns>
        public async Task<List<Application>> GetByAuditStatusAsync(int auditStatus,int?size,int? page)
        {
            int currentPage = page ?? 1;
            int pageSize = size ?? 20;
            return await _context.Applications
                .Where(a => a.AuditStatus == auditStatus)
                .Include(a => a.Merchant)
                .Include(a => a.Admin)
                .OrderByDescending(a => a.SubmittedAt)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Application>> GetByAuditStatusAndMerchantIdAsync(
            string merchantId, 
            int auditStatus, 
            int? size, int? page)
        {
            int currentPage = page ?? 1;
            int pageSize = size ?? 20;
            return await _context.Applications
                .Where(a => a.AuditStatus == auditStatus&&a.MerchantId == merchantId)
                .Include(a => a.Merchant)
                .Include(a => a.Admin)
                .OrderByDescending(a => a.SubmittedAt)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        // 根据时间范围获取申请列表
        // <param name="startTime">开始时间</param>
        // <param name="endTime">结束时间</param>
        // <returns>申请列表</returns>
        public async Task<List<Application>> GetByTimeRangeAsync(DateTime startTime, DateTime endTime,int?size,int? page)
        {
            int currentPage = page ?? 1;
            int pageSize = size ?? 20;
            return await _context.Applications
                .Where(a => a.SubmittedAt >= startTime && a.SubmittedAt <= endTime)
                .Include(a => a.Merchant)
                .Include(a => a.Admin)
                .OrderByDescending(a => a.SubmittedAt)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
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
        
    }
}
