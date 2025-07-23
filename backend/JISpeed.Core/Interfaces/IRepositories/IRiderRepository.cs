using JISpeed.Core.Entities.Rider;

namespace JISpeed.Core.Interfaces.IRepositories
{
    // 骑手仓储接口
    public interface IRiderRepository
    {
        /// 创建新用户
        
        /// <param name="rider">用户实体</param>
        /// <returns>创建的用户实体</returns>
        Task<Rider> CreateAsync(Rider rider);
        
        /// <returns>保存的记录数</returns>
        Task<int> SaveChangesAsync();
    }
    
}