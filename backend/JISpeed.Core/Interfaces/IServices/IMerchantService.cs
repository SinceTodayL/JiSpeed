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
            string? location,string? description);
        
        Task<Merchant> GetMerchantDetailAsync(string merchantId);
        Task<List<string>> GetMerchantNameForSearchAsync(string prefix,int ?limit);

        
        Task<List<Merchant>> GetMerchantByFiltersAsync(
            int? size, int? page,
            int? status,
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
    
    
    public enum MerchantStatus
    { 
        InReview = 0,            // 审核中
        Open = 1,                // 正常营业
        Close = 2,               // 休息中
        Baned =3                 // 封禁中
    }

}