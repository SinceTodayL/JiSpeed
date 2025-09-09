using JISpeed.Core.Entities.Common;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Interfaces.IRepositories.User;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserEntity = JISpeed.Core.Entities.User.User;

namespace JISpeed.Infrastructure.Repositories.User
{
    public class UserRepository : BaseRepository<UserEntity, string>, IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;
        public UserRepository(OracleDbContext context, ILogger<UserRepository> logger) : base(context)
        {
            _logger = logger;
        }

        // 重写GetWithDetailsAsync方法以包含关联数据
        // <param name="userId">用户ID</param>
        // <returns>包含关联数据的用户实体，如果不存在则返回null</returns>
        public override async Task<UserEntity?> GetWithDetailsAsync(string userId)
        {
            return await _context.CustomUsers
                .Include(u => u.ApplicationUser)
                .Include(u => u.Addresses)
                .Include(u => u.CartItems)
                .Include(u => u.Favorites)
                .Include(u => u.Orders)
                .Include(u => u.Coupons)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        // === 业务专用查询方法 ===

        // 获取全部用户信息
        // <returns>用户列表</returns>
        public async Task<List<UserEntity>> GetAllUsersAsync(int size, int page)
        {
            var query = _context.CustomUsers.AsQueryable();

            if (page > 0 && size > 0)
            {
                query = query.Skip((page - 1) * size).Take(size);
            }

            return await query.ToListAsync();
        }

        // 根据ApplicationUserId获取用户信息
        // <param name="applicationUserId">应用用户ID</param>
        // <returns>用户实体，如果不存在则返回null</returns>
        public async Task<UserEntity?> GetByApplicationUserIdAsync(string applicationUserId)
        {
            return await _context.CustomUsers
                .FirstOrDefaultAsync(u => u.ApplicationUserId == applicationUserId);
        }

        // 根据昵称搜索用户
        // <param name="nickname">昵称</param>
        // <returns>用户列表</returns>
        public async Task<List<UserEntity>> SearchByNicknameAsync(string nickname, int size, int page)
        {
            var query = _context.CustomUsers
                .Where(u => u.Nickname.Contains(nickname));

            if (page > 0 && size > 0)
            {
                query = query.Skip((page - 1) * size).Take(size);
            }

            return await query.ToListAsync();
        }

        // 根据等级获取用户列表
        // <param name="level">等级</param>
        // <returns>用户列表</returns>
        public async Task<List<UserEntity>> GetByLevelAsync(int level)
        {
            return await _context.CustomUsers
                .Where(u => u.Level == level)
                .OrderBy(u => u.Nickname)
                .ToListAsync();
        }

        // 重写GetAllAsync方法以包含关联数据和排序
        // <returns>用户列表</returns>
        public override async Task<List<UserEntity>> GetAllAsync()
        {
            return await _context.CustomUsers
                .Include(u => u.ApplicationUser)
                .OrderByDescending(u => u.UserId)
                .ToListAsync();
        }

        // 使用ApplicationUser进行用户的创建
        // <returns>用户实体，如果不存在则返回null</returns>
        public async Task<UserEntity?> CreateFromApplicationUserAsync(ApplicationUser applicationUser)
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
                var existingUser = await GetByApplicationUserIdAsync(applicationUser.Id);
                if (existingUser != null)
                {
                    _logger.LogWarning("用户实体已存在, ApplicationUserId: {ApplicationUserId}", applicationUser.Id);
                    throw new BusinessException("用户实体已存在");
                }

                // 生成用户ID和昵称
                var userId = Guid.NewGuid().ToString("N");
                var userNickname = applicationUser.UserName ?? "用户" + userId.Substring(0, 8);

                // 创建User实体
                var user = new UserEntity
                {
                    UserId = userId,
                    Nickname = userNickname,
                    ApplicationUserId = applicationUser.Id
                };

                // 保存到数据库
                await _dbSet.AddAsync(user);
                await SaveChangesAsync();
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

        /// <summary>
        /// 获取用户收藏列表（分页）
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="page">页码</param>
        /// <param name="size">每页大小</param>
        /// <returns>收藏列表</returns>
        public async Task<List<JISpeed.Core.Entities.User.Favorite>> GetUserFavoritesAsync(string userId, int page = 1, int size = 10)
        {
            try
            {
                // 只查询Favorite基本信息，不加载关联的Dish和Merchant数据
                var query = _context.Favorites
                    .Where(f => f.UserId == userId)
                    .OrderByDescending(f => f.FavorAt);

                if (page > 0 && size > 0)
                {
                    return await query.Skip((page - 1) * size).Take(size).ToListAsync();
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户收藏列表时发生异常，UserId: {UserId}", userId);
                throw new BusinessException("获取用户收藏列表失败");
            }
        }

        /// <summary>
        /// 获取用户收藏数量
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>收藏数量</returns>
        public async Task<int> GetUserFavoritesCountAsync(string userId)
        {
            try
            {
                return await _context.Favorites
                    .Where(f => f.UserId == userId)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户收藏数量时发生异常，UserId: {UserId}", userId);
                throw new BusinessException("获取用户收藏数量失败");
            }
        }

        /// <summary>
        /// 获取用户统计信息（订单数、收藏数、地址数等）
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>统计信息</returns>
        public async Task<(int OrderCount, int FavoriteCount, int AddressCount, int CartItemCount)> GetUserStatsAsync(string userId)
        {
            try
            {
                var orderCount = await _context.Orders.CountAsync(o => o.UserId == userId);
                var favoriteCount = await _context.Favorites.CountAsync(f => f.UserId == userId);
                var addressCount = await _context.Addresses.CountAsync(a => a.UserId == userId);
                var cartItemCount = await _context.CartItems.CountAsync(c => c.UserId == userId);

                return (orderCount, favoriteCount, addressCount, cartItemCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户统计信息时发生异常，UserId: {UserId}", userId);
                throw new BusinessException("获取用户统计信息失败");
            }
        }

        /// <summary>
        /// 获取用户详细信息（包含ApplicationUser和DefaultAddress）
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户详细信息</returns>
        public async Task<UserEntity?> GetUserDetailAsync(string userId)
        {
            try
            {
                return await _context.CustomUsers
                    .Include(u => u.ApplicationUser)
                    .Include(u => u.DefaultAddress)
                    .FirstOrDefaultAsync(u => u.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户详细信息时发生异常，UserId: {UserId}", userId);
                throw new BusinessException("获取用户详细信息失败");
            }
        }
    }
}
