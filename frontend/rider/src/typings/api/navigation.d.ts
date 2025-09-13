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
      /** 步骤ID */
      stepId: string;
      /** 步骤描述 */
      instruction: string;
      /** 距离（米） */
      distance: number;
      /** 预计时间（秒） */
      duration: number;
      /** 起点坐标 */
      startLocation: Coordinate;
      /** 终点坐标 */
      endLocation: Coordinate;
      /** 道路名称 */
      roadName?: string;
    }

    /** 导航路线 */
    export interface NavigationRoute {
      /** 路线ID */
      routeId: string;
      /** 总距离（米） */
      totalDistance: number;
      /** 总预计时间（秒） */
      totalDuration: number;
      /** 路线步骤 */
      steps: RouteStep[];
      /** 起点 */
      startPoint: Coordinate;
      /** 终点 */
      endPoint: Coordinate;
      /** 路线类型 */
      routeType: 'driving' | 'walking' | 'cycling';
    }

    /** 实时导航更新 */
    export interface RealTimeNavigationUpdate {
      /** 更新ID */
      updateId: string;
      /** 当前步骤索引 */
      currentStepIndex: number;
      /** 当前位置 */
      currentLocation: Coordinate;
      /** 剩余距离（米） */
      remainingDistance: number;
      /** 剩余时间（秒） */
      remainingTime: number;
      /** 下一个转弯点 */
      nextTurn?: {
        instruction: string;
        distance: number;
        location: Coordinate;
      };
      /** 交通状况 */
      trafficStatus?: 'smooth' | 'slow' | 'congested';
    }

    /** 服务点信息 */
    export interface ServicePoint {
      /** 服务点ID */
      pointId: string;
      /** 服务点名称 */
      name: string;
      /** 服务点类型 */
      type: 'gas_station' | 'restaurant' | 'hospital' | 'police' | 'other';
      /** 位置坐标 */
      location: Coordinate;
      /** 地址 */
      address: string;
      /** 距离（米） */
      distance: number;
      /** 营业状态 */
      isOpen: boolean;
      /** 联系电话 */
      phone?: string;
      /** 评分 */
      rating?: number;
    }

    // ========== 请求类型 ==========

    /** 获取订单导航路线请求 */
    export interface GetOrderNavigationRequest {
      /** 订单ID */
      orderId: string;
      /** 起点坐标（可选，默认使用骑手当前位置） */
      startLocation?: Coordinate;
      /** 路线类型 */
      routeType?: 'driving' | 'walking' | 'cycling';
    }

    /** 获取订单实时导航更新请求 */
    export interface GetRealTimeNavigationRequest {
      /** 订单ID */
      orderId: string;
      /** 当前位置 */
      currentLocation: Coordinate;
      /** 路线ID */
      routeId: string;
    }

    /** 基础导航路线规划请求 */
    export interface PlanBasicRouteRequest {
      /** 起点坐标 */
      startLocation: Coordinate;
      /** 终点坐标 */
      endLocation: Coordinate;
      /** 路线类型 */
      routeType: 'driving' | 'walking' | 'cycling';
      /** 途经点（可选） */
      waypoints?: Coordinate[];
      /** 避开区域（可选） */
      avoidAreas?: Array<{
        center: Coordinate;
        radius: number;
      }>;
    }

    /** 实时导航路径请求 */
    export interface PlanRealTimeRouteRequest {
      /** 起点坐标 */
      startLocation: Coordinate;
      /** 终点坐标 */
      endLocation: Coordinate;
      /** 当前位置 */
      currentLocation: Coordinate;
      /** 路线类型 */
      routeType: 'driving' | 'walking' | 'cycling';
      /** 实时交通数据 */
      trafficData?: {
        congestionLevel: 'low' | 'medium' | 'high';
        roadClosures?: Array<{
          roadName: string;
          startLocation: Coordinate;
          endLocation: Coordinate;
        }>;
      };
    }

    /** 获取附近服务点请求 */
    export interface GetNearbyServicePointsRequest {
      /** 中心点坐标 */
      centerLocation: Coordinate;
      /** 搜索半径（米） */
      radius: number;
      /** 服务点类型 */
      types?: Array<'gas_station' | 'restaurant' | 'hospital' | 'police' | 'other'>;
      /** 最大返回数量 */
      limit?: number;
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
    export type NearbyServicePointsResponse = ApiResponse<{
      points: ServicePoint[];
      total: number;
    }>;
  }
}
