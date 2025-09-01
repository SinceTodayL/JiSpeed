using System.Collections.Generic;
using System.Threading.Tasks;
using JISpeed.Core.DTOs;

namespace JISpeed.Core.Interfaces.IServices
{
    // 地图服务接口 - 处理地图相关操作
    public interface IMapService
    {
        // 地理编码（地址转坐标）
        Task<(decimal longitude, decimal latitude)?> GeocodeAsync(string address);

        // 逆地理编码（坐标转地址）
        Task<string?> ReverseGeocodeAsync(decimal longitude, decimal latitude);

        // 计算两点之间的距离（米）
        Task<double> CalculateDistanceAsync(decimal startLongitude, decimal startLatitude, decimal endLongitude, decimal endLatitude);

        // 路径规划
        Task<IEnumerable<(decimal longitude, decimal latitude)>> PlanRouteAsync(
            decimal startLongitude, decimal startLatitude, decimal endLongitude, decimal endLatitude);

        // 获取指定位置的行政区划信息
        Task<string?> GetDistrictAsync(decimal longitude, decimal latitude);

        // 获取指定位置的POI信息
        Task<IEnumerable<dynamic>> GetNearbyPOIsAsync(decimal longitude, decimal latitude, int radius = 1000);

        // 获取导航路径（包含路况、预计时间）
        Task<NavigationRouteInfo> GetNavigationRouteAsync(
            decimal startLongitude, decimal startLatitude, 
            decimal endLongitude, decimal endLatitude, 
            string mode = "driving");
    }
}
