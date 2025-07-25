using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JISpeed.Core.Entities.Junctions;
using JISpeed.Core.Interfaces.IRepositories.Junctions;
using JISpeed.Infrastructure.Data;

namespace JISpeed.Infrastructure.Repositories.Junctions
{
    // 订单菜品联结仓储实现 - 处理订单菜品关联的数据访问操作
    public class OrderDishRepository : IOrderDishRepository
    {
        private readonly OracleDbContext _context;

        public OrderDishRepository(OracleDbContext context)
        {
            _context = context;
        }

        // === 基础CRUD操作 ===

        // 根据复合主键获取订单菜品关联
        public async Task<OrderDish?> GetByCompositeKeyAsync(string orderId, string dishId)
        {
            return await _context.OrderDishes
                .Include(od => od.Order)
                .Include(od => od.Dish)
                .FirstOrDefaultAsync(od => od.OrderId == orderId && od.DishId == dishId);
        }

        // 获取所有订单菜品关联
        public async Task<List<OrderDish>> GetAllAsync()
        {
            return await _context.OrderDishes
                .Include(od => od.Order)
                .Include(od => od.Dish)
                .ToListAsync();
        }

        // 创建订单菜品关联
        public async Task<OrderDish> CreateAsync(OrderDish entity)
        {
            await _context.OrderDishes.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        // 删除订单菜品关联
        public async Task<bool> DeleteAsync(string orderId, string dishId)
        {
            var orderDish = await _context.OrderDishes
                .FirstOrDefaultAsync(od => od.OrderId == orderId && od.DishId == dishId);

            if (orderDish != null)
            {
                _context.OrderDishes.Remove(orderDish);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // 检查订单菜品关联是否存在
        public async Task<bool> ExistsAsync(string orderId, string dishId)
        {
            return await _context.OrderDishes
                .AnyAsync(od => od.OrderId == orderId && od.DishId == dishId);
        }

        // 获取订单菜品关联总数
        public async Task<int> CountAsync()
        {
            return await _context.OrderDishes.CountAsync();
        }

        // 分页获取订单菜品关联
        public async Task<List<OrderDish>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _context.OrderDishes
                .Include(od => od.Order)
                .Include(od => od.Dish)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // 保存更改
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // === 业务专用查询方法 ===

        // 根据订单ID查询所有相关菜品
        public async Task<IEnumerable<OrderDish>> GetByOrderIdAsync(string orderId)
        {
            return await _context.OrderDishes
                .Include(od => od.Order)
                .Include(od => od.Dish)
                .Where(od => od.OrderId == orderId)
                .ToListAsync();
        }

        // 根据菜品ID查询所有相关订单
        public async Task<IEnumerable<OrderDish>> GetByDishIdAsync(string dishId)
        {
            return await _context.OrderDishes
                .Include(od => od.Order)
                .Include(od => od.Dish)
                .Where(od => od.DishId == dishId)
                .ToListAsync();
        }

        // 批量根据订单ID查询相关菜品
        public async Task<IEnumerable<OrderDish>> GetByOrderIdsAsync(IEnumerable<string> orderIds)
        {
            return await _context.OrderDishes
                .Include(od => od.Order)
                .Include(od => od.Dish)
                .Where(od => orderIds.Contains(od.OrderId))
                .ToListAsync();
        }

        // 批量根据菜品ID查询相关订单
        public async Task<IEnumerable<OrderDish>> GetByDishIdsAsync(IEnumerable<string> dishIds)
        {
            return await _context.OrderDishes
                .Include(od => od.Order)
                .Include(od => od.Dish)
                .Where(od => dishIds.Contains(od.DishId))
                .ToListAsync();
        }

        // 获取最热销菜品统计
        public async Task<Dictionary<string, int>> GetMostOrderedDishesAsync(int topCount = 10)
        {
            return await _context.OrderDishes
                .GroupBy(od => od.DishId)
                .OrderByDescending(g => g.Count())
                .Take(topCount)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        // 获取最热销菜品ID列表
        public async Task<IEnumerable<string>> GetTopOrderedDishIdsAsync(int topCount = 10)
        {
            return await _context.OrderDishes
                .GroupBy(od => od.DishId)
                .OrderByDescending(g => g.Count())
                .Take(topCount)
                .Select(g => g.Key)
                .ToListAsync();
        }

        // 统计每个菜品的订单数量
        public async Task<Dictionary<string, int>> GetOrderCountByDishAsync()
        {
            return await _context.OrderDishes
                .GroupBy(od => od.DishId)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        // 统计每个订单的菜品数量
        public async Task<Dictionary<string, int>> GetDishCountByOrderAsync()
        {
            return await _context.OrderDishes
                .GroupBy(od => od.OrderId)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        // 检查订单是否包含菜品
        public async Task<bool> OrderHasDishesAsync(string orderId)
        {
            return await _context.OrderDishes.AnyAsync(od => od.OrderId == orderId);
        }

        // 检查菜品是否被订购过
        public async Task<bool> DishHasOrdersAsync(string dishId)
        {
            return await _context.OrderDishes.AnyAsync(od => od.DishId == dishId);
        }

        // 批量创建订单菜品关联
        public async Task<List<OrderDish>> CreateBatchAsync(IEnumerable<OrderDish> entities)
        {
            var entitiesList = entities.ToList();
            await _context.OrderDishes.AddRangeAsync(entitiesList);
            await _context.SaveChangesAsync();
            return entitiesList;
        }

        // 批量删除订单菜品关联
        public async Task<bool> DeleteBatchAsync(IEnumerable<(string OrderId, string DishId)> keys)
        {
            var keysList = keys.ToList();
            var orderDishes = new List<OrderDish>();

            foreach (var (orderId, dishId) in keysList)
            {
                var orderDish = await _context.OrderDishes
                    .FirstOrDefaultAsync(od => od.OrderId == orderId && od.DishId == dishId);
                if (orderDish != null)
                {
                    orderDishes.Add(orderDish);
                }
            }

            if (orderDishes.Any())
            {
                _context.OrderDishes.RemoveRange(orderDishes);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // 根据订单ID删除所有相关菜品关联
        public async Task<bool> DeleteAllByOrderIdAsync(string orderId)
        {
            var orderDishes = await _context.OrderDishes
                .Where(od => od.OrderId == orderId)
                .ToListAsync();

            if (orderDishes.Any())
            {
                _context.OrderDishes.RemoveRange(orderDishes);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // 根据菜品ID删除所有相关订单关联
        public async Task<bool> DeleteAllByDishIdAsync(string dishId)
        {
            var orderDishes = await _context.OrderDishes
                .Where(od => od.DishId == dishId)
                .ToListAsync();

            if (orderDishes.Any())
            {
                _context.OrderDishes.RemoveRange(orderDishes);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // 获取订单中菜品的详细信息
        public async Task<IEnumerable<OrderDish>> GetOrderDishesWithDetailsAsync(string orderId)
        {
            return await _context.OrderDishes
                .Include(od => od.Order)
                .Include(od => od.Dish)
                    .ThenInclude(d => d.Category)
                .Include(od => od.Dish)
                    .ThenInclude(d => d.Merchant)
                .Where(od => od.OrderId == orderId)
                .ToListAsync();
        }

        // 获取菜品在所有订单中的详细信息
        public async Task<IEnumerable<OrderDish>> GetDishOrdersWithDetailsAsync(string dishId)
        {
            return await _context.OrderDishes
                .Include(od => od.Order)
                    .ThenInclude(o => o.User)
                .Include(od => od.Order)
                .Include(od => od.Dish)
                .Where(od => od.DishId == dishId)
                .OrderByDescending(od => od.Order.CreateAt)
                .ToListAsync();
        }

        // 统计特定时间范围内菜品的销量
        public async Task<Dictionary<string, int>> GetDishSalesInPeriodAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.OrderDishes
                .Include(od => od.Order)
                .Where(od => od.Order.CreateAt >= startDate && od.Order.CreateAt <= endDate)
                .GroupBy(od => od.DishId)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        // 获取共同菜品的订单（两个或多个订单包含相同菜品）
        public async Task<IEnumerable<OrderDish>> GetOrdersWithCommonDishesAsync(string dishId)
        {
            return await _context.OrderDishes
                .Include(od => od.Order)
                .Include(od => od.Dish)
                .Where(od => od.DishId == dishId)
                .OrderByDescending(od => od.Order.CreateAt)
                .ToListAsync();
        }
    }
}
