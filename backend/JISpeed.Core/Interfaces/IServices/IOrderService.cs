using JISpeed.Core.Entities.Order;
using OrderEntity = JISpeed.Core.Entities.Order.Order;
using OrderDishEntity = JISpeed.Core.Entities.Junctions.OrderDish;
namespace JISpeed.Core.Interfaces.IServices
{
    public interface IOrderService
    {
        Task<OrderEntity> GetOrderDetailByOrderIdAsync(string orderId);

        Task<List<OrderEntity>> GetOrderIdByUserIdAsync(
            string userId, int? orderStatus,
            int? size, int? page);
        
        Task<bool> UpdateOrderAsync(string orderId, int? orderStatus);

        Task<bool> UpdatePaymentAsync(string payId, int payStatus,int? amount);
        Task<Payment> CreatePaymentByorderIdAsync(string orderId, string channel);
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
}