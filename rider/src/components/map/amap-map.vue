<template>
  <div class="amap-container">
    <!-- 地图容器 -->
    <div ref="mapContainer" class="map-container" :style="{ height: height + 'px' }"></div>
    
    <!-- 地图控制面板 -->
    <div v-if="showControls" class="map-controls">
      <!-- 定位按钮 -->
      <NButton
        v-if="enableLocation"
        type="primary"
        size="small"
        circle
        @click="locateUser"
        :loading="locating"
        class="control-btn"
      >
        <template #icon>
          <Icon icon="mdi:crosshairs-gps" />
        </template>
      </NButton>
      
      <!-- 缩放控制 -->
      <div class="zoom-controls">
        <NButton
          size="small"
          circle
          @click="zoomIn"
          class="control-btn"
        >
          <template #icon>
            <Icon icon="mdi:plus" />
          </template>
        </NButton>
        <NButton
          size="small"
          circle
          @click="zoomOut"
          class="control-btn"
        >
          <template #icon>
            <Icon icon="mdi:minus" />
          </template>
        </NButton>
      </div>
    </div>
    
    <!-- 加载状态 -->
    <div v-if="loading" class="map-loading">
      <NSpin size="large" />
      <div class="loading-text">地图加载中...</div>
    </div>
    
    <!-- 错误状态 -->
    <div v-if="error" class="map-error">
      <Icon icon="mdi:alert-circle" class="error-icon" />
      <div class="error-text">{{ error }}</div>
      <NButton size="small" @click="initMap">重试</NButton>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch, nextTick } from 'vue';
import { NButton, NSpin } from 'naive-ui';
import { Icon } from '@iconify/vue';
import { AMAP_CONFIG } from '@/config/amap';

// 定义组件属性
interface Props {
  /** 地图高度 */
  height?: number;
  /** 初始中心点经度 */
  longitude?: number;
  /** 初始中心点纬度 */
  latitude?: number;
  /** 初始缩放级别 */
  zoom?: number;
  /** 是否显示控制面板 */
  showControls?: boolean;
  /** 是否启用定位功能 */
  enableLocation?: boolean;
  /** 是否显示骑手位置 */
  showRiderLocation?: boolean;
  /** 是否显示订单位置 */
  showOrderLocations?: boolean;
  /** 骑手位置数据 */
  riderLocation?: {
    longitude: number;
    latitude: number;
    accuracy?: number;
    direction?: number;
  };
  /** 订单位置数据 */
  orderLocations?: Array<{
    id: string;
    longitude: number;
    latitude: number;
    title: string;
    address: string;
    status: string;
  }>;
  /** 是否显示路径规划 */
  showRoute?: boolean;
  /** 路径数据 */
  routeData?: Array<{
    longitude: number;
    latitude: number;
  }>;
  /** 是否启用导航功能 */
  enableNavigation?: boolean;
  /** 导航起点 */
  navigationStart?: {
    longitude: number;
    latitude: number;
  };
  /** 导航终点 */
  navigationEnd?: {
    longitude: number;
    latitude: number;
  };
}

const props = withDefaults(defineProps<Props>(), {
  height: 400,
  longitude: 116.397428,
  latitude: 39.90923,
  zoom: 13,
  showControls: true,
  enableLocation: true,
  showRiderLocation: true,
  showOrderLocations: true,
  showRoute: false,
  enableNavigation: false
});

// 定义事件
const emit = defineEmits<{
  /** 地图初始化完成 */
  mapReady: [map: any];
  /** 地图点击事件 */
  mapClick: [lng: number, lat: number];
  /** 骑手位置更新 */
  riderLocationUpdate: [location: { longitude: number; latitude: number }];
  /** 订单位置点击 */
  orderClick: [orderId: string];
  /** 导航开始 */
  navigationStart: [start: { longitude: number; latitude: number }, end: { longitude: number; latitude: number }];
  /** 路径规划完成 */
  routePlanned: [route: any];
}>();

// 响应式数据
const mapContainer = ref<HTMLElement>();
const loading = ref(true);
const error = ref('');
const locating = ref(false);
const map = ref<any>(null);
const riderMarker = ref<any>(null);
const orderMarkers = ref<any[]>([]);
const routePolyline = ref<any>(null);
const driving = ref<any>(null);
const navigationRoute = ref<any>(null);

// 使用统一的高德地图配置

// 初始化地图
const initMap = async () => {
  try {
    loading.value = true;
    error.value = '';
    
    // 检查高德地图API是否已加载
    if (!window.AMap) {
      await loadAMapScript();
    }
    
    // 创建地图实例
    map.value = new window.AMap.Map(mapContainer.value, {
      zoom: props.zoom,
      center: [props.longitude, props.latitude],
      mapStyle: AMAP_CONFIG.mapStyle,
      ...AMAP_CONFIG.mapOptions
    });
    
    // 添加地图控件
    map.value.addControl(new window.AMap.Scale());
    map.value.addControl(new window.AMap.ToolBar());
    
    // 初始化导航服务
    if (props.enableNavigation) {
      driving.value = new window.AMap.Driving({
        map: map.value,
        showTraffic: true,
        hideMarkers: false,
        autoFitView: true
      });
    }
    
    // 监听地图点击事件
    map.value.on('click', (e: any) => {
      emit('mapClick', e.lnglat.lng, e.lnglat.lat);
    });
    
    // 初始化骑手位置标记
    if (props.showRiderLocation && props.riderLocation) {
      addRiderMarker(props.riderLocation);
    }
    
    // 初始化订单位置标记
    if (props.showOrderLocations && props.orderLocations) {
      addOrderMarkers(props.orderLocations);
    }
    
    // 初始化路径规划
    if (props.showRoute && props.routeData) {
      addRoutePolyline(props.routeData);
    }
    
    loading.value = false;
    emit('mapReady', map.value);
  } catch (err: any) {
    error.value = err.message || '地图初始化失败';
    loading.value = false;
  }
};

// 加载高德地图API脚本
const loadAMapScript = (): Promise<void> => {
  return new Promise((resolve, reject) => {
    if (window.AMap) {
      resolve();
      return;
    }
    
    const script = document.createElement('script');
    script.src = `https://webapi.amap.com/maps?v=${AMAP_CONFIG.version}&key=${AMAP_CONFIG.key}&plugin=${AMAP_CONFIG.plugins.join(',')}`;
    script.onload = () => resolve();
    script.onerror = () => reject(new Error('高德地图API加载失败'));
    document.head.appendChild(script);
  });
};

// 添加骑手位置标记
const addRiderMarker = (location: { longitude: number; latitude: number; accuracy?: number; direction?: number }) => {
  if (!map.value) return;
  
  // 移除现有标记
  if (riderMarker.value) {
    map.value.remove(riderMarker.value);
  }
  
  // 创建骑手标记
  const marker = new window.AMap.Marker({
    position: [location.longitude, location.latitude],
    icon: new window.AMap.Icon({
      size: new window.AMap.Size(AMAP_CONFIG.markerIcons.rider.size[0], AMAP_CONFIG.markerIcons.rider.size[1]),
      image: AMAP_CONFIG.markerIcons.rider.image,
      imageSize: new window.AMap.Size(AMAP_CONFIG.markerIcons.rider.size[0], AMAP_CONFIG.markerIcons.rider.size[1])
    }),
    title: '骑手位置'
  });
  
  // 添加精度圆圈
  if (location.accuracy) {
    const circle = new window.AMap.Circle({
      center: [location.longitude, location.latitude],
      radius: location.accuracy,
      ...AMAP_CONFIG.accuracyCircleStyles
    });
    map.value.add(circle);
  }
  
  map.value.add(marker);
  riderMarker.value = marker;
  
  // 设置地图中心
  map.value.setCenter([location.longitude, location.latitude]);
};

// 添加订单位置标记
const addOrderMarkers = (orders: Array<{ id: string; longitude: number; latitude: number; title: string; address: string; status: string }>) => {
  if (!map.value) return;
  
  // 移除现有标记
  orderMarkers.value.forEach(marker => map.value.remove(marker));
  orderMarkers.value = [];
  
  // 创建订单标记
  orders.forEach(order => {
    const marker = new window.AMap.Marker({
      position: [order.longitude, order.latitude],
      icon: new window.AMap.Icon({
        size: new window.AMap.Size(AMAP_CONFIG.markerIcons.order.size[0], AMAP_CONFIG.markerIcons.order.size[1]),
        image: AMAP_CONFIG.markerIcons.order.image,
        imageSize: new window.AMap.Size(AMAP_CONFIG.markerIcons.order.size[0], AMAP_CONFIG.markerIcons.order.size[1])
      }),
      title: order.title
    });
    
    // 添加点击事件
    marker.on('click', () => {
      emit('orderClick', order.id);
    });
    
    map.value.add(marker);
    orderMarkers.value.push(marker);
  });
};

// 添加路径规划
const addRoutePolyline = (routeData: Array<{ longitude: number; latitude: number }>) => {
  if (!map.value || routeData.length < 2) return;
  
  // 移除现有路径
  if (routePolyline.value) {
    map.value.remove(routePolyline.value);
  }
  
  // 创建路径
  const polyline = new window.AMap.Polyline({
    path: routeData.map(point => [point.longitude, point.latitude]),
    ...AMAP_CONFIG.polylineStyles.default
  });
  
  map.value.add(polyline);
  routePolyline.value = polyline;
  
  // 调整地图视野以包含整个路径
  map.value.setFitView([polyline]);
};

// 定位用户位置
const locateUser = () => {
  if (!map.value) return;
  
  locating.value = true;
  
  // 使用高德地图定位服务
  const geolocation = new window.AMap.Geolocation(AMAP_CONFIG.geolocationOptions);
  
  geolocation.getCurrentPosition((status: string, result: any) => {
    locating.value = false;
    
    if (status === 'complete') {
      const location = {
        longitude: result.position.lng,
        latitude: result.position.lat,
        accuracy: result.accuracy
      };
      
      // 更新骑手位置
      addRiderMarker(location);
      emit('riderLocationUpdate', location);
    } else {
      error.value = '定位失败：' + result.message;
    }
  });
};

// 地图缩放
const zoomIn = () => {
  if (map.value) {
    map.value.zoomIn();
  }
};

const zoomOut = () => {
  if (map.value) {
    map.value.zoomOut();
  }
};

// 监听属性变化
watch(() => props.riderLocation, (newLocation) => {
  if (newLocation && props.showRiderLocation) {
    addRiderMarker(newLocation);
  }
}, { deep: true });

watch(() => props.orderLocations, (newOrders) => {
  if (newOrders && props.showOrderLocations) {
    addOrderMarkers(newOrders);
  }
}, { deep: true });

watch(() => props.routeData, (newRoute) => {
  if (newRoute && props.showRoute) {
    addRoutePolyline(newRoute);
  }
}, { deep: true });

// 组件挂载
onMounted(() => {
  nextTick(() => {
    initMap();
  });
});

// 组件卸载
onUnmounted(() => {
  if (map.value) {
    map.value.destroy();
  }
});

// 导航功能
const startNavigation = (start: { longitude: number; latitude: number }, end: { longitude: number; latitude: number }) => {
  if (!driving.value) return;
  
  driving.value.search(
    [start.longitude, start.latitude],
    [end.longitude, end.latitude],
    (status: string, result: any) => {
      if (status === 'complete') {
        navigationRoute.value = result;
        emit('routePlanned', result);
        console.log('导航路径规划完成:', result);
      } else {
        console.error('导航路径规划失败:', result);
      }
    }
  );
};

// 清除导航路径
const clearNavigation = () => {
  if (driving.value) {
    driving.value.clear();
    navigationRoute.value = null;
  }
};

// 计算距离
const calculateDistance = (start: { longitude: number; latitude: number }, end: { longitude: number; latitude: number }) => {
  if (!window.AMap) return 0;
  
  const startPoint = new window.AMap.LngLat(start.longitude, start.latitude);
  const endPoint = new window.AMap.LngLat(end.longitude, end.latitude);
  
  return startPoint.distance(endPoint);
};

// 监听导航属性变化
watch(() => [props.navigationStart, props.navigationEnd], ([start, end]) => {
  if (start && end && props.enableNavigation) {
    startNavigation(start, end);
  }
}, { deep: true });

// 暴露方法给父组件
defineExpose({
  map: map,
  locateUser,
  addRiderMarker,
  addOrderMarkers,
  addRoutePolyline,
  startNavigation,
  clearNavigation,
  calculateDistance
});
</script>

<style scoped>
.amap-container {
  position: relative;
  width: 100%;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.map-container {
  width: 100%;
  background-color: #f5f5f5;
}

.map-controls {
  position: absolute;
  top: 16px;
  right: 16px;
  display: flex;
  flex-direction: column;
  gap: 8px;
  z-index: 1000;
}

.control-btn {
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
}

.zoom-controls {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.map-loading {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 16px;
  z-index: 1000;
}

.loading-text {
  color: #666;
  font-size: 14px;
}

.map-error {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 16px;
  z-index: 1000;
  background: rgba(255, 255, 255, 0.9);
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.error-icon {
  font-size: 32px;
  color: #ff4d4f;
}

.error-text {
  color: #ff4d4f;
  font-size: 14px;
  text-align: center;
}
</style>
