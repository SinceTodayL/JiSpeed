using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JISpeed.Core.Entities.Rider;

namespace JISpeed.Core.Interfaces.IRepositories.Rider
{
    // 绩效仓储接口 - 处理绩效的数据访问操作
    // Performance实体使用复合主键(RiderId, StatsMonth)，所以不能直接继承IBaseRepository<Performance, string>
    public interface IPerformanceRepository
    {
        // === 基础CRUD操作 ===

        // 根据复合主键获取绩效记录
        Task<Performance?> GetByCompositeKeyAsync(string riderId, DateTime statsMonth);

        // 获取所有绩效记录
        Task<List<Performance>> GetAllAsync();

        // 创建绩效记录
        Task<Performance> CreateAsync(Performance entity);

        // 更新绩效记录
        Task<Performance> UpdateAsync(Performance entity);

        // 删除绩效记录
        Task<bool> DeleteAsync(string riderId, DateTime statsMonth);

        // 检查绩效记录是否存在
        Task<bool> ExistsAsync(string riderId, DateTime statsMonth);

        // 获取绩效记录总数
        Task<int> CountAsync();

        // 分页获取绩效记录
        Task<List<Performance>> GetPagedAsync(int pageNumber, int pageSize);

        // 保存更改
        Task<int> SaveChangesAsync();

        // === 业务专用查询方法 ===

        // 根据骑手ID查询绩效记录
        Task<IEnumerable<Performance>> GetByRiderIdAsync(string riderId);

        // 根据统计月份查询绩效记录
        Task<IEnumerable<Performance>> GetByStatsMonthAsync(DateTime statsMonth);

        // 根据统计月份范围查询绩效记录
        Task<IEnumerable<Performance>> GetByMonthRangeAsync(DateTime startMonth, DateTime endMonth);

        // 根据骑手ID和月份范围查询绩效记录
        Task<IEnumerable<Performance>> GetByRiderIdAndMonthRangeAsync(string riderId, DateTime startMonth, DateTime endMonth);

        // 根据总订单量范围查询
        Task<IEnumerable<Performance>> GetByTotalOrdersRangeAsync(int minOrders, int maxOrders);

        // 根据准时率范围查询
        Task<IEnumerable<Performance>> GetByOnTimeRateRangeAsync(decimal minRate, decimal maxRate);

        // 根据好评率范围查询
        Task<IEnumerable<Performance>> GetByGoodReviewRateRangeAsync(decimal minRate, decimal maxRate);

        // 根据收入范围查询
        Task<IEnumerable<Performance>> GetByIncomeRangeAsync(decimal minIncome, decimal maxIncome);

        // 获取最佳绩效骑手 (按指定月份)
        Task<IEnumerable<Performance>> GetTopPerformersByMonthAsync(DateTime statsMonth, int topCount = 10);

        // 获取最佳绩效骑手 (按总订单量)
        Task<IEnumerable<Performance>> GetTopByTotalOrdersAsync(DateTime statsMonth, int topCount = 10);

        // 获取最佳绩效骑手 (按准时率)
        Task<IEnumerable<Performance>> GetTopByOnTimeRateAsync(DateTime statsMonth, int topCount = 10);

        // 获取最佳绩效骑手 (按好评率)
        Task<IEnumerable<Performance>> GetTopByGoodReviewRateAsync(DateTime statsMonth, int topCount = 10);

        // 获取最佳绩效骑手 (按收入)
        Task<IEnumerable<Performance>> GetTopByIncomeAsync(DateTime statsMonth, int topCount = 10);

        // 统计指定月份的绩效概览
        Task<Dictionary<string, object>> GetMonthlyPerformanceOverviewAsync(DateTime statsMonth);

        // 获取骑手绩效趋势 (多个月份)
        Task<IEnumerable<Performance>> GetRiderPerformanceTrendAsync(string riderId, int monthCount = 6);

        // 计算平均绩效指标 (指定月份)
        Task<Dictionary<string, object>> GetAveragePerformanceMetricsAsync(DateTime statsMonth);

        // 获取绩效排名 (指定骑手在指定月份的排名)
        Task<Dictionary<string, int>> GetRiderRankingAsync(string riderId, DateTime statsMonth);
    }
}
