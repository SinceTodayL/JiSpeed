using JISpeed.Core.Constants;
using System;

namespace JISpeed.Core.Exceptions
{
    // 配送模块异常类
    public class DeliveryExceptions
    {
        // 创建配送信息未找到异常
        // orderId: 订单ID
        // 返回: 未找到异常
        public static NotFoundException DeliveryNotFound(string orderId)
        {
            return new NotFoundException(ErrorCodes.DeliveryNotFound, $"配送信息 (订单ID: {orderId}) 未找到");
        }

        // 创建配送状态错误异常
        // orderId: 订单ID
        // currentStatus: 当前状态
        // requiredStatus: 所需状态
        // 返回: 业务异常
        public static BusinessException DeliveryStatusError(string orderId, int currentStatus, int requiredStatus)
        {
            return new BusinessException(ErrorCodes.DeliveryStatusError,
                $"配送 (订单ID: {orderId}) 当前状态 ({currentStatus}) 不允许此操作，需要状态: {requiredStatus}");
        }

        // 创建配送区域不支持异常
        // addressId: 地址ID
        // detailedAddress: 详细地址
        // 返回: 业务异常
        public static BusinessException DeliveryAreaNotSupported(string addressId, string detailedAddress)
        {
            return new BusinessException(ErrorCodes.DeliveryAreaNotSupported,
                $"地址 (ID: {addressId}, {detailedAddress}) 不在配送服务范围内");
        }

        // 创建配送路线规划失败异常
        // orderId: 订单ID
        // riderId: 骑手ID
        // reason: 失败原因
        // 返回: 业务异常
        public static BusinessException DeliveryRouteFailed(string orderId, string riderId, string reason)
        {
            return new BusinessException(ErrorCodes.DeliveryRouteFailed,
                $"骑手 (ID: {riderId}) 配送订单 (ID: {orderId}) 路线规划失败: {reason}");
        }

        // 创建配送距离超出范围异常
        // orderId: 订单ID
        // distance: 配送距离
        // maxDistance: 最大配送距离
        // 返回: 业务异常
        public static BusinessException DeliveryDistanceExceeded(string orderId, decimal distance, decimal maxDistance)
        {
            return new BusinessException(ErrorCodes.DeliveryDistanceExceeded,
                $"订单 (ID: {orderId}) 配送距离 ({distance}km) 超出最大配送范围 ({maxDistance}km)");
        }

        // 创建配送时间冲突异常
        // assignId: 分配编号
        // riderId: 骑手ID
        // conflictAssignId: 冲突的分配编号
        // 返回: 业务异常
        public static BusinessException DeliveryTimeConflict(string assignId, string riderId, string conflictAssignId)
        {
            return new BusinessException(ErrorCodes.DeliveryTimeConflict,
                $"骑手 (ID: {riderId}) 的配送任务 (ID: {assignId}) 与任务 (ID: {conflictAssignId}) 时间冲突");
        }

        // 创建配送延迟异常
        // orderId: 订单ID
        // riderId: 骑手ID
        // expectedTime: 预期送达时间
        // estimatedTime: 估计送达时间
        // 返回: 业务异常
        public static BusinessException DeliveryDelayed(string orderId, string riderId, DateTime expectedTime, DateTime estimatedTime)
        {
            return new BusinessException(ErrorCodes.DeliveryDelayed,
                $"骑手 (ID: {riderId}) 配送订单 (ID: {orderId}) 延迟，预期送达时间: {expectedTime:yyyy-MM-dd HH:mm:ss}，" +
                $"估计送达时间: {estimatedTime:yyyy-MM-dd HH:mm:ss}");
        }

        // 创建配送取消失败异常
        // assignId: 分配编号
        // riderId: 骑手ID
        // reason: 失败原因
        // 返回: 业务异常
        public static BusinessException DeliveryCancelFailed(string assignId, string riderId, string reason)
        {
            return new BusinessException(ErrorCodes.DeliveryCancelFailed,
                $"骑手 (ID: {riderId}) 的配送任务 (ID: {assignId}) 取消失败: {reason}");
        }

        // 创建配送完成确认失败异常
        // assignId: 分配编号
        // riderId: 骑手ID
        // reason: 失败原因
        // 返回: 业务异常
        public static BusinessException DeliveryCompletionFailed(string assignId, string riderId, string reason)
        {
            return new BusinessException(ErrorCodes.DeliveryCompletionFailed,
                $"骑手 (ID: {riderId}) 的配送任务 (ID: {assignId}) 完成确认失败: {reason}");
        }
    }
}
