/**
 * @author luohan
 * 订单管理API接口
 */
import { get, post, put } from '../utils/request.js';

/**
 * 获取所有商家列表
 * @param params - 查询参数
 */
export function getAllMerchants(params: any = {}) {
  console.log('获取所有商家列表', params);
  return get('/api/merchants', params);
}

/**
 * 获取指定商家的订单列表
 * @param merchantId - 商家ID
 * @param params - 查询参数
 */
export function getMerchantOrders(merchantId: string, params: any = {}) {
  console.log(`获取商家 ${merchantId} 的订单列表`, params);
  return get(`/api/merchants/${merchantId}/orders`, params);
}

/**
 * 获取待派单的订单列表
 * @param params - 查询参数 {page, pageSize}
 */
export async function getPendingOrders(params: any = {}) {
  console.log('获取待派单订单列表', params);
  const { page = 1, pageSize = 50 } = params;
  
  try {
    // 1. 获取所有商家列表
    const merchantsResponse = await getAllMerchants({ page: 1, size: 1000 });
    const merchants = merchantsResponse.data || [];
    
    if (!merchants.length) {
      return { data: [], total: 0 };
    }
    
    // 2. 为每个商家获取待派单订单ID（orderStatus=1表示已支付待派单）
    const allOrderIds = [];
    for (const merchant of merchants) {
      try {
        const ordersResponse = await getMerchantOrders(merchant.merchantId, {
          orderStatus: 1, // 已支付待派单
          page: 1,
          size: 1000
        });
        const orderIds = ordersResponse.data || [];
        allOrderIds.push(...orderIds);
      } catch (error) {
        console.warn(`获取商家 ${merchant.merchantId} 订单失败:`, error);
      }
    }
    
    if (!allOrderIds.length) {
      return { data: [], total: 0 };
    }
    
    // 3. 分页处理订单ID
    const startIndex = (page - 1) * pageSize;
    const endIndex = startIndex + pageSize;
    const paginatedOrderIds = allOrderIds.slice(startIndex, endIndex);
    
    // 4. 批量获取订单详情
    const orderDetailsResponse = await getBatchOrderDetails(paginatedOrderIds);
    const orderDetails = orderDetailsResponse.data || [];
    
    return {
      data: orderDetails,
      total: allOrderIds.length,
      page,
      pageSize
    };
  } catch (error) {
    console.error('获取待派单订单失败:', error);
    throw error;
  }
}

/**
 * 自动派单API - 分配订单给最近的在线骑手
 * @param orderId - 订单ID
 */
export function autoAssignOrder(orderId: string) {
  console.log(`自动派单，订单ID: ${orderId}`);
  return post(`/api/OrderAssignment/auto-assign/${orderId}`);
}

/**
 * 手动派单API - 分配订单给指定骑手
 * @param orderAssignmentData - 派单数据 {orderId, riderId}
 */
export function manualAssignOrder(orderAssignmentData: {orderId: string, riderId: string}) {
  console.log('手动派单', orderAssignmentData);
  return post('/api/OrderAssignment/manual-assign', orderAssignmentData);
}

/**
 * 获取订单分配信息
 * @param orderId - 订单ID
 */
export function getOrderAssignment(orderId: string) {
  console.log(`获取订单分配信息，订单ID: ${orderId}`);
  return get(`/api/OrderAssignment/order/${orderId}/assignment`);
}

/**
 * 获取骑手当前分配的订单列表
 * @param riderId - 骑手ID
 */
export function getRiderAssignments(riderId: string) {
  console.log(`获取骑手分配订单，骑手ID: ${riderId}`);
  return get(`/api/OrderAssignment/rider/${riderId}/assignments`);
}

/**
 * 更新订单状态
 * @param statusData - 状态更新数据 {orderId, newStatus, remark}
 */
export function updateOrderStatus(statusData: {orderId: string, newStatus: number, remark?: string}) {
  console.log('更新订单状态', statusData);
  return put('/api/OrderAssignment/update-status', statusData);
}

/**
 * 获取所有进行中的订单状态（用于实时监控）
 * @param params - 查询参数
 */
export async function getAllActiveOrders(params: any = {}) {
  console.log('获取所有进行中的订单状态', params);
  try {
    // 获取所有活跃状态的订单 (状态 3-6: 已接单到已完成)
    const response = await get('/api/orders', {
      status: [3, 4, 5, 6].join(','), // 多状态查询
      pageSize: 100, // 获取更多数据
      ...params
    });
    
    console.log('获取活跃订单响应:', response);
    return response;
  } catch (error) {
    console.error('获取活跃订单失败:', error);
    throw error;
  }
}

/**
 * 获取订单列表（支持多种状态筛选）
 * @param params - 查询参数 {status, page, pageSize, searchTerm}
 */
export function getOrderList(params: any = {}) {
  console.log('获取订单列表', params);
  return get('/api/orders', params);
}

/**
 * 获取订单详情
 * @param orderId - 订单ID
 */
export function getOrderDetail(orderId: string) {
  console.log(`获取订单详情，订单ID: ${orderId}`);
  return get(`/api/orders/${orderId}`);
}

/**
 * 批量获取订单详情
 * @param orderIds - 订单ID列表
 */
export async function getBatchOrderDetails(orderIds: string[]) {
  console.log(`批量获取订单详情，订单数量: ${orderIds.length}`);
  
  try {
    const orderDetails = [];
    for (const orderId of orderIds) {
      try {
        const response = await getOrderDetail(orderId);
        if (response.data) {
          orderDetails.push(response.data);
        }
      } catch (error) {
        console.warn(`获取订单 ${orderId} 详情失败:`, error);
      }
    }
    return { data: orderDetails };
  } catch (error) {
    console.error('批量获取订单详情失败:', error);
    throw error;
  }
}

// 订单状态枚举
export enum OrderStatus {
  Pending = 0,      // 待支付
  Paid = 1,         // 已支付（待派单）
  Assigned = 2,     // 已分配（待接单）
  Accepted = 3,     // 已接单（准备中）
  Preparing = 4,    // 商家准备中
  PickedUp = 5,     // 已取餐（配送中）
  Delivered = 6,    // 已送达
  Completed = 7,    // 已完成
  Cancelled = 8,    // 已取消
  Refunded = 9      // 已退款
}

// 订单状态显示文本映射
export const OrderStatusText = {
  [OrderStatus.Pending]: '待支付',
  [OrderStatus.Paid]: '已支付',
  [OrderStatus.Assigned]: '已分配',
  [OrderStatus.Accepted]: '已接单',
  [OrderStatus.Preparing]: '准备中',
  [OrderStatus.PickedUp]: '配送中',
  [OrderStatus.Delivered]: '已送达',
  [OrderStatus.Completed]: '已完成',
  [OrderStatus.Cancelled]: '已取消',
  [OrderStatus.Refunded]: '已退款'
};

// 订单状态颜色映射
export const OrderStatusColor = {
  [OrderStatus.Pending]: 'warning',
  [OrderStatus.Paid]: 'info',
  [OrderStatus.Assigned]: 'warning',
  [OrderStatus.Accepted]: 'success',
  [OrderStatus.Preparing]: 'info',
  [OrderStatus.PickedUp]: 'success',
  [OrderStatus.Delivered]: 'success',
  [OrderStatus.Completed]: 'success',
  [OrderStatus.Cancelled]: 'error',
  [OrderStatus.Refunded]: 'error'
};

/**
 * 获取骑手当前配送订单状态
 * @param riderId - 骑手ID
 */
export function getRiderDeliveryStatus(riderId: string) {
  console.log(`获取骑手配送状态，骑手ID: ${riderId}`);
  return get(`/api/orders/rider/${riderId}/delivery-status`);
}

/**
 * 获取骑手配送订单列表（包含详细状态）
 * @param riderId - 骑手ID
 * @param params - 查询参数 {status, page, pageSize}
 */
export function getRiderDeliveryOrders(riderId: string, params = {}) {
  console.log(`获取骑手配送订单列表，骑手ID: ${riderId}`, params);
  return get(`/api/orders/rider/${riderId}/delivery-orders`, params);
}

/**
 * 获取骑手实时配送统计
 * @param riderId - 骑手ID
 */
export function getRiderDeliveryStats(riderId: string) {
  console.log(`获取骑手配送统计，骑手ID: ${riderId}`);
  return get(`/api/orders/rider/${riderId}/delivery-stats`);
}

/**
 * 批量获取多个骑手的配送状态
 * @param riderIds - 骑手ID列表
 */
export function getBatchRiderDeliveryStatus(riderIds: string[]) {
  console.log(`批量获取骑手配送状态，骑手数量: ${riderIds.length}`);
  return post('/api/orders/riders/batch-delivery-status', { riderIds });
}

// 配送状态枚举（针对骑手的状态）
export enum DeliveryStatus {
  NoOrders = 0,        // 无订单
  WaitingPickup = 1,   // 等待取餐
  OnTheWay = 2,        // 配送途中
  Delivered = 3,       // 已送达
  Multiple = 4         // 多单配送
}

// 配送状态显示文本
export const DeliveryStatusText = {
  [DeliveryStatus.NoOrders]: '无订单',
  [DeliveryStatus.WaitingPickup]: '等待取餐',
  [DeliveryStatus.OnTheWay]: '配送途中',
  [DeliveryStatus.Delivered]: '已送达',
  [DeliveryStatus.Multiple]: '多单配送'
};

// 配送状态颜色
export const DeliveryStatusColor = {
  [DeliveryStatus.NoOrders]: 'default',
  [DeliveryStatus.WaitingPickup]: 'warning',
  [DeliveryStatus.OnTheWay]: 'processing',
  [DeliveryStatus.Delivered]: 'success',
  [DeliveryStatus.Multiple]: 'purple'
};
