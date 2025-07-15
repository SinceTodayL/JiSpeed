using JISpeed.Api.Common;

namespace JISpeed.Core.Exceptions
{
    /// <summary>
    /// 验证异常 - 用于参数验证失败的场景
    /// </summary>
    public class ValidationException : BaseException
    {
        public ValidationException(string message) 
            : base(ErrorCodes.ParameterValidationFailed, message)
        {
            ShouldLog = false;
        }

        public ValidationException(string message, Exception innerException) 
            : base(ErrorCodes.ParameterValidationFailed, message, innerException)
        {
            ShouldLog = false;
        }
    }
}
