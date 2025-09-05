declare namespace Api {
  namespace Navigation {
    // ========== 基础响应结构 ==========

    /** 通用API响应结构 */
    export interface ApiResponse<T = any> {
      /** 状态码 */
      code: number;
      /** 响应数据 */
      data: T;
      /** 响应消息 */
      message: string;
      /** 时间戳 */
      timestamp: number;
    }

    // ========== 导航数据 ==========

    /** 坐标点 */
    export interface Coordinate {
      /** 经度 */
      longitude: number;
      /** 纬度 */
      latitude: number;
    }

    /** 路线步骤 */
    export interface RouteStep {
      /** 步骤描述 */
      instruction?: string;
      /** 距离（米） */
      distance?: number;
      /** 预计时间（秒） */
      duration?: number;
      /** 道路名称 */
      roadName?: string;
      /** 转向类型 */
      turnType?: string;
      /** 路线坐标点 */
      polyline?: number[][];
    }

    /** 导航路线 */
    export interface NavigationRoute {
      /** 路线ID */
      routeId: string;
      /** 总距离（米） */
      totalDistance: number;
      /** 总预计时间（秒） */
      estimatedDuration: number;
      /** 路线坐标点 */
      polyline: string;
      /** 路线步骤 */
      steps: RouteStep[];
      /** 路线模式 */
      mode: string;
    }

    /** 实时导航更新 */
    export interface RealTimeNavigationUpdate {
      /** 路线ID */
      routeId: string;
      /** 当前经度 */
      currentLongitude: number;
      /** 当前纬度 */
      currentLatitude: number;
      /** 当前速度 */
      currentSpeed: number;
      /** 剩余时间（秒） */
      remainingTime: number;
      /** 剩余距离（米） */
      remainingDistance: number;
      /** 下一个导航指令 */
      nextInstruction: string;
      /** 时间戳 */
      timestamp: string;
    }

    /** 服务点信息 */
    export interface ServicePoint {
      /** 服务点名称 */
      name?: string;
      /** 地址 */
      address?: string;
      /** 经度 */
      longitude?: number;
      /** 纬度 */
      latitude?: number;
      /** 距离（米） */
      distance?: number;
      /** 服务点类型 */
      type?: string;
    }

    // ========== 请求类型 ==========

    /** 获取订单导航路线请求 */
    export interface GetOrderNavigationRequest {
      /** 订单ID */
      orderId: string;
      /** 骑手ID */
      riderId: string;
    }

    /** 获取订单实时导航更新请求 */
    export interface GetRealTimeNavigationRequest {
      /** 订单ID */
      orderId: string;
      /** 骑手ID */
      riderId: string;
    }

    /** 基础导航路线规划请求 */
    export interface PlanBasicRouteRequest {
      /** 起点经度 */
      startLongitude: number;
      /** 起点纬度 */
      startLatitude: number;
      /** 终点经度 */
      endLongitude: number;
      /** 终点纬度 */
      endLatitude: number;
      /** 导航模式 */
      mode: string;
    }

    /** 实时导航路径请求 */
    export interface PlanRealTimeRouteRequest {
      /** 起点经度 */
      startLongitude: number;
      /** 起点纬度 */
      startLatitude: number;
      /** 终点经度 */
      endLongitude: number;
      /** 终点纬度 */
      endLatitude: number;
      /** 导航模式 */
      mode: string;
    }

    /** 获取附近服务点请求 */
    export interface GetNearbyServicePointsRequest {
      /** 经度 */
      longitude: number;
      /** 纬度 */
      latitude: number;
      /** 服务类型，默认all */
      serviceType?: string;
      /** 搜索半径(米)，默认2000 */
      radius?: number;
    }

    // ========== 响应类型 ==========

    /** 订单导航路线响应 */
    export type OrderNavigationResponse = ApiResponse<NavigationRoute>;

    /** 实时导航更新响应 */
    export type RealTimeNavigationResponse = ApiResponse<RealTimeNavigationUpdate>;

    /** 基础路线规划响应 */
    export type BasicRouteResponse = ApiResponse<NavigationRoute>;

    /** 实时路线规划响应 */
    export type RealTimeRouteResponse = ApiResponse<NavigationRoute>;

    /** 附近服务点响应 */
    export type NearbyServicePointsResponse = ApiResponse<ServicePoint[]>;
  }
}
