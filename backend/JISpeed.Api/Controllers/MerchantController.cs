using AutoMapper;
using JISpeed.Api.Common;
using JISpeed.Api.DTOs;
using JISpeed.Core.Constants;
using JISpeed.Core.Entities.Dish;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Api.Controllers
{
    [ApiController]
    [Route("api/")]

    public class MerchantController : ControllerBase
    {
        private readonly IMerchantService _merchantService;
        private readonly IMapper _mapper;
        private readonly ILogger<MerchantController> _logger;
        private readonly OracleDbContext _context;

        public MerchantController(
            IMerchantService merchantService,
            ILogger<MerchantController> logger,
            IMapper mapper,
            OracleDbContext context
        )
        {
            _merchantService = merchantService;
            _logger = logger;
            _mapper = mapper;
            _context = context;
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
            [FromQuery] string? merchantName,
            [FromQuery] string? location,
            [FromQuery] int? status,
            [FromQuery] int? size, [FromQuery] int? page
            )
        {
            try
            {
                _logger.LogInformation("收到获取获取商家列表信息请求");
                // 获取商家详细信息
                var data = await _merchantService.GetMerchantByFiltersAsync(size, page, status, merchantName, location);
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
       
        [HttpGet("merchants/byTag")]
        public async Task<ActionResult<ApiResponse<List<MerchantDtoByTag>>>> GetMerchantByTagAsync(
            [FromQuery] int tag,[FromQuery] string addressId,
            [FromQuery] int? size, [FromQuery] int? page
        )
        {
            try
            {
                _logger.LogInformation("收到获取获取商家列表信息请求");
                // 获取商家详细信息
                var data = await _merchantService.GetMerchantByTagAsync(size, page,tag,addressId);
                // 转换为DTO
                var response = _mapper.Map<List<MerchantDtoByTag>>(data);

                _logger.LogInformation("成功获取商家信息");

                return Ok(ApiResponse<List<MerchantDtoByTag>>.Success(response, "商家信息获取成功"));
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
        public async Task<ActionResult<ApiResponse<string>>> GetNameSuggestionsAsync(string prefix, int? limit)
        {
            try
            {
                _logger.LogInformation("收到智能补全请求");
                if (prefix.Length <= 1)
                    throw new ValidationException("请加长前缀长度来启用智能匹配");
                var response = await _merchantService.GetMerchantNameForSearchAsync(prefix, limit);

                return Ok(ApiResponse<List<string>>.Success(response, "智能补全成功"));
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
        
        /// 根据商家id获取所有评论
        [HttpGet("merchants/{merchantId}/reviews")]
        public async Task<ActionResult<ApiResponse<List<DishReviewDto>>>> GetReviewsByMerchantId(
            string merchantId,
            [FromQuery] int size = 10,
            [FromQuery] int page = 1)
        {
            try
            {
                _logger.LogInformation("收到获取商家评论请求, MerchantID: {MerchantID}", merchantId);

                // 参数验证
                if (string.IsNullOrWhiteSpace(merchantId))
                {
                    _logger.LogWarning("商家ID参数为空");
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.MissingParameter,
                        "商家ID不能为空"));
                }

                if (page < 1) page = 1;
                if (size < 1) size = 10;
                
                var query = _context.Reviews
                    .Where(r => r.Order.MerchantId == merchantId) // 通过 Order 导航属性筛选商家ID
                    .Include(r => r.User) // 加载评论对应的用户信息
                    .Include(r => r.DishReviews) // 加载评论与菜品的联结表
                        .ThenInclude(dr => dr.Dish); // 加载联结表对应的菜品信息

                // 2. 应用排序和分页
                var reviews = await query
                    .OrderByDescending(r => r.ReviewAt) // 按评论时间降序排序
                    .Skip((page - 1) * size) // 跳过指定数量的记录
                    .Take(size) // 取出指定数量的记录
                    .ToListAsync(); // 执行查询

                // 3. 将实体映射为 DTO
                var reviewDtos = reviews.Select(r => new DishReviewDto
                {
                    ReviewId = r.ReviewId,
                    OrderId = r.OrderId,
                    UserId = r.UserId,
                    Rating = r.Rating,
                    Content = r.Content,
                    IsAnonymous = r.IsAnonymous,
                    ReviewAt = r.ReviewAt,
                    // 根据是否匿名设置用户昵称和头像，注意处理 user 可能为 null 的情况
                    UserNickname = r.IsAnonymous == 1 ? "匿名用户" : (r.User?.Nickname ?? "未知用户"),
                    UserAvatarUrl = r.IsAnonymous == 1 ? "/images/default_avatar.png" : (r.User?.AvatarUrl ?? string.Empty)
                }).ToList();

                _logger.LogInformation("成功获取商家评论, MerchantId: {MerchantId}", merchantId);

                return Ok(ApiResponse<List<DishReviewDto>>.Success(reviewDtos, "商家评论获取成功"));
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
                _logger.LogError(ex, "获取商家评论时发生未知异常, MerchantId: {MerchantId}", merchantId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }  
    }
}

