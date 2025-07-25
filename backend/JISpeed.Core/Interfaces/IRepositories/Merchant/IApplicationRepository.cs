using JISpeed.Core.Entities.Merchant;

namespace JISpeed.Core.Interfaces.IRepositories.Merchant
{
    // 商家申请仓储接口
    public interface IApplicationRepository
    {
        // 根据申请ID获取申请信息
        // <param name="applyId">申请ID</param>
        // <returns>申请实体，如果不存在则返回null</returns>
        Task<Application?> GetByIdAsync(string applyId);

        // 根据申请ID获取申请详细信息（包含关联数据）
        // <param name="applyId">申请ID</param>
        // <returns>包含关联数据的申请实体，如果不存在则返回null</returns>
        Task<Application?> GetWithDetailsAsync(string applyId);

        // 根据商家ID获取申请列表
        // <param name="merchantId">商家ID</param>
        // <returns>申请列表</returns>
        Task<List<Application>> GetByMerchantIdAsync(string merchantId);

        // 根据管理员ID获取申请列表
        // <param name="adminId">管理员ID</param>
        // <returns>申请列表</returns>
        Task<List<Application>> GetByAdminIdAsync(string adminId);

        // 根据审核状态获取申请列表
        // <param name="auditStatus">审核状态</param>
        // <returns>申请列表</returns>
        Task<List<Application>> GetByAuditStatusAsync(int auditStatus);

        // 根据时间范围获取申请列表
        // <param name="startTime">开始时间</param>
        // <param name="endTime">结束时间</param>
        // <returns>申请列表</returns>
        Task<List<Application>> GetByTimeRangeAsync(DateTime startTime, DateTime endTime);

        // 根据公司名称搜索申请
        // <param name="companyName">公司名称</param>
        // <returns>申请列表</returns>
        Task<List<Application>> SearchByCompanyNameAsync(string companyName);

        // 获取所有申请列表
        // <returns>申请列表</returns>
        Task<List<Application>> GetAllAsync();

        // 检查申请是否存在
        // <param name="applyId">申请ID</param>
        // <returns>申请是否存在</returns>
        Task<bool> ExistsAsync(string applyId);

        // 创建新申请
        // <param name="application">申请实体</param>
        // <returns>创建的申请实体</returns>
        Task<Application> CreateAsync(Application application);

        // 更新申请信息
        // <param name="application">申请实体</param>
        // <returns>更新的申请实体</returns>
        Task<Application> UpdateAsync(Application application);

        // 删除申请
        // <param name="applyId">申请ID</param>
        // <returns>是否删除成功</returns>
        Task<bool> DeleteAsync(string applyId);

        // 保存更改
        // <returns>保存的记录数</returns>
        Task<int> SaveChangesAsync();
    }
}
