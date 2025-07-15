using JISpeed.Api.Common;

namespace JISpeed.Core.Exceptions
{
    /// <summary>
    /// 未找到资源异常
    /// </summary>
    public class NotFoundException : BaseException
    {
        public NotFoundException(string message) 
            : base(ErrorCodes.ResourceNotFound, message)
        {
            ShouldLog = false;
        }

        public NotFoundException(string resourceName, object key) 
            : base(ErrorCodes.ResourceNotFound, $"{resourceName} ('{key}') 未找到")
        {
            ShouldLog = false;
        }
    }
}
