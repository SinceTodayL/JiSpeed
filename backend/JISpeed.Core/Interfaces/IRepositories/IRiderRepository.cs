using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JISpeed.Core.Entities.Rider;

namespace JISpeed.Core.Interfaces.IRepositories
{
    // ���ֲִ��ӿ� - ��������ģ������ݷ��ʲ���
    public interface IRiderRepository
    {
        // �������û�
        // <param name="rider">�û�ʵ��</param>
        // <returns>�������û�ʵ��</returns>
        Task<Rider> CreateAsync(Rider rider);

        // �������
        // <returns>����ļ�¼��</returns>
        Task<int> SaveChangesAsync();

        // ����ID��ȡ������Ϣ
        // <param name="riderId">����ID</param>
        // <returns>����ʵ��</returns>
        Task<Rider> GetByIdAsync(string riderId);

        // �����ֻ��ż�������Ƿ����
        // <param name="phoneNumber">�ֻ���</param>
        // <returns>�Ƿ����</returns>
        Task<bool> ExistsByPhoneAsync(string phoneNumber);

        // �������
        // <param name="rider">����ʵ��</param>
        Task AddAsync(Rider rider);

        // ����������Ϣ
        // <param name="rider">����ʵ��</param>
        Task UpdateAsync(Rider rider);

        // ��ȡ���ֵĶ��������б�
        // <param name="riderId">����ID</param>
        // <returns>���������б�</returns>
        Task<IEnumerable<Assignment>> GetAssignmentsAsync(string riderId);

        // ����ID��ȡ��������
        // <param name="assignId">����ID</param>
        // <returns>��������</returns>
        Task<Assignment> GetAssignmentByIdAsync(string assignId);

        // ���¶�������
        // <param name="assignment">��������ʵ��</param>
        Task UpdateAssignmentAsync(Assignment assignment);

        // ��ȡ���ֵĿ��ڼ�¼
        // <param name="riderId">����ID</param>
        // <param name="startDate">��ʼ����</param>
        // <param name="endDate">��������</param>
        // <returns>���ڼ�¼�б�</returns>
        Task<IEnumerable<Attendance>> GetAttendancesAsync(string riderId, DateTime? startDate = null, DateTime? endDate = null);

        // ��ӿ��ڼ�¼
        // <param name="attendance">���ڼ�¼</param>
        Task AddAttendanceAsync(Attendance attendance);

        // ���¿��ڼ�¼
        // <param name="attendance">���ڼ�¼</param>
        Task UpdateAttendanceAsync(Attendance attendance);

        // ��ȡ���ֵļ�Ч��¼
        // <param name="riderId">����ID</param>
        // <param name="year">���</param>
        // <param name="month">�·�</param>
        // <returns>��Ч��¼</returns>
        Task<Performance> GetPerformanceAsync(string riderId, int? year = null, int? month = null);

        // ��ȡ���ֵ��Ű��б�
        // <param name="riderId">����ID</param>
        // <param name="startDate">��ʼ����</param>
        // <param name="endDate">��������</param>
        // <returns>�Ű��б�</returns>
        Task<IEnumerable<Schedule>> GetSchedulesAsync(string riderId, DateTime? startDate = null, DateTime? endDate = null);

        // Ϊ���ַ����Ű�
        // <param name="riderSchedule">�����Ű�ʵ��</param>
        Task AssignScheduleAsync(RiderSchedule riderSchedule);

        // ȡ�������Ű�
        // <param name="riderId">����ID</param>
        // <param name="scheduleId">�Ű�ID</param>
        Task RemoveScheduleAsync(string riderId, string scheduleId);
    }
}
