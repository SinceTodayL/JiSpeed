<script setup lang="tsx">
import { ref, reactive, computed, onMounted } from 'vue';
import { NButton, NPopconfirm, NTag } from 'naive-ui';
import type { DataTableColumns } from 'naive-ui';
import { fetchGetAllCoupons, addCoupon, updateCoupon, deleteCoupon, batchDeleteCoupon } from '@/service/api';
import { useMerchantStore } from '@/store/modules/merchant';
import { useAppStore } from '@/store/modules/app';
import { $t } from '@/locales';

import CouponSearch from '@/views/coupon-management/modules/coupon-search.vue';
import CouponOperateDrawer from '@/views/coupon-management/modules/coupon-operate-drawer.vue';

const appStore = useAppStore();
const merchantStore = useMerchantStore();

type Model = Api.Coupon.CouponItem;

const data = ref<Model[]>([]);
const loading = ref(false);
const originalData = ref<Model[]>([]);

// 搜索参数
const searchParams = reactive({
  couponId: null as string | null,
  status: null as string | null,
  faceValueRange: null as [number, number] | null
});

// 表格操作相关
const drawerVisible = ref(false);
const operateType = ref<'add' | 'edit'>('add');
const editingData = ref<Model | null>(null);
const checkedRowKeys = ref<string[]>([]);

// 列显示控制（用于TableHeaderOperation组件）
const columnChecks = ref([
  { key: 'selection', title: '选择', checked: true },
  { key: 'index', title: '序号', checked: true },
  { key: 'couponId', title: '优惠券ID', checked: true },
  { key: 'faceValue', title: '面值', checked: true },
  { key: 'threshold', title: '使用门槛', checked: true },
  { key: 'totalQty', title: '总数量', checked: true },
  { key: 'issuedQty', title: '已发放', checked: true },
  { key: 'remainingQty', title: '剩余数量', checked: true },
  { key: 'status', title: '状态', checked: true },
  { key: 'startTime', title: '开始时间', checked: true },
  { key: 'endTime', title: '结束时间', checked: true },
  { key: 'operate', title: '操作', checked: true }
]);

// 完整的表格列定义
const allColumns = computed((): DataTableColumns<Model> => [
  {
    type: 'selection' as const,
    width: 48
  },
  {
    key: 'index',
    title: $t('common.index'),
    width: 64,
    render: (_row: Model, index: number) => index + 1
  },
  {
    key: 'couponId',
    title: '优惠券ID',
    width: 60,           
    ellipsis: {          
      tooltip: true       // 鼠标悬停显示完整内容
    }
  },
  {
    key: 'faceValue',
    title: '面值',
    width: 100,
    render: (row: Model) => `¥${Number(row.faceValue).toFixed(2)}`
  },
  {
    key: 'threshold',
    title: '使用门槛',
    width: 100,
    render: (row: Model) => `¥${Number(row.threshold).toFixed(2)}`
  },
  {
    key: 'totalQty',
    title: '总数量',
    width: 100,
    render: (row: Model) => `${row.totalQty}张`
  },
  {
    key: 'issuedQty',
    title: '已发放',
    width: 100,
    render: (row: Model) => `${row.issuedQty}张`
  },
  {
    key: 'remainingQty',
    title: '剩余数量',
    width: 100,
    render: (row: Model) => {
      const remaining = row.totalQty - row.issuedQty;
      return `${remaining}张`;
    }
  },
  {
    key: 'status',
    title: '状态',
    width: 100,
    render: (row: Model) => {
      const now = new Date();
      const startTime = new Date(row.startTime);
      const endTime = new Date(row.endTime);
      
      let status = '';
      let type: 'default' | 'success' | 'warning' | 'error' = 'default';
      
      if (now < startTime) {
        status = '未开始';
        type = 'default';
      } else if (now > endTime) {
        status = '已过期';
        type = 'error';
      } else if (row.issuedQty >= row.totalQty) {
        status = '已发完';
        type = 'warning';
      } else {
        status = '进行中';
        type = 'success';
      }
      
      return <NTag type={type}>{status}</NTag>;
    }
  },
  {
    key: 'startTime',
    title: '开始时间',
    width: 180,
    render: (row: Model) => {
      try {
        const date = new Date(row.startTime);
        return date.toLocaleString('zh-CN');
      } catch {
        return row.startTime;
      }
    }
  },
  {
    key: 'endTime',
    title: '结束时间',
    width: 180,
    render: (row: Model) => {
      try {
        const date = new Date(row.endTime);
        return date.toLocaleString('zh-CN');
      } catch {
        return row.endTime;
      }
    }
  },
  {
    key: 'operate',
    title: $t('common.operate'),
    width: 130,
    render: (row: Model) => (
      <div style={{ display: 'flex', gap: '8px' }}>
        <NButton
          type="primary"
          ghost
          size="small"
          onClick={() => handleEdit(row.couponId)}
        >
          {$t('common.edit')}
        </NButton>
        <NPopconfirm onPositiveClick={() => handleDelete(row.couponId)}>
          {{
            default: () => $t('common.confirmDelete'),
            trigger: () => (
              <NButton type="error" ghost size="small">
                {$t('common.delete')}
              </NButton>
            )
          }}
        </NPopconfirm>
      </div>
    )
  }
]);

// 根据columnChecks过滤显示的列
const columns = computed(() => {
  return allColumns.value.filter(column => {
    if (column.type === 'selection') {
      return columnChecks.value.find(item => item.key === 'selection')?.checked !== false;
    }
    if ('key' in column && column.key) {
      const checkItem = columnChecks.value.find(item => item.key === column.key);
      return checkItem?.checked !== false;
    }
    return true;
  });
});

// 获取数据
async function loadData() {
  try {
    loading.value = true;
    const result = await fetchGetAllCoupons(merchantStore.merchantId);

    // 直接判断返回的是否为数组
    if (Array.isArray(result)) {
      data.value = result;
      originalData.value = [...result];
    } else {
      // 请求失败或返回数据格式不正确时，清空数据
      data.value = [];
      originalData.value = [];
    }
  } catch (error) {
    console.error('获取优惠券数据失败:', error);
    window.$message?.error('获取优惠券数据失败，请检查网络连接');
    data.value = [];
    originalData.value = [];
  } finally {
    loading.value = false;
  }
}

// 搜索过滤
function filterData() {
  let filteredData = [...originalData.value];
  
  if (searchParams.couponId) {
    filteredData = filteredData.filter(item => 
      item.couponId.toLowerCase().includes(searchParams.couponId!.toLowerCase())
    );
  }
  
  if (searchParams.status) {
    filteredData = filteredData.filter(item => {
      const now = new Date();
      const startTime = new Date(item.startTime);
      const endTime = new Date(item.endTime);
      
      let status = '';
      if (now < startTime) {
        status = 'not_started';
      } else if (now > endTime) {
        status = 'expired';
      } else if (item.issuedQty >= item.totalQty) {
        status = 'sold_out';
      } else {
        status = 'active';
      }
      
      return status === searchParams.status;
    });
  }
  
  if (searchParams.faceValueRange) {
    const [min, max] = searchParams.faceValueRange;
    filteredData = filteredData.filter(item => 
      item.faceValue >= min && item.faceValue <= max
    );
  }
  
  data.value = filteredData;
}

// 处理函数
function handleSearch() {
  filterData();
}

function handleResetSearch() {
  searchParams.couponId = null;
  searchParams.status = null;
  searchParams.faceValueRange = null;
  data.value = [...originalData.value];
}

function handleAdd() {
  operateType.value = 'add';
  editingData.value = null;
  drawerVisible.value = true;
}

function handleEdit(id: string) {
  operateType.value = 'edit';
  editingData.value = data.value.find(item => item.couponId === id) || null;
  drawerVisible.value = true;
}

async function handleDelete(id: string) {
  try {
    await deleteCoupon(id);
    window.$message?.success('优惠券删除成功');
    
    // 从数据中删除
    const index = data.value.findIndex(item => item.couponId === id);
    if (index > -1) {
      data.value.splice(index, 1);
      const originalIndex = originalData.value.findIndex(item => item.couponId === id);
      if (originalIndex > -1) {
        originalData.value.splice(originalIndex, 1);
      }
    }
  } catch (error) {
    console.error('删除优惠券失败:', error);
    window.$message?.error('删除优惠券失败');
  }
}

// 批量删除
async function handleBatchDelete() {
  if (checkedRowKeys.value.length === 0) {
    window.$message?.warning('请先选择要删除的优惠券');
    return;
  }
  
  try {
    await batchDeleteCoupon(checkedRowKeys.value);
    window.$message?.success(`成功删除${checkedRowKeys.value.length}个优惠券`);
    
    // 从数据中删除选中的项
    checkedRowKeys.value.forEach(id => {
      const index = data.value.findIndex(item => item.couponId === id);
      if (index > -1) {
        data.value.splice(index, 1);
      }
      const originalIndex = originalData.value.findIndex(item => item.couponId === id);
      if (originalIndex > -1) {
        originalData.value.splice(originalIndex, 1);
      }
    });
    
    checkedRowKeys.value = [];
  } catch (error) {
    console.error('批量删除优惠券失败:', error);
    window.$message?.error('批量删除优惠券失败');
  }
}

function refresh() {
  loadData();
}

onMounted(() => {
  loadData();
});
</script>

<template>
  <div class="min-h-500px flex-col-stretch gap-16px overflow-hidden lt-sm:overflow-auto">
    <CouponSearch 
      v-model:coupon-id="searchParams.couponId"
      v-model:status="searchParams.status"
      v-model:face-value-range="searchParams.faceValueRange"
      @reset="handleResetSearch"
      @search="handleSearch"
    />
    <NCard title="优惠券管理" :bordered="false" size="small" class="sm:flex-1-hidden card-wrapper">
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
        :scroll-x="702"
        :loading="loading"
        :row-key="(row: Model) => row.couponId"
        class="sm:h-full"
      />
      <CouponOperateDrawer
        v-model:visible="drawerVisible"
        :operate-type="operateType"
        :row-data="editingData"
        @submitted="loadData"
      />
    </NCard>
  </div>
</template>

<style scoped></style> 