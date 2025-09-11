using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Core.DTOs; 
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace JISpeed.Application.Services.Common
{
    // 高德地图API服务实现
    public class AMapService : IMapService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<AMapService> _logger;

        public AMapService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<AMapService> logger)
        {
            _httpClient = httpClient;
            _apiKey = configuration["AMap:ApiKey"] ?? throw new ArgumentNullException("AMap:ApiKey", "高德地图API密钥未配置");
            _logger = logger;

            // 设置基础URL
            _httpClient.BaseAddress = new Uri("https://restapi.amap.com");
        }

        // 地理编码（地址转坐标）
        public async Task<(decimal longitude, decimal latitude)?> GeocodeAsync(string address)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<JsonElement>(
                    $"/v3/geocode/geo?key={_apiKey}&address={Uri.EscapeDataString(address)}");

                if (response.GetProperty("status").GetString() == "1" &&
                    int.TryParse(response.GetProperty("count").GetString(), out var count) && count > 0)
                {
                    var location = response.GetProperty("geocodes")[0].GetProperty("location").GetString();
                    var parts = location!.Split(',');
                    if (parts.Length == 2 &&
                        decimal.TryParse(parts[0], out var longitude) &&
                        decimal.TryParse(parts[1], out var latitude))
                    {
                        _logger.LogInformation("地理编码成功, Address: {Address}, Longitude: {Longitude}, Latitude: {Latitude}",
                            address, longitude, latitude);
                        return (longitude, latitude);
                    }
                }
                _logger.LogWarning("地理编码未找到结果, Address: {Address}", address);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "地理编码失败, Address: {Address}", address);
                return null;
            }
        }

        // 地理编码（地址转坐标）- 返回详细信息
        public async Task<GeocodeResult?> GeocodeWithDetailsAsync(string address)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<JsonElement>(
                    $"/v3/geocode/geo?key={_apiKey}&address={Uri.EscapeDataString(address)}");

                if (response.GetProperty("status").GetString() == "1" &&
                    int.TryParse(response.GetProperty("count").GetString(), out var count) && count > 0)
                {
                    var geocode = response.GetProperty("geocodes")[0];
                    var location = geocode.GetProperty("location").GetString();
                    var parts = location!.Split(',');

                    if (parts.Length == 2 &&
                        decimal.TryParse(parts[0], out var longitude) &&
                        decimal.TryParse(parts[1], out var latitude))
                    {
                        var result = new GeocodeResult
                        {
                            Longitude = longitude,
                            Latitude = latitude,
                            FormattedAddress = geocode.TryGetProperty("formatted_address", out var formattedAddrProp)
                                ? formattedAddrProp.GetString() : address,
                            Province = geocode.TryGetProperty("province", out var provinceProp)
                                ? provinceProp.GetString() : null,
                            City = geocode.TryGetProperty("city", out var cityProp)
                                ? cityProp.GetString() : null,
                            District = geocode.TryGetProperty("district", out var districtProp)
                                ? districtProp.GetString() : null
                        };

                        _logger.LogInformation("地理编码详细信息成功, Address: {Address}, Longitude: {Longitude}, Latitude: {Latitude}, FormattedAddress: {FormattedAddress}",
                            address, longitude, latitude, result.FormattedAddress);

                        return result;
                    }
                }

                _logger.LogWarning("地理编码未找到结果, Address: {Address}", address);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "地理编码详细信息失败, Address: {Address}", address);
                return null;
            }
        }

        // 逆地理编码（坐标转地址）
        public async Task<string?> ReverseGeocodeAsync(decimal longitude, decimal latitude)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<JsonElement>(
                    $"/v3/geocode/regeo?key={_apiKey}&location={longitude},{latitude}");

                if (response.GetProperty("status").GetString() == "1")
                {
                    return response.GetProperty("regeocode").GetProperty("formatted_address").GetString();
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "逆地理编码失败, Longitude: {Longitude}, Latitude: {Latitude}", longitude, latitude);
                return null;
            }
        }

        // 格式化距离显示
        // 大于等于1000米显示为公里（保留两位小数）
        // 小于1000米显示为米
        // <param name="distanceInMeters">距离（米）</param>
        // <returns>格式化的距离字符串</returns>
        public static string FormatDistance(double distanceInMeters)
        {
            if (distanceInMeters < 0)
            {
                return "大于100公里";
            }

            if (distanceInMeters >= 1000)
            {
                var kilometers = distanceInMeters / 1000.0;
                return $"{kilometers:F2}公里";
            }
            else
            {
                return $"{distanceInMeters:F0}米";
            }
        }

        // 计算两点之间的距离（米）
        public async Task<double> CalculateDistanceAsync(
            decimal startLongitude, decimal startLatitude, decimal endLongitude, decimal endLatitude)
        {
            try
            {
                // 使用步行路径规划API来计算距离
                var response = await _httpClient.GetFromJsonAsync<JsonElement>(
                    $"/v3/direction/walking?key={_apiKey}&origin={startLongitude},{startLatitude}&destination={endLongitude},{endLatitude}");

                // 先记录完整响应用于调试
                _logger.LogInformation("高德API响应: {Response}", response.ToString());

                if (response.GetProperty("status").GetString() == "1")
                {
                    // 检查是否有路径数据
                    if (response.TryGetProperty("route", out var route))
                    {
                        if (route.TryGetProperty("paths", out var paths) && paths.GetArrayLength() > 0)
                        {
                            // 获取第一条路径的距离（字符串类型）
                            var distanceStr = paths[0].GetProperty("distance").GetString();
                            if (double.TryParse(distanceStr, out var distance))
                            {
                                _logger.LogInformation("成功计算距离: {Distance}米", distance);
                                return distance;
                            }
                            else
                            {
                                _logger.LogWarning("无法解析距离字符串: {DistanceStr}", distanceStr);
                                return -1;
                            }
                        }
                    }
                }

                _logger.LogWarning("高德API返回失败，状态: {Status}, 响应: {Response}",
                    response.GetProperty("status").GetString(), response.ToString());
                return -1; // 表示计算失败
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "计算距离失败, StartLong: {StartLong}, StartLat: {StartLat}, EndLong: {EndLong}, EndLat: {EndLat}",
                    startLongitude, startLatitude, endLongitude, endLatitude);
                return -1;
            }
        }

        // 路径规划
        public async Task<IEnumerable<(decimal longitude, decimal latitude)>> PlanRouteAsync(
            decimal startLongitude, decimal startLatitude, decimal endLongitude, decimal endLatitude)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<JsonElement>(
                    $"/v3/direction/walking?key={_apiKey}&origin={startLongitude},{startLatitude}&destination={endLongitude},{endLatitude}");

                var result = new List<(decimal longitude, decimal latitude)>();

                if (response.GetProperty("status").GetString() == "1")
                {
                    var steps = response.GetProperty("route").GetProperty("paths")[0].GetProperty("steps");
                    foreach (var step in steps.EnumerateArray())
                    {
                        var polyline = step.GetProperty("polyline").GetString();
                        var points = polyline!.Split(';');
                        foreach (var point in points)
                        {
                            var coords = point.Split(',');
                            if (coords.Length == 2 &&
                                decimal.TryParse(coords[0], out var longitude) &&
                                decimal.TryParse(coords[1], out var latitude))
                            {
                                result.Add((longitude, latitude));
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "路径规划失败, StartLong: {StartLong}, StartLat: {StartLat}, EndLong: {EndLong}, EndLat: {EndLat}",
                    startLongitude, startLatitude, endLongitude, endLatitude);
                return new List<(decimal longitude, decimal latitude)>();
            }
        }

        // 获取指定位置的行政区划信息
        public async Task<string?> GetDistrictAsync(decimal longitude, decimal latitude)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<JsonElement>(
                    $"/v3/geocode/regeo?key={_apiKey}&location={longitude},{latitude}&extensions=base");

                if (response.GetProperty("status").GetString() == "1")
                {
                    var addressComponent = response.GetProperty("regeocode").GetProperty("addressComponent");
                    var province = addressComponent.GetProperty("province").GetString();
                    var city = addressComponent.GetProperty("city").GetString();
                    var district = addressComponent.GetProperty("district").GetString();

                    return $"{province}{city}{district}";
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取行政区划信息失败, Longitude: {Longitude}, Latitude: {Latitude}", longitude, latitude);
                return null;
            }
        }

        // 获取指定位置的POI信息
        public async Task<IEnumerable<dynamic>> GetNearbyPOIsAsync(decimal longitude, decimal latitude, int radius = 1000)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<JsonElement>(
                    $"/v3/place/around?key={_apiKey}&location={longitude},{latitude}&radius={radius}");

                var result = new List<dynamic>();

                if (response.GetProperty("status").GetString() == "1")
                {
                    var pois = response.GetProperty("pois");
                    foreach (var poi in pois.EnumerateArray())
                    {
                        result.Add(new
                        {
                            Id = poi.GetProperty("id").GetString(),
                            Name = poi.GetProperty("name").GetString(),
                            Type = poi.GetProperty("type").GetString(),
                            Address = poi.GetProperty("address").GetString(),
                            Distance = poi.GetProperty("distance").GetInt32()
                        });
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取POI信息失败, Longitude: {Longitude}, Latitude: {Latitude}, Radius: {Radius}",
                    longitude, latitude, radius);
                return new List<dynamic>();
            }
        }

        // 获取导航路径（包含路况、预计时间）
        public async Task<NavigationRouteInfo> GetNavigationRouteAsync(
            decimal startLongitude, decimal startLatitude,
            decimal endLongitude, decimal endLatitude,
            string mode = "driving")
        {
            try
            {
                // 根据模式选择不同的API端点
                string apiEndpoint = mode switch
                {
                    "driving" => "/v3/direction/driving",
                    "walking" => "/v3/direction/walking",
                    "bicycling" => "/v3/direction/bicycling",
                    "transit" => "/v3/direction/transit/integrated",
                    _ => "/v3/direction/driving"
                };

                // 用 extensions=all 参数获取完整信息
                var response = await _httpClient.GetFromJsonAsync<JsonElement>(
                    $"{apiEndpoint}?key={_apiKey}&origin={startLongitude},{startLatitude}&destination={endLongitude},{endLatitude}&extensions=all&output=json");

                // 记录API响应用于调试
                _logger.LogInformation("高德导航API响应: {Response}", response.ToString());

                // 检查API状态
                if (!response.TryGetProperty("status", out var statusProp) ||
                    statusProp.GetString() != "1")
                {
                    var status = statusProp.GetString() ?? "未知";
                    var info = response.TryGetProperty("info", out var infoProp) ?
                        infoProp.GetString() : "API返回失败";
                    var infocode = response.TryGetProperty("infocode", out var infocodeProp) ?
                        infocodeProp.GetString() : "未知错误码";

                    _logger.LogWarning("高德导航API返回失败, Status: {Status}, Info: {Info}, InfoCode: {InfoCode}",
                        status, info, infocode);

                    throw new InvalidOperationException($"高德地图API错误: {info} (错误码: {infocode})");
                }

                // 获取route信息
                if (!response.TryGetProperty("route", out var routeProp))
                {
                    _logger.LogError("API响应中缺少route字段");
                    throw new InvalidOperationException("API响应格式错误: 缺少route字段");
                }

                var route = routeProp;

                // 获取paths信息
                if (!route.TryGetProperty("paths", out var pathsProp))
                {
                    _logger.LogError("API响应中缺少paths字段");
                    throw new InvalidOperationException("API响应格式错误: 缺少paths字段");
                }

                var paths = pathsProp;

                if (paths.GetArrayLength() == 0)
                {
                    _logger.LogWarning("API响应中paths数组为空");
                    throw new InvalidOperationException("无法获取导航路径: 路径数据为空");
                }

                var path = paths[0];

                // 获取steps信息
                if (!path.TryGetProperty("steps", out var stepsProp))
                {
                    _logger.LogError("API响应中缺少steps字段");
                    throw new InvalidOperationException("API响应格式错误: 缺少steps字段");
                }

                var steps = stepsProp;

                // 获取距离和时长
                var totalDistance = 0.0;
                var estimatedDuration = 0;

                if (path.TryGetProperty("distance", out var pathDistanceProp))
                {
                    if (!double.TryParse(pathDistanceProp.GetString(), out totalDistance))
                    {
                        _logger.LogWarning("无法解析总距离: {DistanceStr}", pathDistanceProp.GetString());
                    }
                }

                if (path.TryGetProperty("duration", out var pathDurationProp))
                {
                    if (!int.TryParse(pathDurationProp.GetString(), out estimatedDuration))
                    {
                        _logger.LogWarning("无法解析总时长: {DurationStr}", pathDurationProp.GetString());
                    }
                }

                var routeInfo = new NavigationRouteInfo
                {
                    RouteId = Guid.NewGuid().ToString(),
                    TotalDistance = totalDistance,
                    EstimatedDuration = estimatedDuration,
                    Mode = mode,
                    Steps = new List<RouteStep>()
                };

                // 解析路径步骤
                foreach (var step in steps.EnumerateArray())
                {
                    var routeStep = new RouteStep
                    {
                        // 获取instruction字段
                        Instruction = step.TryGetProperty("instruction", out var instructionProp) ?
                            instructionProp.GetString() ?? "" : "",

                        // 获取distance字段
                        Distance = step.TryGetProperty("distance", out var stepDistanceProp) ?
                            (double.TryParse(stepDistanceProp.GetString(), out var distance) ? distance : 0) : 0,

                        // 获取duration字段
                        Duration = step.TryGetProperty("duration", out var stepDurationProp) ?
                            (int.TryParse(stepDurationProp.GetString(), out var duration) ? duration : 0) : 0,

                        // 获取road_name字段
                        RoadName = step.TryGetProperty("road_name", out var roadNameProp) ?
                            roadNameProp.GetString() ?? "" : "",

                        // 获取turn_type字段
                        TurnType = step.TryGetProperty("turn_type", out var turnTypeProp) ?
                            turnTypeProp.GetString() ?? "" : ""
                    };

                    // 解析坐标点
                    if (step.TryGetProperty("polyline", out var polylineProp))
                    {
                        var polyline = polylineProp.GetString();
                        if (!string.IsNullOrEmpty(polyline))
                        {
                            var points = polyline.Split(';');
                            foreach (var point in points)
                            {
                                var coords = point.Split(',');
                                if (coords.Length == 2 &&
                                    decimal.TryParse(coords[0], out var longitude) &&
                                    decimal.TryParse(coords[1], out var latitude))
                                {
                                    routeStep.Polyline.Add(new Coordinate
                                    {
                                        Longitude = longitude,
                                        Latitude = latitude
                                    });
                                }
                            }
                        }
                    }

                    routeInfo.Steps.Add(routeStep);
                }

                _logger.LogInformation("成功解析导航路径, 总距离: {Distance}米, 预计时长: {Duration}秒, 步骤数: {StepCount}",
                    totalDistance, estimatedDuration, routeInfo.Steps.Count);

                return routeInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取导航路径失败, StartLong: {StartLong}, StartLat: {StartLat}, EndLong: {EndLong}, EndLat: {EndLat}, Mode: {Mode}",
                    startLongitude, startLatitude, endLongitude, endLatitude, mode);
                throw;
            }
        }
    }
}
