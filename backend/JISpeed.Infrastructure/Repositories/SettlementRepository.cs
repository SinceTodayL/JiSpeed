using JISpeed.Core.Entities.Merchant;
using JISpeed.Core.Interfaces.IRepositories;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories
{
    public class SettlementRepository : ISettlementRepository
    {
        private readonly OracleDbContext _context;

        public SettlementRepository(OracleDbContext context)
        {
            _context = context;
        }

        public async Task<Settlement> CreateAsync(Settlement data)
        {
            var entity = await _context.Settlements.AddAsync(data);
            return entity.Entity;
        }

        public async Task<List<Settlement>> GetByMerchantIdAsync(string merchantId)
        {
            return await _context.Settlements.Where(u => u.MerchantId == merchantId)
                .ToListAsync();
        }

        public async Task<Settlement?> GetByMerchantIdAndSettleIdAsync(string merchantId, string settleId)
        {
            return await _context.Settlements
                .FirstOrDefaultAsync(u => u.MerchantId == merchantId && u.SettleId == settleId);
        }
    }
}