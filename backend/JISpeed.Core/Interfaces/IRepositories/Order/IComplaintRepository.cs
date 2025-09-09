using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JISpeed.Core.Entities.Order;

namespace JISpeed.Core.Interfaces.IRepositories.Order
{
    // 投诉仓储接口 - 处理投诉管理的数据访问操作
    public interface IComplaintRepository : IBaseRepository<Complaint, string>
    {

        Task<List<Complaint>> GetAllAsync(int? size, int? page);

        // === 业务专用查询方法 ===

        // 根据订单ID查询投诉
        Task<IEnumerable<Complaint>> GetByOrderIdAsync(string orderId);

        // 根据用户ID查询投诉列表
        Task<List<Complaint>> GetByUserIdAndStatusAsync(
            string userId,
            int? status,
            int? size, int? page);
        Task<List<Complaint>> GetByUserIdAsync(string userId);

        /// <summary>
        /// 根据用户ID查询投诉列表（分页）
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="page">页码</param>
        /// <param name="size">每页大小</param>
        /// <returns>投诉列表</returns>
        Task<List<Complaint>> GetByUserIdAsync(string userId, int page, int size);

        /// <summary>
        /// 根据用户ID获取投诉数量
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>投诉数量</returns>
        Task<int> GetCountByUserIdAsync(string userId);

        // 根据商家ID查询投诉列表
        Task<List<Complaint>> GetByMerchantIdAndStatusAsync(
            string merchantId,
            int? status,
            int? size, int? page);
        Task<List<Complaint>> GetByMerchantIdAsync(string merchantId);
        Task<List<Complaint>> GetAllByFilterAsync(
            int? status,
            int? size, int? page);


        // 根据投诉状态查询
        Task<IEnumerable<Complaint>> GetByStatusAsync(string status);

        // 根据投诉类型查询
        Task<IEnumerable<Complaint>> GetByTypeAsync(string type);

        // 根据投诉时间范围查询
        Task<IEnumerable<Complaint>> GetByComplaintTimeRangeAsync(DateTime startTime, DateTime endTime);

        // 根据处理时间范围查询
        Task<IEnumerable<Complaint>> GetByHandleTimeRangeAsync(DateTime startTime, DateTime endTime);

        // 获取待处理的投诉
        Task<IEnumerable<Complaint>> GetPendingComplaintsAsync();

        // 获取已处理的投诉
        Task<IEnumerable<Complaint>> GetHandledComplaintsAsync();

        // 根据关键字搜索投诉内容
        Task<IEnumerable<Complaint>> SearchByContentAsync(string keyword);

        // 根据关键字搜索处理结果
        Task<IEnumerable<Complaint>> SearchByHandleResultAsync(string keyword);

        // 统计投诉状态分布
        Task<Dictionary<string, int>> GetStatusCountAsync();

        // 统计投诉类型分布
        Task<Dictionary<string, int>> GetTypeCountAsync();
    }
}
