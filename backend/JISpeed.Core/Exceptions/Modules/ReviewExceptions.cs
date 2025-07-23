using JISpeed.Core.Constants;
using System;

namespace JISpeed.Core.Exceptions
{
    // 评价模块异常类
    public class ReviewExceptions
    {
        // 创建评价未找到异常
        // reviewId: 评价ID
        // 返回: 未找到异常
        public static NotFoundException RatingNotFound(string reviewId)
        {
            return new NotFoundException(ErrorCodes.RatingNotFound, $"评价 (ID: {reviewId}) 未找到");
        }

        // 创建重复评价异常
        // orderId: 订单ID
        // userId: 用户ID
        // 返回: 业务异常
        public static BusinessException DuplicateRating(string orderId, string userId)
        {
            return new BusinessException(ErrorCodes.DuplicateRating,
                $"用户 (ID: {userId}) 已对订单 (ID: {orderId}) 进行过评价，不能重复评价");
        }

        // 创建评价权限错误异常
        // userId: 用户ID
        // orderId: 订单ID
        // 返回: 业务异常
        public static BusinessException RatingPermissionError(string userId, string orderId)
        {
            return new BusinessException(ErrorCodes.RatingPermissionError,
                $"用户 (ID: {userId}) 无权对订单 (ID: {orderId}) 进行评价");
        }

        // 创建评价内容违规异常
        // content: 评价内容
        // reason: 违规原因
        // 返回: 业务异常
        public static BusinessException RatingContentViolation(string content, string reason)
        {
            return new BusinessException(ErrorCodes.RatingContentViolation,
                $"评价内容违规，原因: {reason}");
        }

        // 创建评价时间已过异常
        // orderId: 订单ID
        // deadline: 评价截止时间
        // 返回: 业务异常
        public static BusinessException RatingTimeExpired(string orderId, DateTime deadline)
        {
            return new BusinessException(ErrorCodes.RatingTimeExpired,
                $"订单 (ID: {orderId}) 的评价时间已过，截止时间: {deadline.ToString("yyyy-MM-dd HH:mm:ss")}");
        }
    }
}
