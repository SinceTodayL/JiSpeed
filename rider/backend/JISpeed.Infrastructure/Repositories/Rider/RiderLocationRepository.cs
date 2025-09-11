using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JISpeed.Core.Entities.Rider;
using JISpeed.Core.Interfaces.IRepositories.Rider;
using JISpeed.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace JISpeed.Infrastructure.Repositories.Rider
{
    public class RiderLocationRepository : BaseRepository<RiderLocation, string>, IRiderLocationRepository
    {
        private readonly ILogger<RiderLocationRepository> _logger;

        public RiderLocationRepository(
            OracleDbContext context,
            ILogger<RiderLocationRepository> logger) : base(context)
        {
            _logger = logger;
        }

        // 获取骑手最新位置
        public async Task<RiderLocation?> GetLatestLocationAsync(string riderId)
        {
            try
            {
                return await _context.Set<RiderLocation>()
                    .Where(l => l.RiderId == riderId)
                    .OrderByDescending(l => l.LocationTime)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取骑手最新位置时发生异常, RiderId: {RiderId}", riderId);
                throw;
            }
        }

        // 获取骑手历史轨迹
        public async Task<IEnumerable<RiderLocation>> GetLocationHistoryAsync(string riderId, DateTime startTime, DateTime endTime)
        {
            try
            {
                return await _context.Set<RiderLocation>()
                    .Where(l => l.RiderId == riderId && l.LocationTime >= startTime && l.LocationTime <= endTime)
                    .OrderBy(l => l.LocationTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取骑手历史轨迹时发生异常, RiderId: {RiderId}, StartTime: {StartTime}, EndTime: {EndTime}",
                    riderId, startTime, endTime);
                throw;
            }
        }

        // 获取指定时间范围内的所有骑手位置
        public async Task<IEnumerable<RiderLocation>> GetAllRiderLocationsInTimeRangeAsync(DateTime startTime, DateTime endTime)
        {
            try
            {
                return await _context.Set<RiderLocation>()
                    .Where(l => l.LocationTime >= startTime && l.LocationTime <= endTime)
                    .OrderBy(l => l.RiderId)
                    .ThenBy(l => l.LocationTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取指定时间范围内的所有骑手位置时发生异常, StartTime: {StartTime}, EndTime: {EndTime}",
                    startTime, endTime);
                throw;
            }
        }

        // 获取在线骑手位置
        public async Task<IEnumerable<RiderLocation>> GetOnlineRiderLocationsAsync()
        {
            try
            {
                _logger.LogInformation("开始获取在线骑手位置");
                
                // 先获取所有状态为1的位置记录
                var onlineLocations = await _context.Set<RiderLocation>()
                    .Where(l => l.Status == 1)
                    .OrderByDescending(l => l.LocationTime)
                    .ToListAsync();

                _logger.LogInformation("找到 {Count} 条在线位置记录", onlineLocations.Count);

                // 在内存中按RiderId分组，获取每个骑手的最新位置
                var latestByRider = onlineLocations
                    .GroupBy(l => l.RiderId)
                    .Select(g => g.First()) // 由于已经按时间降序排列，First()就是最新的
                    .ToList();

                _logger.LogInformation("找到 {Count} 个在线骑手", latestByRider.Count);
                return latestByRider;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取在线骑手位置时发生异常");
                throw;
            }
        }

        // 获取指定区域内的骑手位置
        public async Task<IEnumerable<RiderLocation>> GetRiderLocationsInAreaAsync(
            decimal minLongitude, decimal maxLongitude, decimal minLatitude, decimal maxLatitude)
        {
            try
            {
                // 获取每个骑手的最新位置
                var allLatestLocations = await _context.Set<RiderLocation>()
                    .GroupBy(l => l.RiderId)
                    .Select(g => g.OrderByDescending(l => l.LocationTime).FirstOrDefault())
                    .Where(l => l != null)
                    .Cast<RiderLocation>() // 明确转换为非空类型
                    .ToListAsync();

                // 筛选出在指定区域内的位置（移除空引用断言操作符）
                var locationsInArea = allLatestLocations
                    .Where(l => l.Longitude >= minLongitude && l.Longitude <= maxLongitude &&
                                l.Latitude >= minLatitude && l.Latitude <= maxLatitude)
                    .ToList();

                return locationsInArea;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取指定区域内的骑手位置时发生异常, MinLong: {MinLong}, MaxLong: {MaxLong}, MinLat: {MinLat}, MaxLat: {MaxLat}",
                    minLongitude, maxLongitude, minLatitude, maxLatitude);
                throw;
            }
        }

        // 更新骑手状态
        public async Task UpdateRiderStatusAsync(string riderId, int status)
        {
            try
            {
                // 获取骑手最新位置
                var latestLocation = await GetLatestLocationAsync(riderId);

                if (latestLocation != null)
                {
                    // 更新状态
                    latestLocation.Status = status;
                    _context.Update(latestLocation);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新骑手状态时发生异常, RiderId: {RiderId}, Status: {Status}",
                    riderId, status);
                throw;
            }
        }

        // 批量保存骑手位置
        public async Task<IEnumerable<RiderLocation>> BatchSaveLocationsAsync(IEnumerable<RiderLocation> locations)
        {
            try
            {
                foreach (var location in locations)
                {
                    await _context.AddAsync(location);
                }

                await _context.SaveChangesAsync();
                return locations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "批量保存骑手位置时发生异常, LocationCount: {LocationCount}",
                    locations.Count());
                throw;
            }
        }
    }
}
