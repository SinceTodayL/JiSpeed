<script setup lang="tsx">
import { reactive, ref, computed } from 'vue';
import { NButton, NPopconfirm, NTag } from 'naive-ui';
import { fetchGetAllOrders, getOrderStatusText } from '@/service/api';
import { useAppStore } from '@/store/modules/app';
import { useMerchantStore } from '@/store/modules/merchant';
import { $t } from '@/locales';
import OrderSearch from './modules/order-search.vue';
import OrderOperateDrawer from './modules/order-operate-drawer.vue';

const appStore = useAppStore();
const merchantStore = useMerchantStore();

const loading = ref(false);
const data = ref<Api.Order.OrderItem[]>([]);
const originalData = ref<Api.Order.OrderItem[]>([]); // 保存原始数据用于搜索
const checkedRowKeys = ref<string[]>([]);
const drawerVisible = ref(false);
const operateType = ref<'add' | 'edit'>('add');
const editingData = ref<Api.Order.OrderItem | null>(null);

const searchParams = reactive<{
  orderId: string | null;
  userId: string | null;
  orderStatus: number | null;
}>({
  orderId: null,
  userId: null,
  orderStatus: null
});

// 获取数据
const getData = async () => {
  loading.value = true;
  try {
    const result = await fetchGetAllOrders(merchantStore.merchantId);
    console.log('API响应:', result);
    
    // API已经经过处理，直接是 data 数组
    if (result && Array.isArray(result)) {
      originalData.value = result; // 保存原始数据
      data.value = result; // 显示数据
      console.log('成功获取订单数据:', result);
    } else {
      console.warn('API响应格式不正确:', result);
      originalData.value = [];
      data.value = [];
    }
  } catch (error) {
    console.error('获取订单数据失败:', error);
    window.$message?.error('获取订单数据失败，请检查网络连接');
    originalData.value = [];
    data.value = [];
  } finally {
    loading.value = false;
  }
};

// 本地搜索过滤
const filterData = () => {
  let filteredData = [...originalData.value];
  
  // 根据订单ID搜索
  if (searchParams.orderId && searchParams.orderId.trim()) {
    filteredData = filteredData.filter(item => 
      item.orderId.toLowerCase().includes(searchParams.orderId!.toLowerCase())
    );
  }
  
  // 根据用户ID搜索
  if (searchParams.userId && searchParams.userId.trim()) {
    filteredData = filteredData.filter(item => 
      item.userId.toLowerCase().includes(searchParams.userId!.toLowerCase())
    );
  }
  
  // 根据订单状态搜索
  if (searchParams.orderStatus !== null && searchParams.orderStatus !== undefined) {
    const statusValue = Number(searchParams.orderStatus);
    filteredData = filteredData.filter(item => 
      Number(item.orderStatus) === statusValue
    );
  }
  
  data.value = filteredData;
};

// 搜索
const handleSearch = () => {
  filterData();
};

// 重置搜索
const resetSearchParams = () => {
  Object.assign(searchParams, {
    orderId: null,
    userId: null,
    orderStatus: null
  });
  data.value = [...originalData.value]; // 重置为原始数据
};

// 新增
const handleAdd = () => {
  operateType.value = 'add';
  editingData.value = null;
  drawerVisible.value = true;
};

// 编辑
const handleEdit = (orderId: string) => {
  const item = data.value.find(order => order.orderId === orderId);
  if (item) {
    operateType.value = 'edit';
    editingData.value = item;
    drawerVisible.value = true;
  }
};

// 删除
const handleDelete = (orderId: string) => {
  console.log('删除订单:', orderId);
  // TODO: 实现删除功能
  getData();
};

// 批量删除
const handleBatchDelete = () => {
  console.log('批量删除:', checkedRowKeys.value);
  // TODO: 实现批量删除功能
  checkedRowKeys.value = [];
  getData();
};

// 刷新
const refresh = () => {
  getData();
};

// 提交后重新加载
const handleSubmitted = () => {
  getData();
};

// 列配置
const columnChecks = ref([
  { key: 'selection', title: '选择', checked: true },
  { key: 'orderId', title: '订单ID', checked: true },
  { key: 'userId', title: '用户ID', checked: true },
  { key: 'orderAmount', title: '订单金额', checked: true },
  { key: 'orderStatus', title: '订单状态', checked: true },
  { key: 'createAt', title: '创建时间', checked: true },
  { key: 'addressId', title: '地址信息', checked: true },
  { key: 'couponId', title: '优惠券ID', checked: true },
  { key: 'operate', title: '操作', checked: true }
]);

const columns = computed(() => {
  const allColumns = [
    {
      type: 'selection' as const,
      align: 'center' as const,
      width: 48
    },
    {
      key: 'orderId',
      title: '订单ID',
      align: 'center' as const,
      width: 140,
      ellipsis: {
        tooltip: true
      }
    },
    {
      key: 'userId',
      title: '用户ID',
      align: 'center' as const,
      width: 120,
      ellipsis: {
        tooltip: true
      }
    },
    {
      key: 'orderAmount',
      title: '订单金额',
      align: 'center' as const,
      width: 120,
      render: (row: Api.Order.OrderItem) => `¥${row.orderAmount.toFixed(2)}`
    },
    {
      key: 'orderStatus',
      title: '订单状态',
      align: 'center' as const,
      width: 100,
      render: (row: Api.Order.OrderItem) => {
        const statusText = getOrderStatusText(row.orderStatus);
        let type: 'default' | 'success' | 'warning' | 'error' = 'default';
        
        switch (row.orderStatus) {
          case 1:
            type = 'warning'; // 待付款
            break;
          case 2:
            type = 'default'; // 待发货
            break;
          case 3:
            type = 'success'; // 已发货
            break;
          case 4:
            type = 'success'; // 已完成
            break;
          case 5:
            type = 'error'; // 已取消
            break;
          case 49:
            type = 'success'; // 进行中
            break;
          default:
            type = 'default';
        }
        
        return (
          <NTag type={type}>
            {statusText}
          </NTag>
        );
      }
    },
    {
      key: 'createAt',
      title: '创建时间',
      align: 'center' as const,
      width: 160,
      render: (row: Api.Order.OrderItem) => {
        // 格式化时间显示
        const date = new Date(row.createAt);
        return date.toLocaleString('zh-CN');
      }
    },
    {
      key: 'addressId',
      title: '地址信息',
      align: 'center' as const,
      width: 200,
      ellipsis: {
        tooltip: true
      }
    },
    {
      key: 'couponId',
      title: '优惠券ID',
      align: 'center' as const,
      width: 120,
      ellipsis: {
        tooltip: true
      }
    },
    {
      key: 'operate',
      title: '操作',
      align: 'center' as const,
      width: 130,
      render: (row: Api.Order.OrderItem) => (
        <div class="flex-center gap-8px">
          <NButton type="primary" ghost size="small" onClick={() => handleEdit(row.orderId)}>
            查看
          </NButton>
          <NPopconfirm onPositiveClick={() => handleDelete(row.orderId)}>
            {{
              default: () => '确认删除吗？',
              trigger: () => (
                <NButton type="error" ghost size="small">
                  删除
                </NButton>
              )
            }}
          </NPopconfirm>
        </div>
      )
    }
  ];

  return allColumns.filter(column => {
    const checkItem = columnChecks.value.find(item => item.key === column.key);
    return checkItem?.checked !== false;
  });
});

// 初始化数据
getData();
</script>

<template>
  <div class="min-h-500px flex-col-stretch gap-16px overflow-hidden lt-sm:overflow-auto">
    <OrderSearch v-model:model="searchParams" @search="handleSearch" @reset="resetSearchParams" />
    <NCard title="订单管理" :bordered="false" size="small" class="card-wrapper sm:flex-1-hidden">
      <template #header-extra>
        <TableHeaderOperation
          v-model:columns="columnChecks"
          :disabled-delete="checkedRowKeys.length === 0"
          :loading="loading"
          @add="handleAdd"
          @delete="handleBatchDelete"
          @refresh="refresh"
        />
      </template>
      <NDataTable
        v-model:checked-row-keys="checkedRowKeys"
        :columns="columns"
        :data="data"
        size="small"
        :flex-height="!appStore.isMobile"
        :scroll-x="1200"
        :loading="loading"
        remote
        :row-key="(row: Api.Order.OrderItem) => row.orderId"
        class="sm:h-full"
      />
      <OrderOperateDrawer
        v-model:visible="drawerVisible"
        :operate-type="operateType"
        :row-data="editingData"
        @submitted="handleSubmitted"
      />
    </NCard>
  </div>
</template>

<style scoped></style> 