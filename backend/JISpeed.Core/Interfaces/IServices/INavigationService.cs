using System.Collections.Generic;
using System.Threading.Tasks;
using JISpeed.Core.DTOs;

namespace JISpeed.Core.Interfaces.IServices
{
    // 导航服务接口 - 处理导航相关业务逻辑
    public interface INavigationService
    {
        // 获取实时导航路径
        Task<NavigationRouteInfo> GetRealTimeNavigationAsync(
            decimal startLongitude, decimal startLatitude,
            decimal endLongitude, decimal endLatitude,
            string mode = "driving");

        // 获取导航路线信息
        Task<NavigationRouteInfo> GetNavigationRouteAsync(NavigationRouteRequest request);
        
        // 获取实时导航更新（新增）
        Task<NavigationUpdateInfo> GetNavigationUpdatesAsync(string routeId);

        // 获取历史导航记录
        Task<IEnumerable<NavigationRouteInfo>> GetNavigationHistoryAsync(string userId);

        // 保存导航记录
        Task<bool> SaveNavigationRecordAsync(NavigationRouteInfo routeInfo, string userId);

        // 获取路况信息
        Task<string> GetTrafficInfoAsync(decimal longitude, decimal latitude, int radius = 1000);

        // 获取附近的服务点（加油站、餐厅等）
        Task<IEnumerable<dynamic>> GetNearbyServicesAsync(
            decimal longitude, decimal latitude, 
            string serviceType = "all", int radius = 2000);

        // 开始导航
        Task<NavigationSessionInfo> StartNavigationAsync(StartNavigationRequest request);

        // 更新导航状态
        Task<bool> UpdateNavigationStatusAsync(string sessionId, UpdateNavigationStatusRequest request);

        // 获取导航会话信息
        Task<NavigationSessionInfo> GetNavigationSessionAsync(string sessionId);

        // 结束导航
        Task<bool> EndNavigationAsync(string sessionId);

        // 暂停导航
        Task<bool> PauseNavigationAsync(string sessionId);

        // 恢复导航
        Task<bool> ResumeNavigationAsync(string sessionId);
    }
}