namespace JISpeed.Core.Interfaces.IServices
{
    public interface IAutoOrderService
    {

        /// 为新订单安排自动取消任务

        /// <param name="orderId">订单ID</param>
        /// <param name="orderCreateTime">订单创建时间</param>
        void ScheduleOrderCancellation(string orderId, DateTime orderCreateTime);


        /// 为订单安排自动评价任务

        /// <param name="orderId">订单ID</param>
        /// <param name="userId">用户ID</param>
        /// <param name="orderCreateTime">订单创建时间</param>
        void ScheduleAutoReview(string orderId, string userId, DateTime orderCreateTime);


        /// 取消订单的自动任务（当订单状态改变时调用）

        /// <param name="orderId">订单ID</param>
        /// <param name="taskType">任务类型</param>
        void CancelOrderTask(string orderId, int taskType);


        /// 获取当前活跃任务数量

        /// <returns>活跃任务数量</returns>
        int GetActiveTaskCount();


        /// 手动检查和取消已超时未支付的订单

        Task CheckAndCancelOverdueOrdersAsync();


        /// 手动检查和处理已超时未评价的订单

        Task CheckAndAddOverdueReviewsAsync();
    }
}
