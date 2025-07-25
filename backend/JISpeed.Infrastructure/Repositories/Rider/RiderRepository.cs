using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JISpeed.Core.Interfaces.IRepositories.Rider;
using JISpeed.Infrastructure.Data;

namespace JISpeed.Infrastructure.Repositories.Rider
{
    // 骑手仓储实现 - 处理骑手基础信息的数据访问操作
    public class RiderRepository : IRiderRepository
    {
        private readonly OracleDbContext _context;

        public RiderRepository(OracleDbContext context)
        {
            _context = context;
        }

        // 根据主键查询骑手
        public async Task<JISpeed.Core.Entities.Rider.Rider?> GetByIdAsync(string id)
        {
            return await _context.Riders
                .Include(r => r.ApplicationUser)
                .FirstOrDefaultAsync(r => r.RiderId == id);
        }

        // 查询骑手包含详细信息
        public async Task<JISpeed.Core.Entities.Rider.Rider?> GetWithDetailsAsync(string id)
        {
            return await _context.Riders
                .Include(r => r.ApplicationUser)
                .Include(r => r.Assignments)
                .Include(r => r.Attendances)
                .Include(r => r.Performances)
                .Include(r => r.RiderSchedules)
                .FirstOrDefaultAsync(r => r.RiderId == id);
        }

        // 获取所有骑手
        public async Task<List<JISpeed.Core.Entities.Rider.Rider>> GetAllAsync()
        {
            return await _context.Riders
                .Include(r => r.ApplicationUser)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        // 检查骑手是否存在
        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Riders.AnyAsync(r => r.RiderId == id);
        }

        // 创建新骑手
        public async Task<JISpeed.Core.Entities.Rider.Rider> CreateAsync(JISpeed.Core.Entities.Rider.Rider entity)
        {
            await _context.Riders.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        // 更新骑手信息
        public async Task<JISpeed.Core.Entities.Rider.Rider> UpdateAsync(JISpeed.Core.Entities.Rider.Rider entity)
        {
            _context.Riders.Update(entity);
            await _context.SaveChangesAsync();
            await Task.CompletedTask;
            return entity;
        }

        // 删除骑手
        public async Task<bool> DeleteAsync(string id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Riders.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // 保存更改
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // === 业务专用查询方法 ===

        // 根据ApplicationUserId查询骑手
        public async Task<JISpeed.Core.Entities.Rider.Rider?> GetByApplicationUserIdAsync(string applicationUserId)
        {
            return await _context.Riders
                .Include(r => r.ApplicationUser)
                .FirstOrDefaultAsync(r => r.ApplicationUserId == applicationUserId);
        }

        // 根据手机号查询骑手
        public async Task<JISpeed.Core.Entities.Rider.Rider?> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Riders
                .Include(r => r.ApplicationUser)
                .FirstOrDefaultAsync(r => r.PhoneNumber == phoneNumber);
        }

        // 根据手机号检查骑手是否存在
        public async Task<bool> ExistsByPhoneAsync(string phoneNumber)
        {
            return await _context.Riders.AnyAsync(r => r.PhoneNumber == phoneNumber);
        }

        // 根据姓名搜索骑手
        public async Task<IEnumerable<JISpeed.Core.Entities.Rider.Rider>> SearchByNameAsync(string name)
        {
            return await _context.Riders
                .Include(r => r.ApplicationUser)
                .Where(r => r.Name.Contains(name))
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        // 根据车牌号查询骑手
        public async Task<JISpeed.Core.Entities.Rider.Rider?> GetByVehicleNumberAsync(string vehicleNumber)
        {
            return await _context.Riders
                .Include(r => r.ApplicationUser)
                .FirstOrDefaultAsync(r => r.VehicleNumber == vehicleNumber);
        }

        // 检查车牌号是否已存在
        public async Task<bool> ExistsByVehicleNumberAsync(string vehicleNumber)
        {
            return await _context.Riders.AnyAsync(r => r.VehicleNumber == vehicleNumber);
        }

        // 获取有车牌号的骑手
        public async Task<IEnumerable<JISpeed.Core.Entities.Rider.Rider>> GetRidersWithVehicleAsync()
        {
            return await _context.Riders
                .Include(r => r.ApplicationUser)
                .Where(r => !string.IsNullOrEmpty(r.VehicleNumber))
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        // 获取没有车牌号的骑手
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
