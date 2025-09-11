using JISpeed.Core.Entities.User;
using JISpeed.Core.Interfaces.IRepositories.User;
using JISpeed.Core.Interfaces.IRepositories.Dish;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JISpeed.Application.Services.Customer
{
    public class CartService
    {
        private readonly OracleDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IDishRepository _dishRepository;
        private readonly ILogger<CartService> _logger;

        public CartService(
            OracleDbContext context,
            IUserRepository userRepository,
            IDishRepository dishRepository,
            ILogger<CartService> logger)
        {
            _context = context;
            _userRepository = userRepository;
            _dishRepository = dishRepository;
            _logger = logger;
        }


        /// 添加到购物车

        /// <param name="userId">用户ID</param>
        /// <param name="dishId">菜品ID</param>
        /// <returns>购物车项ID，失败返回null</returns>
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

                // 验证菜品是否存在并获取商家ID
                var dish = await _dishRepository.GetByIdAsync(dishId);
                if (dish == null)
                {
                    _logger.LogWarning("菜品不存在，DishId: {DishId}", dishId);
                    return null;
                }

                // 检查是否已在购物车中
                var existing = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.DishId == dishId);

                if (existing != null)
                {
                    _logger.LogInformation("商品已在购物车中，UserId: {UserId}, DishId: {DishId}", userId, dishId);
                    return null; // 已经在购物车中了
                }

                var cartItemId = Guid.NewGuid().ToString("N");

                // 使用原生 SQL 插入，因为 CartItem 实体构造函数受限
                var sql = @"
                    INSERT INTO CARTITEM (CartItemId, UserId, MerchantId, DishId, AddedAt) 
                    VALUES (:cartItemId, :userId, :merchantId, :dishId, :addedAt)";

                var parameters = new[]
                {
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":cartItemId", cartItemId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":userId", userId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":merchantId", dish.MerchantId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":dishId", dishId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":addedAt", DateTime.Now)
                };

                var result = await _context.Database.ExecuteSqlRawAsync(sql, parameters);

                if (result > 0)
                {
                    _logger.LogInformation("添加到购物车成功，CartItemId: {CartItemId}, UserId: {UserId}, DishId: {DishId}",
                        cartItemId, userId, dishId);
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


        /// 从购物车删除商品

        /// <param name="userId">用户ID</param>
        /// <param name="cartItemId">购物车项ID</param>
        /// <returns>删除是否成功</returns>
        public async Task<bool> RemoveFromCartAsync(string userId, string cartItemId)
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
                    _logger.LogInformation("从购物车删除成功，CartItemId: {CartItemId}, UserId: {UserId}",
                        cartItemId, userId);
                }
                else
                {
                    _logger.LogWarning("购物车项不存在，CartItemId: {CartItemId}, UserId: {UserId}",
                        cartItemId, userId);
                }

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "从购物车删除失败，UserId: {UserId}, CartItemId: {CartItemId}", userId, cartItemId);
                return false;
            }
        }


        /// 清空用户购物车

        /// <param name="userId">用户ID</param>
        /// <returns>删除的项目数量</returns>
        public async Task<int> ClearCartAsync(string userId)
        {
            try
            {
                // 验证用户是否存在
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("用户不存在，UserId: {UserId}", userId);
                    return 0;
                }

                var sql = @"DELETE FROM CARTITEM WHERE UserId = :userId";
                var parameters = new[]
                {
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":userId", userId)
                };

                var result = await _context.Database.ExecuteSqlRawAsync(sql, parameters);

                if (result > 0)
                {
                    _logger.LogInformation("清空购物车成功，UserId: {UserId}, 删除了 {Count} 个项目", userId, result);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "清空购物车失败，UserId: {UserId}", userId);
                return 0;
            }
        }


        /// 检查商品是否在购物车中

        /// <param name="userId">用户ID</param>
        /// <param name="dishId">菜品ID</param>
        /// <returns>是否在购物车中</returns>
        public async Task<bool> IsInCartAsync(string userId, string dishId)
        {
            try
            {
                var exists = await _context.CartItems
                    .AnyAsync(c => c.UserId == userId && c.DishId == dishId);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检查购物车状态失败，UserId: {UserId}, DishId: {DishId}", userId, dishId);
                return false;
            }
        }


        /// 获取用户购物车商品数量

        /// <param name="userId">用户ID</param>
        /// <returns>购物车商品数量</returns>
        public async Task<int> GetCartItemCountAsync(string userId)
        {
            try
            {
                var count = await _context.CartItems
                    .CountAsync(c => c.UserId == userId);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取购物车商品数量失败，UserId: {UserId}", userId);
                return 0;
            }
        }
    }
}
