using JISpeed.Api.Common;

namespace JISpeed.Core.Exceptions
{
    // 未授权异常：当用户未登录或未通过认证时使用
    public class UnauthorizedException : BaseException
    {
        // 构造函数：可选参数，默认消息为“未授权访问”
        public UnauthorizedException(string message = "未授权访问")
            : base(ErrorCodes.Unauthorized, message)  // 使用未授权错误码
        {
            ShouldLog = false;
        }
    }

    // 禁止访问异常：当用户已认证但没有权限访问资源时使用
    public class ForbiddenException : BaseException
    {
        // 构造函数：可选参数，默认消息为“禁止访问，权限不足”
        public ForbiddenException(string message = "禁止访问，权限不足")
            : base(ErrorCodes.Forbidden, message)  // 使用禁止访问错误码
        {
            ShouldLog = false;
        }
    }
}
