using JISpeed.Core.Entities.Rider;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Entities.Dish;
using JISpeed.Core.Entities.Merchant;

namespace JISpeed.Core.Interfaces.IServices
{
    public interface IApplicationService
    {
        Task<Application?> GetDetailsAsync(string applyId);
        Task<List<Application>> GetApplicationsByMerchantAsync(string merchantId);
        Task<List<Application>> GetApplicationsByAuditStatusAsync(int auditStatus);
        
        Task<List<Application>> GetApplicationsByTimeRangeAsync(DateTime startTime, DateTime endTime);
        Task<bool> ApproveApplicationAsync(string applyId, string adminId);
        Task<bool> RejectApplicationAsync(string applyId, string adminId, string reason = "拒绝");
        Task<bool> CreateApplicationEntityAsync(string merchantId, Application application);
    }
}