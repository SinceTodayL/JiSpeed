<script setup lang="ts">
import { computed, h, onMounted, ref } from 'vue';
import {
  NButton,
  NCard,
  NDataTable,
  NDescriptions,
  NDescriptionsItem,
  NModal,
  NSpace,
  NTag
} from 'naive-ui';
import { Icon } from '@iconify/vue';
import { getRiderOrderList } from '@/service/api/rider';
import { acceptOrder, rejectOrder, updateOrderStatus } from '@/service/api/order-assignment';
import { useAuthStore } from '@/store/modules/auth';
import { useRiderStore } from '@/store/modules/rider';
import { handleCommonError, handleOrderError } from '@/utils/rider-error-handler';

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

// 状态映射 - 根据后端定义：0=待接单, 1=已接单, 2=已拒绝, 3=已完成
const statusMap = {
  0: { text: '待接单', type: 'warning' },
  1: { text: '已接单', type: 'primary' },
  2: { text: '已拒绝', type: 'error' },
  3: { text: '已完成', type: 'success' }
};


// 获取订单数据
const fetchOrders = async () => {
  loading.value = true;
  try {
    const { data } = await getRiderOrderList(riderId.value, {});
    console.log('data', data);
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
          if (newStatus === 1) {
            // 接单
            await acceptOrder({
              orderId: order.order?.orderId || '',
              riderId: riderId.value
            });
          } else if (newStatus === 2) {
            // 拒绝订单
            await rejectOrder({
              orderId: order.order?.orderId || '',
              riderId: riderId.value,
              reason: '骑手主动拒绝'
            });
          } else {
            // 其他状态更新
            await updateOrderStatus({
              orderId: order.order?.orderId || '',
              newStatus: newStatus
            });
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

    <!-- 操作区域 -->
    <NCard :bordered="false" class="mb-24px bg-white/90 dark:bg-gray-800/90 backdrop-blur-sm rounded-16px shadow-lg">
      <div class="flex justify-end">
        <NButton @click="handleReset">重置</NButton>
      </div>
    </NCard>

    <!-- 订单列表 -->
    <NCard :bordered="false" class="rounded-16px shadow-lg bg-white/90 dark:bg-gray-800/90 backdrop-blur-sm">
      <div class="mb-16px flex items-center justify-between">
        <div>
          <h3 class="text-lg font-semibold mb-8px">订单列表</h3>
          <p class="text-gray-600 text-sm">管理您的配送订单</p>
        </div>
        <NSpace>
          <NButton :type="isOnline ? 'success' : 'warning'" @click="updateOnlineStatus(!isOnline)">
            <Icon :icon="isOnline ? 'mdi:account-check' : 'mdi:account-off'" class="mr-1" />
            {{ isOnline ? '下线' : '上线' }}
          </NButton>
        </NSpace>
      </div>

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
            render: (row: OrderData) => row.order?.deliveryAddress || '-'
          },
          {
            title: '商家名称',
            key: 'merchant',
            render: (row: OrderData) => row.order?.merchantName || '-'
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
                // 待接单状态：显示接单和拒绝按钮
                buttons.push(
                  h(
                    NButton,
                    { size: 'small', type: 'success', onClick: () => handleStatusUpdate(row, 1) },
                    { default: () => '接单' }
                  ),
                  h(
                    NButton,
                    { size: 'small', type: 'warning', onClick: () => handleStatusUpdate(row, 2) },
                    { default: () => '拒绝' }
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

    <!-- 订单详情弹窗 -->
    <NModal v-model:show="showModal" preset="card" title="订单详情" class="w-[600px]" :loading="detailLoading">
      <div v-if="!detailLoading && orderDetail">
        <NDescriptions label-placement="left" bordered :column="1">
          <NDescriptionsItem label="订单号">
            {{ orderDetail.order?.orderId || '-' }}
          </NDescriptionsItem>
          <NDescriptionsItem label="配送地址">
            {{ orderDetail.order?.deliveryAddress || '-' }}
          </NDescriptionsItem>
          <NDescriptionsItem label="商家名称">
            {{ orderDetail.order?.merchantName || '-' }}
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