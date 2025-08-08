import { request } from '../request';

/**
 * 获取商家基本信息
 * @param merchantId 商家ID
 */
export function fetchMerchantInfo(merchantId: string) {
  return request<Api.Merchant.MerchantInfo>({
    url: `/api/merchants/${merchantId}`,
    method: 'get'
  });
}

/**
 * 获取商家销售统计信息 - 7 by default
 * @param merchantId 商家ID
 * @param options 可选查询参数
 */
export function fetchMerchantSalesStats(
  merchantId: string, 
  options?: {
    startTime?: string;
    endTime?: string;
    minAmount?: number;
    maxAmount?: number;
    statType?: number;
    size?: number;
    page?: number;
  }
) {
  // 默认获取最近7天的数据
  const endTime = options?.endTime || new Date().toISOString();
  const startTime = options?.startTime || new Date(Date.now() - 7 * 24 * 60 * 60 * 1000).toISOString();
  
  const params = new URLSearchParams();
  params.append('startTime', startTime);
  params.append('endTime', endTime);
  
  if (options?.minAmount !== undefined) params.append('minAmount', options.minAmount.toString());
  if (options?.maxAmount !== undefined) params.append('maxAmount', options.maxAmount.toString());
  if (options?.statType !== undefined) params.append('statType', options.statType.toString());
  if (options?.size !== undefined) params.append('size', options.size.toString());
  if (options?.page !== undefined) params.append('page', options.page.toString());

  return request<Api.Merchant.SalesStatResponse>({
    url: `/api/merchants/${merchantId}/SalesStat?${params.toString()}`,
    method: 'get'
  });
}

/**
 * 根据商家ID获取所有菜品信息
 * @param merchantId 商家ID
 */
export function fetchMerchantDishes(merchantId: string) {
  return request<Api.Goods.DishItem[]>({
    url: `/api/merchants/${merchantId}/getAllDishes`,
    method: 'get'
  });
} 