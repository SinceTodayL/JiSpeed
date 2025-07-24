using JISpeed.Core.Constants;
using System;

namespace JISpeed.Core.Exceptions
{
    // 用户模块异常类
    public class UserExceptions
    {
        // 创建用户未找到异常
        // userId: 用户ID
        // 返回: 未找到异常
        public static NotFoundException UserNotFound(string userId)
        {
            return new NotFoundException(ErrorCodes.UserNotFound, $"用户 (ID: {userId}) 未找到");
        }

        // 创建用户未找到异常(通过账号)
        // account: 用户账号
        // 返回: 未找到异常
        public static NotFoundException UserNotFoundByAccount(string account)
        {
            return new NotFoundException(ErrorCodes.UserNotFound, $"用户 (账号: {account}) 未找到");
        }

        // 创建用户状态错误异常
        // userId: 用户ID
        // status: 当前状态
        // 返回: 业务异常
        public static BusinessException UserStatusError(string userId, int status)
        {
            string statusText = status == 1 ? "活跃" : status == 2 ? "已删除" : "未知";
            return new BusinessException(ErrorCodes.UserStatusError,
                $"用户 (ID: {userId}) 当前状态 ({statusText}) 不允许执行此操作");
        }

        // 创建用户地址未找到异常
        // addressId: 地址ID
        // userId: 用户ID
        // 返回: 未找到异常
        public static NotFoundException UserAddressNotFound(string addressId, string? userId = null)
        {
            if (userId != null)
                return new NotFoundException(ErrorCodes.UserAddressNotFound, $"用户 (ID: {userId}) 的地址 (ID: {addressId}) 未找到");
            else
                return new NotFoundException(ErrorCodes.UserAddressNotFound, $"地址 (ID: {addressId}) 未找到");
        }

        // 创建用户余额不足异常
        // userId: 用户ID
        // requiredAmount: 所需金额
        // currentBalance: 当前余额
        // 返回: 业务异常
        public static BusinessException UserInsufficientBalance(string userId, decimal requiredAmount, decimal currentBalance)
        {
            return new BusinessException(ErrorCodes.UserInsufficientBalance,
                $"用户 (ID: {userId}) 余额不足，需要: {requiredAmount}，当前余额: {currentBalance}");
        }

        // 创建用户信息不完整异常
        // userId: 用户ID
        // missingFields: 缺失的字段
        // 返回: 业务异常
        public static BusinessException UserIncompleteInfo(string userId, string missingFields)
        {
            return new BusinessException(ErrorCodes.UserIncompleteInfo,
                $"用户 (ID: {userId}) 信息不完整，缺少: {missingFields}");
        }

        // 创建用户已存在异常
        // account: 用户账号
        // 返回: 业务异常
        public static BusinessException UserAlreadyExists(string account)
        {
            return new BusinessException(ErrorCodes.UserAlreadyExists,
                $"用户账号 '{account}' 已存在");
        }

        // 创建用户验证码错误异常
        // account: 用户账号
        // 返回: 业务异常
        public static BusinessException UserVerificationCodeError(string account)
        {
            return new BusinessException(ErrorCodes.UserVerificationCodeError,
                $"用户 (账号: {account}) 提供的验证码不正确");
        }

        // 创建用户登录失败异常
        // account: 用户账号
        // reason: 失败原因
        // 返回: 业务异常
        public static BusinessException UserLoginFailed(string account, string? reason = null)
        {
            if (string.IsNullOrEmpty(reason))
                return new BusinessException(ErrorCodes.UserLoginFailed,
                    $"用户 (账号: {account}) 登录失败，用户名或密码错误");
            else
                return new BusinessException(ErrorCodes.UserLoginFailed,
                    $"用户 (账号: {account}) 登录失败，原因: {reason}");
        }
    }
}
