using System.Threading.Tasks;

namespace JISpeed.Core.Interfaces.IServices
{
    // 自动订单分配服务接口 - 处理订单自动派单的定时任务
    public interface IAutoOrderAssignmentService
    {
        // 启动自动派单服务
        Task StartAsync();
        
        // 停止自动派单服务
        Task StopAsync();
        
        // 手动触发一次派单检查
        Task CheckAndAssignOrdersAsync();
        
        // 获取服务状态
        bool IsRunning { get; }
        
        // 获取上次检查时间
        DateTime? LastCheckTime { get; }
        
        // 获取处理的订单数量
        int ProcessedOrderCount { get; }
    }
}