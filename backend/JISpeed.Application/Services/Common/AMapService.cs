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
                    response.GetProperty("count").GetInt32() > 0)
                {
                    var location = response.GetProperty("geocodes")[0].GetProperty("location").GetString();
                    var parts = location!.Split(',');
                    if (parts.Length == 2 &&
                        decimal.TryParse(parts[0], out var longitude) &&
                        decimal.TryParse(parts[1], out var latitude))
                    {
                        return (longitude, latitude);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "地理编码失败, Address: {Address}", address);
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

                var response = await _httpClient.GetFromJsonAsync<JsonElement>(
                    $"{apiEndpoint}?key={_apiKey}&origin={startLongitude},{startLatitude}&destination={endLongitude},{endLatitude}");

                if (response.GetProperty("status").GetString() == "1")
                {
                    var route = response.GetProperty("route");
                    var paths = route.GetProperty("paths");
                    
                    if (paths.GetArrayLength() > 0)
                    {
                        var path = paths[0];
                        var steps = path.GetProperty("steps");
                        
                        var routeInfo = new NavigationRouteInfo
                        {
                            RouteId = Guid.NewGuid().ToString(),
                            TotalDistance = double.Parse(path.GetProperty("distance").GetString() ?? "0"),
                            EstimatedDuration = int.Parse(path.GetProperty("duration").GetString() ?? "0"),
                            Mode = mode,
                            Steps = new List<RouteStep>()
                        };

                        // 解析路径步骤
                        foreach (var step in steps.EnumerateArray())
                        {
                            var routeStep = new RouteStep
                            {
                                Instruction = step.GetProperty("instruction").GetString() ?? "",
                                Distance = double.Parse(step.GetProperty("distance").GetString() ?? "0"),
                                Duration = int.Parse(step.GetProperty("duration").GetString() ?? "0"),
                                RoadName = step.GetProperty("road_name").GetString() ?? "",
                                TurnType = step.GetProperty("turn_type").GetString() ?? ""
                            };

                            // 解析坐标点
                            var polyline = step.GetProperty("polyline").GetString();
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
                                        routeStep.Polyline.Add((longitude, latitude));
                                    }
                                }
                            }

                            routeInfo.Steps.Add(routeStep);
                        }

                        return routeInfo;
                    }
                }

                throw new InvalidOperationException("无法获取导航路径信息");
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
