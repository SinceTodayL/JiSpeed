import { request } from '../request';

/**
 * 获取商家基本信息
 * @param merchantId 商家ID
 */
export function fetchMerchantInfo(merchantId: string) {
  return request<Api.Merchant.MerchantInfo>({
    url: `/api/merchant/${merchantId}`,
    method: 'get'
  });
}

/**
 * 获取商家销售统计信息
 * @param merchantId 商家ID
 */
export function fetchMerchantSalesStats(merchantId: string) {
  return request<Api.Merchant.SalesStatItem[]>({
    url: `/api/merchant/${merchantId}/SalesStat`,
    method: 'get'
  });
}

/**
 * 根据商家ID获取所有菜品信息
 * @param merchantId 商家ID
 */
export function fetchMerchantDishes(merchantId: string) {
  return request<Api.Goods.DishItem[]>({
    url: `/api/merchant/${merchantId}/getAllDishes`,
    method: 'get'
  });
} 