using JISpeed.Core.Entities.Dish;

namespace JISpeed.Core.Interfaces.IRepositories.Dish
{
    // 菜品分类仓储接口
    public interface ICategoryRepository : IBaseRepository<Category, string>
    {
        // === 业务专用查询方法 ===

        // 根据父级分类ID获取子分类列表
        // <param name="parentId">父级分类ID</param>
        // <returns>子分类列表</returns>
        Task<List<Category>> GetByParentIdAsync(string? parentId);

        // 获取顶级分类列表
        // <returns>顶级分类列表</returns>
        Task<List<Category>> GetTopLevelCategoriesAsync();

        // 根据分类名称搜索分类
        // <param name="categoryName">分类名称</param>
        // <returns>分类列表</returns>
        Task<List<Category>> SearchByNameAsync(string categoryName);

        // 检查分类名称是否存在
        // <param name="categoryName">分类名称</param>
        // <param name="excludeId">排除的分类ID（用于更新时检查）</param>
        // <returns>分类名称是否存在</returns>
        Task<bool> ExistsByNameAsync(string categoryName, string? excludeId = null);
    }
}
