using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using JISpeed.Core.Interfaces.IServices;
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

        // 计算两点之间的距离（米）
        public async Task<double> CalculateDistanceAsync(
            decimal startLongitude, decimal startLatitude, decimal endLongitude, decimal endLatitude)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<JsonElement>(
                    $"/v3/distance?key={_apiKey}&origins={startLongitude},{startLatitude}&destination={endLongitude},{endLatitude}&type=1");

                if (response.GetProperty("status").GetString() == "1" &&
                    response.GetProperty("count").GetInt32() > 0)
                {
                    return response.GetProperty("results")[0].GetProperty("distance").GetDouble();
                }

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
    }
}
