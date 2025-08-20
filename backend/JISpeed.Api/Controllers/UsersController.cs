using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using JISpeed.Api.DTOs;
using JISpeed.Api.DTOS;
using JISpeed.Api.Common;
using JISpeed.Api.Mappers;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Entities.User;
using JISpeed.Core.Entities.Order;
using JISpeed.Core.Interfaces.IRepositories.User;
using JISpeed.Core.Interfaces.IRepositories.Order;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Constants;
using System.ComponentModel.DataAnnotations;
using ValidationException = JISpeed.Core.Exceptions.ValidationException;

namespace JISpeed.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IComplaintRepository _complaintRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(
            ILogger<UsersController> logger,
            IUserService userService,
            IUserRepository userRepository,
            IAddressRepository addressRepository,
            IReviewRepository reviewRepository,
            IComplaintRepository complaintRepository,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userService = userService;
            _userRepository = userRepository;
            _addressRepository = addressRepository;
            _reviewRepository = reviewRepository;
            _complaintRepository = complaintRepository;
            _userManager = userManager;
        }

        /// <summary>
        /// 根据ID获取用户信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户详细信息</returns>
        [HttpGet("{userId}")]
        public async Task<ApiResponse<UserDetailDto>> GetUserById(string userId)
        {
            try
            {
                var user = await _userRepository.GetWithDetailsAsync(userId);
                if (user == null)
                {
                    throw UserExceptions.UserNotFound(userId);
                }

                var stats = await GetUserStatsAsync(userId);
                var userDto = UserMapper.ToUserDetailDto(user, stats);

                return ApiResponse<UserDetailDto>.Success(userDto);
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is BusinessException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取用户信息时发生异常，UserId: {UserId}", userId);
                throw new BusinessException("获取用户信息失败");
            }
        }

        /// 获取用户信息列表
        /// <param name="size">每页大小</param>
        /// <param name="page">页码</param>
        /// <returns>用户信息列表</returns>
        [HttpGet]
        public async Task<ApiResponse<List<UserDetailDto>>> GetUsers([FromQuery] int? size, [FromQuery] int? page)
        {
            try
            {
                var users = await _userRepository.GetAllUsersAsync(size ?? 10, page ?? 1);
                var userDtos = UserMapper.ToUserDetailDtoList(users);

                return ApiResponse<List<UserDetailDto>>.Success(userDtos);
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is BusinessException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取用户列表时发生异常");
                throw new BusinessException("获取用户列表失败");
            }
        }

        /// 根据ID获取用户收藏的商品列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>收藏商品列表</returns>
        [HttpGet("{userId}/favorites")]
        public async Task<ApiResponse<List<UserFavoriteDto>>> GetUserFavorites(string userId,
        [FromQuery] int ?size, [FromQuery] int ?page)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw UserExceptions.UserNotFound(userId);
                }

                var userWithDetails = await _userRepository.GetWithDetailsAsync(userId);
                var allFavorites = userWithDetails?.Favorites ?? new List<Favorite>();
                var paginatedFavorites = allFavorites
                    .Skip(((page ?? 1) - 1) * (size ?? 10))
                    .Take(size ?? 10)
                    .ToList();
                var favoriteDtos = UserMapper.ToUserFavoriteDtoList(paginatedFavorites);

                return ApiResponse<List<UserFavoriteDto>>.Success(favoriteDtos);
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is BusinessException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取用户收藏列表时发生异常，UserId: {UserId}", userId);
                throw new BusinessException("获取用户收藏列表失败");
            }
        }

        /// 根据ID获取用户的所有地址列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>地址列表</returns>
        [HttpGet("{userId}/addresses")]
        public async Task<ApiResponse<List<UserAddressDto>>> GetUserAddresses(string userId,
        [FromQuery] int? size, [FromQuery] int? page)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw UserExceptions.UserNotFound(userId);
                }

                var addresses = await _addressRepository.GetByUserIdAsync(userId);
                var paginatedAddresses = addresses
                    .Skip(((page ?? 1) - 1) * (size ?? 10))
                    .Take(size ?? 10)
                    .ToList();
                var addressDtos = UserMapper.ToUserAddressDtoList(paginatedAddresses);

                return ApiResponse<List<UserAddressDto>>.Success(addressDtos);
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is BusinessException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取用户地址列表时发生异常，UserId: {UserId}", userId);
                throw new BusinessException("获取用户地址列表失败");
            }
        }

        /// 根据ID获取用户的购物车内容
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>购物车内容</returns>
        [HttpGet("{userId}/cart")]
        public async Task<ApiResponse<List<UserCartItemDto>>> GetUserCart(string userId,
        [FromQuery] int? size, [FromQuery] int? page)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw UserExceptions.UserNotFound(userId);
                }

                var userWithDetails = await _userRepository.GetWithDetailsAsync(userId);
                var cartItems = userWithDetails?.CartItems ?? new List<CartItem>();

                var paginatedCartItems = cartItems
                    .Skip(((page ?? 1) - 1) * (size ?? 10))
                    .Take(size ?? 10)
                    .ToList();
                var cartItemDtos = UserMapper.ToUserCartItemDtoList(paginatedCartItems);

                return ApiResponse<List<UserCartItemDto>>.Success(cartItemDtos);
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is BusinessException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取用户购物车时发生异常，UserId: {UserId}", userId);
                throw new BusinessException("获取用户购物车失败");
            }
        }

        /// 根据ID获取用户发布的所有评论
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>评论列表</returns>
        [HttpGet("{userId}/review")]
        public async Task<ApiResponse<List<UserReviewDto>>> GetUserReviews(string userId,
        [FromQuery] int? size, [FromQuery] int? page)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw UserExceptions.UserNotFound(userId);
                }

                var reviews = await _reviewRepository.GetByUserIdAsync(userId);
                var paginatedReviews = reviews
                    .Skip(((page ?? 1) - 1) * (size ?? 10))
                    .Take(size ?? 10)
                    .ToList();
                var reviewDtos = UserMapper.ToUserReviewDtoList(paginatedReviews);

                return ApiResponse<List<UserReviewDto>>.Success(reviewDtos);
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is BusinessException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取用户评论时发生异常，UserId: {UserId}", userId);
                throw new BusinessException("获取用户评论失败");
            }
        }

        /// 根据ID获取用户提交的所有投诉
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>投诉列表</returns>
        [HttpGet("{userId}/complaints")]
        public async Task<ApiResponse<List<UserComplaintDto>>> GetUserComplaints(string userId,
        [FromQuery] int? size, [FromQuery] int? page)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw UserExceptions.UserNotFound(userId);
                }

                var complaints = await _complaintRepository.GetByUserIdAsync(userId);
                var paginatedComplaints = complaints
                    .Skip(((page ?? 1) - 1) * (size ?? 10))
                    .Take(size ?? 10)
                    .ToList();
                var complaintDtos = UserMapper.ToUserComplaintDtoList(paginatedComplaints);

                return ApiResponse<List<UserComplaintDto>>.Success(complaintDtos);
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is BusinessException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取用户投诉时发生异常，UserId: {UserId}", userId);
                throw new BusinessException("获取用户投诉失败");
            }
        }

        /// <summary>
        /// 用户添加收藏夹
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="request">收藏请求</param>
        /// <returns>操作结果</returns>
        [HttpPost("{userId}/favorites")]
        public async Task<ApiResponse<object>> AddFavorite(string userId, [FromBody] AddFavoriteRequest request)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw UserExceptions.UserNotFound(userId);
                }

                if (string.IsNullOrEmpty(request.DishId))
                {
                    throw new ValidationException("菜品ID不能为空");
                }

                // 检查是否已收藏
                var isAlreadyFavorite = await _userService.IsFavoriteAsync(userId, request.DishId);
                if (isAlreadyFavorite)
                {
                    throw new BusinessException("该菜品已在收藏列表中");
                }

                // 添加收藏
                var success = await _userService.AddFavoriteAsync(userId, request.DishId);
                if (!success)
                {
                    throw new BusinessException("添加收藏失败");
                }

                return ApiResponse<object>.Success(new { }, "添加收藏成功");
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is BusinessException || ex is NotFoundException))
            {
                _logger.LogError(ex, "添加收藏时发生异常，UserId: {UserId}, DishId: {DishId}", userId, request.DishId);
                throw new BusinessException("添加收藏失败");
            }
        }

        /// <summary>
        /// 删除收藏
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="dishId">菜品ID</param>
        /// <returns>操作结果</returns>
        [HttpDelete("{userId}/favorites/{dishId}")]
        public async Task<ApiResponse<object>> RemoveFavorite(string userId, string dishId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw UserExceptions.UserNotFound(userId);
                }

                // 删除收藏
                var success = await _userService.RemoveFavoriteAsync(userId, dishId);
                if (!success)
                {
                    throw new BusinessException("取消收藏失败，该菜品可能未被收藏");
                }

                return ApiResponse<object>.Success(new { }, "取消收藏成功");
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is BusinessException || ex is NotFoundException))
            {
                _logger.LogError(ex, "取消收藏时发生异常，UserId: {UserId}, DishId: {DishId}", userId, dishId);
                throw new BusinessException("取消收藏失败");
            }
        }

        /// <summary>
        /// 添加地址
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="request">地址请求</param>
        /// <returns>操作结果</returns>
        [HttpPost("{userId}/addresses")]
        public async Task<ApiResponse<object>> AddAddress(string userId, [FromBody] AddAddressRequest request)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw UserExceptions.UserNotFound(userId);
                }

                if (string.IsNullOrEmpty(request.ReceiverName))
                    throw new ValidationException("收货人姓名不能为空");
                if (string.IsNullOrEmpty(request.ReceiverPhone))
                    throw new ValidationException("收货人手机号不能为空");
                if (string.IsNullOrEmpty(request.DetailedAddress))
                    throw new ValidationException("详细地址不能为空");

                // 创建地址
                var addressId = await _userService.CreateAddressAsync(
                    userId,
                    request.ReceiverName,
                    request.ReceiverPhone,
                    request.DetailedAddress,
                    request.IsDefault);

                if (addressId == null)
                {
                    throw new BusinessException("添加地址失败");
                }

                return ApiResponse<object>.Success(new { AddressId = addressId }, "添加地址成功");
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is BusinessException || ex is NotFoundException))
            {
                _logger.LogError(ex, "添加地址时发生异常，UserId: {UserId}", userId);
                throw new BusinessException("添加地址失败");
            }
        }

        /// <summary>
        /// 修改地址
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="addressId">地址ID</param>
        /// <param name="request">地址请求</param>
        /// <returns>操作结果</returns>
        [HttpPut("{userId}/addresses/{addressId}")]
        public async Task<ApiResponse<object>> UpdateAddress(string userId, string addressId, [FromBody] AddAddressRequest request)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw UserExceptions.UserNotFound(userId);
                }

                var address = await _addressRepository.GetByIdAsync(addressId);
                if (address == null || address.UserId != userId)
                {
                    throw UserExceptions.UserAddressNotFound(addressId, userId);
                }

                if (string.IsNullOrEmpty(request.ReceiverName))
                    throw new ValidationException("收货人姓名不能为空");
                if (string.IsNullOrEmpty(request.ReceiverPhone))
                    throw new ValidationException("收货人手机号不能为空");
                if (string.IsNullOrEmpty(request.DetailedAddress))
                    throw new ValidationException("详细地址不能为空");

                // 更新地址
                var success = await _userService.UpdateAddressAsync(
                    userId,
                    addressId,
                    request.ReceiverName,
                    request.ReceiverPhone,
                    request.DetailedAddress,
                    request.IsDefault);

                if (!success)
                {
                    throw new BusinessException("修改地址失败");
                }

                return ApiResponse<object>.Success(new { }, "修改地址成功");
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is BusinessException || ex is NotFoundException))
            {
                _logger.LogError(ex, "修改地址时发生异常，UserId: {UserId}, AddressId: {AddressId}", userId, addressId);
                throw new BusinessException("修改地址失败");
            }
        }

        /// <summary>
        /// 删除地址
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="addressId">地址ID</param>
        /// <returns>操作结果</returns>
        [HttpDelete("{userId}/addresses/{addressId}")]
        public async Task<ApiResponse<object>> DeleteAddress(string userId, string addressId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw UserExceptions.UserNotFound(userId);
                }

                var address = await _addressRepository.GetByIdAsync(addressId);
                if (address == null || address.UserId != userId)
                {
                    throw UserExceptions.UserAddressNotFound(addressId, userId);
                }

                // 删除地址
                var success = await _userService.DeleteAddressAsync(userId, addressId);
                if (!success)
                {
                    throw new BusinessException("删除地址失败");
                }

                return ApiResponse<object>.Success(new { }, "删除地址成功");
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is BusinessException || ex is NotFoundException))
            {
                _logger.LogError(ex, "删除地址时发生异常，UserId: {UserId}, AddressId: {AddressId}", userId, addressId);
                throw new BusinessException("删除地址失败");
            }
        }

        /// <summary>
        /// 添加到购物车
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="request">购物车请求</param>
        /// <returns>操作结果</returns>
        [HttpPost("{userId}/cart")]
        public async Task<ApiResponse<object>> AddToCart(string userId, [FromBody] AddToCartRequest request)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw UserExceptions.UserNotFound(userId);
                }

                if (string.IsNullOrEmpty(request.DishId))
                {
                    throw new ValidationException("菜品ID不能为空");
                }

                // 检查购物车中是否已有该商品
                var isInCart = await _userService.IsInCartAsync(userId, request.DishId);
                if (isInCart)
                {
                    throw new BusinessException("该菜品已在购物车中");
                }

                // 添加到购物车
                var cartItemId = await _userService.AddToCartAsync(userId, request.DishId);
                if (cartItemId == null)
                {
                    throw new BusinessException("添加到购物车失败");
                }

                return ApiResponse<object>.Success(new { CartItemId = cartItemId }, "添加到购物车成功");
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is BusinessException || ex is NotFoundException))
            {
                _logger.LogError(ex, "添加到购物车时发生异常，UserId: {UserId}, DishId: {DishId}", userId, request.DishId);
                throw new BusinessException("添加到购物车失败");
            }
        }

        /// <summary>
        /// 从购物车删除商品
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="cartItemId">购物车项ID</param>
        /// <returns>操作结果</returns>
        [HttpDelete("{userId}/cart/{cartItemId}")]
        public async Task<ApiResponse<object>> RemoveFromCart(string userId, string cartItemId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw UserExceptions.UserNotFound(userId);
                }

                // 从购物车删除商品
                var success = await _userService.RemoveFromCartAsync(userId, cartItemId);
                if (!success)
                {
                    throw new BusinessException("从购物车删除失败，该商品可能已被删除");
                }

                return ApiResponse<object>.Success(new { }, "从购物车删除成功");
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is BusinessException || ex is NotFoundException))
            {
                _logger.LogError(ex, "从购物车删除时发生异常，UserId: {UserId}, CartItemId: {CartItemId}", userId, cartItemId);
                throw new BusinessException("从购物车删除失败");
            }
        }

        private async Task<UserStatsDto> GetUserStatsAsync(string userId)
        {
            try
            {
                // 获取用户详细信息，包含所有关联数据
                var userWithDetails = await _userRepository.GetWithDetailsAsync(userId);
                if (userWithDetails == null)
                {
                    return new UserStatsDto();
                }

                // 使用服务层方法获取统计数据
                var favoriteCount = await _userService.GetFavoriteCountAsync(userId);
                var cartItemCount = await _userService.GetCartItemCountAsync(userId);
                var addressCount = await _userService.GetAddressCountAsync(userId);

                // 获取评论和投诉数量
                var reviews = await _reviewRepository.GetByUserIdAsync(userId);
                var complaints = await _complaintRepository.GetByUserIdAsync(userId);

                return UserMapper.ToUserStatsDto(
                    totalOrders: userWithDetails.Orders?.Count ?? 0,
                    favoriteCount: favoriteCount,
                    cartItemCount: cartItemCount,
                    availableCouponCount: userWithDetails.Coupons?.Count ?? 0,
                    addressCount: addressCount
                );
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "获取用户统计信息失败，UserId: {UserId}", userId);
                return new UserStatsDto();
            }
        }
    }
}
