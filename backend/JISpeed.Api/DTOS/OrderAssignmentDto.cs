using JISpeed.Core.Entities.Order;
using JISpeed.Core.Interfaces.IServices;

namespace JISpeed.Api.DTOs
{
    // 订单分配请求DTO - 用于手动分配订单
    public class OrderAssignmentRequestDto
    {
        public required string OrderId { get; set; } // 订单ID
        public required string RiderId { get; set; } // 骑手ID
    }

    // 订单分配响应DTO - 返回分配结果
    public class OrderAssignmentResponseDto
    {
        public required string AssignmentId { get; set; } // 分配编号
        public required string OrderId { get; set; } // 订单ID
        public required string RiderId { get; set; } // 骑手ID
        public required DateTime AssignedAt { get; set; } // 分配时间
        public required int AssignmentStatus { get; set; } // 分配状态 (1=待接单, 2=已接单, 3=已拒绝)
        public string? Remark { get; set; } // 备注信息
    }

    // 骑手接单请求DTO
    public class AcceptOrderRequestDto
    {
        public required string OrderId { get; set; } // 订单ID
        public required string RiderId { get; set; } // 骑手ID
    }

    // 骑手拒绝订单请求DTO
    public class RejectOrderRequestDto
    {
        public required string OrderId { get; set; } // 订单ID
        public required string RiderId { get; set; } // 骑手ID
        public string? Reason { get; set; } // 拒绝原因
    }

    // 订单分配状态更新DTO
    public class UpdateOrderStatusRequestDto
    {
        public required string OrderId { get; set; } // 订单ID
        public required OrderStatus NewStatus { get; set; } // 新状态
        public string? Remark { get; set; } // 备注信息
    }

    // 骑手分配信息DTO
    public class RiderAssignmentDto
    {
        public required string AssignmentId { get; set; } // 分配编号
        public required string OrderId { get; set; } // 订单ID
        public required string MerchantId { get; set; } // 商家ID
        public required string UserId { get; set; } // 用户ID
        public required decimal OrderAmount { get; set; } // 订单金额
        public required DateTime AssignedAt { get; set; } // 分配时间
        public required int AssignmentStatus { get; set; } // 分配状态
        public string? Remark { get; set; } // 备注信息
    }

    // 自动分配结果DTO
    public class AutoAssignmentResultDto
    {
        public required bool IsSuccess { get; set; } // 是否分配成功
        public string? AssignmentId { get; set; } // 分配编号（成功时）
        public string? RiderId { get; set; } // 分配的骑手ID（成功时）
        public string? ErrorMessage { get; set; } // 错误信息（失败时）
    }

    // 完整订单信息DTO - 包含骑手、用户、商家、分配信息和订单状态
    public class OrderDetailInfoDto
    {
        // 订单基本信息
        public required string OrderId { get; set; } // 订单ID
        public required DateTime CreateAt { get; set; } // 创建时间
        public required int OrderStatus { get; set; } // 订单状态 (0-8)

        // 用户信息
        public required UserInfoDto User { get; set; }

        // 商家信息
        public required MerchantInfoDto Merchant { get; set; }

        // 分配信息（可能为空）
        public AssignmentInfoDto? Assignment { get; set; }

        // 地址信息
        public required AddressInfoDto Address { get; set; }
    }

    // 用户信息DTO
    public class UserInfoDto
    {
        public required string UserId { get; set; } // 用户ID
        public required string Nickname { get; set; } // 用户昵称
    }

    // 商家信息DTO
    public class MerchantInfoDto
    {
        public required string MerchantId { get; set; } // 商家ID
        public required string MerchantName { get; set; } // 商家名称
        public string? Location { get; set; } // 商家位置
        public decimal? Longitude { get; set; } // 经度
        public decimal? Latitude { get; set; } // 纬度
    }

    // 分配信息DTO
    public class AssignmentInfoDto
    {
        public required string AssignId { get; set; } // 分配编号
        public required string RiderId { get; set; } // 骑手ID
        public required string RiderName { get; set; } // 骑手姓名
        public string? RiderPhoneNumber { get; set; } // 骑手电话
        public string? VehicleNumber { get; set; } // 车辆号码
        public required DateTime AssignedAt { get; set; } // 分配时间
        public required int AcceptedStatus { get; set; } // 接单状态 (0=待接单, 1=已接单, 2=已拒绝)
        public DateTime? AcceptedAt { get; set; } // 接单时间
        public int? TimeOut { get; set; } // 超时分钟数
    }

    // 地址信息DTO
    public class AddressInfoDto
    {
        public required string AddressId { get; set; } // 地址ID
        public required string RecipientName { get; set; } // 收货人姓名
        public required string Address { get; set; } // 详细地址
        public decimal? Longitude { get; set; } // 经度
        public decimal? Latitude { get; set; } // 纬度
    }
}