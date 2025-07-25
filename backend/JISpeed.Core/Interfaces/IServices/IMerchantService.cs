using JISpeed.Core.Entities.Rider;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Entities.Dish;
using JISpeed.Core.Entities.Merchant;
using JISpeed.Core.Interfaces.IRepositories;

namespace JISpeed.Core.Interfaces.IServices
{
    public interface IMerchantService
    {
        /// 创建用户实体（当ApplicationUser的UserType=2时调用）
        
        /// <param name="applicationUser">已创建的ApplicationUser</param>
        /// <param name="nickname">用户昵称，默认使用用户名</param>
        /// <returns>创建的Merchant实体</returns>
        Task<Merchant> CreateUserEntityAsync(ApplicationUser applicationUser, string? nickname = null);

        Task<Merchant?> GetMerchantDetailAsync(string merchantId);
        Task<List<SalesStat>> GetSalesStateAsync(string merchantId);
        Task<List<Dish>> GetDishesAsync(string merchantId);
    }
}