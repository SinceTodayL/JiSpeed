using System;

namespace JISpeed.Core.Exceptions
{
    // 基础异常类：所有自定义异常都应继承此类
    public abstract class BaseException : Exception
    {
        // 错误码：用于标识不同类型的错误
        public int ErrorCode { get; }  // 只读，构造函数中设置

        // 是否记录日志（某些预期的业务异常可能不需要记录）
        public bool ShouldLog { get; protected set; } = true;  // 默认值为true，即需要记录日志

        // 构造函数1：接收错误码和消息
        protected BaseException(int errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;  // 设置错误码
        }

        // 构造函数2：额外接收内部异常
        protected BaseException(int errorCode, string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;  // 设置错误码
        }
    }
}
