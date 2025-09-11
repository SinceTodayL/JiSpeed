using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JISpeed.Core.Entities.Order;

namespace JISpeed.Core.Interfaces.IRepositories.Order
{
    // 订单日志仓储接口 - 处理订单日志记录的数据访问操作
    public interface IOrderLogRepository : IBaseRepository<OrderLog, string>
    {
        // === 业务专用查询方法 ===

        // 根据订单ID查询日志列表
        Task<IEnumerable<OrderLog>> GetByOrderIdAsync(string orderId);

        // 根据状态码查询日志列表
        Task<IEnumerable<OrderLog>> GetByStatusCodeAsync(int statusCode);

        // 根据触发方查询日志列表
        Task<IEnumerable<OrderLog>> GetByActorAsync(string actor);

        // 根据时间范围查询日志列表
        Task<IEnumerable<OrderLog>> GetByTimeRangeAsync(DateTime startTime, DateTime endTime);

        // 根据订单ID和状态码查询日志
        Task<IEnumerable<OrderLog>> GetByOrderIdAndStatusCodeAsync(string orderId, int statusCode);

        // 根据订单ID和时间范围查询日志
        Task<IEnumerable<OrderLog>> GetByOrderIdAndTimeRangeAsync(string orderId, DateTime startTime, DateTime endTime);

        // 根据备注内容搜索日志
        Task<IEnumerable<OrderLog>> SearchByRemarkAsync(string keyword);

        // 获取最近的日志记录
        Task<IEnumerable<OrderLog>> GetRecentLogsAsync(int count = 100);

        // 获取指定订单的最新日志
        Task<OrderLog?> GetLatestLogByOrderIdAsync(string orderId);

        // 统计状态码分布
        Task<Dictionary<int, int>> GetStatusCodeCountAsync();

        // 统计触发方活动分布
        Task<Dictionary<string, int>> GetActorActivityCountAsync();

        // 获取指定时间范围内的日志统计
        Task<Dictionary<string, object>> GetLogStatsByTimeRangeAsync(DateTime startTime, DateTime endTime);
    }
}
