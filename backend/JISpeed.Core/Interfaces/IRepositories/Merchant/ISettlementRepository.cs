using JISpeed.Core.Entities.Merchant;

namespace JISpeed.Core.Interfaces.IRepositories.Merchant
{
    // 结算仓储接口
    public interface ISettlementRepository
    {
        // === 业务专用查询方法 ===

        Task<Settlement?> GetWithDetailsAsync(string settleId);
        // 根据商家ID获取结算记录
        // <param name="merchantId">商家ID</param>
        // <returns>结算记录列表</returns>
        Task<List<Settlement>> GetByMerchantIdAsync(
            string merchantId,
            int ?size,int ?page);

        Task<List<Settlement>> GetAllAsync();

        // 根据结算状态获取记录
        // <param name="status">结算状态</param>
        // <returns>结算记录列表</returns>
        Task<List<Settlement>> GetSettledSettlementsAsync(
            int ?size,int ?page);
        
        Task<List<Settlement>> GetPendingSettlementsAsync(
            int ?size,int ?page);

        // 根据时间范围获取结算记录
        // <param name="startTime">开始时间</param>
        // <param name="endTime">结束时间</param>
        // <returns>结算记录列表</returns>
        Task<List<Settlement>> GetByPeriodRangeAsync(
            DateTime startTime, DateTime endTime,
            int ?size,int ?page);

        // 根据商家ID和时间范围获取结算记录
        // <param name="merchantId">商家ID</param>
        // <param name="startTime">开始时间</param>
        // <param name="endTime">结束时间</param>
        // <returns>结算记录列表</returns>
        Task<List<Settlement>> GetByMerchantIdAndTimeRangeAsync(
            string merchantId, 
            DateTime startTime, DateTime endTime,
            int ?size,int ?page);

        // 获取待结算记录
        // <returns>待结算记录列表</returns>
        Task<List<Settlement>> GetPendingSettlementsByMerchantIdAsync(
            string merchantId,
            int ?size, int ?page);
        Task<List<Settlement>> GetSettledSettlementsByMerchantIdAsync(
            string merchantId,
            int ?size, int ?page);

    }
}
