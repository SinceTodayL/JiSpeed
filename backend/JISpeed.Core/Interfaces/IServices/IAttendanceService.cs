using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JISpeed.Core.Entities.Rider;

namespace JISpeed.Core.Interfaces.IServices
{
    // 考勤服务接口 - 处理考勤业务逻辑
    public interface IAttendanceService
    {
        // === 基础CRUD操作 ===
        Task<Attendance> CreateAttendanceAsync(string riderId, DateTime checkDate);
        Task<Attendance?> GetAttendanceByIdAsync(string attendanceId);
        Task<List<Attendance>> GetAllAttendancesAsync();
        Task<Attendance> UpdateAttendanceAsync(Attendance attendance);
        Task<bool> DeleteAttendanceAsync(string attendanceId);

        // === 签到签退操作 ===
        Task<Attendance> CheckInAsync(string riderId, DateTime checkInTime);
        Task<Attendance> CheckOutAsync(string riderId, DateTime checkOutTime);
        Task<Attendance?> GetTodayAttendanceAsync(string riderId);

        // === 查询操作 ===
        Task<List<Attendance>> GetRiderAttendancesAsync(string riderId);
        Task<List<Attendance>> GetAttendancesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<Attendance>> GetRiderAttendancesByDateRangeAsync(string riderId, DateTime startDate, DateTime endDate);
        Task<Attendance?> GetRiderAttendanceByDateAsync(string riderId, DateTime checkDate);

        // === 考勤状态查询 ===
        Task<List<Attendance>> GetLateAttendancesAsync();
        Task<List<Attendance>> GetAbsentAttendancesAsync();
        Task<List<Attendance>> GetNotCheckedOutAttendancesAsync();
        Task<List<Attendance>> GetTodayAttendancesAsync();

        // === 考勤统计 ===
        Task<Dictionary<string, int>> GetLateCountByRiderAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<string, int>> GetAbsentCountByRiderAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<string, object>> GetRiderAttendanceStatsAsync(string riderId, DateTime startDate, DateTime endDate);

        // === 业务逻辑 ===
        Task<bool> IsRiderLateAsync(string riderId, DateTime checkInTime, DateTime scheduledStartTime);
        Task MarkAbsentAsync(string riderId, DateTime checkDate); // 修改这里：Task 而不是 Task<void>
        Task<TimeSpan?> CalculateWorkingHoursAsync(string riderId, DateTime checkDate);
        Task<List<Attendance>> GetAttendanceReportAsync(DateTime startDate, DateTime endDate);
    }
}
