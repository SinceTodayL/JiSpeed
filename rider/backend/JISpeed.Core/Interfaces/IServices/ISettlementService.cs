using JISpeed.Core.Entities.Merchant;

namespace JISpeed.Core.Interfaces.IServices
{
    public interface ISettlementService
    {
        Task<Settlement?> GetDetailBySettlementIdAsync(string settlementId);
        Task<List<Settlement>> GetSettlementListByFiltersAsync(
            string? merchantId,
            DateTime? startDate,DateTime? endDate,
            bool? isSettled,
            int? size, int?page);

    }
}