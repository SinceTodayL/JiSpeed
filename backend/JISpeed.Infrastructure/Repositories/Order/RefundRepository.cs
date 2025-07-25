using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JISpeed.Core.Entities.Order;
using JISpeed.Core.Interfaces.IRepositories.Order;
using JISpeed.Infrastructure.Data;

namespace JISpeed.Infrastructure.Repositories.Order
{
    // 退款仓储实现 - 处理退款管理的数据访问操作
    public class RefundRepository : IRefundRepository
    {
        private readonly OracleDbContext _context;

        public RefundRepository(OracleDbContext context)
        {
            _context = context;
        }

        // 根据主键查询退款
        public async Task<Refund?> GetByIdAsync(string id)
        {
            return await _context.Refunds
                .Include(r => r.Order)
                .ThenInclude(o => o.User)
                .Include(r => r.Applicant)
                .FirstOrDefaultAsync(r => r.RefundId == id);
        }

        // 查询退款包含详细信息
        public async Task<Refund?> GetWithDetailsAsync(string id)
        {
            return await _context.Refunds
                .Include(r => r.Order)
                .ThenInclude(o => o.User)
                .Include(r => r.Order)
                .ThenInclude(o => o.OrderDishes)
                .ThenInclude(od => od.Dish)
                .Include(r => r.Applicant)
                .FirstOrDefaultAsync(r => r.RefundId == id);
        }

        // 获取所有退款
        public async Task<List<Refund>> GetAllAsync()
        {
            return await _context.Refunds
                .Include(r => r.Order)
                .ThenInclude(o => o.User)
                .Include(r => r.Applicant)
                .OrderByDescending(r => r.ApplyAt)
                .ToListAsync();
        }

        // 检查退款是否存在
        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Refunds.AnyAsync(r => r.RefundId == id);
        }

        // 创建新退款
        public async Task<Refund> CreateAsync(Refund entity)
        {
            var result = await _context.Refunds.AddAsync(entity);
            return result.Entity;
        }

        // 更新退款信息
        public async Task<Refund> UpdateAsync(Refund entity)
        {
            var result = _context.Refunds.Update(entity);
            await Task.CompletedTask;
            return result.Entity;
        }

        // 删除退款
        public async Task<bool> DeleteAsync(string id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Refunds.Remove(entity);
                return true;
            }
            return false;
        }

        // 保存更改
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // === 业务专用查询方法 ===

        // 根据订单ID查询退款记录
        public async Task<IEnumerable<Refund>> GetByOrderIdAsync(string orderId)
        {
            return await _context.Refunds
                .Include(r => r.Order)
                .ThenInclude(o => o.User)
                .Include(r => r.Applicant)
                .Where(r => r.OrderId == orderId)
                .OrderByDescending(r => r.ApplyAt)
                .ToListAsync();
        }

        // 根据用户ID查询退款记录
        public async Task<IEnumerable<Refund>> GetByUserIdAsync(string userId)
        {
            return await _context.Refunds
                .Include(r => r.Order)
                .Include(r => r.Applicant)
                .Where(r => r.ApplicationId == userId)
                .OrderByDescending(r => r.ApplyAt)
                .ToListAsync();
        }

        // 根据商家ID查询退款记录  
        public async Task<IEnumerable<Refund>> GetByMerchantIdAsync(string merchantId)
        {
            return await _context.Refunds
                .Include(r => r.Order)
                .ThenInclude(o => o.User)
                .Include(r => r.Applicant)
                .Where(r => r.Order.UserId == merchantId) // 注意：根据实际的Order实体属性调整
                .OrderByDescending(r => r.ApplyAt)
                .ToListAsync();
        }

        // 根据审核状态查询
        public async Task<IEnumerable<Refund>> GetByStatusAsync(int auditStatus)
        {
            return await _context.Refunds
                .Include(r => r.Order)
                .ThenInclude(o => o.User)
                .Include(r => r.Applicant)
                .Where(r => r.AuditStatus == auditStatus)
                .OrderByDescending(r => r.ApplyAt)
                .ToListAsync();
        }

        // 根据退款原因查询
        public async Task<IEnumerable<Refund>> GetByReasonAsync(string reason)
        {
            return await _context.Refunds
                .Include(r => r.Order)
                .ThenInclude(o => o.User)
                .Include(r => r.Applicant)
                .Where(r => r.Reason != null && r.Reason.Contains(reason))
                .OrderByDescending(r => r.ApplyAt)
                .ToListAsync();
        }

        // 根据申请时间范围查询
        public async Task<IEnumerable<Refund>> GetByApplyTimeRangeAsync(DateTime startTime, DateTime endTime)
        {
            return await _context.Refunds
                .Include(r => r.Order)
                .ThenInclude(o => o.User)
                .Include(r => r.Applicant)
                .Where(r => r.ApplyAt >= startTime && r.ApplyAt <= endTime)
                .OrderByDescending(r => r.ApplyAt)
                .ToListAsync();
        }

        // 根据处理时间范围查询
        public async Task<IEnumerable<Refund>> GetByFinishTimeRangeAsync(DateTime startTime, DateTime endTime)
        {
            return await _context.Refunds
                .Include(r => r.Order)
                .ThenInclude(o => o.User)
                .Include(r => r.Applicant)
                .Where(r => r.FinishAt.HasValue && r.FinishAt.Value >= startTime && r.FinishAt.Value <= endTime)
                .OrderByDescending(r => r.ApplyAt)
                .ToListAsync();
        }

        // 获取待处理的退款申请 (审核状态为0-未审核)
        public async Task<IEnumerable<Refund>> GetPendingRefundsAsync()
        {
            return await _context.Refunds
                .Include(r => r.Order)
                .ThenInclude(o => o.User)
                .Include(r => r.Applicant)
                .Where(r => r.AuditStatus == 0)
                .OrderBy(r => r.ApplyAt)
                .ToListAsync();
        }

        // 获取已处理的退款申请 (审核状态为1-通过或2-拒绝)
        public async Task<IEnumerable<Refund>> GetProcessedRefundsAsync()
        {
            return await _context.Refunds
                .Include(r => r.Order)
                .ThenInclude(o => o.User)
                .Include(r => r.Applicant)
                .Where(r => r.AuditStatus == 1 || r.AuditStatus == 2)
                .OrderByDescending(r => r.FinishAt)
                .ToListAsync();
        }

        // 根据退款金额范围查询
        public async Task<IEnumerable<Refund>> GetByAmountRangeAsync(decimal minAmount, decimal maxAmount)
        {
            return await _context.Refunds
                .Include(r => r.Order)
                .ThenInclude(o => o.User)
                .Include(r => r.Applicant)
                .Where(r => r.RefundAmount >= minAmount && r.RefundAmount <= maxAmount)
                .OrderByDescending(r => r.ApplyAt)
                .ToListAsync();
        }

        // 统计审核状态分布
        public async Task<Dictionary<int, int>> GetStatusCountAsync()
        {
            var statusCounts = await _context.Refunds
                .GroupBy(r => r.AuditStatus)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            return statusCounts.ToDictionary(sc => sc.Status, sc => sc.Count);
        }

        // 统计退款原因分布
        public async Task<Dictionary<string, int>> GetReasonCountAsync()
        {
            var reasonCounts = await _context.Refunds
                .Where(r => !string.IsNullOrEmpty(r.Reason))
                .GroupBy(r => r.Reason)
                .Select(g => new { Reason = g.Key, Count = g.Count() })
                .ToListAsync();

            return reasonCounts.Where(rc => rc.Reason != null)
                              .ToDictionary(rc => rc.Reason!, rc => rc.Count);
        }

        // 计算指定时间范围内的退款总金额 (仅已通过审核的)
        public async Task<decimal> GetTotalRefundAmountAsync(DateTime startTime, DateTime endTime)
        {
            var refunds = await _context.Refunds
                .Where(r => r.ApplyAt >= startTime && r.ApplyAt <= endTime && r.AuditStatus == 1)
                .ToListAsync();

            return refunds.Sum(r => r.RefundAmount);
        }

        // 计算商家的退款总金额 (仅已通过审核的)
        public async Task<decimal> GetTotalRefundAmountByMerchantIdAsync(string merchantId)
        {
            var refunds = await _context.Refunds
                .Where(r => r.Order.UserId == merchantId && r.AuditStatus == 1) // 根据实际Order实体属性调整
                .ToListAsync();

            return refunds.Sum(r => r.RefundAmount);
        }
    }
}
