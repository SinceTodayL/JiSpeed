using JISpeed.Core.Entities.Merchant;
using JISpeed.Core.Interfaces.IRepositories;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories
{
    public class SalesStatRepository : ISalesStatRepository
    {
        private readonly OracleDbContext _context;

        public SalesStatRepository(OracleDbContext context)
        {
            _context = context;
        }
        // 创建新数据
        // </summary>
        // <param name="data">数据实体</param>
        // <returns>创建的数据实体</returns>
        public async Task<SalesStat> CreateAsync(SalesStat data)
        {
            var entity = await _context.SalesStats.AddAsync(data);
            return entity.Entity;
        }
        // 保存更改
        // </summary>
        // <returns>保存的记录数</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        // 获取商家数据统计信息
        // </summary>
        // <param name="merchantId">商家ID</param>
        // <returns>信息实体</returns>
        public async Task<List<SalesStat>> GetDetailsAsync(string merchantId)
        {
            return await _context.SalesStats.
                Where(u => u.MerchantId == merchantId)
                .ToListAsync(); 
        }
    }
}