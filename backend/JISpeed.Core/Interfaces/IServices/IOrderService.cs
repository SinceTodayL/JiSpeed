using JISpeed.Core.Entities.Order;
using JISpeed.Core.Entities.Rider;
using OrderEntity = JISpeed.Core.Entities.Order.Order;
namespace JISpeed.Core.Interfaces.IServices
{
    public interface IOrderService
    {
        // Order
        Task<OrderEntity> GetOrderDetailByOrderIdAsync(string orderId);
        Task<string> CreateOrderByUserIdAsync(
            string userId, decimal orderAmount,
            string? couponId, string addressId,
            string merchantId, List<DishQuantityDto> dishQuantities);
        Task<List<string>> GetOrderIdByUserIdAsync(
            string userId, int? orderStatus,
            int? size, int? page);
        Task<bool> UpdateOrderAsync(string orderId, int? orderStatus);
        Task<List<string>> GetOrderIdByMerchantIdAsync(
            string merchantId, int? orderStatus,
            DateTime? startDate, DateTime? endDate,
            int? size, int? page);
        
        
        
        
        //OrderLog
        Task<OrderLog>GetOrderLogDetailByLogIdAsync(string logId);
        
        
        
        // Payment
        Task<bool> UpdatePaymentAsync(string payId, int payStatus,int? amount);
        Task<Payment> CreatePaymentByOrderIdAsync(string orderId, string channel);
        Task<Payment> GetPaymentDetailByPayIdAsync(string payId);
        
        
        
        
        // Refund
        // 返回OrderLogId
        Task<string> CreateRefundByOrderIdAndUserIdAsync(string userId, string orderId, string reason,decimal amount);
        Task<string> UpdateRefundForMerchantAsync(string merchantId, string refundId, int refundStatus);
        Task<string> UpdateRefundForAdminAsync(string adminId, string refundId, int refundStatus);
        Task<Refund> GetRefundDetailByRefundIdAsync(string refundId);
        Task<List<string>> GetRefundListByFilterAsync(
            string? userId,
            string?merchantId,
            string? adminId,
            int? auditStatus,
            int?size,int?page);
        
        // Complaint
        Task<Complaint> GetComplaintDetailByComplainantIdAsync(string complaintId);
        Task<string> CreateComplaintDetailAsync(
            string orderId,string userId,
            int cmplRole,string? cmplDescription);
        Task<bool>AuditComplaintAsync(string adminId, string complaintId);
        Task<bool>CancelComplaintAsync(string userId, string complaintId);

        Task<List<string>> GetComplaintListByFilterAsync(
            string? userId,
            string? merchantId,
            int? status,
            string? adminId,
            int? size, int? page);




    }
    public enum PayStatus
    {
        Unpaid = 0,    // 未支付（初始状态）
        Paid = 1,      // 已支付
        Cancelled = 2  // 已取消
    }

    public enum OrderStatus
    {
        Unpaid = 0,    // 未支付（初始状态）
        Paid = 1,      // 已支付
        Confirmed = 2, // 确认收货
        Reviewed =3,   // 已经评价
        Aftersales = 4,// 售后中
        AftersalesCompleted = 5, // 售后结束
        Cancelled = 6, // 订单关闭(超时/主动取消)
    }

    public enum OrderLogStatus
    { 
        Created = 0,             // 创建订单
        Paid = 1,                // 已支付
        OutOfDelivery = 2,       // 派送中
        Delivered = 3,           // 送达
        Cancelled = 4,           // 取消
        Aftersales = 5,          // 售后中
        AftersalesCompleted = 6, // 售后结束
    }
    
    public enum RefundStatus
    {
        Default  = 0,         // 默认
        Rejected = 1,         // 商家拒绝
        Refunded = 2,         // 商家退款
        DefaultForAdmin = 3,  // 等待管理员处理
        RejectedForAdmin = 4, // 管理员拒绝退款
        RefundedForAdmin = 5, // 管理员同意退款
    }
    public enum ComplaintStatus
    {
        Default  = 0,         // 处理中
        Resolved = 1,         // 已解决
        Cancelled = 2,        // 关闭
    }

    public enum Role
    {
        User = 0,
        Merchant = 1,
        Rider = 2,
    }
    public class DishQuantityDto
    {
        public required string DishId { get; set; }
        public required int Quantity { get; set; }
    }

}