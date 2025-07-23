using JISpeed.Core.Constants;
using System;

namespace JISpeed.Core.Exceptions
{
    // 业务逻辑异常：用于处理可预期的业务场景异常
    public class BusinessException : BaseException
    {
        // 构造函数1：创建一个基本的业务异常，只提供错误消息，使用通用错误码
        public BusinessException(string message)
            : base(ErrorCodes.GeneralError, message)  // 使用通用错误码
        {
            ShouldLog = false;
        }

        // 构造函数2：创建特定错误码的业务异常
        public BusinessException(int errorCode, string message)
            : base(errorCode, message)
        {
            ShouldLog = false;
        }

        // 构造函数3：接收消息和内部异常
        public BusinessException(string message, Exception innerException)
            : base(ErrorCodes.GeneralError, message, innerException)  // 使用通用错误码
        {
            // 接收内部异常，使用日志
        }

        // 构造函数4：接收自定义错误码、消息和内部异常
        public BusinessException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        {
            // 接收内部异常，使用日志
        }
    }
}
