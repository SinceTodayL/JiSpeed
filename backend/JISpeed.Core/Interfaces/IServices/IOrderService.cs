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
    }
}