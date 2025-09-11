using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JISpeed.Core.Entities.Rider;
using JISpeed.Core.Interfaces.IRepositories.Rider;
using JISpeed.Infrastructure.Data;
using JISpeed.Infrastructure.Repositories;

namespace JISpeed.Infrastructure.Repositories.Rider
{
    // 配送分配仓储实现 - 处理配送分配的数据访问操作
    public class AssignmentRepository : BaseRepository<Assignment, string>, IAssignmentRepository
    {
        public AssignmentRepository(OracleDbContext context) : base(context)
        {
        }

        // 重写GetByIdAsync以包含关联数据
        public override async Task<Assignment?> GetByIdAsync(string id)
        {
            return await _context.Assignments
                .Include(a => a.Rider)
                .Include(a => a.Order)
                .FirstOrDefaultAsync(a => a.AssignId == id);
        }

        // 重写GetWithDetailsAsync以包含关联数据
        public override async Task<Assignment?> GetWithDetailsAsync(string id)
        {
            return await _context.Assignments
                .Include(a => a.Rider)
                .Include(a => a.Order)
                .FirstOrDefaultAsync(a => a.AssignId == id);
        }

        // 重写GetAllAsync以包含关联数据
        public override async Task<List<Assignment>> GetAllAsync()
        {
            return await _context.Assignments
                .Include(a => a.Rider)
                .Include(a => a.Order)
                .ToListAsync();
        }

        // === 业务专用查询方法 ===

        // 根据骑手ID查询分配列表
        public async Task<IEnumerable<Assignment>> GetByRiderIdAsync(string riderId)
        {
            return await _context.Assignments
                .Include(a => a.Rider)
                .Include(a => a.Order)
                    .ThenInclude(o => o.Address)  // 添加地址关联
                .Include(a => a.Order)
                    .ThenInclude(o => o.Merchant) // 添加商家关联
                .Where(a => a.RiderId == riderId)
                .OrderByDescending(a => a.AssignedAt)
                .ToListAsync();
        }

        // 根据接单状态查询分配列表
        public async Task<IEnumerable<Assignment>> GetByAcceptedStatusAsync(int acceptedStatus)
        {
            return await _context.Assignments
                .Include(a => a.Rider)
                .Include(a => a.Order)
                .Where(a => a.AcceptedStatus == acceptedStatus)
                .OrderByDescending(a => a.AssignedAt)
                .ToListAsync();
        }

        // 根据分配时间范围查询
        public async Task<IEnumerable<Assignment>> GetByAssignedTimeRangeAsync(DateTime startTime, DateTime endTime)
        {
            return await _context.Assignments
                .Include(a => a.Rider)
                .Include(a => a.Order)
                .Where(a => a.AssignedAt >= startTime && a.AssignedAt <= endTime)
                .OrderByDescending(a => a.AssignedAt)
                .ToListAsync();
        }

        // 根据接单时间范围查询
        public async Task<IEnumerable<Assignment>> GetByAcceptedTimeRangeAsync(DateTime startTime, DateTime endTime)
        {
            return await _context.Assignments
                .Include(a => a.Rider)
                .Include(a => a.Order)
                .Where(a => a.AcceptedAt.HasValue && a.AcceptedAt >= startTime && a.AcceptedAt <= endTime)
                .OrderByDescending(a => a.AcceptedAt)
                .ToListAsync();
        }

        // 根据骑手ID和接单状态查询
        public async Task<IEnumerable<Assignment>> GetByRiderIdAndStatusAsync(string riderId, int acceptedStatus)
        {
            return await _context.Assignments
                .Include(a => a.Rider)
                .Include(a => a.Order)
                .Where(a => a.RiderId == riderId && a.AcceptedStatus == acceptedStatus)
                .OrderByDescending(a => a.AssignedAt)
                .ToListAsync();
        }

        // 获取待接单的分配 (接单状态为0-待接单)
        public async Task<IEnumerable<Assignment>> GetPendingAssignmentsAsync()
        {
            return await _context.Assignments
                .Include(a => a.Rider)
                .Include(a => a.Order)
                .Where(a => a.AcceptedStatus == 0)
                .OrderBy(a => a.AssignedAt)
                .ToListAsync();
        }

        // 获取已接单的分配 (接单状态为1-已接单)
        public async Task<IEnumerable<Assignment>> GetAcceptedAssignmentsAsync()
        {
            return await _context.Assignments
                .Include(a => a.Rider)
                .Include(a => a.Order)
                .Where(a => a.AcceptedStatus == 1)
                .OrderByDescending(a => a.AcceptedAt)
                .ToListAsync();
        }

        // 获取超时的分配
        public async Task<IEnumerable<Assignment>> GetTimeoutAssignmentsAsync()
        {
            var currentTime = DateTime.Now;
            return await _context.Assignments
                .Include(a => a.Rider)
                .Include(a => a.Order)
                .Where(a => a.TimeOut.HasValue &&
                           a.AssignedAt.AddMinutes(a.TimeOut.Value) <= currentTime &&
                           a.AcceptedStatus == 0)
                .OrderBy(a => a.AssignedAt)
                .ToListAsync();
        }

        // 根据骑手ID获取最近的分配
        public async Task<IEnumerable<Assignment>> GetRecentAssignmentsByRiderIdAsync(string riderId, int count = 10)
        {
            return await _context.Assignments
                .Include(a => a.Rider)
                .Include(a => a.Order)
                .Where(a => a.RiderId == riderId)
                .OrderByDescending(a => a.AssignedAt)
                .Take(count)
                .ToListAsync();
        }

        // 统计接单状态分布
        public async Task<Dictionary<int, int>> GetAcceptedStatusCountAsync()
        {
            return await _context.Assignments
                .GroupBy(a => a.AcceptedStatus)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        // 统计骑手接单效率 (指定时间范围内)
        public async Task<Dictionary<string, object>> GetRiderEfficiencyStatsAsync(string riderId, DateTime startTime, DateTime endTime)
        {
            var assignments = await _context.Assignments
                .Where(a => a.RiderId == riderId &&
                           a.AssignedAt >= startTime &&
                           a.AssignedAt <= endTime)
                .ToListAsync();

            var totalAssignments = assignments.Count;
            var acceptedAssignments = assignments.Count(a => a.AcceptedStatus == 1);
            var acceptanceRate = totalAssignments > 0 ? (double)acceptedAssignments / totalAssignments * 100 : 0;

            var acceptedWithTime = assignments.Where(a => a.AcceptedStatus == 1 &&
                                                         a.AcceptedAt.HasValue).ToList();

            var averageResponseTime = acceptedWithTime.Any()
                ? acceptedWithTime.Average(a => (a.AcceptedAt!.Value - a.AssignedAt).TotalMinutes)
                : 0;

            return new Dictionary<string, object>
            {
                ["TotalAssignments"] = totalAssignments,
                ["AcceptedAssignments"] = acceptedAssignments,
                ["AcceptanceRate"] = Math.Round(acceptanceRate, 2),
                ["AverageResponseTimeMinutes"] = Math.Round(averageResponseTime, 2)
            };
        }

        // 获取骑手当前未完成的分配
        public async Task<IEnumerable<Assignment>> GetActiveAssignmentsByRiderIdAsync(string riderId)
        {
            return await _context.Assignments
                .Include(a => a.Rider)
                .Include(a => a.Order)
                .Where(a => a.RiderId == riderId && a.AcceptedStatus == 1)
                .OrderBy(a => a.AcceptedAt)
                .ToListAsync();
        }
    }
}
