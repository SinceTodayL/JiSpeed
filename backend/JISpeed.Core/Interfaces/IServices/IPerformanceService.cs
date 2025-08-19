using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JISpeed.Core.Entities.Rider;

namespace JISpeed.Core.Interfaces.IServices
{
    // 骑手绩效服务接口 - 定义骑手绩效模块的业务逻辑操作
    public interface IPerformanceService
    {
        // 获取骑手月度绩效
        // <param name="riderId">骑手ID</param>
        // <param name="month">统计月份</param>
        // <returns>绩效实体</returns>
        Task<Performance?> GetRiderMonthlyPerformanceAsync(string riderId, DateTime month);

        // 获取骑手绩效趋势（最近几个月）
        // <param name="riderId">骑手ID</param>
        // <param name="monthCount">月份数量</param>
        // <returns>绩效列表</returns>
        Task<IEnumerable<Performance>> GetRiderPerformanceTrendAsync(string riderId, int monthCount = 6);

        // 获取骑手绩效排名
        // <param name="riderId">骑手ID</param>
        // <param name="month">统计月份</param>
        // <returns>排名信息</returns>
        Task<Dictionary<string, int>> GetRiderRankingAsync(string riderId, DateTime month);

        // 获取绩效优秀骑手列表
        // <param name="month">统计月份</param>
        // <param name="topCount">返回数量</param>
        // <returns>绩效列表</returns>
        Task<IEnumerable<Performance>> GetTopPerformersAsync(DateTime month, int topCount = 10);

        // 获取月度绩效概览
        // <param name="month">统计月份</param>
        // <returns>绩效概览数据</returns>
        Task<Dictionary<string, object>> GetMonthlyPerformanceOverviewAsync(DateTime month);

        // 计算并生成月度绩效数据
        // <param name="riderId">骑手ID</param>
        // <param name="month">统计月份</param>
        // <returns>生成的绩效实体</returns>
        Task<Performance> GenerateMonthlyPerformanceAsync(string riderId, DateTime month);
    }
}