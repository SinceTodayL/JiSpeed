using JISpeed.Core.Entities.Common;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Interfaces.IRepositories.Admin;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AdminEntity = JISpeed.Core.Entities.Admin.Admin;

namespace JISpeed.Infrastructure.Repositories.Admin
{
    public class AdminRepository : BaseRepository<AdminEntity, string>, IAdminRepository
    {
        private readonly ILogger<AdminRepository> _logger;
        public AdminRepository(OracleDbContext context
        ,ILogger<AdminRepository> logger) : base(context)
        {
            _logger = logger;
        }

        // 重写GetWithDetailsAsync方法以包含关联数据
        // <param name="adminId">管理员ID</param>
        // <returns>包含关联数据的管理员实体，如果不存在则返回null</returns>
        public override async Task<AdminEntity?> GetWithDetailsAsync(string adminId)
        {
            return await _context.Admins
                .Include(a => a.ApplicationUser)
                .Include(a => a.Announcements)
                .FirstOrDefaultAsync(a => a.AdminId == adminId);
        }

        // 重写GetAllAsync方法以包含关联数据
        // <returns>管理员列表</returns>
        public override async Task<List<AdminEntity>> GetAllAsync()
        {
            return await _context.Admins
                .Include(a => a.ApplicationUser)
                .ToListAsync();
        }

        // === 业务专用查询方法 ===

        // 根据ApplicationUserId获取管理员信息
        // <param name="applicationUserId">应用用户ID</param>
        // <returns>管理员实体，如果不存在则返回null</returns>
        public async Task<AdminEntity?> GetByApplicationUserIdAsync(string applicationUserId)
        {
            return await _context.Admins
                .FirstOrDefaultAsync(a => a.ApplicationUserId == applicationUserId);
        }

        // 根据权限级别获取管理员列表
        // <param name="permissionLevel">权限级别</param>
        // <returns>管理员列表</returns>
        public async Task<List<AdminEntity>> GetByPermissionLevelAsync(int permissionLevel)
        {
            return await _context.Admins
                .Include(a => a.ApplicationUser)
                .Where(a => a.PermissionLevel == permissionLevel.ToString())
                .ToListAsync();
        }

        // 根据状态获取管理员列表
        // <param name="status">状态</param>
        // <returns>管理员列表</returns>
        public async Task<List<AdminEntity>> GetByStatusAsync(int status)
        {
            return await _context.Admins
                .Include(a => a.ApplicationUser)
                .Where(a => a.ApplicationUser != null && a.ApplicationUser.Status == status)
                .ToListAsync();
        }

        // 根据姓名搜索管理员
        // <param name="name">姓名</param>
        // <returns>管理员列表</returns>
        public async Task<List<AdminEntity>> SearchByNameAsync(string name)
        {
            return await _context.Admins
                .Include(a => a.ApplicationUser)
                .Where(a => a.ApplicationUser != null &&
                           !string.IsNullOrEmpty(a.ApplicationUser.UserName) &&
                           a.ApplicationUser.UserName.Contains(name))
                .ToListAsync();
        }
        
        
        // 使用ApplicationUser进行用户的创建
        // <returns>用户实体，如果不存在则返回null</returns>
        public async Task<AdminEntity?> CreateFromApplicationUserAsync(ApplicationUser applicationUser)
        {
            try
            {
                _logger.LogInformation("开始创建用户实体, ApplicationUserId: {ApplicationUserId}", applicationUser.Id);
            
                // 参数验证
                if (applicationUser == null)
                {
                    throw new ValidationException("ApplicationUser不能为空");
                }
            
                if (applicationUser.UserType != 4)
                {
                    throw new ValidationException($"UserType必须为4，当前值: {applicationUser.UserType}");
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
                var user = new AdminEntity
                {
                    AdminId = userId,
                    PermissionLevel = "Admin",
                    ApplicationUserId = applicationUser.Id
                };
                
                // 保存到数据库
                await _dbSet.AddAsync(user);
                await SaveChangesAsync();
                _logger.LogInformation("用户实体创建成功, UserId: {UserId}, ApplicationUserId: {ApplicationUserId}",
                    user.AdminId, applicationUser.Id);
            
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
