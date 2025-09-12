using JISpeed.Core.Constants;
using System;

namespace JISpeed.Core.Exceptions
{
    // 支付模块异常类
    public class PaymentExceptions
    {
        // 创建支付失败异常
        // payId: 支付ID
        // reason: 失败原因
        // 返回: 业务异常
        public static BusinessException PaymentFailed(string payId, string reason)
        {
            return new BusinessException(ErrorCodes.PaymentFailed,
                $"支付 (ID: {payId}) 处理失败: {reason}");
        }

        // 创建支付超时异常
        // payId: 支付ID
        // 返回: 业务异常
        public static BusinessException PaymentTimeout(string payId)
        {
            return new BusinessException(ErrorCodes.PaymentTimeout,
                $"支付 (ID: {payId}) 处理超时，请稍后查询支付结果");
        }

        // 创建退款失败异常
        // refundId: 退款ID
        // reason: 失败原因
        // 返回: 业务异常
        public static BusinessException RefundFailed(string refundId, string reason)
        {
            return new BusinessException(ErrorCodes.RefundFailed,
                $"退款 (ID: {refundId}) 处理失败: {reason}");
        }

        // 创建支付方式不支持异常
        // channel: 支付渠道
        // 返回: 业务异常
        public static BusinessException PaymentMethodNotSupported(string channel)
        {
            return new BusinessException(ErrorCodes.PaymentMethodNotSupported,
                $"不支持的支付方式: {channel}");
        }

        // 创建交易记录未找到异常
        // payId: 支付ID
        // 返回: 未找到异常
        public static NotFoundException TransactionNotFound(string payId)
        {
            return new NotFoundException(ErrorCodes.TransactionNotFound,
                $"交易记录 (ID: {payId}) 未找到");
        }

        // 创建重复支付异常
        // orderId: 订单ID
        // payId: 已支付的支付ID
        // 返回: 业务异常
        public static BusinessException DuplicatePayment(string orderId, string payId)
        {
            return new BusinessException(ErrorCodes.DuplicatePayment,
                $"订单 (ID: {orderId}) 已支付 (支付ID: {payId})，不能重复支付");
        }

        // 创建支付金额错误异常
        // payId: 支付ID
        // expectedAmount: 预期金额
        // actualAmount: 实际金额
        // 返回: 业务异常
        public static BusinessException PaymentAmountError(string payId, decimal expectedAmount, decimal actualAmount)
        {
            return new BusinessException(ErrorCodes.PaymentAmountError,
                $"支付 (ID: {payId}) 金额错误，预期: {expectedAmount}，实际: {actualAmount}");
        }

        // 创建退款金额超限异常
        // refundId: 退款ID
        // refundAmount: 退款金额
        // payAmount: 原支付金额
        // 返回: 业务异常
        public static BusinessException RefundAmountExceeded(string refundId, decimal refundAmount, decimal payAmount)
        {
            return new BusinessException(ErrorCodes.RefundAmountExceeded,
                $"退款 (ID: {refundId}) 金额 ({refundAmount}) 超过原支付金额 ({payAmount})");
        }
    }
}