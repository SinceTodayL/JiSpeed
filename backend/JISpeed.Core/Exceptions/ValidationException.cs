using JISpeed.Api.Common;

namespace JISpeed.Core.Exceptions
{
    // 验证异常：用于参数验证失败的场景
    public class ValidationException : BaseException
    {
        // 构造函数1：接收验证失败消息
        public ValidationException(string message)
            : base(ErrorCodes.ParameterValidationFailed, message)  // 使用参数验证失败错误码
        {
            ShouldLog = false;
        }

        // 构造函数2：接收消息和内部异常
        public ValidationException(string message, Exception innerException)
            : base(ErrorCodes.ParameterValidationFailed, message, innerException)
        {
            ShouldLog = false;
        }
    }
}
