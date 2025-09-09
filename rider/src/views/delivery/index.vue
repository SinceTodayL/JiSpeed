<script setup lang="ts">
import { computed, h, onMounted, onUnmounted, ref } from 'vue';
import {
  NButton,
  NCard,
  NDataTable,
  NDescriptions,
  NDescriptionsItem,
  NForm,
  NFormItem,
  NInput,
  NModal,
  NSelect,
  NSpace,
  NTabPane,
  NTabs,
  NTag
} from 'naive-ui';
import { Icon } from '@iconify/vue';
import { getRiderOrderList, updateAssignStatus } from '@/service/api/rider';
import {
  getOnlineRidersLocation,
  getRiderCurrentAddress,
  getRiderLatestLocation,
  getRidersInArea,
  updateRiderOnlineStatus
} from '@/service/api/rider-location';
import {
  getNearbyServicePoints,
  planBasicRoute
} from '@/service/api/navigation';
import { type InputTipsResult } from '@/service/api/amap';
import { useAuthStore } from '@/store/modules/auth';
import { useRiderStore } from '@/store/modules/rider';
import { handleCommonError, handleOrderError } from '@/utils/rider-error-handler';
import AmapMap from '@/components/delivery/amap-map.vue';
import AddressSearch from '@/components/AddressSearch.vue';

const authStore = useAuthStore();
const riderStore = useRiderStore();

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
    address: {
      detailedAddress: string;
      receiverName: string;
      receiverPhone: string;
    };
    userId: string;
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

// 搜索和筛选
const searchForm = ref({
  orderId: '',
  status: undefined as 0 | 1 | 2 | 3 | undefined,
  startDate: null as string | null,
  endDate: null as string | null
});

const showModal = ref(false);
const selectedOrder = ref<OrderData | null>(null);
const orderDetail = ref<any>(null);
const detailLoading = ref(false);

// 地图相关状态
const getInitialLocation = () => {
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
};

const currentLocation = ref(getInitialLocation());
const areaRiders = ref<any[]>([]);
const mapInstance = ref<any>(null);
const amapMapRef = ref<any>(null);

// 新增位置相关状态
const currentAddress = ref<string>('');
const isOnline = ref<boolean>(true);

// 位置更新定时器
const locationUpdateTimer = ref<NodeJS.Timeout | null>(null);
const isLocationUpdating = ref<boolean>(false);

// 位置相关状态（简化版）

// 状态映射
const statusMap = {
  0: { text: '未处理', type: 'warning' },
  1: { text: '已接单', type: 'primary' },
  2: { text: '已拒单', type: 'error' },
  3: { text: '已完成', type: 'success' }
};

// 状态选项
const statusOptions = [
  { label: '全部状态', value: undefined },
  { label: '未处理', value: 0 },
  { label: '已接单', value: 1 },
  { label: '已拒单', value: 2 },
  { label: '已完成', value: 3 }
];

// 导航相关状态
const navigationState = ref({
  nearbyServicePoints: [] as Api.Navigation.ServicePoint[],
  currentRoute: null as Api.Navigation.NavigationRoute | null,
  isNavigating: false,
  navigationUpdate: null as Api.Navigation.RealTimeNavigationUpdate | null,
  currentStepIndex: 0,
  navigationSteps: [] as Api.Navigation.RouteStep[]
});

// 导航模态框状态
const showRoutePlanModal = ref(false);
const showServicePointsModal = ref(false);
const showNavigationModal = ref(false);
const showNavigationInstructions = ref(false);

// 路线规划表单
const routePlanForm = ref({
  startAddress: '',
  endAddress: '',
  routeType: 'driving' as 'driving' | 'walking' | 'cycling' | 'transit'
});

// 地址搜索相关状态
const startLocation = ref<{ longitude: number; latitude: number } | null>(null);
const endLocation = ref<{ longitude: number; latitude: number } | null>(null);
const startAddressSearch = ref('');
const endAddressSearch = ref('');

// 路线类型选项
const routeTypeOptions = [
  { label: '驾车', value: 'driving' },
  { label: '步行', value: 'walking' },
  { label: '骑行', value: 'cycling' },
  { label: '公共交通', value: 'transit' }
];

// 获取订单数据
const fetchOrders = async () => {
  loading.value = true;
  try {
    const params = {
      page: pagination.value.page,
      pageSize: pagination.value.pageSize,
      ...(searchForm.value.status !== undefined && { status: searchForm.value.status }),
      ...(searchForm.value.startDate && { startDate: searchForm.value.startDate }),
      ...(searchForm.value.endDate && { endDate: searchForm.value.endDate })
    };

    const { data } = await getRiderOrderList(riderId.value, params);
    console.log('data',data);
    if (Array.isArray(data)) {
      // 如果有搜索条件，进行本地筛选
      let filteredOrders = data as unknown as OrderData[];

      if (searchForm.value.orderId) {
        filteredOrders = filteredOrders.filter(order =>
          order.order?.orderId?.includes(searchForm.value.orderId)
        );
      }

      orders.value = filteredOrders;
      pagination.value.itemCount = filteredOrders.length;
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

    console.log('使用基本信息作为订单详情');
  } catch (error: any) {
    console.error('获取订单详情失败:', error);
    window.$message?.warning('订单详情功能暂时不可用，显示基本信息');
  } finally {
    detailLoading.value = false;
  }
};

// 获取GPS位置
const getCurrentGPSLocation = (): Promise<{ longitude: number; latitude: number }> => {
  return new Promise((resolve, reject) => {
    if (!navigator.geolocation) {
      reject(new Error('浏览器不支持GPS定位'));
      return;
    }

    navigator.geolocation.getCurrentPosition(
      (position) => {
        const location = {
          longitude: position.coords.longitude,
          latitude: position.coords.latitude
        };
        console.log('GPS定位成功:', location);
        resolve(location);
      },
      (error) => {
        console.error('GPS定位失败:', error);
        reject(error);
      },
      {
        enableHighAccuracy: true,
        timeout: 10000,
        maximumAge: 30000
      }
    );
  });
};

// 简化的位置获取逻辑

// 获取骑手位置信息（简化版）
const fetchRiderLocation = async () => {
  try {
    // 1. 先尝试获取GPS位置
    try {
      const gpsLocation = await getCurrentGPSLocation();
      
      // 2. 更新本地位置
      currentLocation.value = gpsLocation;
      
      // 3. 保存到localStorage
      try {
        localStorage.setItem('lastKnownLocation', JSON.stringify(gpsLocation));
      } catch (error) {
        console.warn('保存位置信息失败:', error);
      }
      
      console.log('GPS位置获取成功:', gpsLocation);
      return;
    } catch (gpsError) {
      console.warn('GPS定位失败，尝试从服务器获取位置:', gpsError);
    }

    // 4. 如果GPS失败，从服务器获取最新位置
    const { data } = await getRiderLatestLocation(riderId.value);
    console.log('服务器位置数据:', data);
    
    if (data && (data as any).longitude && (data as any).latitude) {
      const serverLocation = {
        longitude: (data as any).longitude,
        latitude: (data as any).latitude
      };
      currentLocation.value = serverLocation;

      // 保存到localStorage
      try {
        localStorage.setItem('lastKnownLocation', JSON.stringify(serverLocation));
      } catch (error) {
        console.warn('保存位置信息失败:', error);
      }

      console.log('使用服务器位置:', serverLocation);
    } else {
      console.log('服务器位置数据不完整，使用默认位置');
    }
  } catch (error) {
    console.error('获取骑手位置失败:', error);
    window.$message?.warning('位置获取失败，使用默认位置');
  }
};

// 获取区域内骑手数据
const fetchAreaRiders = async () => {
  try {
    window.$message?.loading('正在获取附近骑手...');
    
    // 使用区域查询API
    const { data } = await getRidersInArea({
      minLongitude: currentLocation.value.longitude - 0.1,
      maxLongitude: currentLocation.value.longitude + 0.1,
      minLatitude: currentLocation.value.latitude - 0.1,
      maxLatitude: currentLocation.value.latitude + 0.1,
      pageIndex: 1,
      pageSize: 100
    });

    if (data && Array.isArray(data)) {
      areaRiders.value = data;
      window.$message?.success(`找到 ${data.length} 个附近骑手`);
    } else {
      // 如果区域查询失败，回退到在线骑手查询
      const { data: onlineData } = await getOnlineRidersLocation({
        pageIndex: 1,
        pageSize: 100
      });

      if (onlineData && Array.isArray(onlineData)) {
        // 前端过滤：只显示距离当前位置10公里内的骑手
        const radius = 0.1; // 约10公里
        const filteredRiders = onlineData.filter((rider: any) => {
          const distance = Math.sqrt(
            (rider.longitude - currentLocation.value.longitude) ** 2 +
            (rider.latitude - currentLocation.value.latitude) ** 2
          );
          return distance <= radius;
        });

        areaRiders.value = filteredRiders;
        window.$message?.success(`找到 ${filteredRiders.length} 个附近骑手`);
      } else {
        areaRiders.value = [];
        window.$message?.warning('未找到附近骑手');
      }
    }
  } catch (error) {
    console.error('获取区域内骑手失败:', error);
    areaRiders.value = [];
    window.$message?.error('获取附近骑手失败');
  }
};

// 清除骑手标记
const clearRiderMarkers = () => {
  if (amapMapRef.value) {
    amapMapRef.value.clearRiderMarkers();
  } else {
    window.$message?.warning('地图组件未准备好');
  }
};

// 获取骑手当前位置地址
const fetchCurrentAddress = async () => {
  try {
    const { data } = await getRiderCurrentAddress(riderId.value);
    currentAddress.value = (data as any)?.address || '地址获取失败';
  } catch (error) {
    console.error('获取地址失败:', error);
    currentAddress.value = '地址获取失败';
  }
};


// 开始定时位置更新（简化版）
const startLocationUpdateTimer = () => {
  // 清除现有定时器
  if (locationUpdateTimer.value) {
    clearInterval(locationUpdateTimer.value);
  }

  // 设置新的定时器，每2分钟更新一次位置
  locationUpdateTimer.value = setInterval(async () => {
    if (isOnline.value && !isLocationUpdating.value) {
      isLocationUpdating.value = true;
      try {
        await fetchRiderLocation();
        console.log('定时位置更新完成');
      } catch (error) {
        console.error('定时位置更新失败:', error);
      } finally {
        isLocationUpdating.value = false;
      }
    }
  }, 120000); // 2分钟

  console.log('位置更新定时器已启动（2分钟间隔）');
};

// 停止定时位置更新
const stopLocationUpdateTimer = () => {
  if (locationUpdateTimer.value) {
    clearInterval(locationUpdateTimer.value);
    locationUpdateTimer.value = null;
    console.log('位置更新定时器已停止');
  }
};

// 更新在线状态
const updateOnlineStatus = async (status: boolean) => {
  try {
    await updateRiderOnlineStatus(riderId.value, {
      status: status ? 1 : 0,
      timestamp: new Date().toISOString()
    });
    isOnline.value = status;
    
    // 根据在线状态启动或停止位置更新
    if (status) {
      startLocationUpdateTimer();
      window.$message?.success('已上线，开始位置更新');
    } else {
      stopLocationUpdateTimer();
      window.$message?.success('已下线，停止位置更新');
    }
  } catch (error) {
    console.error('更新状态失败:', error);
    window.$message?.error('更新状态失败');
  }
};


// 搜索订单
const handleSearch = () => {
  pagination.value.page = 1;
  fetchOrders();
};

// 重置搜索
const resetSearch = () => {
  searchForm.value = {
    orderId: '',
    status: undefined,
    startDate: null,
    endDate: null
  };
  pagination.value.page = 1;
  fetchOrders();
};

// 地图相关事件处理
const handleMapReady = (map: any) => {
  mapInstance.value = map;
  console.log('地图加载完成');

  // 地图加载完成后，立即尝试获取骑手位置
  fetchRiderLocation();
};

const handleLocationUpdate = (location: { longitude: number; latitude: number }) => {
  currentLocation.value = location;
};

const handleRiderClick = (rider: any) => {
  console.log('点击骑手:', rider);
  // 可以在这里添加骑手详情弹窗等逻辑
};

// ========== 导航功能 ==========





// 规划基础路线
const planRoute = async (startLoc: { longitude: number; latitude: number }, endLoc: { longitude: number; latitude: number }) => {
  try {
    console.log('startloc',startLoc);
    console.log('endloc',endLoc);

    const requestData = {
      startLongitude: startLoc.longitude,
      startLatitude: startLoc.latitude,
      endLongitude: endLoc.longitude,
      endLatitude: endLoc.latitude,
      mode: routePlanForm.value.routeType
    };
    console.log('请求数据:', requestData);

    const response = await planBasicRoute(requestData);
    console.log('完整响应:', response);
    console.log('响应数据:', response.data);

    if (response.data?.data) {
      // 保存路线数据
      navigationState.value.currentRoute = response.data.data;
      navigationState.value.navigationSteps = response.data.data.steps || [];
      navigationState.value.currentStepIndex = 0;
      
      showRoutePlanModal.value = false;
      showNavigationModal.value = true;
      window.$message?.success('路线规划成功');
    } else {
      console.warn('响应数据为空');
      window.$message?.warning('路线规划返回空数据');
    }
  } catch (error: any) {
    console.error('路线规划失败:', error);
    console.error('错误详情:', {
      message: error.message,
      code: error.code,
      response: error.response?.data,
      status: error.response?.status,
      statusText: error.response?.statusText
    });
    window.$message?.error(`路线规划失败: ${error.message || '未知错误'}`);
  }
};

// 获取附近服务点
const getNearbyServices = async () => {
  try {
    const { data } = await getNearbyServicePoints({
      longitude: currentLocation.value.longitude,
      latitude: currentLocation.value.latitude,
      serviceType: 'all',
      radius: 2000 // 2公里范围内
    });

    if (data) {
      navigationState.value.nearbyServicePoints = data.data;
      showServicePointsModal.value = true;
      window.$message?.success(`找到 ${data.data.length} 个附近服务点`);
    }
  } catch (error: any) {
    console.error('获取附近服务点失败:', error);
    window.$message?.error(`获取附近服务点失败: ${error.message || '未知错误'}`);
  }
};

// 格式化距离
const formatDistance = (distance: number) => {
  if (distance < 1000) {
    return `${distance}米`;
  }
  return `${(distance / 1000).toFixed(1)}公里`;
};

// 格式化时间
const formatDuration = (duration: number) => {
  const hours = Math.floor(duration / 3600);
  const minutes = Math.floor((duration % 3600) / 60);

  if (hours > 0) {
    return `${hours}小时${minutes}分钟`;
  }
  return `${minutes}分钟`;
};

// 处理起点地址选择
const handleStartAddressSelect = async (suggestion: InputTipsResult) => {
  try {
    const [longitude, latitude] = suggestion.location.split(',').map(Number);
    startLocation.value = { longitude, latitude };
    startAddressSearch.value = suggestion.name;
    window.$message?.success(`已选择起点: ${suggestion.name}`);
  } catch (error) {
    console.error('处理起点地址失败:', error);
    window.$message?.error('处理起点地址失败');
  }
};

// 处理终点地址选择
const handleEndAddressSelect = async (suggestion: InputTipsResult) => {
  try {
    const [longitude, latitude] = suggestion.location.split(',').map(Number);
    endLocation.value = { longitude, latitude };
    endAddressSearch.value = suggestion.name;
    window.$message?.success(`已选择终点: ${suggestion.name}`);
  } catch (error) {
    console.error('处理终点地址失败:', error);
    window.$message?.error('处理终点地址失败');
  }
};

// 处理路线规划
const handleRoutePlan = async () => {
  if (!startLocation.value || !endLocation.value) {
    window.$message?.warning('请选择起点和终点地址');
    return;
  }
  try {
    await planRoute(startLocation.value, endLocation.value);
  } catch (error: any) {
    console.error('路线规划失败:', error);
    window.$message?.error(`路线规划失败: ${error.message || '未知错误'}`);
  }
};

// 重置地址搜索状态
const resetAddressSearch = () => {
  startLocation.value = null;
  endLocation.value = null;
  startAddressSearch.value = '';
  endAddressSearch.value = '';
};

// ========== 导航控制函数 ==========

// 开始导航
const startNavigation = () => {
  if (!navigationState.value.currentRoute) {
    window.$message?.warning('请先规划路线');
    return;
  }
  
  navigationState.value.isNavigating = true;
  showNavigationModal.value = false;
  showNavigationInstructions.value = true;
  
  window.$message?.success('导航已开始');
  console.log('开始导航:', navigationState.value.currentRoute);
};

// 结束导航
const endNavigation = () => {
  navigationState.value.isNavigating = false;
  navigationState.value.currentRoute = null;
  navigationState.value.navigationUpdate = null;
  navigationState.value.currentStepIndex = 0;
  navigationState.value.navigationSteps = [];
  showNavigationInstructions.value = false;
  
  window.$message?.success('导航已结束');
  console.log('结束导航');
};

// 重新规划路线
const replanRoute = () => {
  if (!startLocation.value || !endLocation.value) {
    window.$message?.warning('请重新选择起点和终点');
    showRoutePlanModal.value = true;
    return;
  }
  
  planRoute(startLocation.value, endLocation.value);
};

// 处理导航更新
const handleNavigationUpdate = (update: Api.Navigation.RealTimeNavigationUpdate) => {
  navigationState.value.navigationUpdate = update;
  console.log('导航更新:', update);
};

// 处理导航开始事件
const handleNavigationStart = () => {
  console.log('地图导航已开始');
};

// 处理导航结束事件
const handleNavigationEnd = () => {
  console.log('地图导航已结束');
  endNavigation();
};

// ========== 订单配送导航 ==========

// 为订单规划导航路线
const planOrderNavigation = async (order: OrderData) => {
  if (!order.order?.address) {
    window.$message?.warning('订单地址信息不完整');
    return;
  }

  try {
    // 使用当前骑手位置作为起点
    const startPos = currentLocation.value;
    
    // 这里需要将订单地址转换为坐标
    // 实际应用中应该调用地理编码API
    const endPos = {
      longitude: 116.397428 + Math.random() * 0.01, // 模拟订单地址坐标
      latitude: 39.90923 + Math.random() * 0.01
    };

    // 设置起点和终点
    startLocation.value = startPos;
    endLocation.value = endPos;
    startAddressSearch.value = '当前位置';
    endAddressSearch.value = order.order.address.detailedAddress;

    // 规划路线
    await planRoute(startPos, endPos);
    
    // 设置当前订单
    selectedOrder.value = order;
    
    window.$message?.success('已为订单规划导航路线');
  } catch (error) {
    console.error('订单导航规划失败:', error);
    window.$message?.error('订单导航规划失败');
  }
};

// 快速开始订单导航
const quickStartOrderNavigation = async (order: OrderData) => {
  await planOrderNavigation(order);
  if (navigationState.value.currentRoute) {
    startNavigation();
  }
};

// 获取服务点类型文本
const getServiceTypeText = (type: string) => {
  const typeMap: Record<string, string> = {
    gas_station: '加油站',
    restaurant: '餐厅',
    hospital: '医院',
    police: '警察局',
    other: '其他'
  };
  return typeMap[type] || '未知';
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
  // 优先获取骑手位置，确保地图显示正确位置
  await fetchRiderLocation();
  // 获取地址信息
  await fetchCurrentAddress();
  
  // 启动定时位置更新（如果在线）
  if (isOnline.value) {
    startLocationUpdateTimer();
  }
});

// 处理分页变化
const handlePageChange = (page: number) => {
  pagination.value.page = page;
  fetchOrders();
};

// 组件卸载时清理定时器
onUnmounted(() => {
  stopLocationUpdateTimer();
  // 清理地图实例
  if (mapInstance.value) {
    mapInstance.value = null;
  }
});

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
      2: '拒单',
      3: '完成订单'
    }[newStatus] || '更新状态';

  try {
    window.$dialog?.warning({
      title: `确认${statusText}`,
      content: `确定要${statusText}订单 ${order.order?.orderId} 吗？`,
      positiveText: '确定',
      negativeText: '取消',
      onPositiveClick: async () => {
        try {
          // 使用assignId
          const assignId = order.assignId || '';
          await updateAssignStatus(riderId.value, assignId, { acceptedStatus: newStatus });
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
</script>

<template>
  <div class="min-h-full p-24px bg-gradient-to-br from-blue-50 to-indigo-100 dark:from-gray-900 dark:to-gray-800">
    <!-- 页面标题区域 -->
    <NCard :bordered="false" class="mb-24px bg-white/80 dark:bg-gray-800/80 backdrop-blur-sm">
      <div class="flex items-center gap-3">
        <div class="w-12 h-12 bg-gradient-to-r from-blue-500 to-purple-600 rounded-xl flex items-center justify-center">
          <Icon icon="mdi:motorbike" class="text-2xl text-white" />
        </div>
        <div>
          <h1 class="text-2xl text-gray-800 font-bold dark:text-gray-200">配送订单管理</h1>
          <p class="mt-2px text-gray-600 dark:text-gray-400">查看和管理您的配送订单，及时处理订单状态</p>
        </div>
      </div>
    </NCard>

    <!-- 搜索和筛选表单 -->
    <NCard :bordered="false" class="mb-24px bg-white/90 dark:bg-gray-800/90 backdrop-blur-sm rounded-16px shadow-lg">
      <NForm :model="searchForm" inline>
        <NFormItem label="订单号">
          <NInput v-model:value="searchForm.orderId" placeholder="请输入订单号" style="width: 200px" />
        </NFormItem>
        <NFormItem label="状态">
          <NSelect v-model:value="searchForm.status" :options="statusOptions" placeholder="请选择状态" style="width: 150px" />
        </NFormItem>
                 <NFormItem label="创建时间">
           <NSpace>
             <NInput v-model:value="searchForm.startDate" placeholder="开始日期 (YYYY-MM-DD)" style="width: 150px" />
             <span>至</span>
             <NInput v-model:value="searchForm.endDate" placeholder="结束日期 (YYYY-MM-DD)" style="width: 150px" />
           </NSpace>
         </NFormItem>
        <NFormItem>
          <NSpace>
            <NButton type="primary" @click="handleSearch">搜索</NButton>
            <NButton @click="resetSearch">重置</NButton>
          </NSpace>
        </NFormItem>
      </NForm>
    </NCard>

    <!-- 使用标签页分离地图和订单列表 -->
    <NTabs type="line" animated>
      <NTabPane name="orders" tab="订单列表">
        <NCard :bordered="false" class="rounded-16px shadow-lg bg-white/90 dark:bg-gray-800/90 backdrop-blur-sm">
          <NDataTable
            :columns="[
              {
                title: '订单号',
                key: 'orderId',
                render: (row: OrderData) => row.order?.orderId || '-'
              },
              {
                title: '配送地址',
                key: 'address',
                render: (row: OrderData) => row.order?.address?.detailedAddress || '-'
              },
              {
                title: '商家名称',
                key: 'merchant',
                render: (row: OrderData) => row.order?.address?.receiverName || '-'
              },
              {
                title: '状态',
                key: 'status',
                render(row: OrderData) {
                  const status = row.acceptedStatus || 0;
                  const statusInfo = statusMap[status as keyof typeof statusMap] || { text: '未知', type: 'default' };
                  return h(NTag, { type: statusInfo.type as any, size: 'small' }, { default: () => statusInfo.text });
                }
              },
              {
                title: '金额',
                key: 'amount',
                render: (row: OrderData) => `￥${row.order?.orderAmount || 0}`
              },
              {
                title: '创建时间',
                key: 'createAt',
                render: (row: OrderData) => row.order?.createAt || '-'
              },
              {
                title: '操作',
                key: 'actions',
                render(row: OrderData) {
                  const currentStatus = row.acceptedStatus || 0;
                  const buttons: any[] = [];

                  // 详情按钮
                  buttons.push(
                    h(
                      NButton,
                      { size: 'small', type: 'primary', onClick: () => handleDetail(row) },
                      { default: () => '详情' }
                    )
                  );

                  // 根据当前状态显示不同的操作按钮
                  if (currentStatus === 0) {
                    // 未处理状态：显示接单和拒单按钮
                    buttons.push(
                      h(
                        NButton,
                        { size: 'small', type: 'success', onClick: () => handleStatusUpdate(row, 1) },
                        { default: () => '接单' }
                      ),
                      h(
                        NButton,
                        { size: 'small', type: 'warning', onClick: () => handleStatusUpdate(row, 2) },
                        { default: () => '拒单' }
                      )
                    );
                  } else if (currentStatus === 1) {
                    // 已接单状态：显示导航和完成按钮
                    buttons.push(
                      h(
                        NButton,
                        { size: 'small', type: 'info', onClick: () => planOrderNavigation(row) },
                        { default: () => '导航' }
                      ),
                      h(
                        NButton,
                        { size: 'small', type: 'success', onClick: () => handleStatusUpdate(row, 3) },
                        { default: () => '完成' }
                      )
                    );
                  }


                  return h(NSpace, null, {
                    default: () => buttons
                  });
                }
              }
            ]"
            :data="paginatedOrders"
            :loading="loading"
            :pagination="pagination"
            @update:page="handlePageChange"
          />
        </NCard>
      </NTabPane>

      <NTabPane name="map" tab="配送地图">
        <NCard :bordered="false" class="rounded-16px shadow-lg bg-white/90 dark:bg-gray-800/90 backdrop-blur-sm">
          <div class="mb-16px flex items-center justify-between">
            <div>
              <h3 class="text-lg font-semibold mb-8px">实时配送地图</h3>
              <p class="text-gray-600 text-sm">查看骑手位置和配送路线</p>
            </div>
            <NSpace>
              <NButton type="success" @click="fetchRiderLocation()" :loading="isLocationUpdating">
                <Icon icon="mdi:refresh" class="mr-1" />
                刷新位置
              </NButton>
              <NButton :type="isOnline ? 'success' : 'warning'" @click="updateOnlineStatus(!isOnline)">
                <Icon :icon="isOnline ? 'mdi:account-check' : 'mdi:account-off'" class="mr-1" />
                {{ isOnline ? '下线' : '上线' }}
              </NButton>
              <NButton type="primary" @click="fetchAreaRiders">
                <Icon icon="mdi:account-group" class="mr-1" />
                附近骑手
              </NButton>
              <NButton type="error" @click="clearRiderMarkers">
                <Icon icon="mdi:map-marker-off" class="mr-1" />
                清除骑手标记
              </NButton>
              <NButton type="info" @click="getNearbyServices()">
                <Icon icon="mdi:map-marker-multiple" class="mr-1" />
                附近服务
              </NButton>
              <NButton type="warning" @click="showRoutePlanModal = true">
                <Icon icon="mdi:route" class="mr-1" />
                路线规划
              </NButton>
              <NButton 
                v-if="navigationState.currentRoute && !navigationState.isNavigating" 
                type="success" 
                @click="startNavigation"
              >
                <Icon icon="mdi:navigation" class="mr-1" />
                开始导航
              </NButton>
              <NButton 
                v-if="navigationState.isNavigating" 
                type="error" 
                @click="endNavigation"
              >
                <Icon icon="mdi:stop" class="mr-1" />
                结束导航
              </NButton>
              <NButton 
                v-if="navigationState.currentRoute" 
                type="info" 
                @click="replanRoute"
              >
                <Icon icon="mdi:refresh" class="mr-1" />
                重新规划
              </NButton>
            </NSpace>
          </div>

          <!-- 高德地图组件 -->
          <AmapMap
            ref="amapMapRef"
            :center="currentLocation"
            :zoom="12"
            :show-riders="true"
            :navigation-route="navigationState.currentRoute"
            :is-navigating="navigationState.isNavigating"
            :current-order="selectedOrder"
            @map-ready="handleMapReady"
            @location-update="handleLocationUpdate"
            @rider-click="handleRiderClick"
            @navigation-start="handleNavigationStart"
            @navigation-end="handleNavigationEnd"
            @navigation-update="handleNavigationUpdate"
          />

          <!-- 地图信息面板 -->
          <div class="mt-16px grid grid-cols-4 gap-16px">
            <NCard size="small" class="text-center">
              <div class="text-2xl font-bold text-blue-600">{{ currentLocation.longitude.toFixed(6) }}</div>
              <div class="text-sm text-gray-500">经度</div>
            </NCard>
            <NCard size="small" class="text-center">
              <div class="text-2xl font-bold text-green-600">{{ currentLocation.latitude.toFixed(6) }}</div>
              <div class="text-sm text-gray-500">纬度</div>
            </NCard>
            <NCard size="small" class="text-center">
              <div class="text-2xl font-bold text-purple-600">{{ areaRiders.length }}</div>
              <div class="text-sm text-gray-500">区域内骑手</div>
              <div v-if="areaRiders.length === 0" class="text-xs text-gray-400 mt-1">点击"附近骑手"获取</div>
            </NCard>
            <NCard size="small" class="text-center">
              <div class="text-lg font-bold" :class="isOnline ? 'text-green-600' : 'text-gray-500'">
                {{ isOnline ? '在线' : '离线' }}
              </div>
              <div class="text-sm text-gray-500">
                {{ isLocationUpdating ? '更新中...' : '状态' }}
              </div>
            </NCard>
          </div>

          <!-- 地址信息 -->
          <div class="mt-16px">
            <NCard size="small">
              <div class="flex items-center gap-2">
                <Icon icon="mdi:map-marker" class="text-red-500" />
                <span class="text-sm text-gray-600">当前位置：</span>
                <span class="text-sm font-medium">{{ currentAddress || '获取中...' }}</span>
              </div>
            </NCard>
          </div>
        </NCard>
      </NTabPane>
    </NTabs>

    <!-- 订单详情弹窗 -->
    <NModal v-model:show="showModal" preset="card" title="订单详情" class="w-[600px]" :loading="detailLoading">
      <div v-if="!detailLoading && orderDetail">
        <NDescriptions label-placement="left" bordered :column="1">
          <NDescriptionsItem label="订单号">
            {{ orderDetail.order?.orderId || '-' }}
          </NDescriptionsItem>
          <NDescriptionsItem label="配送地址">
            {{ orderDetail.order?.address?.detailedAddress || '-' }}
          </NDescriptionsItem>
          <NDescriptionsItem label="收货人姓名">
            {{ orderDetail.order?.address?.receiverName || '-' }}
          </NDescriptionsItem>
          <NDescriptionsItem label="收货人电话">
            {{ orderDetail.order?.address?.receiverPhone || '-' }}
          </NDescriptionsItem>
          <NDescriptionsItem label="分配时间">
            {{ orderDetail.assignedAt || '-' }}
          </NDescriptionsItem>
          <NDescriptionsItem label="接单时间">
            {{ orderDetail.acceptedAt || '-' }}
          </NDescriptionsItem>
          <NDescriptionsItem label="订单金额">￥{{ orderDetail.order?.orderAmount || 0 }}</NDescriptionsItem>
          <NDescriptionsItem label="订单状态">
            <NTag
              :type="(statusMap[(orderDetail.acceptedStatus || 0) as keyof typeof statusMap]?.type || 'default') as any"
              size="small"
            >
              {{ statusMap[(orderDetail.acceptedStatus || 0) as keyof typeof statusMap]?.text || '未知' }}
            </NTag>
          </NDescriptionsItem>
          <NDescriptionsItem label="创建时间">
            {{ orderDetail.order?.createAt || '-' }}
          </NDescriptionsItem>
          <NDescriptionsItem label="超时时间">
            {{ orderDetail.timeOut || 0 }} 分钟
          </NDescriptionsItem>
        </NDescriptions>
      </div>

      <!-- 显示基本信息（当详情还在加载或没有详情数据时） -->
      <div v-else-if="!detailLoading && selectedOrder">
        <NDescriptions label-placement="left" bordered :column="1">
          <NDescriptionsItem label="订单号">
            {{ selectedOrder.order?.orderId || '-' }}
          </NDescriptionsItem>
          <NDescriptionsItem label="配送地址">
            {{ selectedOrder.order?.address?.detailedAddress || '-' }}
          </NDescriptionsItem>
          <NDescriptionsItem label="收货人姓名">
            {{ selectedOrder.order?.address?.receiverName || '-' }}
          </NDescriptionsItem>
          <NDescriptionsItem label="收货人电话">
            {{ selectedOrder.order?.address?.receiverPhone || '-' }}
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



    <!-- 路线规划弹窗 -->
    <NModal v-model:show="showRoutePlanModal" preset="card" title="路线规划" class="w-[700px]">
      <div class="space-y-16px">
        <NForm label-placement="left" :label-width="80">
          <NFormItem label="起点">
            <AddressSearch
              v-model:model-value="startAddressSearch"
              placeholder="请输入起点地址"
              city="北京"
              :location="`${currentLocation.longitude},${currentLocation.latitude}`"
              @select="handleStartAddressSelect"
            />
          </NFormItem>
          <NFormItem label="终点">
            <AddressSearch
              v-model:model-value="endAddressSearch"
              placeholder="请输入终点地址"
              city="北京"
              :location="`${currentLocation.longitude},${currentLocation.latitude}`"
              @select="handleEndAddressSelect"
            />
          </NFormItem>
          <NFormItem label="路线类型">
            <NSelect v-model:value="routePlanForm.routeType" :options="routeTypeOptions" />
          </NFormItem>
        </NForm>

        <!-- 显示已选择的地址信息 -->
        <div v-if="startLocation || endLocation" class="bg-gray-50 p-12px rounded-lg">
          <h4 class="text-sm font-medium mb-8px text-gray-700">已选择地址：</h4>
          <div v-if="startLocation" class="text-sm text-gray-600 mb-4px">
            <span class="font-medium">起点：</span>{{ startAddressSearch }}
            <span class="text-xs text-gray-500 ml-8px">
              ({{ startLocation.longitude.toFixed(6) }}, {{ startLocation.latitude.toFixed(6) }})
            </span>
          </div>
          <div v-if="endLocation" class="text-sm text-gray-600">
            <span class="font-medium">终点：</span>{{ endAddressSearch }}
            <span class="text-xs text-gray-500 ml-8px">
              ({{ endLocation.longitude.toFixed(6) }}, {{ endLocation.latitude.toFixed(6) }})
            </span>
          </div>
        </div>

        <div class="flex justify-end space-x-8px">
          <NButton @click="showRoutePlanModal = false; resetAddressSearch()">取消</NButton>
          <NButton type="primary" @click="handleRoutePlan" :disabled="!startLocation || !endLocation">
            规划路线
          </NButton>
        </div>
      </div>
    </NModal>

    <!-- 附近服务点弹窗 -->
    <NModal v-model:show="showServicePointsModal" preset="card" title="附近服务点" class="w-[800px]">
      <div v-if="navigationState.nearbyServicePoints.length > 0">
        <NDataTable
          :columns="[
            { title: '名称', key: 'name', width: 150 },
            { title: '类型', key: 'type', width: 100, render: (row: Api.Navigation.ServicePoint) => getServiceTypeText(row.type || '') },
            { title: '地址', key: 'address', width: 200 },
            { title: '距离', key: 'distance', width: 100, render: (row: Api.Navigation.ServicePoint) => formatDistance(row.distance || 0) },
            { title: '状态', key: 'isOpen', width: 80, render: (row: Api.Navigation.ServicePoint) => h(NTag, { type: (row as any).isOpen ? 'success' : 'error' }, { default: () => (row as any).isOpen ? '营业' : '休息' }) },
            { title: '评分', key: 'rating', width: 80, render: (row: Api.Navigation.ServicePoint) => (row as any).rating ? `${(row as any).rating}分` : '-' },
            { title: '电话', key: 'phone', width: 120 }
          ]"
          :data="navigationState.nearbyServicePoints"
          :pagination="{ pageSize: 10 }"
        />
      </div>
      <div v-else class="text-center py-32px text-gray-500">
        暂无附近服务点
      </div>
    </NModal>

    <!-- 导航确认弹窗 -->
    <NModal v-model:show="showNavigationModal" preset="card" title="导航路线确认" class="w-[600px]">
      <div v-if="navigationState.currentRoute" class="space-y-16px">
        <!-- 路线信息 -->
        <div class="bg-blue-50 p-16px rounded-lg">
          <h4 class="text-lg font-semibold mb-12px text-blue-800">路线信息</h4>
          <div class="grid grid-cols-2 gap-16px">
            <div class="text-center">
              <div class="text-2xl font-bold text-blue-600">{{ formatDistance(navigationState.currentRoute.totalDistance) }}</div>
              <div class="text-sm text-gray-600">总距离</div>
            </div>
            <div class="text-center">
              <div class="text-2xl font-bold text-green-600">{{ formatDuration(navigationState.currentRoute.estimatedDuration) }}</div>
              <div class="text-sm text-gray-600">预计时间</div>
            </div>
          </div>
        </div>

        <!-- 路线步骤预览 -->
        <div v-if="navigationState.navigationSteps.length > 0">
          <h4 class="text-lg font-semibold mb-12px">路线步骤</h4>
          <div class="max-h-200px overflow-y-auto space-y-8px">
            <div 
              v-for="(step, index) in navigationState.navigationSteps.slice(0, 5)" 
              :key="index"
              class="flex items-center p-8px bg-gray-50 rounded-lg"
            >
              <div class="w-6 h-6 bg-blue-500 text-white rounded-full flex items-center justify-center text-xs font-bold mr-12px">
                {{ index + 1 }}
              </div>
              <div class="flex-1">
                <div class="text-sm font-medium">{{ step.instruction || '继续直行' }}</div>
                <div class="text-xs text-gray-500">
                  {{ formatDistance(step.distance || 0) }} · {{ formatDuration(step.duration || 0) }}
                </div>
              </div>
            </div>
            <div v-if="navigationState.navigationSteps.length > 5" class="text-center text-gray-500 text-sm">
              还有 {{ navigationState.navigationSteps.length - 5 }} 个步骤...
            </div>
          </div>
        </div>

        <!-- 操作按钮 -->
        <div class="flex justify-end space-x-8px">
          <NButton @click="showNavigationModal = false">取消</NButton>
          <NButton type="primary" @click="startNavigation">
            <Icon icon="mdi:navigation" class="mr-1" />
            开始导航
          </NButton>
        </div>
      </div>
    </NModal>

    <!-- 导航指令弹窗 -->
    <NModal v-model:show="showNavigationInstructions" preset="card" title="导航中" class="w-[400px]">
      <div v-if="navigationState.navigationUpdate" class="space-y-16px">
        <!-- 当前指令 -->
        <div class="bg-blue-50 p-16px rounded-lg text-center">
          <div class="text-lg font-semibold text-blue-800 mb-8px">当前指令</div>
          <div class="text-xl font-bold">{{ navigationState.navigationUpdate.nextInstruction }}</div>
        </div>

        <!-- 导航信息 -->
        <div class="grid grid-cols-2 gap-16px">
          <div class="text-center">
            <div class="text-2xl font-bold text-red-600">{{ formatDistance(navigationState.navigationUpdate.remainingDistance) }}</div>
            <div class="text-sm text-gray-600">剩余距离</div>
          </div>
          <div class="text-center">
            <div class="text-2xl font-bold text-orange-600">{{ formatDuration(navigationState.navigationUpdate.remainingTime) }}</div>
            <div class="text-sm text-gray-600">预计时间</div>
          </div>
        </div>

        <!-- 当前速度 -->
        <div class="text-center">
          <div class="text-lg font-semibold">当前速度: {{ navigationState.navigationUpdate.currentSpeed }} km/h</div>
        </div>

        <!-- 操作按钮 -->
        <div class="flex justify-center space-x-8px">
          <NButton type="error" @click="endNavigation">
            <Icon icon="mdi:stop" class="mr-1" />
            结束导航
          </NButton>
        </div>
      </div>
    </NModal>
  </div>
</template>

<style scoped>
/* 页面整体动画 */
.min-h-full {
  animation: fadeIn 0.6s ease-out;
  min-height: 125vh;
  overflow-y: auto;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(20px);
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

/* 表格行悬停效果 */
.n-data-table .n-data-table-tbody .n-data-table-tr:hover {
  background-color: rgba(59, 130, 246, 0.05);
  transform: scale(1.01);
  transition: all 0.2s ease;
}

/* 标签页动画 */
.n-tabs .n-tabs-tab {
  transition: all 0.3s ease;
  border-radius: 8px;
}

.n-tabs .n-tabs-tab:hover {
  background-color: rgba(59, 130, 246, 0.1);
  transform: translateY(-1px);
}

/* 地图信息卡片动画 */
.grid > div {
  transition: all 0.3s ease;
  cursor: pointer;
}

.grid > div:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.1);
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
  animation: modalSlideIn 0.3s ease-out;
}

@keyframes modalSlideIn {
  from {
    opacity: 0;
    transform: translateY(-20px) scale(0.95);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}
</style>
