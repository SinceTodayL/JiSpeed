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
    [Route("api/")]

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
        [HttpGet("merchants/{merchantId}")]
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

        [HttpGet("merchants")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<MerchantDto>>>> GetAllMerchant(
            [FromQuery] string ?merchantName,
            [FromQuery] string? location,
            [FromQuery] int ?size, [FromQuery] int ?page
            )
        {
            try
            {
                _logger.LogInformation("收到获取获取商家列表信息请求");
                // 获取商家详细信息
                var data = await _merchantService.GetMerchantByFiltersAsync(size,page,merchantName,location);
                // 转换为DTO
                var response = _mapper.Map<List<MerchantDto>>(data);

                _logger.LogInformation("成功获取用户详细信息");

                return Ok(ApiResponse<List<MerchantDto>>.Success(response, "商家信息获取成功"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败");
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "商家不存在");
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.ResourceNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取商家详细信息时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
        [HttpGet("merchants/autocomplete")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<string>>> GetNameSuggestionsAsync(string prefix, int ?limit)
        {
            try
            {
                _logger.LogInformation("收到智能补全请求");
                if (prefix.Length <= 1)
                    throw new ValidationException("请加长前缀长度来启用智能匹配");
                var response = await _merchantService.GetMerchantNameForSearchAsync(prefix, limit);

                return Ok(ApiResponse<List<string>>.Success(response,"智能补全成功"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败");
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "智能补全时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
        
        
        // 根据商家ID修改商家详细信息
        [HttpPatch("merchants/{merchantId}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateMerchantDetail(
            string merchantId,
            [FromBody] UpdateMerchantDto request)
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
                var entity = _mapper.Map<UpdateMerchantDto>(request);

                // 修改商家信息
                await _merchantService.UpdateMerchantDetailAsync(merchantId, entity.MerchantName, entity.Status, entity.ContactInfo, entity.Location, entity.Description);
                
                _logger.LogInformation("成功修改用户详细信息, MerchantId: {MerchantId}", merchantId);

                return Ok(ApiResponse<bool>.Success(true, "商家信息修改成功"));
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

        
    }
    

}

