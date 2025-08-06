using JISpeed.Core.Entities.Merchant;

namespace JISpeed.Core.Interfaces.IRepositories.Merchant
{
    // 销售统计仓储接口 - 复合主键 (StatDate, MerchantId)
    public interface ISalesStatRepository
    {
        // === 基础 CRUD 操作 (复合主键) ===

        // 根据复合主键获取销售统计
        Task<SalesStat?> GetByIdAsync(DateTime statDate, string merchantId);

        // 根据复合主键获取详细信息
        Task<SalesStat?> GetWithDetailsAsync(DateTime statDate, string merchantId);

        // 获取所有销售统计
        Task<List<SalesStat>> GetAllAsync();

        // 检查是否存在
        Task<bool> ExistsAsync(DateTime statDate, string merchantId);

        // 创建销售统计
        Task<SalesStat> CreateAsync(SalesStat salesStat);

        // 更新销售统计
        Task<SalesStat> UpdateAsync(SalesStat salesStat);

        // 删除销售统计
        Task<bool> DeleteAsync(DateTime statDate, string merchantId);

        // 保存更改
        Task<int> SaveChangesAsync();

        // === 业务专用查询方法 ===

        // 根据商家ID获取销售统计
        // <param name="merchantId">商家ID</param>
        // <returns>销售统计列表</returns>
        Task<List<SalesStat>> GetByMerchantIdAsync(string merchantId, int? size, int? page);

        // 根据统计类型获取记录
        // <param name="statType">统计类型</param>
        // <returns>销售统计列表</returns>
        Task<List<SalesStat>> GetByStatTypeAsync(int statType);

        // 根据统计类型获取记录 - 按销售额范围分类
        // <returns>销售统计列表</returns>
        Task<List<SalesStat>> GetBySalesRangeAsync(decimal minAmount, decimal maxAmount, int? size, int? page);

        
        // 根据时间范围获取销售统计
        // <param name="startTime">开始时间</param>
        // <param name="endTime">结束时间</param>
        // <returns>销售统计列表</returns>
        Task<List<SalesStat>> GetByTimeRangeAsync(DateTime startTime, DateTime endTime);

        // 根据商家ID和统计类型获取记录
        // <param name="merchantId">商家ID</param>
        // <param name="statType">统计类型</param>
        // <returns>销售统计列表</returns>
        Task<List<SalesStat>> GetByMerchantIdAndStatTypeAsync(string merchantId, int statType, int? size, int? page);

        // 根据商家ID和时间范围获取统计
        // <param name="merchantId">商家ID</param>
        // <param name="startTime">开始时间</param>
        // <param name="endTime">结束时间</param>
        // <returns>销售统计列表</returns>
        Task<List<SalesStat>> GetByMerchantIdAndTimeRangeAsync(string merchantId, DateTime startTime, DateTime endTime,int ? size, int? page);

        // 获取最新的销售统计
        // <param name="merchantId">商家ID</param>
        // <param name="statType">统计类型</param>
        // <returns>最新的销售统计</returns>
        Task<SalesStat?> GetLatestByMerchantIdAndStatTypeAsync(string merchantId, int statType);
    }
}
