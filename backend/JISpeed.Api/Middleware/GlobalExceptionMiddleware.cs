using System.Net;
using System.Text.Json;
using JISpeed.Api.Common;
using JISpeed.Core.Exceptions;

namespace JISpeed.Api.Middleware
{
    /// <summary>
    /// 全局异常处理中间件
    /// 统一处理应用程序中的所有未捕获异常，并返回标准化的API响应格式
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // 设置响应内容类型
            context.Response.ContentType = "application/json";

            // 根据异常类型处理
            var response = exception switch
            {
                BaseException baseEx => HandleBaseException(baseEx),
                UnauthorizedAccessException => HandleUnauthorizedAccessException(),
                ArgumentException argEx => HandleArgumentException(argEx),
                ArgumentNullException argNullEx => HandleArgumentNullException(argNullEx),
                InvalidOperationException invalidOpEx => HandleInvalidOperationException(invalidOpEx),
                NotSupportedException notSupportedEx => HandleNotSupportedException(notSupportedEx),
                TimeoutException timeoutEx => HandleTimeoutException(timeoutEx),
                _ => HandleGenericException(exception)
            };

            // 记录日志
            LogException(exception, context);

            // 设置HTTP状态码
            context.Response.StatusCode = GetHttpStatusCode(exception);

            // 序列化并返回响应
            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            await context.Response.WriteAsync(jsonResponse);
        }

        /// <summary>
        /// 处理自定义基础异常
        /// </summary>
        private ApiResponse HandleBaseException(BaseException exception)
        {
            return ApiResponse.Fail(exception.ErrorCode, exception.Message);
        }

        /// <summary>
        /// 处理未授权访问异常
        /// </summary>
        private ApiResponse HandleUnauthorizedAccessException()
        {
            return ApiResponse.Fail(ErrorCodes.Unauthorized, "未授权访问");
        }

        /// <summary>
        /// 处理参数异常
        /// </summary>
        private ApiResponse HandleArgumentException(ArgumentException exception)
        {
            return ApiResponse.Fail(ErrorCodes.ParameterValidationFailed, 
                $"参数错误: {exception.Message}");
        }

        /// <summary>
        /// 处理空参数异常
        /// </summary>
        private ApiResponse HandleArgumentNullException(ArgumentNullException exception)
        {
            return ApiResponse.Fail(ErrorCodes.MissingRequiredParameter, 
                $"缺少必填参数: {exception.ParamName}");
        }

        /// <summary>
        /// 处理无效操作异常
        /// </summary>
        private ApiResponse HandleInvalidOperationException(InvalidOperationException exception)
        {
            return ApiResponse.Fail(ErrorCodes.InvalidState, 
                $"操作无效: {exception.Message}");
        }

        /// <summary>
        /// 处理不支持的操作异常
        /// </summary>
        private ApiResponse HandleNotSupportedException(NotSupportedException exception)
        {
            return ApiResponse.Fail(ErrorCodes.MethodNotSupported, 
                $"不支持的操作: {exception.Message}");
        }

        /// <summary>
        /// 处理超时异常
        /// </summary>
        private ApiResponse HandleTimeoutException(TimeoutException exception)
        {
            return ApiResponse.Fail(ErrorCodes.ServiceUnavailable, 
                $"请求超时: {exception.Message}");
        }

        /// <summary>
        /// 处理通用异常
        /// </summary>
        private ApiResponse HandleGenericException(Exception exception)
        {
            // 在开发环境下返回详细错误信息，生产环境下返回通用错误信息
            var message = _environment.IsDevelopment() 
                ? $"系统内部错误: {exception.Message}" 
                : "系统内部错误，请稍后重试";

            return ApiResponse.Fail(ErrorCodes.GeneralSystemError, message);
        }

        /// <summary>
        /// 记录异常日志
        /// </summary>
        private void LogException(Exception exception, HttpContext context)
        {
            var requestPath = context.Request.Path;
            var requestMethod = context.Request.Method;
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            var ipAddress = GetClientIpAddress(context);

            // 根据异常类型决定日志级别
            if (exception is BaseException baseEx && !baseEx.ShouldLog)
            {
                // 业务异常通常只记录信息级别
                _logger.LogInformation("业务异常 - {Method} {Path} | IP: {IP} | 错误码: {ErrorCode} | 消息: {Message}", 
                    requestMethod, requestPath, ipAddress, baseEx.ErrorCode, baseEx.Message);
            }
            else
            {
                // 系统异常记录错误级别，包含完整堆栈信息
                _logger.LogError(exception, 
                    "系统异常 - {Method} {Path} | IP: {IP} | UserAgent: {UserAgent} | 异常类型: {ExceptionType} | 消息: {Message}", 
                    requestMethod, requestPath, ipAddress, userAgent, exception.GetType().Name, exception.Message);
            }
        }

        /// <summary>
        /// 获取HTTP状态码
        /// </summary>
        private static int GetHttpStatusCode(Exception exception)
        {
            return exception switch
            {
                UnauthorizedException => (int)HttpStatusCode.Unauthorized,
                ForbiddenException => (int)HttpStatusCode.Forbidden,
                NotFoundException => (int)HttpStatusCode.NotFound,
                ValidationException => (int)HttpStatusCode.BadRequest,
                BusinessException => (int)HttpStatusCode.BadRequest,
                ArgumentException => (int)HttpStatusCode.BadRequest,
                ArgumentNullException => (int)HttpStatusCode.BadRequest,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                NotSupportedException => (int)HttpStatusCode.MethodNotAllowed,
                TimeoutException => (int)HttpStatusCode.RequestTimeout,
                _ => (int)HttpStatusCode.InternalServerError
            };
        }

        /// <summary>
        /// 获取客户端IP地址
        /// </summary>
        private static string GetClientIpAddress(HttpContext context)
        {
            // 优先从X-Forwarded-For头获取（考虑代理/负载均衡器）
            var xForwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(xForwardedFor))
            {
                // X-Forwarded-For可能包含多个IP，取第一个
                return xForwardedFor.Split(',')[0].Trim();
            }

            // 其次从X-Real-IP头获取
            var xRealIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(xRealIp))
            {
                return xRealIp;
            }

            // 最后从RemoteIpAddress获取
            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}
