import { request } from '../request';

/**
 * 获取商家所有优惠券
 * @param merchantId 商家ID
 */
export function fetchGetAllCoupons(merchantId: string) {
  return request<Api.Coupon.CouponItem[]>({
    url: `/api/coupons/all?merchantId=${merchantId}`,
    method: 'get'
  });
}

// 新增优惠券
export function addCoupon(params: Partial<Api.Coupon.CouponItem>) {
  return request({
    url: '/api/coupons/add',
    method: 'post',
    data: params
  });
}

// 更新优惠券
export function updateCoupon(params: Partial<Api.Coupon.CouponItem>) {
  return request({
    url: '/api/coupons/update',
    method: 'post',
    data: params
  });
}

// 删除优惠券
export function deleteCoupon(couponId: string) {
  return request({
    url: '/api/coupons/delete',
    method: 'post',
    data: { couponId }
  });
}

// 批量删除优惠券
export function batchDeleteCoupon(couponIds: string[]) {
  return request({
    url: '/api/coupons/batchDelete',
    method: 'post',
    data: { couponIds }
  });
} 