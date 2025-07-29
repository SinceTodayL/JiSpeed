using JISpeed.Core.Constants;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Interfaces.IRepositories.Admin;
using JISpeed.Core.Interfaces.IRepositories.Merchant;
using JISpeed.Core.Interfaces.IServices;
using Microsoft.Extensions.Logging;
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

        public async Task<List<ApplicationEntity>> GetApplicationsByTimeRangeAsync(DateTime startTime, DateTime endTime)
        {
            try
            {
                _logger.LogInformation("开始通过时间段获取申请队列详细信息");

                var data = await _applicationRepository.GetByTimeRangeAsync(startTime, endTime);

                if (!data.Any())
                    _logger.LogInformation("该时间段不存在申请");
                else
                    _logger.LogInformation("成功通过时间段获取队列信息");

                return data;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "通过时间段获取队列信息时发生异常");
                throw new BusinessException("通过时间段获取队列信息失败");
            }
        }
        public async Task<List<ApplicationEntity>> GetApplicationsByMerchantAsync(string merchantId)
        {
            try
            {
                _logger.LogInformation("开始获取商家申请队列详细信息, MerchantId: {MerchantId}", merchantId);

                var data = await _applicationRepository.GetByMerchantIdAsync(merchantId);

                if (!data.Any())
                {
                    _logger.LogWarning("申请不存在,  MerchantId: {MerchantId}", merchantId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"申请不存在，ID: {merchantId}");
                }

                _logger.LogInformation("成功获取商家申请队列信息,MerchantId: {MerchantId}", merchantId);

                return data;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取商家申请队列信息时发生异常, MerchantId: {MerchantId}", merchantId);
                throw new BusinessException("获取商家申请队列信息失败");
            }
        }

        public async Task<List<ApplicationEntity>> GetApplicationsByAuditStatusAsync(int auditStatus)
        {
            try
            {
                _logger.LogInformation("开始获取目标处理状态的队列详细信息");
                if (auditStatus < 0 || auditStatus > 2)
                {
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"无该状态，AuditStatus: {auditStatus}");
                }
                var data = await _applicationRepository.GetByAuditStatusAsync(auditStatus);

                if (!data.Any())
                    _logger.LogWarning("无目标处理状态的申请");
                else
                    _logger.LogInformation("成功获取待目标处理状态的队列信息");

                return data;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取目标处理状态的申请队列信息时发生异常");
                throw new BusinessException("获取目标处理状态的申请申请队列信息失败");
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


    }
}