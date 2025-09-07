using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JISpeed.Application.Services.Navigation;
using JISpeed.Core.DTOs;
using JISpeed.Core.Interfaces.IServices;

namespace JISpeed.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class NavigationController : ControllerBase
    {
        private readonly INavigationService _navigationService;

        public NavigationController(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        // 获取订单导航路线（骑手接单后调用）
        [HttpGet("order/{orderId}/rider/{riderId}/route")]
        public async Task<ActionResult<NavigationRouteInfo>> GetOrderNavigationRoute(
            string orderId, string riderId)
        {
            try
            {
                var routeInfo = await _navigationService.GetOrderNavigationRouteAsync(orderId, riderId);
                return Ok(routeInfo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // 获取订单实时导航更新（基于骑手实际位置）
        [HttpGet("order/{orderId}/rider/{riderId}/updates")]
        public async Task<ActionResult<NavigationUpdateInfo>> GetOrderNavigationUpdates(
            string orderId, string riderId)
        {
            try
            {
                var updateInfo = await _navigationService.GetOrderNavigationUpdatesAsync(orderId, riderId);
                return Ok(updateInfo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // 获取导航路线信息（基础导航路线）
        [HttpPost("route")]
        public async Task<ActionResult<NavigationRouteInfo>> GetNavigationRoute([FromBody] NavigationRouteRequest request)
        {
            try
            {
                var routeInfo = await _navigationService.GetNavigationRouteAsync(request);
                return Ok(routeInfo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // 获取实时导航路径（基础路径规划）
        [HttpPost("realtime")]
        public async Task<ActionResult<NavigationRouteInfo>> GetRealTimeNavigation(
            [FromBody] NavigationRouteRequest request)
        {
            try
            {
                var routeInfo = await _navigationService.GetRealTimeNavigationAsync(
                    request.StartLongitude, request.StartLatitude,
                    request.EndLongitude, request.EndLatitude, request.Mode);
                return Ok(routeInfo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // 获取附近的服务点
        [HttpGet("nearby")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetNearbyServices(
            [FromQuery] decimal longitude,
            [FromQuery] decimal latitude,
            [FromQuery] string serviceType = "all",
            [FromQuery] int radius = 2000)
        {
            try
            {
                var services = await _navigationService.GetNearbyServicesAsync(
                    longitude, latitude, serviceType, radius);
                return Ok(services);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}