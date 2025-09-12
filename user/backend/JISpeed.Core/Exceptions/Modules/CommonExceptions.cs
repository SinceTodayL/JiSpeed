using JISpeed.Core.Constants;
using System;

namespace JISpeed.Core.Exceptions
{
    // 通用错误异常类
    public class CommonExceptions
    {
        // 创建通用错误异常
        // message: 错误消息
        // 返回: 业务异常
        public static BusinessException GeneralError(string message)
        {
            return new BusinessException(ErrorCodes.GeneralError, message);
        }

        // 创建参数验证失败异常
        // fieldName: 字段名称
        // reason: 验证失败原因
        // 返回: 验证异常
        public static ValidationException ValidationFailed(string fieldName, string reason)
        {
            return new ValidationException(ErrorCodes.ValidationFailed, $"字段 '{fieldName}' 验证失败: {reason}");
        }

        // 创建缺少必填参数异常
        // paramName: 参数名称
        // 返回: 验证异常
        public static ValidationException MissingParameter(string paramName)
        {
            return new ValidationException(ErrorCodes.MissingParameter, $"缺少必填参数: {paramName}");
        }

        // 创建无效请求格式异常
        // message: 错误消息
        // 返回: 业务异常
        public static BusinessException InvalidRequestFormat(string message)
        {
            return new BusinessException(ErrorCodes.InvalidRequestFormat, $"无效的请求格式: {message}");
        }

        // 创建不支持的操作异常
        // operation: 操作名称
        // 返回: 业务异常
        public static BusinessException UnsupportedOperation(string operation)
        {
            return new BusinessException(ErrorCodes.UnsupportedOperation, $"不支持的操作: {operation}");
        }

        // 创建请求频率限制异常
        // ipAddress: IP地址
        // maxRequests: 最大请求次数
        // timeWindow: 时间窗口(秒)
        // 返回: 业务异常
        public static BusinessException RateLimitExceeded(string ipAddress, int maxRequests, int timeWindow)
        {
            return new BusinessException(ErrorCodes.RateLimitExceeded,
                $"请求过于频繁，IP: {ipAddress}，超出限制: {maxRequests}次/{timeWindow}秒，请稍后再试");
        }
    }
}
