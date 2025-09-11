// 地图相关类型定义

declare namespace MapTypes {
  /** 地图坐标点 */
  interface Point {
    /** 经度 */
    longitude: number;
    /** 纬度 */
    latitude: number;
  }

  /** 地图标记 */
  interface Marker {
    /** 标记ID */
    id: string;
    /** 位置 */
    position: Point;
    /** 标题 */
    title?: string;
    /** 内容 */
    content?: string;
    /** 图标 */
    icon?: string;
    /** 是否可拖拽 */
    draggable?: boolean;
    /** 是否可见 */
    visible?: boolean;
    /** 自定义数据 */
    data?: any;
  }

  /** 路径规划点 */
  interface RoutePoint extends Point {
    /** 点名称 */
    name?: string;
    /** 地址 */
    address?: string;
  }

  /** 路径规划结果 */
  interface RouteResult {
    /** 路径ID */
    id: string;
    /** 路径点 */
    points: RoutePoint[];
    /** 总距离（米） */
    distance: number;
    /** 总时间（秒） */
    duration: number;
    /** 路径策略 */
    policy: string;
    /** 路径描述 */
    description?: string;
  }

  /** 骑手位置信息 */
  interface RiderLocation {
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
    /** 定位类型 */
    locationType?: string;
    /** 状态(1:在线, 0:离线) */
    status: number;
  }

  /** 订单位置信息 */
  interface OrderLocation {
    /** 订单ID */
    orderId: string;
    /** 经度 */
    longitude: number;
    /** 纬度 */
    latitude: number;
    /** 标题 */
    title: string;
    /** 地址 */
    address: string;
    /** 状态 */
    status: string;
    /** 订单金额 */
    amount?: number;
    /** 创建时间 */
    createTime?: string;
  }

  /** 地图配置 */
  interface MapConfig {
    /** 地图高度 */
    height?: number;
    /** 初始中心点 */
    center?: Point;
    /** 初始缩放级别 */
    zoom?: number;
    /** 地图样式 */
    style?: string;
    /** 是否显示控件 */
    showControls?: boolean;
    /** 是否启用定位 */
    enableLocation?: boolean;
  }

  /** 地图事件 */
  interface MapEvents {
    /** 地图初始化完成 */
    onMapReady?: (map: any) => void;
    /** 地图点击事件 */
    onMapClick?: (point: Point) => void;
    /** 标记点击事件 */
    onMarkerClick?: (marker: Marker) => void;
    /** 位置更新事件 */
    onLocationUpdate?: (location: RiderLocation) => void;
    /** 订单点击事件 */
    onOrderClick?: (order: OrderLocation) => void;
  }
}

// 高德地图API类型声明
declare global {
  interface Window {
    AMap: any;
  }
}

export {};
