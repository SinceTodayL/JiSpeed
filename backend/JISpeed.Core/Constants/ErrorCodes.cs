namespace JISpeed.Core.Constants
{
    // 系统错误码定义
    public static class ErrorCodes
    {
        #region 通用错误 (1000-1999)：适用于各种通用场景的错误
        // 通用错误：未归类的一般性错误
        public const int GeneralError = 1000;

        // 参数验证失败：请求参数不符合验证规则
        public const int ValidationFailed = 1001;

        // 缺少必填参数：请求中缺少必需的参数
        public const int MissingParameter = 1002;
        
        // 无效的请求格式：请求格式不正确，如JSON格式错误
        public const int InvalidRequestFormat = 1003;
        
        // 不支持的操作：尝试执行系统不支持的操作
        public const int UnsupportedOperation = 1004;

        // 请求频率限制：请求过于频繁，触发限流
        public const int RateLimitExceeded = 1005;
        
        // 资源未找到：请求的资源不存在
        public const int ResourceNotFound = 1006;
        #endregion


        #region 认证权限 (2000-2999)：用户认证和授权相关的错误
        // 未授权：用户未登录或登录已过期
        public const int Unauthorized = 2001;

        // 禁止访问：用户没有权限执行此操作
        public const int Forbidden = 2002;

        // 账号禁用：用户账号已被禁用
        public const int AccountDisabled = 2003;

        // 无效凭证：用户凭证无效或已过期
        public const int InvalidCredentials = 2004;

        // 令牌过期：认证令牌已过期
        public const int TokenExpired = 2005;

        // 无效令牌：认证令牌格式无效或已失效
        public const int InvalidToken = 2006;
        #endregion


        #region 订单 (10000-10999)：订单相关的错误
        // 骑手未找到：未找到请求的骑手ID
        public const int RiderNotFound = 10001;

        // 骑手状态错误：当前骑手状态异常
        public const int RiderStatusError = 10002;

        // 骑手容量超限：当前骑手容量已超出限制
        public const int RiderCapacityExceeded = 10003;

        // 骑手超出服务区域：骑手超出服务区域范围
        public const int RiderOutOfServiceArea = 10004;

        // 骑手已存在：骑手已存在，无法重复添加
        public const int RiderAlreadyExists = 10005;

        // 骑手认证失败：骑手认证未通过
        public const int RiderAuthenticationFailed = 10006;

        // 骑手位置更新失败：无法更新骑手位置信息
        public const int RiderLocationUpdateFailed = 10007;

        // 骑手评分过低：骑手评分低于系统要求
        public const int RiderRatingTooLow = 10008;

        // 骑手休息中：骑手当前处于休息状态
        public const int RiderOnBreak = 10009;

        // 骑手设备离线：骑手设备未连接到信号
        public const int RiderDeviceOffline = 10010;
        #endregion


        #region 订单 (20000-20999)：订单相关的错误
        // 订单未找到：未找到请求的订单ID
        public const int OrderNotFound = 20001;

        // 订单状态错误：当前订单状态异常
        public const int OrderStatusError = 20002;

        // 订单已取消：订单已取消，无法重复取消
        public const int OrderCancelled = 20003;

        // 订单已分配：订单已分配给其他骑手，无法重复分配
        public const int OrderAlreadyAssigned = 20004;

        // 订单分配失败：无法分配订单
        public const int OrderAssignmentFailed = 20005;

        // 订单金额错误：订单金额与预期不符
        public const int OrderAmountError = 20006;

        // 订单超时：订单超过预定时间未完成
        public const int OrderTimeout = 20007;

        // 订单项目错误：订单中包含的商品信息有误
        public const int OrderItemError = 20008;

        // 订单地址无效：订单地址格式不正确或无法解析
        public const int OrderAddressInvalid = 20009;

        // 订单创建失败：无法创建订单
        public const int OrderCreationFailed = 20010;
        #endregion


        #region 配送 (30000-30999)：配送相关的错误
        // 配送未找到：未找到请求的配送ID
        public const int DeliveryNotFound = 30001;

        // 配送状态错误：当前配送状态异常
        public const int DeliveryStatusError = 30002;

        // 配送区域不支持：目的地区域不在配送范围内
        public const int DeliveryAreaNotSupported = 30003;

        // 配送路线规划失败：无法为目的地规划有效路线
        public const int DeliveryRouteFailed = 30004;

        // 配送距离超限：配送距离超出系统限制
        public const int DeliveryDistanceExceeded = 30005;

        // 配送时间冲突：配送时间与预计时间冲突
        public const int DeliveryTimeConflict = 30006;

        // 配送延迟：配送时间超出预计时间
        public const int DeliveryDelayed = 30007;

        // 配送取消失败：无法取消配送
        public const int DeliveryCancelFailed = 30008;

        // 配送完成失败：无法完成配送
        public const int DeliveryCompletionFailed = 30009;
        #endregion


        #region 商家 (40000-40999)：商家和商品相关的错误
        // 商家未找到：未找到请求的商家ID
        public const int MerchantNotFound = 40001;

        // 商家状态错误：当前商家状态异常
        public const int MerchantStatusError = 40002;

        // 商家已关闭：商家已关闭，无法继续营业
        public const int MerchantClosed = 40003;

        // 商家认证失败：商家认证未通过
        public const int MerchantAuthenticationFailed = 40004;

        // 商品未找到：未找到请求的商品ID
        public const int ProductNotFound = 40005;

        // 商品不可用：商品当前不可用
        public const int ProductUnavailable = 40006;

        // 商品缺货：商品库存不足
        public const int ProductOutOfStock = 40007;

        // 商家营业时间错误：商家营业时间设置有误
        public const int MerchantBusinessHoursError = 40008;

        // 商家订单拒绝：商家拒绝接收订单
        public const int MerchantOrderRejected = 40009;
        #endregion


        #region 用户 (50000-50999)：用户相关的错误
        // 用户未找到：未找到请求的用户ID
        public const int UserNotFound = 50001;

        // 用户状态错误：当前用户状态异常
        public const int UserStatusError = 50002;

        // 用户地址未找到：未找到请求的用户地址ID
        public const int UserAddressNotFound = 50003;

        // 用户余额不足：用户余额不足以支付订单
        public const int UserInsufficientBalance = 50004;

        // 用户信息不完整：用户提供的信息不完整
        public const int UserIncompleteInfo = 50005;

        // 用户已存在：用户已存在，无法重复添加
        public const int UserAlreadyExists = 50006;

        // 用户验证码错误：用户提供的验证码不正确
        public const int UserVerificationCodeError = 50007;

        // 用户登录失败：用户登录失败
        public const int UserLoginFailed = 50008;
        #endregion


        #region 支付 (60000-60999)：支付相关的错误
        // 支付失败：支付失败
        public const int PaymentFailed = 60001;

        // 支付超时：支付超过预定时间未完成
        public const int PaymentTimeout = 60002;

        // 退款失败：无法退款
        public const int RefundFailed = 60003;

        // 支付方式不支持：不支持的支付方式
        public const int PaymentMethodNotSupported = 60004;

        // 交易未找到：未找到请求的交易ID
        public const int TransactionNotFound = 60005;

        // 重复支付：重复支付
        public const int DuplicatePayment = 60006;

        // 支付金额错误：支付金额与预期不符
        public const int PaymentAmountError = 60007;

        // 退款金额超限：退款金额超出原支付金额
        public const int RefundAmountExceeded = 60008;
        #endregion


        #region 评价 (70000-70999)：评价相关的错误
        // 评价未找到：未找到请求的评价ID
        public const int RatingNotFound = 70001;

        // 重复评价：用户已对同一对象进行评价
        public const int DuplicateRating = 70002;

        // 评价权限错误：用户没有权限对对象进行评价
        public const int RatingPermissionError = 70003;

        // 评价内容违规：评价内容违反了相关规定
        public const int RatingContentViolation = 70004;

        // 评价时间过期：评价时间超出规定范围
        public const int RatingTimeExpired = 70005;
        #endregion


        #region 系统错误 (90000-99999)：系统内部错误
        // 系统内部错误：未归类的一般性系统错误
        public const int SystemError = 90001;

        // 数据库错误：数据库操作失败
        public const int DatabaseError = 90002;
        
        // 缓存错误：缓存操作失败
        public const int CacheError = 90003;

        // 外部服务错误：调用外部服务失败
        public const int ExternalServiceError = 90004;

        // 网络错误：网络通信失败
        public const int NetworkError = 90005;

        // 配置错误：系统配置有误
        public const int ConfigurationError = 90006;

        // 服务不可用：系统暂时无法提供服务
        public const int ServiceUnavailable = 90007;
        
        // 文件操作错误：文件读写操作失败
        public const int FileOperationError = 90008;

        // 消息队列错误：消息队列操作失败
        public const int MessageQueueError = 90009;

        // 地图服务错误：地图API调用失败
        public const int MapServiceError = 90010;
        #endregion
    }
}
