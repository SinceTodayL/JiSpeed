import { request } from '../request';

/**
 * 订单相关API
 */

/**
 * 获取商家所有订单
 * @param merchantId 商家ID
 */
export function fetchGetAllOrders(merchantId: string) {
  return request<Api.Order.OrderItem[]>({
    url: `/api/orders/all?merchantId=${merchantId}`,
    method: 'get'
  });
}

/**
 * 订单状态映射
 */
export const ORDER_STATUS_MAP: Api.Order.OrderStatusMap = {
  1: '待付款',
  2: '待发货', 
  3: '已发货',
  4: '已完成',
  5: '已取消',
  49: '进行中' // 根据mock数据中的状态值
};

/**
 * 获取订单状态文本
 * @param status 订单状态码
 */
export function getOrderStatusText(status: number): string {
  return ORDER_STATUS_MAP[status] || '未知状态';
} 