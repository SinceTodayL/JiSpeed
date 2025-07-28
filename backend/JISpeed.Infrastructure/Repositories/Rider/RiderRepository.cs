using JISpeed.Core.Entities.Common;
using JISpeed.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using JISpeed.Core.Interfaces.IRepositories.Rider;
using JISpeed.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using RiderEntity = JISpeed.Core.Entities.Rider.Rider;

namespace JISpeed.Infrastructure.Repositories.Rider
{
    // 骑手仓储实现 - 处理骑手基础信息的数据访问操作
    public class RiderRepository : BaseRepository<RiderEntity, string>, IRiderRepository
    {
        
        private readonly ILogger<RiderRepository> _logger;
        public RiderRepository(OracleDbContext context,
            ILogger<RiderRepository> logger) : base(context)
        {
            _logger = logger;
        }

        // 重写GetByIdAsync以包含关联数据
        // <param name="id">骑手ID</param>
        // <returns>包含关联数据的骑手实体，如果不存在则返回null</returns>
        public override async Task<RiderEntity?> GetByIdAsync(string id)
        {
            return await _context.Riders
                .Include(r => r.ApplicationUser)
                .FirstOrDefaultAsync(r => r.RiderId == id);
        }

        // 重写GetWithDetailsAsync以包含详细关联数据
        // <param name="id">骑手ID</param>
        // <returns>包含详细关联数据的骑手实体，如果不存在则返回null</returns>
        public override async Task<RiderEntity?> GetWithDetailsAsync(string id)
        {
            return await _context.Riders
                .Include(r => r.ApplicationUser)
                .Include(r => r.Assignments)
                .Include(r => r.Attendances)
                .Include(r => r.Performances)
                .Include(r => r.RiderSchedules)
                .FirstOrDefaultAsync(r => r.RiderId == id);
        }

        // 重写GetAllAsync以包含关联数据和排序
        // <returns>骑手列表</returns>
        public override async Task<List<RiderEntity>> GetAllAsync()
        {
            return await _context.Riders
                .Include(r => r.ApplicationUser)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        // === 业务专用查询方法 ===

        // 根据ApplicationUserId查询骑手
        // <param name="applicationUserId">应用用户ID</param>
        // <returns>骑手实体，如果不存在则返回null</returns>
        public async Task<RiderEntity?> GetByApplicationUserIdAsync(string applicationUserId)
        {
            return await _context.Riders
                .Include(r => r.ApplicationUser)
                .FirstOrDefaultAsync(r => r.ApplicationUserId == applicationUserId);
        }

        // 根据手机号查询骑手
        // <param name="phoneNumber">手机号</param>
        // <returns>骑手实体，如果不存在则返回null</returns>
        public async Task<RiderEntity?> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Riders
                .Include(r => r.ApplicationUser)
                .FirstOrDefaultAsync(r => r.PhoneNumber == phoneNumber);
        }

        // 根据手机号检查骑手是否存在
        // <param name="phoneNumber">手机号</param>
        // <returns>骑手是否存在</returns>
        public async Task<bool> ExistsByPhoneAsync(string phoneNumber)
        {
            return await _context.Riders.AnyAsync(r => r.PhoneNumber == phoneNumber);
        }

        // 根据姓名搜索骑手
        // <param name="name">姓名</param>
        // <returns>骑手列表</returns>
        public async Task<IEnumerable<RiderEntity>> SearchByNameAsync(string name)
        {
            return await _context.Riders
                .Include(r => r.ApplicationUser)
                .Where(r => r.Name.Contains(name))
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        // 根据车牌号查询骑手
        // <param name="vehicleNumber">车牌号</param>
        // <returns>骑手实体，如果不存在则返回null</returns>
        public async Task<RiderEntity?> GetByVehicleNumberAsync(string vehicleNumber)
        {
            return await _context.Riders
                .Include(r => r.ApplicationUser)
                .FirstOrDefaultAsync(r => r.VehicleNumber == vehicleNumber);
        }

        // 检查车牌号是否已存在
        // <param name="vehicleNumber">车牌号</param>
        // <returns>车牌号是否存在</returns>
        public async Task<bool> ExistsByVehicleNumberAsync(string vehicleNumber)
        {
            return await _context.Riders.AnyAsync(r => r.VehicleNumber == vehicleNumber);
        }

        // 获取有车牌号的骑手
        // <returns>有车牌号的骑手列表</returns>
        public async Task<IEnumerable<RiderEntity>> GetRidersWithVehicleAsync()
        {
            return await _context.Riders
                .Include(r => r.ApplicationUser)
                .Where(r => !string.IsNullOrEmpty(r.VehicleNumber))
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        // 获取没有车牌号的骑手
        // <returns>没有车牌号的骑手列表</returns>
        public async Task<IEnumerable<RiderEntity>> GetRidersWithoutVehicleAsync()
        {
            return await _context.Riders
                .Include(r => r.ApplicationUser)
                .Where(r => string.IsNullOrEmpty(r.VehicleNumber))
                .OrderBy(r => r.Name)
                .ToListAsync();
        }
        
        // 使用ApplicationUser进行用户的创建
        // <returns>用户实体，如果不存在则返回null</returns>
        public async Task<RiderEntity?> CreateFromApplicationUserAsync(ApplicationUser applicationUser)
        {
            try
            {
                _logger.LogInformation("开始创建用户实体, ApplicationUserId: {ApplicationUserId}", applicationUser.Id);
            
                // 参数验证
                if (applicationUser == null)
                {
                    throw new ValidationException("ApplicationUser不能为空");
                }
            
                if (applicationUser.UserType != 3)
                {
                    throw new ValidationException($"UserType必须为3，当前值: {applicationUser.UserType}");
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
                var user = new RiderEntity
                {
                    RiderId = userId,
                    Name = userNickname, 
                    PhoneNumber = applicationUser.PhoneNumber,
                    ApplicationUserId = applicationUser.Id
                };
                
                // 保存到数据库
                await _dbSet.AddAsync(user);
                await SaveChangesAsync();
                _logger.LogInformation("用户实体创建成功, UserId: {UserId}, ApplicationUserId: {ApplicationUserId}",
                    user.RiderId, applicationUser.Id);
            
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
