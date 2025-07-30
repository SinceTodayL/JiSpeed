using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JISpeed.Core.Entities.Order;

namespace JISpeed.Core.Interfaces.IRepositories.Order
{
    // 退款仓储接口 - 处理退款管理的数据访问操作
    public interface IRefundRepository : IBaseRepository<Refund, string>
    {
        // === 业务专用查询方法 ===

        // 根据订单ID查询退款记录
        Task<IEnumerable<Refund>> GetByOrderIdAsync(string orderId);

        // 根据用户ID查询退款记录
        Task<IEnumerable<Refund>> GetByUserIdAsync(string userId);

        // 根据商家ID查询退款记录
        Task<IEnumerable<Refund>> GetByMerchantIdAsync(string merchantId);

        // 根据处理管理员ID查询退款记录 (Refund实体中没有HandlerId，移除)

        // 根据审核状态查询
        Task<IEnumerable<Refund>> GetByStatusAsync(int auditStatus);

        // 根据退款原因查询
        Task<IEnumerable<Refund>> GetByReasonAsync(string reason);

        // 根据申请时间范围查询
        Task<IEnumerable<Refund>> GetByApplyTimeRangeAsync(DateTime startTime, DateTime endTime);

        // 根据处理时间范围查询
        Task<IEnumerable<Refund>> GetByFinishTimeRangeAsync(DateTime startTime, DateTime endTime);

        // 获取待处理的退款申请
        Task<IEnumerable<Refund>> GetPendingRefundsAsync();

        // 获取已处理的退款申请
        Task<IEnumerable<Refund>> GetProcessedRefundsAsync();

        // 根据退款金额范围查询
        Task<IEnumerable<Refund>> GetByAmountRangeAsync(decimal minAmount, decimal maxAmount);

        // 统计审核状态分布
        Task<Dictionary<int, int>> GetStatusCountAsync();

        // 统计退款原因分布
        Task<Dictionary<string, int>> GetReasonCountAsync();

        // 计算指定时间范围内的退款总金额
        Task<decimal> GetTotalRefundAmountAsync(DateTime startTime, DateTime endTime);

        // 计算商家的退款总金额
        Task<decimal> GetTotalRefundAmountByMerchantIdAsync(string merchantId);
    }
}
