using AutoMapper;
using JISpeed.Api.Common;
using JISpeed.Api.DTOs;
using JISpeed.Core.Constants;
using JISpeed.Core.Entities.Dish;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JISpeed.Api.Controllers
{
    [ApiController]
    [Route("api/merchants")]

    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;
        private readonly IMapper _mapper;
        private readonly ILogger<DishController> _logger;

        public DishController(
            ILogger<DishController> logger,
            IDishService dishService,
            IMapper mapper
        )
        {
            _dishService = dishService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("{merchantId}/dishesByCategory")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<CategoryWithDishesDto>>>> GetMerchantAllDishes(string merchantId)
        {
            try
            {
                _logger.LogInformation("收到获取商家菜品数据统计请求, MerchantID: {MerchantID}", merchantId);
                // 参数验证
                if (string.IsNullOrWhiteSpace(merchantId))
                {
                    _logger.LogWarning("商家ID参数为空");
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.MissingParameter,
                        "商家ID不能为空"));
                }
                var dishes = await _dishService.GetByFiltersAsync(merchantId,null,null,null,null,null,null);
                var groupedResult = dishes
                    .GroupBy(dish => new 
                    { 
                        dish.CategoryId, 
                        CategoryName = dish.Category?.CategoryName ?? "未分类" // 处理分类为空的情况
                    })
                    .Select(group => new CategoryWithDishesDto
                    {
                        CategoryId = group.Key.CategoryId,
                        CategoryName = group.Key.CategoryName,
                        // 将分组内的菜品转换为内层DTO
                        Dishes = group.Select(dish => new DishesDto
                        {
                            DishId = dish.DishId,
                            CategoryId = dish.CategoryId,
                            DishName = dish.DishName,
                            Price = dish.Price,
                            OriginPrice = dish.OriginPrice,
                            CoverUrl = dish.CoverUrl,
                            MonthlySales = dish.MonthlySales,
                            Rating = dish.Rating,
                            OnSale = dish.OnSale,
                            MerchantId = dish.MerchantId,
                            ReviewQuantity = dish.ReviewQuantity,
                        }).ToList()
                    })
                    .OrderBy(c => c.CategoryName) // 按分类名称排序
                    .ToList();              
                _logger.LogInformation("成功获取用户详细信息, MerchantId: {MerchantId}", merchantId);
                return Ok(ApiResponse<List<CategoryWithDishesDto>>.Success(groupedResult,"商家菜品信息获取成功"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败, MerchantId: {MerchantId}", merchantId);
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "商家不存在, MerchantId: {MerchantId}", merchantId);
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.MerchantNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异常, MerchantId: {MerchantId}", merchantId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取商家数据统计信息时发生未知异常, MerchantId: {MerchantId}", merchantId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
        [HttpGet("{merchantId}/dishes")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<DishesDto>>>> GetMerchantAllDishes(
            string merchantId,
            [FromQuery]string? categoryId,
            [FromQuery]bool? orderByRating,
            [FromQuery]bool? orderByHighPrice,
            [FromQuery]bool? orderByLowPrice,
            [FromQuery]int? size,[FromQuery]int? page)
        {
            try
            {
                _logger.LogInformation("收到获取商家菜品数据统计请求, MerchantID: {MerchantID}", merchantId);
                // 参数验证
                if (string.IsNullOrWhiteSpace(merchantId))
                {
                    _logger.LogWarning("商家ID参数为空");
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.MissingParameter,
                        "商家ID不能为空"));
                }
                var data = await _dishService.GetByFiltersAsync(merchantId,categoryId,orderByRating,orderByHighPrice,orderByLowPrice, size, page);
                var dataList = _mapper.Map<List<DishesDto>>(data) ?? new List<DishesDto>();
                _logger.LogInformation("成功获取用户详细信息, MerchantId: {MerchantId}", merchantId);
                return Ok(ApiResponse<List<DishesDto>>.Success(dataList, "商家菜品信息获取成功"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败, MerchantId: {MerchantId}", merchantId);
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "商家不存在, MerchantId: {MerchantId}", merchantId);
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.MerchantNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异常, MerchantId: {MerchantId}", merchantId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取商家数据统计信息时发生未知异常, MerchantId: {MerchantId}", merchantId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }

        [HttpGet("{merchantId}/dish/{dishId}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<DishesDto>>> GetDishes(
            string merchantId,string dishId)
        {
            try
            {
                _logger.LogInformation("收到获取商家查看菜品请求, MerchantID: {MerchantID}", merchantId);
                // 参数验证
                if (string.IsNullOrWhiteSpace(merchantId)&&string.IsNullOrWhiteSpace(dishId))
                {
                    _logger.LogWarning("参数为空");
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.MissingParameter,
                        "参数能为空"));
                }
                var data = await _dishService.GetDisheEntitiesAsync(merchantId,dishId);
                if (data == null)
                {
                    return NotFound(ApiResponse<object>.Fail(
                        ErrorCodes.ProductNotFound,
                        "菜品未找到"));
                }
                var response = _mapper.Map<DishesDto>(data);
                _logger.LogInformation("成功获取菜品详细信息, MerchantId: {MerchantId}", merchantId);
                return Ok(ApiResponse<DishesDto>.Success(response, "商家菜品信息获取成功"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败, MerchantId: {MerchantId}", merchantId);
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "商家不存在, MerchantId: {MerchantId}", merchantId);
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.MerchantNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异常, MerchantId: {MerchantId}", merchantId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取商家数据统计信息时发生未知异常, MerchantId: {MerchantId}", merchantId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
        
        
        [HttpPost("{merchantId}/addNewDish")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> AddNewDish(string merchantId, [FromBody] CreateDishesDto dto)
        {
            try
            {
                _logger.LogInformation("收到商家新增菜品的请求, MerchantID: {MerchantID}", merchantId);
                // 参数验证
                if (string.IsNullOrWhiteSpace(merchantId))
                {
                    _logger.LogWarning("商家ID参数为空");
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.MissingParameter,
                        "商家ID不能为空"));
                }

                var res = await _dishService.CreateDishEntityAsync(merchantId, dto.CategoryId, dto.DishName,dto.Price,dto.OriginPrice,dto.CoverUrl);
                _logger.LogInformation("成功新增菜品, MerchantId: {MerchantId}", merchantId);
                return Ok(ApiResponse<bool>.Success(res));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败, MerchantId: {MerchantId}", merchantId);
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "商家不存在, MerchantId: {MerchantId}", merchantId);
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.MerchantNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异常, MerchantId: {MerchantId}", merchantId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "新增菜品时发生未知异常, MerchantId: {MerchantId}", merchantId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }

        [HttpDelete("{merchantId}/{dishId}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteDish(string merchantId, string dishId)
        {
            try
            {
                _logger.LogInformation("收到商家删除菜品的请求, MerchantID: {MerchantID}，DishID：{DishID}", merchantId,dishId);
                // 参数验证
                if (string.IsNullOrWhiteSpace(merchantId))
                {
                    _logger.LogWarning("商家ID参数为空");
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.MissingParameter,
                        "商家ID不能为空"));
                }

                if (string.IsNullOrWhiteSpace(dishId))
                {
                    _logger.LogWarning("菜品ID参数为空");
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.MissingParameter,
                        "菜品ID不能为空"));
                }
                var res = await _dishService.DeleteDishEntityAsync(merchantId, dishId);

                _logger.LogInformation("成功删除菜品, DishId: {DishId}", dishId);

                return Ok(ApiResponse<bool>.Success(res));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败, MerchantId: {MerchantId}，DishId: {DishId}", merchantId,dishId);
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "商家或菜品不存在, MerchantId: {MerchantId}，DishId: {DishId}", merchantId,dishId);
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.MerchantNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异常，MerchantId: {MerchantId}，DishId: {DishId}", merchantId,dishId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除菜品时发生未知异常，MerchantId: {MerchantId}，DishId: {DishId}", merchantId,dishId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
        [HttpPatch("{merchantId}/{dishId}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> ModifyDish(
            string merchantId, string dishId,
            [FromBody] UpdateDishesDto dto)
        {
            try
            {
                _logger.LogInformation("收到商家修改菜品的请求, MerchantID: {MerchantID}，DishID：{DishID}", merchantId,dishId);
                // 参数验证
                if (string.IsNullOrWhiteSpace(merchantId))
                {
                    _logger.LogWarning("商家ID参数为空");
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.MissingParameter,
                        "商家ID不能为空"));
                }

                if (string.IsNullOrWhiteSpace(dishId))
                {
                    _logger.LogWarning("菜品ID参数为空");
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.MissingParameter,
                        "菜品ID不能为空"));
                }
                
                var res = await _dishService.ModifyDishEntityAsync(merchantId, dishId,dto.CategoryId, dto.DishName,dto.Price,dto.OriginPrice,dto.OnSale,dto.CoverUrl);
                _logger.LogInformation("成功修改菜品, DishId: {DishId}", dishId);

                return Ok(ApiResponse<bool>.Success(res));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败, MerchantId: {MerchantId}，DishId: {DishId}", merchantId,dishId);
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "商家或菜品不存在, MerchantId: {MerchantId}，DishId: {DishId}", merchantId,dishId);
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.MerchantNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异常，MerchantId: {MerchantId}，DishId: {DishId}", merchantId,dishId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "修改菜品时发生未知异常，MerchantId: {MerchantId}，DishId: {DishId}", merchantId,dishId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
        
        [HttpGet("{merchantId}/dish-categories")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> GetMerchantAllCategories(string merchantId)
        {
            try
            {
                _logger.LogInformation("收到查看分类请求, MerchantID: {MerchantID}", merchantId);
                // 参数验证
                if (string.IsNullOrWhiteSpace(merchantId))
                {
                    _logger.LogWarning("参数为空");
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.MissingParameter,
                        "参数能为空"));
                }
                var data = await _dishService.GetMerchantCategory(merchantId);
                if (!data.Any())
                {
                    return NotFound(ApiResponse<object>.Fail(
                        ErrorCodes.ProductNotFound,
                        "无分类"));
                }
                var response = _mapper.Map<List<CategoryDto>>(data);
                _logger.LogInformation("成功获取商家分类信息, MerchantId: {MerchantId}", merchantId);
                return Ok(ApiResponse<List<CategoryDto>>.Success(response, "商家分类信息获取成功"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败, MerchantId: {MerchantId}", merchantId);
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "商家不存在, MerchantId: {MerchantId}", merchantId);
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.MerchantNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异常, MerchantId: {MerchantId}", merchantId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取商家分类信息时发生未知异常, MerchantId: {MerchantId}", merchantId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
    }
   

    }

