using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JISpeed.Api.Common;
using JISpeed.Api.DTOS;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Constants;
using Microsoft.Extensions.Logging;
using JISpeed.Api.DTOs;
using JISpeed.Core.Entities.Rider;

namespace JISpeed.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RiderLocationsController : ControllerBase
    {
        private readonly IRiderLocationService _riderLocationService;
        private readonly ILocationPushService _locationPushService;  // 添加这行
        private readonly ILogger<RiderLocationsController> _logger;

        public RiderLocationsController(
            IRiderLocationService riderLocationService,
            ILocationPushService locationPushService,  // 添加这行
            ILogger<RiderLocationsController> logger)
        {
            _riderLocationService = riderLocationService;
            _locationPushService = locationPushService;  // 添加这行
            _logger = logger;
        }

        // 更新骑手位置
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<RiderLocationDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<RiderLocationDTO>>> UpdateRiderLocation([FromBody] UpdateRiderLocationDTO dto)
        {
            try
            {
                _logger.LogInformation("收到更新骑手位置请求, RiderId: {RiderId}, Longitude: {Longitude}, Latitude: {Latitude}",
                    dto.RiderId, dto.Longitude, dto.Latitude);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.ValidationFailed,
                        "参数验证失败",
                        ModelState));
                }

                var location = await _riderLocationService.UpdateRiderLocationAsync(
                    dto.RiderId,
                    dto.Longitude,
                    dto.Latitude,
                    dto.Accuracy,
                    dto.Speed,
                    dto.Direction,
                    dto.LocationType);

                var locationDto = new RiderLocationDTO
                {
                    LocationId = location.LocationId,
                    RiderId = location.RiderId,
                    Longitude = location.Longitude,
                    Latitude = location.Latitude,
                    LocationTime = location.LocationTime,
                    Accuracy = location.Accuracy,
                    Speed = location.Speed,
                    Direction = location.Direction,
                    LocationType = location.LocationType,
                    Status = location.Status
                };

                return Ok(ApiResponse<RiderLocationDTO>.Success(locationDto, "骑手位置更新成功"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败");
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "资源不存在");
                return NotFound(ApiResponse<object>.Fail(
                    ex.ErrorCode,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新骑手位置时发生异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "更新骑手位置失败"));
            }
        }

        // 获取骑手最新位置
        [HttpGet("{riderId}/latest")]
        [ProducesResponseType(typeof(ApiResponse<RiderLocationDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<RiderLocationDTO>>> GetRiderLatestLocation(string riderId)
        {
            try
            {
                _logger.LogInformation("收到获取骑手最新位置请求, RiderId: {RiderId}", riderId);

                var location = await _riderLocationService.GetRiderLatestLocationAsync(riderId);

                if (location == null)
                {
                    return NotFound(ApiResponse<object>.Fail(
                        ErrorCodes.ResourceNotFound,
                        $"骑手 (ID: {riderId}) 没有位置记录"));
                }

                var locationDto = new RiderLocationDTO
                {
                    LocationId = location.LocationId,
                    RiderId = location.RiderId,
                    Longitude = location.Longitude,
                    Latitude = location.Latitude,
                    LocationTime = location.LocationTime,
                    Accuracy = location.Accuracy,
                    Speed = location.Speed,
                    Direction = location.Direction,
                    LocationType = location.LocationType,
                    Status = location.Status
                };

                return Ok(ApiResponse<RiderLocationDTO>.Success(locationDto));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "骑手不存在, RiderId: {RiderId}", riderId);
                return NotFound(ApiResponse<object>.Fail(
                    ex.ErrorCode,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取骑手最新位置时发生异常, RiderId: {RiderId}", riderId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "获取骑手最新位置失败"));
            }
        }

        // 获取骑手历史轨迹
        [HttpGet("{riderId}/history")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<RiderLocationDTO>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<RiderLocationDTO>>>> GetRiderLocationHistory(
            string riderId,
            [FromQuery] DateTime startTime,
            [FromQuery] DateTime endTime)
        {
            try
            {
                _logger.LogInformation("收到获取骑手历史轨迹请求, RiderId: {RiderId}, StartTime: {StartTime}, EndTime: {EndTime}",
                    riderId, startTime, endTime);

                var locations = await _riderLocationService.GetRiderLocationHistoryAsync(riderId, startTime, endTime);

                var locationDtos = new List<RiderLocationDTO>();
                foreach (var location in locations)
                {
                    locationDtos.Add(new RiderLocationDTO
                    {
                        LocationId = location.LocationId,
                        RiderId = location.RiderId,
                        Longitude = location.Longitude,
                        Latitude = location.Latitude,
                        LocationTime = location.LocationTime,
                        Accuracy = location.Accuracy,
                        Speed = location.Speed,
                        Direction = location.Direction,
                        LocationType = location.LocationType,
                        Status = location.Status
                    });
                }

                return Ok(ApiResponse<IEnumerable<RiderLocationDTO>>.Success(locationDtos));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败");
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "骑手不存在, RiderId: {RiderId}", riderId);
                return NotFound(ApiResponse<object>.Fail(
                    ex.ErrorCode,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取骑手历史轨迹时发生异常, RiderId: {RiderId}", riderId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "获取骑手历史轨迹失败"));
            }
        }

        // 获取指定区域内的骑手
        [HttpGet("area")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<RiderLocationDTO>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<RiderLocationDTO>>>> GetRidersInArea(
            [FromQuery] decimal minLongitude,
            [FromQuery] decimal maxLongitude,
            [FromQuery] decimal minLatitude,
            [FromQuery] decimal maxLatitude)
        {
            try
            {
                _logger.LogInformation("收到获取指定区域内的骑手请求, MinLong: {MinLong}, MaxLong: {MaxLong}, MinLat: {MinLat}, MaxLat: {MaxLat}",
                    minLongitude, maxLongitude, minLatitude, maxLatitude);

                var locations = await _riderLocationService.GetRidersInAreaAsync(
                    minLongitude, maxLongitude, minLatitude, maxLatitude);

                var locationDtos = new List<RiderLocationDTO>();
                foreach (var location in locations)
                {
                    locationDtos.Add(new RiderLocationDTO
                    {
                        LocationId = location.LocationId,
                        RiderId = location.RiderId,
                        Longitude = location.Longitude,
                        Latitude = location.Latitude,
                        LocationTime = location.LocationTime,
                        Accuracy = location.Accuracy,
                        Speed = location.Speed,
                        Direction = location.Direction,
                        LocationType = location.LocationType,
                        Status = location.Status
                    });
                }

                return Ok(ApiResponse<IEnumerable<RiderLocationDTO>>.Success(locationDtos));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败");
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取指定区域内的骑手时发生异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "获取指定区域内的骑手失败"));
            }
        }

        // 获取在线骑手位置
        [HttpGet("online")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<RiderLocationDTO>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<RiderLocationDTO>>>> GetOnlineRiderLocations()
        {
            try
            {
                _logger.LogInformation("收到获取在线骑手位置请求");

                var locations = await _riderLocationService.GetOnlineRiderLocationsAsync();

                var locationDtos = new List<RiderLocationDTO>();
                foreach (var location in locations)
                {
                    locationDtos.Add(new RiderLocationDTO
                    {
                        LocationId = location.LocationId,
                        RiderId = location.RiderId,
                        Longitude = location.Longitude,
                        Latitude = location.Latitude,
                        LocationTime = location.LocationTime,
                        Accuracy = location.Accuracy,
                        Speed = location.Speed,
                        Direction = location.Direction,
                        LocationType = location.LocationType,
                        Status = location.Status
                    });
                }

                return Ok(ApiResponse<IEnumerable<RiderLocationDTO>>.Success(locationDtos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取在线骑手位置时发生异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "获取在线骑手位置失败"));
            }
        }

        // 更新骑手在线状态
        [HttpPatch("{riderId}/status")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<object>>> UpdateRiderStatus(
            string riderId,
            [FromBody] UpdateRiderStatusDTO dto)
        {
            try
            {
                _logger.LogInformation("收到更新骑手在线状态请求, RiderId: {RiderId}, Status: {Status}",
                    riderId, dto.Status);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.ValidationFailed,
                        "参数验证失败",
                        ModelState));
                }

                await _riderLocationService.UpdateRiderStatusAsync(riderId, dto.Status);

                return Ok(ApiResponse<object>.Success(new object(), "骑手在线状态更新成功"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败");
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "骑手不存在, RiderId: {RiderId}", riderId);
                return NotFound(ApiResponse<object>.Fail(
                    ex.ErrorCode,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新骑手在线状态时发生异常, RiderId: {RiderId}", riderId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "更新骑手在线状态失败"));
            }
        }

        // 计算骑手到指定位置的距离
        [HttpGet("{riderId}/distance")]
        [ProducesResponseType(typeof(ApiResponse<DistanceDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<DistanceDTO>>> CalculateDistanceToLocation(
            string riderId,
            [FromQuery] decimal longitude,
            [FromQuery] decimal latitude)
        {
            try
            {
                _logger.LogInformation("收到计算骑手到指定位置的距离请求, RiderId: {RiderId}, Longitude: {Longitude}, Latitude: {Latitude}",
                    riderId, longitude, latitude);

                var distance = await _riderLocationService.CalculateDistanceToLocationAsync(riderId, longitude, latitude);

                var distanceDto = new DistanceDTO
                {
                    RiderId = riderId,
                    TargetLongitude = longitude,
                    TargetLatitude = latitude,
                    Distance = distance,
                    FormattedDistance = JISpeed.Application.Services.Common.AMapService.FormatDistance(distance)
                };

                return Ok(ApiResponse<DistanceDTO>.Success(distanceDto));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败");
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "骑手不存在, RiderId: {RiderId}", riderId);
                return NotFound(ApiResponse<object>.Fail(
                    ex.ErrorCode,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning(ex, "业务规则验证失败, RiderId: {RiderId}", riderId);
                return BadRequest(ApiResponse<object>.Fail(
                    ex.ErrorCode,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "计算骑手到指定位置的距离时发生异常, RiderId: {RiderId}", riderId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "计算骑手到指定位置的距离失败"));
            }
        }

        // 获取骑手当前位置的地址信息
        [HttpGet("{riderId}/address")]
        [ProducesResponseType(typeof(ApiResponse<AddressDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<AddressDTO>>> GetRiderLocationAddress(string riderId)
        {
            try
            {
                _logger.LogInformation("收到获取骑手当前位置的地址信息请求, RiderId: {RiderId}", riderId);

                var address = await _riderLocationService.GetRiderLocationAddressAsync(riderId);

                if (address == null)
                {
                    return NotFound(ApiResponse<object>.Fail(
                        ErrorCodes.ResourceNotFound,
                        $"无法获取骑手 (ID: {riderId}) 的地址信息"));
                }

                var addressDto = new AddressDTO
                {
                    RiderId = riderId,
                    Address = address
                };

                return Ok(ApiResponse<AddressDTO>.Success(addressDto));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "骑手不存在, RiderId: {RiderId}", riderId);
                return NotFound(ApiResponse<object>.Fail(
                    ex.ErrorCode,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning(ex, "业务规则验证失败, RiderId: {RiderId}", riderId);
                return BadRequest(ApiResponse<object>.Fail(
                    ex.ErrorCode,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取骑手当前位置的地址信息时发生异常, RiderId: {RiderId}", riderId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "获取骑手当前位置的地址信息失败"));
            }
        }

        /// 测试位置推送功能

        [HttpPost("test-push/{riderId}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<ActionResult<ApiResponse<object>>> TestLocationPush(string riderId)
        {
            try
            {
                // 创建测试位置数据
                var testLocation = new RiderLocation(
                    riderId,
                    116.397428m,
                    39.90923m,
                    DateTime.Now)
                {
                    LocationId = Guid.NewGuid().ToString("N"),
                    RiderId = riderId,
                    Longitude = 116.397428m,
                    Latitude = 39.90923m,
                    LocationTime = DateTime.Now,
                    Accuracy = 10.0m,
                    Speed = 15.5m,
                    Direction = 90m,
                    LocationType = "GPS",
                    Status = 1,
                    Rider = new JISpeed.Core.Entities.Rider.Rider(
                        riderId,
                        "测试骑手",
                        "13800000000")
                    {
                        RiderId = riderId,
                        Name = "测试骑手",
                        PhoneNumber = "13800000000"
                    }
                };

                await _locationPushService.PushLocationUpdateAsync(riderId, testLocation);

                return Ok(ApiResponse<object>.Success(new
                {
                    riderId,
                    message = "位置推送测试消息已发送",
                    location = new
                    {
                        longitude = testLocation.Longitude,
                        latitude = testLocation.Latitude,
                        accuracy = testLocation.Accuracy,
                        speed = testLocation.Speed,
                        direction = testLocation.Direction,
                        locationTime = testLocation.LocationTime
                    }
                }, "测试推送发送成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "测试位置推送失败, RiderId: {RiderId}", riderId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "测试推送失败",
                    ex.Message));
            }
        }
        // 调试：检查特定骑手的位置信息
        [HttpGet("debug/{riderId}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<ActionResult<ApiResponse<object>>> DebugRiderLocation(string riderId)
        {
            try
            {
                _logger.LogInformation("调试骑手位置信息，RiderId: {RiderId}", riderId);

                // 1. 检查骑手的最新位置
                var latestLocation = await _riderLocationService.GetRiderLatestLocationAsync(riderId);

                // 2. 获取该骑手的所有位置记录
                var allLocations = await _riderLocationService.GetRiderLocationHistoryAsync(
                    riderId,
                    DateTime.Now.AddDays(-30),
                    DateTime.Now);

                // 3. 检查在线骑手（简化查询）
                var onlineLocations = await _riderLocationService.GetOnlineRiderLocationsAsync();

                var debugInfo = new
                {
                    RiderId = riderId,
                    LatestLocation = latestLocation == null ? null : new
                    {
                        latestLocation.LocationId,
                        latestLocation.Status,
                        latestLocation.LocationTime,
                        latestLocation.Longitude,
                        latestLocation.Latitude
                    },
                    LocationCount = allLocations.Count(),
                    OnlineRiderCount = onlineLocations.Count(),
                    AllLocationsStatus = allLocations.Select(l => new { l.LocationId, l.Status, l.LocationTime }).ToList()
                };

                return Ok(ApiResponse<object>.Success(debugInfo));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "调试骑手位置信息时发生异常，RiderId: {RiderId}", riderId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    $"调试失败: {ex.Message}"));
            }
        }

        // 地理编码：根据地址获取经纬度
        [HttpPost("geocode")]
        [ProducesResponseType(typeof(ApiResponse<GeocodeDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<GeocodeDTO>>> GeocodeAddress([FromBody] GeocodeRequestDTO request)
        {
            try
            {
                _logger.LogInformation("收到地理编码请求, Address: {Address}", request.Address);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.ValidationFailed,
                        "参数验证失败",
                        ModelState));
                }

                // 使用高德地图API进行地理编码
                var geocodeResult = await _riderLocationService.GeocodeAddressAsync(request.Address);

                if (geocodeResult == null)
                {
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.ResourceNotFound,
                        "无法找到该地址的坐标信息"));
                }

                var geocodeDto = new GeocodeDTO
                {
                    Address = request.Address,
                    Longitude = geocodeResult.Longitude,
                    Latitude = geocodeResult.Latitude,
                    FormattedAddress = geocodeResult.FormattedAddress,
                    Province = geocodeResult.Province,
                    City = geocodeResult.City,
                    District = geocodeResult.District
                };

                return Ok(ApiResponse<GeocodeDTO>.Success(geocodeDto, "地理编码成功"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败");
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "地理编码时发生异常, Address: {Address}", request.Address);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "地理编码失败"));
            }
        }
    }
}

