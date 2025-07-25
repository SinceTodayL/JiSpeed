using JISpeed.Core.Entities.Merchant;

namespace JISpeed.Core.Interfaces.IRepositories.Merchant
{
    // 商家仓储接口
    public interface IMerchantRepository
    {
        // 根据商家ID获取商家信息
        // <param name="merchantId">商家ID</param>
        // <returns>商家实体，如果不存在则返回null</returns>
        Task<JISpeed.Core.Entities.Merchant.Merchant?> GetByIdAsync(string merchantId);

        // 根据商家ID获取商家详细信息（包含关联数据）
        // <param name="merchantId">商家ID</param>
        // <returns>包含关联数据的商家实体，如果不存在则返回null</returns>
        Task<JISpeed.Core.Entities.Merchant.Merchant?> GetWithDetailsAsync(string merchantId);

        // 根据ApplicationUserId获取商家信息
        // <param name="applicationUserId">应用用户ID</param>
        // <returns>商家实体，如果不存在则返回null</returns>
        Task<JISpeed.Core.Entities.Merchant.Merchant?> GetByApplicationUserIdAsync(string applicationUserId);

        // 根据商家名称搜索商家
        // <param name="merchantName">商家名称</param>
        // <returns>商家列表</returns>
        Task<List<JISpeed.Core.Entities.Merchant.Merchant>> SearchByNameAsync(string merchantName);

        // 根据状态获取商家列表
        // <param name="status">状态</param>
        // <returns>商家列表</returns>
        Task<List<JISpeed.Core.Entities.Merchant.Merchant>> GetByStatusAsync(int status);

        // 根据位置搜索商家
        // <param name="location">位置</param>
        // <returns>商家列表</returns>
        Task<List<JISpeed.Core.Entities.Merchant.Merchant>> SearchByLocationAsync(string location);

        // 获取所有商家列表
        // <returns>商家列表</returns>
        Task<List<JISpeed.Core.Entities.Merchant.Merchant>> GetAllAsync();

        // 检查商家是否存在
        // <param name="merchantId">商家ID</param>
        // <returns>商家是否存在</returns>
        Task<bool> ExistsAsync(string merchantId);

        // 创建新商家
        // <param name="merchant">商家实体</param>
        // <returns>创建的商家实体</returns>
        Task<JISpeed.Core.Entities.Merchant.Merchant> CreateAsync(JISpeed.Core.Entities.Merchant.Merchant merchant);

        // 更新商家信息
        // <param name="merchant">商家实体</param>
        // <returns>更新的商家实体</returns>
        Task<JISpeed.Core.Entities.Merchant.Merchant> UpdateAsync(JISpeed.Core.Entities.Merchant.Merchant merchant);

        // 删除商家
        // <param name="merchantId">商家ID</param>
        // <returns>是否删除成功</returns>
        Task<bool> DeleteAsync(string merchantId);

        // 保存更改
        // <returns>保存的记录数</returns>
        Task<int> SaveChangesAsync();
    }
}