using JISpeed.Core.Entities.Common;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Interfaces.IRepositories.Merchant;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MerchantEntity = JISpeed.Core.Entities.Merchant.Merchant;

namespace JISpeed.Infrastructure.Repositories.Merchant
{
    public class MerchantRepository : BaseRepository<MerchantEntity, string>, IMerchantRepository
    {
        private readonly ILogger<MerchantRepository> _logger;
        public MerchantRepository(OracleDbContext context,
            ILogger<MerchantRepository> logger) : base(context)
        {
            _logger = logger;
        }

        // === 业务专用查询方法 ===

        // 保留兼容性方法：GetUserWithDetailsAsync 映射到 GetWithDetailsAsync
        // <param name="merchantId">商家ID</param>
        // <returns>商家实体详情，如果不存在则返回null</returns>
        public async Task<MerchantEntity?> GetUserWithDetailsAsync(string merchantId)
        {
            return await GetWithDetailsAsync(merchantId);
        }

        // 根据ApplicationUserId获取商家信息
        // <param name="applicationUserId">应用用户ID</param>
        // <returns>商家实体，如果不存在则返回null</returns>
        public async Task<MerchantEntity?> GetByApplicationUserIdAsync(string applicationUserId)
        {
            return await _context.Merchants
                .FirstOrDefaultAsync(u => u.ApplicationUserId == applicationUserId);
        }

        // 根据商家名称搜索商家
        // <param name="name">商家名称</param>
        // <returns>商家列表</returns>
        public async Task<List<MerchantEntity>> SearchByNameAsync(string name)
        {
            return await _context.Merchants
                .Where(m => m.MerchantName.Contains(name))
                .ToListAsync();
        }

        // 根据状态获取商家列表
        // <param name="status">状态</param>
        // <returns>商家列表</returns>
        public async Task<List<MerchantEntity>> GetByStatusAsync(int status)
        {
            return await _context.Merchants
                .Where(m => m.Status == status)
                .ToListAsync();
        }

        // 根据位置搜索商家
        // <param name="location">位置</param>
        // <returns>商家列表</returns>
        public async Task<List<MerchantEntity>> SearchByLocationAsync(string location)
        {
            return await _context.Merchants
                .Where(m => !string.IsNullOrEmpty(m.Location) && m.Location.Contains(location))
                .ToListAsync();
        }
        // 使用ApplicationUser进行用户的创建
        // <returns>用户实体，如果不存在则返回null</returns>
        public async Task<MerchantEntity?> CreateFromApplicationUserAsync(ApplicationUser applicationUser)
        {
            try
            {
                _logger.LogInformation("开始创建用户实体, ApplicationUserId: {ApplicationUserId}", applicationUser.Id);
            
                // 参数验证
                if (applicationUser == null)
                {
                    throw new ValidationException("ApplicationUser不能为空");
                }
            
                if (applicationUser.UserType != 2)
                {
                    throw new ValidationException($"UserType必须为2，当前值: {applicationUser.UserType}");
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
                var user = new MerchantEntity
                {
                    MerchantId = userId,
                    MerchantName = userNickname, 
                    ContactInfo = applicationUser.Email,
                    ApplicationUserId = applicationUser.Id
                };
                
                // 保存到数据库
                await _dbSet.AddAsync(user);
                await SaveChangesAsync();
                _logger.LogInformation("用户实体创建成功, UserId: {UserId}, ApplicationUserId: {ApplicationUserId}",
                    user.MerchantId, applicationUser.Id);
            
                return user;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is BusinessException))
            {
                _logger.LogError(ex, "创建用户实体时发生异常, ApplicationUserId: {ApplicationUserId}",
                    applicationUser?.Id);
                throw new BusinessException("创建用户实体失败");
            }
        }
    }
}