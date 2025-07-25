using JISpeed.Core.Entities.Merchant;

namespace JISpeed.Core.Interfaces.IRepositories
{
    public interface ISettlementRepository
    {
        Task<Settlement> CreateAsync(Settlement data);
        Task<List<Settlement>>GetByMerchantIdAsync(string merchantId);
        Task<Settlement?>GetByMerchantIdAndSettleIdAsync(string merchantId, string settleId);
    }
}