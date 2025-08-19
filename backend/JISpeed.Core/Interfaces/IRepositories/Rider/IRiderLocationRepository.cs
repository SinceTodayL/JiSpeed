using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JISpeed.Core.Entities.Rider;

namespace JISpeed.Core.Interfaces.IRepositories.Rider
{
    // 骑手定位仓储接口 - 处理骑手位置数据的数据访问操作
    public interface IRiderLocationRepository : IBaseRepository<RiderLocation, string>
    {
        // === 业务专用查询方法 ===

        // 获取骑手最新位置
        Task<RiderLocation?> GetLatestLocationAsync(string riderId);

        // 获取骑手历史轨迹
        Task<IEnumerable<RiderLocation>> GetLocationHistoryAsync(string riderId, DateTime startTime, DateTime endTime);

        // 获取指定时间范围内的所有骑手位置
        Task<IEnumerable<RiderLocation>> GetAllRiderLocationsInTimeRangeAsync(DateTime startTime, DateTime endTime);

        // 获取在线骑手位置
        Task<IEnumerable<RiderLocation>> GetOnlineRiderLocationsAsync();

        // 获取指定区域内的骑手位置
        Task<IEnumerable<RiderLocation>> GetRiderLocationsInAreaAsync(decimal minLongitude, decimal maxLongitude, decimal minLatitude, decimal maxLatitude);

        // 更新骑手状态
        Task UpdateRiderStatusAsync(string riderId, int status);

        // 批量保存骑手位置
        Task<IEnumerable<RiderLocation>> BatchSaveLocationsAsync(IEnumerable<RiderLocation> locations);
    }
}
