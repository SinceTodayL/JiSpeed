import { request } from '../request';

/* ========== GET ========== */

/** 1. 根据id获取骑手信息 */
export function getRiderInfo(params: Api.Rider.Request) {
  return request<Api.Rider.InfoData>({
    url: `/api/riders/${params.riderId}`,
    method: 'get'
  });
}

/** 2. 根据id获取骑手订单列表 */
export function getRiderOrderList(riderId: string, params: Api.Rider.OrderListRequest) {
  return request<Api.Rider.Datum[]>({
    url: `/api/riders/${riderId}/assignments`,
    method: 'get',
    params
  });
}

/* ========== POST ========== */

/** 1. 骑手签到 */
export function riderCheckIn(riderId: string, data: Api.Rider.AttendanceRequest) {
  return request<Api.Rider.AttendanceResponse>({
    url: `/api/attendance/checkin/${riderId}`,
    method: 'post',
    data
  });
}

/** 2. 骑手签退 */
export function riderCheckOut(riderId: string, data: Api.Rider.AttendanceRequest) {
  return request<Api.Rider.AttendanceResponse>({
    url: `/api/attendance/checkout/${riderId}`,
    method: 'post',
    data
  });
}

/* ========== PATCH ========== */

/** 1. 更新骑手信息 */
export function updateRiderInfo(riderId: string, data: Api.Rider.UpdateInfoRequest) {
  return request<Api.Rider.UpdateInfoData>({
    url: `/api/riders/${riderId}`,
    method: 'patch',
    data
  });
}

/** 2. 更新订单分配状态 */
export function updateAssignStatus(riderId: string, assignId: string, data: Api.Rider.UpdateAssignStatusRequest) {
  return request<Api.Rider.UpdateAssignData>({
    url: `/api/riders/${riderId}/assignments/${assignId}`,
    method: 'patch',
    data
  });
}




