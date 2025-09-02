<template>
  <div class="amap-container">
    <!-- 地图容器 -->
    <div ref="mapContainer" class="map-container"></div>

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
  showRiders: true
});

// Emits
const emit = defineEmits<{
  'map-ready': [map: any];
  'location-update': [location: { longitude: number; latitude: number }];
  'rider-click': [rider: any];
}>();

// 响应式数据
const mapContainer = ref<HTMLElement>();
const map = ref<any>(null);
const loading = ref(false);
const locating = ref(false);
const mapStatus = ref<'loading' | 'ready' | 'error'>('loading');

// 地图数据
const currentLocation = ref(props.center);
const riders = ref<any[]>([]);
const markers = ref<any[]>([]);
const infoWindow = ref<any>(null);
const geolocationControl = ref<any>(null);

// 监听center变化，更新地图中心
watch(() => props.center, (newCenter) => {
  if (map.value && newCenter) {
    map.value.setCenter([newCenter.longitude, newCenter.latitude]);
    currentLocation.value = newCenter;
  }
}, { immediate: true });

// 添加骑手标记
const addRiderMarkers = async () => {
  if (!map.value || !infoWindow.value) return;

  // 清除现有标记
  markers.value.forEach((marker: any) => marker.remove());
  markers.value = [];

  try {
    // 从API获取在线骑手数据
    const { data } = await getOnlineRidersLocation({
      pageIndex: 1,
      pageSize: 100
    });

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
    riders.value = [];
    return;
  }

  riders.value.forEach((rider: any) => {
    const marker = new window.AMap.Marker({
      position: [rider.longitude, rider.latitude],
      title: `骑手${rider.riderId}`,
      icon: new window.AMap.Icon({
        size: new window.AMap.Size(32, 32),
        // 使用更简洁的圆形图标
        image: 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMzIiIGhlaWdodD0iMzIiIHZpZXdCb3g9IjAgMCAzMiAzMiI+PGNpcmNsZSBjeD0iMTYiIGN5PSIxNiIgcj0iMTQiIGZpbGw9IiM0Mjg1ZjQiLz48Y2lyY2xlIGN4PSIxNiIgY3k9IjE2IiByPSI4IiBmaWxsPSJ3aGl0ZSIvPjwvc3ZnPg==',
        imageSize: new window.AMap.Size(32, 32)
      })
    });

    // 添加点击事件
    marker.on('click', () => {
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
};

// 初始化地图
const initMap = async () => {
  if (!mapContainer.value) return;

  try {
    mapStatus.value = 'loading';
    const AMap = await AMapLoader.load();

    // 创建地图实例
    map.value = new AMap.Map(mapContainer.value, {
      zoom: props.zoom,
      center: [currentLocation.value.longitude, currentLocation.value.latitude],
      mapStyle: 'amap://styles/normal',
      features: ['bg', 'road', 'building'],
      viewMode: '3D'
    });

    // 添加地图控件
    map.value.addControl(new AMap.Scale());
    map.value.addControl(new AMap.ToolBar());

    // 添加定位控件
    const geolocation = new AMap.Geolocation({
      enableHighAccuracy: true,
      timeout: 10000,
      buttonPosition: 'RB',
      buttonOffset: new AMap.Pixel(10, 20),
      zoomToAccuracy: true
    });
    map.value.addControl(geolocation);
    geolocationControl.value = geolocation;

    // 监听定位成功
    geolocation.on('complete', (data: any) => {
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
      map.value.setCenter([data.position.lng, data.position.lat]);

      // 显示成功消息
      window.$message?.success('定位成功！');

      console.log('地图定位成功，新位置:', newLocation);
    });

    // 监听定位失败
    geolocation.on('error', (error: any) => {
      console.error('定位失败:', error);
      window.$message?.error('定位失败，请检查定位权限');
    });

    // 创建信息窗口
    infoWindow.value = new AMap.InfoWindow({
      offset: new AMap.Pixel(0, -30)
    });

    mapStatus.value = 'ready';
    emit('map-ready', map.value);

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
    mapStatus.value = 'error';
  }
};

// 刷新地图
const refreshMap = async () => {
  loading.value = true;
  try {
    if (map.value) {
      map.value.destroy();
    }
    await nextTick();
    await initMap();
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
    await addRiderMarkers();

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

// 生命周期
onMounted(() => {
  initMap();
});

onUnmounted(() => {
  if (map.value) {
    map.value.destroy();
  }
  markers.value.forEach(marker => marker.remove());
});
</script>

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
