import { request } from '../request';

// ========== 骑手基础信息 ==========

/**
 * 获取骑手信息
 *
 * @param riderId 骑手ID
 * @returns Promise<骑手信息数据>
 */
export function getRiderInfo(riderId: string) {
  return request<Api.Rider.RiderInfoData>({
    url: `/api/Riders/${riderId}`,
    method: 'get'
  });
}

/**
 * 获取骑手订单列表
 *
 * @param riderId 骑手ID
 * @param params 查询参数
 * @param params.status 接单状态（0:未处理, 1:已接单, 2:已拒单, 3:已完成）
 * @returns Promise<订单分配数据列表>
 */
export function getRiderOrderList(riderId: string, params: Api.Rider.GetRiderOrderListRequest = {}) {
  return request<Api.Rider.OrderAssignmentData[]>({
    url: `/api/Riders/${riderId}/assignments`,
    method: 'get',
    params
  });
}

/**
 * 更新骑手信息
 *
 * @param riderId 骑手ID
 * @param data 更新数据
 * @returns Promise<更新后的骑手信息数据>
 */
export function updateRiderInfo(riderId: string, data: Api.Rider.UpdateInfoRequest) {
  return request<Api.Rider.UpdateInfoData>({
    url: `/api/Riders/${riderId}`,
    method: 'patch',
    data
  });
}

/**
 * 骑手注册
 *
 * @param data 注册数据
 * @returns Promise<注册响应数据>
 */
export function registerRider(data: Api.Rider.RegisterRequest) {
  return request<Api.Rider.RegisterData>({
    url: '/api/Riders',
    method: 'post',
    data
  });
}

// ========== 订单管理 ==========

/**
 * 更新订单分配状态
 *
 * @param riderId 骑手ID
 * @param assignId 分配ID
 * @param data 更新数据
 * @returns Promise<更新后的分配数据>
 */
export function updateAssignStatus(riderId: string, assignId: string, data: Api.Rider.UpdateAssignStatusRequest) {
  return request<Api.Rider.UpdateAssignData>({
    url: `/api/riders/${riderId}/assignments/${assignId}`,
    method: 'patch',
    data
  });
}

/**
 * 获取骑手列表
 *
 * @param params 查询参数
 * @returns Promise<骑手列表数据>
 */
export function getRiderList(params: Api.Rider.GetRiderListRequest = {}) {
  return request<Api.Rider.RiderListResponse>({
    url: '/api/Riders',
    method: 'get',
    params
  });
}
