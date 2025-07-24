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
    // ���ֲִ�ʵ��
    public class RiderRepository : IRiderRepository
    {
        private readonly OracleDbContext _context;

        public RiderRepository(OracleDbContext context)
        {
            _context = context;
        }

        // �������û�
        // <param name="rider">�û�ʵ��</param>
        // <returns>�������û�ʵ��</returns>
        public async Task<Rider> CreateAsync(Rider rider)
        {
            var entity = await _context.Riders.AddAsync(rider);
            return entity.Entity;
        }

        // �������
        // <returns>����ļ�¼��</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // ����ID��ȡ������Ϣ
        // <param name="riderId">����ID</param>
        // <returns>����ʵ��</returns>
        public async Task<Rider> GetByIdAsync(string riderId)
        {
            return await _context.Riders
                .Include(r => r.ApplicationUser)
                .FirstOrDefaultAsync(r => r.RiderId == riderId);
        }

        // �����ֻ��ż�������Ƿ����
        // <param name="phoneNumber">�ֻ���</param>
        // <returns>�Ƿ����</returns>
        public async Task<bool> ExistsByPhoneAsync(string phoneNumber)
        {
            return await _context.Riders
                .AnyAsync(r => r.PhoneNumber == phoneNumber);
        }

        // �������
        // <param name="rider">����ʵ��</param>
        public async Task AddAsync(Rider rider)
        {
            await _context.Riders.AddAsync(rider);
            await _context.SaveChangesAsync();
        }

        // ����������Ϣ
        // <param name="rider">����ʵ��</param>
        public async Task UpdateAsync(Rider rider)
        {
            _context.Riders.Update(rider);
            await _context.SaveChangesAsync();
        }

        // ��ȡ���ֵĶ��������б�
        // <param name="riderId">����ID</param>
        // <returns>���������б�</returns>
        public async Task<IEnumerable<Assignment>> GetAssignmentsAsync(string riderId)
        {
            return await _context.Assignments
                .Include(a => a.Order)
                .ThenInclude(o => o.Address)
                .Where(a => a.RiderId == riderId)
                .OrderByDescending(a => a.AssignedAt)
                .ToListAsync();
        }

        // ����ID��ȡ��������
        // <param name="assignId">����ID</param>
        // <returns>��������</returns>
        public async Task<Assignment> GetAssignmentByIdAsync(string assignId)
        {
            return await _context.Assignments
                .Include(a => a.Order)
                .FirstOrDefaultAsync(a => a.AssignId == assignId);
        }

        // ���¶�������
        // <param name="assignment">��������ʵ��</param>
        public async Task UpdateAssignmentAsync(Assignment assignment)
        {
            _context.Assignments.Update(assignment);
            await _context.SaveChangesAsync();
        }

        // ��ȡ���ֵĿ��ڼ�¼
        // <param name="riderId">����ID</param>
        // <param name="startDate">��ʼ����</param>
        // <param name="endDate">��������</param>
        // <returns>���ڼ�¼�б�</returns>
        public async Task<IEnumerable<Attendance>> GetAttendancesAsync(string riderId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Attendances
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

        // ��ӿ��ڼ�¼
        // <param name="attendance">���ڼ�¼</param>
        public async Task AddAttendanceAsync(Attendance attendance)
        {
            await _context.Attendances.AddAsync(attendance);
            await _context.SaveChangesAsync();
        }

        // ���¿��ڼ�¼
        // <param name="attendance">���ڼ�¼</param>
        public async Task UpdateAttendanceAsync(Attendance attendance)
        {
            _context.Attendances.Update(attendance);
            await _context.SaveChangesAsync();
        }

        // ��ȡ���ֵļ�Ч��¼
        // <param name="riderId">����ID</param>
        // <param name="year">���</param>
        // <param name="month">�·�</param>
        // <returns>��Ч��¼</returns>
        public async Task<Performance> GetPerformanceAsync(string riderId, int? year = null, int? month = null)
        {
            var query = _context.Performances
                .Where(p => p.RiderId == riderId);

            if (year.HasValue && month.HasValue)
            {
                // ����ָ���·ݵļ�¼
                var targetDate = new DateTime(year.Value, month.Value, 1);
                query = query.Where(p => p.StatsMonth.Year == targetDate.Year && p.StatsMonth.Month == targetDate.Month);
            }

            return await query
                .OrderByDescending(p => p.StatsMonth)
                .FirstOrDefaultAsync();
        }

        // ��ȡ���ֵ��Ű��б�
        // <param name="riderId">����ID</param>
        // <param name="startDate">��ʼ����</param>
        // <param name="endDate">��������</param>
        // <returns>�Ű��б�</returns>
        public async Task<IEnumerable<Schedule>> GetSchedulesAsync(string riderId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.RiderSchedules
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

        // Ϊ���ַ����Ű�
        // <param name="riderSchedule">�����Ű�����</param>
        public async Task AssignScheduleAsync(RiderSchedule riderSchedule)
        {
            await _context.RiderSchedules.AddAsync(riderSchedule);
            await _context.SaveChangesAsync();
        }

        // ȡ�������Ű�
        // <param name="riderId">����ID</param>
        // <param name="scheduleId">�Ű�ID</param>
        public async Task RemoveScheduleAsync(string riderId, string scheduleId)
        {
            var riderSchedule = await _context.RiderSchedules
                .FirstOrDefaultAsync(rs => rs.RiderId == riderId && rs.ScheduleId == scheduleId);

            if (riderSchedule != null)
            {
                _context.RiderSchedules.Remove(riderSchedule);
                await _context.SaveChangesAsync();
            }
        }
    }
}
