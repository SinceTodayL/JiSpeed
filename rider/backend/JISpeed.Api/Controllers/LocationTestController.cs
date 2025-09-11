using Microsoft.AspNetCore.Mvc;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Api.Common;
using JISpeed.Core.Entities.Rider;

namespace JISpeed.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationTestController : ControllerBase
    {
        private readonly ILocationPushService _locationPushService;

        public LocationTestController(ILocationPushService locationPushService)
        {
            _locationPushService = locationPushService;
        }

        // 测试推送位置更新
        [HttpPost("test-location-update/{riderId}")]
        public async Task<ActionResult<ApiResponse<object>>> TestLocationUpdate(string riderId, [FromBody] TestLocationRequest request)
        {
            try
            {
                // 创建测试用的 Rider 对象（参考 RiderService.cs 的方式）
                var testRider = new Rider
                {
                    RiderId = riderId,
                    Name = "测试骑手",
                    PhoneNumber = "13800138000"
                };

                // 创建 RiderLocation 对象（完全参考 RiderLocationService.cs 的方式）
                var location = new RiderLocation(
                    riderId,
                    (decimal)request.Longitude,
                    (decimal)request.Latitude,
                    DateTime.Now)
                {
                    LocationId = Guid.NewGuid().ToString("N"),
                    RiderId = riderId,
                    Longitude = (decimal)request.Longitude,
                    Latitude = (decimal)request.Latitude,
                    LocationTime = DateTime.Now,
                    Accuracy = null,
                    Speed = null,
                    Direction = null,
                    LocationType = "TEST",
                    Status = 1, // 默认在线
                    Rider = testRider
                };

                await _locationPushService.PushLocationUpdateAsync(riderId, location);

                return Ok(ApiResponse<object>.Success(new object(), $"推送位置更新成功 - 位置: {request.Address}"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(400, $"推送失败: {ex.Message}"));
            }
        }

        // 测试推送系统消息
        [HttpPost("test-push-message/{riderId}")]
        public async Task<ActionResult<ApiResponse<object>>> TestPushMessage(string riderId, [FromBody] TestPushRequest request)
        {
            try
            {
                await _locationPushService.PushSystemMessageAsync(riderId, request.MessageType, request.Content);
                return Ok(ApiResponse<object>.Success(new object(), "推送测试消息成功"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(400, $"推送失败: {ex.Message}"));
            }
        }

        // 测试推送订单分配
        [HttpPost("test-order-assign/{riderId}")]
        public async Task<ActionResult<ApiResponse<object>>> TestOrderAssign(string riderId, [FromBody] TestOrderRequest request)
        {
            try
            {
                var orderInfo = new
                {
                    orderNumber = request.OrderId,
                    customerName = "测试用户",
                    pickupAddress = "测试取餐地址",
                    deliveryAddress = "测试配送地址",
                    estimatedTime = DateTime.Now.AddMinutes(30)
                };

                await _locationPushService.PushOrderAssignedAsync(riderId, request.OrderId, orderInfo);
                return Ok(ApiResponse<object>.Success(new object(), "推送订单分配成功"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(400, $"推送失败: {ex.Message}"));
            }
        }
    }

    // 测试请求模型
    public class TestLocationRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; } = "";
    }

    public class TestPushRequest
    {
        public string MessageType { get; set; } = "TEST";
        public string Content { get; set; } = "";
    }

    public class TestOrderRequest
    {
        public string OrderId { get; set; } = "";
    }
}
