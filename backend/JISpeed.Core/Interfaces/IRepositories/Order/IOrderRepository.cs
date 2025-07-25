using JISpeed.Core.Entities.Order;

namespace JISpeed.Core.Interfaces.IRepositories.Order
{
    // 订单仓储接口
    public interface IOrderRepository
    {
        // 根据订单ID获取订单信息
        // <param name="orderId">订单ID</param>
        // <returns>订单实体，如果不存在则返回null</returns>
        Task<JISpeed.Core.Entities.Order.Order?> GetByIdAsync(string orderId);

        // 根据订单ID获取订单详细信息（包含关联数据）
        // <param name="orderId">订单ID</param>
        // <returns>包含关联数据的订单实体，如果不存在则返回null</returns>
        Task<JISpeed.Core.Entities.Order.Order?> GetWithDetailsAsync(string orderId);

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

        // 获取所有订单列表
        // <returns>订单列表</returns>
        Task<List<JISpeed.Core.Entities.Order.Order>> GetAllAsync();

        // 检查订单是否存在
        // <param name="orderId">订单ID</param>
        // <returns>订单是否存在</returns>
        Task<bool> ExistsAsync(string orderId);

        // 创建新订单
        // <param name="order">订单实体</param>
        // <returns>创建的订单实体</returns>
        Task<JISpeed.Core.Entities.Order.Order> CreateAsync(JISpeed.Core.Entities.Order.Order order);

        // 更新订单信息
        // <param name="order">订单实体</param>
        // <returns>更新的订单实体</returns>
        Task<JISpeed.Core.Entities.Order.Order> UpdateAsync(JISpeed.Core.Entities.Order.Order order);

        // 删除订单
        // <param name="orderId">订单ID</param>
        // <returns>是否删除成功</returns>
        Task<bool> DeleteAsync(string orderId);

        // 保存更改
        // <returns>保存的记录数</returns>
        Task<int> SaveChangesAsync();
    }
}
