using JISpeed.Core.Entities.Rider;
using JISpeed.Core.Interfaces.IRepositories;
using JISpeed.Infrastructure.Data;

namespace JISpeed.Infrastructure.Repositories
{
    public class RiderRepository : IRiderRepository
    {
        private readonly OracleDbContext _context;

        public RiderRepository(OracleDbContext context)
        {
            _context = context;
        }
        /// 创建新用户
        /// </summary>
        /// <param name="rider">用户实体</param>
        /// <returns>创建的用户实体</returns>
        public async Task<Rider> CreateAsync(Rider rider)
        {
            var entity = await _context.Riders.AddAsync(rider);
            return entity.Entity;
        }
        /// 保存更改
        /// </summary>
        /// <returns>保存的记录数</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}