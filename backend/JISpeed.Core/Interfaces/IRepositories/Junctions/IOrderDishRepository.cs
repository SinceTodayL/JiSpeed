using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JISpeed.Core.Entities.Junctions;

namespace JISpeed.Core.Interfaces.IRepositories.Junctions
{
    // 订单菜品联结仓储接口 - 处理订单菜品关联的数据访问操作
    // OrderDish实体使用复合主键(OrderId, DishId)
    public interface IOrderDishRepository
    {
        // === 基础CRUD操作 ===

        // 根据复合主键获取订单菜品关联
        Task<OrderDish?> GetByCompositeKeyAsync(string orderId, string dishId);

        // 获取所有订单菜品关联
        Task<List<OrderDish>> GetAllAsync();

        // 创建订单菜品关联
        Task<OrderDish> CreateAsync(OrderDish entity);

        // 删除订单菜品关联
        Task<bool> DeleteAsync(string orderId, string dishId);

        // 检查订单菜品关联是否存在
        Task<bool> ExistsAsync(string orderId, string dishId);

        // 获取订单菜品关联总数
        Task<int> CountAsync();

        // 分页获取订单菜品关联
        Task<List<OrderDish>> GetPagedAsync(int pageNumber, int pageSize);

        // 保存更改
        Task<int> SaveChangesAsync();

        // === 业务专用查询方法 ===

        // 根据订单ID查询所有相关菜品
        Task<IEnumerable<OrderDish>> GetByOrderIdAsync(string orderId);

        // 根据菜品ID查询所有相关订单
        Task<IEnumerable<OrderDish>> GetByDishIdAsync(string dishId);

        // 批量根据订单ID查询相关菜品
        Task<IEnumerable<OrderDish>> GetByOrderIdsAsync(IEnumerable<string> orderIds);

        // 批量根据菜品ID查询相关订单
        Task<IEnumerable<OrderDish>> GetByDishIdsAsync(IEnumerable<string> dishIds);

        // 获取最热销菜品统计
        Task<Dictionary<string, int>> GetMostOrderedDishesAsync(int topCount = 10);

        // 获取最热销菜品ID列表
        Task<IEnumerable<string>> GetTopOrderedDishIdsAsync(int topCount = 10);

        // 统计每个菜品的订单数量
        Task<Dictionary<string, int>> GetOrderCountByDishAsync();

        // 统计每个订单的菜品数量
        Task<Dictionary<string, int>> GetDishCountByOrderAsync();

        // 检查订单是否包含菜品
        Task<bool> OrderHasDishesAsync(string orderId);

        // 检查菜品是否被订购过
        Task<bool> DishHasOrdersAsync(string dishId);

        // 批量创建订单菜品关联
        Task<List<OrderDish>> CreateBatchAsync(IEnumerable<OrderDish> entities);

        // 批量删除订单菜品关联
        Task<bool> DeleteBatchAsync(IEnumerable<(string OrderId, string DishId)> keys);

        // 根据订单ID删除所有相关菜品关联
        Task<bool> DeleteAllByOrderIdAsync(string orderId);

        // 根据菜品ID删除所有相关订单关联
        Task<bool> DeleteAllByDishIdAsync(string dishId);

        // 获取订单中菜品的详细信息
        Task<IEnumerable<OrderDish>> GetOrderDishesWithDetailsAsync(string orderId);

        // 获取菜品在所有订单中的详细信息
        Task<IEnumerable<OrderDish>> GetDishOrdersWithDetailsAsync(string dishId);

        // 统计特定时间范围内菜品的销量
        Task<Dictionary<string, int>> GetDishSalesInPeriodAsync(DateTime startDate, DateTime endDate);

        // 获取共同菜品的订单（两个或多个订单包含相同菜品）
        Task<IEnumerable<OrderDish>> GetOrdersWithCommonDishesAsync(string dishId);
    }
}
