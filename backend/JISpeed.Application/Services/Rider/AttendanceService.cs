using JISpeed.Core.Constants;
using JISpeed.Core.Entities.Rider;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Interfaces.IRepositories.Rider;
using JISpeed.Core.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JISpeed.Application.Services.Rider
{
    // 考勤服务实现 - 处理考勤业务逻辑
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IRiderRepository _riderRepository;

        public AttendanceService(
            IAttendanceRepository attendanceRepository,
            IRiderRepository riderRepository)
        {
            _attendanceRepository = attendanceRepository;
            _riderRepository = riderRepository;
        }

        // === 基础CRUD操作 ===
        public async Task<Attendance> CreateAttendanceAsync(string riderId, DateTime checkDate)
        {
            // 验证骑手是否存在
            var rider = await _riderRepository.GetByIdAsync(riderId);
            if (rider == null)
            {
                throw RiderExceptions.RiderNotFound(riderId);
            }

            // 检查是否已有当日考勤记录
            var existingAttendance = await _attendanceRepository.GetRiderAttendanceByDateAsync(riderId, checkDate.Date);
            if (existingAttendance != null)
            {
                throw new InvalidOperationException($"骑手 {riderId} 在 {checkDate:yyyy-MM-dd} 已有考勤记录");
            }

            // 创建考勤记录
            var attendance = new Attendance(
                riderId,
                checkDate.Date,
                0,  // isLate
                1   // isAbsent
            )
            {
                AttendanceId = Guid.NewGuid().ToString("N"),
                RiderId = riderId,
                CheckDate = checkDate.Date,  
                IsLate = 0,
                IsAbsent = 1,
                CheckInAt = null,
                CheckoutAt = null,
                Rider = rider
            };
            var createdAttendance = await _attendanceRepository.CreateAsync(attendance);
            await _attendanceRepository.SaveChangesAsync();

            return createdAttendance;
        }

        public async Task<Attendance?> GetAttendanceByIdAsync(string attendanceId)
        {
            return await _attendanceRepository.GetByIdAsync(attendanceId);
        }

        public async Task<List<Attendance>> GetAllAttendancesAsync()
        {
            var attendances = await _attendanceRepository.GetAllAsync();
            return attendances.ToList();
        }

        public async Task<Attendance> UpdateAttendanceAsync(Attendance attendance)
        {
            var existingAttendance = await _attendanceRepository.GetByIdAsync(attendance.AttendanceId);
            if (existingAttendance == null)
            {
                throw new ArgumentException($"考勤记录 {attendance.AttendanceId} 不存在");
            }

            var updatedAttendance = await _attendanceRepository.UpdateAsync(attendance);
            await _attendanceRepository.SaveChangesAsync();
            return updatedAttendance;
        }

        public async Task<bool> DeleteAttendanceAsync(string attendanceId)
        {
            var attendance = await _attendanceRepository.GetByIdAsync(attendanceId);
            if (attendance == null)
            {
                return false;
            }

            await _attendanceRepository.DeleteAsync(attendanceId);
            await _attendanceRepository.SaveChangesAsync();
            return true;
        }

        // === 签到签退操作 ===
        public async Task<Attendance> CheckInAsync(string riderId, DateTime checkInTime)
        {
            // 验证骑手是否存在
            var rider = await _riderRepository.GetByIdAsync(riderId);
            if (rider == null)
            {
                throw RiderExceptions.RiderNotFound(riderId);
            }

            var checkDate = checkInTime.Date;

            // 查找或创建当日考勤记录
            var attendance = await _attendanceRepository.GetRiderAttendanceByDateAsync(riderId, checkDate);
            bool isNewRecord = false;
            if (attendance == null)
            {
                // 创建新的考勤记录
                attendance = new Attendance(riderId, checkDate, 0, 0, checkInTime)
                {
                    AttendanceId = Guid.NewGuid().ToString("N"),
                    RiderId = riderId,
                    CheckDate = checkDate,
                    IsLate = 0,
                    IsAbsent = 0,
                    CheckInAt = checkInTime,
                    CheckoutAt = null,
                    Rider = rider
                };
                isNewRecord = true; // 标记为新记录
            }
            else
            {
                // 检查是否已经签到
                if (attendance.CheckInAt.HasValue)
                {
                    throw new BusinessException(ErrorCodes.UnsupportedOperation,
                        $"骑手 {riderId} 今日已签到，签到时间：{attendance.CheckInAt:yyyy-MM-dd HH:mm:ss}");
                }

                // 检查是否已经签退（防止重复操作）
                if (attendance.CheckoutAt.HasValue)
                {
                    throw new BusinessException(ErrorCodes.UnsupportedOperation,
                        $"骑手 {riderId} 今日已签退，无法再次签到");
                }

                // 更新签到时间
                attendance.CheckInAt = checkInTime;
            }

            // 判断是否迟到 (假设上班时间为9:00)
            var scheduledStartTime = checkDate.AddHours(9);
            if (checkInTime > scheduledStartTime)
            {
                attendance.IsLate = 1;
            }

            // 设置为非缺勤状态
            attendance.IsAbsent = 0;

            // 根据是否为新记录选择不同的保存方法
            Attendance resultAttendance;
            if (isNewRecord)
            {
                resultAttendance = await _attendanceRepository.CreateAsync(attendance);
            }
            else
            {
                resultAttendance = await _attendanceRepository.UpdateAsync(attendance);
            }
            await _attendanceRepository.SaveChangesAsync();
            return resultAttendance;
        }

        public async Task<Attendance> CheckOutAsync(string riderId, DateTime checkOutTime)
        {
            var checkDate = checkOutTime.Date;
            var attendance = await _attendanceRepository.GetRiderAttendanceByDateAsync(riderId, checkDate);

            if (attendance == null)
            {
                throw new InvalidOperationException($"骑手 {riderId} 今日未签到，无法签退");
            }

            if (attendance.CheckoutAt.HasValue)
            {
                throw new InvalidOperationException($"骑手 {riderId} 今日已签退");
            }

            attendance.CheckoutAt = checkOutTime;
            var updatedAttendance = await _attendanceRepository.UpdateAsync(attendance);
            await _attendanceRepository.SaveChangesAsync();
            return updatedAttendance;
        }

        public async Task<Attendance?> GetTodayAttendanceAsync(string riderId)
        {
            return await _attendanceRepository.GetRiderAttendanceByDateAsync(riderId, DateTime.Today);
        }

        // === 查询操作 ===
        public async Task<List<Attendance>> GetRiderAttendancesAsync(string riderId)
        {
            var attendances = await _attendanceRepository.GetByRiderIdAsync(riderId);
            return attendances.ToList();
        }

        public async Task<List<Attendance>> GetAttendancesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var attendances = await _attendanceRepository.GetByDateRangeAsync(startDate, endDate);
            return attendances.ToList();
        }

        public async Task<List<Attendance>> GetRiderAttendancesByDateRangeAsync(string riderId, DateTime startDate, DateTime endDate)
        {
            var attendances = await _attendanceRepository.GetByRiderIdAndDateRangeAsync(riderId, startDate, endDate);
            return attendances.ToList();
        }

        public async Task<Attendance?> GetRiderAttendanceByDateAsync(string riderId, DateTime checkDate)
        {
            return await _attendanceRepository.GetRiderAttendanceByDateAsync(riderId, checkDate.Date);
        }

        // === 考勤状态查询 ===
        public async Task<List<Attendance>> GetLateAttendancesAsync()
        {
            var attendances = await _attendanceRepository.GetLateAttendancesAsync();
            return attendances.ToList();
        }

        public async Task<List<Attendance>> GetAbsentAttendancesAsync()
        {
            var attendances = await _attendanceRepository.GetAbsentAttendancesAsync();
            return attendances.ToList();
        }

        public async Task<List<Attendance>> GetNotCheckedOutAttendancesAsync()
        {
            var attendances = await _attendanceRepository.GetNotCheckedOutAsync();
            return attendances.ToList();
        }

        public async Task<List<Attendance>> GetTodayAttendancesAsync()
        {
            var attendances = await _attendanceRepository.GetTodayAttendancesAsync();
            return attendances.ToList();
        }

        // === 考勤统计 ===
        public async Task<Dictionary<string, int>> GetLateCountByRiderAsync(DateTime startDate, DateTime endDate)
        {
            return await _attendanceRepository.GetLateCountByRiderAsync(startDate, endDate);
        }

        public async Task<Dictionary<string, int>> GetAbsentCountByRiderAsync(DateTime startDate, DateTime endDate)
        {
            return await _attendanceRepository.GetAbsentCountByRiderAsync(startDate, endDate);
        }

        public async Task<Dictionary<string, object>> GetRiderAttendanceStatsAsync(string riderId, DateTime startDate, DateTime endDate)
        {
            return await _attendanceRepository.GetRiderAttendanceStatsAsync(riderId, startDate, endDate);
        }

        // === 业务逻辑 ===
        public Task<bool> IsRiderLateAsync(string riderId, DateTime checkInTime, DateTime scheduledStartTime)
        {
            return Task.FromResult(checkInTime > scheduledStartTime);
        }

        public async Task MarkAbsentAsync(string riderId, DateTime checkDate)
        {
            var rider = await _riderRepository.GetByIdAsync(riderId);
            if (rider == null)
            {
                throw RiderExceptions.RiderNotFound(riderId);
            }

            var attendance = await _attendanceRepository.GetRiderAttendanceByDateAsync(riderId, checkDate.Date);

            if (attendance == null)
            {
                // 创建缺勤记录
                attendance = new Attendance(riderId, checkDate.Date, 0, 1)
                {
                    AttendanceId = Guid.NewGuid().ToString("N"),
                    RiderId = riderId,
                    CheckDate = checkDate.Date,
                    IsLate = 0,
                    IsAbsent = 1,
                    CheckInAt = null,
                    CheckoutAt = null,
                    Rider = rider
                };

                await _attendanceRepository.CreateAsync(attendance);
                await _attendanceRepository.SaveChangesAsync();
            }
            else
            {
                // 标记为缺勤
                attendance.IsAbsent = 1;
                await _attendanceRepository.UpdateAsync(attendance);
                await _attendanceRepository.SaveChangesAsync();
            }
        }

        public async Task<TimeSpan?> CalculateWorkingHoursAsync(string riderId, DateTime checkDate)
        {
            var attendance = await _attendanceRepository.GetRiderAttendanceByDateAsync(riderId, checkDate.Date);

            if (attendance?.CheckInAt == null || attendance?.CheckoutAt == null)
            {
                return null;
            }

            return attendance.CheckoutAt.Value - attendance.CheckInAt.Value;
        }

        public async Task<List<Attendance>> GetAttendanceReportAsync(DateTime startDate, DateTime endDate)
        {
            var attendances = await _attendanceRepository.GetByDateRangeAsync(startDate, endDate);
            return attendances.OrderBy(a => a.CheckDate).ThenBy(a => a.Rider.Name).ToList();
        }
    }
}
