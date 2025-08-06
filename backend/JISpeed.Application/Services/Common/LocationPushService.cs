using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Core.Entities.Rider;
using JISpeed.Core.Interfaces;

namespace JISpeed.Application.Services.Common
{
    public class LocationPushService : ILocationPushService
    {
        private readonly IHubContext<Hub> _hubContext;  // 改为Hub
        private readonly ILogger<LocationPushService> _logger;

        public LocationPushService(
            IHubContext<Hub> hubContext,  // 改为Hub
            ILogger<LocationPushService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        // 推送骑手位置更新
        public async Task PushLocationUpdateAsync(string riderId, RiderLocation location)
        {
            try
            {
                var locationData = new
                {
                    riderId = location.RiderId,
                    longitude = location.Longitude,
                    latitude = location.Latitude,
                    locationTime = location.LocationTime,
                    accuracy = location.Accuracy,
                    speed = location.Speed,
                    direction = location.Direction,
                    status = location.Status,
                    locationId = location.LocationId
                };

                // 推送给骑手自己
                await _hubContext.Clients.Group($"rider_{riderId}")
                    .SendAsync("LocationUpdate", locationData);

                _logger.LogInformation("位置更新推送成功, RiderId: {RiderId}, LocationId: {LocationId}",
                    riderId, location.LocationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "位置更新推送失败, RiderId: {RiderId}", riderId);
                throw;
            }
        }

        // 推送订单分配给骑手
        public async Task PushOrderAssignedAsync(string riderId, string orderId, object orderInfo)
        {
            try
            {
                var assignData = new
                {
                    orderId,
                    orderInfo,
                    timestamp = DateTime.UtcNow,
                    messageType = "ORDER_ASSIGNED"
                };

                await _hubContext.Clients.Group($"rider_{riderId}")
                    .SendAsync("OrderAssigned", assignData);

                _logger.LogInformation("订单分配推送成功, RiderId: {RiderId}, OrderId: {OrderId}", riderId, orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "订单分配推送失败, RiderId: {RiderId}, OrderId: {OrderId}", riderId, orderId);
                throw;
            }
        }

        // 推送骑手状态变化
        public async Task PushRiderStatusChangeAsync(string riderId, int status, string? message = null)
        {
            try
            {
                var statusData = new
                {
                    riderId,
                    status,
                    message,
                    timestamp = DateTime.UtcNow,
                    messageType = "STATUS_CHANGE"
                };

                await _hubContext.Clients.Group($"rider_{riderId}")
                    .SendAsync("StatusChange", statusData);

                _logger.LogInformation("骑手状态变化推送成功, RiderId: {RiderId}, Status: {Status}", riderId, status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "骑手状态变化推送失败, RiderId: {RiderId}", riderId);
                throw;
            }
        }

        // 推送给订单关注者
        public async Task PushToOrderWatchersAsync(string orderId, string eventType, object data)
        {
            try
            {
                var eventData = new
                {
                    orderId,
                    eventType,
                    data,
                    timestamp = DateTime.UtcNow
                };

                await _hubContext.Clients.Group($"order_{orderId}")
                    .SendAsync("OrderEvent", eventData);

                _logger.LogInformation("订单事件推送成功, OrderId: {OrderId}, EventType: {EventType}", orderId, eventType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "订单事件推送失败, OrderId: {OrderId}, EventType: {EventType}", orderId, eventType);
                throw;
            }
        }

        // 推送给区域管理员
        public async Task PushToAreaManagersAsync(string areaCode, string eventType, object data)
        {
            try
            {
                var eventData = new
                {
                    areaCode,
                    eventType,
                    data,
                    timestamp = DateTime.UtcNow
                };

                await _hubContext.Clients.Group($"area_{areaCode}")
                    .SendAsync("AreaEvent", eventData);

                _logger.LogInformation("区域事件推送成功, AreaCode: {AreaCode}, EventType: {EventType}", areaCode, eventType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "区域事件推送失败, AreaCode: {AreaCode}, EventType: {EventType}", areaCode, eventType);
                throw;
            }
        }

        // 推送系统消息给骑手
        public async Task PushSystemMessageAsync(string riderId, string messageType, string content)
        {
            try
            {
                var messageData = new
                {
                    messageType,
                    content,
                    timestamp = DateTime.UtcNow
                };

                await _hubContext.Clients.Group($"rider_{riderId}")
                    .SendAsync("SystemMessage", messageData);

                _logger.LogInformation("系统消息推送成功, RiderId: {RiderId}, MessageType: {MessageType}", riderId, messageType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "系统消息推送失败, RiderId: {RiderId}, MessageType: {MessageType}", riderId, messageType);
                throw;
            }
        }
    }
}
