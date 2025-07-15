using JISpeed.Api.Common;

namespace JISpeed.Core.Exceptions
{
    /// <summary>
    /// 业务逻辑异常 - 用于处理可预期的业务场景异常
    /// 这类异常通常不需要记录完整的堆栈信息
    /// </summary>
    public class BusinessException : BaseException
    {
        public BusinessException(string message) 
            : base(ErrorCodes.GeneralBusinessError, message)
        {
            ShouldLog = false; // 业务异常通常不需要记录完整日志
        }

        public BusinessException(int errorCode, string message) 
            : base(errorCode, message)
        {
            ShouldLog = false;
        }

        public BusinessException(string message, Exception innerException) 
            : base(ErrorCodes.GeneralBusinessError, message, innerException)
        {
        }

        public BusinessException(int errorCode, string message, Exception innerException) 
            : base(errorCode, message, innerException)
        {
        }
    }
}
