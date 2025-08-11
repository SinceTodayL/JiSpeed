using JISpeed.Core.Entities.Merchant;
using JISpeed.Core.Interfaces.IRepositories.Merchant;
using JISpeed.Infrastructure.Data;
using JISpeed.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories.Merchant
{
    public class SettlementRepository : BaseRepository<Settlement, string>,ISettlementRepository
    {
      
        public SettlementRepository(OracleDbContext context) : base(context)
        {
           
        }

        // 重写GetWithDetailsAsync以包含关联数据
        // <param name="settleId">结算ID</param>
        // <returns>包含关联数据的结算实体，如果不存在则返回null</returns>
        public override async Task<Settlement?> GetWithDetailsAsync(string settleId)
        {
            return await _context.Settlements
                .Include(s => s.Merchant)
                .ThenInclude(m => m.ApplicationUser)
                .FirstOrDefaultAsync(s => s.SettleId == settleId);
        }

        // 重写GetAllAsync以包含关联数据和排序
        // <returns>结算列表</returns>
        public async Task<List<Settlement>> GetAllAsync(
            int ?size,int ?page)
        {
            int pageSize = size ?? 20;
            int currentPage = page ?? 1;
            return await _context.Settlements
                .Include(s => s.Merchant)
                .OrderByDescending(s => s.PeriodEnd)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // === 业务专用查询方法 ===

        // 根据商家ID获取结算列表
        // <param name="merchantId">商家ID</param>
        // <returns>结算列表</returns>
        public async Task<List<Settlement>> GetByMerchantIdAsync(
            string merchantId,
            int ?size,int ?page)
        {
            int pageSize = size ?? 20;
            int currentPage = page ?? 1;
            return await _context.Settlements
                .Where(s => s.MerchantId == merchantId)
                .OrderByDescending(s => s.PeriodEnd)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        

        // 根据周期范围获取结算列表
        // <param name="startDate">开始日期</param>
        // <param name="endDate">结束日期</param>
        // <returns>结算列表</returns>
        public async Task<List<Settlement>> GetByPeriodRangeAsync(
            DateTime startDate, DateTime endDate,
            int ?size,int ?page) 
        {
            int pageSize = size ?? 20;
            int currentPage = page ?? 1;
            return await _context.Settlements
                .Where(s => s.PeriodStart >= startDate && s.PeriodEnd <= endDate)
                .Include(s => s.Merchant)
                .OrderByDescending(s => s.PeriodEnd)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Settlement>> GetPendingSettlementsAsync(
            int? size, int? page)
        {
            int pageSize = size ?? 20;
            int currentPage = page ?? 1;
            return await _context.Settlements
                .Where(s =>  s.SettledAt == null)
                .OrderByDescending(s => s.PeriodEnd)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        // 根据商家ID和结算状态获取结算列表
        // <param name="merchantId">商家ID</param>
        // <param name="isSettled">是否已结算</param>
        // <returns>结算列表</returns>
        public async Task<List<Settlement>> GetSettledSettlementsAsync(
            int? size, int? page)
        {
            int pageSize = size ?? 20;
            int currentPage = page ?? 1;
            return await _context.Settlements
                .Where(s =>  s.SettledAt != null)
                .OrderByDescending(s => s.PeriodEnd)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<List<Settlement>> GetSettledSettlementsByMerchantIdAsync(
            string merchantId,
            int ?size, int ?page)
        {
            int pageSize = size ?? 20;
            int currentPage = page ?? 1;
            return await _context.Settlements
                .Where(s => s.MerchantId == merchantId && s.SettledAt == null)
                .OrderByDescending(s => s.PeriodEnd)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Settlement>> GetPendingSettlementsByMerchantIdAsync(
            string merchantId,
            int ?size, int ?page)
        {
            int pageSize = size ?? 20;
            int currentPage = page ?? 1;
            return await _context.Settlements
                .Where(s => s.MerchantId == merchantId && s.SettledAt != null)
                .OrderByDescending(s => s.PeriodEnd)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Settlement>> GetByMerchantIdAndTimeRangeAsync(
            string merchantId,
            DateTime startTime, DateTime endTime,
            int? size, int? page)
        {
            int pageSize = size ?? 20;
            int currentPage = page ?? 1;
            return await _context.Settlements
                .Where(s => s.PeriodStart >= startTime && s.PeriodEnd <= endTime && s.MerchantId == merchantId)
                .Include(s => s.Merchant)
                .OrderByDescending(s => s.PeriodEnd)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}