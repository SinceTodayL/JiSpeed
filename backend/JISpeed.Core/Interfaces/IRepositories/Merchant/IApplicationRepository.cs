using JISpeed.Core.Entities.Merchant;

namespace JISpeed.Core.Interfaces.IRepositories.Merchant
{
    public interface IApplicationRepository : IBaseRepository<JISpeed.Core.Entities.Merchant.Application, string>
    {
        Task<List<Application>> GetByMerchantIdAsync(string merchantId);
        Task<List<Application>> GetByAdminIdAsync(string adminId);
        Task<List<Application>> GetByAuditStatusAsync(int auditStatus);
        Task<List<Application>> GetByTimeRangeAsync(DateTime startTime, DateTime endTime);
        Task<List<Application>> SearchByCompanyNameAsync(string companyName);
        
        
        

    }
}