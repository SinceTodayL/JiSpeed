using JISpeed.Core.Entities.Merchant;
using JISpeed.Core.Interfaces.IRepositories.Merchant;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories.Merchant
{
    public class SettlementRepository : ISettlementRepository
    {
        private readonly OracleDbContext _context;

        public SettlementRepository(OracleDbContext context)
        {
            _context = context;
        }

        // 根据结算ID获取结算信息
        // <param name="settleId">结算ID</param>
        // <returns>结算实体，如果不存在则返回null</returns>
        public async Task<Settlement?> GetByIdAsync(string settleId)
        {
            return await _context.Settlements
                .FirstOrDefaultAsync(s => s.SettleId == settleId);
        }

        // 根据结算ID获取结算详细信息（包含关联数据）
        // <param name="settleId">结算ID</param>
        // <returns>包含关联数据的结算实体，如果不存在则返回null</returns>
        public async Task<Settlement?> GetWithDetailsAsync(string settleId)
        {
            return await _context.Settlements
                .Include(s => s.Merchant)
                .ThenInclude(m => m.ApplicationUser)
                .FirstOrDefaultAsync(s => s.SettleId == settleId);
        }

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

        // 获取所有结算列表
        // <returns>结算列表</returns>
        public async Task<List<Settlement>> GetAllAsync()
        {
            return await _context.Settlements
                .Include(s => s.Merchant)
                .OrderByDescending(s => s.PeriodEnd)
                .ToListAsync();
        }

        // 检查结算是否存在
        // <param name="settleId">结算ID</param>
        // <returns>结算是否存在</returns>
        public async Task<bool> ExistsAsync(string settleId)
        {
            return await _context.Settlements
                .AnyAsync(s => s.SettleId == settleId);
        }

        // 创建新结算
        // <param name="settlement">结算实体</param>
        // <returns>创建的结算实体</returns>
        public async Task<Settlement> CreateAsync(Settlement settlement)
        {
            var entity = await _context.Settlements.AddAsync(settlement);
            return entity.Entity;
        }

        // 更新结算信息
        // <param name="settlement">结算实体</param>
        // <returns>更新的结算实体</returns>
        public async Task<Settlement> UpdateAsync(Settlement settlement)
        {
            var entity = _context.Settlements.Update(settlement);
            await Task.CompletedTask; // 解决异步警告
            return entity.Entity;
        }

        // 删除结算
        // <param name="settleId">结算ID</param>
        // <returns>是否删除成功</returns>
        public async Task<bool> DeleteAsync(string settleId)
        {
            var settlement = await GetByIdAsync(settleId);
            if (settlement == null)
                return false;

            _context.Settlements.Remove(settlement);
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