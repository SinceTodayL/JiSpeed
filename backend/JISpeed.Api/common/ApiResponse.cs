using System;
using System.Text.Json.Serialization;


namespace JISpeed.Api.Common
{
    //统一API响应格式
    //T: 返回数据的类型
    public class ApiResponse<T>
    {
        //响应码：0表示成功，非0表示错误
        public int Code { get; set; }

        //响应消息：成功时为操作成功提示，失败时为错误描述
        public string? Message { get; set; }

        //响应数据：成功时包含业务数据，失败时可能包含错误详情或为null
        public T? Data { get; set; }

        //服务器响应时间戳（UTC毫秒）
        public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        //创建成功响应
        //data: 业务数据
        //message: 成功提示信息，默认为“操作成功”
        //returns: 成功的API响应
        public static ApiResponse<T> Success(T data, string message = "操作成功")
        {
            return new ApiResponse<T>
            {
                Code = 0,
                Message = message,
                Data = data
            };
        }

        //创建失败响应
        //code: 错误码
        //message: 错误描述
        //data: 错误详情数据，默认为T的默认值
        //returns: 失败的API响应
        public static ApiResponse<T> Fail(int code, string message, T data = default!)
        {
            return new ApiResponse<T>
            {
                Code = code,
                Message = message,
                Data = data
            };
        }
    }

    //不带泛型的API响应（用于无需返回数据的场景）
    //继承ApiResponse<object>以简化使用
    public class ApiResponse : ApiResponse<object>
    {
        //创建成功响应（无数据）
        //message: 成功提示信息，默认为“操作成功”
        //returns: 成功的API响应
        public static ApiResponse Success(string message = "操作成功")
        {
            return new ApiResponse
            {
                Code = 0,
                Message = message,
                Data = null
            };
        }

        //创建失败响应（无数据）
        //code: 错误码
        //message: 错误描述
        //returns: 失败的API响应
        public static ApiResponse Fail(int code, string message)
        {
            return new ApiResponse
            {
                Code = code,
                Message = message,
                Data = null
            };
        }
    }

    //错误码常量定义
    public static class ErrorCodes
    {
        /*1xxx: 通用请求/参数错误*/
        public const int GeneralRequestError = 1000;      //通用请求错误
        public const int ParameterValidationFailed = 1001;//参数验证失败
        public const int MissingRequiredParameter = 1002; //缺少必填参数
        public const int InvalidRequestFormat = 1003;     //无效的请求格式
        public const int MethodNotSupported = 1004;       //不支持的HTTP方法

        /*2xxx: 认证与授权错误*/
        public const int GeneralAuthError = 2000;         //通用认证错误
        public const int Unauthorized = 2001;             //未授权访问
        public const int Forbidden = 2002;                //禁止访问（无权限）
        public const int AccountDisabled = 2003;          //账号已禁用
        public const int InvalidCredentials = 2004;       //无效的凭据（用户名/密码错误）

        /*3xxx: 业务逻辑错误*/
        public const int GeneralBusinessError = 3000;     //通用业务逻辑错误
        public const int ResourceAlreadyExists = 3001;    //资源已存在
        public const int ResourceNotFound = 3002;         //资源未找到
        public const int InsufficientStock = 3003;        //库存不足
        public const int InvalidState = 3004;             //无效的状态
        public const int InsufficientBalance = 3005;      //余额不足
        public const int TooManyRequests = 3006;          //请求过于频繁

        /*5xxx: 系统/服务内部错误*/
        public const int GeneralSystemError = 5000;       //通用系统内部错误
        public const int DatabaseError = 5001;            //数据库操作错误
        public const int ThirdPartyServiceError = 5002;   //第三方服务错误
        public const int ServiceUnavailable = 5003;       //服务不可用
    }
}