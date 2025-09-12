using JISpeed.Core.Constants;
using System;

namespace JISpeed.Core.Exceptions
{
    // 验证异常：用于参数验证失败的场景
    public class ValidationException : BaseException
    {
        // 构造函数1：接收验证失败消息，使用默认验证失败错误码
        public ValidationException(string message)
            : base(ErrorCodes.ValidationFailed, message)
        {
            ShouldLog = false;
        }

        // 构造函数2：接收自定义错误码和消息
        public ValidationException(int errorCode, string message)
            : base(errorCode, message)
        {
            ShouldLog = false;
        }

        // 构造函数3：接收消息和内部异常，使用默认验证失败错误码
        public ValidationException(string message, Exception innerException)
            : base(ErrorCodes.ValidationFailed, message, innerException)
        {
            ShouldLog = false;
        }

        // 构造函数4：接收自定义错误码、消息和内部异常
        public ValidationException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        {
            ShouldLog = false;
        }
    }
}
