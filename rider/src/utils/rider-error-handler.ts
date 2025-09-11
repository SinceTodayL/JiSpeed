import type { MessageApi } from 'naive-ui';

declare global {
  interface Window {
    $message?: MessageApi;
  }
}

/**
 * 骑手异常处理工具
 * 用于处理骑手相关的业务异常和错误码
 */

// 骑手相关错误码映射
export const RIDER_ERROR_CODES = {
  RIDER_NOT_FOUND: 'RIDER_001',
  RIDER_STATUS_ERROR: 'RIDER_002',
  RIDER_CAPACITY_EXCEEDED: 'RIDER_003',
  RIDER_OUT_OF_SERVICE_AREA: 'RIDER_004',
  RIDER_ALREADY_EXISTS: 'RIDER_005',
  RIDER_AUTHENTICATION_FAILED: 'RIDER_006',
  RIDER_LOCATION_UPDATE_FAILED: 'RIDER_007',
  RIDER_RATING_TOO_LOW: 'RIDER_008',
  RIDER_ON_BREAK: 'RIDER_009',
  RIDER_DEVICE_OFFLINE: 'RIDER_010',
  RIDER_VEHICLE_ERROR: 'RIDER_011'
} as const;

// 错误码对应的用户友好消息
export const RIDER_ERROR_MESSAGES = {
  [RIDER_ERROR_CODES.RIDER_NOT_FOUND]: '骑手信息未找到',
  [RIDER_ERROR_CODES.RIDER_STATUS_ERROR]: '骑手状态异常，无法执行此操作',
  [RIDER_ERROR_CODES.RIDER_CAPACITY_EXCEEDED]: '当前订单数量已达上限，无法接新订单',
  [RIDER_ERROR_CODES.RIDER_OUT_OF_SERVICE_AREA]: '当前位置不在服务范围内',
  [RIDER_ERROR_CODES.RIDER_ALREADY_EXISTS]: '该骑手账号已存在',
  [RIDER_ERROR_CODES.RIDER_AUTHENTICATION_FAILED]: '骑手身份验证失败',
  [RIDER_ERROR_CODES.RIDER_LOCATION_UPDATE_FAILED]: '位置更新失败，请检查网络连接',
  [RIDER_ERROR_CODES.RIDER_RATING_TOO_LOW]: '评分过低，暂时无法接单',
  [RIDER_ERROR_CODES.RIDER_ON_BREAK]: '当前处于休息状态，无法接单',
  [RIDER_ERROR_CODES.RIDER_DEVICE_OFFLINE]: '设备离线，请检查网络连接',
  [RIDER_ERROR_CODES.RIDER_VEHICLE_ERROR]: '车辆信息异常，请更新车辆信息'
} as const;

/**
 * 处理骑手相关异常
 * @param error 错误对象
 * @param defaultMessage 默认错误消息
 */
export function handleRiderError(error: any, defaultMessage = '操作失败，请稍后重试') {
  let message = defaultMessage;
  let errorCode = '';

  // 获取后端错误码和消息
  if (error?.response?.data) {
    const responseData = error.response.data;
    errorCode = String(responseData.code || '');
    message = responseData.msg || message;
  }

  // 检查是否为骑手相关错误码
  if (errorCode && RIDER_ERROR_MESSAGES[errorCode as keyof typeof RIDER_ERROR_MESSAGES]) {
    message = RIDER_ERROR_MESSAGES[errorCode as keyof typeof RIDER_ERROR_MESSAGES];
  }

  // 显示错误消息
  window.$message?.error(message);


  return {
    errorCode,
    message,
    originalError: error
  };
}

/**
 * 处理订单相关异常
 * @param error 错误对象
 * @param defaultMessage 默认错误消息
 */
export function handleOrderError(error: any, defaultMessage = '订单操作失败，请稍后重试') {
  let message = defaultMessage;
  let errorCode = '';

  if (error?.response?.data) {
    const responseData = error.response.data;
    errorCode = String(responseData.code || '');
    message = responseData.msg || message;
  }

  // 订单相关错误码处理
  const ORDER_ERROR_MESSAGES: Record<string, string> = {
    ORDER_001: '订单不存在',
    ORDER_002: '订单状态不允许此操作',
    ORDER_003: '订单已被其他骑手接单',
    ORDER_004: '订单已超时',
    ORDER_005: '订单金额异常',
    ORDER_006: '订单地址信息不完整'
  };

  if (errorCode && ORDER_ERROR_MESSAGES[errorCode]) {
    message = ORDER_ERROR_MESSAGES[errorCode];
  }

  window.$message?.error(message);


  return {
    errorCode,
    message,
    originalError: error
  };
}

/**
 * 处理考勤相关异常
 * @param error 错误对象
 * @param defaultMessage 默认错误消息
 */
export function handleAttendanceError(error: any, defaultMessage = '考勤操作失败，请稍后重试') {
  let message = defaultMessage;
  let errorCode = '';

  if (error?.response?.data) {
    const responseData = error.response.data;
    errorCode = String(responseData.code || '');
    message = responseData.msg || message;
  }

  // 考勤相关错误码处理
  const ATTENDANCE_ERROR_MESSAGES: Record<string, string> = {
    ATTENDANCE_001: '今日已打卡',
    ATTENDANCE_002: '打卡时间异常',
    ATTENDANCE_003: '位置信息不准确',
    ATTENDANCE_004: '考勤记录不存在',
    ATTENDANCE_005: '不在工作时间范围内'
  };

  if (errorCode && ATTENDANCE_ERROR_MESSAGES[errorCode]) {
    message = ATTENDANCE_ERROR_MESSAGES[errorCode];
  }

  window.$message?.error(message);


  return {
    errorCode,
    message,
    originalError: error
  };
}

/**
 * 通用异常处理
 * @param error 错误对象
 * @param context 操作上下文
 */
export function handleCommonError(error: any, context = '操作') {
  let message = `${context}失败，请稍后重试`;
  let errorCode = '';

  if (error?.response?.data) {
    const responseData = error.response.data;
    errorCode = String(responseData.code || '');
    message = responseData.msg || message;
  }

  // 网络错误处理
  if (error?.code === 'NETWORK_ERROR') {
    message = '网络连接失败，请检查网络设置';
  } else if (error?.code === 'TIMEOUT') {
    message = '请求超时，请稍后重试';
  }

  window.$message?.error(message);


  return {
    errorCode,
    message,
    originalError: error
  };
}
