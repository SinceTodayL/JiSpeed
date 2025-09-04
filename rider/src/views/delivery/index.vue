<script setup lang="ts">
import { computed, h, onMounted, ref } from 'vue';
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
  NTag,
  NTabs,
  NTabPane
} from 'naive-ui';
import { getRiderOrderList, updateAssignStatus } from '@/service/api/rider';
import { getOnlineRidersLocation, getRiderLatestLocation } from '@/service/api/rider-location';
import { useAuthStore } from '@/store/modules/auth';
import { useRiderStore } from '@/store/modules/rider';
import { handleCommonError, handleOrderError } from '@/utils/rider-error-handler';
import AmapMap from '@/components/delivery/amap-map.vue';

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

// 获取骑手位置信息
const fetchRiderLocation = async () => {
  try {
    const { data } = await getRiderLatestLocation(riderId.value);
    console.log('位置',data);
    if (data) {
      console.log('if');
      const newLocation = {
        longitude: (data as any).longitude,
        latitude: (data as any).latitude
      };
      currentLocation.value = newLocation;

      // 保存到localStorage
      try {
        localStorage.setItem('lastKnownLocation', JSON.stringify(newLocation));
      } catch (error) {
        console.warn('保存位置信息失败:', error);
      }

      console.log('获取到骑手位置:', newLocation);
    } else {
      console.log('骑手位置数据不完整，使用默认位置');
    }
  } catch (error) {
    console.log('获取骑手位置失败，使用默认位置:', error);
  }
};

// 获取区域内骑手数据
const fetchAreaRiders = async () => {
  try {
    // 暂时使用获取所有在线骑手API，避免后端区域查询错误
    const { data } = await getOnlineRidersLocation({
      pageIndex: 1,
      pageSize: 100
    });

    if (data && Array.isArray(data)) {
      // 前端过滤：只显示距离当前位置10公里内的骑手
      const radius = 0.1; // 约10公里
      const filteredRiders = data.filter((rider: any) => {
        const distance = Math.sqrt(
          (rider.longitude - currentLocation.value.longitude) ** 2 +
          (rider.latitude - currentLocation.value.latitude) ** 2
        );
        return distance <= radius;
      });

      areaRiders.value = filteredRiders;
    } else {
      areaRiders.value = [];
    }
  } catch (error) {
    console.error('获取在线骑手失败:', error);
    areaRiders.value = [];
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
  await fetchAreaRiders();
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
  <div class="h-full p-24px">
    <!-- 页面标题区域 -->
    <NCard :bordered="false" class="mb-24px">
      <div class="flex items-center justify-between">
        <div class="flex-1">
          <h1 class="text-2xl text-gray-800 font-bold dark:text-gray-200">配送订单管理</h1>
          <p class="mt-8px text-gray-600 dark:text-gray-400">查看和管理您的配送订单，及时处理订单状态。</p>
        </div>
      </div>
    </NCard>

    <!-- 搜索和筛选表单 -->
    <NCard :bordered="false" class="mb-24px">
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
        <NCard :bordered="false" class="rounded-16px shadow-sm">
          <NDataTable
            :columns="[
              {
                title: '订单号',
                key: 'orderId',
                render: row => row.order?.orderId || '-'
              },
              {
                title: '配送地址',
                key: 'address',
                render: row => row.order?.address?.detailedAddress || '-'
              },
              {
                title: '商家名称',
                key: 'merchant',
                render: row => row.order?.address?.receiverName || '-'
              },
              {
                title: '状态',
                key: 'status',
                render(row) {
                  const status = row.acceptedStatus || 0;
                  const statusInfo = statusMap[status as keyof typeof statusMap] || { text: '未知', type: 'default' };
                  return h(NTag, { type: statusInfo.type as any, size: 'small' }, { default: () => statusInfo.text });
                }
              },
              {
                title: '金额',
                key: 'amount',
                render: row => `￥${row.order?.orderAmount || 0}`
              },
              {
                title: '创建时间',
                key: 'createAt',
                render: row => row.order?.createAt || '-'
              },
              {
                title: '操作',
                key: 'actions',
                render(row) {
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
                    // 已接单状态：显示完成按钮
                    buttons.push(
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
        <NCard :bordered="false" class="rounded-16px shadow-sm">
          <div class="mb-16px flex items-center justify-between">
            <div>
              <h3 class="text-lg font-semibold mb-8px">实时配送地图</h3>
              <p class="text-gray-600 text-sm">查看骑手位置和配送路线</p>
            </div>

          </div>

          <!-- 高德地图组件 -->
          <AmapMap
            :center="currentLocation"
            :zoom="12"
            :show-riders="true"
            @map-ready="handleMapReady"
            @location-update="handleLocationUpdate"
            @rider-click="handleRiderClick"
          />

          <!-- 地图信息面板 -->
          <div class="mt-16px grid grid-cols-3 gap-16px">
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
  </div>
</template>

<style scoped>
.n-card {
  background-color: var(--n-color);
  transition: all 0.3s ease;
}

.n-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.1);
}

.n-button {
  transition: all 0.3s ease;
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
}

.n-button:hover {
  transform: translateY(-1px);
}

/* 确保按钮内容居中 */
.n-button .n-button__content {
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
  width: 100% !important;
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
</style>
