using JISpeed.Core.Entities.Rider;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Entities.Dish;
using JISpeed.Core.Entities.Merchant;
using JISpeed.Core.Interfaces.IRepositories;

namespace JISpeed.Core.Interfaces.IServices
{
    public interface IMerchantService
    {

        Task<Merchant?> GetMerchantDetailAsync(string merchantId);
        Task<List<SalesStat>> GetSalesStateAsync(string merchantId);
        Task<List<Dish>> GetDishesAsync(string merchantId);
    }
}