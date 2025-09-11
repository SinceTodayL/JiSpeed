using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JISpeed.Core.Entities.Junctions;

namespace JISpeed.Core.Interfaces.IRepositories.Junctions
{
    // 菜品评论联结仓储接口 - 处理菜品评论关联的数据访问操作
    // DishReview实体使用复合主键(DishId, ReviewId)
    public interface IDishReviewRepository
    {
        // === 基础CRUD操作 ===

        // 根据复合主键获取菜品评论关联
        Task<DishReview?> GetByCompositeKeyAsync(string dishId, string reviewId);

        // 获取所有菜品评论关联
        Task<List<DishReview>> GetAllAsync();

        // 创建菜品评论关联
        Task<DishReview> CreateAsync(DishReview entity);

        // 删除菜品评论关联
        Task<bool> DeleteAsync(string dishId, string reviewId);

        // 检查菜品评论关联是否存在
        Task<bool> ExistsAsync(string dishId, string reviewId);

        // 获取菜品评论关联总数
        Task<int> CountAsync();

        // 分页获取菜品评论关联
        Task<List<DishReview>> GetPagedAsync(int pageNumber, int pageSize);

        // 保存更改
        Task<int> SaveChangesAsync();

        // === 业务专用查询方法 ===

        // 根据菜品ID查询所有相关评论
        Task<IEnumerable<DishReview>> GetByDishIdAsync(string dishId);

        // 根据评论ID查询所有相关菜品
        Task<IEnumerable<DishReview>> GetByReviewIdAsync(string reviewId);

        // 批量根据菜品ID查询相关评论
        Task<IEnumerable<DishReview>> GetByDishIdsAsync(IEnumerable<string> dishIds);

        // 批量根据评论ID查询相关菜品
        Task<IEnumerable<DishReview>> GetByReviewIdsAsync(IEnumerable<string> reviewIds);

        // 获取最受评论的菜品统计
        Task<Dictionary<string, int>> GetMostReviewedDishesAsync(int topCount = 10);

        // 获取评论最多的菜品ID列表
        Task<IEnumerable<string>> GetTopReviewedDishIdsAsync(int topCount = 10);

        // 统计每个菜品的评论数量
        Task<Dictionary<string, int>> GetReviewCountByDishAsync();

        // 检查菜品是否有评论
        Task<bool> DishHasReviewsAsync(string dishId);

        // 检查评论是否关联了菜品
        Task<bool> ReviewHasDishesAsync(string reviewId);

        // 批量创建菜品评论关联
        Task<List<DishReview>> CreateBatchAsync(IEnumerable<DishReview> entities);

        // 批量删除菜品评论关联
        Task<bool> DeleteBatchAsync(IEnumerable<(string DishId, string ReviewId)> keys);

        // 根据菜品ID删除所有相关评论关联
        Task<bool> DeleteAllByDishIdAsync(string dishId);

        // 根据评论ID删除所有相关菜品关联
        Task<bool> DeleteAllByReviewIdAsync(string reviewId);
    }
}
