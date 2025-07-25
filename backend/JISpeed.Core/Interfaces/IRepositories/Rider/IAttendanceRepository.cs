using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JISpeed.Core.Entities.Rider;

namespace JISpeed.Core.Interfaces.IRepositories.Rider
{
    // 考勤仓储接口 - 处理考勤的数据访问操作
    public interface IAttendanceRepository : IBaseRepository<Attendance, string>
    {
        // === 业务专用查询方法 ===

        // 根据骑手ID查询考勤记录
        Task<IEnumerable<Attendance>> GetByRiderIdAsync(string riderId);

        // 根据考勤日期查询考勤记录
        Task<IEnumerable<Attendance>> GetByCheckDateAsync(DateTime checkDate);

        // 根据考勤日期范围查询考勤记录
        Task<IEnumerable<Attendance>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        // 根据骑手ID和日期范围查询考勤记录
        Task<IEnumerable<Attendance>> GetByRiderIdAndDateRangeAsync(string riderId, DateTime startDate, DateTime endDate);

        // 获取迟到记录
        Task<IEnumerable<Attendance>> GetLateAttendancesAsync();

        // 获取缺勤记录
        Task<IEnumerable<Attendance>> GetAbsentAttendancesAsync();

        // 根据是否迟到状态查询
        Task<IEnumerable<Attendance>> GetByLateStatusAsync(int isLate);

        // 根据是否缺勤状态查询
        Task<IEnumerable<Attendance>> GetByAbsentStatusAsync(int isAbsent);

        // 获取骑手指定日期的考勤记录
        Task<Attendance?> GetRiderAttendanceByDateAsync(string riderId, DateTime checkDate);

        // 获取今日考勤记录
        Task<IEnumerable<Attendance>> GetTodayAttendancesAsync();

        // 统计迟到次数 (指定时间范围内)
        Task<Dictionary<string, int>> GetLateCountByRiderAsync(DateTime startDate, DateTime endDate);

        // 统计缺勤次数 (指定时间范围内)
        Task<Dictionary<string, int>> GetAbsentCountByRiderAsync(DateTime startDate, DateTime endDate);

        // 获取骑手的考勤统计信息
        Task<Dictionary<string, object>> GetRiderAttendanceStatsAsync(string riderId, DateTime startDate, DateTime endDate);

        // 获取未签退的考勤记录
        Task<IEnumerable<Attendance>> GetNotCheckedOutAsync();

        // 根据签到时间范围查询
        Task<IEnumerable<Attendance>> GetByCheckInTimeRangeAsync(DateTime startTime, DateTime endTime);

        // 根据签退时间范围查询
        Task<IEnumerable<Attendance>> GetByCheckOutTimeRangeAsync(DateTime startTime, DateTime endTime);
    }
}
