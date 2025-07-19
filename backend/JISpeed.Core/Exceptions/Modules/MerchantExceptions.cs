using JISpeed.Core.Constants;
using System;

namespace JISpeed.Core.Exceptions
{
    // 商家模块异常类
    public class MerchantExceptions
    {
        // 创建商家未找到异常
        // merchantId: 商家ID
        // 返回: 未找到异常
        public static NotFoundException MerchantNotFound(string merchantId)
        {
            return new NotFoundException(ErrorCodes.MerchantNotFound, $"商家 (ID: {merchantId}) 未找到");
        }

        // 创建商家未找到异常(根据商家名称)
        // merchantName: 商家名称
        // 返回: 未找到异常
        public static NotFoundException MerchantNotFoundByName(string merchantName)
        {
            return new NotFoundException(ErrorCodes.MerchantNotFound, $"商家 (名称: {merchantName}) 未找到");
        }

        // 创建商家状态错误异常
        // merchantId: 商家ID
        // currentStatus: 当前状态
        // requiredStatus: 所需状态
        // 返回: 业务异常
        public static BusinessException MerchantStatusError(string merchantId, int currentStatus, int requiredStatus)
        {
            string currentStatusText = GetMerchantStatusText(currentStatus);
            string requiredStatusText = GetMerchantStatusText(requiredStatus);

            return new BusinessException(ErrorCodes.MerchantStatusError,
                $"商家 (ID: {merchantId}) 当前状态 ({currentStatusText}) 不允许此操作，需要状态: {requiredStatusText}");
        }

        // 创建商家已关闭异常
        // merchantId: 商家ID
        // 返回: 业务异常
        public static BusinessException MerchantClosed(string merchantId)
        {
            return new BusinessException(ErrorCodes.MerchantClosed,
                $"商家 (ID: {merchantId}) 已关闭，当前不接受订单");
        }

        // 创建商家认证失败异常
        // merchantId: 商家ID
        // reason: 失败原因
        // 返回: 业务异常
        public static BusinessException MerchantAuthenticationFailed(string merchantId, string reason)
        {
            return new BusinessException(ErrorCodes.MerchantAuthenticationFailed,
                $"商家 (ID: {merchantId}) 认证失败: {reason}");
        }

        // 创建商品未找到异常
        // dishId: 菜品ID
        // 返回: 未找到异常
        public static NotFoundException ProductNotFound(string dishId)
        {
            return new NotFoundException(ErrorCodes.ProductNotFound, $"菜品 (ID: {dishId}) 未找到");
        }

        // 创建商品未找到异常(根据菜品名称)
        // dishName: 菜品名称
        // 返回: 未找到异常
        public static NotFoundException ProductNotFoundByName(string dishName)
        {
            return new NotFoundException(ErrorCodes.ProductNotFound, $"菜品 (名称: {dishName}) 未找到");
        }

        // 创建商品已下架异常
        // dishId: 菜品ID
        // dishName: 菜品名称
        // 返回: 业务异常
        public static BusinessException ProductUnavailable(string dishId, string dishName)
        {
            return new BusinessException(ErrorCodes.ProductUnavailable,
                $"菜品 (ID: {dishId}, 名称: {dishName}) 已下架，当前不可购买");
        }

        // 创建商品库存不足异常
        // dishId: 菜品ID
        // dishName: 菜品名称
        // requestedQuantity: 请求数量
        // availableQuantity: 可用数量
        // 返回: 业务异常
        public static BusinessException ProductOutOfStock(string dishId, string dishName, int requestedQuantity, int availableQuantity)
        {
            return new BusinessException(ErrorCodes.ProductOutOfStock,
                $"菜品 (ID: {dishId}, 名称: {dishName}) 库存不足，请求: {requestedQuantity}，可用: {availableQuantity}");
        }

        // 创建商家营业时间错误异常
        // merchantId: 商家ID
        // merchantName: 商家名称
        // currentTime: 当前时间
        // 返回: 业务异常
        public static BusinessException MerchantBusinessHoursError(string merchantId, string merchantName, string currentTime)
        {
            return new BusinessException(ErrorCodes.MerchantBusinessHoursError,
                $"商家 (ID: {merchantId}, 名称: {merchantName}) 当前不在营业时间内，当前时间: {currentTime}");
        }

        // 创建商家订单拒绝异常
        // merchantId: 商家ID
        // orderId: 订单ID
        // reason: 拒绝原因
        // 返回: 业务异常
        public static BusinessException MerchantOrderRejected(string merchantId, string orderId, string reason)
        {
            return new BusinessException(ErrorCodes.MerchantOrderRejected,
                $"商家 (ID: {merchantId}) 拒绝接受订单 (ID: {orderId})，原因: {reason}");
        }

        // 获取商家状态文本描述
        // status: 状态码
        // 返回: 状态文本描述
        private static string GetMerchantStatusText(int status)
        {
            return status switch
            {
                1 => "正常营业",
                2 => "临时休息",
                3 => "已关闭",
                _ => $"未知状态({status})"
            };
        }
    }
}
