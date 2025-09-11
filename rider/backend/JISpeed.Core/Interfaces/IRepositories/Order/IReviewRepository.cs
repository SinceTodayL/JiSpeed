using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JISpeed.Core.Entities.Order;

namespace JISpeed.Core.Interfaces.IRepositories.Order
{
    // 评价仓储接口 - 处理订单评价的数据访问操作
    public interface IReviewRepository : IBaseRepository<Review, string>
    {
        // === 业务专用查询方法 ===

        // 根据订单ID查询评价
        Task<Review?> GetByOrderIdAsync(string orderId);

        // 根据用户ID查询评价列表
        Task<IEnumerable<Review>> GetByUserIdAsync(string userId);


        /// 根据用户ID查询评价列表（分页）

        /// <param name="userId">用户ID</param>
        /// <param name="page">页码</param>
        /// <param name="size">每页大小</param>
        /// <returns>评价列表</returns>
        Task<List<Review>> GetByUserIdAsync(string userId, int page, int size);


        /// 根据用户ID获取评价数量

        /// <param name="userId">用户ID</param>
        /// <returns>评价数量</returns>
        Task<int> GetCountByUserIdAsync(string userId);

        // 根据商家ID查询评价列表
        Task<IEnumerable<Review>> GetByMerchantIdAsync(string merchantId);

        // 根据评分范围查询评价
        Task<IEnumerable<Review>> GetByRatingRangeAsync(int minRating, int maxRating);

        // 根据评价时间范围查询
        Task<IEnumerable<Review>> GetByReviewTimeRangeAsync(DateTime startTime, DateTime endTime);

        // 获取好评(4-5星)
        Task<IEnumerable<Review>> GetPositiveReviewsAsync();

        // 获取差评(1-2星)
        Task<IEnumerable<Review>> GetNegativeReviewsAsync();

        // 获取有评价内容的评价
        Task<IEnumerable<Review>> GetReviewsWithContentAsync();

        // 根据关键字搜索评价内容
        Task<IEnumerable<Review>> SearchByContentAsync(string keyword);

        // 计算商家平均评分
        Task<double> GetAverageRatingByMerchantIdAsync(string merchantId);

        // 统计商家各评分数量
        Task<Dictionary<int, int>> GetRatingCountByMerchantIdAsync(string merchantId);
    }
}
