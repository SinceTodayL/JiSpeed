using JISpeed.Core.Entities.Merchant;
using JISpeed.Core.Interfaces.IRepositories;
using JISpeed.Infrastructure.Data;

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
        /// </summary>
        /// <param name="merchant">用户实体</param>
        /// <returns>创建的用户实体</returns>
        public async Task<Merchant> CreateAsync(Merchant merchant)
        {
            var entity = await _context.Merchants.AddAsync(merchant);
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