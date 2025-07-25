using JISpeed.Core.Entities.User;
using JISpeed.Core.Interfaces.IRepositories.User;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories.User
{
    public class AddressRepository : IAddressRepository
    {
        private readonly OracleDbContext _context;

        public AddressRepository(OracleDbContext context)
        {
            _context = context;
        }

        // 根据地址ID获取地址信息
        // <param name="addressId">地址ID</param>
        // <returns>地址实体，如果不存在则返回null</returns>
        public async Task<Address?> GetByIdAsync(string addressId)
        {
            return await _context.Addresses
                .FirstOrDefaultAsync(a => a.AddressId == addressId);
        }

        // 根据地址ID获取地址详细信息（包含关联数据）
        // <param name="addressId">地址ID</param>
        // <returns>包含关联数据的地址实体，如果不存在则返回null</returns>
        public async Task<Address?> GetWithDetailsAsync(string addressId)
        {
            return await _context.Addresses
                .Include(a => a.User)
                .ThenInclude(u => u.ApplicationUser)
                .FirstOrDefaultAsync(a => a.AddressId == addressId);
        }

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

        // 根据用户ID获取默认地址
        // <param name="userId">用户ID</param>
        // <returns>默认地址，如果不存在则返回null</returns>
        public async Task<Address?> GetDefaultByUserIdAsync(string userId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == userId && a.IsDefault == 1)
                .FirstOrDefaultAsync();
        }

        // 获取所有地址列表
        // <returns>地址列表</returns>
        public async Task<List<Address>> GetAllAsync()
        {
            return await _context.Addresses
                .Include(a => a.User)
                .OrderBy(a => a.UserId)
                .ThenByDescending(a => a.IsDefault)
                .ToListAsync();
        }

        // 检查地址是否存在
        // <param name="addressId">地址ID</param>
        // <returns>地址是否存在</returns>
        public async Task<bool> ExistsAsync(string addressId)
        {
            return await _context.Addresses
                .AnyAsync(a => a.AddressId == addressId);
        }

        // 创建新地址
        // <param name="address">地址实体</param>
        // <returns>创建的地址实体</returns>
        public async Task<Address> CreateAsync(Address address)
        {
            var entity = await _context.Addresses.AddAsync(address);
            return entity.Entity;
        }

        // 更新地址信息
        // <param name="address">地址实体</param>
        // <returns>更新的地址实体</returns>
        public async Task<Address> UpdateAsync(Address address)
        {
            var entity = _context.Addresses.Update(address);
            await Task.CompletedTask; // 解决异步警告
            return entity.Entity;
        }

        // 删除地址
        // <param name="addressId">地址ID</param>
        // <returns>是否删除成功</returns>
        public async Task<bool> DeleteAsync(string addressId)
        {
            var address = await GetByIdAsync(addressId);
            if (address == null)
                return false;

            _context.Addresses.Remove(address);
            return true;
        }

        // 保存更改
        // <returns>保存的记录数</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
