using JISpeed.Core.Entities.Dish;
using JISpeed.Core.Interfaces.IRepositories.Dish;
using JISpeed.Infrastructure.Data;
using JISpeed.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories.Dish
{
    public class CategoryRepository : BaseRepository<Category, string>, ICategoryRepository
    {
        public CategoryRepository(OracleDbContext context) : base(context)
        {
        }

        // 重写GetWithDetailsAsync以包含关联数据
        // <param name="categoryId">分类ID</param>
        // <returns>包含关联数据的分类实体，如果不存在则返回null</returns>
        public override async Task<Category?> GetWithDetailsAsync(string categoryId)
        {
            return await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.SubCategories)
                .Include(c => c.Dishes)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        }

        // 重写GetAllAsync以包含关联数据和排序
        // <returns>分类列表</returns>
        public override async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Include(c => c.ParentCategory)
                .OrderBy(c => c.SortOrder)
                .ThenBy(c => c.CategoryName)
                .ToListAsync();
        }

        // === 业务专用查询方法 ===

        // 根据父分类ID获取子分类列表
        // <param name="parentId">父分类ID</param>
        // <returns>子分类列表</returns>
        public async Task<List<Category>> GetByParentIdAsync(string? parentId)
        {
            return await _context.Categories
                .Where(c => c.ParentId == parentId)
                .OrderBy(c => c.SortOrder)
                .ThenBy(c => c.CategoryName)
                .ToListAsync();
        }

        // 获取顶级分类列表
        // <returns>顶级分类列表</returns>
        public async Task<List<Category>> GetTopLevelCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.ParentId == null)
                .OrderBy(c => c.SortOrder)
                .ThenBy(c => c.CategoryName)
                .ToListAsync();
        }

        // 根据分类名称搜索分类
        // <param name="categoryName">分类名称</param>
        // <returns>分类列表</returns>
        public async Task<List<Category>> SearchByNameAsync(string categoryName)
        {
            return await _context.Categories
                .Where(c => c.CategoryName.Contains(categoryName))
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        // 检查分类名称是否存在
        // <param name="categoryName">分类名称</param>
        // <param name="excludeId">排除的分类ID（用于更新时检查）</param>
        // <returns>分类名称是否存在</returns>
        public async Task<bool> ExistsByNameAsync(string categoryName, string? excludeId = null)
        {
            var query = _context.Categories.Where(c => c.CategoryName == categoryName);

            if (!string.IsNullOrEmpty(excludeId))
            {
                query = query.Where(c => c.CategoryId != excludeId);
            }

            return await query.AnyAsync();
        }
    }
}
