import { request } from '../request';
import { fetchDishDetail } from './goods';

/**
 * 订单相关API
 */

/**
 * 获取商家所有订单ID列表
 * @param merchantId 商家ID
 * @param params 查询参数
 */
export function fetchGetAllOrders(
  merchantId: string, 
  params?: {
    startDate?: string;
    endDate?: string;
    size?: number;
    page?: number;
    orderStatus?: number;
  }
) {
  const queryParams = new URLSearchParams();
  
  if (params?.startDate) queryParams.append('startDate', params.startDate);
  if (params?.endDate) queryParams.append('endDate', params.endDate);
  if (params?.size) queryParams.append('size', params.size.toString());
  if (params?.page) queryParams.append('page', params.page.toString());
  if (params?.orderStatus !== undefined) queryParams.append('orderStatus', params.orderStatus.toString());
  
  const queryString = queryParams.toString();
  const url = `/api/merchants/${merchantId}/orders${queryString ? `?${queryString}` : ''}`;
  
  return request<string[]>({
    url,
    method: 'get'
  });
}

/**
 * 订单状态映射
 */
export const ORDER_STATUS_MAP: Api.Order.OrderStatusMap = {
  0: '未支付',      // Unpaid
  1: '已支付',      // Paid
  2: '确认收货',    // Confirmed
  3: '已评价',      // Reviewed
  4: '售后中',      // Aftersales
  5: '售后结束',    // AftersalesCompleted
  6: '订单关闭'     // Cancelled
};

/**
 * 根据订单ID获取订单详情
 * @param orderId 订单ID
 */
export function fetchOrderDetail(orderId: string) {
  return request<Api.Order.OrderDetailItem>({
    url: `/api/orders/${orderId}`,
    method: 'get'
  });
}

/**
 * 批量获取订单详情
 * @param orderIds 订单ID数组
 */
export async function fetchOrderDetailsBatch(orderIds: string[]): Promise<any[]> {
  try {
    const promises = orderIds.map(orderId => 
      fetchOrderDetail(orderId).then(response => {
        // @sa/axios返回包装对象，需要提取data字段
        return response?.data || null;
      }).catch(error => {
        console.warn(`获取订单详情失败 ${orderId}:`, error);
        return null;
      })
    );
    
    const results = await Promise.all(promises);
    return results.filter((result): result is any => result !== null);
  } catch (error) {
    console.error('批量获取订单详情失败:', error);
    throw error;
  }
}

/**
 * 批量获取菜品详情
 * @param merchantId 商家ID
 * @param dishIds 菜品ID数组
 */
export async function fetchDishDetailsBatch(merchantId: string, dishIds: string[]): Promise<Record<string, Api.Goods.DishItem>> {
  try {
    console.log('=== 开始批量获取菜品详情 ===');
    console.log('商家ID:', merchantId);
    console.log('菜品ID列表:', dishIds);
    
    const promises = dishIds.map(dishId => 
      fetchDishDetail(merchantId, dishId).then(response => {
        console.log(`菜品${dishId}获取成功:`, response);
        const dishData = response?.data;
        return { dishId, dishData };
      }).catch(error => {
        console.warn(`获取菜品详情失败 ${dishId}:`, error);
        console.warn('错误详情:', error.response?.data);
        return { dishId, dishData: null };
      })
    );
    
    const results = await Promise.all(promises);
    console.log('批量获取结果:', results);
    
    const dishMap: Record<string, Api.Goods.DishItem> = {};
    
    results.forEach(({ dishId, dishData }) => {
      if (dishData) {
        dishMap[dishId] = dishData;
      }
    });
    
    console.log('最终菜品详情映射:', dishMap);
    return dishMap;
  } catch (error) {
    console.error('批量获取菜品详情失败:', error);
    return {};
  }
}

/**
 * 获取订单状态文本
 * @param status 订单状态码
 */
export function getOrderStatusText(status: number): string {
  return ORDER_STATUS_MAP[status] || '未知状态';
} 