using JISpeed.Core.Entities.Merchant;

namespace JISpeed.Core.Interfaces.IRepositories
{
    // 商家仓储接口
    public interface ISalesStatRepository
    {
        /// 创建新用户
        
        /// <param name="merchant">用户实体</param>
        /// <returns>创建的用户实体</returns>
        Task<SalesStat> CreateAsync(SalesStat data);
        
        /// <returns>保存的记录数</returns>
        Task<int> SaveChangesAsync();
        
        // <param name="userId">用户ID</param>
        // <returns>包含关联数据的用户实体，如果不存在则返回null</returns>
        Task<List<SalesStat>> GetDetailsAsync(string merchantId);
    }
    
}