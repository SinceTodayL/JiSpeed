/**
 * 评论相关API
 */

/**
 * 获取菜品的所有评论
 * @param merchantId 商家ID
 * @param dishId 菜品ID
 */
export function fetchDishReviews(merchantId: string, dishId: string) {
  // 使用提供的 Mock API 地址
  return fetch('https://m1.apifoxmock.com/m2/6776921-6489236-default/326111349')
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
        throw new Error(data.message || '获取菜品评论失败');
      }
    });
}

/**
 * 新增菜品评论
 * @param merchantId 商家ID
 * @param dishId 菜品ID
 * @param reviewData 评论数据
 */
export function createDishReview(merchantId: string, dishId: string, reviewData: Api.Review.CreateReviewRequest) {
  // TODO: 实际项目中这里应该调用真实的创建评论API
  return new Promise((resolve) => {
    setTimeout(() => {
      resolve({
        code: 0,
        message: '评论提交成功',
        data: {
          reviewId: 'REVIEW_' + Date.now(),
          ...reviewData,
          reviewAt: new Date().toISOString(),
          status: 2 // 默认已通过状态
        }
      });
    }, 1000);
  });
}

/**
 * 更新菜品评论
 * @param merchantId 商家ID
 * @param dishId 菜品ID
 * @param reviewData 更新的评论数据
 */
export function updateDishReview(merchantId: string, dishId: string, reviewData: Api.Review.UpdateReviewRequest) {
  // TODO: 实际项目中这里应该调用真实的更新评论API
  return new Promise((resolve) => {
    setTimeout(() => {
      resolve({
        code: 0,
        message: '评论更新成功',
        data: reviewData
      });
    }, 800);
  });
}

/**
 * 删除菜品评论
 * @param merchantId 商家ID
 * @param dishId 菜品ID
 * @param reviewId 评论ID
 */
export function deleteDishReview(merchantId: string, dishId: string, reviewId: string) {
  // TODO: 实际项目中这里应该调用真实的删除评论API
  return new Promise((resolve) => {
    setTimeout(() => {
      resolve({
        code: 0,
        message: '评论删除成功',
        data: { reviewId }
      });
    }, 500);
  });
}

/**
 * 评论状态映射
 */
export const REVIEW_STATUS_MAP = {
  1: '待审核',
  2: '已通过', 
  3: '已拒绝'
};

/**
 * 获取评论状态文本
 * @param status 状态码
 */
export function getReviewStatusText(status: number): string {
  return REVIEW_STATUS_MAP[status as keyof typeof REVIEW_STATUS_MAP] || '未知状态';
} 