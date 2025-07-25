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
    // 菜品评论联结仓储实现 - 处理菜品评论关联的数据访问操作
    public class DishReviewRepository : IDishReviewRepository
    {
        private readonly OracleDbContext _context;

        public DishReviewRepository(OracleDbContext context)
        {
            _context = context;
        }

        // === 基础CRUD操作 ===

        // 根据复合主键获取菜品评论关联
        public async Task<DishReview?> GetByCompositeKeyAsync(string dishId, string reviewId)
        {
            return await _context.DishReviews
                .Include(dr => dr.Dish)
                .Include(dr => dr.Review)
                .FirstOrDefaultAsync(dr => dr.DishId == dishId && dr.ReviewId == reviewId);
        }

        // 获取所有菜品评论关联
        public async Task<List<DishReview>> GetAllAsync()
        {
            return await _context.DishReviews
                .Include(dr => dr.Dish)
                .Include(dr => dr.Review)
                .ToListAsync();
        }

        // 创建菜品评论关联
        public async Task<DishReview> CreateAsync(DishReview entity)
        {
            await _context.DishReviews.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        // 删除菜品评论关联
        public async Task<bool> DeleteAsync(string dishId, string reviewId)
        {
            var dishReview = await _context.DishReviews
                .FirstOrDefaultAsync(dr => dr.DishId == dishId && dr.ReviewId == reviewId);

            if (dishReview != null)
            {
                _context.DishReviews.Remove(dishReview);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // 检查菜品评论关联是否存在
        public async Task<bool> ExistsAsync(string dishId, string reviewId)
        {
            return await _context.DishReviews
                .AnyAsync(dr => dr.DishId == dishId && dr.ReviewId == reviewId);
        }

        // 获取菜品评论关联总数
        public async Task<int> CountAsync()
        {
            return await _context.DishReviews.CountAsync();
        }

        // 分页获取菜品评论关联
        public async Task<List<DishReview>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _context.DishReviews
                .Include(dr => dr.Dish)
                .Include(dr => dr.Review)
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

        // 根据菜品ID查询所有相关评论
        public async Task<IEnumerable<DishReview>> GetByDishIdAsync(string dishId)
        {
            return await _context.DishReviews
                .Include(dr => dr.Dish)
                .Include(dr => dr.Review)
                .Where(dr => dr.DishId == dishId)
                .ToListAsync();
        }

        // 根据评论ID查询所有相关菜品
        public async Task<IEnumerable<DishReview>> GetByReviewIdAsync(string reviewId)
        {
            return await _context.DishReviews
                .Include(dr => dr.Dish)
                .Include(dr => dr.Review)
                .Where(dr => dr.ReviewId == reviewId)
                .ToListAsync();
        }

        // 批量根据菜品ID查询相关评论
        public async Task<IEnumerable<DishReview>> GetByDishIdsAsync(IEnumerable<string> dishIds)
        {
            return await _context.DishReviews
                .Include(dr => dr.Dish)
                .Include(dr => dr.Review)
                .Where(dr => dishIds.Contains(dr.DishId))
                .ToListAsync();
        }

        // 批量根据评论ID查询相关菜品
        public async Task<IEnumerable<DishReview>> GetByReviewIdsAsync(IEnumerable<string> reviewIds)
        {
            return await _context.DishReviews
                .Include(dr => dr.Dish)
                .Include(dr => dr.Review)
                .Where(dr => reviewIds.Contains(dr.ReviewId))
                .ToListAsync();
        }

        // 获取最受评论的菜品统计
        public async Task<Dictionary<string, int>> GetMostReviewedDishesAsync(int topCount = 10)
        {
            return await _context.DishReviews
                .GroupBy(dr => dr.DishId)
                .OrderByDescending(g => g.Count())
                .Take(topCount)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        // 获取评论最多的菜品ID列表
        public async Task<IEnumerable<string>> GetTopReviewedDishIdsAsync(int topCount = 10)
        {
            return await _context.DishReviews
                .GroupBy(dr => dr.DishId)
                .OrderByDescending(g => g.Count())
                .Take(topCount)
                .Select(g => g.Key)
                .ToListAsync();
        }

        // 统计每个菜品的评论数量
        public async Task<Dictionary<string, int>> GetReviewCountByDishAsync()
        {
            return await _context.DishReviews
                .GroupBy(dr => dr.DishId)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        // 检查菜品是否有评论
        public async Task<bool> DishHasReviewsAsync(string dishId)
        {
            return await _context.DishReviews.AnyAsync(dr => dr.DishId == dishId);
        }

        // 检查评论是否关联了菜品
        public async Task<bool> ReviewHasDishesAsync(string reviewId)
        {
            return await _context.DishReviews.AnyAsync(dr => dr.ReviewId == reviewId);
        }

        // 批量创建菜品评论关联
        public async Task<List<DishReview>> CreateBatchAsync(IEnumerable<DishReview> entities)
        {
            var entitiesList = entities.ToList();
            await _context.DishReviews.AddRangeAsync(entitiesList);
            await _context.SaveChangesAsync();
            return entitiesList;
        }

        // 批量删除菜品评论关联
        public async Task<bool> DeleteBatchAsync(IEnumerable<(string DishId, string ReviewId)> keys)
        {
            var keysList = keys.ToList();
            var dishReviews = new List<DishReview>();

            foreach (var (dishId, reviewId) in keysList)
            {
                var dishReview = await _context.DishReviews
                    .FirstOrDefaultAsync(dr => dr.DishId == dishId && dr.ReviewId == reviewId);
                if (dishReview != null)
                {
                    dishReviews.Add(dishReview);
                }
            }

            if (dishReviews.Any())
            {
                _context.DishReviews.RemoveRange(dishReviews);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // 根据菜品ID删除所有相关评论关联
        public async Task<bool> DeleteAllByDishIdAsync(string dishId)
        {
            var dishReviews = await _context.DishReviews
                .Where(dr => dr.DishId == dishId)
                .ToListAsync();

            if (dishReviews.Any())
            {
                _context.DishReviews.RemoveRange(dishReviews);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // 根据评论ID删除所有相关菜品关联
        public async Task<bool> DeleteAllByReviewIdAsync(string reviewId)
        {
            var dishReviews = await _context.DishReviews
                .Where(dr => dr.ReviewId == reviewId)
                .ToListAsync();

            if (dishReviews.Any())
            {
                _context.DishReviews.RemoveRange(dishReviews);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
