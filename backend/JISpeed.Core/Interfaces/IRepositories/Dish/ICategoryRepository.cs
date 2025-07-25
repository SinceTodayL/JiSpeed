using JISpeed.Core.Entities.Dish;

namespace JISpeed.Core.Interfaces.IRepositories.Dish
{
    // 菜品分类仓储接口
    public interface ICategoryRepository
    {
        // 根据分类ID获取分类信息
        // <param name="categoryId">分类ID</param>
        // <returns>分类实体，如果不存在则返回null</returns>
        Task<Category?> GetByIdAsync(string categoryId);

        // 根据分类ID获取分类详细信息（包含关联数据）
        // <param name="categoryId">分类ID</param>
        // <returns>包含关联数据的分类实体，如果不存在则返回null</returns>
        Task<Category?> GetWithDetailsAsync(string categoryId);

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

        // 获取所有分类列表
        // <returns>分类列表</returns>
        Task<List<Category>> GetAllAsync();

        // 检查分类是否存在
        // <param name="categoryId">分类ID</param>
        // <returns>分类是否存在</returns>
        Task<bool> ExistsAsync(string categoryId);

        // 检查分类名称是否存在
        // <param name="categoryName">分类名称</param>
        // <param name="excludeId">排除的分类ID（用于更新时检查）</param>
        // <returns>分类名称是否存在</returns>
        Task<bool> ExistsByNameAsync(string categoryName, string? excludeId = null);

        // 创建新分类
        // <param name="category">分类实体</param>
        // <returns>创建的分类实体</returns>
        Task<Category> CreateAsync(Category category);

        // 更新分类信息
        // <param name="category">分类实体</param>
        // <returns>更新的分类实体</returns>
        Task<Category> UpdateAsync(Category category);

        // 删除分类
        // <param name="categoryId">分类ID</param>
        // <returns>是否删除成功</returns>
        Task<bool> DeleteAsync(string categoryId);

        // 保存更改
        // <returns>保存的记录数</returns>
        Task<int> SaveChangesAsync();
    }
}
