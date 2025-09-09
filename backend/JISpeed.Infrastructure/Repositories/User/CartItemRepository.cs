using JISpeed.Core.Entities.User;
using JISpeed.Core.Interfaces.IRepositories.User;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JISpeed.Infrastructure.Repositories.User
{

    /// 购物车项仓储实现

    public class CartItemRepository : BaseRepository<CartItem, string>, ICartItemRepository
    {
        private readonly ILogger<CartItemRepository> _logger;

        public CartItemRepository(OracleDbContext context, ILogger<CartItemRepository> logger) : base(context)
        {
            _logger = logger;
        }


        /// 根据用户ID获取购物车项（分页）

        /// <param name="userId">用户ID</param>
        /// <param name="page">页码</param>
        /// <param name="size">每页大小</param>
        /// <returns>购物车项列表</returns>
        public async Task<List<CartItem>> GetByUserIdAsync(string userId, int page = 1, int size = 10)
        {
            try
            {
                // 只查询CartItem基本信息，不加载Dish和Merchant关联数据
                var query = _context.CartItems
                    .Where(c => c.UserId == userId)
                    .OrderByDescending(c => c.AddedAt);

                if (page > 0 && size > 0)
                {
                    return await query.Skip((page - 1) * size).Take(size).ToListAsync();
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户购物车项时发生异常，UserId: {UserId}", userId);
                throw;
            }
        }


        /// 根据用户ID获取购物车项数量

        /// <param name="userId">用户ID</param>
        /// <returns>购物车项数量</returns>
        public async Task<int> GetCountByUserIdAsync(string userId)
        {
            try
            {
                return await _context.CartItems
                    .Where(c => c.UserId == userId)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户购物车项数量时发生异常，UserId: {UserId}", userId);
                throw;
            }
        }


        /// 根据用户ID和菜品ID获取购物车项

        /// <param name="userId">用户ID</param>
        /// <param name="dishId">菜品ID</param>
        /// <returns>购物车项</returns>
        public async Task<CartItem?> GetByUserIdAndDishIdAsync(string userId, string dishId)
        {
            try
            {
                // 只查询CartItem基本信息
                return await _context.CartItems
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.DishId == dishId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "根据用户ID和菜品ID获取购物车项时发生异常，UserId: {UserId}, DishId: {DishId}", userId, dishId);
                throw;
            }
        }


        /// 根据用户ID和购物车项ID获取购物车项

        /// <param name="userId">用户ID</param>
        /// <param name="cartItemId">购物车项ID</param>
        /// <returns>购物车项</returns>
        public async Task<CartItem?> GetByUserIdAndCartItemIdAsync(string userId, string cartItemId)
        {
            try
            {
                // 只查询CartItem基本信息
                return await _context.CartItems
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.CartItemId == cartItemId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "根据用户ID和购物车项ID获取购物车项时发生异常，UserId: {UserId}, CartItemId: {CartItemId}", userId, cartItemId);
                throw;
            }
        }


        /// 删除用户的所有购物车项

        /// <param name="userId">用户ID</param>
        /// <returns>删除的数量</returns>
        public async Task<int> DeleteByUserIdAsync(string userId)
        {
            try
            {
                var cartItems = await _context.CartItems
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                if (cartItems.Any())
                {
                    _context.CartItems.RemoveRange(cartItems);
                    await SaveChangesAsync();
                }

                return cartItems.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除用户购物车项时发生异常，UserId: {UserId}", userId);
                throw;
            }
        }
    }
}
