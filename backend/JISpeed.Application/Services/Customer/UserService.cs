using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JISpeed.Core.Entities.User;
using JISpeed.Core.Entities.Common;
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
        private readonly IUserRepository _userRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly OracleDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository,
            IAddressRepository addressRepository,
            OracleDbContext context,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _addressRepository = addressRepository;
            _context = context;
            _logger = logger;
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
                    INSERT INTO FAVORITE (UserId, DishId, FavorAt) 
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
                    WHERE UserId = :userId AND DishId = :dishId";

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
        public async Task<string?> AddToCartAsync(string userId, string dishId)
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
                    INSERT INTO CARTITEM (CartItemId, UserId, DishId, AddedAt) 
                    VALUES (:cartItemId, :userId, :dishId, :addedAt)";

                var parameters = new[]
                {
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":cartItemId", cartItemId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":userId", userId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":dishId", dishId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":addedAt", DateTime.UtcNow)
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
                    WHERE CartItemId = :cartItemId AND UserId = :userId";

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

                // 如果设置为默认地址，先取消其他默认地址
                if (isDefault == 1)
                {
                    var updateDefaultSql = @"
                        UPDATE ADDRESS 
                        SET IsDefault = 0 
                        WHERE UserId = :userId AND IsDefault = 1";

                    var updateParams = new[]
                    {
                        new Oracle.ManagedDataAccess.Client.OracleParameter(":userId", userId)
                    };

                    await _context.Database.ExecuteSqlRawAsync(updateDefaultSql, updateParams);
                }

                // 插入新地址
                var sql = @"
                    INSERT INTO ADDRESS (AddressId, UserId, ReceiverName, ReceiverPhone, DetailedAddress, IsDefault) 
                    VALUES (:addressId, :userId, :receiverName, :receiverPhone, :detailedAddress, :isDefault)";

                var parameters = new[]
                {
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":addressId", addressId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":userId", userId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":receiverName", receiverName),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":receiverPhone", receiverPhone),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":detailedAddress", detailedAddress),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":isDefault", isDefault)
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
                    WHERE AddressId = :addressId AND UserId = :userId";

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
    }
}
