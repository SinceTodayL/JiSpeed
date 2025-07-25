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
    // 评价仓储实现 - 处理订单评价的数据访问操作
    public class ReviewRepository : IReviewRepository
    {
        private readonly OracleDbContext _context;

        public ReviewRepository(OracleDbContext context)
        {
            _context = context;
        }

        // 根据主键查询评价
        public async Task<Review?> GetByIdAsync(string id)
        {
            return await _context.Reviews
                .Include(r => r.Order)
                .ThenInclude(o => o.User)
                .Include(r => r.User)
                .Include(r => r.DishReviews)
                .ThenInclude(dr => dr.Dish)
                .FirstOrDefaultAsync(r => r.ReviewId == id);
        }

        // 查询评价包含详细信息
        public async Task<Review?> GetWithDetailsAsync(string id)
        {
            return await _context.Reviews
                .Include(r => r.Order)
                .ThenInclude(o => o.User)
                .Include(r => r.User)
                .Include(r => r.DishReviews)
                .ThenInclude(dr => dr.Dish)
                .Include(r => r.Order)
                .ThenInclude(o => o.OrderDishes)
                .ThenInclude(od => od.Dish)
                .FirstOrDefaultAsync(r => r.ReviewId == id);
        }

        // 获取所有评价
        public async Task<List<Review>> GetAllAsync()
        {
            return await _context.Reviews
                .Include(r => r.Order)
                .ThenInclude(o => o.User)
                .Include(r => r.User)
                .OrderByDescending(r => r.ReviewAt)
                .ToListAsync();
        }

        // 检查评价是否存在
        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Reviews.AnyAsync(r => r.ReviewId == id);
        }

        // 创建新评价
        public async Task<Review> CreateAsync(Review entity)
        {
            await _context.Reviews.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        // 更新评价信息
        public async Task<Review> UpdateAsync(Review entity)
        {
            _context.Reviews.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        // 删除评价
        public async Task<bool> DeleteAsync(string id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Reviews.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // 保存更改
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Reviews.CountAsync();
        }

        public async Task<List<Review>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _context.Reviews
                .Include(r => r.Order)
                .ThenInclude(o => o.User)
                .Include(r => r.User)
                .OrderByDescending(r => r.ReviewAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // === 业务专用查询方法 ===

        // 根据订单ID查询评价 - 接口要求返回单个评价
        public async Task<Review?> GetByOrderIdAsync(string orderId)
        {
            return await _context.Reviews
                .Include(r => r.Order)
                .ThenInclude(o => o.User)
                .Include(r => r.User)
                .Include(r => r.DishReviews)
                .ThenInclude(dr => dr.Dish)
                .FirstOrDefaultAsync(r => r.OrderId == orderId);
        }

        // 根据用户ID查询评价列表
        public async Task<IEnumerable<Review>> GetByUserIdAsync(string userId)
        {
            return await _context.Reviews
                .Include(r => r.Order)
                .Include(r => r.User)
                .Include(r => r.DishReviews)
                .ThenInclude(dr => dr.Dish)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.ReviewAt)
                .ToListAsync();
        }

        // 根据商家ID查询评价列表 - 通过Order.OrderDishes.Dish.MerchantId
        public async Task<IEnumerable<Review>> GetByMerchantIdAsync(string merchantId)
        {
            return await _context.Reviews
                .Include(r => r.Order)
                .ThenInclude(o => o.User)
                .Include(r => r.Order)
                .ThenInclude(o => o.OrderDishes)
                .ThenInclude(od => od.Dish)
                .Include(r => r.User)
                .Include(r => r.DishReviews)
                .ThenInclude(dr => dr.Dish)
                .Where(r => r.Order.OrderDishes.Any(od => od.Dish.MerchantId == merchantId))
                .OrderByDescending(r => r.ReviewAt)
                .ToListAsync();
        }

        // 根据菜品ID查询评价列表
        public async Task<IEnumerable<Review>> GetByDishIdAsync(string dishId)
        {
            return await _context.Reviews
                .Include(r => r.Order)
                .ThenInclude(o => o.User)
                .Include(r => r.User)
                .Include(r => r.DishReviews)
                .ThenInclude(dr => dr.Dish)
                .Where(r => r.DishReviews.Any(dr => dr.DishId == dishId))
                .OrderByDescending(r => r.ReviewAt)
                .ToListAsync();
        }

        // 根据评分筛选评价
        public async Task<IEnumerable<Review>> GetByRatingAsync(int rating)
        {
            return await _context.Reviews
                .Include(r => r.Order)
                .ThenInclude(o => o.User)
                .Include(r => r.User)
                .Include(r => r.DishReviews)
                .ThenInclude(dr => dr.Dish)
                .Where(r => r.Rating == rating)
                .OrderByDescending(r => r.ReviewAt)
                .ToListAsync();
        }

        // 根据评分范围筛选评价
        public async Task<IEnumerable<Review>> GetByRatingRangeAsync(int minRating, int maxRating)
        {
            return await _context.Reviews
                .Include(r => r.Order)
                .ThenInclude(o => o.User)
                .Include(r => r.User)
                .Include(r => r.DishReviews)
                .ThenInclude(dr => dr.Dish)
                .Where(r => r.Rating >= minRating && r.Rating <= maxRating)
                .OrderByDescending(r => r.ReviewAt)
                .ToListAsync();
        }

        // 根据评价时间范围查询
        public async Task<IEnumerable<Review>> GetByReviewTimeRangeAsync(DateTime startTime, DateTime endTime)
        {
            return await _context.Reviews
                .Include(r => r.Order)
                .ThenInclude(o => o.User)
                .Include(r => r.User)
                .Include(r => r.DishReviews)
                .ThenInclude(dr => dr.Dish)
                .Where(r => r.ReviewAt >= startTime && r.ReviewAt <= endTime)
                .OrderByDescending(r => r.ReviewAt)
                .ToListAsync();
        }

        // 获取匿名评价
        public async Task<IEnumerable<Review>> GetAnonymousReviewsAsync()
        {
            return await _context.Reviews
                .Include(r => r.Order)
                .ThenInclude(o => o.User)
                .Include(r => r.User)
                .Include(r => r.DishReviews)
                .ThenInclude(dr => dr.Dish)
                .Where(r => r.IsAnonymous == 1)
                .OrderByDescending(r => r.ReviewAt)
                .ToListAsync();
        }

        // 获取非匿名评价
        public async Task<IEnumerable<Review>> GetNamedReviewsAsync()
        {
            return await _context.Reviews
                .Include(r => r.Order)
                .ThenInclude(o => o.User)
                .Include(r => r.User)
                .Include(r => r.DishReviews)
                .ThenInclude(dr => dr.Dish)
                .Where(r => r.IsAnonymous == 2)
                .OrderByDescending(r => r.ReviewAt)
                .ToListAsync();
        }

        // 根据关键字搜索评价内容
        public async Task<IEnumerable<Review>> SearchByContentAsync(string keyword)
        {
            return await _context.Reviews
                .Include(r => r.Order)
                .ThenInclude(o => o.User)
                .Include(r => r.User)
                .Include(r => r.DishReviews)
                .ThenInclude(dr => dr.Dish)
                .Where(r => r.Content != null && r.Content.Contains(keyword))
                .OrderByDescending(r => r.ReviewAt)
                .ToListAsync();
        }

        // 获取好评(4-5星) - 接口要求的方法
        public async Task<IEnumerable<Review>> GetPositiveReviewsAsync()
        {
            return await _context.Reviews
                .Include(r => r.Order)
                .ThenInclude(o => o.User)
                .Include(r => r.User)
                .Include(r => r.DishReviews)
                .ThenInclude(dr => dr.Dish)
                .Where(r => r.Rating >= 4)
                .OrderByDescending(r => r.ReviewAt)
                .ToListAsync();
        }

        // 获取差评(1-2星) - 接口要求的方法
        public async Task<IEnumerable<Review>> GetNegativeReviewsAsync()
        {
            return await _context.Reviews
                .Include(r => r.Order)
                .ThenInclude(o => o.User)
                .Include(r => r.User)
                .Include(r => r.DishReviews)
                .ThenInclude(dr => dr.Dish)
                .Where(r => r.Rating <= 2)
                .OrderByDescending(r => r.ReviewAt)
                .ToListAsync();
        }

        // 获取有评价内容的评价 - 接口要求的方法
        public async Task<IEnumerable<Review>> GetReviewsWithContentAsync()
        {
            return await _context.Reviews
                .Include(r => r.Order)
                .ThenInclude(o => o.User)
                .Include(r => r.User)
                .Include(r => r.DishReviews)
                .ThenInclude(dr => dr.Dish)
                .Where(r => !string.IsNullOrEmpty(r.Content))
                .OrderByDescending(r => r.ReviewAt)
                .ToListAsync();
        }

        // 计算商家平均评分 - 接口要求的方法名
        public async Task<double> GetAverageRatingByMerchantIdAsync(string merchantId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.Order)
                .ThenInclude(o => o.OrderDishes)
                .ThenInclude(od => od.Dish)
                .Where(r => r.Order.OrderDishes.Any(od => od.Dish.MerchantId == merchantId))
                .ToListAsync();

            return reviews.Any() ? reviews.Average(r => r.Rating) : 0.0;
        }

        // 统计商家各评分数量 - 接口要求的方法名
        public async Task<Dictionary<int, int>> GetRatingCountByMerchantIdAsync(string merchantId)
        {
            var ratingCounts = await _context.Reviews
                .Include(r => r.Order)
                .ThenInclude(o => o.OrderDishes)
                .ThenInclude(od => od.Dish)
                .Where(r => r.Order.OrderDishes.Any(od => od.Dish.MerchantId == merchantId))
                .GroupBy(r => r.Rating)
                .Select(g => new { Rating = g.Key, Count = g.Count() })
                .ToListAsync();

            return ratingCounts.ToDictionary(rc => rc.Rating, rc => rc.Count);
        }
    }
}
