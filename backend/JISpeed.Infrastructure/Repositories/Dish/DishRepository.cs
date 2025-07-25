using JISpeed.Core.Entities.Dish;
using JISpeed.Core.Interfaces.IRepositories.Dish;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories.Dish
{
    public class DishRepository : IDishRepository
    {
        private readonly OracleDbContext _context;

        public DishRepository(OracleDbContext context)
        {
            _context = context;
        }

        // 根据菜品ID获取菜品信息
        // <param name="dishId">菜品ID</param>
        // <returns>菜品实体，如果不存在则返回null</returns>
        public async Task<JISpeed.Core.Entities.Dish.Dish?> GetByIdAsync(string dishId)
        {
            return await _context.Dishes
                .FirstOrDefaultAsync(d => d.DishId == dishId);
        }

        // 根据菜品ID获取菜品详细信息（包含关联数据）
        // <param name="dishId">菜品ID</param>
        // <returns>包含关联数据的菜品实体，如果不存在则返回null</returns>
        public async Task<JISpeed.Core.Entities.Dish.Dish?> GetWithDetailsAsync(string dishId)
        {
            return await _context.Dishes
                .Include(d => d.Merchant)
                .Include(d => d.Category)
                .Include(d => d.OrderDishes)
                .Include(d => d.DishReviews)
                .FirstOrDefaultAsync(d => d.DishId == dishId);
        }

        // 根据商家ID获取菜品列表
        // <param name="merchantId">商家ID</param>
        // <returns>菜品列表</returns>
        public async Task<List<JISpeed.Core.Entities.Dish.Dish>> GetByMerchantIdAsync(string merchantId)
        {
            return await _context.Dishes
                .Where(d => d.MerchantId == merchantId)
                .Include(d => d.Category)
                .OrderBy(d => d.CategoryId)
                .ThenBy(d => d.DishName)
                .ToListAsync();
        }

        // 根据分类ID获取菜品列表
        // <param name="categoryId">分类ID</param>
        // <returns>菜品列表</returns>
        public async Task<List<JISpeed.Core.Entities.Dish.Dish>> GetByCategoryIdAsync(string categoryId)
        {
            return await _context.Dishes
                .Where(d => d.CategoryId == categoryId)
                .Include(d => d.Merchant)
                .OrderBy(d => d.DishName)
                .ToListAsync();
        }

        // 获取所有菜品列表
        // <returns>菜品列表</returns>
        public async Task<List<JISpeed.Core.Entities.Dish.Dish>> GetAllAsync()
        {
            return await _context.Dishes
                .Include(d => d.Merchant)
                .Include(d => d.Category)
                .OrderBy(d => d.MerchantId)
                .ThenBy(d => d.CategoryId)
                .ThenBy(d => d.DishName)
                .ToListAsync();
        }

        // 检查菜品是否存在
        // <param name="dishId">菜品ID</param>
        // <returns>菜品是否存在</returns>
        public async Task<bool> ExistsAsync(string dishId)
        {
            return await _context.Dishes
                .AnyAsync(d => d.DishId == dishId);
        }

        // 创建新菜品
        // <param name="dish">菜品实体</param>
        // <returns>创建的菜品实体</returns>
        public async Task<JISpeed.Core.Entities.Dish.Dish> CreateAsync(JISpeed.Core.Entities.Dish.Dish dish)
        {
            var entity = await _context.Dishes.AddAsync(dish);
            return entity.Entity;
        }

        // 更新菜品信息
        // <param name="dish">菜品实体</param>
        // <returns>更新的菜品实体</returns>
        public async Task<JISpeed.Core.Entities.Dish.Dish> UpdateAsync(JISpeed.Core.Entities.Dish.Dish dish)
        {
            var entity = _context.Dishes.Update(dish);
            await Task.CompletedTask; // 解决异步警告
            return entity.Entity;
        }

        // 删除菜品
        // <param name="dishId">菜品ID</param>
        // <returns>是否删除成功</returns>
        public async Task<bool> DeleteAsync(string dishId)
        {
            var dish = await GetByIdAsync(dishId);
            if (dish == null)
                return false;

            _context.Dishes.Remove(dish);
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
