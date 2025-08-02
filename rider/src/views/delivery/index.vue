<script setup lang="ts">
import { computed, h, onMounted, ref } from 'vue';
import { NButton, NCard, NDescriptions, NDescriptionsItem, NModal, NSpace, NTag } from 'naive-ui';
import { getRiderOrderList, updateAssignStatus } from '@/service/api/rider';
import { handleCommonError, handleOrderError } from '@/utils/rider-error-handler';

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

// 骑手ID，实际项目中可能从用户信息或路由参数中获取
const riderId = '1663b73718a54c65b32f5b6787972949';

// 订单列表
const orders = ref<OrderData[]>([]);
const loading = ref(false);
const pagination = ref({
  page: 1,
  pageSize: 10,
  itemCount: 0
});

const showModal = ref(false);
const selectedOrder = ref<OrderData | null>(null);

// 状态映射
const statusMap = {
  0: { text: '未处理', type: 'warning' },
  1: { text: '配送中', type: 'info' },
  2: { text: '已接单', type: 'primary' },
  3: { text: '已完成', type: 'success' },
  4: { text: '已拒单', type: 'error' }
};

// 获取订单数据
const fetchOrders = async () => {
  loading.value = true;
  try {
    const params = {
      page: pagination.value.page,
      pageSize: pagination.value.pageSize
    };

    const { data } = await getRiderOrderList(riderId, params);
    if (Array.isArray(data)) {
      orders.value = data as OrderData[];
      pagination.value.itemCount = data.length;
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

// 首次加载时获取数据
onMounted(fetchOrders);

// 处理分页变化
const handlePageChange = (page: number) => {
  pagination.value.page = page;
  fetchOrders();
};

// 订单详情
function handleDetail(order: OrderData) {
  selectedOrder.value = order;
  showModal.value = true;
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
          await updateAssignStatus(riderId, assignId, { acceptedStatus: newStatus });
          window.$message?.success(`${statusText}成功！`);
          // 刷新订单列表
          await fetchOrders();
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
            render: row => row.order?.deliveryAddress || '-'
          },
          {
            title: '商家名称',
            key: 'merchant',
            render: row => row.order?.merchantName || '-'
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

    <!-- 订单详情弹窗 -->
    <NModal v-model:show="showModal" preset="card" title="订单详情" class="w-[600px]">
      <NDescriptions v-if="selectedOrder" label-placement="left" bordered :column="1">
        <NDescriptionsItem label="订单号">
          {{ selectedOrder.order?.orderId || '-' }}
        </NDescriptionsItem>
        <NDescriptionsItem label="配送地址">
          {{ selectedOrder.order?.deliveryAddress || '-' }}
        </NDescriptionsItem>
        <NDescriptionsItem label="商家名称">
          {{ selectedOrder.order?.merchantName || '-' }}
        </NDescriptionsItem>
        <NDescriptionsItem label="分配时间">
          {{ selectedOrder.assignedAt || '-' }}
        </NDescriptionsItem>
        <NDescriptionsItem label="订单金额">￥{{ selectedOrder.order?.orderAmount || 0 }}</NDescriptionsItem>
        <NDescriptionsItem label="订单状态">
          <NTag
            :type="(statusMap[(selectedOrder.acceptedStatus || 0) as keyof typeof statusMap]?.type || 'default') as any"
            size="small"
          >
            {{ statusMap[(selectedOrder.acceptedStatus || 0) as keyof typeof statusMap]?.text || '未知' }}
          </NTag>
        </NDescriptionsItem>
        <NDescriptionsItem label="创建时间">
          {{ selectedOrder.order?.createAt || '-' }}
        </NDescriptionsItem>
      </NDescriptions>
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
