// 高德地图配置
export const AMAP_CONFIG = {
  // 高德地图API密钥 - 从环境变量获取
  key: import.meta.env.VITE_AMAP_API_KEY || 'your_amap_key_here',
  
  // API版本
  version: '2.0',
  
  // 需要加载的插件
  plugins: [
    'AMap.Scale',           // 比例尺
    'AMap.ToolBar',         // 工具栏
    'AMap.Geolocation',     // 定位服务
    'AMap.Driving',         // 驾车路径规划
    'AMap.Walking',         // 步行路径规划
    'AMap.Transfer',        // 公交路径规划
    'AMap.DistrictSearch',  // 行政区划查询
    'AMap.Geocoder',        // 地理编码
    'AMap.AutoComplete',    // 自动完成
    'AMap.PlaceSearch',     // 地点搜索
    'AMap.Weather'          // 天气查询
  ],
  
  // 默认地图样式
  mapStyle: 'amap://styles/normal',
  
  // 默认中心点（北京）
  defaultCenter: {
    longitude: 116.397428,
    latitude: 39.90923
  },
  
  // 默认缩放级别
  defaultZoom: 13,
  
  // 地图配置选项
  mapOptions: {
    // 是否显示路网
    showRoad: true,
    // 是否显示建筑物
    showBuilding: true,
    // 是否显示标注
    showLabel: true,
    // 是否显示交通
    showTraffic: false,
    // 是否显示热力图
    showHeatmap: false,
    // 是否显示卫星图
    showSatellite: false,
    // 是否显示实时路况
    showRealtimeTraffic: false
  },
  
  // 定位配置
  geolocationOptions: {
    // 是否启用高精度定位
    enableHighAccuracy: true,
    // 定位超时时间（毫秒）
    timeout: 10000,
    // 定位缓存时间（毫秒）
    maximumAge: 0,
    // 是否自动转换坐标
    convert: true,
    // 是否显示定位按钮
    showButton: false,
    // 定位按钮位置
    buttonPosition: 'LB',
    // 是否显示定位标记
    showMarker: false,
    // 是否显示精度圆圈
    showCircle: false,
    // 是否自动移动到定位位置
    panToLocation: true,
    // 是否缩放到精度范围
    zoomToAccuracy: true
  },
  
  // 路径规划配置
  routeOptions: {
    // 默认路径规划策略
    policy: 'LEAST_TIME', // 最快路径
    // 是否避开高速
    avoidHighways: false,
    // 是否避开收费
    avoidTolls: false,
    // 是否避开拥堵
    avoidCongestion: false
  },
  
  // 标记图标配置
  markerIcons: {
    // 骑手位置图标
    rider: {
      size: [32, 32],
      image: 'data:image/svg+xml;base64,' + btoa(`
        <svg width="32" height="32" viewBox="0 0 32 32" xmlns="http://www.w3.org/2000/svg">
          <circle cx="16" cy="16" r="12" fill="#1890ff" stroke="#fff" stroke-width="2"/>
          <circle cx="16" cy="16" r="4" fill="#fff"/>
          <path d="M16 4 L20 16 L16 28 L12 16 Z" fill="#1890ff" opacity="0.6"/>
        </svg>
      `)
    },
    // 订单位置图标
    order: {
      size: [24, 24],
      image: 'data:image/svg+xml;base64,' + btoa(`
        <svg width="24" height="24" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
          <circle cx="12" cy="12" r="10" fill="#52c41a" stroke="#fff" stroke-width="2"/>
          <text x="12" y="16" text-anchor="middle" fill="#fff" font-size="12" font-weight="bold">O</text>
        </svg>
      `)
    },
    // 商家位置图标
    merchant: {
      size: [24, 24],
      image: 'data:image/svg+xml;base64,' + btoa(`
        <svg width="24" height="24" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
          <circle cx="12" cy="12" r="10" fill="#fa8c16" stroke="#fff" stroke-width="2"/>
          <text x="12" y="16" text-anchor="middle" fill="#fff" font-size="12" font-weight="bold">M</text>
        </svg>
      `)
    },
    // 用户位置图标
    user: {
      size: [24, 24],
      image: 'data:image/svg+xml;base64,' + btoa(`
        <svg width="24" height="24" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
          <circle cx="12" cy="12" r="10" fill="#722ed1" stroke="#fff" stroke-width="2"/>
          <text x="12" y="16" text-anchor="middle" fill="#fff" font-size="12" font-weight="bold">U</text>
        </svg>
      `)
    }
  },
  
  // 路径样式配置
  polylineStyles: {
    // 默认路径样式
    default: {
      strokeColor: '#1890ff',
      strokeWeight: 4,
      strokeOpacity: 0.8,
      lineJoin: 'round',
      lineCap: 'round'
    },
    // 已完成的路径样式
    completed: {
      strokeColor: '#52c41a',
      strokeWeight: 4,
      strokeOpacity: 0.8,
      lineJoin: 'round',
      lineCap: 'round'
    },
    // 当前路径样式
    current: {
      strokeColor: '#fa8c16',
      strokeWeight: 4,
      strokeOpacity: 0.8,
      lineJoin: 'round',
      lineCap: 'round'
    }
  },
  
  // 精度圆圈样式
  accuracyCircleStyles: {
    fillColor: '#1890ff',
    fillOpacity: 0.1,
    strokeColor: '#1890ff',
    strokeOpacity: 0.3,
    strokeWeight: 1
  }
};

// 地图样式选项
export const MAP_STYLES = [
  { value: 'amap://styles/normal', label: '标准样式' },
  { value: 'amap://styles/dark', label: '暗色样式' },
  { value: 'amap://styles/light', label: '浅色样式' },
  { value: 'amap://styles/fresh', label: '清新样式' },
  { value: 'amap://styles/grey', label: '灰色样式' },
  { value: 'amap://styles/graffiti', label: '涂鸦样式' },
  { value: 'amap://styles/macaron', label: '马卡龙样式' },
  { value: 'amap://styles/blue', label: '蓝色样式' },
  { value: 'amap://styles/darkblue', label: '深蓝样式' },
  { value: 'amap://styles/wine', label: '酒红样式' }
];

// 路径规划策略选项
export const ROUTE_POLICIES = [
  { value: 'LEAST_TIME', label: '最快路径' },
  { value: 'LEAST_DISTANCE', label: '最短路径' },
  { value: 'LEAST_FEE', label: '最少收费' },
  { value: 'LEAST_TRANSFER', label: '最少换乘' },
  { value: 'LEAST_WALKING', label: '最少步行' },
  { value: 'MOST_COMFORTABLE', label: '最舒适' },
  { value: 'NO_HIGHWAY', label: '不走高速' },
  { value: 'NO_TOLL', label: '不走收费' }
];

// 地图类型选项
export const MAP_TYPES = [
  { value: 'normal', label: '标准地图' },
  { value: 'satellite', label: '卫星地图' },
  { value: 'roadnet', label: '路网地图' },
  { value: 'hybrid', label: '混合地图' }
];

// 导出默认配置
export default AMAP_CONFIG;
