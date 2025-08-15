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
        private readonly IDishRepository _dishRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<DishService> _logger;

        public DishService(
            IMerchantRepository merchantRepository,
            ICategoryRepository categoryRepository,
            IDishRepository dishRepository,
            ILogger<DishService> logger
            )
        {
            _merchantRepository = merchantRepository;
            _categoryRepository = categoryRepository;
            _dishRepository = dishRepository;
            _logger = logger;
        }
        
        // 使用MerchantId和DIsh实体新增菜品
        // <returns>true，如果失败则返回false</returns>
        public async Task<bool> CreateDishEntityAsync(
            string merchantId,
            string categoryId,string dishName,
            decimal ?price, decimal originPrice,
            string? coverUrl, string? description,
            int? stockQuantity)
        {
            try
            {
                _logger.LogInformation("开始新增菜品的请求, MerchantId: {MerchantId}", merchantId);

                var merchantEntity = await _merchantRepository.GetByIdAsync(merchantId);

                if (merchantEntity == null)
                {
                    _logger.LogWarning("商家不存在, MerchantId: {MerchantId}", merchantId);
                    throw new NotFoundException(ErrorCodes.MerchantNotFound, $"无相关数据，ID: {merchantId}");
                }

                _logger.LogInformation("成功获取商家详细信息, MerchantId: {MerchantId}", merchantId);
                var categoryEntity = await _categoryRepository.GetByIdAsync(categoryId);
                if (categoryEntity == null)
                {
                    _logger.LogWarning("分类不存在, CategoryId: {CategoryId}",  categoryId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"无相关数据，ID: {categoryId}");
                }

                var dish = new Dish
                {
                    DishId = Guid.NewGuid().ToString("N"),
                    CategoryId = categoryId,
                    DishName = dishName,
                    Price = price??originPrice,
                    OriginPrice = originPrice,
                    CoverUrl = coverUrl??"default",
                    MonthlySales = 0,
                    Rating = 0,
                    OnSale = 0,
                    MerchantId = merchantId,
                    ReviewQuantity = 0,
                    StockQuantity = stockQuantity??1,
                    Merchant = merchantEntity,
                    Category = categoryEntity,
                    Description = description??"这是道好吃的菜",
                };
                var res = await _dishRepository.CreateAsync(dish);
                if (res==null)
                {
                    _logger.LogWarning("创建失败");
                    throw new BusinessException("获取商家数据统计信息失败");
                }
                await _dishRepository.SaveChangesAsync();
                _logger.LogInformation("新增菜品成功, MerchantId: {MerchantId}", merchantId);
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
        public async Task<List<Dish>> GetByFiltersAsync(
            string merchantId,
            string? categoryId,
            bool? orderByRating,
            bool? orderByHighPrice,
            bool? orderByLowPrice,
            int? size, int? page)
        {
            try
            {
                _logger.LogInformation("开始获取商家菜品统计信息, MerchantId: {MerchantId}", merchantId);

                List<Dish> ?data;
                if (categoryId!=null)
                {
                    data = await _dishRepository.GetByMerchantIdAndCategoryIdAsync(merchantId, categoryId,size,page);
                }
                else if (orderByRating.HasValue && orderByRating.Value)
                {
                    data = await _dishRepository.GetByMerchantIdAndOrderByRatingAsync(merchantId, size, page);
                }
                else if (orderByLowPrice.HasValue && orderByLowPrice.Value)
                {
                    data = await _dishRepository.GetByMerchantIdAndOrderByLowPriceAsync(merchantId, size, page);
                }
                else if (orderByHighPrice.HasValue && orderByHighPrice.Value)
                {
                    data = await _dishRepository.GetByMerchantIdAndOrderByHighPriceAsync(merchantId, size, page);
                }
                else
                    data = await _dishRepository.GetByMerchantIdAndOrderByCategoryAsync(merchantId,size,page);

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

        public async Task<bool> ModifyDishEntityAsync(string merchantId, string dishId, 
            string? categoryId, string? dishName,
            decimal? price, decimal? originPrice,
            int? onSale, string? coverUrl,
            string? description,int? stockQuantity)
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

                data.CategoryId = categoryId??data.CategoryId;
                data.DishName = dishName??data.DishName;
                data.CoverUrl = coverUrl??data.CoverUrl;
                data.Price = price??data.Price;
                data.OriginPrice = originPrice??data.OriginPrice;
                data.OnSale = onSale??data.OnSale;
                data.StockQuantity = stockQuantity??data.StockQuantity;
                data.Description = description??data.Description;
                await _dishRepository.SaveChangesAsync();
                
                _logger.LogInformation("成功修改商家菜品信息, DishID: {DishID}", dishId);
              
                return true;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "修改商家菜品时发生异常, DishID: {DishID}", dishId);
                throw new BusinessException("修改商家菜品时失败");
            }
        }

        public async Task<Dish?> GetDisheEntitiesAsync(
            string merchantId,
            string dishId
            )
        {
            try
            {
                _logger.LogInformation("开始获取商家菜品统计信息, MerchantId: {MerchantId}", merchantId);

                var data = await _dishRepository.GetByMerchantIdAndDishIdAsync(merchantId, dishId);

                if (data == null )
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

        public async Task<List<Category>> GetMerchantCategory(string merchantId)
        {
            try
            {
                _logger.LogInformation("开始获取商家菜品统计信息, MerchantId: {MerchantId}", merchantId);

                var data = await _dishRepository.GetByMerchantIdForCategoryAsync(merchantId);

                if (data == null||!data.Any())
                {
                    _logger.LogWarning("无相关数据, MerchantId: {MerchantId}", merchantId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"无相关数据，ID: {merchantId}");
                }
                var categories = data
                    .Select(dish => dish.Category) // 提取分类实体
                    .DistinctBy(cat => cat.CategoryId) // 按分类ID去重
                    .OrderBy(cat => cat.SortOrder) 
                    .ThenBy(cat=>cat.CategoryName)
                    .ToList();

                
                _logger.LogInformation("成功获取商家菜品统计信息, MerchantId: {MerchantId}", merchantId);

                return categories;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取商家菜品统计信息时发生异常, MerchantId: {MerchantId}", merchantId);
                throw new BusinessException("获取商家菜品统计信息失败");
            }
        }

    }

}