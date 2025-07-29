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

    public class MerchantController : ControllerBase
    {
        private readonly IMerchantService _merchantService;
        private readonly IMapper _mapper;
        private readonly ILogger<MerchantController> _logger;

        public MerchantController(
            IMerchantService merchantService,
            ILogger<MerchantController> logger,
            IMapper mapper
        )
        {
            _merchantService = merchantService;
            _logger = logger;
            _mapper = mapper;
        }

        // 根据商家ID获取商家详细信息
        [HttpGet("{merchantId}")]
        public async Task<ActionResult<ApiResponse<MerchantDto>>> GetMerchantDetail(string merchantId)
        {
            try
            {
                _logger.LogInformation("收到获取商家详细信息请求, MerchantID: {MerchantID}", merchantId);

                // 参数验证
                if (string.IsNullOrWhiteSpace(merchantId))
                {
                    _logger.LogWarning("商家ID参数为空");
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.MissingParameter,
                        "商家ID不能为空"));
                }
                // 获取商家详细信息
                var user = await _merchantService.GetMerchantDetailAsync(merchantId);
                // 转换为DTO
                var merchantDetailDto = _mapper.Map<MerchantDto>(user);

                _logger.LogInformation("成功获取用户详细信息, MerchantId: {MerchantId}, MerchantName: {MerchantName}",
                    user.MerchantId, user.MerchantName);

                return Ok(ApiResponse<MerchantDto>.Success(merchantDetailDto, "商家信息获取成功"));
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
                _logger.LogError(ex, "获取商家详细信息时发生未知异常, MerchantId: {MerchantId}", merchantId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }

        [HttpGet("{merchantId}/SalesStat")]
        public async Task<ActionResult<ApiResponse<MerchantDto>>> GetMerchantSalesStat(string merchantId)
        {
            try
            {
                _logger.LogInformation("收到获取商家数据统计请求, MerchantID: {MerchantID}", merchantId);
                // 参数验证
                if (string.IsNullOrWhiteSpace(merchantId))
                {
                    _logger.LogWarning("商家ID参数为空");
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.MissingParameter,
                        "商家ID不能为空"));
                }

                await _merchantService.GetMerchantDetailAsync(merchantId);

                var data = await _merchantService.GetSalesStateAsync(merchantId);

                var dataList = _mapper.Map<List<SalesStatDto>>(data) ?? new List<SalesStatDto>();
                _logger.LogInformation("成功获取用户详细信息, MerchantId: {MerchantId}", merchantId);

                return Ok(ApiResponse<List<SalesStatDto>>.Success(dataList, "商家信息获取成功"));
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
                _logger.LogWarning(ex, "无相关数据, MerchantId: {MerchantId}", merchantId);
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
    }

    }

