namespace JISpeed.Core.Interfaces.IRepositories
{
    // 基础仓储接口，定义通用的CRUD操作
    // <typeparam name="TEntity">实体类型</typeparam>
    // <typeparam name="TKey">主键类型</typeparam>
    public interface IBaseRepository<TEntity, TKey> where TEntity : class
    {
        // 根据主键获取实体
        // <param name="id">主键</param>
        // <returns>实体，如果不存在则返回null</returns>
        Task<TEntity?> GetByIdAsync(TKey id);

        // 根据主键获取实体详细信息（包含关联数据）
        // <param name="id">主键</param>
        // <returns>包含关联数据的实体，如果不存在则返回null</returns>
        Task<TEntity?> GetWithDetailsAsync(TKey id);

        // 获取所有实体列表
        // <returns>实体列表</returns>
        Task<List<TEntity>> GetAllAsync();

        // 检查实体是否存在
        // <param name="id">主键</param>
        // <returns>实体是否存在</returns>
        Task<bool> ExistsAsync(TKey id);

        // 创建新实体
        // <param name="entity">实体</param>
        // <returns>创建的实体</returns>
        Task<TEntity> CreateAsync(TEntity entity);

        // 更新实体信息
        // <param name="entity">实体</param>
        // <returns>更新的实体</returns>
        Task<TEntity> UpdateAsync(TEntity entity);

        // 删除实体
        // <param name="id">主键</param>
        // <returns>是否删除成功</returns>
        Task<bool> DeleteAsync(TKey id);

        // 保存更改
        // <returns>保存的记录数</returns>
        Task<int> SaveChangesAsync();
    }
}
