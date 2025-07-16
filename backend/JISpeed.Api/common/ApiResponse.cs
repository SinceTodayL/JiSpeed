using System;
using System.Text.Json.Serialization;
using JISpeed.Core.Constants;

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
