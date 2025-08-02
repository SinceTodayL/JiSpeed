import { request } from '../request';

/**
 * 获取商家所有菜品
 * @param merchantId 商家ID
 */
export function fetchGetAllDishes(merchantId: string) {
  return request<Api.Goods.DishItem[]>({
    url: `/api/merchant/${merchantId}/getAllDishes`,
    method: 'get'
  });
} 