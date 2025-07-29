using JISpeed.Core.Entities.Merchant;
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

        // 使用MerchantId获取商家详细信息
        // <returns>用户实体，如果不存在则返回null</returns>
        public async Task<MerchantEntity> GetMerchantDetailAsync(string merchantId)
        {
            try
            {
                _logger.LogInformation("开始获取商家详细信息, MerchantId: {MerchantId}", merchantId);

                var user = await _merchantRepository.GetByIdAsync(merchantId);

                if (user == null)
                {
                    _logger.LogWarning("商家不存在, MerchantId: {MerchantId}", merchantId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"商家不存在，ID: {merchantId}");
                }

                _logger.LogInformation("成功获取商家详细信息, MerchantId: {MerchantId}, MerchantName: {MerchantName}",
                    merchantId, user.MerchantName);

                return user;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取商家详细信息时发生异常, MerchantId: {MerchantId}", merchantId);
                throw new BusinessException("获取商家信息失败");
            }
        }
        // 使用MerchantId获取商家数据统计信息列表
        // <returns>实体列表，如果不存在则返回空列表</returns>
        public async Task<List<SalesStat>> GetSalesStateAsync(string merchantId)
        {
            try
            {
                _logger.LogInformation("开始获取商家数据统计信息, MerchantId: {MerchantId}", merchantId);
                

                var data = await _salesStatRepository.GetByMerchantIdAsync(merchantId);

                if (data == null ||!data.Any())
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