using JISpeed.Core.Entities.Common;

namespace JISpeed.Core.Interfaces.IRepositories.Merchant
{
    // 商家仓储接口
    public interface IMerchantRepository : IBaseRepository<JISpeed.Core.Entities.Merchant.Merchant, string>
    {
        // === 业务专用查询方法 ===

        Task<List<JISpeed.Core.Entities.Merchant.Merchant>> GetAllMerchantsAsync(int? size,int? page);

        
        // 根据ApplicationUserId获取商家信息
        // <param name="applicationUserId">应用用户ID</param>
        // <returns>商家实体，如果不存在则返回null</returns>
        Task<JISpeed.Core.Entities.Merchant.Merchant?> GetByApplicationUserIdAsync(string applicationUserId);

        // 根据商家名称搜索商家
        // <param name="merchantName">商家名称</param>
        // <returns>商家列表</returns>
        Task<List<JISpeed.Core.Entities.Merchant.Merchant>> SearchByNameAsync(string merchantName,int? size,int? page);

        // 根据状态获取商家列表
        // <param name="status">状态</param>
        // <returns>商家列表</returns>
        Task<List<JISpeed.Core.Entities.Merchant.Merchant>> GetByStatusAsync(int status);

        // 根据位置搜索商家
        // <param name="location">位置</param>
        // <returns>商家列表</returns>
        Task<List<JISpeed.Core.Entities.Merchant.Merchant>> SearchByLocationAsync(string location,int? size,int? page);
        
        // 使用ApplicationUser进行用户的创建
        // <returns>用户实体，如果不存在则返回null</returns>
        Task<Core.Entities.Merchant.Merchant?> CreateFromApplicationUserAsync(ApplicationUser applicationUser);

    }
}