using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JISpeed.Core.Entities.Reconciliation;
using JISpeed.Core.Interfaces.IRepositories.Reconciliation;
using JISpeed.Infrastructure.Data;

namespace JISpeed.Infrastructure.Repositories.Reconciliation
{
    // 对账仓储实现 - 处理对账异常的数据访问操作
    public class ReconciliationRepository : IReconciliationRepository
    {
        private readonly OracleDbContext _context;

        public ReconciliationRepository(OracleDbContext context)
        {
            _context = context;
        }

        // ===== 实现IBaseRepository接口 =====

        public async Task<JISpeed.Core.Entities.Reconciliation.Reconciliation?> GetByIdAsync(string id)
        {
            return await _context.Reconciliations
                .Include(r => r.Orders)
                .FirstOrDefaultAsync(r => r.ReconId == id);
        }

        public async Task<JISpeed.Core.Entities.Reconciliation.Reconciliation?> GetWithDetailsAsync(string id)
        {
            return await _context.Reconciliations
                .Include(r => r.Orders)
                    .ThenInclude(o => o.User)
                .FirstOrDefaultAsync(r => r.ReconId == id);
        }

        public async Task<List<JISpeed.Core.Entities.Reconciliation.Reconciliation>> GetAllAsync()
        {
            return await _context.Reconciliations
                .Include(r => r.Orders)
                .OrderByDescending(r => r.FoundAt)
                .ToListAsync();
        }

        public async Task<JISpeed.Core.Entities.Reconciliation.Reconciliation> CreateAsync(JISpeed.Core.Entities.Reconciliation.Reconciliation entity)
        {
            await _context.Reconciliations.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<JISpeed.Core.Entities.Reconciliation.Reconciliation> AddAsync(JISpeed.Core.Entities.Reconciliation.Reconciliation entity)
        {
            await _context.Reconciliations.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<JISpeed.Core.Entities.Reconciliation.Reconciliation> UpdateAsync(JISpeed.Core.Entities.Reconciliation.Reconciliation entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            await Task.CompletedTask;
            return entity;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var reconciliation = await _context.Reconciliations.FindAsync(id);
            if (reconciliation != null)
            {
                _context.Reconciliations.Remove(reconciliation);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<int> CountAsync()
        {
            return await _context.Reconciliations.CountAsync();
        }

        public async Task<List<JISpeed.Core.Entities.Reconciliation.Reconciliation>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _context.Reconciliations
                .Include(r => r.Orders)
                .OrderByDescending(r => r.FoundAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Reconciliations.AnyAsync(r => r.ReconId == id);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // ===== 业务专用方法 =====

        // 根据账期范围查询对账记录
        public async Task<IEnumerable<JISpeed.Core.Entities.Reconciliation.Reconciliation>> GetByPeriodRangeAsync(DateTime periodStart, DateTime periodEnd)
        {
            return await _context.Reconciliations
                .Include(r => r.Orders)
                .Where(r => r.PeriodStart >= periodStart && r.PeriodEnd <= periodEnd)
                .OrderByDescending(r => r.FoundAt)
                .ToListAsync();
        }

        // 根据发现时间范围查询对账记录
        public async Task<IEnumerable<JISpeed.Core.Entities.Reconciliation.Reconciliation>> GetByFoundTimeRangeAsync(DateTime startTime, DateTime endTime)
        {
            return await _context.Reconciliations
                .Include(r => r.Orders)
                .Where(r => r.FoundAt >= startTime && r.FoundAt <= endTime)
                .OrderByDescending(r => r.FoundAt)
                .ToListAsync();
        }

        // 根据异常类型查询对账记录
        public async Task<IEnumerable<JISpeed.Core.Entities.Reconciliation.Reconciliation>> GetByReconTypeAsync(int reconType)
        {
            return await _context.Reconciliations
                .Include(r => r.Orders)
                .Where(r => r.ReconType == reconType)
                .OrderByDescending(r => r.FoundAt)
                .ToListAsync();
        }

        // 根据解决状态查询对账记录
        public async Task<IEnumerable<JISpeed.Core.Entities.Reconciliation.Reconciliation>> GetByResolvedStatusAsync(bool isResolved)
        {
            return await _context.Reconciliations
                .Include(r => r.Orders)
                .Where(r => r.IsResolved == isResolved)
                .OrderByDescending(r => r.FoundAt)
                .ToListAsync();
        }

        // 获取未解决的对账异常
        public async Task<IEnumerable<JISpeed.Core.Entities.Reconciliation.Reconciliation>> GetUnresolvedReconciliationsAsync()
        {
            return await _context.Reconciliations
                .Include(r => r.Orders)
                .Where(r => !r.IsResolved)
                .OrderByDescending(r => r.DiffAmount)
                .ToListAsync();
        }

        // 获取已解决的对账异常
        public async Task<IEnumerable<JISpeed.Core.Entities.Reconciliation.Reconciliation>> GetResolvedReconciliationsAsync()
        {
            return await _context.Reconciliations
                .Include(r => r.Orders)
                .Where(r => r.IsResolved)
                .OrderByDescending(r => r.FoundAt)
                .ToListAsync();
        }

        // 根据差额金额范围查询
        public async Task<IEnumerable<JISpeed.Core.Entities.Reconciliation.Reconciliation>> GetByDiffAmountRangeAsync(decimal minAmount, decimal maxAmount)
        {
            return await _context.Reconciliations
                .Include(r => r.Orders)
                .Where(r => r.DiffAmount >= minAmount && r.DiffAmount <= maxAmount)
                .OrderByDescending(r => r.DiffAmount)
                .ToListAsync();
        }

        // 根据受影响订单数范围查询
        public async Task<IEnumerable<JISpeed.Core.Entities.Reconciliation.Reconciliation>> GetByAffectedOrdersRangeAsync(int minOrders, int maxOrders)
        {
            return await _context.Reconciliations
                .Include(r => r.Orders)
                .Where(r => r.AffectedOrders >= minOrders && r.AffectedOrders <= maxOrders)
                .OrderByDescending(r => r.AffectedOrders)
                .ToListAsync();
        }

        // 获取高优先级异常 (差额金额大于指定值)
        public async Task<IEnumerable<JISpeed.Core.Entities.Reconciliation.Reconciliation>> GetHighPriorityReconciliationsAsync(decimal threshold)
        {
            return await _context.Reconciliations
                .Include(r => r.Orders)
                .Where(r => Math.Abs(r.DiffAmount) > threshold)
                .OrderByDescending(r => Math.Abs(r.DiffAmount))
                .ToListAsync();
        }

        // 统计异常类型分布
        public async Task<Dictionary<int, int>> GetReconTypeCountAsync()
        {
            return await _context.Reconciliations
                .GroupBy(r => r.ReconType)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        // 统计解决状态分布
        public async Task<Dictionary<bool, int>> GetResolvedStatusCountAsync()
        {
            return await _context.Reconciliations
                .GroupBy(r => r.IsResolved)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        // 获取最近的对账异常
        public async Task<IEnumerable<JISpeed.Core.Entities.Reconciliation.Reconciliation>> GetRecentReconciliationsAsync(int count = 10)
        {
            return await _context.Reconciliations
                .Include(r => r.Orders)
                .OrderByDescending(r => r.FoundAt)
                .Take(count)
                .ToListAsync();
        }

        // 计算总差额金额 (指定时间范围内)
        public async Task<decimal> GetTotalDiffAmountAsync(DateTime startTime, DateTime endTime)
        {
            return await _context.Reconciliations
                .Where(r => r.FoundAt >= startTime && r.FoundAt <= endTime)
                .SumAsync(r => Math.Abs(r.DiffAmount));
        }

        // 计算未解决异常的总差额
        public async Task<decimal> GetUnresolvedTotalDiffAmountAsync()
        {
            return await _context.Reconciliations
                .Where(r => !r.IsResolved)
                .SumAsync(r => Math.Abs(r.DiffAmount));
        }

        // 获取对账异常统计概览
        public async Task<Dictionary<string, object>> GetReconciliationOverviewAsync(DateTime startTime, DateTime endTime)
        {
            var reconciliations = await _context.Reconciliations
                .Where(r => r.FoundAt >= startTime && r.FoundAt <= endTime)
                .ToListAsync();

            var totalCount = reconciliations.Count;
            var resolvedCount = reconciliations.Count(r => r.IsResolved);
            var unresolvedCount = reconciliations.Count(r => !r.IsResolved);
            var totalDiffAmount = reconciliations.Sum(r => Math.Abs(r.DiffAmount));
            var averageDiffAmount = totalCount > 0 ? reconciliations.Average(r => Math.Abs(r.DiffAmount)) : 0;
            var totalAffectedOrders = reconciliations.Sum(r => r.AffectedOrders);
            var resolutionRate = totalCount > 0 ? (double)resolvedCount / totalCount * 100 : 0;

            return new Dictionary<string, object>
            {
                ["TotalCount"] = totalCount,
                ["ResolvedCount"] = resolvedCount,
                ["UnresolvedCount"] = unresolvedCount,
                ["TotalDiffAmount"] = Math.Round(totalDiffAmount, 2),
                ["AverageDiffAmount"] = Math.Round(averageDiffAmount, 2),
                ["TotalAffectedOrders"] = totalAffectedOrders,
                ["ResolutionRate"] = Math.Round(resolutionRate, 2)
            };
        }

        // 根据账期查询对账记录
        public async Task<IEnumerable<JISpeed.Core.Entities.Reconciliation.Reconciliation>> GetBySpecificPeriodAsync(DateTime periodStart, DateTime periodEnd)
        {
            return await _context.Reconciliations
                .Include(r => r.Orders)
                .Where(r => r.PeriodStart == periodStart && r.PeriodEnd == periodEnd)
                .OrderByDescending(r => r.DiffAmount)
                .ToListAsync();
        }

        // 获取长期未解决的异常 (超过指定天数)
        public async Task<IEnumerable<JISpeed.Core.Entities.Reconciliation.Reconciliation>> GetLongStandingIssuesAsync(int daysThreshold)
        {
            var thresholdDate = DateTime.Now.AddDays(-daysThreshold);
            return await _context.Reconciliations
                .Include(r => r.Orders)
                .Where(r => !r.IsResolved && r.FoundAt <= thresholdDate)
                .OrderBy(r => r.FoundAt)
                .ToListAsync();
        }

        // 统计每月对账异常数量
        public async Task<Dictionary<string, int>> GetMonthlyReconciliationCountAsync(int year)
        {
            var reconciliations = await _context.Reconciliations
                .Where(r => r.FoundAt.Year == year)
                .ToListAsync();

            return reconciliations
                .GroupBy(r => r.FoundAt.ToString("yyyy-MM"))
                .ToDictionary(g => g.Key, g => g.Count());
        }

        // 获取异常金额最大的对账记录
        public async Task<IEnumerable<JISpeed.Core.Entities.Reconciliation.Reconciliation>> GetTopByDiffAmountAsync(int topCount = 10)
        {
            return await _context.Reconciliations
                .Include(r => r.Orders)
                .OrderByDescending(r => Math.Abs(r.DiffAmount))
                .Take(topCount)
                .ToListAsync();
        }
    }
}
