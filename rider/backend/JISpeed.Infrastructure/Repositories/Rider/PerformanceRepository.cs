using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JISpeed.Core.Entities.Rider;
using JISpeed.Core.Interfaces.IRepositories.Rider;
using JISpeed.Infrastructure.Data;

namespace JISpeed.Infrastructure.Repositories.Rider
{
    // 绩效仓储实现 - 处理绩效的数据访问操作
    public class PerformanceRepository : IPerformanceRepository
    {
        private readonly OracleDbContext _context;

        public PerformanceRepository(OracleDbContext context)
        {
            _context = context;
        }

        // === 基础CRUD操作 ===

        // 根据复合主键获取绩效记录
        public async Task<Performance?> GetByCompositeKeyAsync(string riderId, DateTime statsMonth)
        {
            return await _context.Performances
                .Include(p => p.Rider)
                .FirstOrDefaultAsync(p => p.RiderId == riderId && p.StatsMonth == statsMonth);
        }

        // 获取所有绩效记录
        public async Task<List<Performance>> GetAllAsync()
        {
            return await _context.Performances
                .Include(p => p.Rider)
                .OrderByDescending(p => p.StatsMonth)
                .ThenByDescending(p => p.Income)
                .ToListAsync();
        }

        // 创建绩效记录
        public async Task<Performance> CreateAsync(Performance entity)
        {
            await _context.Performances.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        // 更新绩效记录
        public async Task<Performance> UpdateAsync(Performance entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            await Task.CompletedTask;
            return entity;
        }

        // 删除绩效记录
        public async Task<bool> DeleteAsync(string riderId, DateTime statsMonth)
        {
            var performance = await _context.Performances
                .FirstOrDefaultAsync(p => p.RiderId == riderId && p.StatsMonth == statsMonth);

            if (performance != null)
            {
                _context.Performances.Remove(performance);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // 检查绩效记录是否存在
        public async Task<bool> ExistsAsync(string riderId, DateTime statsMonth)
        {
            return await _context.Performances
                .AnyAsync(p => p.RiderId == riderId && p.StatsMonth == statsMonth);
        }

        // 获取绩效记录总数
        public async Task<int> CountAsync()
        {
            return await _context.Performances.CountAsync();
        }

        // 分页获取绩效记录
        public async Task<List<Performance>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _context.Performances
                .Include(p => p.Rider)
                .OrderByDescending(p => p.StatsMonth)
                .ThenByDescending(p => p.Income)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // 保存更改
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // === 业务专用查询方法 ===

        // 根据骑手ID查询绩效记录
        public async Task<IEnumerable<Performance>> GetByRiderIdAsync(string riderId)
        {
            return await _context.Performances
                .Include(p => p.Rider)
                .Where(p => p.RiderId == riderId)
                .OrderByDescending(p => p.StatsMonth)
                .ToListAsync();
        }

        // 根据统计月份查询绩效记录
        public async Task<IEnumerable<Performance>> GetByStatsMonthAsync(DateTime statsMonth)
        {
            return await _context.Performances
                .Include(p => p.Rider)
                .Where(p => p.StatsMonth == statsMonth)
                .OrderByDescending(p => p.Income)
                .ToListAsync();
        }

        // 根据统计月份范围查询绩效记录
        public async Task<IEnumerable<Performance>> GetByMonthRangeAsync(DateTime startMonth, DateTime endMonth)
        {
            return await _context.Performances
                .Include(p => p.Rider)
                .Where(p => p.StatsMonth >= startMonth && p.StatsMonth <= endMonth)
                .OrderByDescending(p => p.StatsMonth)
                .ThenByDescending(p => p.Income)
                .ToListAsync();
        }

        // 根据骑手ID和月份范围查询绩效记录
        public async Task<IEnumerable<Performance>> GetByRiderIdAndMonthRangeAsync(string riderId, DateTime startMonth, DateTime endMonth)
        {
            return await _context.Performances
                .Include(p => p.Rider)
                .Where(p => p.RiderId == riderId && p.StatsMonth >= startMonth && p.StatsMonth <= endMonth)
                .OrderByDescending(p => p.StatsMonth)
                .ToListAsync();
        }

        // 根据总订单量范围查询
        public async Task<IEnumerable<Performance>> GetByTotalOrdersRangeAsync(int minOrders, int maxOrders)
        {
            return await _context.Performances
                .Include(p => p.Rider)
                .Where(p => p.TotalOrders >= minOrders && p.TotalOrders <= maxOrders)
                .OrderByDescending(p => p.TotalOrders)
                .ToListAsync();
        }

        // 根据准时率范围查询
        public async Task<IEnumerable<Performance>> GetByOnTimeRateRangeAsync(decimal minRate, decimal maxRate)
        {
            return await _context.Performances
                .Include(p => p.Rider)
                .Where(p => p.OnTimeRate >= minRate && p.OnTimeRate <= maxRate)
                .OrderByDescending(p => p.OnTimeRate)
                .ToListAsync();
        }

        // 根据好评率范围查询
        public async Task<IEnumerable<Performance>> GetByGoodReviewRateRangeAsync(decimal minRate, decimal maxRate)
        {
            return await _context.Performances
                .Include(p => p.Rider)
                .Where(p => p.GoodReviewRate >= minRate && p.GoodReviewRate <= maxRate)
                .OrderByDescending(p => p.GoodReviewRate)
                .ToListAsync();
        }

        // 根据收入范围查询
        public async Task<IEnumerable<Performance>> GetByIncomeRangeAsync(decimal minIncome, decimal maxIncome)
        {
            return await _context.Performances
                .Include(p => p.Rider)
                .Where(p => p.Income >= minIncome && p.Income <= maxIncome)
                .OrderByDescending(p => p.Income)
                .ToListAsync();
        }

        // 获取最佳绩效骑手 (按指定月份)
        public async Task<IEnumerable<Performance>> GetTopPerformersByMonthAsync(DateTime statsMonth, int topCount = 10)
        {
            return await _context.Performances
                .Include(p => p.Rider)
                .Where(p => p.StatsMonth == statsMonth)
                .OrderByDescending(p => p.Income)
                .ThenByDescending(p => p.OnTimeRate)
                .ThenByDescending(p => p.GoodReviewRate)
                .Take(topCount)
                .ToListAsync();
        }

        // 获取最佳绩效骑手 (按总订单量)
        public async Task<IEnumerable<Performance>> GetTopByTotalOrdersAsync(DateTime statsMonth, int topCount = 10)
        {
            return await _context.Performances
                .Include(p => p.Rider)
                .Where(p => p.StatsMonth == statsMonth)
                .OrderByDescending(p => p.TotalOrders)
                .Take(topCount)
                .ToListAsync();
        }

        // 获取最佳绩效骑手 (按准时率)
        public async Task<IEnumerable<Performance>> GetTopByOnTimeRateAsync(DateTime statsMonth, int topCount = 10)
        {
            return await _context.Performances
                .Include(p => p.Rider)
                .Where(p => p.StatsMonth == statsMonth)
                .OrderByDescending(p => p.OnTimeRate)
                .Take(topCount)
                .ToListAsync();
        }

        // 获取最佳绩效骑手 (按好评率)
        public async Task<IEnumerable<Performance>> GetTopByGoodReviewRateAsync(DateTime statsMonth, int topCount = 10)
        {
            return await _context.Performances
                .Include(p => p.Rider)
                .Where(p => p.StatsMonth == statsMonth)
                .OrderByDescending(p => p.GoodReviewRate)
                .Take(topCount)
                .ToListAsync();
        }

        // 获取最佳绩效骑手 (按收入)
        public async Task<IEnumerable<Performance>> GetTopByIncomeAsync(DateTime statsMonth, int topCount = 10)
        {
            return await _context.Performances
                .Include(p => p.Rider)
                .Where(p => p.StatsMonth == statsMonth)
                .OrderByDescending(p => p.Income)
                .Take(topCount)
                .ToListAsync();
        }

        // 统计指定月份的绩效概览
        public async Task<Dictionary<string, object>> GetMonthlyPerformanceOverviewAsync(DateTime statsMonth)
        {
            var performances = await _context.Performances
                .Where(p => p.StatsMonth == statsMonth)
                .ToListAsync();

            if (!performances.Any())
            {
                return new Dictionary<string, object>
                {
                    ["TotalRiders"] = 0,
                    ["TotalOrders"] = 0,
                    ["AverageOnTimeRate"] = 0,
                    ["AverageGoodReviewRate"] = 0,
                    ["TotalIncome"] = 0,
                    ["AverageIncome"] = 0
                };
            }

            var totalRiders = performances.Count;
            var totalOrders = performances.Sum(p => p.TotalOrders);
            var averageOnTimeRate = performances.Average(p => p.OnTimeRate);
            var averageGoodReviewRate = performances.Average(p => p.GoodReviewRate);
            var totalIncome = performances.Sum(p => p.Income);
            var averageIncome = performances.Average(p => p.Income);

            return new Dictionary<string, object>
            {
                ["TotalRiders"] = totalRiders,
                ["TotalOrders"] = totalOrders,
                ["AverageOnTimeRate"] = Math.Round(averageOnTimeRate, 2),
                ["AverageGoodReviewRate"] = Math.Round(averageGoodReviewRate, 2),
                ["TotalIncome"] = Math.Round(totalIncome, 2),
                ["AverageIncome"] = Math.Round(averageIncome, 2)
            };
        }

        // 获取骑手绩效趋势 (多个月份)
        public async Task<IEnumerable<Performance>> GetRiderPerformanceTrendAsync(string riderId, int monthCount = 6)
        {
            return await _context.Performances
                .Include(p => p.Rider)
                .Where(p => p.RiderId == riderId)
                .OrderByDescending(p => p.StatsMonth)
                .Take(monthCount)
                .ToListAsync();
        }

        // 计算平均绩效指标 (指定月份)
        public async Task<Dictionary<string, object>> GetAveragePerformanceMetricsAsync(DateTime statsMonth)
        {
            var performances = await _context.Performances
                .Where(p => p.StatsMonth == statsMonth)
                .ToListAsync();

            if (!performances.Any())
            {
                return new Dictionary<string, object>
                {
                    ["AverageTotalOrders"] = 0,
                    ["AverageOnTimeRate"] = 0,
                    ["AverageGoodReviewRate"] = 0,
                    ["AverageBadReviewRate"] = 0,
                    ["AverageIncome"] = 0
                };
            }

            return new Dictionary<string, object>
            {
                ["AverageTotalOrders"] = Math.Round(performances.Average(p => p.TotalOrders), 2),
                ["AverageOnTimeRate"] = Math.Round(performances.Average(p => p.OnTimeRate), 2),
                ["AverageGoodReviewRate"] = Math.Round(performances.Average(p => p.GoodReviewRate), 2),
                ["AverageBadReviewRate"] = Math.Round(performances.Average(p => p.BadReviewRate), 2),
                ["AverageIncome"] = Math.Round(performances.Average(p => p.Income), 2)
            };
        }

        // 获取绩效排名 (指定骑手在指定月份的排名)
        public async Task<Dictionary<string, int>> GetRiderRankingAsync(string riderId, DateTime statsMonth)
        {
            var performances = await _context.Performances
                .Where(p => p.StatsMonth == statsMonth)
                .OrderByDescending(p => p.Income)
                .Select(p => p.RiderId)
                .ToListAsync();

            var incomeRank = performances.IndexOf(riderId) + 1;

            var onTimeRateRank = await _context.Performances
                .Where(p => p.StatsMonth == statsMonth)
                .OrderByDescending(p => p.OnTimeRate)
                .Select(p => p.RiderId)
                .ToListAsync();

            var goodReviewRateRank = await _context.Performances
                .Where(p => p.StatsMonth == statsMonth)
                .OrderByDescending(p => p.GoodReviewRate)
                .Select(p => p.RiderId)
                .ToListAsync();

            var totalOrdersRank = await _context.Performances
                .Where(p => p.StatsMonth == statsMonth)
                .OrderByDescending(p => p.TotalOrders)
                .Select(p => p.RiderId)
                .ToListAsync();

            return new Dictionary<string, int>
            {
                ["IncomeRank"] = incomeRank > 0 ? incomeRank : 0,
                ["OnTimeRateRank"] = onTimeRateRank.IndexOf(riderId) + 1,
                ["GoodReviewRateRank"] = goodReviewRateRank.IndexOf(riderId) + 1,
                ["TotalOrdersRank"] = totalOrdersRank.IndexOf(riderId) + 1
            };
        }
    }
}
