using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JISpeed.Core.Entities.Rider;
using JISpeed.Core.Interfaces.IRepositories;
using JISpeed.Infrastructure.Data;

namespace JISpeed.Infrastructure.Repositories
{
    // 骑手仓储实现
    public class RiderRepository : IRiderRepository
    {
        private readonly OracleDbContext _dbContext;

        public RiderRepository(OracleDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // 根据ID获取骑手信息
        // <param name="riderId">骑手ID</param>
        // <returns>骑手实体</returns>
        public async Task<Rider> GetByIdAsync(string riderId)
        {
            return await _dbContext.Riders
                .Include(r => r.ApplicationUser)
                .FirstOrDefaultAsync(r => r.RiderId == riderId);
        }

        // 根据手机号检查骑手是否存在
        // <param name="phoneNumber">手机号</param>
        // <returns>是否存在</returns>
        public async Task<bool> ExistsByPhoneAsync(string phoneNumber)
        {
            return await _dbContext.Riders
                .AnyAsync(r => r.PhoneNumber == phoneNumber);
        }

        // 添加骑手
        // <param name="rider">骑手实体</param>
        public async Task AddAsync(Rider rider)
        {
            await _dbContext.Riders.AddAsync(rider);
            await _dbContext.SaveChangesAsync();
        }

        // 更新骑手信息
        // <param name="rider">骑手实体</param>
        public async Task UpdateAsync(Rider rider)
        {
            _dbContext.Riders.Update(rider);
            await _dbContext.SaveChangesAsync();
        }

        // 获取骑手的订单分配列表
        // <param name="riderId">骑手ID</param>
        // <returns>订单分配列表</returns>
        public async Task<IEnumerable<Assignment>> GetAssignmentsAsync(string riderId)
        {
            return await _dbContext.Assignments
                .Include(a => a.Order)
                .ThenInclude(o => o.Address)
                .Where(a => a.RiderId == riderId)
                .OrderByDescending(a => a.AssignedAt)
                .ToListAsync();
        }

        // 根据ID获取订单分配
        // <param name="assignId">分配ID</param>
        // <returns>订单分配</returns>
        public async Task<Assignment> GetAssignmentByIdAsync(string assignId)
        {
            return await _dbContext.Assignments
                .Include(a => a.Order)
                .FirstOrDefaultAsync(a => a.AssignId == assignId);
        }

        // 更新订单分配
        // <param name="assignment">订单分配实体</param>
        public async Task UpdateAssignmentAsync(Assignment assignment)
        {
            _dbContext.Assignments.Update(assignment);
            await _dbContext.SaveChangesAsync();
        }

        // 获取骑手的考勤记录
        // <param name="riderId">骑手ID</param>
        // <param name="startDate">开始日期</param>
        // <param name="endDate">结束日期</param>
        // <returns>考勤记录列表</returns>
        public async Task<IEnumerable<Attendance>> GetAttendancesAsync(string riderId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbContext.Attendances
                .Where(a => a.RiderId == riderId);

            if (startDate.HasValue)
            {
                query = query.Where(a => a.CheckDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(a => a.CheckDate <= endDate.Value);
            }

            return await query
                .OrderByDescending(a => a.CheckDate)
                .ToListAsync();
        }

        // 添加考勤记录
        // <param name="attendance">考勤记录</param>
        public async Task AddAttendanceAsync(Attendance attendance)
        {
            await _dbContext.Attendances.AddAsync(attendance);
            await _dbContext.SaveChangesAsync();
        }

        // 更新考勤记录
        // <param name="attendance">考勤记录</param>
        public async Task UpdateAttendanceAsync(Attendance attendance)
        {
            _dbContext.Attendances.Update(attendance);
            await _dbContext.SaveChangesAsync();
        }

        // 获取骑手的绩效记录
        // <param name="riderId">骑手ID</param>
        // <param name="year">年份</param>
        // <param name="month">月份</param>
        // <returns>绩效记录</returns>
        public async Task<Performance> GetPerformanceAsync(string riderId, int? year = null, int? month = null)
        {
            var query = _dbContext.Performances
                .Where(p => p.RiderId == riderId);

            if (year.HasValue && month.HasValue)
            {
                // 创建指定年月的日期
                var targetDate = new DateTime(year.Value, month.Value, 1);
                query = query.Where(p => p.StatsMonth.Year == targetDate.Year && p.StatsMonth.Month == targetDate.Month);
            }

            return await query
                .OrderByDescending(p => p.StatsMonth)
                .FirstOrDefaultAsync();
        }

        // 获取骑手的排班列表
        // <param name="riderId">骑手ID</param>
        // <param name="startDate">开始日期</param>
        // <param name="endDate">结束日期</param>
        // <returns>排班列表</returns>
        public async Task<IEnumerable<Schedule>> GetSchedulesAsync(string riderId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbContext.RiderSchedules
                .Where(rs => rs.RiderId == riderId)
                .Select(rs => rs.Schedule);

            if (startDate.HasValue)
            {
                query = query.Where(s => s.WorkDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(s => s.WorkDate <= endDate.Value);
            }

            return await query
                .OrderBy(s => s.WorkDate)
                .ToListAsync();
        }

        // 为骑手分配排班
        // <param name="riderSchedule">骑手排班关联</param>
        public async Task AssignScheduleAsync(RiderSchedule riderSchedule)
        {
            await _dbContext.RiderSchedules.AddAsync(riderSchedule);
            await _dbContext.SaveChangesAsync();
        }

        // 取消骑手排班
        // <param name="riderId">骑手ID</param>
        // <param name="scheduleId">排班ID</param>
        public async Task RemoveScheduleAsync(string riderId, string scheduleId)
        {
            var riderSchedule = await _dbContext.RiderSchedules
                .FirstOrDefaultAsync(rs => rs.RiderId == riderId && rs.ScheduleId == scheduleId);

            if (riderSchedule != null)
            {
                _dbContext.RiderSchedules.Remove(riderSchedule);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
