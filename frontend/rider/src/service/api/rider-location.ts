import { request } from '../request';

/* ========== 骑手位置管理 API ========== */

/** 获取骑手最新位置 */
export function getRiderLatestLocation(riderId: string) {
  return request<Api.Rider.LocationLatestResponse>({
    url: `/api/RiderLocations/${riderId}/latest`,
    method: 'get'
  });
}

/** 获取骑手历史轨迹 */
export function getRiderLocationHistory(params: Api.Rider.LocationHistoryRequest) {
  return request<Api.Rider.LocationHistoryResponse>({
    url: `/api/Riderlocations/${params.riderId}/history`,
    method: 'get',
    params
  });
}

/** 获取指定区域内的骑手 */
export function getRidersInArea(params: Api.Rider.LocationAreaRequest) {
  return request<Api.Rider.LocationAreaResponse>({
    url: `/api/Riderlocations/area`,
    method: 'get',
    params
  });
}

/** 获取所有在线骑手位置 */
export function getOnlineRidersLocation(params: Api.Rider.LocationOnlineRequest) {
  return request<Api.Rider.LocationOnlineResponse>({
    url: `/api/Riderlocations/online`,
    method: 'get',
    params
  });
}

/** 计算骑手到指定点的距离 */
export function calculateRiderDistance(params: Api.Rider.LocationDistanceRequest) {
  return request<Api.Rider.LocationDistanceResponse>({
    url: `/api/Riderlocations/${params.riderId}/distance`,
    method: 'get',
    params
  });
}

/** 获取骑手当前位置的地址信息 */
export function getRiderCurrentAddress(riderId: string) {
  return request<Api.Rider.LocationAddressResponse>({
    url: `/api/Riderlocations/${riderId}/address`,
    method: 'get'
  });
}

/** 更新骑手位置 */
export function updateRiderLocation(data: Api.Rider.LocationUpdateRequest) {
  return request<Api.Rider.LocationUpdateResponse>({
    url: `/api/Riderlocations`,
    method: 'post',
    data
  });
}

/** 更新骑手在线状态 */
export function updateRiderOnlineStatus(riderId: string, data: Api.Rider.LocationStatusRequest) {
  return request<Api.Rider.LocationStatusResponse>({
    url: `/api/Riderlocations/${riderId}/status`,
    method: 'patch',
    data
  });
}

// ========== 新增位置相关接口 ==========

/**
 * 获取骑手当前的位置信息
 */
export function getRiderCurrentLocationInfo(riderId: string) {
  return request<Api.Rider.CurrentLocationInfoResponse>({
    url: `/api/Riderlocations/${riderId}/current-info`,
    method: 'get'
  });
}
