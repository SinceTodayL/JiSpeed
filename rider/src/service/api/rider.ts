import { request } from '../request';

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
export function getRiderOrderList(
  riderId: string,
  params: Api.Rider.GetRiderOrderListRequest = {}
) {
  return request<Api.Rider.OrderAssignmentData[]>({
    url: `/api/Riders/${riderId}/assignments`,
    method: 'get',
    params
  });
}

/* ========== 骑手签到 ========== */
export function riderCheckIn(riderId: string, data: Api.Rider.AttendanceRequest) {
  return request<Api.Rider.AttendanceResponse>({
    url: `/api/attendance/checkin/${riderId}`,
    method: 'post',
    data
  });
}

/* ========== 骑手签退 ========== */
export function riderCheckOut(riderId: string, data: Api.Rider.AttendanceRequest) {
  return request<Api.Rider.AttendanceResponse>({
    url: `/api/attendance/checkout/${riderId}`,
    method: 'post',
    data
  });
}


/**
 * 更新骑手信息
 *
 * @param riderId 骑手ID
 * @returns Promise<骑手信息数据>
 */
export function updateRiderInfo(riderId: string, data: Api.Rider.UpdateInfoRequest) {
  return request<Api.Rider.UpdateInfoData>({
    url: `/api/Riders/${riderId}`,
    method: 'patch',
    data
  });
}

/* ========== 更新订单分配状态 ========== */
export function updateAssignStatus(riderId: string, assignId: string, data: Api.Rider.UpdateAssignStatusRequest) {
  return request<Api.Rider.UpdateAssignData>({
    url: `/api/riders/${riderId}/assignments/${assignId}`,
    method: 'patch',
    data
  });
}
