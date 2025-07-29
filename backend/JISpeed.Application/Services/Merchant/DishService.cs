using JISpeed.Core.Entities.Merchant;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Core.Constants;
using JISpeed.Core.Entities.Dish;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Interfaces.IRepositories.Dish;
using JISpeed.Core.Interfaces.IRepositories.Merchant;
using Microsoft.Extensions.Logging;
using MerchantEntity = JISpeed.Core.Entities.Merchant.Merchant;

namespace JISpeed.Application.Services.Merchant
{
    public class DishService : IDishService
    {
        private readonly IMerchantRepository _merchantRepository;
        private readonly ISalesStatRepository _salesStatRepository;
        private readonly IDishRepository _dishRepository;
        private readonly ILogger<DishService> _logger;

        public DishService(
            IMerchantRepository merchantRepository,
            ISalesStatRepository salesStatRepository,
            IDishRepository dishRepository,
            ILogger<DishService> logger
            )
        {
            _merchantRepository = merchantRepository;
            _salesStatRepository = salesStatRepository;
            _dishRepository = dishRepository;
            _logger = logger;
        }
        
        // 使用MerchantId和DIsh实体新增菜品
        // <returns>true，如果失败则返回false</returns>
        public async Task<bool> CreateDishEntityAsync(string merchantId, Dish dish)
        {
            try
            {
                _logger.LogInformation("开始新增菜品的请求, MerchantId: {MerchantId}", merchantId);

                var user = await _merchantRepository.GetByIdAsync(merchantId);

                if (user == null)
                {
                    _logger.LogWarning("商家不存在, MerchantId: {MerchantId}", merchantId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"无相关数据，ID: {merchantId}");
                }

                _logger.LogInformation("成功获取商家详细信息, MerchantId: {MerchantId}, MerchantName: {MerchantName}",
                    merchantId, user.MerchantName);

                var res = await _dishRepository.CreateAsync(dish);
                if (res==null)
                {
                    _logger.LogWarning("创建失败");
                    throw new BusinessException("获取商家数据统计信息失败");
                }
                
                await _dishRepository.SaveChangesAsync();
                _logger.LogInformation("新增菜品成功, MerchantId: {MerchantId}, MerchantName: {MerchantName}",
                    merchantId, user.MerchantName);
                return true;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "新增菜品时发生异常, MerchantId: {MerchantId}", merchantId);
                throw new BusinessException("获取商家数据统计信息失败");
            }
        }
        
        // 使用MerchantId获取商家菜品统计信息列表
        // <returns>实体列表，如果不存在则返回空列表</returns>
        public async Task<List<Dish>> GetDisheEntitiesAsync(string merchantId)
        {
            try
            {
                _logger.LogInformation("开始获取商家菜品统计信息, MerchantId: {MerchantId}", merchantId);

                var data = await _dishRepository.GetByMerchantIdAsync(merchantId);

                if (data == null ||!data.Any())
                {
                    _logger.LogWarning("无相关数据, MerchantId: {MerchantId}", merchantId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"无相关数据，ID: {merchantId}");
                }

                _logger.LogInformation("成功获取商家菜品统计信息, MerchantId: {MerchantId}", merchantId);

                return data;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取商家菜品统计信息时发生异常, MerchantId: {MerchantId}", merchantId);
                throw new BusinessException("获取商家菜品统计信息失败");
            }
        }

        public async Task<bool> DeleteDishEntityAsync(string merchantId, string dishId)
        {
            try
            {
                _logger.LogInformation("开始删除商家菜品统计信息, MerchantId: {MerchantId}，DishID：{DishID}", merchantId,dishId);
                

                var data = await _dishRepository.GetByMerchantIdAndDishIdAsync(merchantId, dishId);

                if (data == null)
                {
                    _logger.LogWarning("无相关数据, MerchantId: {MerchantId}，DishID：{DishID}", merchantId,dishId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"无相关数据，DishID: {dishId}");
                }

                var res =  await _dishRepository.DeleteAsync(dishId);
                _logger.LogInformation("成功删除商家菜品统计信息, DishID: {DishID}", dishId);
                await _dishRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "删除商家菜品时发生异常, DishID: {DishID}", dishId);
                throw new BusinessException("删除商家菜品时失败");
            }
        }

        public async Task<bool> ModifyDishEntityAsync(string merchantId, string dishId, Dish dish)
        {
            try
            {
                _logger.LogInformation("开始修改商家菜品统计信息, MerchantId: {MerchantId}，DishID：{DishID}", merchantId,dishId);

                var data = await _dishRepository.GetByMerchantIdAndDishIdAsync(merchantId, dishId);

                if (data == null)
                {
                    _logger.LogWarning("无相关数据, MerchantId: {MerchantId}，DishID：{DishID}", merchantId,dishId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"无相关数据，DishID: {dishId}");
                }

                data.CategoryId = dish.CategoryId;
                data.DishName = dish.DishName;
                data.CoverUrl = dish.CoverUrl;
                data.Price = dish.Price;
                data.OriginPrice = dish.OriginPrice;
                data.OnSale = dish.OnSale;
                
                
                _logger.LogInformation("成功修改商家菜品统计信息, DishID: {DishID}", dishId);
                await _dishRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "修改商家菜品时发生异常, DishID: {DishID}", dishId);
                throw new BusinessException("修改商家菜品时失败");
            }
        }

    }

}