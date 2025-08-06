using JISpeed.Core.Entities.Rider;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Entities.Dish;
using JISpeed.Core.Entities.Merchant;

namespace JISpeed.Core.Interfaces.IServices
{
    public interface IMerchantService
    {
        
        
        // 获取商店主体
        Task<bool> UpdateMerchantDetailAsync(
            string merchantId,string? merchantName,
            int? status, string? contactInfo,
            string? location);
        
        Task<Merchant> GetMerchantDetailAsync(string merchantId);

        
        Task<List<Merchant>> GetMerchantByFiltersAsync(
            int? size, int? page,
            string? merchantName,
            string? location);

        
        // 获取商家数据统计信息列表
        Task<SalesStat?> GetSalesStateByMerchantIdAndDateTimeAsync(string merchantId, DateTime statTime);
        // 自定义筛选
        Task<List<SalesStat>> GetByFiltersAsync(string merchantId, 
            int? statType, int? size, int? page,
            DateTime? startTime,DateTime? endTime,
            decimal? minAmount, decimal?maxAmount);
    }
}