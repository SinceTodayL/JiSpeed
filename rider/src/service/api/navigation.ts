import { request } from '../request';

/**
 * 导航管理API
 */

// ========== 订单导航 ==========

/**
 * 获取订单导航路线
 */
export function getOrderNavigationRoute(params: Api.Navigation.GetOrderNavigationRequest) {
  return request<Api.Navigation.OrderNavigationResponse>({
    url: `/api/Navigation/order/${params.orderId}/rider/${params.riderId}/route`,
    method: 'get',
    params
  });
}

/**
 * 获取订单实时导航更新
 */
export function getOrderRealTimeNavigation(params: Api.Navigation.GetRealTimeNavigationRequest) {
  return request<Api.Navigation.RealTimeNavigationResponse>({
    url: `/api/navigation/order/${params.orderId}/realtime`,
    method: 'get',
    params
  });
}

// ========== 路线规划 ==========

/**
 * 基础导航路线规划
 */
export function planBasicRoute(data: Api.Navigation.PlanBasicRouteRequest) {
  return request<Api.Navigation.BasicRouteResponse>({
    url: '/api/navigation/plan/basic',
    method: 'post',
    data
  });
}

/**
 * 实时导航路径
 */
export function planRealTimeRoute(data: Api.Navigation.PlanRealTimeRouteRequest) {
  return request<Api.Navigation.RealTimeRouteResponse>({
    url: '/api/navigation/plan/realtime',
    method: 'post',
    data
  });
}

// ========== 服务点查询 ==========

/**
 * 获取附近服务点
 */
export function getNearbyServicePoints(params: Api.Navigation.GetNearbyServicePointsRequest) {
  return request<Api.Navigation.NearbyServicePointsResponse>({
    url: '/api/navigation/service-points/nearby',
    method: 'get',
    params
  });
}
