using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JISpeed.Core.Entities.Rider;
using JISpeed.Core.Interfaces.IRepositories.Rider;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Core.Exceptions;
using Microsoft.Extensions.Logging;
using JISpeed.Core.Constants;

namespace JISpeed.Application.Services.Rider
{
    public class RiderLocationService : IRiderLocationService
    {
        private readonly IRiderLocationRepository _riderLocationRepository;
        private readonly IRiderRepository _riderRepository;
        private readonly IMapService _mapService;
        private readonly ILocationPushService _locationPushService;
        private readonly ILogger<RiderLocationService> _logger;

        public RiderLocationService(
            IRiderLocationRepository riderLocationRepository,
            IRiderRepository riderRepository,
            IMapService mapService,
            ILocationPushService locationPushService,
            ILogger<RiderLocationService> logger)
        {
            _riderLocationRepository = riderLocationRepository;
            _riderRepository = riderRepository;
            _mapService = mapService;
            _locationPushService = locationPushService;
            _logger = logger;
        }

        // 更新骑手位置
        public async Task<RiderLocation> UpdateRiderLocationAsync(
            string riderId, decimal longitude, decimal latitude,
            decimal? accuracy = null, decimal? speed = null, decimal? direction = null, string? locationType = null)
        {
            try
            {
                _logger.LogInformation("开始更新骑手位置, RiderId: {RiderId}, Longitude: {Longitude}, Latitude: {Latitude}",
                    riderId, longitude, latitude);

                // 验证骑手是否存在
                var rider = await _riderRepository.GetByIdAsync(riderId);
                if (rider == null)
                {
                    _logger.LogWarning("骑手不存在, RiderId: {RiderId}", riderId);
                    throw RiderExceptions.RiderNotFound(riderId);
                }

                // 创建位置记录
                var location = new RiderLocation(
                    riderId,
                    longitude,
                    latitude,
                    DateTime.Now)
                {
                    LocationId = Guid.NewGuid().ToString("N"),
                    RiderId = riderId,
                    Longitude = longitude,
                    Latitude = latitude,
                    LocationTime = DateTime.Now,
                    Accuracy = accuracy,
                    Speed = speed,
                    Direction = direction,
                    LocationType = locationType,
                    Status = 1, // 默认在线
                    Rider = rider
                };

                // 保存位置记录
                await _riderLocationRepository.CreateAsync(location);
                await _riderLocationRepository.SaveChangesAsync();

                // 实时推送位置更新
                try
                {
                    await _locationPushService.PushLocationUpdateAsync(riderId, location);
                }
                catch (Exception pushEx)
                {
                    // 推送失败不影响位置保存，只记录日志
                    _logger.LogWarning(pushEx, "位置推送失败, RiderId: {RiderId}, LocationId: {LocationId}", 
                        riderId, location.LocationId);
                }

                _logger.LogInformation("骑手位置更新成功, RiderId: {RiderId}, LocationId: {LocationId}",
                    riderId, location.LocationId);

                return location;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "更新骑手位置时发生异常, RiderId: {RiderId}", riderId);
                throw CommonExceptions.GeneralError($"更新骑手位置失败: {ex.Message}");
            }
        }

        // 获取骑手最新位置
        public async Task<RiderLocation?> GetRiderLatestLocationAsync(string riderId)
        {
            try
            {
                _logger.LogInformation("开始获取骑手最新位置, RiderId: {RiderId}", riderId);

                // 验证骑手是否存在
                var riderExists = await _riderRepository.GetByIdAsync(riderId) != null;
                if (!riderExists)
                {
                    _logger.LogWarning("骑手不存在, RiderId: {RiderId}", riderId);
                    throw RiderExceptions.RiderNotFound(riderId);
                }

                // 获取最新位置
                var location = await _riderLocationRepository.GetLatestLocationAsync(riderId);

                _logger.LogInformation("获取骑手最新位置成功, RiderId: {RiderId}, Found: {Found}",
                    riderId, location != null);

                return location;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取骑手最新位置时发生异常, RiderId: {RiderId}", riderId);
                throw CommonExceptions.GeneralError($"获取骑手最新位置失败: {ex.Message}");
            }
        }

        // 获取骑手历史轨迹
        public async Task<IEnumerable<RiderLocation>> GetRiderLocationHistoryAsync(
            string riderId, DateTime startTime, DateTime endTime)
        {
            try
            {
                _logger.LogInformation("开始获取骑手历史轨迹, RiderId: {RiderId}, StartTime: {StartTime}, EndTime: {EndTime}",
                    riderId, startTime, endTime);

                // 验证骑手是否存在
                var riderExists = await _riderRepository.GetByIdAsync(riderId) != null;
                if (!riderExists)
                {
                    _logger.LogWarning("骑手不存在, RiderId: {RiderId}", riderId);
                    throw RiderExceptions.RiderNotFound(riderId);
                }

                // 验证时间范围
                if (startTime > endTime)
                {
                    throw CommonExceptions.ValidationFailed("timeRange", "开始时间不能晚于结束时间");
                }

                // 获取历史轨迹
                var locations = await _riderLocationRepository.GetLocationHistoryAsync(riderId, startTime, endTime);

                _logger.LogInformation("获取骑手历史轨迹成功, RiderId: {RiderId}, LocationCount: {LocationCount}",
                    riderId, locations.Count());

                return locations;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取骑手历史轨迹时发生异常, RiderId: {RiderId}", riderId);
                throw CommonExceptions.GeneralError($"获取骑手历史轨迹失败: {ex.Message}");
            }
        }

        // 获取指定区域内的骑手
        public async Task<IEnumerable<RiderLocation>> GetRidersInAreaAsync(
            decimal minLongitude, decimal maxLongitude, decimal minLatitude, decimal maxLatitude)
        {
            try
            {
                _logger.LogInformation("开始获取指定区域内的骑手, MinLong: {MinLong}, MaxLong: {MaxLong}, MinLat: {MinLat}, MaxLat: {MaxLat}",
                    minLongitude, maxLongitude, minLatitude, maxLatitude);

                // 验证区域范围
                if (minLongitude > maxLongitude || minLatitude > maxLatitude)
                {
                    throw CommonExceptions.ValidationFailed("area", "区域范围无效");
                }

                // 获取区域内的骑手位置
                var locations = await _riderLocationRepository.GetRiderLocationsInAreaAsync(
                    minLongitude, maxLongitude, minLatitude, maxLatitude);

                _logger.LogInformation("获取指定区域内的骑手成功, LocationCount: {LocationCount}", locations.Count());

                return locations;
            }
            catch (Exception ex) when (!(ex is ValidationException))
            {
                _logger.LogError(ex, "获取指定区域内的骑手时发生异常");
                throw CommonExceptions.GeneralError($"获取指定区域内的骑手失败: {ex.Message}");
            }
        }

        // 获取在线骑手位置
        public async Task<IEnumerable<RiderLocation>> GetOnlineRiderLocationsAsync()
        {
            try
            {
                _logger.LogInformation("开始获取在线骑手位置");

                // 获取在线骑手位置
                var locations = await _riderLocationRepository.GetOnlineRiderLocationsAsync();

                _logger.LogInformation("获取在线骑手位置成功, LocationCount: {LocationCount}", locations.Count());

                return locations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取在线骑手位置时发生异常");
                throw CommonExceptions.GeneralError($"获取在线骑手位置失败: {ex.Message}");
            }
        }

        // 更新骑手在线状态
        public async Task UpdateRiderStatusAsync(string riderId, int status)
        {
            try
            {
                _logger.LogInformation("开始更新骑手在线状态, RiderId: {RiderId}, Status: {Status}",
                    riderId, status);

                // 验证骑手是否存在
                var riderExists = await _riderRepository.GetByIdAsync(riderId) != null;
                if (!riderExists)
                {
                    _logger.LogWarning("骑手不存在, RiderId: {RiderId}", riderId);
                    throw RiderExceptions.RiderNotFound(riderId);
                }

                // 验证状态值
                if (status != 0 && status != 1)
                {
                    throw CommonExceptions.ValidationFailed("status", "状态值无效，应为0(离线)或1(在线)");
                }

                // 更新骑手状态
                await _riderLocationRepository.UpdateRiderStatusAsync(riderId, status);

                _logger.LogInformation("更新骑手在线状态成功, RiderId: {RiderId}, Status: {Status}",
                    riderId, status);
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "更新骑手在线状态时发生异常, RiderId: {RiderId}", riderId);
                throw CommonExceptions.GeneralError($"更新骑手在线状态失败: {ex.Message}");
            }
        }

        // 计算骑手到指定位置的距离（米）
        public async Task<double> CalculateDistanceToLocationAsync(string riderId, decimal longitude, decimal latitude)
        {
            try
            {
                _logger.LogInformation("开始计算骑手到指定位置的距离, RiderId: {RiderId}, Longitude: {Longitude}, Latitude: {Latitude}",
                    riderId, longitude, latitude);

                // 获取骑手最新位置
                var location = await GetRiderLatestLocationAsync(riderId);
                if (location == null)
                {
                    _logger.LogWarning("骑手没有位置记录, RiderId: {RiderId}", riderId);
                    throw new BusinessException(ErrorCodes.ResourceNotFound, $"骑手 (ID: {riderId}) 没有位置记录");
                }

                // 使用高德地图API计算距离
                var distance = await _mapService.CalculateDistanceAsync(
                    location.Longitude, location.Latitude, longitude, latitude);

                _logger.LogInformation("计算骑手到指定位置的距离成功, RiderId: {RiderId}, Distance: {Distance}米",
                    riderId, distance);

                return distance;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException || ex is BusinessException))
            {
                _logger.LogError(ex, "计算骑手到指定位置的距离时发生异常, RiderId: {RiderId}", riderId);
                throw CommonExceptions.GeneralError($"计算骑手到指定位置的距离失败: {ex.Message}");
            }
        }

        // 获取骑手当前位置的地址信息
        public async Task<string?> GetRiderLocationAddressAsync(string riderId)
        {
            try
            {
                _logger.LogInformation("开始获取骑手当前位置的地址信息, RiderId: {RiderId}", riderId);

                // 获取骑手最新位置
                var location = await GetRiderLatestLocationAsync(riderId);
                if (location == null)
                {
                    _logger.LogWarning("骑手没有位置记录, RiderId: {RiderId}", riderId);
                    throw new BusinessException(ErrorCodes.ResourceNotFound, $"骑手 (ID: {riderId}) 没有位置记录");
                }

                // 使用高德地图API进行逆地理编码
                var address = await _mapService.ReverseGeocodeAsync(location.Longitude, location.Latitude);

                _logger.LogInformation("获取骑手当前位置的地址信息成功, RiderId: {RiderId}, Address: {Address}",
                    riderId, address);

                return address;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException || ex is BusinessException))
            {
                _logger.LogError(ex, "获取骑手当前位置的地址信息时发生异常, RiderId: {RiderId}", riderId);
                throw CommonExceptions.GeneralError($"获取骑手当前位置的地址信息失败: {ex.Message}");
            }
        }
    }
}
