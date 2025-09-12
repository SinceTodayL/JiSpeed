using JISpeed.Core.Entities.Admin;

namespace JISpeed.Core.Interfaces.IRepositories
{
    // 平台仓储接口
    public interface IAdminRepository
    {
        /// 创建新用户
        
        /// <param name="admin">用户实体</param>
        /// <returns>创建的用户实体</returns>
        Task<Admin> CreateAsync(Admin admin);
        
        /// <returns>保存的记录数</returns>
        Task<int> SaveChangesAsync();
    }
    
}