using JISpeed.Core.Entities.Dish;
using JISpeed.Core.Interfaces.IRepositories.Dish;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories.Dish
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly OracleDbContext _context;

        public CategoryRepository(OracleDbContext context)
        {
            _context = context;
        }

        // 根据分类ID获取分类信息
        // <param name="categoryId">分类ID</param>
        // <returns>分类实体，如果不存在则返回null</returns>
        public async Task<Category?> GetByIdAsync(string categoryId)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        }

        // 根据分类ID获取分类详细信息（包含关联数据）
        // <param name="categoryId">分类ID</param>
        // <returns>包含关联数据的分类实体，如果不存在则返回null</returns>
        public async Task<Category?> GetWithDetailsAsync(string categoryId)
        {
            return await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.SubCategories)
                .Include(c => c.Dishes)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        }

        // 根据父级分类ID获取子分类列表
        // <param name="parentId">父级分类ID</param>
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

        // 获取所有分类列表
        // <returns>分类列表</returns>
        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Include(c => c.ParentCategory)
                .OrderBy(c => c.SortOrder)
                .ThenBy(c => c.CategoryName)
                .ToListAsync();
        }

        // 检查分类是否存在
        // <param name="categoryId">分类ID</param>
        // <returns>分类是否存在</returns>
        public async Task<bool> ExistsAsync(string categoryId)
        {
            return await _context.Categories
                .AnyAsync(c => c.CategoryId == categoryId);
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

        // 创建新分类
        // <param name="category">分类实体</param>
        // <returns>创建的分类实体</returns>
        public async Task<Category> CreateAsync(Category category)
        {
            var entity = await _context.Categories.AddAsync(category);
            return entity.Entity;
        }

        // 更新分类信息
        // <param name="category">分类实体</param>
        // <returns>更新的分类实体</returns>
        public async Task<Category> UpdateAsync(Category category)
        {
            var entity = _context.Categories.Update(category);
            await Task.CompletedTask; // 解决异步警告
            return entity.Entity;
        }

        // 删除分类
        // <param name="categoryId">分类ID</param>
        // <returns>是否删除成功</returns>
        public async Task<bool> DeleteAsync(string categoryId)
        {
            var category = await GetByIdAsync(categoryId);
            if (category == null)
                return false;

            _context.Categories.Remove(category);
            return true;
        }

        // 保存更改
        // <returns>保存的记录数</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
