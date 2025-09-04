using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JISpeed.Core.Entities.User;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Entities.Order;
using JISpeed.Core.Entities.Junctions;
using JISpeed.Core.Interfaces.IRepositories.User;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Constants;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JISpeed.Application.Services.Customer
{
    public class UserService : IUserService
    {
        
        private readonly IMapService _mapService;
        private readonly IUserRepository _userRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly OracleDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository,
            IAddressRepository addressRepository,
            OracleDbContext context,
            ILogger<UserService> logger,
            IMapService mapService)
        {
            _userRepository = userRepository;
            _addressRepository = addressRepository;
            _context = context;
            _logger = logger;
            _mapService = mapService;
        }


        /// 创建用户实体（当ApplicationUser的UserType=1时调用）

        /// <param name="applicationUser">已创建的ApplicationUser</param>
        /// <param name="nickname">用户昵称，默认使用用户名</param>
        /// <returns>创建的User实体</returns>
        public async Task<User> CreateUserEntityAsync(ApplicationUser applicationUser, string? nickname = null)
        {
            try
            {
                _logger.LogInformation("开始创建用户实体, ApplicationUserId: {ApplicationUserId}", applicationUser.Id);

                // 参数验证
                if (applicationUser == null)
                {
                    throw new ValidationException("ApplicationUser不能为空");
                }

                if (applicationUser.UserType != 1)
                {
                    throw new ValidationException($"UserType必须为1，当前值: {applicationUser.UserType}");
                }

                // 检查是否已存在关联的User实体
                var existingUser = await _userRepository.GetByApplicationUserIdAsync(applicationUser.Id);
                if (existingUser != null)
                {
                    _logger.LogWarning("用户实体已存在, ApplicationUserId: {ApplicationUserId}", applicationUser.Id);
                    throw new BusinessException("用户实体已存在");
                }

                // 生成用户ID和昵称
                var userId = Guid.NewGuid().ToString("N");
                var userNickname = nickname ?? applicationUser.UserName ?? "用户" + userId.Substring(0, 8);

                // 创建User实体
                var user = new User
                {
                    UserId = userId,
                    Nickname = userNickname,
                    ApplicationUserId = applicationUser.Id,
                    Gender = 1, // 默认性别：男性
                    Birthday = null
                };

                // 保存到数据库
                await _userRepository.CreateAsync(user);
                await _userRepository.SaveChangesAsync();

                _logger.LogInformation("用户实体创建成功, UserId: {UserId}, ApplicationUserId: {ApplicationUserId}",
                    user.UserId, applicationUser.Id);

                return user;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is BusinessException))
            {
                _logger.LogError(ex, "创建用户实体时发生异常, ApplicationUserId: {ApplicationUserId}",
                    applicationUser?.Id);
                throw new BusinessException("创建用户实体失败");
            }
        }


        /// 根据ID获取用户信息

        /// <param name="userId">用户ID</param>
        /// <returns>用户实体</returns>
        public async Task<User> GetUserByIdAsync(string userId)
        {
            try
            {
                _logger.LogInformation("开始获取用户信息, UserId: {UserId}", userId);

                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogWarning("用户ID不能为空");
                    throw CommonExceptions.ValidationFailed("userId", "不能为空");
                }

                var user = await _userRepository.GetByIdAsync(userId);

                if (user == null)
                {
                    _logger.LogWarning("用户不存在, UserId: {UserId}", userId);
                    throw UserExceptions.UserNotFound(userId);
                }

                _logger.LogInformation("成功获取用户信息, UserId: {UserId}, Nickname: {Nickname}",
                    userId, user.Nickname);

                return user;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取用户信息时发生异常, UserId: {UserId}", userId);
                throw CommonExceptions.GeneralError($"获取用户信息失败: {ex.Message}");
            }
        }


        /// 更新用户信息

        /// <param name="user">更新的用户实体</param>
        /// <returns>更新后的用户实体</returns>
        public async Task<User> UpdateUserAsync(User user)
        {
            try
            {
                _logger.LogInformation("开始更新用户信息, UserId: {UserId}", user.UserId);

                // 参数验证
                if (user == null || string.IsNullOrWhiteSpace(user.UserId))
                {
                    throw CommonExceptions.ValidationFailed("userId", "用户ID不能为空");
                }

                // 检查用户是否存在
                var existingUser = await _userRepository.GetByIdAsync(user.UserId);
                if (existingUser == null)
                {
                    _logger.LogWarning("用户不存在, UserId: {UserId}", user.UserId);
                    throw UserExceptions.UserNotFound(user.UserId);
                }

                // 更新用户信息
                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveChangesAsync();

                _logger.LogInformation("用户信息更新成功, UserId: {UserId}", user.UserId);

                return user;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "更新用户信息时发生异常, UserId: {UserId}", user.UserId);
                throw CommonExceptions.GeneralError($"更新用户信息失败: {ex.Message}");
            }
        }


        /// 根据ApplicationUserId获取用户信息

        /// <param name="applicationUserId">ApplicationUser ID</param>
        /// <returns>用户实体</returns>
        public async Task<User?> GetUserByApplicationUserIdAsync(string applicationUserId)
        {
            try
            {
                _logger.LogInformation("开始根据ApplicationUserId获取用户信息, ApplicationUserId: {ApplicationUserId}", applicationUserId);

                if (string.IsNullOrWhiteSpace(applicationUserId))
                {
                    _logger.LogWarning("ApplicationUserId不能为空");
                    throw CommonExceptions.ValidationFailed("applicationUserId", "不能为空");
                }

                var user = await _userRepository.GetByApplicationUserIdAsync(applicationUserId);

                if (user != null)
                {
                    _logger.LogInformation("成功获取用户信息, ApplicationUserId: {ApplicationUserId}, UserId: {UserId}",
                        applicationUserId, user.UserId);
                }
                else
                {
                    _logger.LogInformation("未找到对应的用户实体, ApplicationUserId: {ApplicationUserId}", applicationUserId);
                }

                return user;
            }
            catch (Exception ex) when (!(ex is ValidationException))
            {
                _logger.LogError(ex, "根据ApplicationUserId获取用户信息时发生异常, ApplicationUserId: {ApplicationUserId}", applicationUserId);
                throw CommonExceptions.GeneralError($"获取用户信息失败: {ex.Message}");
            }
        }

        #region 收藏相关方法


        /// 添加收藏

        /// <param name="userId">用户ID</param>
        /// <param name="dishId">菜品ID</param>
        /// <returns>添加是否成功</returns>
        public async Task<bool> AddFavoriteAsync(string userId, string dishId)
        {
            try
            {
                // 验证用户是否存在
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("用户不存在，UserId: {UserId}", userId);
                    return false;
                }

                // 检查是否已收藏
                var existing = await _context.Favorites
                    .FirstOrDefaultAsync(f => f.UserId == userId && f.DishId == dishId);

                if (existing != null)
                {
                    _logger.LogInformation("菜品已在收藏列表中，UserId: {UserId}, DishId: {DishId}", userId, dishId);
                    return false; // 已经收藏了
                }

                // 使用原生 SQL 插入，因为 Favorite 实体构造函数受限
                var sql = @"
                    INSERT INTO FAVORITE (""UserId"", ""DishId"", ""FavorAt"") 
                    VALUES (:userId, :dishId, :favorAt)";

                var parameters = new[]
                {
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":userId", userId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":dishId", dishId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":favorAt", DateTime.UtcNow)
                };

                var result = await _context.Database.ExecuteSqlRawAsync(sql, parameters);

                if (result > 0)
                {
                    _logger.LogInformation("添加收藏成功，UserId: {UserId}, DishId: {DishId}", userId, dishId);
                }

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "添加收藏失败，UserId: {UserId}, DishId: {DishId}", userId, dishId);
                return false;
            }
        }

        public async Task<List<User>> GetUsersByFiltersAsync(int? size, int? page, string? userId, string? nickname)
        {
            List<User> users = new List<User>();
            if (userId != null)
            {
                User? user = await _userRepository.GetByIdAsync(userId);
                if (user != null)
                {
                    users.Add(user);
                }
            }
            else if (nickname != null)
            {
                users = await _userRepository.SearchByNicknameAsync(nickname, size ?? 10, page ?? 1);
            }
            else
            {
                users = await _userRepository.GetAllUsersAsync(size ?? 10, page ?? 1);
            }

            return users;
        }


        /// 移除收藏

        /// <param name="userId">用户ID</param>
        /// <param name="dishId">菜品ID</param>
        /// <returns>移除是否成功</returns>
        public async Task<bool> RemoveFavoriteAsync(string userId, string dishId)
        {
            try
            {
                // 验证用户是否存在
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("用户不存在，UserId: {UserId}", userId);
                    return false;
                }

                var sql = @"
                    DELETE FROM FAVORITE 
                    WHERE ""UserId"" = :userId AND ""DishId"" = :dishId";

                var parameters = new[]
                {
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":userId", userId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":dishId", dishId)
                };

                var result = await _context.Database.ExecuteSqlRawAsync(sql, parameters);

                if (result > 0)
                {
                    _logger.LogInformation("删除收藏成功，UserId: {UserId}, DishId: {DishId}", userId, dishId);
                }

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除收藏失败，UserId: {UserId}, DishId: {DishId}", userId, dishId);
                return false;
            }
        }


        /// 检查是否已收藏

        /// <param name="userId">用户ID</param>
        /// <param name="dishId">菜品ID</param>
        /// <returns>是否已收藏</returns>
        public async Task<bool> IsFavoriteAsync(string userId, string dishId)
        {
            try
            {
                var existing = await _context.Favorites
                    .FirstOrDefaultAsync(f => f.UserId == userId && f.DishId == dishId);

                return existing != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检查收藏状态失败，UserId: {UserId}, DishId: {DishId}", userId, dishId);
                return false;
            }
        }


        /// 获取收藏数量

        /// <param name="userId">用户ID</param>
        /// <returns>收藏数量</returns>
        public async Task<int> GetFavoriteCountAsync(string userId)
        {
            try
            {
                return await _context.Favorites
                    .CountAsync(f => f.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取收藏数量失败，UserId: {UserId}", userId);
                return 0;
            }
        }

        #endregion

        #region 购物车相关方法


        /// 添加到购物车

        /// <param name="userId">用户ID</param>
        /// <param name="dishId">菜品ID</param>
        /// <returns>购物车项ID</returns>
        public async Task<string?> AddToCartAsync(string userId, string dishId, string merchantId)
        {
            try
            {
                // 验证用户是否存在
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("用户不存在，UserId: {UserId}", userId);
                    return null;
                }

                // 检查是否已在购物车中
                var existing = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.DishId == dishId);

                if (existing != null)
                {
                    _logger.LogInformation("菜品已在购物车中，UserId: {UserId}, DishId: {DishId}", userId, dishId);
                    return null; // 已经在购物车中了
                }

                var cartItemId = Guid.NewGuid().ToString("N");

                // 使用原生 SQL 插入
                var sql = @"
                    INSERT INTO CARTITEM (""CartItemId"", ""UserId"", ""DishId"", ""AddedAt"", ""MerchantId"") 
                    VALUES (:cartItemId, :userId, :dishId, :addedAt, :merchantId)";

                var parameters = new[]
                {
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":cartItemId", cartItemId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":userId", userId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":dishId", dishId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":addedAt", DateTime.UtcNow),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":merchantId", merchantId)
                };

                var result = await _context.Database.ExecuteSqlRawAsync(sql, parameters);

                if (result > 0)
                {
                    _logger.LogInformation("添加到购物车成功，UserId: {UserId}, DishId: {DishId}, CartItemId: {CartItemId}",
                        userId, dishId, cartItemId);
                    return cartItemId;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "添加到购物车失败，UserId: {UserId}, DishId: {DishId}", userId, dishId);
                return null;
            }
        }


        /// 从购物车移除

        /// <param name="userId">用户ID</param>
        /// <param name="cartItemId">购物车项ID</param>
        /// <returns>移除是否成功</returns>
        public async Task<bool> RemoveFromCartAsync(string userId, string cartItemId)
        {
            try
            {
                var sql = @"
                    DELETE FROM CARTITEM 
                    WHERE ""CartItemId"" = :cartItemId AND ""UserId"" = :userId";

                var parameters = new[]
                {
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":cartItemId", cartItemId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":userId", userId)
                };

                var result = await _context.Database.ExecuteSqlRawAsync(sql, parameters);

                if (result > 0)
                {
                    _logger.LogInformation("从购物车删除成功，UserId: {UserId}, CartItemId: {CartItemId}", userId, cartItemId);
                }

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "从购物车删除失败，UserId: {UserId}, CartItemId: {CartItemId}", userId, cartItemId);
                return false;
            }
        }


        /// 检查是否在购物车中

        /// <param name="userId">用户ID</param>
        /// <param name="dishId">菜品ID</param>
        /// <returns>是否在购物车中</returns>
        public async Task<bool> IsInCartAsync(string userId, string dishId)
        {
            try
            {
                var existing = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.DishId == dishId);

                return existing != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检查购物车状态失败，UserId: {UserId}, DishId: {DishId}", userId, dishId);
                return false;
            }
        }


        /// 获取购物车项数量

        /// <param name="userId">用户ID</param>
        /// <returns>购物车项数量</returns>
        public async Task<int> GetCartItemCountAsync(string userId)
        {
            try
            {
                return await _context.CartItems
                    .CountAsync(c => c.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取购物车项数量失败，UserId: {UserId}", userId);
                return 0;
            }
        }

        #endregion

        #region 地址相关方法


        /// 创建地址

        /// <param name="userId">用户ID</param>
        /// <param name="receiverName">收货人姓名</param>
        /// <param name="receiverPhone">收货人手机号</param>
        /// <param name="detailedAddress">详细地址</param>
        /// <param name="isDefault">是否默认地址</param>
        /// <returns>地址ID</returns>
        public async Task<string?> CreateAddressAsync(string userId, string receiverName, string receiverPhone, string detailedAddress, int isDefault)
        {
            try
            {
                // 验证用户是否存在
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("用户不存在，UserId: {UserId}", userId);
                    return null;
                }

                var addressId = Guid.NewGuid().ToString("N");

                var geocode = await _mapService.GeocodeAsync(detailedAddress);
                decimal latitude = 0;
                decimal longitude = 0;
                if (geocode != null || geocode.HasValue)
                {
                    latitude = geocode.Value.latitude;
                    longitude = geocode.Value.longitude;
                }
                else
                {
                    _logger.LogWarning("地址解析失败，无法获取经纬度，DetailedAddress: {DetailedAddress}", detailedAddress);
                    throw new BusinessException("地址解析失败，无法获取经纬度");
                }

                _logger.LogInformation("经纬度为：{Latitude}, {Longitude}", latitude, longitude);

                // 如果设置为默认地址，先取消其他默认地址
                if (isDefault == 1)
                {
                    var updateDefaultSql = @"
                        UPDATE ADDRESS 
                        SET ""IsDefault"" = 0 
                        WHERE ""UserId"" = :userId AND ""IsDefault"" = 1";

                    var updateParams = new[]
                    {
                        new Oracle.ManagedDataAccess.Client.OracleParameter(":userId", userId)
                    };

                    await _context.Database.ExecuteSqlRawAsync(updateDefaultSql, updateParams);
                }

                //TODO: 经纬度
                // 插入新地址
                var sql = @"
                    INSERT INTO ADDRESS (""AddressId"", ""UserId"", ""ReceiverName"", 
                    ""ReceiverPhone"", ""DetailedAddress"", ""IsDefault"", ""Latitude"", ""Longitude"") 
                    VALUES (:addressId, :userId, :receiverName, :receiverPhone, :detailedAddress, :isDefault, :latitude, :longitude)";

                var parameters = new[]
                {
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":addressId", addressId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":userId", userId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":receiverName", receiverName),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":receiverPhone", receiverPhone),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":detailedAddress", detailedAddress),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":isDefault", isDefault),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":latitude", latitude),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":longitude", longitude)
                };

                var result = await _context.Database.ExecuteSqlRawAsync(sql, parameters);

                if (result > 0)
                {
                    _logger.LogInformation("创建地址成功，UserId: {UserId}, AddressId: {AddressId}", userId, addressId);
                    return addressId;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建地址失败，UserId: {UserId}", userId);
                return null;
            }
        }


        /// 更新地址

        /// <param name="userId">用户ID</param>
        /// <param name="addressId">地址ID</param>
        /// <param name="receiverName">收货人姓名</param>
        /// <param name="receiverPhone">收货人手机号</param>
        /// <param name="detailedAddress">详细地址</param>
        /// <param name="isDefault">是否默认地址</param>
        /// <returns>更新是否成功</returns>
        public async Task<bool> UpdateAddressAsync(string userId, string addressId, string receiverName, string receiverPhone, string detailedAddress, int isDefault)
        {
            try
            {
                // 如果设置为默认地址，先取消其他默认地址
                if (isDefault == 1)
                {
                    var updateDefaultSql = @"
                        UPDATE ADDRESS 
                        SET IsDefault = 0 
                        WHERE UserId = :userId AND IsDefault = 1 AND AddressId != :addressId";

                    var updateParams = new[]
                    {
                        new Oracle.ManagedDataAccess.Client.OracleParameter(":userId", userId),
                        new Oracle.ManagedDataAccess.Client.OracleParameter(":addressId", addressId)
                    };

                    await _context.Database.ExecuteSqlRawAsync(updateDefaultSql, updateParams);
                }

                // 更新地址
                var sql = @"
                    UPDATE ADDRESS 
                    SET ReceiverName = :receiverName, 
                        ReceiverPhone = :receiverPhone, 
                        DetailedAddress = :detailedAddress, 
                        IsDefault = :isDefault 
                    WHERE AddressId = :addressId AND UserId = :userId";

                var parameters = new[]
                {
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":receiverName", receiverName),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":receiverPhone", receiverPhone),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":detailedAddress", detailedAddress),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":isDefault", isDefault),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":addressId", addressId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":userId", userId)
                };

                var result = await _context.Database.ExecuteSqlRawAsync(sql, parameters);

                if (result > 0)
                {
                    _logger.LogInformation("更新地址成功，UserId: {UserId}, AddressId: {AddressId}", userId, addressId);
                }

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新地址失败，UserId: {UserId}, AddressId: {AddressId}", userId, addressId);
                return false;
            }
        }


        /// 删除地址

        /// <param name="userId">用户ID</param>
        /// <param name="addressId">地址ID</param>
        /// <returns>删除是否成功</returns>
        public async Task<bool> DeleteAddressAsync(string userId, string addressId)
        {
            try
            {
                var sql = @"
                    DELETE FROM ADDRESS 
                    WHERE ""AddressId"" = :addressId AND ""UserId"" = :userId";

                var parameters = new[]
                {
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":addressId", addressId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":userId", userId)
                };

                var result = await _context.Database.ExecuteSqlRawAsync(sql, parameters);

                if (result > 0)
                {
                    _logger.LogInformation("删除地址成功，UserId: {UserId}, AddressId: {AddressId}", userId, addressId);
                }

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除地址失败，UserId: {UserId}, AddressId: {AddressId}", userId, addressId);
                return false;
            }
        }


        /// 获取地址数量

        /// <param name="userId">用户ID</param>
        /// <returns>地址数量</returns>
        public async Task<int> GetAddressCountAsync(string userId)
        {
            try
            {
                return await _context.Addresses
                    .CountAsync(a => a.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取地址数量失败，UserId: {UserId}", userId);
                return 0;
            }
        }

        #endregion

        #region 评论相关方法

        /// <summary>
        /// 用户对订单进行评论，并同步更新订单下所有菜品的评分和评论数。
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="orderId">订单ID</param>
        /// <param name="rating">订单总评分</param>
        /// <param name="content">评论内容 (可选)</param>
        /// <param name="isAnonymous">是否匿名 (1: 是, 2: 否)</param>
        /// <returns>操作是否成功</returns>
        public async Task<bool> AddReviewAsync(string userId, string orderId, int rating, string? content = null, int isAnonymous = 2)
        {
            // 数据验证：确保订单存在且属于该用户
            // 这一步最好仍然使用 EF Core 的查询，以利用其对象映射能力
            var order = await _context.Orders
                .Include(o => o.OrderDishes)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null)
            {
                _logger.LogWarning("添加评论失败：订单 {OrderId} 不存在或不属于用户 {UserId}", orderId, userId);
                return false;
            }

            if (rating < 1 || rating > 5)
            {
                _logger.LogWarning("添加评论失败：评分 {Rating} 超出有效范围 (1-5)", rating);
                return false;
            }

            // 使用数据库事务保证所有操作的原子性
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var reviewId = Guid.NewGuid().ToString("N");
                var reviewAt = DateTime.UtcNow;

                // 1. 使用原生 SQL 插入 REVIEW 表
                var insertReviewSql = @"
                INSERT INTO REVIEW (""ReviewId"", ""OrderId"", ""UserId"", ""Rating"", ""Content"", ""IsAnonymous"", ""ReviewAt"") 
                VALUES (:reviewId, :orderId, :userId, :rating, :content, :isAnonymous, :reviewAt)";

                var reviewParams = new[]
                {
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":reviewId", reviewId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":orderId", orderId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":userId", userId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":rating", rating),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":content", content ?? (object)DBNull.Value),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":isAnonymous", isAnonymous),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":reviewAt", reviewAt)
                };

                await _context.Database.ExecuteSqlRawAsync(insertReviewSql, reviewParams);

                // 2. 遍历订单中的所有菜品，使用原生 SQL 更新 Dish 表和插入 Dish_Review 联结表
                foreach (var orderDish in order.OrderDishes)
                {
                    // 获取当前菜品的评论数和评分
                    var dish = await _context.Dishes.AsNoTracking().FirstOrDefaultAsync(d => d.DishId == orderDish.DishId);

                    if (dish != null)
                    {
                        // 计算新的评论数量和评分
                        var newReviewQuantity = dish.ReviewQuantity + 1;
                        var newRating = ((dish.Rating * dish.ReviewQuantity) + rating) / newReviewQuantity;

                        // 使用原生 SQL 更新 DISH 表
                        var updateDishSql = @"
                            UPDATE ""DISH""
                            SET ""Rating"" = :newRating, ""ReviewQuantity"" = :newReviewQuantity
                            WHERE ""DishId"" = :dishId";

                        var dishParams = new[]
                        {
                            new Oracle.ManagedDataAccess.Client.OracleParameter(":newRating", newRating),
                            new Oracle.ManagedDataAccess.Client.OracleParameter(":newReviewQuantity", newReviewQuantity),
                            new Oracle.ManagedDataAccess.Client.OracleParameter(":dishId", dish.DishId)
                        };
                        await _context.Database.ExecuteSqlRawAsync(updateDishSql, dishParams);

                        // 使用原生 SQL 插入 DISH_REVIEW 联结表
                        var insertDishReviewSql = @"
                        INSERT INTO ""DISH_REVIEW"" (""DishId"", ""ReviewId"")
                        VALUES (:dishId, :reviewId)";

                        var dishReviewParams = new[]
                        {
                            new Oracle.ManagedDataAccess.Client.OracleParameter(":dishId", dish.DishId),
                            new Oracle.ManagedDataAccess.Client.OracleParameter(":reviewId", reviewId)
                        };
                        await _context.Database.ExecuteSqlRawAsync(insertDishReviewSql, dishReviewParams);
                    }
                }

                await transaction.CommitAsync();

                _logger.LogInformation("添加评论成功，UserId: {UserId}, OrderId: {OrderId}, ReviewId: {ReviewId}",
                    userId, orderId, reviewId);

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "添加评论失败，UserId: {UserId}, OrderId: {OrderId}", userId, orderId);
                return false;
            }
        }

        /// 删除评论
        /// <param name="userId">用户ID</param>
        /// <param name="reviewId">评论ID</param>
        /// <returns>删除是否成功</returns>
        public async Task<bool> DeleteReviewAsync(string userId, string reviewId)
        {
            try
            {
                var sql = @"
                    DELETE FROM REVIEW
                    WHERE ""ReviewId"" = :reviewId AND ""UserId"" = :userId";

                var parameters = new[]
                {
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":reviewId", reviewId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":userId", userId)
                };

                var result = await _context.Database.ExecuteSqlRawAsync(sql, parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除评论失败，UserId: {UserId}, ReviewId: {ReviewId}", userId, reviewId);
                return false;
            }
        }

        #endregion

        #region 投诉相关方法

        public async Task<string> AddComplaintAsync(string UserId, string OrderId, int Role, int Status, string Description)
        {
            try
            {
                var complaintId = Guid.NewGuid().ToString("N");
                var createdAt = DateTime.UtcNow;

                var sql = @"
                    INSERT INTO COMPLAINT (""ComplaintId"", ""ComplainantId"", ""OrderId"", ""CmplRole"", ""CmplStatus"", ""CmplDescription"", ""CreatedAt"") 
                    VALUES (:complaintId, :userId, :orderId, :role, :status, :description, :createdAt)";

                var parameters = new[]
                {
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":complaintId", complaintId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":userId", UserId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":orderId", OrderId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":role", Role),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":status", Status),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":description", Description),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":createdAt", createdAt)
                };

                var result = await _context.Database.ExecuteSqlRawAsync(sql, parameters);

                if (result > 0)
                {
                    _logger.LogInformation("创建投诉成功，UserId: {UserId}, ComplaintId: {ComplaintId}", UserId, complaintId);
                    return complaintId;
                }

                _logger.LogWarning("创建投诉失败，UserId: {UserId}", UserId);
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建投诉失败，UserId: {UserId}", UserId);
                return string.Empty;
            }
        }

        #endregion
    }
}
