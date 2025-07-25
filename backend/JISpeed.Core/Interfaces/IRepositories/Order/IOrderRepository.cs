using JISpeed.Core.Entities.Order;

namespace JISpeed.Core.Interfaces.IRepositories.Order
{
    // 订单仓储接口
    public interface IOrderRepository : IBaseRepository<JISpeed.Core.Entities.Order.Order, string>
    {
        // === 业务专用查询方法 ===

        // 根据用户ID获取订单列表
        // <param name="userId">用户ID</param>
        // <returns>订单列表</returns>
        Task<List<JISpeed.Core.Entities.Order.Order>> GetByUserIdAsync(string userId);

        // 根据状态获取订单列表
        // <param name="status">状态</param>
        // <returns>订单列表</returns>
        Task<List<JISpeed.Core.Entities.Order.Order>> GetByStatusAsync(int status);

        // 根据用户ID和状态获取订单列表
        // <param name="userId">用户ID</param>
        // <param name="status">状态</param>
        // <returns>订单列表</returns>
        Task<List<JISpeed.Core.Entities.Order.Order>> GetByUserIdAndStatusAsync(string userId, int status);

        // 根据时间范围获取订单列表
        // <param name="startTime">开始时间</param>
        // <param name="endTime">结束时间</param>
        // <returns>订单列表</returns>
        Task<List<JISpeed.Core.Entities.Order.Order>> GetByTimeRangeAsync(DateTime startTime, DateTime endTime);
    }
}
