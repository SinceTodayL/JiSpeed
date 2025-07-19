using Microsoft.AspNetCore.Mvc;
using JISpeed.Api.Common;
using JISpeed.Core.Exceptions;
using System;

namespace JISpeed.Api.Controllers
{
    // 模块异常测试控制器 - 用于测试各模块的异常处理
    [ApiController]
    [Route("api/[controller]")]
    public class ModuleExceptionTestController : ControllerBase
    {
        private readonly ILogger<ModuleExceptionTestController> _logger;

        public ModuleExceptionTestController(ILogger<ModuleExceptionTestController> logger)
        {
            _logger = logger;
        }

        #region 骑手模块异常测试

        // 测试骑手未找到异常
        [HttpGet("rider/not-found")]
        public ApiResponse<string> TestRiderNotFoundException()
        {
            string riderId = "R12345";
            throw RiderExceptions.RiderNotFound(riderId);
        }

        // 测试骑手状态错误异常
        [HttpGet("rider/status-error")]
        public ApiResponse<string> TestRiderStatusException()
        {
            string riderId = "R12345";
            string currentStatus = "Busy";
            string requiredStatus = "Available";
            throw RiderExceptions.RiderStatusError(riderId, currentStatus, requiredStatus);
        }

        // 测试骑手运力已满异常
        [HttpGet("rider/capacity-exceeded")]
        public ApiResponse<string> TestRiderCapacityExceededException()
        {
            string riderId = "R12345";
            int currentOrders = 5;
            throw RiderExceptions.RiderCapacityExceeded(riderId, currentOrders);
        }

        // 测试骑手超出服务区域异常
        [HttpGet("rider/out-of-service-area")]
        public ApiResponse<string> TestRiderOutOfServiceAreaException()
        {
            string riderId = "R12345";
            string location = "远郊区某地";
            throw RiderExceptions.RiderOutOfServiceArea(riderId, location);
        }

        #endregion

        #region 订单模块异常测试

        // 测试订单未找到异常
        [HttpGet("order/not-found")]
        public ApiResponse<string> TestOrderNotFoundException()
        {
            string orderId = "O98765";
            throw OrderExceptions.OrderNotFound(orderId);
        }

        // 测试订单状态错误异常
        [HttpGet("order/status-error")]
        public ApiResponse<string> TestOrderStatusException()
        {
            string orderId = "O98765";
            int currentStatus = 3; // 3表示"已结束"
            int requiredStatus = 2; // 2表示"配送中"
            throw OrderExceptions.OrderStatusError(orderId, currentStatus, requiredStatus);
        }

        // 测试订单已取消异常
        [HttpGet("order/cancelled")]
        public ApiResponse<string> TestOrderCancelledException()
        {
            string orderId = "O98765";
            throw OrderExceptions.OrderCancelled(orderId);
        }

        // 测试订单已分配异常
        [HttpGet("order/already-assigned")]
        public ApiResponse<string> TestOrderAlreadyAssignedException()
        {
            string orderId = "O98765";
            string riderId = "R12345";
            throw OrderExceptions.OrderAlreadyAssigned(orderId, riderId);
        }

        #endregion

        #region 配送模块异常测试

        // 测试配送未找到异常
        [HttpGet("delivery/not-found")]
        public ApiResponse<string> TestDeliveryNotFoundException()
        {
            string orderId = "O98765";
            throw DeliveryExceptions.DeliveryNotFound(orderId);
        }

        // 测试配送状态错误异常
        [HttpGet("delivery/status-error")]
        public ApiResponse<string> TestDeliveryStatusException()
        {
            string orderId = "O98765";
            int currentStatus = 3; // 3表示"已完成"
            int requiredStatus = 2; // 2表示"配送中"
            throw DeliveryExceptions.DeliveryStatusError(orderId, currentStatus, requiredStatus);
        }

        // 测试配送区域不支持异常
        [HttpGet("delivery/area-not-supported")]
        public ApiResponse<string> TestDeliveryAreaNotSupportedException()
        {
            string addressId = "A54321";
            string detailedAddress = "远郊区某地";
            throw DeliveryExceptions.DeliveryAreaNotSupported(addressId, detailedAddress);
        }

        // 测试配送路线规划失败异常
        [HttpGet("delivery/route-failed")]
        public ApiResponse<string> TestDeliveryRouteFailedException()
        {
            string orderId = "O98765";
            string riderId = "R12345";
            string reason = "目的地无法到达";
            throw DeliveryExceptions.DeliveryRouteFailed(orderId, riderId, reason);
        }

        #endregion

        #region 商家模块异常测试

        // 测试商家未找到异常
        [HttpGet("merchant/not-found")]
        public ApiResponse<string> TestMerchantNotFoundException()
        {
            string merchantId = "M789";
            throw MerchantExceptions.MerchantNotFound(merchantId);
        }

        // 测试商家已关闭异常
        [HttpGet("merchant/closed")]
        public ApiResponse<string> TestMerchantClosedException()
        {
            string merchantId = "M789";
            throw MerchantExceptions.MerchantClosed(merchantId);
        }

        // 测试商家状态错误异常
        [HttpGet("merchant/status-error")]
        public ApiResponse<string> TestMerchantStatusException()
        {
            string merchantId = "M789";
            int currentStatus = 3; // 3表示"已关闭"
            int requiredStatus = 1; // 1表示"正在营业"
            throw MerchantExceptions.MerchantStatusError(merchantId, currentStatus, requiredStatus);
        }

        // 测试商品未找到异常
        [HttpGet("merchant/product-not-found")]
        public ApiResponse<string> TestProductNotFoundException()
        {
            string productId = "P12345";
            throw MerchantExceptions.ProductNotFound(productId);
        }

        // 测试商品缺货异常
        [HttpGet("merchant/product-out-of-stock")]
        public ApiResponse<string> TestProductOutOfStockException()
        {
            string productId = "P12345";
            string productName = "测试商品";
            int requestedQuantity = 10;
            int availableQuantity = 5;
            throw MerchantExceptions.ProductOutOfStock(productId, productName, requestedQuantity, availableQuantity);
        }

        #endregion

        #region 用户模块异常测试

        // 测试用户未找到异常
        [HttpGet("user/not-found")]
        public ApiResponse<string> TestUserNotFoundException()
        {
            string userId = "U12345";
            throw UserExceptions.UserNotFound(userId);
        }

        // 测试用户状态错误异常
        [HttpGet("user/status-error")]
        public ApiResponse<string> TestUserStatusException()
        {
            string userId = "U12345";
            int status = 2; // 2表示"已删除"
            throw UserExceptions.UserStatusError(userId, status);
        }

        // 测试用户地址未找到异常
        [HttpGet("user/address-not-found")]
        public ApiResponse<string> TestUserAddressNotFoundException()
        {
            string addressId = "A12345";
            string userId = "U12345";
            throw UserExceptions.UserAddressNotFound(addressId, userId);
        }

        // 测试用户余额不足异常
        [HttpGet("user/insufficient-balance")]
        public ApiResponse<string> TestUserInsufficientBalanceException()
        {
            string userId = "U12345";
            decimal requiredAmount = 100.00M;
            decimal currentBalance = 50.00M;
            throw UserExceptions.UserInsufficientBalance(userId, requiredAmount, currentBalance);
        }

        // 测试用户已存在异常
        [HttpGet("user/already-exists")]
        public ApiResponse<string> TestUserAlreadyExistsException()
        {
            string account = "test@example.com";
            throw UserExceptions.UserAlreadyExists(account);
        }

        // 测试用户登录失败异常
        [HttpGet("user/login-failed")]
        public ApiResponse<string> TestUserLoginFailedException()
        {
            string account = "test@example.com";
            string reason = "密码错误";
            throw UserExceptions.UserLoginFailed(account, reason);
        }

        #endregion

        #region 支付模块异常测试

        // 测试支付失败异常
        [HttpGet("payment/failed")]
        public ApiResponse<string> TestPaymentFailedException()
        {
            string payId = "P12345";
            string reason = "银行卡余额不足";
            throw PaymentExceptions.PaymentFailed(payId, reason);
        }

        // 测试支付超时异常
        [HttpGet("payment/timeout")]
        public ApiResponse<string> TestPaymentTimeoutException()
        {
            string payId = "P12345";
            throw PaymentExceptions.PaymentTimeout(payId);
        }

        // 测试退款失败异常
        [HttpGet("payment/refund-failed")]
        public ApiResponse<string> TestRefundFailedException()
        {
            string refundId = "R12345";
            string reason = "原交易已关闭";
            throw PaymentExceptions.RefundFailed(refundId, reason);
        }

        // 测试交易未找到异常
        [HttpGet("payment/transaction-not-found")]
        public ApiResponse<string> TestTransactionNotFoundException()
        {
            string payId = "P12345";
            throw PaymentExceptions.TransactionNotFound(payId);
        }

        // 测试重复支付异常
        [HttpGet("payment/duplicate-payment")]
        public ApiResponse<string> TestDuplicatePaymentException()
        {
            string orderId = "O12345";
            string payId = "P12345";
            throw PaymentExceptions.DuplicatePayment(orderId, payId);
        }

        #endregion

        #region 认证模块异常测试

        // 测试未授权异常
        [HttpGet("auth/unauthorized")]
        public ApiResponse<string> TestUnauthorizedException()
        {
            string userId = "U12345";
            throw AuthExceptions.Unauthorized(userId);
        }

        // 测试禁止访问异常
        [HttpGet("auth/forbidden")]
        public ApiResponse<string> TestForbiddenException()
        {
            string userId = "U12345";
            string resource = "管理员控制台";
            throw AuthExceptions.Forbidden(userId, resource);
        }

        // 测试账号已禁用异常
        [HttpGet("auth/account-disabled")]
        public ApiResponse<string> TestAccountDisabledException()
        {
            string userId = "U12345";
            string account = "test@example.com";
            throw AuthExceptions.AccountDisabled(userId, account);
        }

        // 测试无效凭证异常
        [HttpGet("auth/invalid-credentials")]
        public ApiResponse<string> TestInvalidCredentialsException()
        {
            string account = "test@example.com";
            throw AuthExceptions.InvalidCredentials(account);
        }

        // 测试令牌过期异常
        [HttpGet("auth/token-expired")]
        public ApiResponse<string> TestTokenExpiredException()
        {
            string userId = "U12345";
            throw AuthExceptions.TokenExpired(userId);
        }

        #endregion

        #region 评价模块异常测试

        // 测试评价未找到异常
        [HttpGet("rating/not-found")]
        public ApiResponse<string> TestRatingNotFoundException()
        {
            string ratingId = "R12345";
            throw ReviewExceptions.RatingNotFound(ratingId);
        }

        // 测试重复评价异常
        [HttpGet("rating/duplicate")]
        public ApiResponse<string> TestDuplicateRatingException()
        {
            string orderId = "O12345";
            string userId = "U12345";
            throw ReviewExceptions.DuplicateRating(orderId, userId);
        }

        // 测试评价权限错误异常
        [HttpGet("rating/permission-error")]
        public ApiResponse<string> TestRatingPermissionErrorException()
        {
            string userId = "U12345";
            string orderId = "O12345";
            throw ReviewExceptions.RatingPermissionError(userId, orderId);
        }

        // 测试评价内容违规异常
        [HttpGet("rating/content-violation")]
        public ApiResponse<string> TestRatingContentViolationException()
        {
            string content = "包含敏感词汇的评价内容";
            string reason = "包含敏感词汇";
            throw ReviewExceptions.RatingContentViolation(content, reason);
        }

        #endregion

        #region 系统错误测试

        // 测试数据库错误异常
        [HttpGet("system/database-error")]
        public ApiResponse<string> TestDatabaseException()
        {
            string tableName = "Users";
            string operation = "查询";
            string message = "连接超时";
            throw SystemExceptions.DatabaseError(tableName, operation, message);
        }

        // 测试网络错误异常
        [HttpGet("system/network-error")]
        public ApiResponse<string> TestNetworkException()
        {
            string endpoint = "https://api.example.com";
            string message = "连接被拒绝";
            throw SystemExceptions.NetworkError(endpoint, message);
        }

        // 测试系统服务不可用异常
        [HttpGet("system/service-unavailable")]
        public ApiResponse<string> TestServiceUnavailableException()
        {
            string serviceName = "支付服务";
            throw SystemExceptions.ServiceUnavailable(serviceName);
        }

        // 测试配置错误异常
        [HttpGet("system/configuration-error")]
        public ApiResponse<string> TestConfigurationErrorException()
        {
            string configKey = "DatabaseConnection";
            string message = "配置值无效";
            throw SystemExceptions.ConfigurationError(configKey, message);
        }

        // 测试通用错误异常
        [HttpGet("common/general-error")]
        public ApiResponse<string> TestGeneralErrorException()
        {
            string message = "发生了未预期的错误";
            throw CommonExceptions.GeneralError(message);
        }

        // 测试参数验证失败异常
        [HttpGet("common/validation-failed")]
        public ApiResponse<string> TestValidationFailedException()
        {
            string fieldName = "email";
            string reason = "格式不正确";
            throw CommonExceptions.ValidationFailed(fieldName, reason);
        }

        #endregion
    }
}
