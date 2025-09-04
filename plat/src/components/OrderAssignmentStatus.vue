<template>
  <div class="order-assignment-status">
    <NCard 
      :title="`${riderName || '骑手'}的订单分配状态`" 
      embedded 
      :loading="loading"
      size="small"
    >
      <template #header-extra>
        <NButton size="small" @click="refreshData" :loading="loading">
          <template #icon>
            <NIcon><RefreshOutline /></NIcon>
          </template>
          刷新
        </NButton>
      </template>

      <!-- 分配统计卡片 -->
      <NGrid x-gap="12" y-gap="12" cols="2 s:3 m:4" responsive="screen" class="mb-4">
        <NGi>
          <NStatistic label="总分配数" :value="statistics.total">
            <template #suffix>
              <NText depth="3">单</NText>
            </template>
          </NStatistic>
        </NGi>
        <NGi>
          <NStatistic label="待接单" :value="statistics.pending" class="text-orange-500">
            <template #suffix>
              <NText depth="3">单</NText>
            </template>
          </NStatistic>
        </NGi>
        <NGi>
          <NStatistic label="已接单" :value="statistics.accepted" class="text-green-500">
            <template #suffix>
              <NText depth="3">单</NText>
            </template>
          </NStatistic>
        </NGi>
        <NGi>
          <NStatistic label="已拒绝" :value="statistics.rejected" class="text-red-500">
            <template #suffix>
              <NText depth="3">单</NText>
            </template>
          </NStatistic>
        </NGi>
      </NGrid>

      <!-- 分配列表 -->
      <NDataTable
        :columns="columns"
        :data="assignmentData"
        :loading="loading"
        :pagination="paginationConfig"
        size="small"
        max-height="400"
        :scroll-x="800"
      />
    </NCard>
  </div>
</template>

<script setup lang="tsx">
import { ref, onMounted, computed, watch, onUnmounted } from 'vue';
import {
  NCard,
  NButton,
  NIcon,
  NDataTable,
  NTag,
  NStatistic,
  NGrid,
  NGi,
  NText,
  NTime,
  useMessage,
  type DataTableColumns
} from 'naive-ui';
import {
  RefreshOutline,
  CheckmarkCircleOutline,
  CloseCircleOutline,
  TimeOutline,
  DocumentTextOutline
} from '@vicons/ionicons5';
import { getRiderAssignments } from '@/api/order';

interface Props {
  riderId?: string;
  riderName?: string;
  autoRefresh?: boolean; // 是否自动刷新
  refreshInterval?: number; // 刷新间隔(ms)，默认30秒
}

const props = withDefaults(defineProps<Props>(), {
  autoRefresh: true,
  refreshInterval: 30000
});

const message = useMessage();
const loading = ref(false);
const assignmentData = ref<any[]>([]);
let refreshTimer: NodeJS.Timeout | null = null;

// 分配状态枚举
const assignmentStatusMap = {
  0: { label: '待接单', color: 'warning', icon: TimeOutline },
  1: { label: '已接单', color: 'success', icon: CheckmarkCircleOutline },
  2: { label: '已拒绝', color: 'error', icon: CloseCircleOutline }
};

// 统计数据
const statistics = computed(() => {
  const total = assignmentData.value.length;
  const pending = assignmentData.value.filter(item => item.acceptedStatus === 0).length;
  const accepted = assignmentData.value.filter(item => item.acceptedStatus === 1).length;
  const rejected = assignmentData.value.filter(item => item.acceptedStatus === 2).length;
  
  return { total, pending, accepted, rejected };
});

// 分页配置
const paginationConfig = {
  pageSize: 10,
  showSizePicker: true,
  pageSizes: [10, 20, 50],
  showQuickJumper: true
};

// 表格列配置
const columns: DataTableColumns = [
  {
    title: '订单ID',
    key: 'orderId',
    width: 120,
    ellipsis: {
      tooltip: true
    }
  },
  {
    title: '订单金额',
    key: 'orderAmount',
    width: 100,
    render: (row) => `¥${row.orderAmount?.toFixed(2) || '0.00'}`
  },
  {
    title: '分配时间',
    key: 'assignedAt',
    width: 160,
    render: (row) => (
      <NTime value={new Date(row.assignedAt)} format="MM-dd HH:mm:ss" />
    )
  },
  {
    title: '分配状态',
    key: 'acceptedStatus',
    width: 100,
    render: (row) => {
      const status = assignmentStatusMap[row.acceptedStatus] || { label: '未知', color: 'default', icon: DocumentTextOutline };
      return (
        <NTag type={status.color} size="small">
          {{
            icon: () => <NIcon component={status.icon} />,
            default: () => status.label
          }}
        </NTag>
      );
    }
  },
  {
    title: '接单时间',
    key: 'acceptedAt',
    width: 160,
    render: (row) => {
      if (row.acceptedAt) {
        return <NTime value={new Date(row.acceptedAt)} format="MM-dd HH:mm:ss" />;
      }
      return <NText depth="3">未接单</NText>;
    }
  },
  {
    title: '超时(分钟)',
    key: 'timeOut',
    width: 100,
    render: (row) => row.timeOut || <NText depth="3">无限制</NText>
  }
];

// 获取骑手分配数据
async function fetchAssignmentData() {
  if (!props.riderId) {
    assignmentData.value = [];
    return;
  }

  try {
    loading.value = true;
    console.log(`获取骑手分配数据，骑手ID: ${props.riderId}`);
    
    const response = await getRiderAssignments(props.riderId);
    console.log('骑手分配数据响应:', response);
    
    if (response && Array.isArray(response)) {
      // 按分配时间倒序排列
      assignmentData.value = response.sort((a, b) => 
        new Date(b.assignedAt).getTime() - new Date(a.assignedAt).getTime()
      );
      console.log(`成功获取${assignmentData.value.length}条分配记录`);
    } else {
      assignmentData.value = [];
      console.warn('获取到的分配数据格式不正确:', response);
    }
  } catch (error) {
    console.error('获取骑手分配数据失败:', error);
    message.error('获取订单分配数据失败: ' + (error.message || '未知错误'));
    assignmentData.value = [];
  } finally {
    loading.value = false;
  }
}

// 手动刷新数据
function refreshData() {
  fetchAssignmentData();
}

// 设置自动刷新
function setupAutoRefresh() {
  if (refreshTimer) {
    clearInterval(refreshTimer);
    refreshTimer = null;
  }
  
  if (props.autoRefresh && props.riderId) {
    refreshTimer = setInterval(() => {
      fetchAssignmentData();
    }, props.refreshInterval);
  }
}

// 监听 riderId 变化
watch(() => props.riderId, (newRiderId) => {
  if (newRiderId) {
    fetchAssignmentData();
    setupAutoRefresh();
  } else {
    assignmentData.value = [];
    if (refreshTimer) {
      clearInterval(refreshTimer);
      refreshTimer = null;
    }
  }
}, { immediate: true });

// 监听自动刷新配置变化
watch(() => [props.autoRefresh, props.refreshInterval], () => {
  setupAutoRefresh();
}, { immediate: true });

// 组件挂载时获取数据
onMounted(() => {
  if (props.riderId) {
    fetchAssignmentData();
  }
});

// 组件卸载时清除定时器
onUnmounted(() => {
  if (refreshTimer) {
    clearInterval(refreshTimer);
  }
});
</script>

<style scoped>
.order-assignment-status {
  width: 100%;
}

.mb-4 {
  margin-bottom: 1rem;
}

.text-orange-500 {
  color: #f59e0b;
}

.text-green-500 {
  color: #10b981;
}

.text-red-500 {
  color: #ef4444;
}
</style>
