using JISpeed.Core.Entities.Dish;
using JISpeed.Core.Interfaces.IRepositories;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories
{
    public class DishRepository : IDishRepository
    {
        private readonly OracleDbContext _context;

        public DishRepository(OracleDbContext context)
        {
            _context = context;
        }
        // 创建新数据
        // </summary>
        // <param name="data">数据实体</param>
        // <returns>创建的数据实体</returns>
        public async Task<Dish> CreateAsync(Dish data)
        {
            var entity = await _context.Dishes.AddAsync(data);
            return entity.Entity;
        }
        // 保存更改
        // </summary>
        // <returns>保存的记录数</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        // 获取商家菜品统计信息
        // </summary>
        // <param name="merchantId">商家ID</param>
        // <returns>信息实体</returns>
        public async Task<List<Dish>> GetDetailsAsync(string merchantId)
        {
            return await _context.Dishes.
                Where(u => u.MerchantId == merchantId)
                .ToListAsync(); 
        }
    }
}