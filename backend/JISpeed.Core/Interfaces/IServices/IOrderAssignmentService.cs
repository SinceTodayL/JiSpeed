using System.Threading.Tasks;
using JISpeed.Core.Interfaces.IServices;

namespace JISpeed.Core.Interfaces.IServices
{
    // 订单分配服务接口 - 处理订单自动分配和骑手接单逻辑
    public interface IOrderAssignmentService
    {
        // 自动分配订单给最近的在线骑手
        // <param name="orderId">订单ID</param>
        // <returns>分配编号，如果分配失败返回null</returns>
        Task<string?> AutoAssignOrderAsync(string orderId);
        
        // 手动分配订单给指定骑手
        // <param name="orderId">订单ID</param>
        // <param name="riderId">骑手ID</param>
        // <returns>是否分配成功</returns>
        Task<bool> AssignOrderToRiderAsync(string orderId, string riderId);
        
        // 骑手接受订单
        // <param name="orderId">订单ID</param>
        // <param name="riderId">骑手ID</param>
        // <returns>是否接受成功</returns>
        Task<bool> AcceptOrderAsync(string orderId, string riderId);
        
        // 骑手拒绝订单
        // <param name="orderId">订单ID</param>
        // <param name="riderId">骑手ID</param>
        // <returns>是否拒绝成功</returns>
        Task<bool> RejectOrderAsync(string orderId, string riderId);
        
        // 更新订单状态
        // <param name="orderId">订单ID</param>
        // <param name="newStatus">新状态</param>
        // <returns>是否更新成功</returns>
        Task<bool> UpdateOrderStatusAsync(string orderId, OrderStatus newStatus);
        
        // 获取订单的分配信息
        // <param name="orderId">订单ID</param>
        // <returns>分配信息，如果未分配返回null</returns>
        Task<dynamic?> GetOrderAssignmentAsync(string orderId);
        
        // 获取骑手的当前分配
        // <param name="riderId">骑手ID</param>
        // <returns>骑手的当前分配列表</returns>
        Task<IEnumerable<dynamic>> GetRiderAssignmentsAsync(string riderId);
    }
}