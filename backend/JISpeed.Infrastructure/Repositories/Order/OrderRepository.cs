using JISpeed.Core.Entities.Order;
using JISpeed.Core.Interfaces.IRepositories.Order;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories.Order
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OracleDbContext _context;

        public OrderRepository(OracleDbContext context)
        {
            _context = context;
        }

        // 根据订单ID获取订单信息
        // <param name="orderId">订单ID</param>
        // <returns>订单实体，如果不存在则返回null</returns>
        public async Task<JISpeed.Core.Entities.Order.Order?> GetByIdAsync(string orderId)
        {
            return await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        // 根据订单ID获取订单详细信息（包含关联数据）
        // <param name="orderId">订单ID</param>
        // <returns>包含关联数据的订单实体，如果不存在则返回null</returns>
        public async Task<JISpeed.Core.Entities.Order.Order?> GetWithDetailsAsync(string orderId)
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

        // 获取所有订单列表
        // <returns>订单列表</returns>
        public async Task<List<JISpeed.Core.Entities.Order.Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDishes)
                    .ThenInclude(od => od.Dish)
                .OrderByDescending(o => o.CreateAt)
                .ToListAsync();
        }

        // 检查订单是否存在
        // <param name="orderId">订单ID</param>
        // <returns>订单是否存在</returns>
        public async Task<bool> ExistsAsync(string orderId)
        {
            return await _context.Orders
                .AnyAsync(o => o.OrderId == orderId);
        }

        // 创建新订单
        // <param name="order">订单实体</param>
        // <returns>创建的订单实体</returns>
        public async Task<JISpeed.Core.Entities.Order.Order> CreateAsync(JISpeed.Core.Entities.Order.Order order)
        {
            var entity = await _context.Orders.AddAsync(order);
            return entity.Entity;
        }

        // 更新订单信息
        // <param name="order">订单实体</param>
        // <returns>更新的订单实体</returns>
        public async Task<JISpeed.Core.Entities.Order.Order> UpdateAsync(JISpeed.Core.Entities.Order.Order order)
        {
            var entity = _context.Orders.Update(order);
            await Task.CompletedTask; // 解决异步警告
            return entity.Entity;
        }

        // 删除订单
        // <param name="orderId">订单ID</param>
        // <returns>是否删除成功</returns>
        public async Task<bool> DeleteAsync(string orderId)
        {
            var order = await GetByIdAsync(orderId);
            if (order == null)
                return false;

            _context.Orders.Remove(order);
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
