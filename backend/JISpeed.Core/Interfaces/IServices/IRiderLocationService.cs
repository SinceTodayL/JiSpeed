using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JISpeed.Core.Entities.Rider;

namespace JISpeed.Core.Interfaces.IServices
{
    // 骑手定位服务接口 - 定义骑手定位模块的业务逻辑操作
    public interface IRiderLocationService
    {
        // 更新骑手位置
        Task<RiderLocation> UpdateRiderLocationAsync(string riderId, decimal longitude, decimal latitude,
            decimal? accuracy = null, decimal? speed = null, decimal? direction = null, string? locationType = null);

        // 获取骑手最新位置
        Task<RiderLocation?> GetRiderLatestLocationAsync(string riderId);

        // 获取骑手历史轨迹
        Task<IEnumerable<RiderLocation>> GetRiderLocationHistoryAsync(string riderId, DateTime startTime, DateTime endTime);

        // 获取指定区域内的骑手
        Task<IEnumerable<RiderLocation>> GetRidersInAreaAsync(decimal minLongitude, decimal maxLongitude,
            decimal minLatitude, decimal maxLatitude);

        // 获取在线骑手位置
        Task<IEnumerable<RiderLocation>> GetOnlineRiderLocationsAsync();

        // 更新骑手在线状态
        Task UpdateRiderStatusAsync(string riderId, int status);

        // 计算骑手到指定位置的距离（米）
        Task<double> CalculateDistanceToLocationAsync(string riderId, decimal longitude, decimal latitude);

        // 获取骑手当前位置的地址信息
        Task<string?> GetRiderLocationAddressAsync(string riderId);

        // 地理编码：根据地址获取经纬度
        Task<GeocodeResult?> GeocodeAddressAsync(string address);
    }
}
