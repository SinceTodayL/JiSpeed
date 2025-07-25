using JISpeed.Core.Entities.Merchant;

namespace JISpeed.Core.Interfaces.IRepositories.Merchant
{
    // 销售统计仓储接口
    public interface ISalesStatRepository
    {
        // 根据复合主键获取销售统计信息
        // <param name="statDate">统计日期</param>
        // <param name="merchantId">商家ID</param>
        // <returns>销售统计实体，如果不存在则返回null</returns>
        Task<SalesStat?> GetByIdAsync(DateTime statDate, string merchantId);

        // 根据复合主键获取销售统计详细信息（包含关联数据）
        // <param name="statDate">统计日期</param>
        // <param name="merchantId">商家ID</param>
        // <returns>包含关联数据的销售统计实体，如果不存在则返回null</returns>
        Task<SalesStat?> GetWithDetailsAsync(DateTime statDate, string merchantId);

        // 根据商家ID获取销售统计列表
        // <param name="merchantId">商家ID</param>
        // <returns>销售统计列表</returns>
        Task<List<SalesStat>> GetByMerchantIdAsync(string merchantId);

        // 根据时间范围获取销售统计列表
        // <param name="startDate">开始日期</param>
        // <param name="endDate">结束日期</param>
        // <returns>销售统计列表</returns>
        Task<List<SalesStat>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        // 根据商家ID和时间范围获取销售统计列表
        // <param name="merchantId">商家ID</param>
        // <param name="startDate">开始日期</param>
        // <param name="endDate">结束日期</param>
        // <returns>销售统计列表</returns>
        Task<List<SalesStat>> GetByMerchantIdAndDateRangeAsync(string merchantId, DateTime startDate, DateTime endDate);

        // 获取所有销售统计列表
        // <returns>销售统计列表</returns>
        Task<List<SalesStat>> GetAllAsync();

        // 检查销售统计是否存在
        // <param name="statDate">统计日期</param>
        // <param name="merchantId">商家ID</param>
        // <returns>销售统计是否存在</returns>
        Task<bool> ExistsAsync(DateTime statDate, string merchantId);

        // 创建新销售统计
        // <param name="salesStat">销售统计实体</param>
        // <returns>创建的销售统计实体</returns>
        Task<SalesStat> CreateAsync(SalesStat salesStat);

        // 更新销售统计信息
        // <param name="salesStat">销售统计实体</param>
        // <returns>更新的销售统计实体</returns>
        Task<SalesStat> UpdateAsync(SalesStat salesStat);

        // 删除销售统计
        // <param name="statDate">统计日期</param>
        // <param name="merchantId">商家ID</param>
        // <returns>是否删除成功</returns>
        Task<bool> DeleteAsync(DateTime statDate, string merchantId);

        // 保存更改
        // <returns>保存的记录数</returns>
        Task<int> SaveChangesAsync();
    }
}