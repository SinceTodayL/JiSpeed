using JISpeed.Api.Common;
using JISpeed.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace JISpeed.Api.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminTestController : ControllerBase
    {
        private readonly ILogger<AdminTestController> _logger;
        private readonly IServiceProvider _serviceProvider;

        public AdminTestController(
            ILogger<AdminTestController> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }


        /// 获取活跃的订单任务

        [HttpGet("test/active-order-tasks")]
        public ActionResult<ApiResponse<object>> GetActiveOrderTasks()
        {
            try
            {
                _logger.LogInformation("获取活跃订单任务");
                var autoOrderService = _serviceProvider.GetService(typeof(IAutoOrderService)) as IAutoOrderService;

                if (autoOrderService == null)
                {
                    _logger.LogWarning("未能获取AutoOrderService服务实例");
                    return NotFound(ApiResponse<object>.Fail(1001, "未能获取AutoOrderService服务实例"));
                }

                int activeTaskCount = autoOrderService.GetActiveTaskCount();

                return Ok(ApiResponse<object>.Success(new
                {
                    activeTaskCount = activeTaskCount
                }, "成功获取活跃订单任务数量"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取活跃订单任务时发生异常");
                return StatusCode(500, ApiResponse<object>.Fail(1000, $"系统错误: {ex.Message}"));
            }
        }

    }
}
