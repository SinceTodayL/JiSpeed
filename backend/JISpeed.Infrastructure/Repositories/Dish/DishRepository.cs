using JISpeed.Core.Entities.Dish;
using JISpeed.Core.Interfaces.IRepositories.Dish;
using JISpeed.Infrastructure.Data;
using JISpeed.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories.Dish
{
    public class DishRepository : BaseRepository<JISpeed.Core.Entities.Dish.Dish, string>, IDishRepository
    {
        public DishRepository(OracleDbContext context) : base(context)
        {
        }

        // 重写GetWithDetailsAsync方法以包含关联数据
        // <param name="dishId">菜品ID</param>
        // <returns>包含关联数据的菜品实体，如果不存在则返回null</returns>
        public override async Task<JISpeed.Core.Entities.Dish.Dish?> GetWithDetailsAsync(string dishId)
        {
            return await _context.Dishes
                .Include(d => d.Merchant)
                .Include(d => d.Category)
                .Include(d => d.OrderDishes)
                .Include(d => d.DishReviews)
                .FirstOrDefaultAsync(d => d.DishId == dishId);
        }

        // 重写GetAllAsync方法以包含关联数据和排序
        // <returns>菜品列表</returns>
        public override async Task<List<JISpeed.Core.Entities.Dish.Dish>> GetAllAsync()
        {
            return await _context.Dishes
                .Include(d => d.Merchant)
                .Include(d => d.Category)
                .OrderBy(d => d.MerchantId)
                .ThenBy(d => d.CategoryId)
                .ThenBy(d => d.DishName)
                .ToListAsync();
        }

        // === 业务专用查询方法 ===

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
        // 根据商家ID和菜品ID获取菜品
        // <param name="merchantId","dishId">商家ID，菜品ID</param>
        // <returns>菜品，不存在则返回null</returns>
        public async Task<JISpeed.Core.Entities.Dish.Dish?> GetByMerchantIdAndDishIdAsync(string merchantId,string dishId)
        {
            return await _context.Dishes.FirstOrDefaultAsync(d => d.MerchantId == merchantId && d.DishId == dishId);
        }
    }
}
