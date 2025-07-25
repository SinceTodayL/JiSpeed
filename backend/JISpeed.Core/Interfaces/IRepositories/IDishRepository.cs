using JISpeed.Core.Entities.Dish;


namespace JISpeed.Core.Interfaces.IRepositories
{
    // Dish仓储接口
    public interface IDishRepository
    {
        /// 创建新用户
        
        /// <param name="merchant">用户实体</param>
        /// <returns>创建的用户实体</returns>
        Task<Dish> CreateAsync(Dish data);
        
        /// <returns>保存的记录数</returns>
        Task<int> SaveChangesAsync();
        
        // <param name="merchantId">用户ID</param>
        // <returns>包含关联数据的用户实体，如果不存在则返回null</returns>
        Task<List<Dish>> GetDetailsAsync(string merchantId);
    }
    
}