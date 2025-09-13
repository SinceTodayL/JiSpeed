<template>
  <div class="order-overview">
    <n-card title="订单分配总览" class="mb-4">
      <template #header-extra>
        <div class="flex items-center gap-3">
          <n-button 
            type="primary" 
            size="small" 
            :loading="loading"
            @click="refreshData"
          >
            <template #icon>
              <n-icon><RefreshOutline /></n-icon>
            </template>
            刷新
          </n-button>
          <n-select
            v-model:value="selectedStatus"
            :options="statusOptions"
            placeholder="筛选状态"
            style="width: 120px"
            size="small"
            @update:value="handleStatusChange"
          />
          <n-text depth="3" style="font-size: 12px">
            最后更新: {{ lastUpdateTime ? lastUpdateTime.toLocaleTimeString() : '-' }}
          </n-text>
        </div>
      </template>

      <!-- 统计概览 -->
      <div class="mb-6">
        <n-grid cols="5" x-gap="16">
          <n-gi>
            <n-statistic label="总订单数" :value="orderStats.totalOrders">
              <template #suffix>
                <n-text type="info">单</n-text>
              </template>
            </n-statistic>
          </n-gi>
          <n-gi>
            <n-statistic label="已分配" :value="orderStats.assigned">
              <template #suffix>
                <n-text type="success">单</n-text>
              </template>
            </n-statistic>
          </n-gi>
          <n-gi>
            <n-statistic label="配送中" :value="orderStats.delivering">
              <template #suffix>
                <n-text type="warning">单</n-text>
              </template>
            </n-statistic>
          </n-gi>
          <n-gi>
            <n-statistic label="今日完成" :value="orderStats.completed">
              <template #suffix>
                <n-text type="success">单</n-text>
              </template>
            </n-statistic>
          </n-gi>
          <n-gi>
            <n-statistic label="分配率" :value="orderStats.assignmentRate">
              <template #suffix>
                <n-text type="info">%</n-text>
              </template>
            </n-statistic>
          </n-gi>
        </n-grid>
      </div>

      <!-- 骑手订单状态表格 -->
      <n-data-table
        :columns="columns"
        :data="filteredData"
        :loading="loading"
        :pagination="paginationProps"
        flex-height
        striped
        size="small"
        :scroll-x="1200"
        style="height: 400px"
      />
    </n-card>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted, h } from 'vue'
import { 
  NCard, 
  NButton, 
  NIcon, 
  NSelect, 
  NText, 
  NGrid, 
  NGi, 
  NStatistic, 
  NDataTable, 
  NBadge,
  NTime,
  useMessage 
} from 'naive-ui'
import { RefreshOutline } from '@vicons/ionicons5'
import { getOrderAssignmentOverview } from '../api/order.ts'

// Props
const props = defineProps({
  autoRefresh: {
    type: Boolean,
    default: true
  },
  refreshInterval: {
    type: Number,
    default: 30000 // 30秒
  }
});

// 响应式数据
const loading = ref(false)
const lastUpdateTime = ref(null)
const selectedStatus = ref('all')
const message = useMessage()

// 订单统计数据
const orderStats = ref({
  totalOrders: 0,
  assigned: 0,
  delivering: 0,
  completed: 0,
  assignmentRate: 0
})

// 骑手数据
const ridersData = ref([])

// 状态选项
const statusOptions = [
  { label: '全部状态', value: 'all' },
  { label: '空闲', value: '空闲' },
  { label: '配送中', value: '配送中' },
  { label: '离线', value: '离线' }
]

// 过滤数据
const filteredData = computed(() => {
  if (selectedStatus.value === 'all') {
    return ridersData.value
  }
  return ridersData.value.filter(item => item.status === selectedStatus.value)
})

// 表格列定义
const columns = [
  {
    title: '骑手ID',
    key: 'riderId',
    width: 100,
    fixed: 'left'
  },
  {
    title: '骑手姓名',
    key: 'riderName',
    width: 120,
    fixed: 'left'
  },
  {
    title: '当前状态',
    key: 'status',
    width: 100,
    render(row) {
      const statusMap = {
        '空闲': { type: 'success', text: '空闲' },
        '配送中': { type: 'warning', text: '配送中' },
        '离线': { type: 'error', text: '离线' }
      }
      const status = statusMap[row.status] || { type: 'default', text: '未知' }
      return h(NBadge, { type: status.type, text: status.text })
    }
  },
  {
    title: '当前订单',
    key: 'currentOrders',
    width: 100,
    render(row) {
      return h('span', { 
        style: { 
          fontWeight: 'bold', 
          color: row.currentOrders > 0 ? '#f0a020' : '#666' 
        } 
      }, `${row.currentOrders || 0} 单`)
    }
  },
  {
    title: '今日完成',
    key: 'todayCompleted',
    width: 100,
    render(row) {
      return h('span', { style: { color: '#18a058' } }, `${row.todayCompleted || 0} 单`)
    }
  },
  {
    title: '平均配送时间',
    key: 'avgDeliveryTime',
    width: 120,
    render(row) {
      return row.avgDeliveryTime ? `${row.avgDeliveryTime} 分钟` : '-'
    }
  },
  {
    title: '最近订单时间',
    key: 'lastOrderTime',
    width: 160,
    render(row) {
      return row.lastOrderTime 
        ? h(NTime, { time: new Date(row.lastOrderTime), format: 'MM-dd HH:mm' })
        : '-'
    }
  },
  {
    title: '位置信息',
    key: 'location',
    width: 150,
    render(row) {
      return row.location || '位置未知'
    }
  },
  {
    title: '操作',
    key: 'actions',
    width: 120,
    fixed: 'right',
    render(row) {
      return h('div', { class: 'flex gap-2' }, [
        h(NButton, 
          { 
            size: 'small', 
            type: 'primary',
            onClick: () => viewRiderDetails(row.riderId)
          }, 
          '查看详情'
        )
      ])
    }
  }
]

// 分页配置
const paginationProps = {
  pageSize: 15,
  showSizePicker: true,
  pageSizes: [10, 15, 20, 50],
  showQuickJumper: true,
  prefix: ({ itemCount }) => `共 ${itemCount} 个骑手`
}

// 自动刷新定时器
let refreshTimer = null

// 刷新数据
const refreshData = async () => {
  if (loading.value) return
  
  loading.value = true
  lastUpdateTime.value = new Date()
  
  try {
    console.log('刷新订单分配总览数据...')
    
    // 调用真实API获取数据
    const response = await getOrderAssignmentOverview()
    
    orderStats.value = response.orderStats
    ridersData.value = response.ridersOverview
    
    console.log('数据刷新完成', { orderStats: orderStats.value, ridersCount: ridersData.value.length })
  } catch (error) {
    console.error('刷新数据失败:', error)
    
    // 发生错误时显示错误信息和空数据
    orderStats.value = {
      totalOrders: 0,
      assigned: 0,
      delivering: 0,
      completed: 0,
      assignmentRate: 0
    }
    ridersData.value = []
    
    // 根据错误类型显示不同的错误信息
    let errorMessage = '获取订单数据失败'
    if (error.response?.status === 401) {
      errorMessage = '认证失败，请重新登录'
    } else if (error.response?.status === 404) {
      errorMessage = 'API接口不存在，请联系管理员'
    } else if (error.response?.status >= 500) {
      errorMessage = '服务器内部错误，请稍后重试'
    } else if (error.code === 'NETWORK_ERROR') {
      errorMessage = '网络连接失败，请检查网络设置'
    }
    
    message.error(errorMessage)
  } finally {
    loading.value = false
  }
}

const handleStatusChange = () => {
  // 状态改变时不需要重新请求数据，只需要过滤本地数据
  console.log('筛选状态改变:', selectedStatus.value)
}

const viewRiderDetails = (riderId) => {
  // 这里可以路由到骑手详情页或打开详情弹窗
  console.log('查看骑手详情:', riderId)
  message.info(`查看骑手 ${riderId} 的详细信息`)
}

// 启动自动刷新
const startAutoRefresh = () => {
  if (props.autoRefresh && props.refreshInterval > 0) {
    refreshTimer = setInterval(() => {
      refreshData()
    }, props.refreshInterval)
    console.log(`已启动自动刷新，间隔: ${props.refreshInterval / 1000}秒`)
  }
}

// 停止自动刷新
const stopAutoRefresh = () => {
  if (refreshTimer) {
    clearInterval(refreshTimer)
    refreshTimer = null
    console.log('已停止自动刷新')
  }
}

// 生命周期
onMounted(() => {
  refreshData()
  startAutoRefresh()
})

onUnmounted(() => {
  stopAutoRefresh()
})
</script>

<style scoped>
.order-overview {
  width: 100%;
}

.flex {
  display: flex;
  align-items: center;
}

.gap-2 {
  gap: 8px;
}

.gap-3 {
  gap: 12px;
}

.items-center {
  align-items: center;
}

.mb-4 {
  margin-bottom: 16px;
}

.mb-6 {
  margin-bottom: 24px;
}
</style>