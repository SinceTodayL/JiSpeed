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
    // 考勤仓储实现 - 处理考勤的数据访问操作
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly OracleDbContext _context;

        public AttendanceRepository(OracleDbContext context)
        {
            _context = context;
        }

        // ===== 实现IBaseRepository接口 =====

        public async Task<Attendance?> GetByIdAsync(string id)
        {
            return await _context.Attendances
                .Include(a => a.Rider)
                .Include(a => a.ScheduleAttendances)
                .FirstOrDefaultAsync(a => a.AttendanceId == id);
        }

        public async Task<Attendance?> GetWithDetailsAsync(string id)
        {
            return await _context.Attendances
                .Include(a => a.Rider)
                .Include(a => a.ScheduleAttendances)
                .FirstOrDefaultAsync(a => a.AttendanceId == id);
        }

        public async Task<List<Attendance>> GetAllAsync()
        {
            return await _context.Attendances
                .Include(a => a.Rider)
                .Include(a => a.ScheduleAttendances)
                .ToListAsync();
        }

        public async Task<Attendance> CreateAsync(Attendance entity)
        {
            await _context.Attendances.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Attendance> AddAsync(Attendance entity)
        {
            await _context.Attendances.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Attendance> UpdateAsync(Attendance entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            await Task.CompletedTask;
            return entity;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance != null)
            {
                _context.Attendances.Remove(attendance);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<int> CountAsync()
        {
            return await _context.Attendances.CountAsync();
        }

        public async Task<List<Attendance>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _context.Attendances
                .Include(a => a.Rider)
                .Include(a => a.ScheduleAttendances)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Attendances.AnyAsync(a => a.AttendanceId == id);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // ===== 业务专用方法 =====

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

        // 根据考勤日期查询考勤记录
        public async Task<IEnumerable<Attendance>> GetByCheckDateAsync(DateTime checkDate)
        {
            var date = checkDate.Date;
            return await _context.Attendances
                .Include(a => a.Rider)
                .Include(a => a.ScheduleAttendances)
                .Where(a => a.CheckDate.Date == date)
                .OrderBy(a => a.CheckInAt)
                .ToListAsync();
        }

        // 根据考勤日期范围查询考勤记录
        public async Task<IEnumerable<Attendance>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var start = startDate.Date;
            var end = endDate.Date;
            return await _context.Attendances
                .Include(a => a.Rider)
                .Include(a => a.ScheduleAttendances)
                .Where(a => a.CheckDate.Date >= start && a.CheckDate.Date <= end)
                .OrderByDescending(a => a.CheckDate)
                .ToListAsync();
        }

        // 根据骑手ID和日期范围查询考勤记录
        public async Task<IEnumerable<Attendance>> GetByRiderIdAndDateRangeAsync(string riderId, DateTime startDate, DateTime endDate)
        {
            var start = startDate.Date;
            var end = endDate.Date;
            return await _context.Attendances
                .Include(a => a.Rider)
                .Include(a => a.ScheduleAttendances)
                .Where(a => a.RiderId == riderId && a.CheckDate.Date >= start && a.CheckDate.Date <= end)
                .OrderByDescending(a => a.CheckDate)
                .ToListAsync();
        }

        // 获取迟到记录
        public async Task<IEnumerable<Attendance>> GetLateAttendancesAsync()
        {
            return await _context.Attendances
                .Include(a => a.Rider)
                .Include(a => a.ScheduleAttendances)
                .Where(a => a.IsLate == 1)
                .OrderByDescending(a => a.CheckDate)
                .ToListAsync();
        }

        // 获取缺勤记录
        public async Task<IEnumerable<Attendance>> GetAbsentAttendancesAsync()
        {
            return await _context.Attendances
                .Include(a => a.Rider)
                .Include(a => a.ScheduleAttendances)
                .Where(a => a.IsAbsent == 1)
                .OrderByDescending(a => a.CheckDate)
                .ToListAsync();
        }

        // 根据是否迟到状态查询
        public async Task<IEnumerable<Attendance>> GetByLateStatusAsync(int isLate)
        {
            return await _context.Attendances
                .Include(a => a.Rider)
                .Include(a => a.ScheduleAttendances)
                .Where(a => a.IsLate == isLate)
                .OrderByDescending(a => a.CheckDate)
                .ToListAsync();
        }

        // 根据是否缺勤状态查询
        public async Task<IEnumerable<Attendance>> GetByAbsentStatusAsync(int isAbsent)
        {
            return await _context.Attendances
                .Include(a => a.Rider)
                .Include(a => a.ScheduleAttendances)
                .Where(a => a.IsAbsent == isAbsent)
                .OrderByDescending(a => a.CheckDate)
                .ToListAsync();
        }

        // 获取骑手指定日期的考勤记录
        public async Task<Attendance?> GetRiderAttendanceByDateAsync(string riderId, DateTime checkDate)
        {
            var date = checkDate.Date;
            return await _context.Attendances
                .Include(a => a.Rider)
                .Include(a => a.ScheduleAttendances)
                .FirstOrDefaultAsync(a => a.RiderId == riderId && a.CheckDate.Date == date);
        }

        // 获取今日考勤记录
        public async Task<IEnumerable<Attendance>> GetTodayAttendancesAsync()
        {
            var today = DateTime.Today;
            return await _context.Attendances
                .Include(a => a.Rider)
                .Include(a => a.ScheduleAttendances)
                .Where(a => a.CheckDate.Date == today)
                .OrderBy(a => a.CheckInAt)
                .ToListAsync();
        }

        // 统计迟到次数 (指定时间范围内)
        public async Task<Dictionary<string, int>> GetLateCountByRiderAsync(DateTime startDate, DateTime endDate)
        {
            var start = startDate.Date;
            var end = endDate.Date;
            return await _context.Attendances
                .Where(a => a.IsLate == 1 && a.CheckDate.Date >= start && a.CheckDate.Date <= end)
                .GroupBy(a => a.RiderId)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        // 统计缺勤次数 (指定时间范围内)
        public async Task<Dictionary<string, int>> GetAbsentCountByRiderAsync(DateTime startDate, DateTime endDate)
        {
            var start = startDate.Date;
            var end = endDate.Date;
            return await _context.Attendances
                .Where(a => a.IsAbsent == 1 && a.CheckDate.Date >= start && a.CheckDate.Date <= end)
                .GroupBy(a => a.RiderId)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        // 获取骑手的考勤统计信息
        public async Task<Dictionary<string, object>> GetRiderAttendanceStatsAsync(string riderId, DateTime startDate, DateTime endDate)
        {
            var start = startDate.Date;
            var end = endDate.Date;

            var attendances = await _context.Attendances
                .Where(a => a.RiderId == riderId && a.CheckDate.Date >= start && a.CheckDate.Date <= end)
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
