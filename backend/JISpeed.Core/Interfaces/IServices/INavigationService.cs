using System.Collections.Generic;
using System.Threading.Tasks;
using JISpeed.Core.DTOs;

namespace JISpeed.Core.Interfaces.IServices
{
    // 导航服务接口
    public interface INavigationService
    {
        // 根据订单获取导航路线（骑手接单后调用）
        Task<NavigationRouteInfo> GetOrderNavigationRouteAsync(string orderId, string riderId);
        
        // 获取订单实时导航更新（基于骑手实际位置）
        Task<NavigationUpdateInfo> GetOrderNavigationUpdatesAsync(string orderId, string riderId);

        // 获取实时导航路径（基础路径规划）
        Task<NavigationRouteInfo> GetRealTimeNavigationAsync(
            decimal startLongitude, decimal startLatitude,
            decimal endLongitude, decimal endLatitude,
            string mode = "driving");

        // 获取导航路线信息（基础导航路线）
        Task<NavigationRouteInfo> GetNavigationRouteAsync(NavigationRouteRequest request);

        // 获取附近的服务点（加油站、餐厅等）
        Task<IEnumerable<dynamic>> GetNearbyServicesAsync(
            decimal longitude, decimal latitude, 
            string serviceType = "all", int radius = 2000);
    }
}