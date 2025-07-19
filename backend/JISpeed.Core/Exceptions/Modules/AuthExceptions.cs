using JISpeed.Core.Constants;
using System;

namespace JISpeed.Core.Exceptions
{
	// 认证与授权异常类
	public class AuthExceptions
	{
		// 创建未授权访问异常
		// userId: 用户ID
		// 返回: 业务异常
		public static BusinessException Unauthorized(string userId = null)
		{
			return new BusinessException(ErrorCodes.Unauthorized,
				userId == null ? "未授权访问，请先登录" : $"用户 (ID: {userId}) 未授权访问，请先登录");
		}

		// 创建禁止访问异常
		// userId: 用户ID
		// resource: 资源名称
		// 返回: 业务异常
		public static BusinessException Forbidden(string userId, string resource)
		{
			return new BusinessException(ErrorCodes.Forbidden,
				$"用户 (ID: {userId}) 无权访问资源: {resource}");
		}

		// 创建账号已禁用异常
		// userId: 用户ID
		// account: 账号
		// 返回: 业务异常
		public static BusinessException AccountDisabled(string userId, string account)
		{
			return new BusinessException(ErrorCodes.AccountDisabled,
				$"账号 (ID: {userId}, 账号: {account}) 已被禁用，请联系管理员");
		}

		// 创建无效凭据异常
		// account: 账号
		// 返回: 业务异常
		public static BusinessException InvalidCredentials(string account)
		{
			return new BusinessException(ErrorCodes.InvalidCredentials,
				$"账号 ({account}) 或密码错误");
		}

		// 创建令牌已过期异常
		// userId: 用户ID
		// 返回: 业务异常
		public static BusinessException TokenExpired(string userId)
		{
			return new BusinessException(ErrorCodes.TokenExpired,
				$"用户 (ID: {userId}) 的认证令牌已过期，请重新登录");
		}

		// 创建无效令牌异常
		// reason: 无效原因
		// 返回: 业务异常
		public static BusinessException InvalidToken(string reason)
		{
			return new BusinessException(ErrorCodes.InvalidToken,
				$"无效的认证令牌: {reason}");
		}
	}
}
