using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JISpeed.Core.Entities.Rider;

namespace JISpeed.Core.Interfaces.IRepositories
{
    // 骑手仓储接口 - 处理骑手模块的数据访问操作
    public interface IRiderRepository
    {
        // 创建新用户
        // <param name="rider">用户实体</param>
        // <returns>创建的用户实体</returns>
        Task<Rider> CreateAsync(Rider rider);

        // 保存更改
        // <returns>保存的记录数</returns>
        Task<int> SaveChangesAsync();

        // 根据ID获取骑手信息
        // <param name="riderId">骑手ID</param>
        // <returns>骑手实体</returns>
        Task<Rider> GetByIdAsync(string riderId);

        // 根据手机号检查骑手是否存在
        // <param name="phoneNumber">手机号</param>
        // <returns>是否存在</returns>
        Task<bool> ExistsByPhoneAsync(string phoneNumber);

        // 添加骑手
        // <param name="rider">骑手实体</param>
        Task AddAsync(Rider rider);

        // 更新骑手信息
        // <param name="rider">骑手实体</param>
        Task UpdateAsync(Rider rider);

        // 获取骑手的订单分配列表
        // <param name="riderId">骑手ID</param>
        // <returns>订单分配列表</returns>
        Task<IEnumerable<Assignment>> GetAssignmentsAsync(string riderId);

        // 根据ID获取订单分配
        // <param name="assignId">分配ID</param>
        // <returns>订单分配</returns>
        Task<Assignment> GetAssignmentByIdAsync(string assignId);

        // 更新订单分配
        // <param name="assignment">订单分配实体</param>
        Task UpdateAssignmentAsync(Assignment assignment);

        // 获取骑手的考勤记录
        // <param name="riderId">骑手ID</param>
        // <param name="startDate">开始日期</param>
        // <param name="endDate">结束日期</param>
        // <returns>考勤记录列表</returns>
        Task<IEnumerable<Attendance>> GetAttendancesAsync(string riderId, DateTime? startDate = null, DateTime? endDate = null);

        // 添加考勤记录
        // <param name="attendance">考勤记录</param>
        Task AddAttendanceAsync(Attendance attendance);

        // 更新考勤记录
        // <param name="attendance">考勤记录</param>
        Task UpdateAttendanceAsync(Attendance attendance);

        // 获取骑手的绩效记录
        // <param name="riderId">骑手ID</param>
        // <param name="year">年份</param>
        // <param name="month">月份</param>
        // <returns>绩效记录</returns>
        Task<Performance> GetPerformanceAsync(string riderId, int? year = null, int? month = null);

        // 获取骑手的排班列表
        // <param name="riderId">骑手ID</param>
        // <param name="startDate">开始日期</param>
        // <param name="endDate">结束日期</param>
        // <returns>排班列表</returns>
        Task<IEnumerable<Schedule>> GetSchedulesAsync(string riderId, DateTime? startDate = null, DateTime? endDate = null);

        // 为骑手分配排班
        // <param name="riderSchedule">骑手排班实体</param>
        Task AssignScheduleAsync(RiderSchedule riderSchedule);

        // 取消骑手排班
        // <param name="riderId">骑手ID</param>
        // <param name="scheduleId">排班ID</param>
        Task RemoveScheduleAsync(string riderId, string scheduleId);
    }
}
