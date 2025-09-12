using JISpeed.Core.Entities.Merchant;

namespace JISpeed.Core.Interfaces.IRepositories
{
    // 商家仓储接口
    public interface IMerchantRepository
    {
        /// 创建新用户
        
        /// <param name="merchant">用户实体</param>
        /// <returns>创建的用户实体</returns>
        Task<Merchant> CreateAsync(Merchant merchant);
        
        /// <returns>保存的记录数</returns>
        Task<int> SaveChangesAsync();
    }
    
}