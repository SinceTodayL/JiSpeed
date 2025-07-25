using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JISpeed.Core.Interfaces.IRepositories.Rider;
using JISpeed.Infrastructure.Data;
using JISpeed.Infrastructure.Repositories;

namespace JISpeed.Infrastructure.Repositories.Rider
{
    // 骑手仓储实现 - 处理骑手基础信息的数据访问操作
    public class RiderRepository : BaseRepository<JISpeed.Core.Entities.Rider.Rider, string>, IRiderRepository
    {
        public RiderRepository(OracleDbContext context) : base(context)
        {
        }

        // 重写GetByIdAsync以包含关联数据
        // <param name="id">骑手ID</param>
        // <returns>包含关联数据的骑手实体，如果不存在则返回null</returns>
        public override async Task<JISpeed.Core.Entities.Rider.Rider?> GetByIdAsync(string id)
        {
            return await _context.Riders
                .Include(r => r.ApplicationUser)
                .FirstOrDefaultAsync(r => r.RiderId == id);
        }

        // 重写GetWithDetailsAsync以包含详细关联数据
        // <param name="id">骑手ID</param>
        // <returns>包含详细关联数据的骑手实体，如果不存在则返回null</returns>
        public override async Task<JISpeed.Core.Entities.Rider.Rider?> GetWithDetailsAsync(string id)
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
        public override async Task<List<JISpeed.Core.Entities.Rider.Rider>> GetAllAsync()
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
        public async Task<JISpeed.Core.Entities.Rider.Rider?> GetByApplicationUserIdAsync(string applicationUserId)
        {
            return await _context.Riders
                .Include(r => r.ApplicationUser)
                .FirstOrDefaultAsync(r => r.ApplicationUserId == applicationUserId);
        }

        // 根据手机号查询骑手
        // <param name="phoneNumber">手机号</param>
        // <returns>骑手实体，如果不存在则返回null</returns>
        public async Task<JISpeed.Core.Entities.Rider.Rider?> GetByPhoneNumberAsync(string phoneNumber)
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
        public async Task<IEnumerable<JISpeed.Core.Entities.Rider.Rider>> SearchByNameAsync(string name)
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
        public async Task<JISpeed.Core.Entities.Rider.Rider?> GetByVehicleNumberAsync(string vehicleNumber)
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
        public async Task<IEnumerable<JISpeed.Core.Entities.Rider.Rider>> GetRidersWithVehicleAsync()
        {
            return await _context.Riders
                .Include(r => r.ApplicationUser)
                .Where(r => !string.IsNullOrEmpty(r.VehicleNumber))
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        // 获取没有车牌号的骑手
        // <returns>没有车牌号的骑手列表</returns>
        public async Task<IEnumerable<JISpeed.Core.Entities.Rider.Rider>> GetRidersWithoutVehicleAsync()
        {
            return await _context.Riders
                .Include(r => r.ApplicationUser)
                .Where(r => string.IsNullOrEmpty(r.VehicleNumber))
                .OrderBy(r => r.Name)
                .ToListAsync();
        }
    }
}
