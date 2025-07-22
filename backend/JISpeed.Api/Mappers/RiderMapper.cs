using System;
using System.Collections.Generic;
using System.Linq;
using JISpeed.Api.DTOS;
using JISpeed.Core.Entities.Rider;
using JISpeed.Core.Entities.Order;

namespace JISpeed.Api.Mappers
{
    // ����ӳ���� - ����ʵ���DTO֮���ת��
    public static class RiderMapper
    {
        // ��Riderʵ��ת��ΪRiderDTO
        // <param name="rider">����ʵ��</param>
        // <returns>����DTO</returns>
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

        // ��Riderʵ���Performanceʵ��ת��ΪRiderDetailDTO
        // <param name="rider">����ʵ��</param>
        // <param name="performance">��Чʵ��</param>
        // <param name="currentTaskCount">��ǰ������</param>
        // <param name="todayCompletedOrders">������ɶ�����</param>
        // <returns>��������DTO</returns>
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

        // ��CreateRiderDTOת��ΪRiderʵ��
        // <param name="dto">��������DTO</param>
        // <returns>����ʵ��</returns>
        public static Rider ToRiderEntity(CreateRiderDTO dto)
        {
            if (dto == null) return null;

            // ����һ���µ�����ID
            string riderId = Guid.NewGuid().ToString("N");

            // ʹ�����Գ�ʼ����ȷ����������required����
            return new Rider(riderId, dto.Name, dto.PhoneNumber, dto.ApplicationUserId)
            {
                // ��Щ�������ڹ��캯�������ã�������required���ԣ���Ҫ�ٴ���ʽ����
                RiderId = riderId,
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber,
                // ���ÿ�ѡ����
                VehicleNumber = dto.VehicleNumber
            };
        }

        // ʹ��UpdateRiderDTO����Riderʵ��
        // <param name="rider">����ʵ��</param>
        // <param name="dto">��������DTO</param>
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

            if (dto.VehicleNumber != null) // ��������Ϊnull
            {
                rider.VehicleNumber = dto.VehicleNumber;
            }
        }

        // ��Assignmentʵ��ת��ΪAssignmentDTO
        // <param name="assignment">��������ʵ��</param>
        // <returns>��������DTO</returns>
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

        // ��Assignmentʵ�弯��ת��ΪAssignmentDTO����
        // <param name="assignments">��������ʵ�弯��</param>
        // <returns>��������DTO����</returns>
        public static IEnumerable<AssignmentDTO> ToAssignmentDTOs(IEnumerable<Assignment> assignments)
        {
            if (assignments == null) return Enumerable.Empty<AssignmentDTO>();

            return assignments.Select(ToAssignmentDTO);
        }

        // ��Orderʵ��ת��ΪOrderSummaryDTO
        // <param name="order">����ʵ��</param>
        // <returns>����ժҪDTO</returns>
        public static OrderSummaryDTO ToOrderSummaryDTO(Order order)
        {
            if (order == null) return null;

            return new OrderSummaryDTO
            {
                OrderId = order.OrderId,
                OrderAmount = order.OrderAmount,
                CreateAt = order.CreateAt,
                OrderStatus = order.OrderStatus,
                DeliveryAddress = order.Address?.DetailedAddress ?? "δ֪��ַ",
                MerchantName = "�̼�����" // ���������Ҫ�������ط���ȡ�̼�����
            };
        }

        // ��Performanceʵ��ת��ΪPerformanceDTO
        // <param name="performance">��Чʵ��</param>
        // <returns>��ЧDTO</returns>
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

        // ��Attendanceʵ��ת��ΪAttendanceDTO
        // <param name="attendance">����ʵ��</param>
        // <returns>����DTO</returns>
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

        // ��Scheduleʵ��ת��ΪScheduleDTO
        // <param name="schedule">�Ű�ʵ��</param>
        // <returns>�Ű�DTO</returns>
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
