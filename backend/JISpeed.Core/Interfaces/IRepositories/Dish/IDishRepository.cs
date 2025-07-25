using JISpeed.Core.Entities.Dish;

namespace JISpeed.Core.Interfaces.IRepositories.Dish
{
    // 菜品仓储接口
    public interface IDishRepository : IBaseRepository<Entities.Dish.Dish, string>
    {
        // === 业务专用查询方法 ===

        // 根据商家ID获取菜品列表
        // <param name="merchantId">商家ID</param>
        // <returns>菜品列表</returns>
        Task<List<Entities.Dish.Dish>> GetByMerchantIdAsync(string merchantId);

        // 根据分类ID获取菜品列表
        // <param name="categoryId">分类ID</param>
        // <returns>菜品列表</returns>
        Task<List<Entities.Dish.Dish>> GetByCategoryIdAsync(string categoryId);
    }
}
