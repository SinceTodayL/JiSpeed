using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JISpeed.Api.DTOS
{
    // ������ϢDTO - ������Ӧ
    public class RiderDTO
    {
        // ����ID
        public string? RiderId { get; set; }

        // ��������
        public string? Name { get; set; }

        // �ֻ���
        public string? PhoneNumber { get; set; }

        // ���ƺ�
        public string? VehicleNumber { get; set; }

        // �����֤�û�ID
        public string? ApplicationUserId { get; set; }
    }

    // ��������DTO - ����������Ϣ
    public class RiderDetailDTO : RiderDTO
    {
        // ��Ч��Ϣ
        public PerformanceDTO? Performance { get; set; }

        // ��ǰ������
        public int CurrentTaskCount { get; set; }

        // ������ɶ�����
        public int TodayCompletedOrders { get; set; }
    }

    // ������������DTO
    public class CreateRiderDTO
    {
        // ��������
        [Required(ErrorMessage = "������������Ϊ��")]
        [StringLength(50, ErrorMessage = "�����������Ȳ��ܳ���50���ַ�")]
        public string Name { get; set; } = string.Empty;

        // �ֻ���
        [Required(ErrorMessage = "�ֻ��Ų���Ϊ��")]
        [RegularExpression(@"^1[3-9]\d{9}$", ErrorMessage = "�ֻ��Ÿ�ʽ����ȷ")]
        [StringLength(20, ErrorMessage = "�ֻ��ų��Ȳ��ܳ���20���ַ�")]
        public string PhoneNumber { get; set; } = string.Empty;

        // ���ƺ�
        [StringLength(20, ErrorMessage = "���ƺų��Ȳ��ܳ���20���ַ�")]
        public string? VehicleNumber { get; set; }

        // �����֤�û�ID
        public string? ApplicationUserId { get; set; }
    }

    // ������������DTO
    public class UpdateRiderDTO
    {
        // ��������
        [StringLength(50, ErrorMessage = "�����������Ȳ��ܳ���50���ַ�")]
        public string? Name { get; set; }

        // �ֻ���
        [RegularExpression(@"^1[3-9]\d{9}$", ErrorMessage = "�ֻ��Ÿ�ʽ����ȷ")]
        [StringLength(20, ErrorMessage = "�ֻ��ų��Ȳ��ܳ���20���ַ�")]
        public string? PhoneNumber { get; set; }

        // ���ƺ�
        [StringLength(20, ErrorMessage = "���ƺų��Ȳ��ܳ���20���ַ�")]
        public string? VehicleNumber { get; set; }
    }

    // ��������DTO
    public class AssignmentDTO
    {
        // ����ID
        public string? AssignId { get; set; }

        // ����ʱ��
        public DateTime AssignedAt { get; set; }

        // �ӵ�״̬
        public int AcceptedStatus { get; set; }

        // �ӵ�ʱ��
        public DateTime? AcceptedAt { get; set; }

        // ��ʱʱ�䣨���ӣ�
        public int? TimeOut { get; set; }

        // ������Ϣ
        public OrderSummaryDTO? Order { get; set; }
    }

    // ����ժҪDTO
    public class OrderSummaryDTO
    {
        // ����ID
        public string? OrderId { get; set; }

        // �������
        public decimal OrderAmount { get; set; }

        // ����ʱ��
        public DateTime CreateAt { get; set; }

        // ����״̬
        public int OrderStatus { get; set; }

        // ���͵�ַ
        public string? DeliveryAddress { get; set; }

        // �̼�����
        public string? MerchantName { get; set; }
    }

    // ���¶�������״̬����DTO
    public class UpdateAssignmentStatusDTO
    {
        // �ӵ�״̬
        [Required(ErrorMessage = "�ӵ�״̬����Ϊ��")]
        [Range(0, 3, ErrorMessage = "�ӵ�״ֵ̬��Ч")]
        public int AcceptedStatus { get; set; }
    }

    // ��ЧDTO
    public class PerformanceDTO
    {
        // ͳ���·�
        public DateTime StatsMonth { get; set; }

        // �ܶ�����
        public int TotalOrders { get; set; }

        // ׼ʱ��
        public decimal OnTimeRate { get; set; }

        // ������
        public decimal GoodReviewRate { get; set; }

        // ������
        public decimal BadReviewRate { get; set; }

        // ������
        public decimal Income { get; set; }
    }

    // ���ڼ�¼DTO
    public class AttendanceDTO
    {
        // ����ID
        public string? AttendanceId { get; set; }

        // ��������
        public DateTime CheckDate { get; set; }

        // ǩ��ʱ��
        public DateTime? CheckInAt { get; set; }

        // ǩ��ʱ��
        public DateTime? CheckoutAt { get; set; }

        // �Ƿ�ٵ�
        public int IsLate { get; set; }

        // �Ƿ�ȱ��
        public int IsAbsent { get; set; }
    }

    // �Ű�DTO
    public class ScheduleDTO
    {
        // �Ű�ID
        public string? ScheduleId { get; set; }

        // ��������
        public DateTime WorkDate { get; set; }

        // ��ο�ʼʱ��
        public DateTime ShiftStart { get; set; }

        // ��ν���ʱ��
        public DateTime ShiftEnd { get; set; }
    }
}
