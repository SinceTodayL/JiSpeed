import { get, post } from '../utils/request.js';

/**
 * 管理员发放优惠券
 * @param adminId - 管理员ID
 * @param coupon - 优惠券信息
 */
export function issueCoupon(adminId, coupon) {
  console.log(`管理员 ${adminId} 发放优惠券:`, coupon);
  return post(`/api/admin/${adminId}/coupons`, coupon);
}

/**
 * 获取优惠券列表
 * @param params - 查询参数
 */
export function getCouponList(params = {}) {
  console.log('获取优惠券列表', params);
  return get('/api/coupons', params);
}

/**
 * 获取用户的优惠券
 * @param userId - 用户ID
 * @param params - 查询参数
 */
export function getUserCoupons(userId, params = {}) {
  console.log(`获取用户 ${userId} 的优惠券`, params);
  return get(`/api/users/${userId}/coupons`, params);
}

