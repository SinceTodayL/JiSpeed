using JISpeed.Core.Entities.Order;

namespace JISpeed.Core.Interfaces.IRepositories.Order
{
    // 支付仓储接口
    public interface IPaymentRepository : IBaseRepository<Payment, string>
    {
        
        // === 业务专用查询方法 ===

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

    }
}