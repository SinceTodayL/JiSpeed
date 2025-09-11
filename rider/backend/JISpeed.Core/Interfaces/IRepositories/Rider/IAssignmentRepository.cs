using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JISpeed.Core.Entities.Rider;

namespace JISpeed.Core.Interfaces.IRepositories.Rider
{
    // 配送分配仓储接口 - 处理配送分配的数据访问操作
    public interface IAssignmentRepository : IBaseRepository<Assignment, string>
    {
        // === 业务专用查询方法 ===

        // 根据骑手ID查询分配列表
        Task<IEnumerable<Assignment>> GetByRiderIdAsync(string riderId);

        // 根据接单状态查询分配列表
        Task<IEnumerable<Assignment>> GetByAcceptedStatusAsync(int acceptedStatus);

        // 根据分配时间范围查询
        Task<IEnumerable<Assignment>> GetByAssignedTimeRangeAsync(DateTime startTime, DateTime endTime);

        // 根据接单时间范围查询
        Task<IEnumerable<Assignment>> GetByAcceptedTimeRangeAsync(DateTime startTime, DateTime endTime);

        // 根据骑手ID和接单状态查询
        Task<IEnumerable<Assignment>> GetByRiderIdAndStatusAsync(string riderId, int acceptedStatus);

        // 获取待接单的分配 (接单状态为0-待接单)
        Task<IEnumerable<Assignment>> GetPendingAssignmentsAsync();

        // 获取已接单的分配 (接单状态为1-已接单)
        Task<IEnumerable<Assignment>> GetAcceptedAssignmentsAsync();

        // 获取超时的分配
        Task<IEnumerable<Assignment>> GetTimeoutAssignmentsAsync();

        // 根据骑手ID获取最近的分配
        Task<IEnumerable<Assignment>> GetRecentAssignmentsByRiderIdAsync(string riderId, int count = 10);

        // 统计接单状态分布
        Task<Dictionary<int, int>> GetAcceptedStatusCountAsync();

        // 统计骑手接单效率 (指定时间范围内)
        Task<Dictionary<string, object>> GetRiderEfficiencyStatsAsync(string riderId, DateTime startTime, DateTime endTime);

        // 获取骑手当前未完成的分配
        Task<IEnumerable<Assignment>> GetActiveAssignmentsByRiderIdAsync(string riderId);
    }
}
