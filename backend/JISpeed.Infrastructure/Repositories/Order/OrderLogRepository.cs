using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JISpeed.Core.Entities.Order;
using JISpeed.Core.Interfaces.IRepositories.Order;
using JISpeed.Infrastructure.Data;

namespace JISpeed.Infrastructure.Repositories.Order
{
    // 订单日志仓储实现 - 处理订单日志记录的数据访问操作
    public class OrderLogRepository : IOrderLogRepository
    {
        private readonly OracleDbContext _context;

        public OrderLogRepository(OracleDbContext context)
        {
            _context = context;
        }

        // 根据主键查询订单日志
        public async Task<OrderLog?> GetByIdAsync(string id)
        {
            return await _context.OrderLogs
                .Include(ol => ol.Order)
                .ThenInclude(o => o.User)
                .FirstOrDefaultAsync(ol => ol.LogId == id);
        }

        // 查询订单日志包含详细信息
        public async Task<OrderLog?> GetWithDetailsAsync(string id)
        {
            return await _context.OrderLogs
                .Include(ol => ol.Order)
                .ThenInclude(o => o.User)
                .Include(ol => ol.Order)
                .ThenInclude(o => o.OrderDishes)
                .ThenInclude(od => od.Dish)
                .FirstOrDefaultAsync(ol => ol.LogId == id);
        }

        // 获取所有订单日志
        public async Task<List<OrderLog>> GetAllAsync()
        {
            return await _context.OrderLogs
                .Include(ol => ol.Order)
                .ThenInclude(o => o.User)
                .OrderByDescending(ol => ol.LoggedAt)
                .ToListAsync();
        }

        // 检查订单日志是否存在
        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.OrderLogs.AnyAsync(ol => ol.LogId == id);
        }

        // 创建新订单日志
        public async Task<OrderLog> CreateAsync(OrderLog entity)
        {
            var result = await _context.OrderLogs.AddAsync(entity);
            return result.Entity;
        }

        // 更新订单日志信息
        public async Task<OrderLog> UpdateAsync(OrderLog entity)
        {
            var result = _context.OrderLogs.Update(entity);
            await Task.CompletedTask;
            return result.Entity;
        }

        // 删除订单日志
        public async Task<bool> DeleteAsync(string id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.OrderLogs.Remove(entity);
                return true;
            }
            return false;
        }

        // 保存更改
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // === 业务专用查询方法 ===

        // 根据订单ID查询日志列表
        public async Task<IEnumerable<OrderLog>> GetByOrderIdAsync(string orderId)
        {
            return await _context.OrderLogs
                .Include(ol => ol.Order)
                .ThenInclude(o => o.User)
                .Where(ol => ol.OrderId == orderId)
                .OrderByDescending(ol => ol.LoggedAt)
                .ToListAsync();
        }

        // 根据状态码查询日志列表
        public async Task<IEnumerable<OrderLog>> GetByStatusCodeAsync(int statusCode)
        {
            return await _context.OrderLogs
                .Include(ol => ol.Order)
                .ThenInclude(o => o.User)
                .Where(ol => ol.StatusCode == statusCode)
                .OrderByDescending(ol => ol.LoggedAt)
                .ToListAsync();
        }

        // 根据触发方查询日志列表
        public async Task<IEnumerable<OrderLog>> GetByActorAsync(string actor)
        {
            return await _context.OrderLogs
                .Include(ol => ol.Order)
                .ThenInclude(o => o.User)
                .Where(ol => ol.Actor == actor)
                .OrderByDescending(ol => ol.LoggedAt)
                .ToListAsync();
        }

        // 根据时间范围查询日志列表
        public async Task<IEnumerable<OrderLog>> GetByTimeRangeAsync(DateTime startTime, DateTime endTime)
        {
            return await _context.OrderLogs
                .Include(ol => ol.Order)
                .ThenInclude(o => o.User)
                .Where(ol => ol.LoggedAt >= startTime && ol.LoggedAt <= endTime)
                .OrderByDescending(ol => ol.LoggedAt)
                .ToListAsync();
        }

        // 根据订单ID和状态码查询日志
        public async Task<IEnumerable<OrderLog>> GetByOrderIdAndStatusCodeAsync(string orderId, int statusCode)
        {
            return await _context.OrderLogs
                .Include(ol => ol.Order)
                .ThenInclude(o => o.User)
                .Where(ol => ol.OrderId == orderId && ol.StatusCode == statusCode)
                .OrderByDescending(ol => ol.LoggedAt)
                .ToListAsync();
        }

        // 根据订单ID和时间范围查询日志
        public async Task<IEnumerable<OrderLog>> GetByOrderIdAndTimeRangeAsync(string orderId, DateTime startTime, DateTime endTime)
        {
            return await _context.OrderLogs
                .Include(ol => ol.Order)
                .ThenInclude(o => o.User)
                .Where(ol => ol.OrderId == orderId && ol.LoggedAt >= startTime && ol.LoggedAt <= endTime)
                .OrderByDescending(ol => ol.LoggedAt)
                .ToListAsync();
        }

        // 根据备注内容搜索日志
        public async Task<IEnumerable<OrderLog>> SearchByRemarkAsync(string keyword)
        {
            return await _context.OrderLogs
                .Include(ol => ol.Order)
                .ThenInclude(o => o.User)
                .Where(ol => ol.Remark != null && ol.Remark.Contains(keyword))
                .OrderByDescending(ol => ol.LoggedAt)
                .ToListAsync();
        }

        // 获取最近的日志记录
        public async Task<IEnumerable<OrderLog>> GetRecentLogsAsync(int count = 100)
        {
            return await _context.OrderLogs
                .Include(ol => ol.Order)
                .ThenInclude(o => o.User)
                .OrderByDescending(ol => ol.LoggedAt)
                .Take(count)
                .ToListAsync();
        }

        // 获取指定订单的最新日志
        public async Task<OrderLog?> GetLatestLogByOrderIdAsync(string orderId)
        {
            return await _context.OrderLogs
                .Include(ol => ol.Order)
                .ThenInclude(o => o.User)
                .Where(ol => ol.OrderId == orderId)
                .OrderByDescending(ol => ol.LoggedAt)
                .FirstOrDefaultAsync();
        }

        // 统计状态码分布
        public async Task<Dictionary<int, int>> GetStatusCodeCountAsync()
        {
            var statusCounts = await _context.OrderLogs
                .GroupBy(ol => ol.StatusCode)
                .Select(g => new { StatusCode = g.Key, Count = g.Count() })
                .ToListAsync();

            return statusCounts.ToDictionary(sc => sc.StatusCode, sc => sc.Count);
        }

        // 统计触发方活动分布
        public async Task<Dictionary<string, int>> GetActorActivityCountAsync()
        {
            var actorCounts = await _context.OrderLogs
                .GroupBy(ol => ol.Actor)
                .Select(g => new { Actor = g.Key, Count = g.Count() })
                .ToListAsync();

            return actorCounts.ToDictionary(ac => ac.Actor, ac => ac.Count);
        }

        // 获取指定时间范围内的日志统计
        public async Task<Dictionary<string, object>> GetLogStatsByTimeRangeAsync(DateTime startTime, DateTime endTime)
        {
            var logs = await _context.OrderLogs
                .Where(ol => ol.LoggedAt >= startTime && ol.LoggedAt <= endTime)
                .ToListAsync();

            var stats = new Dictionary<string, object>
            {
                ["TotalLogs"] = logs.Count,
                ["UniqueOrders"] = logs.Select(l => l.OrderId).Distinct().Count(),
                ["UniqueActors"] = logs.Select(l => l.Actor).Distinct().Count(),
                ["StatusCodeDistribution"] = logs.GroupBy(l => l.StatusCode)
                                                .ToDictionary(g => g.Key.ToString(), g => g.Count()),
                ["ActorDistribution"] = logs.GroupBy(l => l.Actor)
                                           .ToDictionary(g => g.Key, g => g.Count()),
                ["TimeRange"] = new { StartTime = startTime, EndTime = endTime }
            };

            return stats;
        }
    }
}
