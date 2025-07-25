using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JISpeed.Core.Entities.Order;
using JISpeed.Core.Interfaces.IRepositories.Order;
using JISpeed.Infrastructure.Data;
using JISpeed.Infrastructure.Repositories.Common;

namespace JISpeed.Infrastructure.Repositories.Order
{
    // 投诉仓储实现 - 处理投诉管理的数据访问操作
    public class ComplaintRepository : BaseRepository<Complaint, string>, IComplaintRepository
    {
        public ComplaintRepository(OracleDbContext context) : base(context)
        {
        }

        // 重写：根据主键查询投诉
        public override async Task<Complaint?> GetByIdAsync(string id)
        {
            return await _context.Complaints
                .Include(c => c.Order)
                .ThenInclude(o => o.User)
                .Include(c => c.Complainant)
                .FirstOrDefaultAsync(c => c.ComplaintId == id);
        }

        // 重写：查询投诉包含详细信息
        public override async Task<Complaint?> GetWithDetailsAsync(string id)
        {
            return await _context.Complaints
                .Include(c => c.Order)
                .ThenInclude(o => o.User)
                .Include(c => c.Complainant)
                .Include(c => c.Order)
                .ThenInclude(o => o.OrderDishes)
                .ThenInclude(od => od.Dish)
                .FirstOrDefaultAsync(c => c.ComplaintId == id);
        }

        // 重写：获取所有投诉
        public override async Task<List<Complaint>> GetAllAsync()
        {
            return await _context.Complaints
                .Include(c => c.Order)
                .ThenInclude(o => o.User)
                .Include(c => c.Complainant)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        // === 业务专用查询方法 ===

        // 根据订单ID查询投诉
        public async Task<IEnumerable<Complaint>> GetByOrderIdAsync(string orderId)
        {
            return await _context.Complaints
                .Include(c => c.Order)
                .ThenInclude(o => o.User)
                .Include(c => c.Complainant)
                .Where(c => c.OrderId == orderId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        // 根据用户ID查询投诉列表
        public async Task<IEnumerable<Complaint>> GetByUserIdAsync(string userId)
        {
            return await _context.Complaints
                .Include(c => c.Order)
                .Include(c => c.Complainant)
                .Where(c => c.Order.UserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        // 根据商家ID查询投诉列表
        public async Task<IEnumerable<Complaint>> GetByMerchantIdAsync(string merchantId)
        {
            return await _context.Complaints
                .Include(c => c.Order)
                .ThenInclude(o => o.User)
                .Include(c => c.Order)
                .ThenInclude(o => o.OrderDishes)
                .ThenInclude(od => od.Dish)
                .Include(c => c.Complainant)
                .Where(c => c.Order.OrderDishes.Any(od => od.Dish.MerchantId == merchantId))
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        // 根据处理管理员ID查询投诉
        public async Task<IEnumerable<Complaint>> GetByHandlerIdAsync(string handlerId)
        {
            return await _context.Complaints
                .Include(c => c.Order)
                .ThenInclude(o => o.User)
                .Include(c => c.Complainant)
                .Where(c => c.ComplainantId == handlerId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        // 根据投诉状态查询
        public async Task<IEnumerable<Complaint>> GetByStatusAsync(string status)
        {
            int statusCode = int.Parse(status);
            return await _context.Complaints
                .Include(c => c.Order)
                .ThenInclude(o => o.User)
                .Include(c => c.Complainant)
                .Where(c => c.CmplStatus == statusCode)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        // 根据投诉类型查询
        public async Task<IEnumerable<Complaint>> GetByTypeAsync(string type)
        {
            int roleCode = int.Parse(type);
            return await _context.Complaints
                .Include(c => c.Order)
                .ThenInclude(o => o.User)
                .Include(c => c.Complainant)
                .Where(c => c.CmplRole == roleCode)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        // 根据投诉时间范围查询
        public async Task<IEnumerable<Complaint>> GetByComplaintTimeRangeAsync(DateTime startTime, DateTime endTime)
        {
            return await _context.Complaints
                .Include(c => c.Order)
                .ThenInclude(o => o.User)
                .Include(c => c.Complainant)
                .Where(c => c.CreatedAt >= startTime && c.CreatedAt <= endTime)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        // 根据处理时间范围查询 - 由于实体没有处理时间，使用创建时间替代
        public async Task<IEnumerable<Complaint>> GetByHandleTimeRangeAsync(DateTime startTime, DateTime endTime)
        {
            return await _context.Complaints
                .Include(c => c.Order)
                .ThenInclude(o => o.User)
                .Include(c => c.Complainant)
                .Where(c => c.CreatedAt >= startTime && c.CreatedAt <= endTime)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        // 获取待处理的投诉
        public async Task<IEnumerable<Complaint>> GetPendingComplaintsAsync()
        {
            return await _context.Complaints
                .Include(c => c.Order)
                .ThenInclude(o => o.User)
                .Include(c => c.Complainant)
                .Where(c => c.CmplStatus == 1 || c.CmplStatus == 2) // 1: 待处理, 2: 处理中
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        // 获取已处理的投诉
        public async Task<IEnumerable<Complaint>> GetHandledComplaintsAsync()
        {
            return await _context.Complaints
                .Include(c => c.Order)
                .ThenInclude(o => o.User)
                .Include(c => c.Complainant)
                .Where(c => c.CmplStatus == 3 || c.CmplStatus == 4) // 3: 已解决, 4: 已关闭
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        // 根据关键字搜索投诉内容
        public async Task<IEnumerable<Complaint>> SearchByContentAsync(string keyword)
        {
            return await _context.Complaints
                .Include(c => c.Order)
                .ThenInclude(o => o.User)
                .Include(c => c.Complainant)
                .Where(c => c.CmplDescription != null && c.CmplDescription.Contains(keyword))
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        // 根据关键字搜索处理结果 - 由于实体没有处理结果字段，搜索描述
        public async Task<IEnumerable<Complaint>> SearchByHandleResultAsync(string keyword)
        {
            return await _context.Complaints
                .Include(c => c.Order)
                .ThenInclude(o => o.User)
                .Include(c => c.Complainant)
                .Where(c => c.CmplDescription != null && c.CmplDescription.Contains(keyword))
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        // 统计投诉状态分布
        public async Task<Dictionary<string, int>> GetStatusCountAsync()
        {
            var statusCounts = await _context.Complaints
                .GroupBy(c => c.CmplStatus)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            return statusCounts.ToDictionary(sc => sc.Status.ToString(), sc => sc.Count);
        }

        // 统计投诉类型分布
        public async Task<Dictionary<string, int>> GetTypeCountAsync()
        {
            var typeCounts = await _context.Complaints
                .GroupBy(c => c.CmplRole)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToListAsync();

            return typeCounts.ToDictionary(tc => tc.Type.ToString(), tc => tc.Count);
        }
    }
}
