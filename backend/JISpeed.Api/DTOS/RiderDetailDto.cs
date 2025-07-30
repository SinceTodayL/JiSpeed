using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JISpeed.Api.DTOS
{
    // 骑手信息DTO - 用于响应
    public class RiderDTO
    {
        // 骑手ID
        public string? RiderId { get; set; }

        // 骑手姓名
        public string? Name { get; set; }

        // 手机号
        public string? PhoneNumber { get; set; }

        // 车牌号
        public string? VehicleNumber { get; set; }

        // 身份认证用户ID
        public string? ApplicationUserId { get; set; }
    }

    // 骑手详情DTO - 包含更多信息
    public class RiderDetailDTO : RiderDTO
    {
        // 绩效信息
        public PerformanceDTO? Performance { get; set; }

        // 当前任务数
        public int CurrentTaskCount { get; set; }

        // 今日完成订单数
        public int TodayCompletedOrders { get; set; }
    }

    // 创建骑手请求DTO
    public class CreateRiderDTO
    {
        // 骑手姓名
        [Required(ErrorMessage = "骑手姓名不能为空")]
        [StringLength(50, ErrorMessage = "骑手姓名长度不能超过50个字符")]
        public string Name { get; set; } = string.Empty;

        // 手机号
        [Required(ErrorMessage = "手机号不能为空")]
        [RegularExpression(@"^1[3-9]\d{9}$", ErrorMessage = "手机号格式不正确")]
        [StringLength(20, ErrorMessage = "手机号长度不能超过20个字符")]
        public string PhoneNumber { get; set; } = string.Empty;

        // 车牌号
        [StringLength(20, ErrorMessage = "车牌号长度不能超过20个字符")]
        public string? VehicleNumber { get; set; }

        // 身份认证用户ID
        public string? ApplicationUserId { get; set; }
    }

    // 更新骑手请求DTO
    public class UpdateRiderDTO
    {
        // 骑手姓名
        [StringLength(50, ErrorMessage = "骑手姓名长度不能超过50个字符")]
        public string? Name { get; set; }

        // 手机号
        [RegularExpression(@"^1[3-9]\d{9}$", ErrorMessage = "手机号格式不正确")]
        [StringLength(20, ErrorMessage = "手机号长度不能超过20个字符")]
        public string? PhoneNumber { get; set; }

        // 车牌号
        [StringLength(20, ErrorMessage = "车牌号长度不能超过20个字符")]
        public string? VehicleNumber { get; set; }
    }

    // 订单分配DTO
    public class AssignmentDTO
    {
        // 分配ID
        public string? AssignId { get; set; }

        // 分配时间
        public DateTime AssignedAt { get; set; }

        // 接单状态
        public int AcceptedStatus { get; set; }

        // 接单时间
        public DateTime? AcceptedAt { get; set; }

        // 超时时间（分钟）
        public int? TimeOut { get; set; }

        // 订单信息
        public OrderSummaryDTO? Order { get; set; }
    }

    // 订单摘要DTO
    public class OrderSummaryDTO
    {
        // 订单ID
        public string? OrderId { get; set; }

        // 订单金额
        public decimal OrderAmount { get; set; }

        // 创建时间
        public DateTime CreateAt { get; set; }

        // 订单状态
        public int OrderStatus { get; set; }

        // 配送地址
        public string? DeliveryAddress { get; set; }

        // 商家名称
        public string? MerchantName { get; set; }
    }

    // 更新订单分配状态请求DTO
    public class UpdateAssignmentStatusDTO
    {
        // 接单状态
        [Required(ErrorMessage = "接单状态不能为空")]
        [Range(0, 3, ErrorMessage = "接单状态值无效")]
        public int AcceptedStatus { get; set; }
    }

    // 绩效DTO
    public class PerformanceDTO
    {
        // 统计月份
        public DateTime StatsMonth { get; set; }

        // 总订单量
        public int TotalOrders { get; set; }

        // 准时率
        public decimal OnTimeRate { get; set; }

        // 好评率
        public decimal GoodReviewRate { get; set; }

        // 差评率
        public decimal BadReviewRate { get; set; }

        // 总收入
        public decimal Income { get; set; }
    }

    // 考勤记录DTO
    public class AttendanceDTO
    {
        // 考勤ID
        public string? AttendanceId { get; set; }

        // 考勤日期
        public DateTime CheckDate { get; set; }

        // 签到时间
        public DateTime? CheckInAt { get; set; }

        // 签退时间
        public DateTime? CheckoutAt { get; set; }

        // 是否迟到
        public int IsLate { get; set; }

        // 是否缺勤
        public int IsAbsent { get; set; }
    }

    // 排班DTO
    public class ScheduleDTO
    {
        // 排班ID
        public string? ScheduleId { get; set; }

        // 工作日期
        public DateTime WorkDate { get; set; }

        // 班次开始时间
        public DateTime ShiftStart { get; set; }

        // 班次结束时间
        public DateTime ShiftEnd { get; set; }
    }
}
