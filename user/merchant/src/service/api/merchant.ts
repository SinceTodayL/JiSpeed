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

/**
 * 更新商家信息
 * @param merchantId 商家ID
 * @param data 要更新的商家信息
 */
export function updateMerchantInfo(merchantId: string, data: {
  merchantName?: string;
  status?: number;
  contactInfo?: string;
  location?: string;
}) {
  return request<{ code: number; message: string; data: Api.Merchant.MerchantInfo }>({
    url: `/api/merchants/${merchantId}`,
    method: 'patch',
    data
  });
}

/**
 * 获取商家排行榜（默认按销售额降序排列）
 * @param params 可选的筛选参数
 */
export function fetchMerchantsRanking(params?: {
  merchantName?: string;
  location?: string;
  size?: number;
  page?: number;
  status?: number;
}) {
  return request<Api.Merchant.MerchantRankingItem[]>({
    url: '/api/merchants',
    method: 'get',
    params
  });
}

/**
 * 根据商家ID和日期获取销售统计
 * @param merchantId 商家ID
 * @param statDate 统计日期 (YYYY-MM-DD格式)
 */
export function fetchMerchantSalesStatByDate(merchantId: string, statDate: string) {
  return request<Api.Merchant.SalesStatByDateItem>({
    url: `/api/merchants/${merchantId}/SalesStat/${statDate}`,
    method: 'get'
  });
}

/**
 * 获取商家最近30天销售总额
 * @param merchantId 商家ID
 */
export async function fetchMerchant30DaysSales(merchantId: string): Promise<number> {
  try {
    const promises: Promise<Api.Merchant.SalesStatByDateItem>[] = [];
    const today = new Date();
    
    // 获取最近30天的销售数据
    for (let i = 0; i < 30; i++) {
      const date = new Date(today.getTime() - i * 24 * 60 * 60 * 1000);
      const dateString = date.toISOString().split('T')[0]; // YYYY-MM-DD格式
      promises.push(
        fetchMerchantSalesStatByDate(merchantId, dateString).catch(() => null as any)
      );
    }
    
    const results = await Promise.all(promises);
    let totalSales = 0;
    
    results.forEach(result => {
      if (result) {
        // 尝试不同的数据结构
        const resultAny = result as any;
        let salesAmount = 0;
        
        if (resultAny?.data?.salesAmount) {
          salesAmount = resultAny.data.salesAmount;
        } else if (resultAny?.salesAmount) {
          salesAmount = resultAny.salesAmount;
        }
        
        totalSales += salesAmount;
      }
    });
    
    return totalSales;
  } catch (error) {
    console.warn(`获取商家${merchantId}30天销售数据失败:`, error);
    return 0;
  }
} 