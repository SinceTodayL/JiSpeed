using JISpeed.Core.Entities.User;
using JISpeed.Core.Interfaces.IRepositories.User;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JISpeed.Application.Services.Customer
{
    public class FavoriteService
    {
        private readonly OracleDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<FavoriteService> _logger;

        public FavoriteService(
            OracleDbContext context,
            IUserRepository userRepository,
            ILogger<FavoriteService> logger)
        {
            _context = context;
            _userRepository = userRepository;
            _logger = logger;
        }


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


        /// 删除收藏

        /// <param name="userId">用户ID</param>
        /// <param name="dishId">菜品ID</param>
        /// <returns>删除是否成功</returns>
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
                else
                {
                    _logger.LogWarning("收藏记录不存在，UserId: {UserId}, DishId: {DishId}", userId, dishId);
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
                var exists = await _context.Favorites
                    .AnyAsync(f => f.UserId == userId && f.DishId == dishId);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检查收藏状态失败，UserId: {UserId}, DishId: {DishId}", userId, dishId);
                return false;
            }
        }


        /// 获取用户收藏数量

        /// <param name="userId">用户ID</param>
        /// <returns>收藏数量</returns>
        public async Task<int> GetFavoriteCountAsync(string userId)
        {
            try
            {
                var count = await _context.Favorites
                    .CountAsync(f => f.UserId == userId);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取收藏数量失败，UserId: {UserId}", userId);
                return 0;
            }
        }
    }
}
