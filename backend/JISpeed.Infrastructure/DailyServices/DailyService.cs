using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using JISpeed.Core.Entities.Merchant;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace JISpeed.Infrastructure.DailyServices
{
    public class DailyCreator : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider; // 用于获取作用域服务
        private readonly ILogger<DailyCreator> _logger;
        private System.Timers.Timer _statTimer;
        private System.Timers.Timer _settlementTimer;

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
            _logger.LogInformation("每日任务创建服务已启动");
            ScheduleStatTimer();        // 零点统计任务
            ScheduleSettlementTimer();  // 一点结算任务
            await base.StartAsync(cancellationToken);
        }

        // 计算下次执行时间（每天 0 点）
        private void ScheduleStatTimer()
        {
            var now = DateTime.Now;
            // 今日 0 点已过，则下次执行时间为明天 0 点；否则为今日 0 点

            var nextRunTime = now.Date.AddDays(1);
            var interval = nextRunTime - now;
             // 测试
            // var nextRunTime = now.Date.AddSeconds(100);
            // var interval = TimeSpan.FromSeconds(5);
            
            _statTimer?.Dispose(); // 释放旧定时器
            _statTimer = new System.Timers.Timer(interval.TotalMilliseconds);
            _statTimer.Elapsed += async (sender, e) =>
            {
                await CreateBlankSaleStatesAsync();
                ScheduleStatTimer();

            };
            _statTimer.AutoReset = false; // 关闭自动重置，每次执行后重新计算时间
            _statTimer.Start();

            _logger.LogInformation($"下次执行每日任务时间：{nextRunTime:yyyy-MM-dd HH:mm:ss}");
        }
        private void ScheduleSettlementTimer()
        {
            var now = DateTime.Now;
            // 今日 1 点已过，则下次执行时间为明天 1 点；否则为今日 1 点
           
            var nextRunTime = now.Date.AddDays(1).AddHours(1);
            var interval = nextRunTime - now;
            // 测试
            // var nextRunTime = now.Date.AddSeconds(100);
            // var interval = TimeSpan.FromSeconds(5);
            
            _settlementTimer?.Dispose(); // 释放旧定时器
            _settlementTimer = new System.Timers.Timer(interval.TotalMilliseconds);
            _settlementTimer.Elapsed += async (sender, e) =>
            {
                await CreateSettlementsAsync();
                ScheduleSettlementTimer();
            };
            _settlementTimer.AutoReset = false; // 关闭自动重置，每次执行后重新计算时间
            _settlementTimer.Start();

            _logger.LogInformation($"下次执行每日任务时间：{nextRunTime:yyyy-MM-dd HH:mm:ss}");
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
        }
        
        // 核心逻辑：为所有商家创建当日Settlement
        private async Task CreateSettlementsAsync()
        {
            try
            {
                _logger.LogInformation("开始创建每日结算单...");
                var statDate = DateTime.Now.Date; // 统计日期为当天（如 2025-01-01 00:00:00）

                // 使用作用域服务访问数据库（避免单例持有 DbContext）
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<OracleDbContext>();

                // 1. 查询所有商家 ID（假设商家表为 Merchants，主键为 MerchantId）
                var merchants = await dbContext.Merchants
                    .Include(m => m.ApplicationUser)
                    .Include(m => m.Orders)
                        .ThenInclude(o=>o.Payments)
                    .ToListAsync();
                

                // 2. 为需要创建的商家生成结算单
                var settlements = new List<Settlement>(); // 初始化集合
                foreach (var merchant in merchants)
                {
                    var periodStart = DateTime.Today.AddDays(-1);
                    var periodEnd = DateTime.Today;
                    decimal grossAmount = 0;
                    var commissionFee = 7;
                    if (merchant.ApplicationUser!=null&&merchant.ApplicationUser.CreatedAt>=statDate.AddDays(-30))
                    {
                        commissionFee = 0;
                    }

                    foreach (var order in merchant.Orders)
                    {
                        // 订单创建日期为昨日0点至昨日23:59
                        if(order.CreateAt>=periodStart&&order.CreateAt<periodEnd)
                        {
                            if (order.OrderStatus != (int)OrderStatus.Cancelled &&
                                order.OrderStatus != (int)OrderStatus.Unpaid &&
                                order.OrderStatus != (int)OrderStatus.Aftersales)
                            {
                                // 检查是否成功支付
                                foreach (var orderPayment in order.Payments)
                                {
                                    if(orderPayment.PayStatus==(int)PayStatus.Paid)
                                        grossAmount+=orderPayment.PayAmount;
                                }
                            }
                        }                    }
                    settlements.Add(new Settlement()
                    {
                        SettleId = Guid.NewGuid().ToString("N"),
                        PeriodStart = periodStart,
                        PeriodEnd = periodEnd,
                        MerchantId = merchant.MerchantId,
                        CommissionFee = commissionFee,
                        GrossAmount = grossAmount,
                        NetAmount = grossAmount*(100-commissionFee)/100,
                        Merchant = merchant,
                        SettledAt = DateTime.Now
                    });
                }
                // 3. 批量插入数据库
                dbContext.Settlements.AddRange(settlements);
                await dbContext.SaveChangesAsync();

                _logger.LogInformation($"[{statDate:yyyy-MM-dd}] 成功为 {merchants.Count} 个商家创建结算单");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建每日结算单失败");
            }
        }
        

        // 服务停止时释放资源
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _settlementTimer?.Dispose();
            _statTimer?.Dispose();
            _logger.LogInformation("每日任务创建服务已停止");
            await base.StopAsync(cancellationToken);
        }

        // BackgroundService 核心方法（无需重写，通过 Timer 触发任务）
        protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;
    }
}