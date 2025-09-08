<script setup lang="ts">
import { nextTick, onMounted, onUnmounted, ref, watch } from 'vue';
import { NButton } from 'naive-ui';
import { AMapLoader } from '@/config/amap';
import { getOnlineRidersLocation } from '@/service/api/rider-location';

// Props
interface Props {
  center?: { longitude: number; latitude: number };
  zoom?: number;
  showRiders?: boolean;
  navigationRoute?: Api.Navigation.NavigationRoute | null;
  isNavigating?: boolean;
  currentOrder?: any;
}

const props = withDefaults(defineProps<Props>(), {
  center: () => {
    // 尝试从localStorage获取上次保存的位置，如果没有则使用更通用的默认位置
    const savedLocation = localStorage.getItem('lastKnownLocation');
    if (savedLocation) {
      try {
        return JSON.parse(savedLocation);
      } catch {
        // 解析失败时使用默认位置
      }
    }
    // 使用更通用的默认位置（中国中心附近）
    return { longitude: 104.195397, latitude: 35.86166 };
  },
  zoom: 12,
  showRiders: true,
  navigationRoute: null,
  isNavigating: false,
  currentOrder: null
});

// Emits
const emit = defineEmits<{
  'map-ready': [map: any];
  'location-update': [location: { longitude: number; latitude: number }];
  'rider-click': [rider: any];
  'navigation-start': [];
  'navigation-end': [];
  'navigation-update': [update: Api.Navigation.RealTimeNavigationUpdate];
}>();

// 响应式数据
const mapContainer = ref<HTMLElement>();
const map = ref<any>(null);
const loading = ref(false);
const locating = ref(false);
const mapStatus = ref<'loading' | 'ready' | 'error'>('loading');
const isDestroyed = ref(false);

// 地图数据
const currentLocation = ref(props.center);
const riders = ref<any[]>([]);
const markers = ref<any[]>([]);
const infoWindow = ref<any>(null);
const geolocationControl = ref<any>(null);

// 导航相关
const routePolyline = ref<any>(null);
const startMarker = ref<any>(null);
const endMarker = ref<any>(null);
const navigationMarkers = ref<any[]>([]);
const currentStepIndex = ref<number>(0);
const navigationTimer = ref<NodeJS.Timeout | null>(null);
const isNavigationActive = ref<boolean>(false);

// 标记状态管理
const ridersMarkersLoaded = ref<boolean>(false);

// 监听center变化，更新地图中心
watch(() => props.center, (newCenter) => {
  if (map.value && newCenter) {
    map.value.setCenter([newCenter.longitude, newCenter.latitude]);
    currentLocation.value = newCenter;
  }
}, { immediate: true });

// 创建骑手图标
const createRiderIcon = () => {
  // 创建一个简单的圆形图标，不依赖外部资源
  const iconSize = new window.AMap.Size(24, 24);
  
  // 使用Canvas创建图标
  const canvas = document.createElement('canvas');
  canvas.width = 24;
  canvas.height = 24;
  const ctx = canvas.getContext('2d', { willReadFrequently: true });
  
  if (ctx) {
    // 绘制外圈（蓝色）
    ctx.beginPath();
    ctx.arc(12, 12, 10, 0, 2 * Math.PI);
    ctx.fillStyle = '#4285f4';
    ctx.fill();
    
    // 绘制内圈（白色）
    ctx.beginPath();
    ctx.arc(12, 12, 6, 0, 2 * Math.PI);
    ctx.fillStyle = '#ffffff';
    ctx.fill();
    
    // 绘制中心点
    ctx.beginPath();
    ctx.arc(12, 12, 2, 0, 2 * Math.PI);
    ctx.fillStyle = '#4285f4';
    ctx.fill();
  }
  
  return new window.AMap.Icon({
    size: iconSize,
    image: canvas.toDataURL(),
    imageSize: iconSize
  });
};

// 添加骑手标记
const addRiderMarkers = async () => {
  if (!map.value || !infoWindow.value || isDestroyed.value) return;

  // 如果已经加载过骑手标记，先清除
  if (ridersMarkersLoaded.value) {
    markers.value.forEach((marker: any) => marker.remove());
    markers.value = [];
    ridersMarkersLoaded.value = false;
  }

  try {
    // 从API获取在线骑手数据
    const { data } = await getOnlineRidersLocation({
      pageIndex: 1,
      pageSize: 100
    });

    if (isDestroyed.value) return;

    if (data && Array.isArray(data)) {
      riders.value = data;
      console.log('获取到骑手数据:', data.length, '个');
    } else {
      riders.value = [];
      console.log('未获取到骑手数据');
      return;
    }
  } catch (error) {
    console.error('获取骑手位置失败:', error);
    if (!isDestroyed.value) {
      riders.value = [];
    }
    return;
  }

  riders.value.forEach((rider: any) => {
    if (isDestroyed.value) return;
    
    // 创建自定义骑手图标
    const riderIcon = createRiderIcon();
    
    const marker = new window.AMap.Marker({
      position: [rider.longitude, rider.latitude],
      title: `骑手${rider.riderId}`,
      icon: riderIcon,
      anchor: 'center'
    });

    // 添加点击事件
    marker.on('click', () => {
      if (isDestroyed.value) return;
      
      infoWindow.value.setContent(`
        <div style="padding: 10px;">
          <h4 style="margin: 0 0 8px 0; color: #333;">骑手${rider.riderId}</h4>
          <p style="margin: 4px 0; color: #666;">状态: ${rider.status === 1 ? '在线' : '离线'}</p>
          <p style="margin: 4px 0; color: #666;">经度: ${rider.longitude.toFixed(6)}</p>
          <p style="margin: 4px 0; color: #666;">纬度: ${rider.latitude.toFixed(6)}</p>
          <p style="margin: 4px 0; color: #666;">更新时间: ${new Date(rider.locationTime).toLocaleString()}</p>
        </div>
      `);
      infoWindow.value.open(map.value, marker.getPosition());
      emit('rider-click', rider);
    });

    marker.setMap(map.value);
    markers.value.push(marker);
  });
  
  // 标记骑手标记已加载
  ridersMarkersLoaded.value = true;
};

// 初始化地图
const initMap = async () => {
  if (!mapContainer.value || isDestroyed.value) {
    console.warn('地图容器未准备好或组件已销毁');
    return;
  }

  try {
    if (!isDestroyed.value) mapStatus.value = 'loading';
    console.log('开始加载高德地图...', {
      container: mapContainer.value,
      containerSize: {
        width: mapContainer.value.offsetWidth,
        height: mapContainer.value.offsetHeight
      }
    });
    
    const AMap = await AMapLoader.load();
    if (isDestroyed.value) return;
    
    console.log('高德地图加载成功');

    // 确保容器有尺寸
    if (mapContainer.value.offsetWidth === 0 || mapContainer.value.offsetHeight === 0) {
      console.warn('地图容器尺寸为0，等待容器渲染...');
      setTimeout(() => initMap(), 200);
      return;
    }

    // 创建地图实例
    map.value = new AMap.Map(mapContainer.value, {
      zoom: props.zoom,
      center: [currentLocation.value.longitude, currentLocation.value.latitude],
      mapStyle: 'amap://styles/normal',
      features: ['bg', 'road', 'building'],
      viewMode: '2D' // 改为2D模式，更稳定
    });

    // 添加地图控件
    map.value.addControl(new AMap.Scale());
    map.value.addControl(new AMap.ToolBar());

    // 添加定位控件（禁用自动标记显示）
    const geolocation = new AMap.Geolocation({
      enableHighAccuracy: true,
      timeout: 10000,
      buttonPosition: 'RB',
      buttonOffset: new AMap.Pixel(10, 20),
      zoomToAccuracy: true,
      showMarker: false, // 禁用自动显示当前位置标记
      showButton: true,  // 保留定位按钮
      markerOptions: {
        // 即使showMarker为false，这里也可以设置标记样式（如果需要的话）
        icon: new AMap.Icon({
          size: new AMap.Size(0, 0), // 设置为0尺寸，实际上不显示
          image: 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg=='
        })
      }
    });
    map.value.addControl(geolocation);
    geolocationControl.value = geolocation;

    // 监听定位成功
    geolocation.on('complete', (data: any) => {
      if (isDestroyed.value) return;
      
      const newLocation = {
        longitude: data.position.lng,
        latitude: data.position.lat
      };
      currentLocation.value = newLocation;

      // 保存位置到localStorage
      try {
        localStorage.setItem('lastKnownLocation', JSON.stringify(newLocation));
      } catch (error) {
        console.warn('保存位置信息失败:', error);
      }

      // 立即通知父组件位置更新
      emit('location-update', newLocation);

      // 移动地图到当前位置
      if (map.value && !isDestroyed.value) {
        map.value.setCenter([data.position.lng, data.position.lat]);
      }

      // 显示成功消息
      window.$message?.success('定位成功！');

      console.log('地图定位成功，新位置:', newLocation);
    });

    // 监听定位失败
    geolocation.on('error', (error: any) => {
      if (isDestroyed.value) return;
      
      console.error('定位失败:', error);
      window.$message?.error('定位失败，请检查定位权限');
    });

    // 创建信息窗口
    infoWindow.value = new AMap.InfoWindow({
      offset: new AMap.Pixel(0, -30)
    });

    if (!isDestroyed.value) {
      mapStatus.value = 'ready';
      emit('map-ready', map.value);
    }

    // 如果显示骑手，则添加骑手标记
    if (props.showRiders) {
      await addRiderMarkers();
    }

    // 立即尝试定位骑手位置
    if (geolocationControl.value) {
      try {
        // 延迟一点执行，确保地图完全加载
        setTimeout(() => {
          geolocationControl.value.getCurrentPosition();
        }, 500);
      } catch (error) {
        console.log('初始定位失败，使用传入的位置:', error);
      }
    }

  } catch (error) {
    console.error('地图初始化失败:', error);
    if (!isDestroyed.value) {
      mapStatus.value = 'error';
    }
    
    // 显示错误信息
    if (mapContainer.value) {
      mapContainer.value.innerHTML = `
        <div style="
          display: flex;
          flex-direction: column;
          align-items: center;
          justify-content: center;
          height: 100%;
          background: #f5f5f5;
          border-radius: 12px;
          color: #666;
          text-align: center;
          padding: 20px;
        ">
          <div style="font-size: 48px; margin-bottom: 16px;">🗺️</div>
          <h3 style="margin: 0 0 8px 0; color: #333;">地图加载失败</h3>
          <p style="margin: 0 0 16px 0; font-size: 14px;">网络连接问题或API密钥无效</p>
          <button onclick="window.location.reload()" style="
            background: #4285f4;
            color: white;
            border: none;
            padding: 8px 16px;
            border-radius: 6px;
            cursor: pointer;
            font-size: 14px;
          ">重新加载</button>
        </div>
      `;
    }
  }
};

// 绘制导航路线
const drawNavigationRoute = (route: Api.Navigation.NavigationRoute) => {
  if (!map.value || !route.polyline) return;
  
  // 清除现有路线
  clearNavigationRoute();
  
  try {
    console.log('开始绘制导航路线...');
    
    // 解析路线坐标
    const path = route.polyline.split(';').map((point: string) => {
      const [lng, lat] = point.split(',').map(Number);
      return [lng, lat] as [number, number];
    });
    
    // 创建路线
    routePolyline.value = new AMap.Polyline({
      path: path,
      borderWeight: 3,
      strokeColor: '#3366FF',
      strokeOpacity: 0.8,
      strokeWeight: 6,
      strokeStyle: 'solid',
      lineJoin: 'round',
      lineCap: 'round',
      zIndex: 5
    });
    
    map.value.add(routePolyline.value);
    
    // 添加起点和终点标记
    if (path.length > 0) {
      // 起点标记
      startMarker.value = new AMap.Marker({
        position: path[0] as [number, number],
        icon: new AMap.Icon({
          size: new AMap.Size(32, 32),
          image: 'data:image/svg+xml;base64,' + btoa(`
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="#4CAF50">
              <path d="M12 2C8.13 2 5 5.13 5 9c0 5.25 7 13 7 13s7-7.75 7-13c0-3.87-3.13-7-7-7zm0 9.5c-1.38 0-2.5-1.12-2.5-2.5s1.12-2.5 2.5-2.5 2.5 1.12 2.5 2.5-1.12 2.5-2.5 2.5z"/>
            </svg>
          `),
          imageSize: new AMap.Size(32, 32)
        }),
        title: '起点'
      });
      
      // 终点标记
      endMarker.value = new AMap.Marker({
        position: path[path.length - 1] as [number, number],
        icon: new AMap.Icon({
          size: new AMap.Size(32, 32),
          image: 'data:image/svg+xml;base64,' + btoa(`
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="#F44336">
              <path d="M12 2C8.13 2 5 5.13 5 9c0 5.25 7 13 7 13s7-7.75 7-13c0-3.87-3.13-7-7-7zm0 9.5c-1.38 0-2.5-1.12-2.5-2.5s1.12-2.5 2.5-2.5 2.5 1.12 2.5 2.5-1.12 2.5-2.5 2.5z"/>
            </svg>
          `),
          imageSize: new AMap.Size(32, 32)
        }),
        title: '终点'
      });
      
      map.value.add(startMarker.value);
      map.value.add(endMarker.value);
    }
    
    // 调整地图视野以显示完整路线
    map.value.setFitView([routePolyline.value], false, [50, 50, 50, 50]);
    
    console.log('导航路线绘制成功');
  } catch (error) {
    console.error('导航路线绘制失败:', error);
  }
};

// 清除导航路线
const clearNavigationRoute = () => {
  if (routePolyline.value) {
    map.value?.remove(routePolyline.value);
    routePolyline.value = null;
  }
  if (startMarker.value) {
    map.value?.remove(startMarker.value);
    startMarker.value = null;
  }
  if (endMarker.value) {
    map.value?.remove(endMarker.value);
    endMarker.value = null;
  }
  // 清除导航标记
  navigationMarkers.value.forEach(marker => marker.remove());
  navigationMarkers.value = [];
};

// 开始导航
const startNavigation = () => {
  if (!props.navigationRoute || !map.value) return;
  
  isNavigationActive.value = true;
  currentStepIndex.value = 0;
  
  // 绘制路线
  drawNavigationRoute(props.navigationRoute);
  
  // 开始实时导航更新
  startRealTimeNavigation();
  
  emit('navigation-start');
  console.log('导航已开始');
};

// 结束导航
const endNavigation = () => {
  isNavigationActive.value = false;
  currentStepIndex.value = 0;
  
  // 清除导航路线
  clearNavigationRoute();
  
  // 停止实时更新
  if (navigationTimer.value) {
    clearInterval(navigationTimer.value);
    navigationTimer.value = null;
  }
  
  emit('navigation-end');
  console.log('导航已结束');
};

// 开始实时导航更新
const startRealTimeNavigation = () => {
  if (navigationTimer.value) {
    clearInterval(navigationTimer.value);
  }
  
  // 每5秒更新一次导航状态
  navigationTimer.value = setInterval(() => {
    if (isNavigationActive.value && props.currentOrder) {
      updateNavigationStatus();
    }
  }, 5000);
};

// 更新导航状态
const updateNavigationStatus = async () => {
  if (!props.currentOrder || !isNavigationActive.value) return;
  
  try {
    // 这里应该调用实时导航API
    // const response = await getOrderRealTimeNavigation({
    //   orderId: props.currentOrder.orderId,
    //   riderId: props.currentOrder.riderId
    // });
    
    // 模拟实时导航更新
    const mockUpdate: Api.Navigation.RealTimeNavigationUpdate = {
      routeId: props.navigationRoute?.routeId || '',
      currentLongitude: currentLocation.value.longitude,
      currentLatitude: currentLocation.value.latitude,
      currentSpeed: 30, // km/h
      remainingTime: 1200, // 20分钟
      remainingDistance: 5000, // 5公里
      nextInstruction: '前方200米右转',
      timestamp: new Date().toISOString()
    };
    
    emit('navigation-update', mockUpdate);
  } catch (error) {
    console.error('更新导航状态失败:', error);
  }
};

// 刷新地图
const refreshMap = async () => {
  if (isDestroyed.value) return;
  
  loading.value = true;
  try {
    console.log('开始刷新地图...');
    
    // 清除现有标记和路线
    markers.value.forEach(marker => marker.remove());
    markers.value = [];
    clearNavigationRoute();
    
    // 重新初始化地图
    if (map.value) {
      map.value.destroy();
      map.value = null;
    }
    
    // 重置状态
    mapStatus.value = 'loading';
    
    // 等待一下再重新初始化
    setTimeout(async () => {
      await initMap();
      window.$message?.success('地图刷新成功');
    }, 100);
    
  } catch (error) {
    console.error('刷新地图失败:', error);
    window.$message?.error('地图刷新失败');
  } finally {
    loading.value = false;
  }
};

// 定位我
const locateMe = async () => {
  if (!map.value || !geolocationControl.value) {
    window.$message?.warning('地图未初始化完成，请稍后再试');
    return;
  }

  locating.value = true;
  try {
    geolocationControl.value.getCurrentPosition();
  } catch (error) {
    console.error('定位失败:', error);
    window.$message?.error('定位失败，请检查定位权限');
  } finally {
    locating.value = false;
  }
};

// 显示骑手
const showRidersAction = async () => {
  loading.value = true;
  try {
    // 如果已经显示过骑手标记，则刷新数据；否则添加标记
    if (ridersMarkersLoaded.value) {
      // 刷新骑手数据
      await addRiderMarkers();
    } else {
      // 首次添加骑手标记
      await addRiderMarkers();
    }

    if (riders.value.length > 0) {
      window.$message?.success(`成功显示 ${riders.value.length} 个骑手位置`);
      // 调整地图视野以显示所有骑手
      if (riders.value.length > 1) {
        const bounds = new window.AMap.Bounds();
        riders.value.forEach((rider: any) => {
          bounds.extend([rider.longitude, rider.latitude]);
        });
        map.value.setBounds(bounds, { padding: [50, 50, 50, 50] });
      }
    } else {
      window.$message?.warning('当前区域暂无在线骑手');
    }
  } catch (error) {
    console.error('显示骑手失败:', error);
    window.$message?.error('显示骑手失败，请稍后重试');
  } finally {
    loading.value = false;
  }
};

// 清除骑手标记
const clearRiderMarkers = () => {
  if (markers.value.length > 0) {
    markers.value.forEach((marker: any) => marker.remove());
    markers.value = [];
    ridersMarkersLoaded.value = false;
    riders.value = [];
    window.$message?.success('已清除骑手标记');
  } else {
    window.$message?.info('当前没有显示骑手标记');
  }
};

// 监听props变化
watch(() => props.center, (newCenter) => {
  if (newCenter && map.value && !isDestroyed.value) {
    console.log('中心点变化，更新地图:', newCenter);
    map.value.setCenter([newCenter.longitude, newCenter.latitude]);
    currentLocation.value = newCenter;
  }
}, { deep: true });

// 监听导航路线变化
watch(() => props.navigationRoute, (newRoute) => {
  if (newRoute && map.value && !isDestroyed.value) {
    console.log('导航路线变化，绘制路线:', newRoute);
    drawNavigationRoute(newRoute);
  } else if (!newRoute && map.value) {
    console.log('清除导航路线');
    clearNavigationRoute();
  }
}, { deep: true });

// 监听导航状态变化
watch(() => props.isNavigating, (navigating) => {
  if (navigating && map.value && props.navigationRoute) {
    console.log('进入导航模式');
    startNavigation();
  } else if (!navigating) {
    console.log('退出导航模式');
    endNavigation();
  }
});

// 生命周期
onMounted(async () => {
  // 等待下一个tick确保DOM完全渲染
  await nextTick();
  // 再延迟一点确保容器完全准备好
  setTimeout(() => {
    initMap();
  }, 100);
});

onUnmounted(() => {
  isDestroyed.value = true;
  
  // 停止导航
  endNavigation();
  
  if (map.value) {
    map.value.destroy();
  }
  markers.value.forEach(marker => marker.remove());
});

// 暴露给父组件的方法
defineExpose({
  clearRiderMarkers,
  addRiderMarkers,
  refreshLocation: locateMe
});
</script>

<template>
  <div class="amap-container">
    <!-- 地图容器 -->
    <div ref="mapContainer" class="map-container">
      <!-- 加载状态 -->
      <div v-if="mapStatus === 'loading'" class="map-loading">
        <div class="loading-spinner"></div>
        <p>地图加载中...</p>
      </div>
    </div>

    <!-- 地图控制面板 -->
    <div class="map-controls">
      <NButton type="primary" size="small" @click="refreshMap" :loading="loading">
        <template #icon>
          <span class="text-lg">🔄</span>
        </template>
        刷新地图
      </NButton>

      <NButton type="info" size="small" @click="locateMe" :loading="locating">
        <template #icon>
          <span class="text-lg">📍</span>
        </template>
        定位我
      </NButton>

      <NButton type="success" size="small" @click="showRidersAction" :loading="loading">
        显示骑手 ({{ riders.length }})
      </NButton>
    </div>


  </div>
</template>

<style scoped>
.amap-container {
  position: relative;
  width: 100%;
  height: 100%;
}

.map-container {
  width: 100%;
  height: 400px;
  border-radius: 12px;
  overflow: hidden;
}

.map-loading {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.9);
  z-index: 1000;
  border-radius: 12px;
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #4285f4;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 16px;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
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



/* 响应式设计 */
@media (max-width: 768px) {
  .map-controls {
    top: 8px;
    right: 8px;
  }

  .map-container {
    height: 300px;
  }
}
</style>
