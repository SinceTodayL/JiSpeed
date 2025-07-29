using AutoMapper;
using JISpeed.Api.Common;
using JISpeed.Api.DTOs;
using JISpeed.Core.Constants;
using JISpeed.Core.Entities.Dish;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace JISpeed.Api.Controllers
{
    [ApiController]
    [Route("api/merchant/")]

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
        
        [HttpGet("{merchantId}/getAllDishes")]
        public async Task<ActionResult<ApiResponse<DishesDto>>> GetMerchantAllDishes(string merchantId)
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
                var data = await _dishService.GetDisheEntitiesAsync(merchantId);
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
                    ErrorCodes.ResourceNotFound,
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
        public async Task<ActionResult<ApiResponse<bool>>> AddNewDish(string merchantId, [FromBody] DishesDto dto)
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

                var dish = _mapper.Map<Dish>(dto);
                dish.DishId = Guid.NewGuid().ToString("N");
                dish.MerchantId = merchantId;

                var res = await _dishService.CreateDishEntityAsync(merchantId, dish);
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
                    ErrorCodes.ResourceNotFound,
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
        public async Task<ActionResult> DeleteDish(string merchantId, string dishId)
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
                    ErrorCodes.ResourceNotFound,
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
        [HttpPut("{merchantId}/{dishId}")]
        public async Task<ActionResult> ModifyDish(string merchantId, string dishId,[FromBody] DishesDto dto)
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
                
                var dish = _mapper.Map<Dish>(dto);
                var res = await _dishService.ModifyDishEntityAsync(merchantId, dishId, dish);
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
                    ErrorCodes.ResourceNotFound,
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
    }

    }

