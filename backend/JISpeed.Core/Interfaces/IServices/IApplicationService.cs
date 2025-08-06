using JISpeed.Core.Entities.Merchant;

namespace JISpeed.Core.Interfaces.IServices
{
    public interface IApplicationService
    {
        Task<Application?> GetDetailsAsync(string applyId);
        Task<List<Application>> GetApplicationsByMerchantAsync(
            string merchantId,
            int? auditStatus,
            int? size, int? page);

        Task<bool> ApproveApplicationAsync(string applyId, string adminId);
        Task<bool> RejectApplicationAsync(string applyId, string adminId, string reason = "拒绝");
        Task<bool> CreateApplicationEntityAsync(string merchantId, Application application);
        
        Task<List<Application>> GetByFiltersAsync(
            int? auditStatus, 
            int? size, int? page,
            DateTime? startDate,DateTime? endDate,
            string? merchantId,
            bool? checkProfile,
            string adminId);
    }
}