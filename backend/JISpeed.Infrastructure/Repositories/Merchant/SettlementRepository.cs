using JISpeed.Core.Entities.Merchant;
using JISpeed.Core.Interfaces.IRepositories.Merchant;
using JISpeed.Infrastructure.Data;
using JISpeed.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories.Merchant
{
    public class SettlementRepository : BaseRepository<Settlement, string>
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
        public override async Task<List<Settlement>> GetAllAsync()
        {
            return await _context.Settlements
                .Include(s => s.Merchant)
                .OrderByDescending(s => s.PeriodEnd)
                .ToListAsync();
        }

        // === 业务专用查询方法 ===

        // 根据商家ID获取结算列表
        // <param name="merchantId">商家ID</param>
        // <returns>结算列表</returns>
        public async Task<List<Settlement>> GetByMerchantIdAsync(string merchantId)
        {
            return await _context.Settlements
                .Where(s => s.MerchantId == merchantId)
                .OrderByDescending(s => s.PeriodEnd)
                .ToListAsync();
        }

        // 根据结算状态获取结算列表（是否已结算）
        // <param name="isSettled">是否已结算</param>
        // <returns>结算列表</returns>
        public async Task<List<Settlement>> GetBySettlementStatusAsync(bool isSettled)
        {
            return await _context.Settlements
                .Where(s => isSettled ? s.SettledAt != null : s.SettledAt == null)
                .Include(s => s.Merchant)
                .OrderByDescending(s => s.PeriodEnd)
                .ToListAsync();
        }

        // 根据周期范围获取结算列表
        // <param name="startDate">开始日期</param>
        // <param name="endDate">结束日期</param>
        // <returns>结算列表</returns>
        public async Task<List<Settlement>> GetByPeriodRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Settlements
                .Where(s => s.PeriodStart >= startDate && s.PeriodEnd <= endDate)
                .Include(s => s.Merchant)
                .OrderByDescending(s => s.PeriodEnd)
                .ToListAsync();
        }

        // 根据商家ID和结算状态获取结算列表
        // <param name="merchantId">商家ID</param>
        // <param name="isSettled">是否已结算</param>
        // <returns>结算列表</returns>
        public async Task<List<Settlement>> GetByMerchantIdAndStatusAsync(string merchantId, bool isSettled)
        {
            return await _context.Settlements
                .Where(s => s.MerchantId == merchantId &&
                           (isSettled ? s.SettledAt != null : s.SettledAt == null))
                .OrderByDescending(s => s.PeriodEnd)
                .ToListAsync();
        }

        // 根据商家ID和结算ID获取结算信息
        // <param name="merchantId">商家ID</param>
        // <param name="settleId">结算ID</param>
        // <returns>结算实体，如果不存在则返回null</returns>
        public async Task<Settlement?> GetByMerchantIdAndSettleIdAsync(string merchantId, string settleId)
        {
            return await _context.Settlements
                .FirstOrDefaultAsync(s => s.MerchantId == merchantId && s.SettleId == settleId);
        }
    }
}