import { request } from '../request';

/**
 * 订单分配管理API
 */

// ========== 订单分配 ==========

/**
 * 自动分配订单
 */
export function autoAssignOrder(data: Api.OrderAssignment.AutoAssignRequest) {
  return request<Api.OrderAssignment.AutoAssignResponse>({
    url: '/api/orderassignment/auto-assign',
    method: 'post',
    data
  });
}

/**
 * 手动分配订单
 */
export function manualAssignOrder(data: Api.OrderAssignment.ManualAssignRequest) {
  return request<Api.OrderAssignment.ManualAssignResponse>({
    url: '/api/orderassignment/manual-assign',
    method: 'post',
    data
  });
}

/**
 * 骑手接受订单
 */
export function acceptOrder(riderId: string, data: Api.OrderAssignment.AcceptOrderRequest) {
  return request<Api.OrderAssignment.AcceptOrderResponse>({
    url: `/api/orderassignment/rider/${riderId}/accept`,
    method: 'post',
    data
  });
}

/**
 * 骑手拒绝订单
 */
export function rejectOrder(riderId: string, data: Api.OrderAssignment.RejectOrderRequest) {
  return request<Api.OrderAssignment.RejectOrderResponse>({
    url: `/api/orderassignment/rider/${riderId}/reject`,
    method: 'post',
    data
  });
}

/**
 * 更新订单状态
 */
export function updateOrderStatus(assignmentId: string, data: Api.OrderAssignment.UpdateOrderStatusRequest) {
  return request<Api.OrderAssignment.UpdateOrderStatusResponse>({
    url: `/api/orderassignment/${assignmentId}/status`,
    method: 'put',
    data
  });
}

// ========== 订单分配查询 ==========

/**
 * 获取订单分配信息
 */
export function getOrderAssignmentInfo(assignmentId: string) {
  return request<Api.OrderAssignment.OrderAssignmentInfoResponse>({
    url: `/api/orderassignment/${assignmentId}`,
    method: 'get'
  });
}

/**
 * 获取骑手分配列表
 */
export function getRiderAssignmentList(riderId: string, params: Api.OrderAssignment.GetRiderAssignmentListRequest = {}) {
  return request<Api.OrderAssignment.RiderAssignmentListResponse>({
    url: `/api/orderassignment/rider/${riderId}/list`,
    method: 'get',
    params
  });
}
