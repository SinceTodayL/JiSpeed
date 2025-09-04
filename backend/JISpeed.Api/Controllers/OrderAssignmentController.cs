using JISpeed.Api.DTOs;
using JISpeed.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JISpeed.Api.Controllers
{
    // 订单分配控制器 - 处理订单分配、骑手接单等业务逻辑
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderAssignmentController : ControllerBase
    {
        private readonly IOrderAssignmentService _orderAssignmentService;
        private readonly ILogger<OrderAssignmentController> _logger;

        public OrderAssignmentController(
            IOrderAssignmentService orderAssignmentService,
            ILogger<OrderAssignmentController> logger)
        {
            _orderAssignmentService = orderAssignmentService;
            _logger = logger;
        }

        // 自动分配订单给最近的在线骑手
        // <param name="orderId">订单ID</param>
        // <returns>分配结果</returns>
        [HttpPost("auto-assign/{orderId}")]
        public async Task<ActionResult<AutoAssignmentResultDto>> AutoAssignOrder(string orderId)
        {
            try
            {
                _logger.LogInformation("开始自动分配订单: {OrderId}", orderId);

                var assignmentId = await _orderAssignmentService.AutoAssignOrderAsync(orderId);

                if (assignmentId != null)
                {
                    var result = new AutoAssignmentResultDto
                    {
                        IsSuccess = true,
                        AssignmentId = assignmentId,
                        RiderId = null // 需要从分配记录中获取
                    };

                    _logger.LogInformation("订单 {OrderId} 自动分配成功，分配编号: {AssignmentId}", orderId, assignmentId);
                    return Ok(result);
                }
                else
                {
                    var result = new AutoAssignmentResultDto
                    {
                        IsSuccess = false,
                        ErrorMessage = "没有可用的骑手或分配失败"
                    };

                    _logger.LogWarning("订单 {OrderId} 自动分配失败", orderId);
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "自动分配订单 {OrderId} 时发生异常", orderId);
                return StatusCode(500, new AutoAssignmentResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "服务器内部错误"
                });
            }
        }

        // 手动分配订单给指定骑手
        // <param name="request">分配请求</param>
        // <returns>分配结果</returns>
        [HttpPost("manual-assign")]
        public async Task<ActionResult<bool>> ManualAssignOrder([FromBody] OrderAssignmentRequestDto request)
        {
            try
            {
                _logger.LogInformation("手动分配订单 {OrderId} 给骑手 {RiderId}", request.OrderId, request.RiderId);

                var result = await _orderAssignmentService.AssignOrderToRiderAsync(request.OrderId, request.RiderId);

                if (result)
                {
                    _logger.LogInformation("订单 {OrderId} 手动分配成功", request.OrderId);
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning("订单 {OrderId} 手动分配失败", request.OrderId);
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "手动分配订单 {OrderId} 时发生异常", request.OrderId);
                return StatusCode(500, false);
            }
        }

        // 骑手接受订单
        // <param name="request">接单请求</param>
        // <returns>接单结果</returns>
        [HttpPost("accept-order")]
        public async Task<ActionResult<bool>> AcceptOrder([FromBody] AcceptOrderRequestDto request)
        {
            try
            {
                _logger.LogInformation("骑手 {RiderId} 接受订单 {OrderId}", request.RiderId, request.OrderId);

                var result = await _orderAssignmentService.AcceptOrderAsync(request.OrderId, request.RiderId);

                if (result)
                {
                    _logger.LogInformation("骑手 {RiderId} 成功接受订单 {OrderId}", request.RiderId, request.OrderId);
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning("骑手 {RiderId} 接受订单 {OrderId} 失败", request.RiderId, request.OrderId);
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "骑手 {RiderId} 接受订单 {OrderId} 时发生异常", request.RiderId, request.OrderId);
                return StatusCode(500, false);
            }
        }

        // 骑手拒绝订单
        // <param name="request">拒绝请求</param>
        // <returns>拒绝结果</returns>
        [HttpPost("reject-order")]
        public async Task<ActionResult<bool>> RejectOrder([FromBody] RejectOrderRequestDto request)
        {
            try
            {
                _logger.LogInformation("骑手 {RiderId} 拒绝订单 {OrderId}", request.RiderId, request.OrderId);

                var result = await _orderAssignmentService.RejectOrderAsync(request.OrderId, request.RiderId);

                if (result)
                {
                    _logger.LogInformation("骑手 {RiderId} 成功拒绝订单 {OrderId}", request.RiderId, request.OrderId);
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning("骑手 {RiderId} 拒绝订单 {OrderId} 失败", request.RiderId, request.OrderId);
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "骑手 {RiderId} 拒绝订单 {OrderId} 时发生异常", request.RiderId, request.OrderId);
                return StatusCode(500, false);
            }
        }

        // 更新订单状态
        // <param name="request">状态更新请求</param>
        // <returns>更新结果</returns>
        [HttpPut("update-status")]
        public async Task<ActionResult<bool>> UpdateOrderStatus([FromBody] UpdateOrderStatusRequestDto request)
        {
            try
            {
                _logger.LogInformation("更新订单 {OrderId} 状态为 {NewStatus}", request.OrderId, request.NewStatus);

                var result = await _orderAssignmentService.UpdateOrderStatusAsync(request.OrderId, request.NewStatus);

                if (result)
                {
                    _logger.LogInformation("订单 {OrderId} 状态更新成功", request.OrderId);
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning("订单 {OrderId} 状态更新失败", request.OrderId);
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新订单 {OrderId} 状态时发生异常", request.OrderId);
                return StatusCode(500, false);
            }
        }

        // 获取订单的分配信息
        // <param name="orderId">订单ID</param>
        // <returns>分配信息</returns>
        [HttpGet("order/{orderId}/assignment")]
        public async Task<ActionResult<dynamic>> GetOrderAssignment(string orderId)
        {
            try
            {
                _logger.LogInformation("获取订单 {OrderId} 的分配信息", orderId);

                var assignment = await _orderAssignmentService.GetOrderAssignmentAsync(orderId);

                if (assignment != null)
                {
                    return Ok(assignment);
                }
                else
                {
                    return NotFound("未找到该订单的分配信息");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取订单 {OrderId} 分配信息时发生异常", orderId);
                return StatusCode(500, "服务器内部错误");
            }
        }

        // 获取骑手的当前分配
        // <param name="riderId">骑手ID</param>
        // <returns>骑手的当前分配列表</returns>
        [HttpGet("rider/{riderId}/assignments")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetRiderAssignments(string riderId)
        {
            try
            {
                _logger.LogInformation("获取骑手 {RiderId} 的当前分配", riderId);

                var assignments = await _orderAssignmentService.GetRiderAssignmentsAsync(riderId);

                return Ok(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取骑手 {RiderId} 分配信息时发生异常", riderId);
                return StatusCode(500, "服务器内部错误");
            }
        }
    }
}
