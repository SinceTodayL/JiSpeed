using JISpeed.Core.Entities.Order;

namespace JISpeed.Core.Interfaces.IRepositories.Order
{
    // 支付仓储接口
    public interface IPaymentRepository
    {
        // 根据支付ID获取支付信息
        // <param name="payId">支付ID</param>
        // <returns>支付实体，如果不存在则返回null</returns>
        Task<Payment?> GetByIdAsync(string payId);

        // 根据支付ID获取支付详细信息（包含关联数据）
        // <param name="payId">支付ID</param>
        // <returns>包含关联数据的支付实体，如果不存在则返回null</returns>
        Task<Payment?> GetWithDetailsAsync(string payId);

        // 根据订单ID获取支付列表
        // <param name="orderId">订单ID</param>
        // <returns>支付列表</returns>
        Task<List<Payment>> GetByOrderIdAsync(string orderId);

        // 根据支付状态获取支付列表
        // <param name="payStatus">支付状态</param>
        // <returns>支付列表</returns>
        Task<List<Payment>> GetByPayStatusAsync(int payStatus);

        // 根据支付渠道获取支付列表
        // <param name="channel">支付渠道</param>
        // <returns>支付列表</returns>
        Task<List<Payment>> GetByChannelAsync(string channel);

        // 根据时间范围获取支付列表
        // <param name="startTime">开始时间</param>
        // <param name="endTime">结束时间</param>
        // <returns>支付列表</returns>
        Task<List<Payment>> GetByTimeRangeAsync(DateTime startTime, DateTime endTime);

        // 获取所有支付列表
        // <returns>支付列表</returns>
        Task<List<Payment>> GetAllAsync();

        // 检查支付是否存在
        // <param name="payId">支付ID</param>
        // <returns>支付是否存在</returns>
        Task<bool> ExistsAsync(string payId);

        // 创建新支付
        // <param name="payment">支付实体</param>
        // <returns>创建的支付实体</returns>
        Task<Payment> CreateAsync(Payment payment);

        // 更新支付信息
        // <param name="payment">支付实体</param>
        // <returns>更新的支付实体</returns>
        Task<Payment> UpdateAsync(Payment payment);

        // 删除支付
        // <param name="payId">支付ID</param>
        // <returns>是否删除成功</returns>
        Task<bool> DeleteAsync(string payId);

        // 保存更改
        // <returns>保存的记录数</returns>
        Task<int> SaveChangesAsync();
    }
}
