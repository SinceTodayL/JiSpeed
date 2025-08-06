using JISpeed.Core.Constants;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Interfaces.IRepositories.Admin;
using JISpeed.Core.Interfaces.IRepositories.Merchant;
using JISpeed.Core.Interfaces.IServices;
using Microsoft.Extensions.Logging;
using StackExchange.Redis.Profiling;
using ApplicationEntity = JISpeed.Core.Entities.Merchant.Application;
namespace JISpeed.Application.Services.Merchant
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IMerchantRepository _merchantRepository;
        private readonly ILogger<ApplicationService> _logger;
        private readonly IAdminRepository  _adminRepository;
        public ApplicationService(
            IApplicationRepository applicationRepository,
            IMerchantRepository merchantRepository,
            IAdminRepository  adminRepository,
            ILogger<ApplicationService> logger)
        {
            _applicationRepository = applicationRepository;
            _merchantRepository = merchantRepository;
            _adminRepository = adminRepository;
            _logger = logger;
        }

        public async Task<ApplicationEntity?> GetDetailsAsync(string applyId)
        {
            try
            {
                _logger.LogInformation("开始获取申请详细信息, ApplyId: {ApplyId}", applyId);

                var data = await _applicationRepository.GetWithDetailsAsync(applyId);

                if (data == null)
                {
                    _logger.LogWarning("申请不存在, ApplyId: {ApplyId}", applyId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"申请不存在，ID: {applyId}");
                }

                _logger.LogInformation("成功获取申请详细信息, ApplyId: {ApplyId}", applyId);

                return data;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取申请详细信息时发生异常, ApplyId: {ApplyId}", applyId);
                throw new BusinessException("获取申请信息失败");
            }
        }

        public async Task<List<ApplicationEntity>> GetApplicationsByMerchantAsync(
            string merchantId,
            int? auditStatus,
            int? size, int? page)
        {
            try
            {
                _logger.LogInformation("开始通过时间段获取申请队列详细信息");
              
                List<ApplicationEntity>? data;
                if (auditStatus.HasValue)
                {
                    if (auditStatus < 0 || auditStatus > 2)
                    {
                        throw new NotFoundException(ErrorCodes.ResourceNotFound, $"无该状态，AuditStatus: {auditStatus}");
                    }
                    data = await _applicationRepository.GetByAuditStatusAndMerchantIdAsync(merchantId,auditStatus.Value,size,page);
                }
                else 
                {
                    data  = await _applicationRepository.GetByMerchantIdAsync(merchantId,size,page);
                }
                if (!data.Any())
                    _logger.LogInformation("该筛选条件内不存在申请");
                else
                    _logger.LogInformation("成功通过时间段获取队列信息");

                return data;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取队列信息时发生异常");
                throw new BusinessException("获取队列信息失败");
            }
        }

        
        public async Task<bool> ApproveApplicationAsync(string applyId, string adminId)
        {
            try
            {
                var res = await _adminRepository.GetByIdAsync(adminId);
                if (res == null)
                {
                    _logger.LogInformation("管理员不存在, AdminId: {AdminId}", adminId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"管理员不存在，ID: {adminId}");
                }
                var data = await _applicationRepository.GetWithDetailsAsync(applyId);
                if (data == null)
                {
                    _logger.LogWarning("申请不存在, ApplyId: {ApplyId}", applyId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"申请不存在，ID: {applyId}");
                }

                data.AuditAt = DateTime.UtcNow;
                data.AuditStatus = 1;
                data.AdminId = adminId;
                await _applicationRepository.SaveChangesAsync();
                

                _logger.LogInformation("通过申请，ApplyId: {ApplyId}", applyId);

                return true;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "通过申请时发生异常，ApplyId: {ApplyId}", applyId);
                throw new BusinessException("通过申请时失败");
            }
        }

        public async Task<bool> RejectApplicationAsync(string applyId, string adminId, string reason)
        {
            try
            {
                var res = await _adminRepository.GetByIdAsync(adminId);
                if (res == null)
                {
                    _logger.LogInformation("管理员不存在, AdminId: {AdminId}", adminId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"管理员不存在，ID: {adminId}");
                }
                var data = await _applicationRepository.GetWithDetailsAsync(applyId);
                if (data == null)
                {
                    _logger.LogWarning("申请不存在, ApplyId: {ApplyId}", applyId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"申请不存在，ID: {applyId}");
                }

                data.AuditAt = DateTime.UtcNow;
                data.AuditStatus = 2;
                data.AdminId = adminId;
                data.RejectReason = reason;
                await _applicationRepository.SaveChangesAsync();
                

                _logger.LogInformation("拒绝申请，ApplyId: {ApplyId}", applyId);

                return true;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "拒绝申请时发生异常，ApplyId: {ApplyId}", applyId);
                throw new BusinessException("拒绝申请时失败");
            }
        }

        public async Task<bool> CreateApplicationEntityAsync(string merchantId, ApplicationEntity application)
        {
            try
            {
                _logger.LogInformation("开始创建申请的请求, MerchantId: {MerchantId}", merchantId);

                var user = await _merchantRepository.GetByIdAsync(merchantId);

                if (user == null)
                {
                    _logger.LogWarning("商家不存在, MerchantId: {MerchantId}", merchantId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"无相关数据，ID: {merchantId}");
                }
                
                var res = await _applicationRepository.CreateAsync(application);

                if (res==null)
                {
                    _logger.LogWarning("创建失败");
                    throw new BusinessException("获取商家数据统计信息失败");
                }
                await _applicationRepository.SaveChangesAsync();

                _logger.LogInformation("创建申请成功, MerchantId: {MerchantId}", merchantId);
                return true;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "创建申请时发生异常, MerchantId: {MerchantId}", merchantId);
                throw new BusinessException("创建申请失败");
            }
        }

        public async Task<List<ApplicationEntity>> GetByFiltersAsync(
            int? auditStatus,
            int? size, int? page,
            DateTime? startDate, DateTime? endDate,
            string? merchantId, 
            bool? checkProfile,
            string adminId)
        {
            try
            {
                _logger.LogInformation("开始通过时间段获取申请队列详细信息");
                var res = await _adminRepository.ExistsAsync(adminId);
                if (!res)
                    throw new NotFoundException(ErrorCodes.InvalidCredentials,"该管理员不存在");
                
                List<ApplicationEntity>? data;
                if (auditStatus.HasValue)
                {
                    if (auditStatus < 0 || auditStatus > 2)
                    {
                        throw new NotFoundException(ErrorCodes.ResourceNotFound, $"无该状态，AuditStatus: {auditStatus}");
                    }
                    data = await _applicationRepository.GetByAuditStatusAsync(auditStatus.Value,size,page);

                }
                else if (startDate.HasValue || endDate.HasValue)
                {
                    var start = startDate.HasValue ? startDate.Value : DateTime.MinValue;
                    var end = endDate.HasValue ? endDate.Value : DateTime.Now;
                    data = await _applicationRepository.GetByTimeRangeAsync(start, end,size,page);
                }
                else if (merchantId != null)
                {
                    data  = await _applicationRepository.GetByMerchantIdAsync(merchantId,size,page);
                }
                else if (checkProfile.HasValue && checkProfile.Value)
                {
                    data = await _applicationRepository.GetByAdminIdAsync(adminId, size, page);
                }
                else
                {
                    data = await _applicationRepository.GetAllAsync(size, page);
                }

                if (!data.Any())
                    _logger.LogInformation("该筛选条件内不存在申请");
                else
                    _logger.LogInformation("成功通过时间段获取队列信息");

                return data;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取队列信息时发生异常");
                throw new BusinessException("获取队列信息失败");
            }
        }

    }
}