using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using JISpeed.Core.Entities.Merchant;
using JISpeed.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace JISpeed.Infrastructure.DailyServices
{
    public class DailyCreator : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider; // 用于获取作用域服务
        private readonly ILogger<DailyCreator> _logger;
        private System.Timers.Timer _timer;

        public DailyCreator(
            IServiceProvider serviceProvider, 
            ILogger<DailyCreator> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        // 服务启动时初始化定时器
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("每日空白销售统计创建服务已启动");
            ScheduleNextRun();
            await base.StartAsync(cancellationToken);
        }

        // 计算下次执行时间（每天 0 点）
        private void ScheduleNextRun()
        {
            var now = DateTime.Now;
            // 今日 0 点已过，则下次执行时间为明天 0 点；否则为今日 0 点
           
            var nextRunTime = now.Date.AddDays(1);
            var interval = nextRunTime - now;
            // 测试
            //var nextRunTime = now.Date.AddSeconds(100);
            //var interval = TimeSpan.FromSeconds(5);
            
            _timer?.Dispose(); // 释放旧定时器
            _timer = new System.Timers.Timer(interval.TotalMilliseconds);
            _timer.Elapsed += async (sender, e) => await CreateBlankSaleStatesAsync();
            _timer.AutoReset = false; // 关闭自动重置，每次执行后重新计算时间
            _timer.Start();

            _logger.LogInformation($"下次创建空白销售统计时间：{nextRunTime:yyyy-MM-dd HH:mm:ss}");
        }

        // 核心逻辑：为所有商家创建当日空白 SaleState
        private async Task CreateBlankSaleStatesAsync()
        {
            try
            {
                _logger.LogInformation("开始创建每日空白销售统计...");
                var statDate = DateTime.Now.Date; // 统计日期为当天（如 2025-01-01 00:00:00）

                // 使用作用域服务访问数据库（避免单例持有 DbContext）
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<OracleDbContext>();

                // 1. 查询所有商家 ID（假设商家表为 Merchants，主键为 MerchantId）
                var merchants = await dbContext.Merchants
                    .ToListAsync();
                

                // 2. 为需要创建的商家生成空白销售统计
                var blankSaleStates = new List<SalesStat>(); // 初始化集合
                foreach (var merchant in merchants)
                {
                    blankSaleStates.Add(new SalesStat()
                    {
                        StatDate = statDate,
                        MerchantId = merchant.MerchantId,
                        SalesAmount = 0,
                        SalesQty = 0,
                        Merchant = merchant // 可选：如果需要导航属性
                    });
                }
                // 3. 批量插入数据库
                dbContext.SalesStats.AddRange(blankSaleStates);
                await dbContext.SaveChangesAsync();

                _logger.LogInformation($"[{statDate:yyyy-MM-dd}] 成功为 {merchants.Count} 个商家创建空白销售统计");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建每日空白销售统计失败");
            }
            finally
            {
                ScheduleNextRun(); // 执行完毕后，重新计算下次执行时间
            }
        }

        // 服务停止时释放资源
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Dispose();
            _logger.LogInformation("每日空白销售统计创建服务已停止");
            await base.StopAsync(cancellationToken);
        }

        // BackgroundService 核心方法（无需重写，通过 Timer 触发任务）
        protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;
    }
}