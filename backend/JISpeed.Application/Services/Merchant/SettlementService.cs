using JISpeed.Core.Constants;
using JISpeed.Core.Entities.Dish;
using JISpeed.Core.Entities.Merchant;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Interfaces.IRepositories.Merchant;
using JISpeed.Core.Interfaces.IServices;
using Microsoft.Extensions.Logging;

namespace JISpeed.Application.Services.Merchant
{
    public class SettlementService: ISettlementService
    {
        private readonly ILogger<SettlementService> _logger;
        private readonly ISettlementRepository _settlementRepository;
        private readonly IMerchantRepository _merchantRepository;

        public SettlementService(
            ISettlementRepository settlementRepository,
            IMerchantRepository merchantRepository,
            ILogger<SettlementService> logger
        )
        {
            _settlementRepository = settlementRepository;
            _merchantRepository = merchantRepository;
            _logger = logger;
        }

        public async Task<Settlement?> GetDetailBySettlementIdAsync(string settlementId)
        {
            try
            {
                _logger.LogInformation("开始获取结算单详情, SettlementId: {SettlementId}", settlementId);

                var entity = await _settlementRepository.GetWithDetailsAsync(settlementId);

                if (entity == null)
                {
                    _logger.LogWarning("结算单不存在, SettlementId: {SettlementId}", settlementId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"无相关数据，ID: {settlementId}");
                }

                _logger.LogInformation("成功获取结算单详情, SettlementId: {SettlementId}", settlementId);
               return entity;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取结算单详情时发生异常, SettlementId: {SettlementId}", settlementId);
                throw new BusinessException("获取结算单详情失败");
            }
        }

        public async Task<List<Settlement>> GetSettlementListByFiltersAsync(
            string? merchantId,
            DateTime? startDate,DateTime? endDate,
            bool? isSettled,
            int? size, int?page)
        {
            try
            {
                _logger.LogInformation("开始获取结算单列表");
                List<Settlement>? settlements;
                if (merchantId != null)
                {
                    // 用于商家获取结算单
                    var res = await _merchantRepository.ExistsAsync(merchantId);
                    if (res == false)
                    {
                        _logger.LogWarning("商家不存在, MerchantId: {MerchantId}", merchantId);
                        throw new NotFoundException(ErrorCodes.MerchantNotFound, $"无相关数据，ID: {merchantId}");
                    }
                    
                    if (startDate != null || endDate != null)
                    {
                        var start = startDate ?? DateTime.MinValue;
                        var end = endDate ?? DateTime.Now;
                        settlements = await _settlementRepository.GetByMerchantIdAndTimeRangeAsync(merchantId, start, end, size, page);
                    }
                    else if (isSettled.HasValue && isSettled.Value)
                        settlements =
                            await _settlementRepository.GetSettledSettlementsByMerchantIdAsync(merchantId, size, page);
                    else if (isSettled.HasValue && !isSettled.Value)
                        settlements =
                            await _settlementRepository.GetPendingSettlementsByMerchantIdAsync(merchantId, size, page);
                    else
                        settlements =
                            await _settlementRepository.GetByMerchantIdAsync(merchantId, size, page);
                }
                else
                {
                    // 用于管理员获取结算单
                    if (startDate != null || endDate != null)
                    {
                        var start = startDate ?? DateTime.MinValue;
                        var end = endDate ?? DateTime.Now;
                        settlements = await _settlementRepository.GetByPeriodRangeAsync(start, end, size, page);
                    }
                    else if (isSettled.HasValue && isSettled.Value)
                        settlements =
                            await _settlementRepository.GetSettledSettlementsAsync(size, page);
                    else if (isSettled.HasValue && !isSettled.Value)
                        settlements =
                            await _settlementRepository.GetPendingSettlementsAsync(size, page);
                    else
                        settlements =
                            await _settlementRepository.GetAllAsync(size, page);
                }
                _logger.LogInformation("成功获取结算单列表");
                return settlements;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取结算单列表时发生异常");
                throw new BusinessException("获取结算单列表失败");
            }
        }


    }
}