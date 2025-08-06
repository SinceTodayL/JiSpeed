using JISpeed.Core.Entities.Merchant;

namespace JISpeed.Core.Interfaces.IRepositories.Merchant
{
    public interface IApplicationRepository : IBaseRepository<JISpeed.Core.Entities.Merchant.Application, string>
    {
        Task<List<Application>> GetAllAsync(int? size,int? page);
        Task<List<Application>> GetByMerchantIdAsync(string merchantId,int?size,int? page);
        Task<List<Application>> GetByAdminIdAsync(string adminId,int? size,int? page);
        Task<List<Application>> GetByAuditStatusAsync(int auditStatus,int?size,int? page);
        Task<List<Application>> GetByAuditStatusAndMerchantIdAsync(string merchantId,int auditStatus,int?size,int? page);

        Task<List<Application>> GetByTimeRangeAsync(DateTime startTime, DateTime endTime,int?size,int? page);
        Task<List<Application>> SearchByCompanyNameAsync(string companyName);
        
        
        

    }
}