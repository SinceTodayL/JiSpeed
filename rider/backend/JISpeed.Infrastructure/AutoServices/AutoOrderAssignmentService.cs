using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Core.Entities.Order;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace JISpeed.Infrastructure.AutoServices
{
    public class AutoOrderAssignmentService : BackgroundService, IAutoOrderAssignmentService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AutoOrderAssignmentService> _logger;
        private readonly ConcurrentDictionary<string, DateTime> _processedOrders;
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(5); // 1秒检查一次
        private bool _isRunning = false;
        private DateTime? _lastCheckTime = null;
        private int _processedOrderCount = 0;

        public AutoOrderAssignmentService(
            IServiceProvider serviceProvider, 
            ILogger<AutoOrderAssignmentService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _processedOrders = new ConcurrentDictionary<string, DateTime>();
        }

        public bool IsRunning => _isRunning;
        public DateTime? LastCheckTime => _lastCheckTime;
        public int ProcessedOrderCount => _processedOrderCount;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("自动派单服务已启动，检查间隔: {Interval}秒", _checkInterval.TotalSeconds);
            _isRunning = true;

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await CheckAndAssignOrdersAsync();
                    await Task.Delay(_checkInterval, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("自动派单服务已停止");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "自动派单服务执行过程中发生错误");
            }
            finally
            {
                _isRunning = false;
            }
        }

        public async Task CheckAndAssignOrdersAsync()
        {
            try
            {
                _lastCheckTime = DateTime.Now;
                
                using var scope = _serviceProvider.CreateScope();
                var orderAssignmentService = scope.ServiceProvider.GetRequiredService<IOrderAssignmentService>();
                var context = scope.ServiceProvider.GetRequiredService<OracleDbContext>();

                // 查找所有已支付但未分配的订单
                var pendingOrders = await context.Orders
                    .Where(o => o.OrderStatus == (int)OrderStatus.Paid && 
                               string.IsNullOrEmpty(o.AssignId))
                    .Select(o => new { o.OrderId, o.CreateAt })
                    .ToListAsync();

                if (!pendingOrders.Any())
                {
                    return; // 没有待分配的订单
                }

                _logger.LogInformation("发现 {Count} 个待分配订单", pendingOrders.Count);

                // 逐个处理订单分配
                foreach (var order in pendingOrders)
                {
                    try
                    {
                        // 检查是否已经处理过（避免重复处理）
                        if (_processedOrders.ContainsKey(order.OrderId))
                        {
                            continue;
                        }

                        _logger.LogInformation("开始自动分配订单: {OrderId}", order.OrderId);

                        // 调用现有的自动分配服务
                        var assignId = await orderAssignmentService.AutoAssignOrderAsync(order.OrderId);

                        if (!string.IsNullOrEmpty(assignId))
                        {
                            // 记录成功分配的订单
                            _processedOrders.TryAdd(order.OrderId, DateTime.Now);
                            _processedOrderCount++;
                            
                            _logger.LogInformation("订单 {OrderId} 自动分配成功，分配编号: {AssignId}", 
                                order.OrderId, assignId);
                        }
                        else
                        {
                            _logger.LogWarning("订单 {OrderId} 自动分配失败，可能没有可用骑手", order.OrderId);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "处理订单 {OrderId} 分配时发生错误", order.OrderId);
                    }
                }

                // 清理过期的处理记录（保留1小时）
                var cutoffTime = DateTime.Now.AddHours(-1);
                var expiredOrders = _processedOrders
                    .Where(kvp => kvp.Value < cutoffTime)
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var orderId in expiredOrders)
                {
                    _processedOrders.TryRemove(orderId, out _);
                }

                if (expiredOrders.Any())
                {
                    _logger.LogDebug("清理了 {Count} 个过期的处理记录", expiredOrders.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检查待分配订单时发生错误");
            }
        }

        public Task StartAsync()
        {
            if (!_isRunning)
            {
                _logger.LogInformation("手动启动自动派单服务");
                _isRunning = true;
            }
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            if (_isRunning)
            {
                _logger.LogInformation("手动停止自动派单服务");
                _isRunning = false;
            }
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _isRunning = false;
            base.Dispose();
        }
    }
}