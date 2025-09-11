import { request } from '../request';

// API响应类型
interface ApiResponse<T = any> {
  code: string;
  message: string;
  data: T;
}

// 骑手位置相关API接口

/**
 * 骑手位置信息
 */
export interface RiderLocation {
  /** 位置ID */
  locationId?: string;
  /** 骑手ID */
  riderId?: string;
  /** 经度 */
  longitude: number;
  /** 纬度 */
  latitude: number;
  /** 定位时间 */
  locationTime: string;
  /** 定位精度(米) */
  accuracy?: number;
  /** 速度(km/h) */
  speed?: number;
  /** 方向(0-360度) */
  direction?: number;
  /** 定位类型(GPS、网络等) */
  locationType?: string;
  /** 状态(1:在线, 0:离线) */
  status: number;
}

/**
 * 更新骑手位置请求
 */
export interface UpdateRiderLocationRequest {
  /** 骑手ID */
  riderId: string;
  /** 经度 */
  longitude: number;
  /** 纬度 */
  latitude: number;
  /** 定位精度(米) */
  accuracy?: number;
  /** 速度(km/h) */
  speed?: number;
  /** 方向(0-360度) */
  direction?: number;
  /** 定位类型(GPS、网络等) */
  locationType?: string;
}

/**
 * 更新骑手状态请求
 */
export interface UpdateRiderStatusRequest {
  /** 状态(1:在线, 0:离线) */
  status: number;
}

/**
 * 距离信息
 */
export interface DistanceInfo {
  /** 骑手ID */
  riderId?: string;
  /** 目标经度 */
  targetLongitude: number;
  /** 目标纬度 */
  targetLatitude: number;
  /** 距离(米) */
  distance: number;
  /** 格式化的距离字符串 */
  formattedDistance?: string;
}

/**
 * 地址信息
 */
export interface AddressInfo {
  /** 骑手ID */
  riderId?: string;
  /** 地址信息 */
  address?: string;
}

/**
 * 地理编码请求
 */
export interface GeocodeRequest {
  /** 地址 */
  address: string;
}

/**
 * 地理编码响应
 */
export interface GeocodeResponse {
  /** 原始地址 */
  address?: string;
  /** 经度 */
  longitude: number;
  /** 纬度 */
  latitude: number;
  /** 格式化地址 */
  formattedAddress?: string;
  /** 省份 */
  province?: string;
  /** 城市 */
  city?: string;
  /** 区县 */
  district?: string;
}

/**
 * 更新骑手位置
 */
export function updateRiderLocation(data: UpdateRiderLocationRequest) {
  return request<ApiResponse<RiderLocation>>({
    url: '/api/RiderLocations',
    method: 'post',
    data
  });
}

/**
 * 获取骑手最新位置
 */
export function getRiderLatestLocation(riderId: string) {
  return request<ApiResponse<RiderLocation>>({
    url: `/api/RiderLocations/${riderId}/latest`,
    method: 'get'
  });
}

/**
 * 获取骑手历史轨迹
 */
export function getRiderLocationHistory(
  riderId: string,
  startTime: string,
  endTime: string
) {
  return request<ApiResponse<RiderLocation[]>>({
    url: `/api/RiderLocations/${riderId}/history`,
    method: 'get',
    params: { startTime, endTime }
  });
}

/**
 * 获取指定区域内的骑手
 */
export function getRidersInArea(
  minLongitude: number,
  maxLongitude: number,
  minLatitude: number,
  maxLatitude: number
) {
  return request<ApiResponse<RiderLocation[]>>({
    url: '/api/RiderLocations/area',
    method: 'get',
    params: { minLongitude, maxLongitude, minLatitude, maxLatitude }
  });
}

/**
 * 获取在线骑手位置
 */
export function getOnlineRiderLocations() {
  return request<ApiResponse<RiderLocation[]>>({
    url: '/api/RiderLocations/online',
    method: 'get'
  });
}

/**
 * 更新骑手在线状态
 */
export function updateRiderStatus(riderId: string, data: UpdateRiderStatusRequest) {
  return request<ApiResponse<object>>({
    url: `/api/RiderLocations/${riderId}/status`,
    method: 'patch',
    data
  });
}

/**
 * 计算骑手到指定位置的距离
 */
export function calculateDistanceToLocation(
  riderId: string,
  longitude: number,
  latitude: number
) {
  return request<ApiResponse<DistanceInfo>>({
    url: `/api/RiderLocations/${riderId}/distance`,
    method: 'get',
    params: { longitude, latitude }
  });
}

/**
 * 获取骑手当前位置的地址信息
 */
export function getRiderLocationAddress(riderId: string) {
  return request<ApiResponse<AddressInfo>>({
    url: `/api/RiderLocations/${riderId}/address`,
    method: 'get'
  });
}

/**
 * 地理编码：根据地址获取经纬度
 */
export function geocodeAddress(data: GeocodeRequest) {
  return request<ApiResponse<GeocodeResponse>>({
    url: '/api/RiderLocations/geocode',
    method: 'post',
    data
  });
}

/**
 * 测试位置推送功能
 */
export function testLocationPush(riderId: string) {
  return request<ApiResponse<object>>({
    url: `/api/RiderLocations/test-push/${riderId}`,
    method: 'post'
  });
}

/**
 * 调试骑手位置信息
 */
export function debugRiderLocation(riderId: string) {
  return request<ApiResponse<object>>({
    url: `/api/RiderLocations/debug/${riderId}`,
    method: 'get'
  });
}
