import { request } from '../request';

/**
 * 订单分配管理API
 */

// ========== 订单分配 ==========

/**
 * 骑手接受订单
 */
export function acceptOrder(data: Api.OrderAssignment.AcceptOrderRequest) {
  return request<Api.OrderAssignment.AcceptOrderResponse>({
    url: '/api/OrderAssignment/accept-order',
    method: 'post',
    data
  });
}

/**
 * 骑手拒绝订单
 */
export function rejectOrder(data: Api.OrderAssignment.RejectOrderRequest) {
  return request<Api.OrderAssignment.RejectOrderResponse>({
    url: '/api/OrderAssignment/reject-order',
    method: 'post',
    data
  });
}

/**
 * 更新订单状态
 */
export function updateOrderStatus(data: Api.OrderAssignment.UpdateOrderStatusRequest) {
  return request<Api.OrderAssignment.UpdateOrderStatusResponse>({
    url: '/api/OrderAssignment/update-status',
    method: 'put',
    data
  });
}

// ========== 订单分配查询 ==========

/**
 * 获取订单分配信息
 */
export function getOrderAssignmentInfo(orderId: string) {
  return request<Api.OrderAssignment.OrderAssignmentInfoResponse>({
    url: `/api/OrderAssignment/order/${orderId}/assignment`,
    method: 'get'
  });
}

/**
 * 获取骑手分配列表
 */
export function getRiderAssignmentList(riderId: string, params: Api.OrderAssignment.GetRiderAssignmentListRequest = {}) {
  return request<Api.OrderAssignment.RiderAssignmentListResponse>({
    url: `/api/OrderAssignment/rider/${riderId}/assignments`,
    method: 'get',
    params
  });
}
