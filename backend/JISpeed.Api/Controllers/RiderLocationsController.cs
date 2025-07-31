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

namespace JISpeed.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RiderLocationsController : ControllerBase
    {
        private readonly IRiderLocationService _riderLocationService;
        private readonly ILogger<RiderLocationsController> _logger;

        public RiderLocationsController(
            IRiderLocationService riderLocationService,
            ILogger<RiderLocationsController> logger)
        {
            _riderLocationService = riderLocationService;
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

                return Ok(ApiResponse<object>.Success(null, "骑手在线状态更新成功"));
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
                    Distance = distance
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
    }
}
