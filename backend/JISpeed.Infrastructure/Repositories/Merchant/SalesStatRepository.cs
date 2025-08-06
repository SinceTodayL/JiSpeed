using JISpeed.Core.Entities.Merchant;
using JISpeed.Core.Interfaces.IRepositories.Merchant;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories.Merchant
{
    public class SalesStatRepository : ISalesStatRepository
    {
        private readonly OracleDbContext _context;

        public SalesStatRepository(OracleDbContext context)
        {
            _context = context;
        }

        // 根据复合主键获取销售统计信息
        // <param name="statDate">统计日期</param>
        // <param name="merchantId">商家ID</param>
        // <returns>销售统计实体，如果不存在则返回null</returns>
        public async Task<SalesStat?> GetByIdAsync(DateTime statDate, string merchantId)
        {
            return await _context.SalesStats
                .FirstOrDefaultAsync(s => s.StatDate.Date == statDate.Date && s.MerchantId == merchantId);
        }

        // 根据复合主键获取销售统计详细信息（包含关联数据）
        // <param name="statDate">统计日期</param>
        // <param name="merchantId">商家ID</param>
        // <returns>包含关联数据的销售统计实体，如果不存在则返回null</returns>
        public async Task<SalesStat?> GetWithDetailsAsync(DateTime statDate, string merchantId)
        {
            return await _context.SalesStats
                .Include(s => s.Merchant)
                .ThenInclude(m => m.ApplicationUser)
                .FirstOrDefaultAsync(s => s.StatDate.Date == statDate.Date && s.MerchantId == merchantId);
        }

        // 根据商家ID获取销售统计列表
        // <param name="merchantId">商家ID</param>
        // <returns>销售统计列表</returns>
        public async Task<List<SalesStat>> GetByMerchantIdAsync(string merchantId, int? size, int? page)
        {
            int pageSize = size ?? 20;  // 默认每页20条
            int currentPage = page ?? 1; // 默认第1页
            
            return await _context.SalesStats
                .Where(s => s.MerchantId == merchantId)
                .OrderByDescending(s => s.StatDate)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // 根据时间范围获取销售统计列表
        // <param name="startDate">开始日期</param>
        // <param name="endDate">结束日期</param>
        // <returns>销售统计列表</returns>
        public async Task<List<SalesStat>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.SalesStats
                .Where(s => s.StatDate.Date >= startDate.Date && s.StatDate.Date <= endDate.Date)
                .Include(s => s.Merchant)
                .OrderByDescending(s => s.StatDate)
                .ThenBy(s => s.MerchantId)
                .ToListAsync();
        }

        // 根据商家ID和时间范围获取销售统计列表
        // <param name="merchantId">商家ID</param>
        // <param name="startDate">开始日期</param>
        // <param name="endDate">结束日期</param>
        // <returns>销售统计列表</returns>
        public async Task<List<SalesStat>> GetByMerchantIdAndDateRangeAsync(string merchantId, DateTime startDate, DateTime endDate)
        {
            return await _context.SalesStats
                .Where(s => s.MerchantId == merchantId &&
                           s.StatDate.Date >= startDate.Date &&
                           s.StatDate.Date <= endDate.Date)
                .OrderByDescending(s => s.StatDate)
                .ToListAsync();
        }

        // 获取所有销售统计列表
        // <returns>销售统计列表</returns>
        public async Task<List<SalesStat>> GetAllAsync()
        {
            return await _context.SalesStats
                .Include(s => s.Merchant)
                .OrderByDescending(s => s.StatDate)
                .ThenBy(s => s.MerchantId)
                .ToListAsync();
        }

        // 检查销售统计是否存在
        // <param name="statDate">统计日期</param>
        // <param name="merchantId">商家ID</param>
        // <returns>销售统计是否存在</returns>
        public async Task<bool> ExistsAsync(DateTime statDate, string merchantId)
        {
            return await _context.SalesStats
                .AnyAsync(s => s.StatDate.Date == statDate.Date && s.MerchantId == merchantId);
        }

        // 创建新销售统计
        // <param name="salesStat">销售统计实体</param>
        // <returns>创建的销售统计实体</returns>
        public async Task<SalesStat> CreateAsync(SalesStat salesStat)
        {
            var entity = await _context.SalesStats.AddAsync(salesStat);
            return entity.Entity;
        }

        // 更新销售统计信息
        // <param name="salesStat">销售统计实体</param>
        // <returns>更新的销售统计实体</returns>
        public async Task<SalesStat> UpdateAsync(SalesStat salesStat)
        {
            var entity = _context.SalesStats.Update(salesStat);
            await Task.CompletedTask; // 解决异步警告
            return entity.Entity;
        }

        // 删除销售统计
        // <param name="statDate">统计日期</param>
        // <param name="merchantId">商家ID</param>
        // <returns>是否删除成功</returns>
        public async Task<bool> DeleteAsync(DateTime statDate, string merchantId)
        {
            var salesStat = await GetByIdAsync(statDate, merchantId);
            if (salesStat == null)
                return false;

            _context.SalesStats.Remove(salesStat);
            return true;
        }

        // 保存更改
        // <returns>保存的记录数</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // === 业务专用查询方法 ===

        // 根据统计类型获取记录 - 按销售额范围分类
        // <param name="statType">统计类型 (1=低销售额, 2=中销售额, 3=高销售额)</param>
        // <returns>销售统计列表</returns>
        public async Task<List<SalesStat>> GetByStatTypeAsync(int statType)
        {
            return statType switch
            {
                1 => await _context.SalesStats // 低销售额 (0-1000)
                    .Where(s => s.SalesAmount <= 1000)
                    .Include(s => s.Merchant)
                    .OrderByDescending(s => s.StatDate)
                    .ToListAsync(),
                2 => await _context.SalesStats // 中销售额 (1000-5000)
                    .Where(s => s.SalesAmount > 1000 && s.SalesAmount <= 5000)
                    .Include(s => s.Merchant)
                    .OrderByDescending(s => s.StatDate)
                    .ToListAsync(),
                3 => await _context.SalesStats // 高销售额 (5000+)
                    .Where(s => s.SalesAmount > 5000)
                    .Include(s => s.Merchant)
                    .OrderByDescending(s => s.StatDate)
                    .ToListAsync(),
                _ => await _context.SalesStats
                    .Include(s => s.Merchant)
                    .OrderByDescending(s => s.StatDate)
                    .ToListAsync()
            };
        }
        
        // 根据统计类型获取记录 - 按销售额范围分类
        // <returns>销售统计列表</returns>
        public async Task<List<SalesStat>> GetBySalesRangeAsync(decimal minAmount,decimal maxAmount,int?size, int? page)
        {
            int pageSize = size ?? 20;  // 默认每页20条
            int currentPage = page ?? 1; // 默认第1页

            return await _context.SalesStats // 低销售额 (0-1000)
                .Where(s => s.SalesAmount <= maxAmount && s.SalesAmount >= minAmount)
                .Include(s => s.Merchant)
                .OrderByDescending(s => s.StatDate)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // 根据时间范围获取销售统计 (重命名以避免重复)
        // <param name="startTime">开始时间</param>
        // <param name="endTime">结束时间</param>
        // <returns>销售统计列表</returns>
        public async Task<List<SalesStat>> GetByTimeRangeAsync(DateTime startTime, DateTime endTime)
        {
            return await _context.SalesStats
                .Where(s => s.StatDate.Date >= startTime.Date && s.StatDate.Date <= endTime.Date)
                .Include(s => s.Merchant)
                .OrderByDescending(s => s.StatDate)
                .ThenBy(s => s.MerchantId)
                .ToListAsync();
        }

        // 根据商家ID和统计类型获取记录
        // <param name="merchantId">商家ID</param>
        // <param name="statType">统计类型</param>
        // <returns>销售统计列表</returns>
        public async Task<List<SalesStat>> GetByMerchantIdAndStatTypeAsync(string merchantId, int statType,int? size, int? page)
        {
            var baseQuery = _context.SalesStats.Where(s => s.MerchantId == merchantId);
            int pageSize = size ?? 20;  // 默认每页20条
            int currentPage = page ?? 1; // 默认第1页
            
            return statType switch
            {
                1 => await baseQuery.Where(s => s.SalesAmount <= 1000)
                    .OrderByDescending(s => s.StatDate)
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(),
                2 => await baseQuery.Where(s => s.SalesAmount > 1000 && s.SalesAmount <= 5000)
                    .OrderByDescending(s => s.StatDate)
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(),
                3 => await baseQuery.Where(s => s.SalesAmount > 5000)
                    .OrderByDescending(s => s.StatDate)
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(),
                _ => await baseQuery.OrderByDescending(s => s.StatDate)
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync()
            };
        }

        // 根据商家ID和时间范围获取统计 (接口方法)
        // <param name="merchantId">商家ID</param>
        // <param name="startTime">开始时间</param>
        // <param name="endTime">结束时间</param>
        // <returns>销售统计列表</returns>
        public async Task<List<SalesStat>> GetByMerchantIdAndTimeRangeAsync(string merchantId, DateTime startTime, DateTime endTime,int ?size, int? page)
        {
            int pageSize = size ?? 20;  // 默认每页20条
            int currentPage = page ?? 1; // 默认第1页
            return await _context.SalesStats
                .Where(s => s.MerchantId == merchantId &&
                           s.StatDate.Date >= startTime.Date &&
                           s.StatDate.Date <= endTime.Date)
                .OrderByDescending(s => s.StatDate)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // 获取最新的销售统计
        // <param name="merchantId">商家ID</param>
        // <param name="statType">统计类型</param>
        // <returns>最新的销售统计</returns>
        public async Task<SalesStat?> GetLatestByMerchantIdAndStatTypeAsync(string merchantId, int statType)
        {
            var baseQuery = _context.SalesStats.Where(s => s.MerchantId == merchantId);

            return statType switch
            {
                1 => await baseQuery.Where(s => s.SalesAmount <= 1000)
                    .OrderByDescending(s => s.StatDate).FirstOrDefaultAsync(),
                2 => await baseQuery.Where(s => s.SalesAmount > 1000 && s.SalesAmount <= 5000)
                    .OrderByDescending(s => s.StatDate).FirstOrDefaultAsync(),
                3 => await baseQuery.Where(s => s.SalesAmount > 5000)
                    .OrderByDescending(s => s.StatDate).FirstOrDefaultAsync(),
                _ => await baseQuery.OrderByDescending(s => s.StatDate).FirstOrDefaultAsync()
            };
        }
    }
}