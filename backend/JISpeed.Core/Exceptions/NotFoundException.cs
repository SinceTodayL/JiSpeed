using JISpeed.Core.Constants;
using System;

namespace JISpeed.Core.Exceptions
{
    // 未找到资源异常：用于表示请求的资源不存在的情况
    public class NotFoundException : BaseException
    {
        // 构造函数1：接收错误码和消息
        public NotFoundException(int errorCode, string message)
            : base(errorCode, message)
        {
            ShouldLog = false;
        }

        // 构造函数2：接收错误码、资源名称和资源标识符，自动格式化消息
        public NotFoundException(int errorCode, string resourceName, object key)
            : base(errorCode, $"{resourceName} ('{key}') 未找到")
        {
            ShouldLog = false;
        }
    }
}
