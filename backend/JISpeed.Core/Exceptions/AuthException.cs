using JISpeed.Api.Common;

namespace JISpeed.Core.Exceptions
{
    /// <summary>
    /// 未授权异常
    /// </summary>
    public class UnauthorizedException : BaseException
    {
        public UnauthorizedException(string message = "未授权访问") 
            : base(ErrorCodes.Unauthorized, message)
        {
            ShouldLog = false;
        }
    }

    /// <summary>
    /// 禁止访问异常
    /// </summary>
    public class ForbiddenException : BaseException
    {
        public ForbiddenException(string message = "禁止访问，权限不足") 
            : base(ErrorCodes.Forbidden, message)
        {
            ShouldLog = false;
        }
    }
}
