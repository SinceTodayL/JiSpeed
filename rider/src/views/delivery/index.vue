<script setup lang="ts">
import { computed, h, onMounted, onUnmounted, ref } from 'vue';
import {
  NButton,
  NCard,
  NDataTable,
  NDescriptions,
  NDescriptionsItem,
  NModal,
  NSpace,
  NTag,
  NTab,
  NTabs,
  NTabPane
} from 'naive-ui';
import { useMessage } from 'naive-ui';
import { Icon } from '@iconify/vue';
import { getRiderOrderList } from '@/service/api/rider';
import { acceptOrder, rejectOrder, updateOrderStatus, updateAssignmentStatus, confirmDelivery } from '@/service/api/order-assignment';
import { useAuthStore } from '@/store/modules/auth';
import { useRiderStore } from '@/store/modules/rider';
import { handleCommonError, handleOrderError } from '@/utils/rider-error-handler';
import { useI18n } from 'vue-i18n';
import AmapMap from '@/components/map/amap-map.vue';
import { getRiderLatestLocation, updateRiderLocation, geocodeAddress } from '@/service/api/rider-location';
import type { RiderLocation } from '@/service/api/rider-location';

// 订单位置类型定义
interface OrderLocation {
  id: string;
  orderId: string;
  longitude: number;
  latitude: number;
  title: string;
  address: string;
  status: string;
  amount?: number;
  createTime?: string;
}

const authStore = useAuthStore();
const riderStore = useRiderStore();
const { t } = useI18n();
const message = useMessage();

// 定义通用订单数据类型
interface OrderData {
  assignId?: string; // 分配ID字段
  assignedAt?: string; // 分配时间
  acceptedStatus?: number; // 接单状态
  acceptedAt?: string; // 接单时间
  timeOut?: number; // 超时时间
  order?: {
    orderId: string;
    orderAmount: number;
    createAt: string;
    orderStatus: number;
    deliveryAddress: string;
    merchantName: string;
  };
}

// 骑手ID - 使用统一的 riderStore 获取
const riderId = computed(() => riderStore.riderId || authStore.userInfo.userId);

// 订单列表
const orders = ref<OrderData[]>([]);
const loading = ref(false);
const pagination = ref({
  page: 1,
  pageSize: 10,
  itemCount: 0
});

// 重置功能
const resetForm = ref({});

const showModal = ref(false);
const selectedOrder = ref<OrderData | null>(null);
const orderDetail = ref<any>(null);
const detailLoading = ref(false);

// 骑手在线状态
const isOnline = ref<boolean>(true);

// 地图相关数据
const riderLocation = ref<RiderLocation | null>(null);
const orderLocations = ref<OrderLocation[]>([]);
const mapLoading = ref(false);
const mapError = ref('');
const showMap = ref(false);

// 导航相关数据
const navigationStart = ref<{ longitude: number; latitude: number } | undefined>(undefined);
const navigationEnd = ref<{ longitude: number; latitude: number } | undefined>(undefined);
const navigationRoute = ref<any>(null);
const showNavigation = ref(false);

// 实时定位相关
const locationWatchId = ref<number | null>(null);
const isLocationWatching = ref(false);
const locationUpdateInterval = ref<NodeJS.Timeout | null>(null);

// 状态映射 - 根据后端定义：0=待接单, 1=已接单, 2=已拒绝, 3=已配送
const statusMap = {
  0: { text: '待接单', type: 'warning' },
  1: { text: '已接单', type: 'primary' },
  2: { text: '已拒绝', type: 'error' },
  3: { text: '已配送', type: 'success' }
};


// 获取订单数据
const fetchOrders = async () => {
  loading.value = true;
  try {
    const { data } = await getRiderOrderList(riderId.value, {});
    if (Array.isArray(data)) {
      orders.value = data as unknown as OrderData[];
      pagination.value.itemCount = orders.value.length;
    } else {
      orders.value = [];
      pagination.value.itemCount = 0;
    }
  } catch (error) {
    handleCommonError(error, '获取订单数据');
  } finally {
    loading.value = false;
  }
};

// 接单操作
const acceptOrderAction = async (order: OrderData) => {
  if (!order.assignId) {
    message.error('订单分配ID不存在');
    return;
  }
  
  try {
    await updateAssignmentStatus(riderId.value, order.assignId, {
      AcceptedStatus: 1 // 1 = 已接单
    });
    
    message.success('接单成功');
    await fetchOrders(); // 刷新订单列表
    await fetchOrderDetail(order.assignId); // 刷新详情
  } catch (error) {
    handleOrderError(error, '接单失败');
  }
};

// 拒单操作
const rejectOrderAction = async (order: OrderData) => {
  if (!order.assignId) {
    message.error('订单分配ID不存在');
    return;
  }
  
  try {
    await updateAssignmentStatus(riderId.value, order.assignId, {
      AcceptedStatus: 2 // 2 = 已拒绝
    });
    
    message.success('拒单成功');
    await fetchOrders(); // 刷新订单列表
  } catch (error) {
    handleOrderError(error, '拒单失败');
  }
};

// 完成订单操作
const completeOrderAction = async (order: OrderData) => {
  if (!order.assignId) {
    message.error('订单分配ID不存在');
    return;
  }
  
  try {
    await confirmDelivery(riderId.value, order.assignId);
    
    message.success('订单完成');
    await fetchOrders(); // 刷新订单列表
    await fetchOrderDetail(order.assignId); // 刷新详情
  } catch (error) {
    handleOrderError(error, '完成订单失败');
  }
};

// 获取订单详情
const fetchOrderDetail = async (_assignId: string) => {
  detailLoading.value = true;
  try {
    if (selectedOrder.value) {
      orderDetail.value = {
        ...selectedOrder.value,
        // 添加一些模拟的分配信息
        assignedAt: selectedOrder.value.assignedAt || new Date().toISOString(),
        acceptedAt: selectedOrder.value.acceptedAt || null,
        timeOut: 30 // 默认30分钟超时
      };
    }

  } catch (error: any) {
    console.error('获取订单详情失败:', error);
    window.$message?.warning('订单详情功能暂时不可用，显示基本信息');
  } finally {
    detailLoading.value = false;
  }
};

// 更新在线状态
const updateOnlineStatus = async (status: boolean) => {
  try {
    isOnline.value = status;
    window.$message?.success(status ? '已上线' : '已下线');
  } catch (error) {
    console.error('更新状态失败:', error);
    window.$message?.error('更新状态失败');
  }
};

// 重置功能
const handleReset = () => {
  pagination.value.page = 1;
  fetchOrders();
};

// 首次加载时获取数据
onMounted(async () => {
  // 检查用户信息是否已初始化
  if (!authStore.userInfo.userId && authStore.token) {
    await authStore.initUserInfo();
  }

  // 如果仍然没有用户信息，使用模拟数据
  if (!authStore.userInfo.userId) {
    Object.assign(authStore.userInfo, {
      userId: `rider_${Date.now()}`,
      userName: '测试骑手',
      roles: ['rider'],
      buttons: []
    });
  }

  await fetchOrders();
  await fetchRiderLocation();
  
  // 生成订单位置数据
  orderLocations.value = await generateOrderLocations(orders.value);
  
  // 开始定时更新位置
  startLocationUpdateInterval();
});

// 组件卸载时清理资源
onUnmounted(() => {
  stopLocationWatching();
});

// 处理分页变化
const handlePageChange = (page: number) => {
  pagination.value.page = page;
  fetchOrders();
};

// 订单详情
async function handleDetail(order: OrderData) {
  try {
    selectedOrder.value = order;
    showModal.value = true;

    // 重置详情数据
    orderDetail.value = null;

    // 如果有assignId，获取详细订单信息
    if (order.assignId) {
      await fetchOrderDetail(order.assignId);
    }
  } catch (error) {
    console.error('打开订单详情失败:', error);
    window.$message?.error('打开订单详情失败，请稍后重试');
  }
}

// 订单状态操作
async function handleStatusUpdate(order: OrderData, newStatus: number) {
  const statusText =
    {
      1: '接单',
      2: '拒绝',
      3: '确认配送'
    }[newStatus] || '更新状态';

  try {
    window.$dialog?.warning({
      title: `确认${statusText}`,
      content: `确定要${statusText}订单 ${order.order?.orderId} 吗？`,
      positiveText: '确定',
      negativeText: '取消',
      onPositiveClick: async () => {
        try {
          if (newStatus === 1) {
            // 接单 - 使用分配状态更新接口
            if (order.assignId) {
              await updateAssignmentStatus(riderId.value, order.assignId, {
                AcceptedStatus: 1
              });
            } else {
              // 如果没有分配ID，使用接单接口
              await acceptOrder({
                OrderId: order.order?.orderId || '',
                RiderId: riderId.value
              });
            }
          } else if (newStatus === 2) {
            // 拒绝订单 - 使用分配状态更新接口
            if (order.assignId) {
              await updateAssignmentStatus(riderId.value, order.assignId, {
                AcceptedStatus: 2
              });
            } else {
              // 如果没有分配ID，使用拒绝接口
              await rejectOrder({
                OrderId: order.order?.orderId || '',
                RiderId: riderId.value,
                Reason: '骑手主动拒绝'
              });
            }
          } else if (newStatus === 3) {
            // 确认配送（完成订单）- 使用专门的确认配送接口
            if (order.assignId) {
              await confirmDelivery(riderId.value, order.assignId);
            } else {
              // 如果没有分配ID，使用分配状态更新接口
              await updateAssignmentStatus(riderId.value, order.assignId || '', {
                AcceptedStatus: 3
              });
            }
          } else {
            // 其他状态更新 - 使用分配状态更新接口
            if (order.assignId) {
              await updateAssignmentStatus(riderId.value, order.assignId, {
                AcceptedStatus: newStatus
              });
            } else {
              // 如果没有分配ID，使用订单状态更新接口
              await updateOrderStatus({
                OrderId: order.order?.orderId || '',
                NewStatus: newStatus
              });
            }
          }
          
          window.$message?.success(`${statusText}成功！`);
          // 刷新订单列表
          await fetchOrders();
          // 如果详情弹窗打开，也刷新详情
          if (showModal.value && order.assignId) {
            await fetchOrderDetail(order.assignId);
          }
        } catch (error: any) {
          handleOrderError(error, `${statusText}失败，请稍后重试`);
        }
      }
    });
  } catch (error) {
    console.error('操作失败', error);
  }
}

// 分页数据
const paginatedOrders = computed(() => {
  if (!orders.value || !Array.isArray(orders.value)) return [];
  return orders.value;
});

// 地图相关方法
// 获取骑手位置
const fetchRiderLocation = async () => {
  try {
    mapLoading.value = true;
    mapError.value = '';
    
    const { data } = await getRiderLatestLocation(riderId.value);
    if (data && data.data) {
      riderLocation.value = data.data;
      console.log('骑手位置获取成功:', data.data);
    } else {
      // 设置默认位置（北京）
      riderLocation.value = {
        riderId: riderId.value,
        longitude: 116.397428,
        latitude: 39.90923,
        locationTime: new Date().toISOString(),
        status: 1
      };
    }
  } catch (error: any) {
    mapError.value = '获取骑手位置失败';
    console.error('获取骑手位置失败:', error);
    // 设置默认位置（北京）
    riderLocation.value = {
      riderId: riderId.value,
      longitude: 116.397428,
      latitude: 39.90923,
      locationTime: new Date().toISOString(),
      status: 1
    };
  } finally {
    mapLoading.value = false;
  }
};

// 更新骑手位置
const updateRiderLocationData = async (
  longitude: number, 
  latitude: number, 
  accuracy?: number,
  speed?: number,
  direction?: number
) => {
  try {
    await updateRiderLocation({
      riderId: riderId.value,
      longitude,
      latitude,
      accuracy,
      speed,
      direction,
      locationType: 'GPS'
    });
    
    // 更新本地位置数据
    riderLocation.value = {
      riderId: riderId.value,
      longitude,
      latitude,
      accuracy,
      speed,
      direction,
      locationTime: new Date().toISOString(),
      status: 1,
      locationType: 'GPS'
    };
    
    console.log('骑手位置更新成功:', riderLocation.value);
  } catch (error: any) {
    console.error('更新骑手位置失败:', error);
    message.error('更新位置失败: ' + (error.message || '未知错误'));
  }
};

// 处理地图点击事件
const handleMapClick = (lng: number, lat: number) => {
  console.log('地图点击位置:', { longitude: lng, latitude: lat });
};

// 处理骑手位置更新
const handleRiderLocationUpdate = async (location: { longitude: number; latitude: number }) => {
  try {
    // 更新本地状态
    riderLocation.value = {
      ...riderLocation.value,
      longitude: location.longitude,
      latitude: location.latitude,
      locationTime: new Date().toISOString(),
      status: 1 // 在线状态
    };
    
    // 发送到服务器
    await updateRiderLocation({
      riderId: riderId.value,
      longitude: location.longitude,
      latitude: location.latitude,
      locationType: 'GPS'
    });
    
    console.log('骑手位置已更新:', location);
  } catch (error) {
    console.error('更新骑手位置失败:', error);
    message.error('位置更新失败');
  }
};

// 处理订单位置点击
const handleOrderClick = (orderId: string) => {
  const order = orders.value.find(o => o.order?.orderId === orderId);
  if (order) {
    handleDetail(order);
  }
};

// 开始导航到订单位置
const startNavigationToOrder = (order: OrderData) => {
  if (!riderLocation.value || !order.order?.deliveryAddress) {
    message.warning('无法获取位置信息');
    return;
  }
  
  // 查找订单位置
  const orderLocation = orderLocations.value.find(ol => ol.orderId === order.order?.orderId);
  if (!orderLocation) {
    message.warning('无法获取订单位置');
    return;
  }
  
  // 设置导航起点和终点
  navigationStart.value = {
    longitude: riderLocation.value.longitude,
    latitude: riderLocation.value.latitude
  };
  
  navigationEnd.value = {
    longitude: orderLocation.longitude,
    latitude: orderLocation.latitude
  };
  
  showNavigation.value = true;
  message.success('开始导航到订单位置');
};

// 处理路径规划完成
const handleRoutePlanned = (route: any) => {
  navigationRoute.value = route;
  console.log('路径规划完成:', route);
};

// 清除导航
const clearNavigation = () => {
  navigationStart.value = undefined;
  navigationEnd.value = undefined;
  navigationRoute.value = null;
  showNavigation.value = false;
};

// 开始实时定位
const startLocationWatching = () => {
  if (isLocationWatching.value) return;
  
  if ('geolocation' in navigator) {
    isLocationWatching.value = true;
    
    // 使用浏览器定位API获取GPS位置，然后更新到服务器
    locationWatchId.value = navigator.geolocation.watchPosition(
      async (position) => {
        const location = {
          longitude: position.coords.longitude,
          latitude: position.coords.latitude,
          accuracy: position.coords.accuracy,
          speed: position.coords.speed,
          direction: position.coords.heading,
          locationTime: new Date().toISOString(),
          status: 1,
          locationType: 'GPS'
        };
        
        // 发送到服务器并更新本地位置
        await updateRiderLocationData(
          location.longitude, 
          location.latitude, 
          location.accuracy,
          location.speed || undefined,
          location.direction || undefined
        );
      },
      (error) => {
        console.error('定位失败:', error);
        message.error('定位失败: ' + error.message);
        stopLocationWatching();
      },
      {
        enableHighAccuracy: true,
        timeout: 10000,
        maximumAge: 5000
      }
    );
    
    message.success('开始实时定位');
  } else {
    message.error('浏览器不支持定位功能');
  }
};

// 停止实时定位
const stopLocationWatching = () => {
  if (locationWatchId.value !== null) {
    navigator.geolocation.clearWatch(locationWatchId.value);
    locationWatchId.value = null;
  }
  
  if (locationUpdateInterval.value) {
    clearInterval(locationUpdateInterval.value);
    locationUpdateInterval.value = null;
  }
  
  isLocationWatching.value = false;
  message.info('停止实时定位');
};

// 定时更新位置（从后端API获取）
const startLocationUpdateInterval = () => {
  if (locationUpdateInterval.value) return;
  
  locationUpdateInterval.value = setInterval(async () => {
    try {
      // 从后端API获取骑手最新位置
      await fetchRiderLocation();
      console.log('定时更新骑手位置成功');
    } catch (error) {
      console.error('定时更新位置失败:', error);
    }
  }, 30000); // 每30秒从后端获取一次位置
};

// 生成订单位置数据
const generateOrderLocations = async (orders: OrderData[]): Promise<OrderLocation[]> => {
  const orderLocations: OrderLocation[] = [];
  
  for (const order of orders) {
    if (order.order?.deliveryAddress) {
      try {
        // 使用地理编码获取真实坐标
        const { data } = await geocodeAddress({
          address: order.order.deliveryAddress
        });
        
        if (data && data.data) {
          orderLocations.push({
            id: order.order.orderId || '',
            orderId: order.order.orderId || '',
            longitude: data.data.longitude,
            latitude: data.data.latitude,
            title: `订单 ${order.order.orderId}`,
            address: order.order.deliveryAddress,
            status: getStatusText(order.acceptedStatus || 0),
            amount: order.order.orderAmount || 0,
            createTime: order.order.createAt || ''
          });
        } else {
          // 如果地理编码失败，使用默认坐标
          orderLocations.push({
            id: order.order.orderId || '',
            orderId: order.order.orderId || '',
            longitude: 116.397428,
            latitude: 39.90923,
            title: `订单 ${order.order.orderId}`,
            address: order.order.deliveryAddress,
            status: getStatusText(order.acceptedStatus || 0),
            amount: order.order.orderAmount || 0,
            createTime: order.order.createAt || ''
          });
        }
      } catch (error) {
        console.error(`地理编码失败 ${order.order.orderId}:`, error);
        // 地理编码失败时使用默认坐标
        orderLocations.push({
          id: order.order.orderId || '',
          orderId: order.order.orderId || '',
          longitude: 116.397428,
          latitude: 39.90923,
          title: `订单 ${order.order.orderId}`,
          address: order.order.deliveryAddress,
          status: getStatusText(order.acceptedStatus || 0),
          amount: order.order.orderAmount || 0,
          createTime: order.order.createAt || ''
        });
      }
    }
  }
  
  return orderLocations;
};

// 获取状态文本
const getStatusText = (status: number): string => {
  const statusMap = {
    0: '待接单',
    1: '已接单',
    2: '已拒绝',
    3: '已配送'
  };
  return statusMap[status as keyof typeof statusMap] || '未知';
};
</script>

<template>
  <div class="min-h-full p-24px bg-gradient-to-br from-blue-50 via-indigo-50 to-purple-50 dark:from-gray-900 dark:via-gray-800 dark:to-gray-900 relative overflow-hidden">
    <!-- 背景装饰元素 -->
    <div class="absolute inset-0 overflow-hidden pointer-events-none">
      <div class="absolute -top-40 -right-40 w-80 h-80 bg-gradient-to-br from-blue-400/20 to-purple-600/20 rounded-full blur-3xl"></div>
      <div class="absolute -bottom-40 -left-40 w-80 h-80 bg-gradient-to-tr from-indigo-400/20 to-pink-600/20 rounded-full blur-3xl"></div>
      <div class="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 w-96 h-96 bg-gradient-to-r from-cyan-400/10 to-blue-600/10 rounded-full blur-3xl"></div>
    </div>

    <!-- 页面标题区域 -->
    <NCard :bordered="false" class="mb-24px bg-white/80 dark:bg-gray-800/80 backdrop-blur-sm">
      <div class="flex items-center gap-3">
        <div class="w-12 h-12 bg-gradient-to-r from-blue-500 to-purple-600 rounded-xl flex items-center justify-center">
          <Icon icon="mdi:motorbike" class="text-2xl text-white" />
        </div>
        <div class="flex-1">
           <h1 class="text-2xl text-gray-800 font-bold dark:text-gray-200">
             {{ t('rider.delivery.title') }}
           </h1>
           <p class="mt-2px text-gray-600 dark:text-gray-400">
             {{ t('rider.delivery.subtitle') }}
           </p>
        </div>
        <div class="flex items-center space-x-16px">
          <!-- 状态指示器 -->
          <div class="flex items-center gap-2 px-3 py-1 bg-green-100 dark:bg-green-900/30 rounded-full">
            <div class="w-1.5 h-1.5 bg-green-500 rounded-full animate-pulse"></div>
             <span class="text-xs font-medium text-green-700 dark:text-green-400">{{ t('rider.delivery.realTime') }}</span>
           </div>
           <div class="flex items-center gap-2 px-3 py-1 bg-blue-100 dark:bg-blue-900/30 rounded-full">
             <Icon icon="mdi:shield-check" class="text-blue-600 dark:text-blue-400 text-sm" />
             <span class="text-xs font-medium text-blue-700 dark:text-blue-400">{{ t('rider.delivery.secure') }}</span>
          </div>
        </div>
      </div>
    </NCard>

    <!-- 统计卡片区域 -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <!-- 总订单数 -->
      <NCard :bordered="false" class="bg-gradient-to-br from-blue-500 to-blue-600 text-white shadow-xl hover:shadow-2xl transition-all duration-300 hover:-translate-y-1">
        <div class="flex items-center justify-between py-2">
          <div>
             <p class="text-blue-100 text-sm font-medium">{{ t('rider.delivery.totalOrders') }}</p>
            <p class="text-2xl font-bold mt-1">{{ orders.length }}</p>
          </div>
          <div class="w-10 h-10 bg-white/20 rounded-lg flex items-center justify-center">
            <Icon icon="mdi:package-variant" class="text-xl" />
          </div>
        </div>
      </NCard>

      <!-- 待接单 -->
      <NCard :bordered="false" class="bg-gradient-to-br from-orange-500 to-orange-600 text-white shadow-xl hover:shadow-2xl transition-all duration-300 hover:-translate-y-1">
        <div class="flex items-center justify-between py-2">
          <div>
             <p class="text-orange-100 text-sm font-medium">{{ t('rider.delivery.pendingOrders') }}</p>
            <p class="text-2xl font-bold mt-1">{{ orders.filter(o => (o.acceptedStatus || 0) === 0).length }}</p>
          </div>
          <div class="w-10 h-10 bg-white/20 rounded-lg flex items-center justify-center">
            <Icon icon="mdi:clock-outline" class="text-xl" />
          </div>
        </div>
      </NCard>

      <!-- 已接单 -->
      <NCard :bordered="false" class="bg-gradient-to-br from-green-500 to-green-600 text-white shadow-xl hover:shadow-2xl transition-all duration-300 hover:-translate-y-1">
        <div class="flex items-center justify-between py-2">
          <div>
             <p class="text-green-100 text-sm font-medium">{{ t('rider.delivery.acceptedOrders') }}</p>
            <p class="text-2xl font-bold mt-1">{{ orders.filter(o => (o.acceptedStatus || 0) === 1).length }}</p>
          </div>
          <div class="w-10 h-10 bg-white/20 rounded-lg flex items-center justify-center">
            <Icon icon="mdi:check-circle" class="text-xl" />
          </div>
        </div>
      </NCard>

      <!-- 已配送 -->
      <NCard :bordered="false" class="bg-gradient-to-br from-purple-500 to-purple-600 text-white shadow-xl hover:shadow-2xl transition-all duration-300 hover:-translate-y-1">
        <div class="flex items-center justify-between py-2">
          <div>
             <p class="text-purple-100 text-sm font-medium">{{ t('rider.delivery.deliveredOrders') }}</p>
            <p class="text-2xl font-bold mt-1">{{ orders.filter(o => (o.acceptedStatus || 0) === 3).length }}</p>
          </div>
          <div class="w-10 h-10 bg-white/20 rounded-lg flex items-center justify-center">
            <Icon icon="mdi:truck-delivery" class="text-xl" />
          </div>
        </div>
      </NCard>
    </div>

    <!-- 操作区域 -->
    <NCard :bordered="false" class="mb-6 bg-white/90 dark:bg-gray-800/90 backdrop-blur-md rounded-2xl shadow-lg border-0">
      <div class="flex items-center justify-end py-3">
        <div class="flex items-center gap-3">
          <NButton 
            @click="handleReset" 
            type="primary" 
            ghost
            class="px-6 py-2 rounded-xl font-medium"
          >
             <Icon icon="mdi:refresh" class="mr-2" />
             {{ t('rider.delivery.refresh') }}
          </NButton>
          <NButton 
            :type="isOnline ? 'success' : 'warning'" 
            @click="updateOnlineStatus(!isOnline)"
            class="px-6 py-2 rounded-xl font-medium"
          >
            <Icon :icon="isOnline ? 'mdi:account-check' : 'mdi:account-off'" class="mr-2" />
             {{ isOnline ? t('rider.delivery.goOffline') : t('rider.delivery.goOnline') }}
          </NButton>
        </div>
      </div>
    </NCard>

    <!-- 订单列表 -->
    <NCard :bordered="false" class="rounded-2xl shadow-xl bg-white/95 dark:bg-gray-800/95 backdrop-blur-md border-0 overflow-hidden">
      <!-- 列表头部 -->
      <div class="bg-gradient-to-r from-gray-50 to-blue-50 dark:from-gray-700 dark:to-gray-800 px-6 py-4 border-b border-gray-200/50 dark:border-gray-600/50">
        <div class="flex items-center justify-between">
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 bg-gradient-to-r from-blue-500 to-purple-600 rounded-xl flex items-center justify-center">
              <Icon icon="mdi:format-list-bulleted" class="text-white text-lg" />
            </div>
            <div>
               <h3 class="text-xl font-bold text-gray-800 dark:text-gray-200">{{ t('rider.delivery.orderList') }}</h3>
               <p class="text-gray-600 dark:text-gray-400 text-sm">{{ t('rider.delivery.orderListDesc') }}</p>
            </div>
          </div>
          <div class="flex items-center gap-2 px-3 py-1 bg-white/80 dark:bg-gray-700/80 rounded-full">
            <div class="w-2 h-2 bg-green-500 rounded-full animate-pulse"></div>
            <span class="text-sm font-medium text-gray-700 dark:text-gray-300">{{ t('rider.delivery.realTimeUpdate') }}</span>
          </div>
        </div>
      </div>

      <div class="p-4">
        <NDataTable
          :columns="[
            {
              title: '订单信息',
              key: 'orderInfo',
              width: 200,
              render: (row: OrderData) => {
                return h('div', { class: 'flex items-center gap-3' }, [
                  h('div', { class: 'w-10 h-10 bg-gradient-to-r from-blue-500 to-purple-600 rounded-lg flex items-center justify-center' }, [
                    h(Icon, { icon: 'mdi:package-variant', class: 'text-white text-lg' })
                  ]),
                  h('div', { class: 'flex-1' }, [
                    h('div', { class: 'font-semibold text-gray-800 dark:text-gray-200 text-sm' }, row.order?.orderId || '-'),
                    h('div', { class: 'text-xs text-gray-500 dark:text-gray-400' }, '订单编号')
                  ])
                ]);
              }
            },
            {
              title: '配送地址',
              key: 'address',
              width: 250,
              render: (row: OrderData) => {
                return h('div', { class: 'flex items-center gap-2' }, [
                  h(Icon, { icon: 'mdi:map-marker', class: 'text-red-500 text-sm flex-shrink-0' }),
                  h('div', { class: 'text-sm text-gray-700 dark:text-gray-300 truncate' }, row.order?.deliveryAddress || '-')
                ]);
              }
            },
            {
              title: '商家信息',
              key: 'merchant',
              width: 150,
              render: (row: OrderData) => {
                return h('div', { class: 'flex items-center gap-2' }, [
                  h(Icon, { icon: 'mdi:store', class: 'text-blue-500 text-sm flex-shrink-0' }),
                  h('div', { class: 'text-sm text-gray-700 dark:text-gray-300 truncate' }, row.order?.merchantName || '-')
                ]);
              }
            },
            {
              title: '订单状态',
              key: 'status',
              width: 120,
              render(row: OrderData) {
                const status = row.acceptedStatus || 0;
                const statusInfo = statusMap[status as keyof typeof statusMap] || { text: '未知', type: 'default' };
                const statusColors = {
                  0: 'from-orange-400 to-orange-500',
                  1: 'from-green-400 to-green-500',
                  2: 'from-red-400 to-red-500',
                  3: 'from-purple-400 to-purple-500'
                };
                return h('div', { class: 'flex items-center gap-2' }, [
                  h('div', { 
                    class: `w-2 h-2 rounded-full bg-gradient-to-r ${statusColors[status as keyof typeof statusColors] || 'from-gray-400 to-gray-500'}`
                  }),
                  h(NTag, { 
                    type: statusInfo.type as any, 
                    size: 'small',
                    class: 'px-3 py-1 rounded-full font-medium'
                  }, { default: () => statusInfo.text })
                ]);
              }
            },
            {
              title: '订单金额',
              key: 'amount',
              width: 120,
              render: (row: OrderData) => {
                return h('div', { class: 'text-right' }, [
                  h('div', { class: 'text-lg font-bold text-green-600 dark:text-green-400' }, `￥${row.order?.orderAmount || 0}`),
                ]);
              }
            },
            {
              title: '创建时间',
              key: 'createAt',
              width: 150,
              render: (row: OrderData) => {
                return h('div', { class: 'flex items-center gap-2' }, [
                  h(Icon, { icon: 'mdi:clock-outline', class: 'text-gray-400 text-sm flex-shrink-0' }),
                  h('div', { class: 'text-sm text-gray-600 dark:text-gray-400' }, row.order?.createAt || '-')
                ]);
              }
            },
            {
              title: '操作',
              key: 'actions',
              width: 200,
              render(row: OrderData) {
                const currentStatus = row.acceptedStatus || 0;
                const buttons: any[] = [];

                // 详情按钮
                buttons.push(
                  h(
                    NButton,
                    { 
                      size: 'small', 
                      type: 'primary', 
                      ghost: true,
                      onClick: () => handleDetail(row),
                      class: 'px-4 py-1 rounded-lg font-medium'
                    },
                    { default: () => [
                      h(Icon, { icon: 'mdi:eye', class: 'mr-1 text-sm' }),
                      '详情'
                    ]}
                  )
                );

                // 根据当前状态显示不同的操作按钮
                if (currentStatus === 0) {
                  // 待接单状态：显示接单和拒绝按钮
                  buttons.push(
                    h(
                      NButton,
                      { 
                        size: 'small', 
                        type: 'success', 
                        onClick: () => acceptOrderAction(row),
                        class: 'px-4 py-1 rounded-lg font-medium'
                      },
                      { default: () => [
                        h(Icon, { icon: 'mdi:check', class: 'mr-1 text-sm' }),
                        '接单'
                      ]}
                    ),
                    h(
                      NButton,
                      { 
                        size: 'small', 
                        type: 'warning', 
                        onClick: () => rejectOrderAction(row),
                        class: 'px-4 py-1 rounded-lg font-medium'
                      },
                      { default: () => [
                        h(Icon, { icon: 'mdi:close', class: 'mr-1 text-sm' }),
                        '拒绝'
                      ]}
                    )
                  );
                } else if (currentStatus === 1) {
                  // 已接单状态：显示确认配送按钮
                  buttons.push(
                    h(
                      NButton,
                      { 
                        size: 'small', 
                        type: 'success', 
                        onClick: () => completeOrderAction(row),
                        class: 'px-4 py-1 rounded-lg font-medium'
                      },
                      { default: () => [
                        h(Icon, { icon: 'mdi:truck-delivery', class: 'mr-1 text-sm' }),
                        '确认配送'
                      ]}
                    )
                  );
                }

                return h(NSpace, { size: 'small' }, {
                  default: () => buttons
                });
              }
            }
          ]"
          :data="paginatedOrders"
          :loading="loading"
          :pagination="pagination"
          @update:page="handlePageChange"
          class="modern-table"
        />
      </div>
    </NCard>

    <!-- 订单详情弹窗 -->
    <NModal 
      v-model:show="showModal" 
      preset="card" 
      class="w-[900px] max-w-[95vw]"
      :loading="detailLoading"
      :mask-closable="false"
    >
      <template #header>
        <div class="flex items-center gap-3">
          <div class="w-10 h-10 bg-gradient-to-r from-blue-500 to-purple-600 rounded-xl flex items-center justify-center">
            <Icon icon="mdi:package-variant" class="text-white text-lg" />
          </div>
          <div>
            <h3 class="text-xl font-bold text-gray-800 dark:text-gray-200">订单详情</h3>
            <p class="text-sm text-gray-600 dark:text-gray-400">查看订单完整信息</p>
          </div>
        </div>
      </template>

      <div v-if="!detailLoading && orderDetail">
        <!-- 标签页 -->
        <NTabs type="line" animated>
          <NTabPane name="details" tab="订单详情">
            <div class="space-y-6">
              <!-- 订单基本信息 -->
              <div class="bg-gradient-to-r from-blue-50 to-purple-50 dark:from-gray-700 dark:to-gray-800 rounded-2xl p-6">
                <h4 class="text-lg font-semibold text-gray-800 dark:text-gray-200 mb-4 flex items-center gap-2">
                  <Icon icon="mdi:information" class="text-blue-500" />
                  基本信息
                </h4>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div class="flex items-center gap-3">
                    <Icon icon="mdi:identifier" class="text-gray-500 text-lg" />
                    <div>
                      <p class="text-sm text-gray-600 dark:text-gray-400">订单号</p>
                      <p class="font-semibold text-gray-800 dark:text-gray-200">{{ orderDetail.order?.orderId || '-' }}</p>
                    </div>
                  </div>
                  <div class="flex items-center gap-3">
                    <Icon icon="mdi:currency-usd" class="text-green-500 text-lg" />
                    <div>
                      <p class="text-sm text-gray-600 dark:text-gray-400">订单金额</p>
                      <p class="font-bold text-green-600 dark:text-green-400 text-xl">￥{{ orderDetail.order?.orderAmount || 0 }}</p>
                    </div>
                  </div>
                </div>
              </div>

              <!-- 配送信息 -->
              <div class="bg-gradient-to-r from-orange-50 to-red-50 dark:from-gray-700 dark:to-gray-800 rounded-2xl p-6">
                <h4 class="text-lg font-semibold text-gray-800 dark:text-gray-200 mb-4 flex items-center gap-2">
                  <Icon icon="mdi:map-marker" class="text-red-500" />
                  配送信息
                </h4>
                <div class="space-y-3">
                  <div class="flex items-start gap-3">
                    <Icon icon="mdi:map-marker-outline" class="text-red-500 text-lg mt-1" />
                    <div>
                      <p class="text-sm text-gray-600 dark:text-gray-400">配送地址</p>
                      <p class="font-medium text-gray-800 dark:text-gray-200">{{ orderDetail.order?.deliveryAddress || '-' }}</p>
                    </div>
                  </div>
                  <div class="flex items-center gap-3">
                    <Icon icon="mdi:store" class="text-blue-500 text-lg" />
                    <div>
                      <p class="text-sm text-gray-600 dark:text-gray-400">商家名称</p>
                      <p class="font-medium text-gray-800 dark:text-gray-200">{{ orderDetail.order?.merchantName || '-' }}</p>
                    </div>
                  </div>
                </div>
              </div>

              <!-- 状态信息 -->
              <div class="bg-gradient-to-r from-green-50 to-emerald-50 dark:from-gray-700 dark:to-gray-800 rounded-2xl p-6">
                <h4 class="text-lg font-semibold text-gray-800 dark:text-gray-200 mb-4 flex items-center gap-2">
                  <Icon icon="mdi:timeline" class="text-green-500" />
                  状态信息
                </h4>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div class="flex items-center gap-3">
                    <div class="w-3 h-3 bg-gradient-to-r from-green-400 to-green-500 rounded-full"></div>
                    <div>
                      <p class="text-sm text-gray-600 dark:text-gray-400">当前状态</p>
                      <NTag
                        :type="(statusMap[(orderDetail.acceptedStatus || 0) as keyof typeof statusMap]?.type || 'default') as any"
                        size="medium"
                        class="px-3 py-1 rounded-full font-medium"
                      >
                        {{ statusMap[(orderDetail.acceptedStatus || 0) as keyof typeof statusMap]?.text || '未知' }}
                      </NTag>
                    </div>
                  </div>
                  <div class="flex items-center gap-3">
                    <Icon icon="mdi:clock-outline" class="text-gray-500 text-lg" />
                    <div>
                      <p class="text-sm text-gray-600 dark:text-gray-400">超时时间</p>
                      <p class="font-medium text-gray-800 dark:text-gray-200">{{ orderDetail.timeOut || 0 }} 分钟</p>
                    </div>
                  </div>
                </div>
              </div>

              <!-- 时间信息 -->
              <div class="bg-gradient-to-r from-purple-50 to-indigo-50 dark:from-gray-700 dark:to-gray-800 rounded-2xl p-6">
                <h4 class="text-lg font-semibold text-gray-800 dark:text-gray-200 mb-4 flex items-center gap-2">
                  <Icon icon="mdi:clock" class="text-purple-500" />
                  时间信息
                </h4>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div class="flex items-center gap-3">
                    <Icon icon="mdi:calendar-plus" class="text-gray-500 text-lg" />
                    <div>
                      <p class="text-sm text-gray-600 dark:text-gray-400">创建时间</p>
                      <p class="font-medium text-gray-800 dark:text-gray-200">{{ orderDetail.order?.createAt || '-' }}</p>
                    </div>
                  </div>
                  <div class="flex items-center gap-3">
                    <Icon icon="mdi:calendar-check" class="text-blue-500 text-lg" />
                    <div>
                      <p class="text-sm text-gray-600 dark:text-gray-400">分配时间</p>
                      <p class="font-medium text-gray-800 dark:text-gray-200">{{ orderDetail.assignedAt || '-' }}</p>
                    </div>
                  </div>
                  <div class="flex items-center gap-3">
                    <Icon icon="mdi:calendar-heart" class="text-green-500 text-lg" />
                    <div>
                      <p class="text-sm text-gray-600 dark:text-gray-400">接单时间</p>
                      <p class="font-medium text-gray-800 dark:text-gray-200">{{ orderDetail.acceptedAt || '-' }}</p>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </NTabPane>
          
          <!-- 地图标签页 -->
          <NTabPane name="map" tab="地图视图">
            <div class="space-y-4">
              <!-- 地图控制面板 -->
              <div class="flex items-center justify-between p-4 bg-gray-50 dark:bg-gray-700 rounded-lg">
                <div class="flex items-center gap-4">
                  <div class="flex items-center gap-2">
                    <div class="w-3 h-3 bg-blue-500 rounded-full"></div>
                    <span class="text-sm text-gray-600 dark:text-gray-400">骑手位置</span>
                  </div>
                  <div class="flex items-center gap-2">
                    <div class="w-3 h-3 bg-green-500 rounded-full"></div>
                    <span class="text-sm text-gray-600 dark:text-gray-400">订单位置</span>
                  </div>
                </div>
                <div class="flex items-center gap-2">
                  <NButton 
                    size="small" 
                    type="primary" 
                    @click="fetchRiderLocation"
                    :loading="mapLoading"
                  >
                    <template #icon>
                      <Icon icon="mdi:refresh" />
                    </template>
                    刷新位置
                  </NButton>
                  
                  <NButton 
                    v-if="selectedOrder && riderLocation"
                    size="small" 
                    type="success" 
                    @click="startNavigationToOrder(selectedOrder)"
                  >
                    <template #icon>
                      <Icon icon="mdi:navigation" />
                    </template>
                    导航到订单
                  </NButton>
                  
                  <NButton 
                    v-if="showNavigation"
                    size="small" 
                    type="warning" 
                    @click="clearNavigation"
                  >
                    <template #icon>
                      <Icon icon="mdi:close" />
                    </template>
                    清除导航
                  </NButton>
                  
                  <NButton 
                    v-if="!isLocationWatching"
                    size="small" 
                    type="info" 
                    @click="startLocationWatching"
                  >
                    <template #icon>
                      <Icon icon="mdi:crosshairs-gps" />
                    </template>
                    开始定位
                  </NButton>
                  
                  <NButton 
                    v-if="isLocationWatching"
                    size="small" 
                    type="error" 
                    @click="stopLocationWatching"
                  >
                    <template #icon>
                      <Icon icon="mdi:stop" />
                    </template>
                    停止定位
                  </NButton>
                </div>
              </div>
              
              <!-- 地图组件 -->
              <div class="rounded-lg overflow-hidden border border-gray-200 dark:border-gray-600">
                <AmapMap
                  :height="400"
                  :show-controls="true"
                  :enable-location="true"
                  :show-rider-location="true"
                  :show-order-locations="true"
                  :enable-navigation="showNavigation"
                  :navigation-start="navigationStart"
                  :navigation-end="navigationEnd"
                  :rider-location="riderLocation ? {
                    longitude: riderLocation.longitude,
                    latitude: riderLocation.latitude,
                    accuracy: riderLocation.accuracy,
                    direction: riderLocation.direction
                  } : undefined"
                  :order-locations="orderLocations"
                  @map-click="handleMapClick"
                  @rider-location-update="handleRiderLocationUpdate"
                  @order-click="handleOrderClick"
                  @route-planned="handleRoutePlanned"
                />
              </div>
              
              <!-- 位置信息 -->
              <div v-if="riderLocation" class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div class="p-4 bg-blue-50 dark:bg-blue-900/20 rounded-lg">
                  <h4 class="font-semibold text-blue-800 dark:text-blue-200 mb-2 flex items-center gap-2">
                    <Icon icon="mdi:crosshairs-gps" />
                    骑手位置
                  </h4>
                  <div class="space-y-1 text-sm">
                    <p class="text-gray-600 dark:text-gray-400">
                      经度: {{ riderLocation.longitude.toFixed(6) }}
                    </p>
                    <p class="text-gray-600 dark:text-gray-400">
                      纬度: {{ riderLocation.latitude.toFixed(6) }}
                    </p>
                    <p class="text-gray-600 dark:text-gray-400">
                      更新时间: {{ new Date(riderLocation.locationTime).toLocaleString() }}
                    </p>
                    <p v-if="riderLocation.accuracy" class="text-gray-600 dark:text-gray-400">
                      精度: {{ riderLocation.accuracy }}米
                    </p>
                  </div>
                </div>
                
                <div class="p-4 bg-green-50 dark:bg-green-900/20 rounded-lg">
                  <h4 class="font-semibold text-green-800 dark:text-green-200 mb-2 flex items-center gap-2">
                    <Icon icon="mdi:map-marker" />
                    订单位置
                  </h4>
                  <div class="space-y-1 text-sm">
                    <p class="text-gray-600 dark:text-gray-400">
                      地址: {{ orderDetail.order?.deliveryAddress || '-' }}
                    </p>
                    <p class="text-gray-600 dark:text-gray-400">
                      商家: {{ orderDetail.order?.merchantName || '-' }}
                    </p>
                    <p class="text-gray-600 dark:text-gray-400">
                      订单号: {{ orderDetail.order?.orderId || '-' }}
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </NTabPane>
        </NTabs>
      </div>

      <!-- 显示基本信息（当详情还在加载或没有详情数据时） -->
      <div v-else-if="!detailLoading && selectedOrder">
        <NDescriptions label-placement="left" bordered :column="1">
          <NDescriptionsItem label="订单号">
            {{ selectedOrder.order?.orderId || '-' }}
          </NDescriptionsItem>
          <NDescriptionsItem label="配送地址">
            {{ selectedOrder.order?.deliveryAddress || '-' }}
          </NDescriptionsItem>
          <NDescriptionsItem label="商家名称">
            {{ selectedOrder.order?.merchantName || '-' }}
          </NDescriptionsItem>
          <NDescriptionsItem label="订单金额">￥{{ selectedOrder.order?.orderAmount || 0 }}</NDescriptionsItem>
          <NDescriptionsItem label="创建时间">
            {{ selectedOrder.order?.createAt || '-' }}
          </NDescriptionsItem>
          <NDescriptionsItem label="订单状态">
            <NTag
              :type="(statusMap[(selectedOrder.acceptedStatus || 0) as keyof typeof statusMap]?.type || 'default') as any"
              size="small"
            >
              {{ statusMap[(selectedOrder.acceptedStatus || 0) as keyof typeof statusMap]?.text || '未知' }}
            </NTag>
          </NDescriptionsItem>
        </NDescriptions>
        <div class="mt-16px text-center text-gray-500">
          <p>详细分配信息加载中...</p>
        </div>
      </div>

      <!-- 加载状态 -->
      <div v-else-if="detailLoading" class="text-center py-32px">
        <p class="text-gray-500">正在加载订单详情...</p>
      </div>
    </NModal>
  </div>
</template>

<style scoped>
/* 页面整体动画 */
.min-h-full {
  animation: fadeIn 0.8s cubic-bezier(0.4, 0, 0.2, 1);
  min-height: 200vh;
  overflow-y: auto;
  position: relative;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(30px) scale(0.95);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

/* 统计卡片动画 */
.grid > .n-card {
  animation: slideUp 0.6s ease-out;
  animation-fill-mode: both;
}

.grid > .n-card:nth-child(1) { animation-delay: 0.1s; }
.grid > .n-card:nth-child(2) { animation-delay: 0.2s; }
.grid > .n-card:nth-child(3) { animation-delay: 0.3s; }
.grid > .n-card:nth-child(4) { animation-delay: 0.4s; }

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(40px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* 卡片样式增强 */
.n-card {
  background-color: var(--n-color);
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.n-card:hover {
  transform: translateY(-4px) scale(1.02);
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1), 0 0 0 1px rgba(255, 255, 255, 0.05);
}

/* 输入框样式增强 */
.n-input {
  transition: all 0.3s ease;
  border-radius: 12px;
}

.n-input:hover {
  border-color: var(--n-primary-color);
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.n-input:focus {
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.2);
}

/* 按钮样式增强 */
.n-button {
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
  border-radius: 12px;
  font-weight: 600;
  position: relative;
  overflow: hidden;
}

.n-button::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.2), transparent);
  transition: left 0.5s;
}

.n-button:hover::before {
  left: 100%;
}

.n-button:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.15);
}

.n-button:active {
  transform: translateY(0);
}

/* 确保按钮内容居中 */
.n-button .n-button__content {
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
  width: 100% !important;
  gap: 8px;
}

/* 更具体的按钮样式覆盖 */
.n-button .n-button__content .n-button__text {
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
}

/* 确保图标和文字都居中 */
.n-button .n-button__content .n-button__icon {
  margin-right: 4px !important;
}

/* 现代化表格样式 */
.modern-table {
  border-radius: 16px;
  overflow: hidden;
}

.modern-table .n-data-table-thead {
  background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
}

.modern-table .n-data-table-thead .n-data-table-th {
  background: transparent;
  border: none;
  font-weight: 600;
  color: #475569;
  padding: 16px 12px;
}

.modern-table .n-data-table-tbody .n-data-table-tr {
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  border-bottom: 1px solid rgba(0, 0, 0, 0.05);
}

.modern-table .n-data-table-tbody .n-data-table-tr:hover {
  background: linear-gradient(135deg, rgba(59, 130, 246, 0.05) 0%, rgba(147, 51, 234, 0.05) 100%);
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.1);
}

.modern-table .n-data-table-tbody .n-data-table-td {
  padding: 16px 12px;
  border: none;
  vertical-align: middle;
}

/* 状态指示器动画 */
@keyframes statusPulse {
  0%, 100% {
    opacity: 1;
    transform: scale(1);
  }
  50% {
    opacity: 0.7;
    transform: scale(1.1);
  }
}

.status-indicator {
  animation: statusPulse 2s infinite;
}

/* 渐变背景特殊效果 */
.bg-gradient-to-br {
  position: relative;
  overflow: hidden;
}

.bg-gradient-to-br::before {
  content: '';
  position: absolute;
  top: -50%;
  left: -50%;
  width: 200%;
  height: 200%;
  background: linear-gradient(45deg, transparent, rgba(255, 255, 255, 0.1), transparent);
  animation: shimmer 3s infinite;
}

@keyframes shimmer {
  0% {
    transform: translateX(-100%) translateY(-100%) rotate(45deg);
  }
  100% {
    transform: translateX(100%) translateY(100%) rotate(45deg);
  }
}

/* 自定义滚动条 */
::-webkit-scrollbar {
  width: 8px;
}

::-webkit-scrollbar-track {
  background: rgba(0, 0, 0, 0.05);
  border-radius: 4px;
}

::-webkit-scrollbar-thumb {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 4px;
}

::-webkit-scrollbar-thumb:hover {
  background: linear-gradient(135deg, #5a67d8 0%, #6b46c1 100%);
}

/* 暗色模式滚动条 */
.dark ::-webkit-scrollbar-track {
  background: rgba(255, 255, 255, 0.05);
}

.dark ::-webkit-scrollbar-thumb {
  background: linear-gradient(135deg, #4c51bf 0%, #553c9a 100%);
}

.dark ::-webkit-scrollbar-thumb:hover {
  background: linear-gradient(135deg, #434190 0%, #4c1d95 100%);
}

/* 响应式设计 */
@media (max-width: 768px) {
  .n-grid {
    grid-template-columns: 1fr !important;
  }

  .n-gi {
    grid-column: span 24 !important;
  }

  .flex.gap-24px {
    flex-direction: column;
    gap: 16px;
  }
}

/* 加载状态动画 */
.n-button[loading] {
  position: relative;
}

.n-button[loading]::after {
  content: '';
  position: absolute;
  width: 16px;
  height: 16px;
  border: 2px solid transparent;
  border-top: 2px solid currentColor;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* 模态框动画增强 */
.n-modal .n-card {
  animation: modalSlideIn 0.4s cubic-bezier(0.4, 0, 0.2, 1);
}

@keyframes modalSlideIn {
  from {
    opacity: 0;
    transform: translateY(-30px) scale(0.9);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

/* 详情卡片动画 */
.space-y-6 > div {
  animation: cardFadeIn 0.5s ease-out;
  animation-fill-mode: both;
}

.space-y-6 > div:nth-child(1) { animation-delay: 0.1s; }
.space-y-6 > div:nth-child(2) { animation-delay: 0.2s; }
.space-y-6 > div:nth-child(3) { animation-delay: 0.3s; }
.space-y-6 > div:nth-child(4) { animation-delay: 0.4s; }

@keyframes cardFadeIn {
  from {
    opacity: 0;
    transform: translateX(-20px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

/* 加载状态美化 */
.n-button[loading] {
  position: relative;
  overflow: hidden;
}

.n-button[loading]::after {
  content: '';
  position: absolute;
  top: 50%;
  left: 50%;
  width: 16px;
  height: 16px;
  margin: -8px 0 0 -8px;
  border: 2px solid transparent;
  border-top: 2px solid currentColor;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

/* 响应式优化 */
@media (max-width: 1024px) {
  .grid.grid-cols-4 {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 640px) {
  .grid.grid-cols-4 {
    grid-template-columns: 1fr;
  }
  
  .min-h-full {
    padding: 16px;
  }
  
  .n-card {
    margin-bottom: 16px;
  }
}
</style>