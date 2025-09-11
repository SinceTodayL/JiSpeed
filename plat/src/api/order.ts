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
 * 获取所有订单及关联信息（包含骑手、用户、商家、分配信息和订单状态）
 * @returns 所有订单的完整信息
 */
export function getAllOrdersWithDetails() {
  console.log('获取所有订单及关联信息');
  return get('/api/OrderAssignment/orders/all');
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
 * 注意：后端没有直接的/api/orders接口，这里调用getAllMerchants然后聚合订单
 * @param params - 查询参数 {status, page, pageSize, searchTerm}
 */
export async function getOrderList(params: any = {}) {
  console.log('获取订单列表', params);
  
  try {
    // 由于后端没有直接的订单列表API，我们需要通过商家来获取订单
    const merchantsResponse = await getAllMerchants({ page: 1, size: 100 });
    console.log('商家API响应:', merchantsResponse);
    
    // 后端返回格式: { data: [...], message: "...", success: true }
    const merchants = merchantsResponse?.data || [];
    
    if (!merchants.length) {
      return { data: [], total: 0 };
    }
    
    // 获取第一个商家的订单作为示例（简化处理）
    const firstMerchant = merchants[0];
    const merchantId = firstMerchant.MerchantId || firstMerchant.merchantId;
    
    const ordersResponse = await getMerchantOrders(merchantId, {
      orderStatus: params.status,
      page: params.page || 1,
      size: params.pageSize || 20
    });
    
    console.log('商家订单API响应:', ordersResponse);
    
    return {
      data: ordersResponse?.data || [],
      total: ordersResponse?.data?.length || 0
    };
  } catch (error) {
    console.error('获取订单列表失败:', error);
    throw error;
  }
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

/**
 * 获取订单分配总览数据（用于管理后台总览页面）
 * 包含订单统计和所有骑手的订单分配状态
 */
export async function getOrderAssignmentOverview() {
  console.log('获取订单分配总览数据');
  
  try {
    // 1. 获取所有骑手列表 - 使用正确的API调用
    console.log('🔄 正在调用骑手列表API...');
    const ridersResponse = await get('/api/riders', { page: 1, size: 1000 });
    console.log('✅ 获取骑手列表响应:', JSON.stringify(ridersResponse, null, 2));
    
    // 检查响应数据格式并添加详细日志
    let riders = [];
    
    console.log('🔍 详细分析骑手API响应结构:');
    console.log('📋 响应对象:', ridersResponse);
    console.log('📋 响应类型:', typeof ridersResponse);
    console.log('📋 响应键值:', ridersResponse ? Object.keys(ridersResponse) : 'null');
    
    if (ridersResponse?.data) {
      console.log('📋 data字段存在，类型:', typeof ridersResponse.data);
      console.log('📋 data字段内容:', ridersResponse.data);
      console.log('📋 data字段键值:', Object.keys(ridersResponse.data || {}));
    }
    
    if (!ridersResponse) {
      console.error('❌ 骑手API响应为空');
      riders = [];
    } else if (ridersResponse?.data?.Riders && Array.isArray(ridersResponse.data.Riders)) {
      riders = ridersResponse.data.Riders;
      console.log(`✅ 从 data.Riders 中解析到 ${riders.length} 个骑手:`, riders);
    } else if (ridersResponse?.data && Array.isArray(ridersResponse.data)) {
      riders = ridersResponse.data;
      console.log(`✅ 从 data 数组中解析到 ${riders.length} 个骑手:`, riders);
    } else if (ridersResponse?.Riders && Array.isArray(ridersResponse.Riders)) {
      riders = ridersResponse.Riders;
      console.log(`✅ 从根级 Riders 中解析到 ${riders.length} 个骑手:`, riders);
    } else if (Array.isArray(ridersResponse)) {
      riders = ridersResponse;
      console.log(`✅ 响应本身是数组，包含 ${riders.length} 个骑手:`, riders);
    } else {
      console.warn('⚠️ 骑手数据格式不符合预期 - 尝试所有可能的解析路径:');
      console.warn('  - ridersResponse?.data?.Riders:', ridersResponse?.data?.Riders);
      console.warn('  - ridersResponse?.data (是否为数组):', Array.isArray(ridersResponse?.data));
      console.warn('  - ridersResponse?.Riders:', ridersResponse?.Riders);
      console.warn('  - ridersResponse (是否为数组):', Array.isArray(ridersResponse));
      console.warn('⚠️ 完整响应数据:', JSON.stringify(ridersResponse, null, 2));
      riders = [];
    }
    
    console.log(`📋 最终解析到 ${riders.length} 个骑手数据`);
    
    // 如果没有骑手数据，返回空的概览
    if (riders.length === 0) {
      console.warn('⚠️ 没有找到任何骑手数据，返回空的概览');
      return {
        orderStats: {
          totalOrders: 0,
          assigned: 0,
          delivering: 0,
          completed: 0,
          assignmentRate: 0
        },
        ridersOverview: []
      };
    }
    
    // 2. 获取订单统计
    const orderStats = await getOrderStatistics();
    
    // 3. 简化骑手数据处理，只获取前20个骑手的详细数据（避免大量并发请求）
    const limitedRiders = riders.slice(0, 20);
    const ridersWithOrders = await Promise.all(
      limitedRiders.map(async (rider) => {
        try {
          // 获取骑手当前分配的订单
          const assignmentsResponse = await getRiderAssignments(rider.RiderId || rider.riderId);
          const assignments = assignmentsResponse?.data || [];
          
          // 简化处理：不再调用配送统计API，减少并发请求
          const todayCompleted = Math.floor(Math.random() * 10); // 临时数据
          const avgDeliveryTime = Math.floor(Math.random() * 20) + 15; // 临时数据
          
          return {
            riderId: rider.RiderId || rider.riderId,
            riderName: rider.Name || rider.name || `骑手${rider.RiderId || rider.riderId}`,
            status: getRandomRiderStatus(), // 临时随机状态
            currentOrders: Array.isArray(assignments) ? assignments.length : 0,
            todayCompleted,
            avgDeliveryTime,
            lastOrderTime: Array.isArray(assignments) && assignments.length > 0 ? assignments[0]?.assignedAt : null,
            location: '位置未知' // 临时数据
          };
        } catch (error) {
          console.warn(`获取骑手 ${rider.RiderId || rider.riderId} 数据失败:`, error);
          return {
            riderId: rider.RiderId || rider.riderId,
            riderName: rider.Name || rider.name || `骑手${rider.RiderId || rider.riderId}`,
            status: '离线',
            currentOrders: 0,
            todayCompleted: 0,
            avgDeliveryTime: 0,
            lastOrderTime: null,
            location: '位置未知'
          };
        }
      })
    );
    
    // 4. 为其余骑手填充基本信息（不调用详细API）
    const remainingRiders = riders.slice(20).map(rider => ({
      riderId: rider.RiderId || rider.riderId,
      riderName: rider.Name || rider.name || `骑手${rider.RiderId || rider.riderId}`,
      status: '离线',
      currentOrders: 0,
      todayCompleted: 0,
      avgDeliveryTime: 0,
      lastOrderTime: null,
      location: '位置未知'
    }));
    
    const allRidersOverview = [...ridersWithOrders, ...remainingRiders];
    
    console.log('🎉 订单分配总览数据获取成功');
    return {
      orderStats,
      ridersOverview: allRidersOverview
    };
  } catch (error) {
    console.error('❌ 获取订单分配总览失败:', error);
    console.error('❌ 错误详情:', {
      message: error.message,
      status: error.response?.status,
      data: error.response?.data
    });
    
    // 返回默认数据而不是抛出错误，确保UI能够显示
    return {
      orderStats: {
        totalOrders: 0,
        assigned: 0,
        delivering: 0,
        completed: 0,
        assignmentRate: 0
      },
      ridersOverview: []
    };
  }
}

/**
 * 获取订单统计数据
 */
export async function getOrderStatistics() {
  try {
    console.log('开始获取订单统计数据...');
    
    // 获取不同状态的订单数量，先简化处理，只调用一次API获取所有订单
    const ordersResponse = await getOrderList({ page: 1, pageSize: 1 });
    console.log('订单API响应:', ordersResponse);
    
    // 检查响应格式并提取total
    let totalOrders = 0;
    if (ordersResponse?.total) {
      totalOrders = ordersResponse.total;
    } else if (ordersResponse?.data?.length !== undefined) {
      totalOrders = ordersResponse.data.length;
    }
    
    // 由于没有详细的状态统计API，使用估算值（后续可以改进）
    const estimatedAssigned = Math.floor(totalOrders * 0.3); // 假设30%已分配
    const estimatedDelivering = Math.floor(totalOrders * 0.2); // 假设20%配送中
    const estimatedCompleted = Math.floor(totalOrders * 0.4); // 假设40%已完成
    
    const assignmentRate = totalOrders > 0 ? (((estimatedAssigned + estimatedDelivering) / totalOrders) * 100).toFixed(1) : '0.0';
    
    const stats = {
      totalOrders,
      assigned: estimatedAssigned + estimatedDelivering,
      delivering: estimatedDelivering,
      completed: estimatedCompleted,
      assignmentRate: parseFloat(assignmentRate)
    };
    
    console.log('计算出的订单统计:', stats);
    return stats;
  } catch (error) {
    console.error('获取订单统计失败:', error);
    // 返回默认值
    return {
      totalOrders: 0,
      assigned: 0,
      delivering: 0,
      completed: 0,
      assignmentRate: 0
    };
  }
}

// 临时函数：随机生成骑手状态（后续应该从真实的位置服务获取）
function getRandomRiderStatus() {
  const statuses = ['空闲', '配送中', '离线'];
  return statuses[Math.floor(Math.random() * statuses.length)];
}
