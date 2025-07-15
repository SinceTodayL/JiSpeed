using Microsoft.AspNetCore.Mvc;
using JISpeed.Api.Common;
using JISpeed.Core.Exceptions;

namespace JISpeed.Api.Controllers
{
    /// <summary>
    /// 测试控制器 - 用于演示全局异常处理
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 测试成功响应
        /// </summary>
        [HttpGet("success")]
        public ApiResponse<string> TestSuccess()
        {
            return ApiResponse<string>.Success("测试成功", "这是一个成功的响应");
        }

        /// <summary>
        /// 测试业务异常
        /// </summary>
        [HttpGet("business-exception")]
        public ApiResponse<string> TestBusinessException()
        {
            throw new BusinessException("这是一个业务异常示例");
        }

        /// <summary>
        /// 测试验证异常
        /// </summary>
        [HttpGet("validation-exception")]
        public ApiResponse<string> TestValidationException()
        {
            throw new ValidationException("参数验证失败示例");
        }

        /// <summary>
        /// 测试未找到异常
        /// </summary>
        [HttpGet("not-found-exception")]
        public ApiResponse<string> TestNotFoundException()
        {
            throw new NotFoundException("用户", 123);
        }

        /// <summary>
        /// 测试未授权异常
        /// </summary>
        [HttpGet("unauthorized-exception")]
        public ApiResponse<string> TestUnauthorizedException()
        {
            throw new UnauthorizedException();
        }

        /// <summary>
        /// 测试禁止访问异常
        /// </summary>
        [HttpGet("forbidden-exception")]
        public ApiResponse<string> TestForbiddenException()
        {
            throw new ForbiddenException();
        }

        /// <summary>
        /// 测试系统异常
        /// </summary>
        [HttpGet("system-exception")]
        public ApiResponse<string> TestSystemException()
        {
            throw new InvalidOperationException("这是一个系统异常示例");
        }

        /// <summary>
        /// 测试参数异常
        /// </summary>
        [HttpGet("argument-exception")]
        public ApiResponse<string> TestArgumentException()
        {
            throw new ArgumentException("参数不正确", "testParam");
        }

        /// <summary>
        /// 测试空参数异常
        /// </summary>
        [HttpGet("argument-null-exception")]
        public ApiResponse<string> TestArgumentNullException()
        {
            throw new ArgumentNullException("testParam", "参数不能为空");
        }

        /// <summary>
        /// 测试通用异常
        /// </summary>
        [HttpGet("generic-exception")]
        public ApiResponse<string> TestGenericException()
        {
            throw new Exception("这是一个通用异常示例");
        }
    }
}
