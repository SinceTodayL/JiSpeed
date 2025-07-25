using JISpeed.Core.Entities.Dish;

namespace JISpeed.Core.Interfaces.IRepositories.Dish
{
    // 菜品仓储接口
    public interface IDishRepository
    {
        // 根据菜品ID获取菜品信息
        // <param name="dishId">菜品ID</param>
        // <returns>菜品实体，如果不存在则返回null</returns>
        Task<Entities.Dish.Dish?> GetByIdAsync(string dishId);

        // 根据菜品ID获取菜品详细信息（包含关联数据）
        // <param name="dishId">菜品ID</param>
        // <returns>包含关联数据的菜品实体，如果不存在则返回null</returns>
        Task<Entities.Dish.Dish?> GetWithDetailsAsync(string dishId);

        // 根据商家ID获取菜品列表
        // <param name="merchantId">商家ID</param>
        // <returns>菜品列表</returns>
        Task<List<Entities.Dish.Dish>> GetByMerchantIdAsync(string merchantId);

        // 根据分类ID获取菜品列表
        // <param name="categoryId">分类ID</param>
        // <returns>菜品列表</returns>
        Task<List<Entities.Dish.Dish>> GetByCategoryIdAsync(string categoryId);

        // 获取所有菜品列表
        // <returns>菜品列表</returns>
        Task<List<Entities.Dish.Dish>> GetAllAsync();

        // 检查菜品是否存在
        // <param name="dishId">菜品ID</param>
        // <returns>菜品是否存在</returns>
        Task<bool> ExistsAsync(string dishId);

        // 创建新菜品
        // <param name="dish">菜品实体</param>
        // <returns>创建的菜品实体</returns>
        Task<Entities.Dish.Dish> CreateAsync(Entities.Dish.Dish dish);

        // 更新菜品信息
        // <param name="dish">菜品实体</param>
        // <returns>更新的菜品实体</returns>
        Task<Entities.Dish.Dish> UpdateAsync(Entities.Dish.Dish dish);

        // 删除菜品
        // <param name="dishId">菜品ID</param>
        // <returns>是否删除成功</returns>
        Task<bool> DeleteAsync(string dishId);

        // 保存更改
        // <returns>保存的记录数</returns>
        Task<int> SaveChangesAsync();
    }
}
