using JISpeed.Core.Entities.Order;
using JISpeed.Core.Interfaces.IRepositories.Order;
using JISpeed.Infrastructure.Data;
using JISpeed.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories.Order
{
    public class OrderRepository : BaseRepository<JISpeed.Core.Entities.Order.Order, string>, IOrderRepository
    {
        public OrderRepository(OracleDbContext context) : base(context)
        {
        }

        // 重写GetWithDetailsAsync以包含关联数据
        // <param name="orderId">订单ID</param>
        // <returns>包含关联数据的订单实体，如果不存在则返回null</returns>
        public override async Task<JISpeed.Core.Entities.Order.Order?> GetWithDetailsAsync(string orderId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.Coupon)
                .Include(o => o.OrderDishes)
                    .ThenInclude(od => od.Dish)
                .Include(o => o.Payments)
                .Include(o => o.Reviews)
                .Include(o => o.AddressId)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        // 重写GetAllAsync以包含关联数据和排序
        // <returns>订单列表</returns>
        public override async Task<List<JISpeed.Core.Entities.Order.Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDishes)
                    .ThenInclude(od => od.Dish)
                .OrderByDescending(o => o.CreateAt)
                .ToListAsync();
        }

        // === 业务专用查询方法 ===

        // 根据用户ID获取订单列表
        // <param name="userId">用户ID</param>
        // <returns>订单列表</returns>
        public async Task<List<JISpeed.Core.Entities.Order.Order>> GetByUserIdAsync(string userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderDishes)
                    .ThenInclude(od => od.Dish)
                .OrderByDescending(o => o.CreateAt)
                .ToListAsync();
        }

        // 根据状态获取订单列表
        // <param name="status">状态</param>
        // <returns>订单列表</returns>
        public async Task<List<JISpeed.Core.Entities.Order.Order>> GetByStatusAsync(int status)
        {
            return await _context.Orders
                .Where(o => o.OrderStatus == status)
                .Include(o => o.User)
                .Include(o => o.OrderDishes)
                    .ThenInclude(od => od.Dish)
                .OrderByDescending(o => o.CreateAt)
                .ToListAsync();
        }

        // 根据用户ID和状态获取订单列表
        // <param name="userId">用户ID</param>
        // <param name="status">状态</param>
        // <returns>订单列表</returns>
        public async Task<List<JISpeed.Core.Entities.Order.Order>> GetByUserIdAndStatusAsync(string userId, int status)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId && o.OrderStatus == status)
                .Include(o => o.OrderDishes)
                    .ThenInclude(od => od.Dish)
                .OrderByDescending(o => o.CreateAt)
                .ToListAsync();
        }

        // 根据时间范围获取订单列表
        // <param name="startTime">开始时间</param>
        // <param name="endTime">结束时间</param>
        // <returns>订单列表</returns>
        public async Task<List<JISpeed.Core.Entities.Order.Order>> GetByTimeRangeAsync(DateTime startTime, DateTime endTime)
        {
            return await _context.Orders
                .Where(o => o.CreateAt >= startTime && o.CreateAt <= endTime)
                .Include(o => o.User)
                .Include(o => o.OrderDishes)
                    .ThenInclude(od => od.Dish)
                .OrderByDescending(o => o.CreateAt)
                .ToListAsync();
        }
    }
}
