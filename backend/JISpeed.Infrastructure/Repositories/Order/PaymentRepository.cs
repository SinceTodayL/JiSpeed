using JISpeed.Core.Entities.Order;
using JISpeed.Core.Interfaces.IRepositories.Order;
using JISpeed.Infrastructure.Data;
using JISpeed.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories.Order
{
    public class PaymentRepository : BaseRepository<Payment, string>,IPaymentRepository
    {
        public PaymentRepository(OracleDbContext context) : base(context)
        {
        }

        // 重写GetWithDetailsAsync以包含关联数据
        // <param name="payId">支付ID</param>
        // <returns>包含关联数据的支付实体，如果不存在则返回null</returns>
        public override async Task<Payment?> GetWithDetailsAsync(string payId)
        {
            return await _context.Payments
                .Include(p => p.Order)
                    .ThenInclude(o => o.User)
                .Include(p => p.Order)
                    .ThenInclude(o => o.OrderDishes)
                        .ThenInclude(od => od.Dish)
                .FirstOrDefaultAsync(p => p.PayId == payId);
        }

        // 重写GetAllAsync以包含关联数据和排序
        // <returns>支付列表</returns>
        public override async Task<List<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .Include(p => p.Order)
                    .ThenInclude(o => o.User)
                .OrderByDescending(p => p.PayTime)
                .ToListAsync();
        }

        // === 业务专用查询方法 ===

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
    }
}
