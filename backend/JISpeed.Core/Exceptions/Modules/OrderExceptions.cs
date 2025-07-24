using JISpeed.Core.Constants;
using System;

namespace JISpeed.Core.Exceptions
{
    // 订单模块异常类
    public class OrderExceptions
    {
        // 创建订单未找到异常
        // orderId: 订单ID
        // 返回: 未找到异常
        public static NotFoundException OrderNotFound(string orderId)
        {
            return new NotFoundException(ErrorCodes.OrderNotFound, $"订单 (ID: {orderId}) 未找到");
        }

        // 创建订单状态错误异常
        // orderId: 订单ID
        // currentStatus: 当前状态
        // requiredStatus: 所需状态
        // 返回: 业务异常
        public static BusinessException OrderStatusError(string orderId, int currentStatus, int requiredStatus)
        {
            string statusText = currentStatus == 1 ? "待处理" :
                               (currentStatus == 2 ? "处理中" :
                               (currentStatus == 3 ? "已解决" : "已关闭"));

            string requiredText = requiredStatus == 1 ? "待处理" :
                                 (requiredStatus == 2 ? "处理中" :
                                 (requiredStatus == 3 ? "已解决" : "已关闭"));

            return new BusinessException(ErrorCodes.OrderStatusError,
                $"订单 (ID: {orderId}) 当前状态 ({statusText}) 不允许此操作，需要状态: {requiredText}");
        }

        // 创建订单已取消异常
        // orderId: 订单ID
        // 返回: 业务异常
        public static BusinessException OrderCancelled(string orderId)
        {
            return new BusinessException(ErrorCodes.OrderCancelled,
                $"订单 (ID: {orderId}) 已被取消，无法执行此操作");
        }

        // 创建订单已分配异常
        // orderId: 订单ID
        // riderId: 骑手ID
        // 返回: 业务异常
        public static BusinessException OrderAlreadyAssigned(string orderId, string riderId)
        {
            return new BusinessException(ErrorCodes.OrderAlreadyAssigned,
                $"订单 (ID: {orderId}) 已分配给骑手 (ID: {riderId})");
        }

        // 创建订单分配失败异常
        // orderId: 订单ID
        // riderId: 骑手ID
        // reason: 失败原因
        // 返回: 业务异常
        public static BusinessException OrderAssignmentFailed(string orderId, string riderId, string reason)
        {
            return new BusinessException(ErrorCodes.OrderAssignmentFailed,
                $"订单 (ID: {orderId}) 分配给骑手 (ID: {riderId}) 失败: {reason}");
        }

        // 创建订单金额错误异常
        // orderId: 订单ID
        // expectedAmount: 预期金额
        // actualAmount: 实际金额
        // 返回: 业务异常
        public static BusinessException OrderAmountError(string orderId, decimal expectedAmount, decimal actualAmount)
        {
            return new BusinessException(ErrorCodes.OrderAmountError,
                $"订单 (ID: {orderId}) 金额错误，预期: {expectedAmount}，实际: {actualAmount}");
        }

        // 创建订单超时异常
        // orderId: 订单ID
        // 返回: 业务异常
        public static BusinessException OrderTimeout(string orderId)
        {
            return new BusinessException(ErrorCodes.OrderTimeout,
                $"订单 (ID: {orderId}) 处理超时");
        }

        // 创建订单商品错误异常
        // orderId: 订单ID
        // dishId: 菜品ID
        // reason: 错误原因
        // 返回: 业务异常
        public static BusinessException OrderItemError(string orderId, string dishId, string reason)
        {
            return new BusinessException(ErrorCodes.OrderItemError,
                $"订单 (ID: {orderId}) 菜品 (ID: {dishId}) 错误: {reason}");
        }

        // 创建订单地址无效异常
        // orderId: 订单ID
        // addressId: 地址ID
        // 返回: 业务异常
        public static BusinessException OrderAddressInvalid(string orderId, string addressId)
        {
            return new BusinessException(ErrorCodes.OrderAddressInvalid,
                $"订单 (ID: {orderId}) 地址 (ID: {addressId}) 无效");
        }

        // 创建订单创建失败异常
        // userId: 用户ID
        // reason: 失败原因
        // 返回: 业务异常
        public static BusinessException OrderCreationFailed(string userId, string reason)
        {
            return new BusinessException(ErrorCodes.OrderCreationFailed,
                $"用户 (ID: {userId}) 创建订单失败: {reason}");
        }
    }
}
