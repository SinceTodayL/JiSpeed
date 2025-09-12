using JISpeed.Core.Entities.Rider;

namespace JISpeed.Core.Interfaces.IServices
{
    // 位置推送服务接口
    public interface ILocationPushService
    {
        // 推送骑手位置更新
        Task PushLocationUpdateAsync(string riderId, RiderLocation location);

        // 推送订单分配给骑手
        Task PushOrderAssignedAsync(string riderId, string orderId, object orderInfo);

        // 推送骑手状态变化
        Task PushRiderStatusChangeAsync(string riderId, int status, string? message = null);

        // 推送给订单关注者（用户、商家）
        Task PushToOrderWatchersAsync(string orderId, string eventType, object data);

        // 推送给区域管理员
        Task PushToAreaManagersAsync(string areaCode, string eventType, object data);

        // 推送系统消息给骑手
        Task PushSystemMessageAsync(string riderId, string messageType, string content);
    }
}
