using JISpeed.Core.Entities.Order;
using OrderEntity = JISpeed.Core.Entities.Order.Order;
using OrderDishEntity = JISpeed.Core.Entities.Junctions.OrderDish;
namespace JISpeed.Core.Interfaces.IServices
{
    public interface IOrderService
    {
        Task<OrderEntity> GetOrderDetailByOrderIdAsync(string orderId);

        Task<string> CreateOrderByUserIdAsync(
            string userId, decimal orderAmount,
            string? couponId, string addressId,
            string merchantId, List<DishQuantityDto> dishQuantities);
        Task<List<string>> GetOrderIdByUserIdAsync(
            string userId, int? orderStatus,
            int? size, int? page);
        
        Task<bool> UpdateOrderAsync(string orderId, int? orderStatus);

        Task<bool> UpdatePaymentAsync(string payId, int payStatus,int? amount);
        Task<Payment> CreatePaymentByorderIdAsync(string orderId, string channel);
        Task<List<string>> GetOrderIdByMerchantIdAsync(
            string merchantId, int? orderStatus,
            DateTime? startDate, DateTime? endDate,
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
        Cancelled = 5, // 订单关闭(超时/主动取消)
    }

    public enum OrderLogStatus
    {
        Created = 0,        // 创建订单
        Paid = 1,           // 已支付
        OutOfDelivery = 2,  // 派送中
        Delivered = 3,      // 送达
        Cancelled = 4       // 取消
    }
    
    public class DishQuantityDto
    {
        public required string DishId { get; set; }
        public required int Quantity { get; set; }
    }
}