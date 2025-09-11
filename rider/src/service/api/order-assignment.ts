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

/**
 * 更新分配状态（推荐使用此接口）
 */
export function updateAssignmentStatus(riderId: string, assignId: string, data: Api.OrderAssignment.UpdateAssignmentStatusRequest) {
  return request<Api.OrderAssignment.UpdateAssignmentStatusResponse>({
    url: `/api/Riders/${riderId}/assignments/${assignId}`,
    method: 'patch',
    data
  });
}

/**
 * 确认配送（完成订单）
 */
export function confirmDelivery(riderId: string, assignId: string) {
  return request<Api.OrderAssignment.UpdateAssignmentStatusResponse>({
    url: `/api/Riders/${riderId}/assignments/${assignId}`,
    method: 'patch',
    data: {
      AcceptedStatus: 3  // 3 = 确认送达
    }
  });
}

// ========== 订单分配查询 ==========

/**
 * 获取订单分配信息
 * 注意：此API暂时不可用，返回404错误
 */
export function getOrderAssignmentInfo(orderId: string) {
  // 临时返回空数据，避免404错误
  return Promise.resolve({
    data: null,
    code: 200,
    message: '订单分配信息功能暂未实现'
  });
  
  // 原始API调用（暂时注释）
  // return request<Api.OrderAssignment.OrderAssignmentInfoResponse>({
  //   url: `/api/OrderAssignment/order/${orderId}/assignment`,
  //   method: 'get'
  // });
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
