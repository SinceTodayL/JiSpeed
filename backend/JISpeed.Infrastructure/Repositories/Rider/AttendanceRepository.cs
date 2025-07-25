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
    // 考勤仓储实现 - 处理考勤的数据访问操作
    public class AttendanceRepository : BaseRepository<Attendance, string>
    {
        public AttendanceRepository(OracleDbContext context) : base(context)
        {
        }

        // 重写GetByIdAsync和GetWithDetailsAsync以包含关联数据
        public override async Task<Attendance?> GetByIdAsync(string id)
        {
            return await _context.Attendances
                .Include(a => a.Rider)
                .Include(a => a.ScheduleAttendances)
                .FirstOrDefaultAsync(a => a.AttendanceId == id);
        }

        public override async Task<Attendance?> GetWithDetailsAsync(string id)
        {
            return await _context.Attendances
                .Include(a => a.Rider)
                .Include(a => a.ScheduleAttendances)
                .FirstOrDefaultAsync(a => a.AttendanceId == id);
        }

        public override async Task<List<Attendance>> GetAllAsync()
        {
            return await _context.Attendances
                .Include(a => a.Rider)
                .Include(a => a.ScheduleAttendances)
                .ToListAsync();
        }

        // === 业务专用查询方法 ===

        // 根据骑手ID查询考勤记录
        public async Task<IEnumerable<Attendance>> GetByRiderIdAsync(string riderId)
        {
            return await _context.Attendances
                .Include(a => a.Rider)
                .Include(a => a.ScheduleAttendances)
                .Where(a => a.RiderId == riderId)
                .OrderByDescending(a => a.CheckDate)
                .ToListAsync();
        }

        // 根据日期查询考勤记录
        public async Task<IEnumerable<Attendance>> GetByDateAsync(DateTime date)
        {
            return await _context.Attendances
                .Include(a => a.Rider)
                .Include(a => a.ScheduleAttendances)
                .Where(a => a.CheckDate.Date == date.Date)
                .OrderBy(a => a.RiderId)
                .ToListAsync();
        }

        // 根据骑手ID和日期查询考勤记录
        public async Task<Attendance?> GetByRiderIdAndDateAsync(string riderId, DateTime date)
        {
            return await _context.Attendances
                .Include(a => a.Rider)
                .Include(a => a.ScheduleAttendances)
                .FirstOrDefaultAsync(a => a.RiderId == riderId && a.CheckDate.Date == date.Date);
        }

        // 根据时间范围查询考勤记录
        public async Task<IEnumerable<Attendance>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Attendances
                .Include(a => a.Rider)
                .Include(a => a.ScheduleAttendances)
                .Where(a => a.CheckDate.Date >= startDate.Date && a.CheckDate.Date <= endDate.Date)
                .OrderByDescending(a => a.CheckDate)
                .ThenBy(a => a.RiderId)
                .ToListAsync();
        }

        // 根据骑手ID和时间范围查询考勤记录
        public async Task<IEnumerable<Attendance>> GetByRiderIdAndDateRangeAsync(string riderId, DateTime startDate, DateTime endDate)
        {
            return await _context.Attendances
                .Include(a => a.Rider)
                .Include(a => a.ScheduleAttendances)
                .Where(a => a.RiderId == riderId &&
                           a.CheckDate.Date >= startDate.Date &&
                           a.CheckDate.Date <= endDate.Date)
                .OrderByDescending(a => a.CheckDate)
                .ToListAsync();
        }

        // 获取迟到的考勤记录
        public async Task<IEnumerable<Attendance>> GetLateAttendancesAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Attendances
                .Include(a => a.Rider)
                .Include(a => a.ScheduleAttendances)
                .Where(a => a.IsLate == 1);

            if (startDate.HasValue)
                query = query.Where(a => a.CheckDate.Date >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(a => a.CheckDate.Date <= endDate.Value.Date);

            return await query
                .OrderByDescending(a => a.CheckDate)
                .ToListAsync();
        }

        // 获取缺勤的考勤记录
        public async Task<IEnumerable<Attendance>> GetAbsentAttendancesAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Attendances
                .Include(a => a.Rider)
                .Include(a => a.ScheduleAttendances)
                .Where(a => a.IsAbsent == 1);

            if (startDate.HasValue)
                query = query.Where(a => a.CheckDate.Date >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(a => a.CheckDate.Date <= endDate.Value.Date);

            return await query
                .OrderByDescending(a => a.CheckDate)
                .ToListAsync();
        }

        // 计算骑手考勤统计
        public async Task<Dictionary<string, object>> GetAttendanceStatsAsync(string riderId, DateTime startDate, DateTime endDate)
        {
            var attendances = await _context.Attendances
                .Where(a => a.RiderId == riderId &&
                           a.CheckDate.Date >= startDate.Date &&
                           a.CheckDate.Date <= endDate.Date)
                .ToListAsync();

            var totalDays = attendances.Count;
            var lateDays = attendances.Count(a => a.IsLate == 1);
            var absentDays = attendances.Count(a => a.IsAbsent == 1);
            var normalDays = totalDays - lateDays - absentDays;

            var lateRate = totalDays > 0 ? (double)lateDays / totalDays * 100 : 0;
            var absentRate = totalDays > 0 ? (double)absentDays / totalDays * 100 : 0;
            var attendanceRate = totalDays > 0 ? (double)normalDays / totalDays * 100 : 0;

            return new Dictionary<string, object>
            {
                ["TotalDays"] = totalDays,
                ["LateDays"] = lateDays,
                ["AbsentDays"] = absentDays,
                ["NormalDays"] = normalDays,
                ["LateRate"] = Math.Round(lateRate, 2),
                ["AbsentRate"] = Math.Round(absentRate, 2),
                ["AttendanceRate"] = Math.Round(attendanceRate, 2)
            };
        }

        // 获取未签退的考勤记录
        public async Task<IEnumerable<Attendance>> GetNotCheckedOutAsync()
        {
            return await _context.Attendances
                .Include(a => a.Rider)
                .Include(a => a.ScheduleAttendances)
                .Where(a => a.CheckInAt.HasValue && !a.CheckoutAt.HasValue)
                .OrderBy(a => a.CheckInAt)
                .ToListAsync();
        }

        // 根据签到时间范围查询
        public async Task<IEnumerable<Attendance>> GetByCheckInTimeRangeAsync(DateTime startTime, DateTime endTime)
        {
            return await _context.Attendances
                .Include(a => a.Rider)
                .Include(a => a.ScheduleAttendances)
                .Where(a => a.CheckInAt.HasValue && a.CheckInAt >= startTime && a.CheckInAt <= endTime)
                .OrderBy(a => a.CheckInAt)
                .ToListAsync();
        }

        // 根据签退时间范围查询
        public async Task<IEnumerable<Attendance>> GetByCheckOutTimeRangeAsync(DateTime startTime, DateTime endTime)
        {
            return await _context.Attendances
                .Include(a => a.Rider)
                .Include(a => a.ScheduleAttendances)
                .Where(a => a.CheckoutAt.HasValue && a.CheckoutAt >= startTime && a.CheckoutAt <= endTime)
                .OrderBy(a => a.CheckoutAt)
                .ToListAsync();
        }
    }
}
