// using Microsoft.Extensions.Hosting;
// using Microsoft.Extensions.Logging;
// using System.Timers;
// using JISpeed.Core.Entities.Merchant;
// using JISpeed.Infrastructure.Data;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;
//
// namespace JISpeed.Infrastructure.DailyServices
// {
//     public class DailySaleStatService : BackgroundService
//     {
//         private readonly IServiceProvider _serviceProvider; // 用于获取作用域服务
//         private readonly ILogger<DailySaleStatService> _logger;
//         private System.Timers.Timer _timer;
//         private DateTime _lastRunTime = DateTime.MinValue; // 记录上次执行时间
//
//         public DailySaleStatService(
//             IServiceProvider serviceProvider,
//             ILogger<DailySaleStatService> logger,
//             System.Timers.Timer timer
//         )
//         {
//             _serviceProvider = serviceProvider;
//             _logger = logger;
//             _timer = timer;
//         }
//
//         // 任务启动时初始化定时器
//         protected Task OnStartedAsync(CancellationToken cancellationToken)
//         {
//             _logger.LogInformation("每日销售统计服务已启动");
//             ScheduleTimer(); // 调度定时器
//             return Task.CompletedTask;
//         }
//
//         // 调度定时器，设置每天凌晨1点执行
//         private void ScheduleTimer()
//         {
//             // 计算距离下次执行（次日凌晨1点）的时间间隔
//             var now = DateTime.Now;
//             var nextRunTime = new DateTime(now.Year, now.Month, now.Day, 1, 0, 0).AddDays(1); // 次日1点
//             var interval = nextRunTime - now;
//
//             // 防止间隔为负（如当前时间已过凌晨1点，调整为当天1点）
//             if (interval.TotalMilliseconds < 0)
//             {
//                 nextRunTime = nextRunTime.AddDays(1);
//                 interval = nextRunTime - now;
//             }
//
//             // 初始化定时器
//             _timer = new System.Timers.Timer(interval.TotalMilliseconds);
//             _timer.Elapsed += async (sender, e) => await ExecuteTaskAsync(); // 触发时执行任务
//             _timer.AutoReset = false; // 关闭自动重置（每次执行后重新计算下次时间）
//             _timer.Start();
//
//             _logger.LogInformation($"下次执行时间：{nextRunTime:yyyy-MM-dd HH:mm:ss}");
//         }
//
//         // 执行核心任务：计算当日销售数据并创建SaleState
//         private async Task ExecuteTaskAsync()
//         {
//             try
//             {
//                 _logger.LogInformation("开始执行每日销售统计任务");
//                 var statDate = DateTime.Now.Date.AddDays(-1); // 统计“昨天”的数据（凌晨1点统计前一天）
//
//                 // 使用作用域服务访问DbContext（避免单例服务持有DbContext）
//                 using var scope = _serviceProvider.CreateScope();
//                 var dbContext = scope.ServiceProvider.GetRequiredService<OracleDbContext>();
//
//                 // 1. 检查是否已创建当日统计（避免重复执行）
//                 var existingStat = await dbContext.SalesStats
//                     .FirstOrDefaultAsync(s => s.StatDate == statDate);
//                 if (existingStat != null)
//                 {
//                     _logger.LogInformation($"[{statDate:yyyy-MM-dd}] 销售统计已存在，跳过执行");
//                     return;
//                 }
//
//                 // 2. 查询当日所有订单（statDate 00:00:00 至 23:59:59）
//                 var startTime = statDate;
//                 var endTime = statDate.AddDays(1).AddTicks(-1); // 23:59:59.999...
//                 var orders = await dbContext.Orders
//                     .Where(o => o.CreateAt >= startTime && o.CreateAt <= endTime)
//                     .ToListAsync();
//
//                 // 3. 计算销售数据
//                 var totalOrders = orders.Count;
//                 var totalAmount = orders.Sum(o => o.OrderAmount);
//
//                 // 4. 创建SaleState记录
//                 var saleState = new SalesStat()
//                 {
//                     StatDate = statDate,
//                     SalesQty = totalOrders,
//                     SalesAmount = totalAmount,
//                     Merchant = 
//                 };
//                 dbContext.SalesStats.Add(saleState);
//                 await dbContext.SaveChangesAsync();
//
//                 _logger.LogInformation($"[{statDate:yyyy-MM-dd}] 销售统计创建成功：订单数={totalOrders}，总金额={totalAmount:C}");
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, "每日销售统计任务执行失败");
//             }
//             finally
//             {
//                 ScheduleTimer(); // 任务执行后重新调度下次执行
//             }
//         }
//
//         // 服务停止时释放资源
//         public override async Task StopAsync(CancellationToken cancellationToken)
//         {
//             _timer?.Dispose();
//             _logger.LogInformation("每日销售统计服务已停止");
//             await base.StopAsync(cancellationToken);
//         }
//
//         // 后台服务核心方法（无需重写，因已通过Timer实现）
//         protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;
//     }
// }