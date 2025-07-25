using JISpeed.Core.Interfaces.IRepositories;
using JISpeed.Core.Interfaces.IRepositories.Merchant;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using MerchantEntity = JISpeed.Core.Entities.Merchant.Merchant;

namespace JISpeed.Infrastructure.Repositories
{
    public class MerchantRepository : IMerchantRepository
    {
        private readonly OracleDbContext _context;

        public MerchantRepository(OracleDbContext context)
        {
            _context = context;
        }
        /// 创建新用户

        /// <param name="merchant">用户实体</param>
        /// <returns>创建的用户实体</returns>
        public async Task<MerchantEntity> CreateAsync(MerchantEntity merchant)
        {
            var entity = await _context.Merchants.AddAsync(merchant);
            return entity.Entity;
        }
        /// 保存更改

        /// <returns>保存的记录数</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<MerchantEntity?> GetUserWithDetailsAsync(string merchantId)
        {
            return await _context.Merchants
                .FirstOrDefaultAsync(u => u.MerchantId == merchantId);
        }

        public async Task<MerchantEntity?> GetByIdAsync(string merchantId)
        {
            return await _context.Merchants
                .FirstOrDefaultAsync(u => u.MerchantId == merchantId);
        }

        public async Task<MerchantEntity?> GetWithDetailsAsync(string merchantId)
        {
            return await _context.Merchants
                .FirstOrDefaultAsync(u => u.MerchantId == merchantId);
        }

        public async Task<MerchantEntity?> GetByApplicationUserIdAsync(string applicationUserId)
        {
            return await _context.Merchants
                .FirstOrDefaultAsync(u => u.ApplicationUserId == applicationUserId);
        }

        public async Task<List<MerchantEntity>> SearchByNameAsync(string name)
        {
            return await _context.Merchants
                .Where(m => m.MerchantName.Contains(name))
                .ToListAsync();
        }

        public async Task<List<MerchantEntity>> GetByStatusAsync(int status)
        {
            return await _context.Merchants
                .Where(m => m.Status == status)
                .ToListAsync();
        }

        public async Task<List<MerchantEntity>> SearchByLocationAsync(string location)
        {
            return await _context.Merchants
                .Where(m => !string.IsNullOrEmpty(m.Location) && m.Location.Contains(location))
                .ToListAsync();
        }

        public async Task<List<MerchantEntity>> GetAllAsync()
        {
            return await _context.Merchants.ToListAsync();
        }

        public async Task<bool> ExistsAsync(string merchantId)
        {
            return await _context.Merchants
                .AnyAsync(m => m.MerchantId == merchantId);
        }

        public async Task<MerchantEntity> UpdateAsync(MerchantEntity merchant)
        {
            _context.Merchants.Update(merchant);
            await Task.CompletedTask; // 修复CS1998警告
            return merchant;
        }

        public async Task<bool> DeleteAsync(string merchantId)
        {
            var merchant = await GetByIdAsync(merchantId);
            if (merchant == null)
                return false;

            _context.Merchants.Remove(merchant);
            return true;
        }
    }
}