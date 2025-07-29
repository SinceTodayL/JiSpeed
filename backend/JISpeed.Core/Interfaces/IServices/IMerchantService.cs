using JISpeed.Core.Entities.Rider;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Entities.Dish;
using JISpeed.Core.Entities.Merchant;

namespace JISpeed.Core.Interfaces.IServices
{
    public interface IMerchantService
    {
        
        
        // 获取商店主体
        Task<Merchant> GetMerchantDetailAsync(string merchantId);
        
        // 获取商家数据统计信息列表
        Task<List<SalesStat>> GetSalesStateAsync(string merchantId);

        
    }
}