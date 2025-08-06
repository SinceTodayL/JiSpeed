using Microsoft.AspNetCore.SignalR;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace JISpeed.Api.Hubs
{
    public class LocationHub : Hub, ILocationHub  // 实现接口
    {
        private readonly ILogger<LocationHub> _logger;
        private readonly IRiderLocationService _riderLocationService;

        public LocationHub(
            ILogger<LocationHub> logger,
            IRiderLocationService riderLocationService)
        {
            _logger = logger;
            _riderLocationService = riderLocationService;
        }

        // 骑手加入自己的位置组
        public async Task JoinRiderGroup(string riderId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"rider_{riderId}");
            _logger.LogInformation("骑手加入位置组, RiderId: {RiderId}, ConnectionId: {ConnectionId}",
                riderId, Context.ConnectionId);
        }

        // 用户/商家关注订单位置
        public async Task JoinOrderGroup(string orderId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"order_{orderId}");
            _logger.LogInformation("用户加入订单组, OrderId: {OrderId}, ConnectionId: {ConnectionId}",
                orderId, Context.ConnectionId);
        }

        // 管理员关注区域骑手
        public async Task JoinAreaGroup(string areaCode)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"area_{areaCode}");
            _logger.LogInformation("管理员加入区域组, AreaCode: {AreaCode}, ConnectionId: {ConnectionId}",
                areaCode, Context.ConnectionId);
        }

        // 离开骑手组
        public async Task LeaveRiderGroup(string riderId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"rider_{riderId}");
            _logger.LogInformation("骑手离开位置组, RiderId: {RiderId}, ConnectionId: {ConnectionId}",
                riderId, Context.ConnectionId);
        }

        // 离开订单组
        public async Task LeaveOrderGroup(string orderId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"order_{orderId}");
            _logger.LogInformation("用户离开订单组, OrderId: {OrderId}, ConnectionId: {ConnectionId}",
                orderId, Context.ConnectionId);
        }

        // 连接断开时的清理
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("客户端断开连接, ConnectionId: {ConnectionId}, Exception: {Exception}",
                Context.ConnectionId, exception?.Message);
            await base.OnDisconnectedAsync(exception);
        }

        // 连接建立时的处理
        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("客户端建立连接, ConnectionId: {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }
    }
}
