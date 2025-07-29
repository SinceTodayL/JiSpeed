using JISpeed.Core.Entities.Rider;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Entities.Dish;
using JISpeed.Core.Entities.Merchant;

namespace JISpeed.Core.Interfaces.IServices
{
    public interface IDishService
    {


        // 创建菜品主体
        Task<bool> CreateDishEntityAsync(string merchantId, Dish dishId);
        // 删除菜品节点
        Task<bool> DeleteDishEntityAsync(string merchantId, string dish);
        // 修改菜品主体
        Task<bool>ModifyDishEntityAsync(string merchantId, string dishId, Dish dish);
        // 获取商家菜品列表
        Task<List<Dish>> GetDisheEntitiesAsync(string merchantId);
        
    }
}