using JISpeed.Core.Entities.Merchant;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Interfaces.IRepositories;
using JISpeed.Core.Interfaces.IRepositories.Merchant;
using JISpeed.Core.Interfaces.IRepositories.Dish;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Core.Constants;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Interfaces.IRepositories.Merchant;
using Microsoft.Extensions.Logging;
using MerchantEntity = JISpeed.Core.Entities.Merchant.Merchant;

namespace JISpeed.Application.Services.Merchant
{
    public class MerchantService : IMerchantService
    {
        private readonly IMerchantRepository _merchantRepository;
        private readonly ISalesStatRepository _salesStatRepository;
        private readonly ILogger<MerchantService> _logger;

        public MerchantService(
            IMerchantRepository merchantRepository,
            ISalesStatRepository salesStatRepository,
            ILogger<MerchantService> logger
            )
        {
            _merchantRepository = merchantRepository;
            _salesStatRepository = salesStatRepository;
            _logger = logger;
        }

        public async Task<List<string>> GetMerchantNameForSearchAsync(string prefix, int? limit)
        {
            var data = await _merchantRepository.GetMerchantNamesAsync(prefix, limit);
            return data;
        }

        // 使用MerchantId获取商家详细信息
        // <returns>用户实体，如果不存在则返回null</returns>
        public async Task<MerchantEntity> GetMerchantDetailAsync(string merchantId)
        {
            try
            {
                _logger.LogInformation("开始获取商家详细信息, MerchantId: {MerchantId}", merchantId);

                var user = await _merchantRepository.GetByIdAsync(merchantId);

                if (merchant == null)
                {
                    _logger.LogWarning("商家不存在, MerchantId: {MerchantId}", merchantId);
                    throw new NotFoundException(ErrorCodes.MerchantNotFound, $"商家不存在，ID: {merchantId}");
                }

                _logger.LogInformation("成功获取商家详细信息, MerchantId: {MerchantId}, MerchantName: {MerchantName}",
                    merchantId, merchant.MerchantName);

                return merchant;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取商家详细信息时发生异常, MerchantId: {MerchantId}", merchantId);
                throw new BusinessException("获取商家信息失败");
            }
        }

        public async Task<bool> UpdateMerchantDetailAsync(
            string merchantId, string? merchantName,
            int? status, string? contactInfo,
            string? location)
        {
            try
            {
                var user = await _merchantRepository.GetByIdAsync(merchantId);
                if (user == null)
                {
                    _logger.LogWarning("商家不存在, MerchantId: {MerchantId}", merchantId);
                    throw new NotFoundException(ErrorCodes.MerchantNotFound, $"商家不存在，ID: {merchantId}");
                }
                _logger.LogInformation("开始更新商家详细信息, MerchantId: {MerchantId}", merchantId);

                user.MerchantName = merchantName?? user.MerchantName;
                user.ContactInfo = contactInfo?? user.ContactInfo;
                user.Location = location?? user.Location;
                user.Status = status?? user.Status;
                await _merchantRepository.SaveChangesAsync();
                _logger.LogInformation("成功修改商家详细信息, MerchantId: {MerchantId}, MerchantName: {MerchantName}",
                    merchantId, user.MerchantName);

                return true;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取商家详细信息时发生异常, MerchantId: {MerchantId}", merchantId);
                throw new BusinessException("获取商家信息失败");
            }
        }
        public async Task<List<MerchantEntity>> GetMerchantByFiltersAsync(
            int? size, int? page,
            string? merchantName,
            string? location)
        {
            try
            {
                _logger.LogInformation("开始获取商家列表信息");

                List<MerchantEntity>? data;
                if (merchantName != null)
                {
                    data = await _merchantRepository.SearchByNameAsync(merchantName,size,page);
                }
                else if (location != null)
                {
                    data = await _merchantRepository.SearchByLocationAsync(location,size,page);
                }
                else
                {
                    data = await _merchantRepository.GetAllMerchantsAsync(size,page);
                }
                if (data == null||!data.Any())
                {
                    _logger.LogWarning("商家不存在");
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, "商家不存在");
                }
                
                return data;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取商家列表信息时发生异常");
                throw new BusinessException("获取商家列表信息失败");
            }
        }
        public async Task<SalesStat?> GetSalesStateByMerchantIdAndDateTimeAsync(string merchantId, DateTime statTime)
        {
            try
            {
                _logger.LogInformation("开始获取商家数据统计信息, MerchantId: {MerchantId}", merchantId);
                

                var data = await _salesStatRepository.GetByIdAsync(statTime,merchantId);

                if (data == null)
                {
                    _logger.LogWarning("无相关数据, MerchantId: {MerchantId}", merchantId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"无相关数据，ID: {merchantId}");
                }

                _logger.LogInformation("成功获取商家数据统计信息, MerchantId: {MerchantId}", merchantId);

                return data;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取商家数据统计信息时发生异常, MerchantId: {MerchantId}", merchantId);
                throw new BusinessException("获取商家数据统计信息失败");
            }
        }

        

        public async Task<List<SalesStat>> GetByFiltersAsync(
            string merchantId, 
            int? statType, int? size, int? page,
            DateTime? startTime, DateTime? endTime, 
            decimal? minAmount, decimal? maxAmount)
        {
            try
            {
                _logger.LogInformation("开始获取商家数据统计信息, MerchantId: {MerchantId}", merchantId);
                List<SalesStat>? data;
                if(startTime.HasValue||endTime.HasValue)
                {
                    var start = startTime.HasValue ? startTime.Value : DateTime.MinValue;
                    var end = endTime.HasValue ? endTime.Value : DateTime.Now;
                    data = await _salesStatRepository.GetByMerchantIdAndTimeRangeAsync(merchantId, start, end, size, page);
                } 
                else if (minAmount.HasValue || maxAmount.HasValue)
                {
                    var min = minAmount??0;
                    var max = maxAmount??Decimal.MaxValue;
                    data = await _salesStatRepository.GetBySalesRangeAsync(min, max, size, page);
                }
                else if (statType.HasValue)
                    data = await _salesStatRepository.GetByMerchantIdAndStatTypeAsync(merchantId, statType.Value,size,page);
                else
                    data = await _salesStatRepository.GetByMerchantIdAsync(merchantId,size,page);

                if (data == null || !data.Any())
                {
                    _logger.LogWarning("无相关数据, MerchantId: {MerchantId}", merchantId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"无相关数据，ID: {merchantId}");
                }

                _logger.LogInformation("成功获取商家数据统计信息, MerchantId: {MerchantId}", merchantId);

                return data;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取商家数据统计信息时发生异常, MerchantId: {MerchantId}", merchantId);
                throw new BusinessException("获取商家数据统计信息失败");
            }
        }


    }

}