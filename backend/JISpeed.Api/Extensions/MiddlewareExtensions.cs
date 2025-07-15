using JISpeed.Api.Middleware;

namespace JISpeed.Api.Extensions
{
    /// <summary>
    /// 中间件扩展方法
    /// </summary>
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// 添加全局异常处理中间件
        /// </summary>
        /// <param name="app">应用程序构建器</param>
        /// <returns>应用程序构建器</returns>
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}
