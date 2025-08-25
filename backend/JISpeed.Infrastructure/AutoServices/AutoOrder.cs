using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using JISpeed.Core.Entities.Order;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Infrastructure.Data;
using System.Collections.Concurrent;
using OrderEntity = JISpeed.Core.Entities.Order.Order;

namespace JISpeed.Infrastructure.AutoServices
{
    public class AutoOrderService : BackgroundService, IAutoOrderService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AutoOrderService> _logger;
        private readonly ConcurrentDictionary<string, OrderTask> _orderTasks;

        public AutoOrderService(IServiceProvider serviceProvider, ILogger<AutoOrderService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _orderTasks = new ConcurrentDictionary<string, OrderTask>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("订单自动化服务已启动");

            // 恢复现有订单的未完成任务
            await RecoverExistingTasksAsync();

            // 保持服务运行
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }


        /// 恢复现有订单的任务

        private async Task RecoverExistingTasksAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<OracleDbContext>();

                var currentTime = DateTime.Now;
                var cutoffTimeForUnpaid = currentTime.AddMinutes(-15);
                var cutoffTimeForConfirmed = currentTime.AddDays(-3);

                // 处理所有未支付订单
                var allUnpaidOrders = await context.Orders
                    .Where(o => o.OrderStatus == 0) // 所有未支付订单
                    .Select(o => new { o.OrderId, o.CreateAt })
                    .ToListAsync();

                // 立即处理已超时的未支付订单
                var overdueToCancelOrders = allUnpaidOrders
                    .Where(o => currentTime.Subtract(o.CreateAt).TotalMinutes >= 15)
                    .ToList();

                // 需要安排定时任务的未支付订单
                var unpaidOrders = allUnpaidOrders
                    .Where(o => currentTime.Subtract(o.CreateAt).TotalMinutes < 15)
                    .ToList();

                // 查找所有已确认但未评价的订单（不限制创建时间）
                var allConfirmedOrders = await context.Orders
                    .Where(o => o.OrderStatus == 2) // 已确认
                    .Select(o => new { o.OrderId, o.UserId, o.CreateAt })
                    .ToListAsync();

                // 分离需要立即处理和需要定时处理的订单
                var overdueToReviewOrders = allConfirmedOrders
                    .Where(o => currentTime.Subtract(o.CreateAt).TotalDays >= 3)
                    .ToList();

                var confirmedOrders = allConfirmedOrders
                    .Where(o => currentTime.Subtract(o.CreateAt).TotalDays < 3)
                    .ToList();

                // 立即处理已经超时的订单
                foreach (var order in overdueToCancelOrders)
                {
                    _logger.LogInformation($"发现超时未支付订单: {order.OrderId}，创建于 {order.CreateAt:yyyy-MM-dd HH:mm:ss}，立即取消");
                    // 立即处理，但不使用Task.Run以避免并发问题
                    await CancelOrderAsync(order.OrderId);
                }

                // 为未超时未支付订单安排取消任务
                foreach (var order in unpaidOrders)
                {
                    ScheduleOrderCancellation(order.OrderId, order.CreateAt);
                }

                // 立即处理已经超过3天的已确认订单
                foreach (var order in overdueToReviewOrders)
                {
                    _logger.LogInformation($"发现超过3天未评价的已确认订单: {order.OrderId}，创建于 {order.CreateAt:yyyy-MM-dd HH:mm:ss}，立即添加默认评价");
                    // 立即处理，但不使用Task.Run以避免并发问题
                    await AddAutoReviewAsync(order.OrderId, order.UserId);
                }

                // 为未超过3天的已确认订单安排自动评价任务
                foreach (var order in confirmedOrders)
                {
                    ScheduleAutoReview(order.OrderId, order.UserId, order.CreateAt);
                }

                _logger.LogInformation($"恢复了 {unpaidOrders.Count} 个订单取消任务和 {confirmedOrders.Count} 个自动评价任务，立即处理了 {overdueToCancelOrders.Count} 个超时未支付订单和 {overdueToReviewOrders.Count} 个超时未评价订单");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "恢复订单任务时发生错误");
            }
        }


        /// 为新订单安排自动取消任务

        public void ScheduleOrderCancellation(string orderId, DateTime orderCreateTime)
        {
            try
            {
                var timeToCancel = orderCreateTime.AddMinutes(15);
                var delay = timeToCancel - DateTime.Now;

                if (delay <= TimeSpan.Zero)
                {
                    // 已经超时，立即执行
                    _ = Task.Run(() => CancelOrderAsync(orderId));
                    return;
                }

                var timer = new System.Timers.Timer(delay.TotalMilliseconds)
                {
                    AutoReset = false,
                    Enabled = true
                };

                timer.Elapsed += async (sender, e) =>
                {
                    await CancelOrderAsync(orderId);
                    timer.Dispose();

                    // 从任务字典中移除
                    _orderTasks.TryRemove($"{orderId}_Cancel", out _);
                };

                var task = new OrderTask
                {
                    OrderId = orderId,
                    TaskType = TaskType.Cancel,
                    Timer = timer,
                    ScheduledTime = timeToCancel
                };

                _orderTasks.TryAdd($"{orderId}_Cancel", task);
                _logger.LogInformation($"已为订单 {orderId} 安排自动取消任务，将在 {timeToCancel:yyyy-MM-dd HH:mm:ss} 执行");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"为订单 {orderId} 安排取消任务时发生错误");
            }
        }


        /// 为订单安排自动评价任务

        public void ScheduleAutoReview(string orderId, string userId, DateTime orderCreateTime)
        {
            try
            {
                var timeToReview = orderCreateTime.AddDays(3);
                var delay = timeToReview - DateTime.Now;

                if (delay <= TimeSpan.Zero)
                {
                    // 已经超时，立即执行
                    _ = Task.Run(() => AddAutoReviewAsync(orderId, userId));
                    return;
                }

                var timer = new System.Timers.Timer(delay.TotalMilliseconds)
                {
                    AutoReset = false,
                    Enabled = true
                };

                timer.Elapsed += async (sender, e) =>
                {
                    await AddAutoReviewAsync(orderId, userId);
                    timer.Dispose();

                    // 从任务字典中移除
                    _orderTasks.TryRemove($"{orderId}_Review", out _);
                };

                var task = new OrderTask
                {
                    OrderId = orderId,
                    UserId = userId,
                    TaskType = TaskType.Review,
                    Timer = timer,
                    ScheduledTime = timeToReview
                };

                _orderTasks.TryAdd($"{orderId}_Review", task);
                _logger.LogInformation($"已为订单 {orderId} 安排自动评价任务，将在 {timeToReview:yyyy-MM-dd HH:mm:ss} 执行");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"为订单 {orderId} 安排评价任务时发生错误");
            }
        }


        /// 取消订单的自动任务

        public void CancelOrderTask(string orderId, int taskType)
        {
            try
            {
                var taskTypeEnum = (TaskType)taskType;
                var taskKey = $"{orderId}_{taskTypeEnum}";

                if (_orderTasks.TryRemove(taskKey, out var task))
                {
                    task.Timer.Stop();
                    task.Timer.Dispose();
                    _logger.LogInformation($"已取消订单 {orderId} 的 {taskTypeEnum} 任务");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"取消订单 {orderId} 任务时发生错误");
            }
        }


        /// 自动取消超时未支付订单

        private async Task CancelOrderAsync(string orderId)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<OracleDbContext>();

                var order = await context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning($"订单 {orderId} 不存在");
                    return;
                }

                // 检查订单是否仍然是未支付状态
                if (order.OrderStatus != 0)
                {
                    _logger.LogInformation($"订单 {orderId} 状态已改变 (当前状态: {order.OrderStatus})，跳过自动取消");
                    return;
                }

                // 更新订单状态为已取消
                order.OrderStatus = 6; // Cancelled

                // 恢复菜品库存
                var orderDishes = await context.OrderDishes
                    .Where(od => od.OrderId == orderId)
                    .ToListAsync();

                foreach (var orderDish in orderDishes)
                {
                    var dish = await context.Dishes.FindAsync(orderDish.DishId);
                    if (dish != null)
                    {
                        dish.StockQuantity += orderDish.Quantity;
                    }
                }

                // 创建订单日志
                var orderLog = new OrderLog
                {
                    LogId = Guid.NewGuid().ToString("N"), // 使用无连字符格式，确保长度为32字符
                    StatusCode = 4, // OrderLogStatus.Cancelled
                    Actor = "system",
                    OrderId = orderId,
                    Remark = "系统自动取消超时未支付订单",
                    LoggedAt = DateTime.Now,
                    Order = order
                };

                context.OrderLogs.Add(orderLog);
                await context.SaveChangesAsync();

                _logger.LogInformation($"成功自动取消超时未支付订单: {orderId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"自动取消订单 {orderId} 时发生错误");
            }
        }


        /// 自动添加默认好评

        private async Task AddAutoReviewAsync(string orderId, string userId)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<OracleDbContext>();

                var order = await context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning($"订单 {orderId} 不存在");
                    return;
                }

                // 检查订单是否已评价或状态不正确
                if (order.OrderStatus != 2) // Confirmed
                {
                    _logger.LogInformation($"订单 {orderId} 状态不正确 (当前状态: {order.OrderStatus})，跳过自动评价");
                    return;
                }

                // 检查是否已有评价
                var existingReview = await context.Reviews
                    .FirstOrDefaultAsync(r => r.OrderId == orderId);

                if (existingReview != null)
                {
                    _logger.LogInformation($"订单 {orderId} 已有评价，跳过自动评价");
                    return;
                }

                // 检查用户是否存在
                var user = await context.CustomUsers.FindAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning($"用户 {userId} 不存在，跳过自动评价");
                    return;
                }

                // 创建默认好评，使用公共构造函数并设置导航属性
                var review = new Review(orderId, userId, 5, "系统默认好评", 1)
                {
                    ReviewId = Guid.NewGuid().ToString("N"),
                    OrderId = orderId,
                    UserId = userId,
                    Rating = 5,
                    IsAnonymous = 1,
                    ReviewAt = DateTime.Now,
                    Order = order,
                    User = user
                };

                context.Reviews.Add(review);

                // 更新订单状态为已评价
                order.OrderStatus = 3; // Reviewed

                // 创建订单日志
                var orderLog = new OrderLog
                {
                    LogId = Guid.NewGuid().ToString("N"),
                    StatusCode = 3, // 自定义状态码表示自动评价
                    Actor = "system",
                    OrderId = orderId,
                    Remark = "系统自动添加默认好评",
                    LoggedAt = DateTime.Now,
                    Order = order
                };

                context.OrderLogs.Add(orderLog);
                await context.SaveChangesAsync();

                _logger.LogInformation($"成功为订单 {orderId} 添加自动默认好评");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"为订单 {orderId} 添加自动评价时发生错误");
            }
        }


        /// 获取当前活跃任务数量

        public int GetActiveTaskCount()
        {
            return _orderTasks.Count;
        }


        /// 手动检查和取消已超时未支付的订单

        public async Task CheckAndCancelOverdueOrdersAsync()
        {
            try
            {
                _logger.LogInformation("开始手动检查超时未支付订单");
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<OracleDbContext>();

                var currentTime = DateTime.Now;

                // 查找所有已超时的未支付订单（创建时间超过15分钟）
                var overdueOrders = await context.Orders
                    .Where(o => o.OrderStatus == 0 &&
                           currentTime.Subtract(o.CreateAt).TotalMinutes >= 15)
                    .Select(o => new { o.OrderId, o.CreateAt })
                    .ToListAsync();

                _logger.LogInformation($"发现 {overdueOrders.Count} 个超时未支付订单");

                // 逐个取消超时订单
                foreach (var order in overdueOrders)
                {
                    _logger.LogInformation($"开始取消超时未支付订单: {order.OrderId}, 创建时间: {order.CreateAt:yyyy-MM-dd HH:mm:ss}");
                    await CancelOrderAsync(order.OrderId);
                }

                _logger.LogInformation($"超时未支付订单检查完成，处理了 {overdueOrders.Count} 个订单");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "手动检查超时未支付订单时发生错误");
            }
        }


        /// 手动检查和处理已超时未评价的订单

        public async Task CheckAndAddOverdueReviewsAsync()
        {
            try
            {
                _logger.LogInformation("开始手动检查超时未评价订单");
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<OracleDbContext>();

                var currentTime = DateTime.Now;

                // 查找所有已超时的已确认未评价订单（创建时间超过3天且状态为已确认）
                var overdueReviewOrders = await context.Orders
                    .Where(o => o.OrderStatus == 2 &&
                           currentTime.Subtract(o.CreateAt).TotalDays >= 3)
                    .Select(o => new { o.OrderId, o.UserId, o.CreateAt })
                    .ToListAsync();

                _logger.LogInformation($"发现 {overdueReviewOrders.Count} 个超时未评价的已确认订单");

                // 逐个添加默认评价
                foreach (var order in overdueReviewOrders)
                {
                    _logger.LogInformation($"开始为超时未评价订单添加默认评价: {order.OrderId}, 创建时间: {order.CreateAt:yyyy-MM-dd HH:mm:ss}");
                    await AddAutoReviewAsync(order.OrderId, order.UserId);
                }

                _logger.LogInformation($"超时未评价订单检查完成，处理了 {overdueReviewOrders.Count} 个订单");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "手动检查超时未评价订单时发生错误");
            }
        }

        public override void Dispose()
        {
            // 清理所有定时器
            foreach (var task in _orderTasks.Values)
            {
                task.Timer?.Stop();
                task.Timer?.Dispose();
            }
            _orderTasks.Clear();
            base.Dispose();
        }
    }


    /// 任务类型枚举

    public enum TaskType
    {
        Cancel = 0,  // 取消任务
        Review = 1   // 评价任务
    }


    /// 订单任务

    public class OrderTask
    {
        public required string OrderId { get; set; }
        public string? UserId { get; set; }
        public required TaskType TaskType { get; set; }
        public required System.Timers.Timer Timer { get; set; }
        public required DateTime ScheduledTime { get; set; }
    }
}