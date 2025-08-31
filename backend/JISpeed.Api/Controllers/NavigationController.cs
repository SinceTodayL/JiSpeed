using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JISpeed.Application.Services.Navigation;
using JISpeed.Core.DTOs;
using JISpeed.Core.Interfaces.IServices;

namespace JISpeed.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NavigationController : ControllerBase
    {
        private readonly INavigationService _navigationService;

        public NavigationController(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        // 获取导航路线信息
        // <param name="request">导航请求参数</param>
        // <returns>导航路线信息</returns>
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

        // 获取实时导航更新
        // <param name="routeId">路线ID</param>
        // <returns>实时导航信息</returns>
        [HttpGet("route/{routeId}/updates")]
        public async Task<ActionResult<NavigationUpdateInfo>> GetNavigationUpdates(string routeId)
        {
            try
            {
                var updateInfo = await _navigationService.GetNavigationUpdatesAsync(routeId);
                return Ok(updateInfo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // 开始导航
        // <param name="request">开始导航请求</param>
        // <returns>导航会话信息</returns>
        [HttpPost("start")]
        public async Task<ActionResult<NavigationSessionInfo>> StartNavigation([FromBody] StartNavigationRequest request)
        {
            try
            {
                var sessionInfo = await _navigationService.StartNavigationAsync(request);
                return Ok(sessionInfo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // 结束导航
        // <param name="sessionId">导航会话ID</param>
        // <returns>操作结果</returns>
        [HttpPost("end/{sessionId}")]
        public async Task<ActionResult> EndNavigation(string sessionId)
        {
            try
            {
                await _navigationService.EndNavigationAsync(sessionId);
                return Ok(new { message = "导航已结束" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // 更新导航状态
        // <param name="sessionId">导航会话ID</param>
        // <param name="request">状态更新请求</param>
        // <returns>操作结果</returns>
        [HttpPut("session/{sessionId}/status")]
        public async Task<ActionResult> UpdateNavigationStatus(string sessionId, [FromBody] UpdateNavigationStatusRequest request)
        {
            try
            {
                await _navigationService.UpdateNavigationStatusAsync(sessionId, request);
                return Ok(new { message = "导航状态已更新" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}