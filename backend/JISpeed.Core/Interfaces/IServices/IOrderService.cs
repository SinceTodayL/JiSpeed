using JISpeed.Core.Entities.Order;
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
        Default  = 0, // 默认
        Rejected = 1, // 拒绝
        Refunded = 2, // 退款
    }
    
    public class DishQuantityDto
    {
        public required string DishId { get; set; }
        public required int Quantity { get; set; }
    }
}