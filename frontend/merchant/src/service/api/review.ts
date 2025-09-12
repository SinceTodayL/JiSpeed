import { request } from '../request';

/**
 * 评论相关API
 */

/**
 * Get all reviews for a dish
 * @param merchantId Merchant ID
 * @param dishId Dish ID
 */
export function fetchDishReviews(merchantId: string, dishId: string) {
  return request<Api.Review.ReviewItem[]>({
    url: `/api/reviews/all?merchantId=${merchantId}&dishId=${dishId}`,
    method: 'get'
  });
}

/**
 * Get all reviews for a merchant
 * @param merchantId Merchant ID
 */
export function fetchMerchantReviews(merchantId: string) {
  return request<Api.Review.MerchantReviewsResponse>({
    url: `/api/merchants/${merchantId}/reviews`,
    method: 'get'
  });
}

/**
 * 新增菜品评论
 * @param reviewData 评论数据
 */
export function createDishReview(reviewData: Api.Review.CreateReviewRequest) {
  return request({
    url: '/api/reviews/create',
    method: 'post',
    data: reviewData
  });
}

/**
 * 更新菜品评论
 * @param reviewData 更新的评论数据
 */
export function updateDishReview(reviewData: Api.Review.UpdateReviewRequest) {
  return request({
    url: '/api/reviews/update',
    method: 'post',
    data: reviewData
  });
}

/**
 * 删除菜品评论
 * @param reviewId 评论ID
 */
export function deleteDishReview(reviewId: string) {
  return request({
    url: '/api/reviews/delete',
    method: 'post',
    data: { reviewId }
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