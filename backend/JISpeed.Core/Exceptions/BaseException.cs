using System;

namespace JISpeed.Core.Exceptions
{
    /// <summary>
    /// 基础异常类，所有自定义异常都应继承此类
    /// </summary>
    public abstract class BaseException : Exception
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int ErrorCode { get; }

        /// <summary>
        /// 是否记录日志（某些预期的业务异常可能不需要记录）
        /// </summary>
        public bool ShouldLog { get; protected set; } = true;

        protected BaseException(int errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        protected BaseException(int errorCode, string message, Exception innerException) 
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
}
