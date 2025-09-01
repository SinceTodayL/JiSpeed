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
}