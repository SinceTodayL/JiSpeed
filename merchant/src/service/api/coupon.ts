/**
 * 优惠券相关API
 */

/**
 * 获取商家所有优惠券
 * @param merchantId 商家ID (暂时不使用，因为使用固定的Mock地址)
 */
export function fetchGetAllCoupons(merchantId: string) {
  return fetch('https://m1.apifoxmock.com/m2/6776921-6489236-default/325550885')
    .then(response => {
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      return response.json();
    })
    .then(data => {
      if (data.code === 0) {
        return data.data;
      } else {
        throw new Error(data.message || '获取优惠券数据失败');
      }
    });
}

// 新增优惠券
export function addCoupon(params: Partial<Api.Coupon.CouponItem>) {
  return new Promise((resolve) => {
    // 模拟API调用延迟
    setTimeout(() => {
      resolve({
        code: 0,
        message: '优惠券添加成功',
        data: {
          couponId: 'COUPON_' + Date.now(),
          ...params
        }
      });
    }, 500);
  });
}

// 更新优惠券
export function updateCoupon(params: Partial<Api.Coupon.CouponItem>) {
  return new Promise((resolve) => {
    // 模拟API调用延迟
    setTimeout(() => {
      resolve({
        code: 0,
        message: '优惠券更新成功',
        data: params
      });
    }, 500);
  });
}

// 删除优惠券
export function deleteCoupon(couponId: string) {
  return new Promise((resolve) => {
    // 模拟API调用延迟
    setTimeout(() => {
      resolve({
        code: 0,
        message: '优惠券删除成功',
        data: null
      });
    }, 300);
  });
}

// 批量删除优惠券
export function batchDeleteCoupon(couponIds: string[]) {
  return new Promise((resolve) => {
    // 模拟API调用延迟
    setTimeout(() => {
      resolve({
        code: 0,
        message: `成功删除${couponIds.length}个优惠券`,
        data: null
      });
    }, 500);
  });
} 