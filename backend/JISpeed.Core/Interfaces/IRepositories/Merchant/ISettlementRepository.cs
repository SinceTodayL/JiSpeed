using JISpeed.Core.Entities.Merchant;

namespace JISpeed.Core.Interfaces.IRepositories.Merchant
{
    // 结算仓储接口
    public interface ISettlementRepository : IBaseRepository<Settlement, string>
    {
        // === 业务专用查询方法 ===

        // 根据商家ID获取结算记录
        // <param name="merchantId">商家ID</param>
        // <returns>结算记录列表</returns>
        Task<List<Settlement>> GetByMerchantIdAsync(string merchantId);

        // 根据结算状态获取记录
        // <param name="status">结算状态</param>
        // <returns>结算记录列表</returns>
        Task<List<Settlement>> GetByStatusAsync(int status);

        // 根据时间范围获取结算记录
        // <param name="startTime">开始时间</param>
        // <param name="endTime">结束时间</param>
        // <returns>结算记录列表</returns>
        Task<List<Settlement>> GetByTimeRangeAsync(DateTime startTime, DateTime endTime);

        // 根据商家ID和时间范围获取结算记录
        // <param name="merchantId">商家ID</param>
        // <param name="startTime">开始时间</param>
        // <param name="endTime">结束时间</param>
        // <returns>结算记录列表</returns>
        Task<List<Settlement>> GetByMerchantIdAndTimeRangeAsync(string merchantId, DateTime startTime, DateTime endTime);

        // 获取待结算记录
        // <returns>待结算记录列表</returns>
        Task<List<Settlement>> GetPendingSettlementsAsync();
    }
}
