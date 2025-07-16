namespace JISpeed.Core.Constants
{
	// 错误码常量定义
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
