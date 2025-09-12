// JiSpeed 错误码常量
// 通用错误 (1000-1999)
export const GENERAL_ERROR = {
  GENERAL_ERROR: 1000,             // 通用错误
  VALIDATION_FAILED: 1001,         // 参数验证失败
  MISSING_PARAMETER: 1002,         // 缺少必填参数
  INVALID_REQUEST_FORMAT: 1003,    // 无效的请求格式
  UNSUPPORTED_OPERATION: 1004,     // 不支持的操作
  RATE_LIMIT_EXCEEDED: 1005,       // 请求频率限制
  RESOURCE_NOT_FOUND: 1006,        // 资源未找到
};

// 认证权限 (2000-2999)
export const AUTH_ERROR = {
  UNAUTHORIZED: 2001,              // 未授权
  FORBIDDEN: 2002,                 // 禁止访问
  ACCOUNT_DISABLED: 2003,          // 账号禁用
  INVALID_CREDENTIALS: 2004,       // 无效凭证
  TOKEN_EXPIRED: 2005,             // 令牌过期
  INVALID_TOKEN: 2006,             // 无效令牌
};

// 骑手模块 (10000-10999)
export const RIDER_ERROR = {
  RIDER_NOT_FOUND: 10001,          // 骑手未找到
  RIDER_STATUS_ERROR: 10002,       // 骑手状态错误
  RIDER_CAPACITY_EXCEEDED: 10003,  // 骑手容量超限
  RIDER_OUT_OF_SERVICE_AREA: 10004,// 骑手超出服务区域
  RIDER_ALREADY_EXISTS: 10005,     // 骑手已存在
  RIDER_AUTHENTICATION_FAILED: 10006, // 骑手认证失败
  RIDER_LOCATION_UPDATE_FAILED: 10007, // 骑手位置更新失败
  RIDER_RATING_TOO_LOW: 10008,     // 骑手评分过低
  RIDER_ON_BREAK: 10009,           // 骑手休息中
  RIDER_DEVICE_OFFLINE: 10010,     // 骑手设备离线
};

// 订单模块 (20000-20999)
export const ORDER_ERROR = {
  ORDER_NOT_FOUND: 20001,          // 订单未找到
  ORDER_STATUS_ERROR: 20002,       // 订单状态错误
  ORDER_CANCELLED: 20003,          // 订单已取消
  ORDER_ALREADY_ASSIGNED: 20004,   // 订单已分配
  ORDER_ASSIGNMENT_FAILED: 20005,  // 订单分配失败
  ORDER_AMOUNT_ERROR: 20006,       // 订单金额错误
  ORDER_TIMEOUT: 20007,            // 订单超时
  ORDER_ITEM_ERROR: 20008,         // 订单项目错误
  ORDER_ADDRESS_INVALID: 20009,    // 订单地址无效
  ORDER_CREATION_FAILED: 20010,    // 订单创建失败
};

// 配送模块 (30000-30999)
export const DELIVERY_ERROR = {
  DELIVERY_NOT_FOUND: 30001,       // 配送未找到
  DELIVERY_STATUS_ERROR: 30002,    // 配送状态错误
  DELIVERY_AREA_NOT_SUPPORTED: 30003, // 配送区域不支持
  DELIVERY_ROUTE_FAILED: 30004,    // 配送路线规划失败
  DELIVERY_DISTANCE_EXCEEDED: 30005, // 配送距离超限
  DELIVERY_TIME_CONFLICT: 30006,   // 配送时间冲突
  DELIVERY_DELAYED: 30007,         // 配送延迟
  DELIVERY_CANCEL_FAILED: 30008,   // 配送取消失败
  DELIVERY_COMPLETION_FAILED: 30009, // 配送完成失败
};

// 商家模块 (40000-40999)
export const MERCHANT_ERROR = {
  MERCHANT_NOT_FOUND: 40001,       // 商家未找到
  MERCHANT_STATUS_ERROR: 40002,    // 商家状态错误
  MERCHANT_CLOSED: 40003,          // 商家已关闭
  MERCHANT_AUTHENTICATION_FAILED: 40004, // 商家认证失败
  PRODUCT_NOT_FOUND: 40005,        // 商品未找到
  PRODUCT_UNAVAILABLE: 40006,      // 商品不可用
  PRODUCT_OUT_OF_STOCK: 40007,     // 商品缺货
  MERCHANT_BUSINESS_HOURS_ERROR: 40008, // 商家营业时间错误
  MERCHANT_ORDER_REJECTED: 40009,  // 商家订单拒绝
};

// 用户模块 (50000-50999)
export const USER_ERROR = {
  USER_NOT_FOUND: 50001,           // 用户未找到
  USER_STATUS_ERROR: 50002,        // 用户状态错误
  USER_ADDRESS_NOT_FOUND: 50003,   // 用户地址未找到
  USER_INSUFFICIENT_BALANCE: 50004,// 用户余额不足
  USER_INCOMPLETE_INFO: 50005,     // 用户信息不完整
  USER_ALREADY_EXISTS: 50006,      // 用户已存在
  USER_VERIFICATION_CODE_ERROR: 50007, // 用户验证码错误
  USER_LOGIN_FAILED: 50008,        // 用户登录失败
};

// 支付模块 (60000-60999)
export const PAYMENT_ERROR = {
  PAYMENT_FAILED: 60001,           // 支付失败
  PAYMENT_TIMEOUT: 60002,          // 支付超时
  REFUND_FAILED: 60003,            // 退款失败
  PAYMENT_METHOD_NOT_SUPPORTED: 60004, // 支付方式不支持
  TRANSACTION_NOT_FOUND: 60005,    // 交易未找到
  DUPLICATE_PAYMENT: 60006,        // 重复支付
  PAYMENT_AMOUNT_ERROR: 60007,     // 支付金额错误
  REFUND_AMOUNT_EXCEEDED: 60008,   // 退款金额超限
};

// 评价模块 (70000-70999)
export const RATING_ERROR = {
  RATING_NOT_FOUND: 70001,         // 评价未找到
  DUPLICATE_RATING: 70002,         // 重复评价
  RATING_PERMISSION_ERROR: 70003,  // 评价权限错误
  RATING_CONTENT_VIOLATION: 70004, // 评价内容违规
  RATING_TIME_EXPIRED: 70005,      // 评价时间过期
};

// 系统错误 (90000-99999)
export const SYSTEM_ERROR = {
  SYSTEM_ERROR: 90001,             // 系统内部错误
  DATABASE_ERROR: 90002,           // 数据库错误
  CACHE_ERROR: 90003,              // 缓存错误
  EXTERNAL_SERVICE_ERROR: 90004,   // 外部服务错误
  NETWORK_ERROR: 90005,            // 网络错误
  CONFIGURATION_ERROR: 90006,      // 配置错误
  SERVICE_UNAVAILABLE: 90007,      // 服务不可用
  FILE_OPERATION_ERROR: 90008,     // 文件操作错误
  MESSAGE_QUEUE_ERROR: 90009,      // 消息队列错误
  MAP_SERVICE_ERROR: 90010,        // 地图服务错误
};

// 错误码分组
export const ERROR_CODES = {
  ...GENERAL_ERROR,
  ...AUTH_ERROR,
  ...RIDER_ERROR,
  ...ORDER_ERROR,
  ...DELIVERY_ERROR,
  ...MERCHANT_ERROR,
  ...USER_ERROR,
  ...PAYMENT_ERROR,
  ...RATING_ERROR,
  ...SYSTEM_ERROR
};


// 根据错误码获取错误类型
export function getErrorType(code) {
  if (code === 0) return 'SUCCESS';
  if (code >= 1000 && code < 2000) return 'GENERAL_ERROR';
  if (code >= 2000 && code < 3000) return 'AUTH_ERROR';
  if (code >= 10000 && code < 11000) return 'RIDER_ERROR';
  if (code >= 20000 && code < 21000) return 'ORDER_ERROR';
  if (code >= 30000 && code < 31000) return 'DELIVERY_ERROR';
  if (code >= 40000 && code < 41000) return 'MERCHANT_ERROR';
  if (code >= 50000 && code < 51000) return 'USER_ERROR';
  if (code >= 60000 && code < 61000) return 'PAYMENT_ERROR';
  if (code >= 70000 && code < 71000) return 'RATING_ERROR';
  if (code >= 90000 && code < 100000) return 'SYSTEM_ERROR';
  return 'UNKNOWN_ERROR';
}


// 判断是否为业务错误（可预期的错误）
export function isBusinessError(code) {
  // 系统错误码范围为90000-99999，其他为业务错误
  return code > 0 && code < 90000;
}


// 判断是否需要重新登录的错误
export function isAuthError(code) {
  return code === AUTH_ERROR.UNAUTHORIZED || 
         code === AUTH_ERROR.TOKEN_EXPIRED || 
         code === AUTH_ERROR.INVALID_TOKEN;
}

export default ERROR_CODES;
