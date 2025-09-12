using JISpeed.Core.Constants;
using System;

namespace JISpeed.Core.Exceptions
{
    // 系统错误异常类
    public class SystemExceptions
    {
        // 创建系统内部错误异常
        // message: 错误消息
        // 返回: 业务异常
        public static BusinessException SystemError(string message)
        {
            return new BusinessException(ErrorCodes.SystemError, $"系统内部错误: {message}");
        }

        // 创建数据库错误异常
        // tableName: 表名
        // operation: 操作类型
        // message: 错误消息
        // 返回: 业务异常
        public static BusinessException DatabaseError(string tableName, string operation, string message)
        {
            return new BusinessException(ErrorCodes.DatabaseError,
                $"数据库错误: 表 '{tableName}' {operation}操作失败 - {message}");
        }

        // 创建缓存错误异常
        // key: 缓存键
        // operation: 操作类型
        // 返回: 业务异常
        public static BusinessException CacheError(string key, string operation)
        {
            return new BusinessException(ErrorCodes.CacheError,
                $"缓存服务错误: 键 '{key}' {operation}操作失败");
        }

        // 创建第三方服务错误异常
        // serviceName: 服务名称
        // operation: 操作类型
        // message: 错误消息
        // 返回: 业务异常
        public static BusinessException ExternalServiceError(string serviceName, string operation, string message)
        {
            return new BusinessException(ErrorCodes.ExternalServiceError,
                $"第三方服务错误: {serviceName} {operation}操作失败 - {message}");
        }

        // 创建网络错误异常
        // endpoint: 目标端点
        // message: 错误消息
        // 返回: 业务异常
        public static BusinessException NetworkError(string endpoint, string message)
        {
            return new BusinessException(ErrorCodes.NetworkError,
                $"网络通信错误: 连接 {endpoint} 失败 - {message}");
        }

        // 创建配置错误异常
        // configKey: 配置键
        // message: 错误消息
        // 返回: 业务异常
        public static BusinessException ConfigurationError(string configKey, string message)
        {
            return new BusinessException(ErrorCodes.ConfigurationError,
                $"配置错误: 键 '{configKey}' - {message}");
        }

        // 创建服务不可用异常
        // serviceName: 服务名称
        // 返回: 业务异常
        public static BusinessException ServiceUnavailable(string serviceName)
        {
            return new BusinessException(ErrorCodes.ServiceUnavailable,
                $"服务不可用: {serviceName} 当前无法访问，请稍后重试");
        }

        // 创建文件操作错误异常
        // filePath: 文件路径
        // operation: 操作类型
        // message: 错误消息
        // 返回: 业务异常
        public static BusinessException FileOperationError(string filePath, string operation, string message)
        {
            return new BusinessException(ErrorCodes.FileOperationError,
                $"文件操作错误: 文件 '{filePath}' {operation}失败 - {message}");
        }

        // 创建消息队列错误异常
        // queueName: 队列名称
        // operation: 操作类型
        // message: 错误消息
        // 返回: 业务异常
        public static BusinessException MessageQueueError(string queueName, string operation, string message)
        {
            return new BusinessException(ErrorCodes.MessageQueueError,
                $"消息队列错误: 队列 '{queueName}' {operation}失败 - {message}");
        }

        // 创建地图服务错误异常
        // operation: 操作类型
        // message: 错误消息
        // 返回: 业务异常
        public static BusinessException MapServiceError(string operation, string message)
        {
            return new BusinessException(ErrorCodes.MapServiceError,
                $"地图服务错误: {operation}失败 - {message}");
        }
    }
}
