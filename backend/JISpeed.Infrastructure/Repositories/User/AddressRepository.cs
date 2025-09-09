using JISpeed.Core.Entities.User;
using JISpeed.Core.Interfaces.IRepositories.User;
using JISpeed.Infrastructure.Data;
using JISpeed.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories.User
{
    public class AddressRepository : BaseRepository<Address, string>, IAddressRepository
    {
        public AddressRepository(OracleDbContext context) : base(context)
        {
        }

        // 重写GetWithDetailsAsync以包含关联数据
        // <param name="addressId">地址ID</param>
        // <returns>包含关联数据的地址实体，如果不存在则返回null</returns>
        public override async Task<Address?> GetWithDetailsAsync(string addressId)
        {
            return await _context.Addresses
                .Include(a => a.User)
                .ThenInclude(u => u.ApplicationUser)
                .FirstOrDefaultAsync(a => a.AddressId == addressId);
        }

        // 重写GetAllAsync以包含关联数据和排序
        // <returns>地址列表</returns>
        public override async Task<List<Address>> GetAllAsync()
        {
            return await _context.Addresses
                .Include(a => a.User)
                .OrderBy(a => a.UserId)
                .ThenByDescending(a => a.IsDefault)
                .ToListAsync();
        }

        // === 业务专用查询方法 ===

        // 根据用户ID获取地址列表
        // <param name="userId">用户ID</param>
        // <returns>地址列表</returns>
        public async Task<List<Address>> GetByUserIdAsync(string userId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ToListAsync();
        }

        /// <summary>
        /// 根据用户ID获取地址列表（分页）
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="page">页码</param>
        /// <param name="size">每页大小</param>
        /// <returns>地址列表</returns>
        public async Task<List<Address>> GetByUserIdAsync(string userId, int page, int size)
        {
            var query = _context.Addresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.AddressId);

            if (page > 0 && size > 0)
            {
                return await query.Skip((page - 1) * size).Take(size).ToListAsync();
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// 根据用户ID获取地址数量
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>地址数量</returns>
        public async Task<int> GetCountByUserIdAsync(string userId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == userId)
                .CountAsync();
        }

        // 根据用户ID获取默认地址
        // <param name="userId">用户ID</param>
        // <returns>默认地址，如果不存在则返回null</returns>
        public async Task<Address?> GetDefaultByUserIdAsync(string userId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == userId && a.IsDefault == 1)
                .FirstOrDefaultAsync();
        }
    }
}
