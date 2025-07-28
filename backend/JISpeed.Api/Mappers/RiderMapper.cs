using System;
using System.Collections.Generic;
using System.Linq;
using JISpeed.Api.DTOS;
using JISpeed.Core.Entities.Rider;
using JISpeed.Core.Entities.Order;

namespace JISpeed.Api.Mappers
{
    // 骑手映射器 - 负责实体和DTO之间的转换
    public static class RiderMapper
    {
        // 将Rider实体转换为RiderDTO
        // <param name="rider">骑手实体</param>
        // <returns>骑手DTO</returns>
        public static RiderDTO ToRiderDTO(Rider rider)
        {
            if (rider == null) return null;

            return new RiderDTO
            {
                RiderId = rider.RiderId,
                Name = rider.Name,
                PhoneNumber = rider.PhoneNumber,
                VehicleNumber = rider.VehicleNumber,
                ApplicationUserId = rider.ApplicationUserId
            };
        }

        // 将Rider实体和Performance实体转换为RiderDetailDTO
        // <param name="rider">骑手实体</param>
        // <param name="performance">绩效实体</param>
        // <param name="currentTaskCount">当前任务数</param>
        // <param name="todayCompletedOrders">今日完成订单数</param>
        // <returns>骑手详情DTO</returns>
        public static RiderDetailDTO ToRiderDetailDTO(
            Rider rider,
            Performance performance = null,
            int currentTaskCount = 0,
            int todayCompletedOrders = 0)
        {
            if (rider == null) return null;

            return new RiderDetailDTO
            {
                RiderId = rider.RiderId,
                Name = rider.Name,
                PhoneNumber = rider.PhoneNumber,
                VehicleNumber = rider.VehicleNumber,
                ApplicationUserId = rider.ApplicationUserId,
                Performance = performance != null ? ToPerformanceDTO(performance) : null,
                CurrentTaskCount = currentTaskCount,
                TodayCompletedOrders = todayCompletedOrders
            };
        }

        // 将CreateRiderDTO转换为Rider实体
        // <param name="dto">创建骑手DTO</param>
        // <returns>骑手实体</returns>
        public static Rider ToRiderEntity(CreateRiderDTO dto)
        {
            if (dto == null) return null;

            // 创建一个新的骑手ID
            string riderId = Guid.NewGuid().ToString("N");

            // 使用属性初始化器确保设置所有required属性
            return new Rider(riderId, dto.Name, dto.PhoneNumber, dto.ApplicationUserId)
            {
                // 这些属性已在构造函数中设置，但由于required特性，需要再次显式设置
                RiderId = riderId,
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber,
                // 设置可选属性
                VehicleNumber = dto.VehicleNumber
            };
        }

        // 使用UpdateRiderDTO更新Rider实体
        // <param name="rider">骑手实体</param>
        // <param name="dto">更新骑手DTO</param>
        public static void UpdateRiderFromDTO(Rider rider, UpdateRiderDTO dto)
        {
            if (rider == null || dto == null) return;

            if (!string.IsNullOrEmpty(dto.Name))
            {
                rider.Name = dto.Name;
            }

            if (!string.IsNullOrEmpty(dto.PhoneNumber))
            {
                rider.PhoneNumber = dto.PhoneNumber;
            }

            if (dto.VehicleNumber != null) // 允许设置为null
            {
                rider.VehicleNumber = dto.VehicleNumber;
            }
        }

        // 将Assignment实体转换为AssignmentDTO
        // <param name="assignment">订单分配实体</param>
        // <returns>订单分配DTO</returns>
        public static AssignmentDTO ToAssignmentDTO(Assignment assignment)
        {
            if (assignment == null) return null;

            return new AssignmentDTO
            {
                AssignId = assignment.AssignId,
                AssignedAt = assignment.AssignedAt,
                AcceptedStatus = assignment.AcceptedStatus,
                AcceptedAt = assignment.AcceptedAt,
                TimeOut = assignment.TimeOut,
                Order = assignment.Order != null ? ToOrderSummaryDTO(assignment.Order) : null
            };
        }

        // 将Assignment实体集合转换为AssignmentDTO集合
        // <param name="assignments">订单分配实体集合</param>
        // <returns>订单分配DTO集合</returns>
        public static IEnumerable<AssignmentDTO> ToAssignmentDTOs(IEnumerable<Assignment> assignments)
        {
            if (assignments == null) return Enumerable.Empty<AssignmentDTO>();

            return assignments.Select(ToAssignmentDTO);
        }

        // 将Order实体转换为OrderSummaryDTO
        // <param name="order">订单实体</param>
        // <returns>订单摘要DTO</returns>
        public static OrderSummaryDTO ToOrderSummaryDTO(Order order)
        {
            if (order == null) return null;

            return new OrderSummaryDTO
            {
                OrderId = order.OrderId,
                OrderAmount = order.OrderAmount,
                CreateAt = order.CreateAt,
                OrderStatus = order.OrderStatus,
                DeliveryAddress = order.Address?.DetailedAddress ?? "未知地址",
                MerchantName = "商家名称" // 这里可能需要从其他地方获取商家名称
            };
        }

        // 将Performance实体转换为PerformanceDTO
        // <param name="performance">绩效实体</param>
        // <returns>绩效DTO</returns>
        public static PerformanceDTO ToPerformanceDTO(Performance performance)
        {
            if (performance == null) return null;

            return new PerformanceDTO
            {
                StatsMonth = performance.StatsMonth,
                TotalOrders = performance.TotalOrders,
                OnTimeRate = performance.OnTimeRate,
                GoodReviewRate = performance.GoodReviewRate,
                BadReviewRate = performance.BadReviewRate,
                Income = performance.Income
            };
        }

        // 将Attendance实体转换为AttendanceDTO
        // <param name="attendance">考勤实体</param>
        // <returns>考勤DTO</returns>
        public static AttendanceDTO ToAttendanceDTO(Attendance attendance)
        {
            if (attendance == null) return null;

            return new AttendanceDTO
            {
                AttendanceId = attendance.AttendanceId,
                CheckDate = attendance.CheckDate,
                CheckInAt = attendance.CheckInAt,
                CheckoutAt = attendance.CheckoutAt,
                IsLate = attendance.IsLate,
                IsAbsent = attendance.IsAbsent
            };
        }

        // 将Schedule实体转换为ScheduleDTO
        // <param name="schedule">排班实体</param>
        // <returns>排班DTO</returns>
        public static ScheduleDTO ToScheduleDTO(Schedule schedule)
        {
            if (schedule == null) return null;

            return new ScheduleDTO
            {
                ScheduleId = schedule.ScheduleId,
                WorkDate = schedule.WorkDate,
                ShiftStart = schedule.ShiftStart,
                ShiftEnd = schedule.ShiftEnd
            };
        }
    }
}
