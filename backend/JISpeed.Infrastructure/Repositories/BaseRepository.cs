using JISpeed.Core.Interfaces.IRepositories;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories
{
    // 基础仓储实现类，提供通用的CRUD操作
    // <typeparam name="TEntity">实体类型</typeparam>
    // <typeparam name="TKey">主键类型</typeparam>
    public abstract class BaseRepository<TEntity, TKey> : IBaseRepository<TEntity, TKey>
        where TEntity : class
    {
        protected readonly OracleDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        protected BaseRepository(OracleDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        // 根据主键获取实体
        // <param name="id">主键</param>
        // <returns>实体，如果不存在则返回null</returns>
        public virtual async Task<TEntity?> GetByIdAsync(TKey id)
        {
            return await _dbSet.FindAsync(id);
        }

        // 根据主键获取实体详细信息（包含关联数据）
        // 子类可以重写此方法来包含特定的关联数据
        // <param name="id">主键</param>
        // <returns>包含关联数据的实体，如果不存在则返回null</returns>
        public virtual async Task<TEntity?> GetWithDetailsAsync(TKey id)
        {
            return await GetByIdAsync(id);
        }

        // 获取所有实体列表
        // <returns>实体列表</returns>
        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        // 检查实体是否存在
        // <param name="id">主键</param>
        // <returns>实体是否存在</returns>
        public virtual async Task<bool> ExistsAsync(TKey id)
        {
            var entity = await _dbSet.FindAsync(id);
            return entity != null;
        }

        // 创建新实体
        // <param name="entity">实体</param>
        // <returns>创建的实体</returns>
        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            var entityEntry = await _dbSet.AddAsync(entity);
            return entityEntry.Entity;
        }

        // 更新实体信息
        // <param name="entity">实体</param>
        // <returns>更新的实体</returns>
        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            var entityEntry = _dbSet.Update(entity);
            await Task.CompletedTask; // 修复CS1998警告
            return entityEntry.Entity;
        }

        // 删除实体
        // <param name="id">主键</param>
        // <returns>是否删除成功</returns>
        public virtual async Task<bool> DeleteAsync(TKey id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
                return false;

            _dbSet.Remove(entity);
            return true;
        }

        // 保存更改
        // <returns>保存的记录数</returns>
        public virtual async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
