using JISpeed.Core.Entities.Order;
using JISpeed.Core.Interfaces.IRepositories.Order;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories.Order
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly OracleDbContext _context;

        public PaymentRepository(OracleDbContext context)
        {
            _context = context;
        }

        // 根据支付ID获取支付信息
        // <param name="payId">支付ID</param>
        // <returns>支付实体，如果不存在则返回null</returns>
        public async Task<Payment?> GetByIdAsync(string payId)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p => p.PayId == payId);
        }

        // 根据支付ID获取支付详细信息（包含关联数据）
        // <param name="payId">支付ID</param>
        // <returns>包含关联数据的支付实体，如果不存在则返回null</returns>
        public async Task<Payment?> GetWithDetailsAsync(string payId)
        {
            return await _context.Payments
                .Include(p => p.Order)
                    .ThenInclude(o => o.User)
                .Include(p => p.Order)
                    .ThenInclude(o => o.OrderDishes)
                        .ThenInclude(od => od.Dish)
                .FirstOrDefaultAsync(p => p.PayId == payId);
        }

        // 根据订单ID获取支付列表
        // <param name="orderId">订单ID</param>
        // <returns>支付列表</returns>
        public async Task<List<Payment>> GetByOrderIdAsync(string orderId)
        {
            return await _context.Payments
                .Where(p => p.OrderId == orderId)
                .OrderByDescending(p => p.PayTime)
                .ToListAsync();
        }

        // 根据支付状态获取支付列表
        // <param name="payStatus">支付状态</param>
        // <returns>支付列表</returns>
        public async Task<List<Payment>> GetByPayStatusAsync(int payStatus)
        {
            return await _context.Payments
                .Where(p => p.PayStatus == payStatus)
                .Include(p => p.Order)
                    .ThenInclude(o => o.User)
                .OrderByDescending(p => p.PayTime)
                .ToListAsync();
        }

        // 根据支付渠道获取支付列表
        // <param name="channel">支付渠道</param>
        // <returns>支付列表</returns>
        public async Task<List<Payment>> GetByChannelAsync(string channel)
        {
            return await _context.Payments
                .Where(p => p.Channel == channel)
                .Include(p => p.Order)
                .OrderByDescending(p => p.PayTime)
                .ToListAsync();
        }

        // 根据时间范围获取支付列表
        // <param name="startTime">开始时间</param>
        // <param name="endTime">结束时间</param>
        // <returns>支付列表</returns>
        public async Task<List<Payment>> GetByTimeRangeAsync(DateTime startTime, DateTime endTime)
        {
            return await _context.Payments
                .Where(p => p.PayTime >= startTime && p.PayTime <= endTime)
                .Include(p => p.Order)
                    .ThenInclude(o => o.User)
                .OrderByDescending(p => p.PayTime)
                .ToListAsync();
        }

        // 获取所有支付列表
        // <returns>支付列表</returns>
        public async Task<List<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .Include(p => p.Order)
                    .ThenInclude(o => o.User)
                .OrderByDescending(p => p.PayTime)
                .ToListAsync();
        }

        // 检查支付是否存在
        // <param name="payId">支付ID</param>
        // <returns>支付是否存在</returns>
        public async Task<bool> ExistsAsync(string payId)
        {
            return await _context.Payments
                .AnyAsync(p => p.PayId == payId);
        }

        // 创建新支付
        // <param name="payment">支付实体</param>
        // <returns>创建的支付实体</returns>
        public async Task<Payment> CreateAsync(Payment payment)
        {
            var entity = await _context.Payments.AddAsync(payment);
            return entity.Entity;
        }

        // 更新支付信息
        // <param name="payment">支付实体</param>
        // <returns>更新的支付实体</returns>
        public async Task<Payment> UpdateAsync(Payment payment)
        {
            var entity = _context.Payments.Update(payment);
            await Task.CompletedTask; // 解决异步警告
            return entity.Entity;
        }

        // 删除支付
        // <param name="payId">支付ID</param>
        // <returns>是否删除成功</returns>
        public async Task<bool> DeleteAsync(string payId)
        {
            var payment = await GetByIdAsync(payId);
            if (payment == null)
                return false;

            _context.Payments.Remove(payment);
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
