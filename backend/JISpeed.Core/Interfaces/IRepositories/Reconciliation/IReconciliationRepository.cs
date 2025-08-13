using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JISpeed.Core.Entities.Reconciliation;
using ReconciliationEntity = JISpeed.Core.Entities.Reconciliation.Reconciliation;

namespace JISpeed.Core.Interfaces.IRepositories.Reconciliation
{
    // 对账仓储接口 - 处理对账异常的数据访问操作
    public interface IReconciliationRepository : IBaseRepository<ReconciliationEntity, string>
    {

        Task<List<ReconciliationEntity>> GetAllAsync(
            int? size, int? page);
        // === 业务专用查询方法 ===
        // 根据账期范围查询对账记录
        Task<IEnumerable<ReconciliationEntity>> GetByPeriodRangeAsync(DateTime periodStart, DateTime periodEnd);

        // 根据发现时间范围查询对账记录
        Task<IEnumerable<ReconciliationEntity>> GetByFoundTimeRangeAsync(DateTime startTime, DateTime endTime);

        // 根据异常类型查询对账记录
        Task<List<ReconciliationEntity>> GetByReconTypeAsync(
            int reconType,
            int?size,int ?page);

        // 根据解决状态查询对账记录
        Task<List<ReconciliationEntity>> GetByResolvedStatusAsync(
            bool isResolved,
            int? size,int? page);

        // 获取未解决的对账异常
        Task<IEnumerable<ReconciliationEntity>> GetUnresolvedReconciliationsAsync();

        // 获取已解决的对账异常
        Task<IEnumerable<ReconciliationEntity>> GetResolvedReconciliationsAsync();

        // 根据差额金额范围查询
        Task<IEnumerable<ReconciliationEntity>> GetByDiffAmountRangeAsync(decimal minAmount, decimal maxAmount);

        // 根据受影响订单数范围查询
        Task<IEnumerable<ReconciliationEntity>> GetByAffectedOrdersRangeAsync(int minOrders, int maxOrders);

        // 获取高优先级异常 (差额金额大于指定值)
        Task<IEnumerable<ReconciliationEntity>> GetHighPriorityReconciliationsAsync(decimal threshold);
        // 统计异常类型分布
        Task<Dictionary<int, int>> GetReconTypeCountAsync();

        // 统计解决状态分布
        Task<Dictionary<bool, int>> GetResolvedStatusCountAsync();
        // 获取最近的对账异常
        Task<IEnumerable<ReconciliationEntity>> GetRecentReconciliationsAsync(int count = 10);

        // 计算总差额金额 (指定时间范围内)
        Task<decimal> GetTotalDiffAmountAsync(DateTime startTime, DateTime endTime);

        // 计算未解决异常的总差额
        Task<decimal> GetUnresolvedTotalDiffAmountAsync();

        // 获取对账异常统计概览
        Task<Dictionary<string, object>> GetReconciliationOverviewAsync(DateTime startTime, DateTime endTime);
        // 根据账期查询对账记录
        Task<IEnumerable<ReconciliationEntity>> GetBySpecificPeriodAsync(DateTime periodStart, DateTime periodEnd);

        // 获取长期未解决的异常 (超过指定天数)
        Task<IEnumerable<ReconciliationEntity>> GetLongStandingIssuesAsync(int daysThreshold);

        // 统计每月对账异常数量
        Task<Dictionary<string, int>> GetMonthlyReconciliationCountAsync(int year);
        // 获取异常金额最大的对账记录
        Task<IEnumerable<ReconciliationEntity>> GetTopByDiffAmountAsync(int topCount = 10);
    }
}
