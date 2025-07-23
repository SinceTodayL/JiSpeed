using JISpeed.Core.Entities.Admin;
using JISpeed.Core.Interfaces.IRepositories;
using JISpeed.Infrastructure.Data;

namespace JISpeed.Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly OracleDbContext _context;

        public AdminRepository(OracleDbContext context)
        {
            _context = context;
        }
        /// 创建新用户
        /// </summary>
        /// <param name="rider">用户实体</param>
        /// <returns>创建的用户实体</returns>
        public async Task<Admin> CreateAsync(Admin admin)
        {
            var entity = await _context.Admins.AddAsync(admin);
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