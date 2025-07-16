using JISpeed.Api.Common;

namespace JISpeed.Core.Exceptions
{
    // 未找到资源异常
    public class NotFoundException : BaseException
    {
        // 构造函数1：接收自定义消息
        public NotFoundException(string message)
            : base(ErrorCodes.ResourceNotFound, message)  // 使用资源未找到错误码
        {
            ShouldLog = false;
        }

        // 构造函数2：接收资源名称和资源标识符，自动格式化消息
        public NotFoundException(string resourceName, object key)
            : base(ErrorCodes.ResourceNotFound, $"{resourceName} ('{key}') 未找到")  // 使用字符串插值语法格式化消息
        {
            ShouldLog = false;
        }
    }
}
