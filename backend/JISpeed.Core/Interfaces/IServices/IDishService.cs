using JISpeed.Core.Entities.Rider;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Entities.Dish;
using JISpeed.Core.Entities.User;
using JISpeed.Core.Entities.Order;
using JISpeed.Core.Entities.Merchant;

namespace JISpeed.Core.Interfaces.IServices
{
    public interface IDishService
    {
        // 创建菜品主体
        Task<bool> CreateDishEntityAsync(
            string merchantId,
            string categoryId, string dishName,
            decimal? price, decimal originPrice,
            string? coverUrl, string? description,
            int? stockQuantity);
        // 删除菜品节点
        Task<bool> DeleteDishEntityAsync(string merchantId, string dish);
        // 修改菜品主体
        Task<bool> ModifyDishEntityAsync(
            string merchantId, string dishId,
            string? categoryId, string? dishName,
            decimal? price, decimal? originPrice,
            int? onSale, string? coverUrl,
            string? description, int? stockQuantity);
        // 获取商家菜品列表
        Task<List<Dish>> GetByFiltersAsync(
            string merchantId,
            string? categoryId,
            bool? orderByRating,
            bool? orderByHighPrice,
            bool? orderByLowPrice,
            int? size, int? page);
        Task<Dish?> GetDisheEntitiesAsync(
            string merchantId,
            string dishId);

        Task<List<Category>> GetMerchantCategory(string merchantId);

        Task<List<(Review Review, User User)>> GetReviewsByDishIdAsync(string dishId);
        
    }
}