using JISpeed.Core.Entities.Merchant;

namespace JISpeed.Core.Interfaces.IRepositories.Merchant
{
    // 结算仓储接口
    public interface ISettlementRepository
    {
        // 根据结算ID获取结算信息
        // <param name="settleId">结算ID</param>
        // <returns>结算实体，如果不存在则返回null</returns>
        Task<Settlement?> GetByIdAsync(string settleId);

        // 根据结算ID获取结算详细信息（包含关联数据）
        // <param name="settleId">结算ID</param>
        // <returns>包含关联数据的结算实体，如果不存在则返回null</returns>
        Task<Settlement?> GetWithDetailsAsync(string settleId);

        // 根据商家ID获取结算列表
        // <param name="merchantId">商家ID</param>
        // <returns>结算列表</returns>
        Task<List<Settlement>> GetByMerchantIdAsync(string merchantId);

        // 根据结算状态获取结算列表（是否已结算）
        // <param name="isSettled">是否已结算</param>
        // <returns>结算列表</returns>
        Task<List<Settlement>> GetBySettlementStatusAsync(bool isSettled);

        // 根据周期范围获取结算列表
        // <param name="startDate">开始日期</param>
        // <param name="endDate">结束日期</param>
        // <returns>结算列表</returns>
        Task<List<Settlement>> GetByPeriodRangeAsync(DateTime startDate, DateTime endDate);

        // 根据商家ID和结算状态获取结算列表
        // <param name="merchantId">商家ID</param>
        // <param name="isSettled">是否已结算</param>
        // <returns>结算列表</returns>
        Task<List<Settlement>> GetByMerchantIdAndStatusAsync(string merchantId, bool isSettled);

        // 根据商家ID和结算ID获取结算信息
        // <param name="merchantId">商家ID</param>
        // <param name="settleId">结算ID</param>
        // <returns>结算实体，如果不存在则返回null</returns>
        Task<Settlement?> GetByMerchantIdAndSettleIdAsync(string merchantId, string settleId);

        // 获取所有结算列表
        // <returns>结算列表</returns>
        Task<List<Settlement>> GetAllAsync();

        // 检查结算是否存在
        // <param name="settleId">结算ID</param>
        // <returns>结算是否存在</returns>
        Task<bool> ExistsAsync(string settleId);

        // 创建新结算
        // <param name="settlement">结算实体</param>
        // <returns>创建的结算实体</returns>
        Task<Settlement> CreateAsync(Settlement settlement);

        // 更新结算信息
        // <param name="settlement">结算实体</param>
        // <returns>更新的结算实体</returns>
        Task<Settlement> UpdateAsync(Settlement settlement);

        // 删除结算
        // <param name="settleId">结算ID</param>
        // <returns>是否删除成功</returns>
        Task<bool> DeleteAsync(string settleId);

        // 保存更改
        // <returns>保存的记录数</returns>
        Task<int> SaveChangesAsync();
    }
}