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
    
    <!-- 分配详情弹窗 -->
    <n-modal 
      v-model:show="showAssignmentModal" 
      preset="card" 
      style="width: 800px; max-height: 80vh;" 
      title="骑手分配详情"
      :mask-closable="false"
    >
      <template #header>
        <div class="flex items-center gap-3">
          <n-icon size="20" color="#409eff">
            <PersonOutline />
          </n-icon>
          <span class="font-medium">
            {{ currentRiderData?.riderName || '未知骑手' }} - 订单分配详情
          </span>
        </div>
      </template>

      <div v-if="!assignmentDetailLoading">
        <!-- 骑手基本信息 -->
        <div class="mb-6 p-4 bg-gray-50 rounded-lg">
          <h4 class="text-gray-800 font-medium mb-3 flex items-center gap-2">
            <n-icon color="#409eff">
              <PersonOutline />
            </n-icon>
            骑手基本信息
          </h4>
          <n-descriptions :column="3" label-placement="left">
            <n-descriptions-item label="骑手ID">
              <n-text code>{{ currentRiderData?.riderId || '-' }}</n-text>
            </n-descriptions-item>
            <n-descriptions-item label="当前状态">
              <n-tag 
                :type="currentRiderData?.status === '空闲' ? 'success' : 
                      currentRiderData?.status === '配送中' ? 'warning' : 'error'"
              >
                {{ currentRiderData?.status || '未知' }}
              </n-tag>
            </n-descriptions-item>
            <n-descriptions-item label="当前订单">
              <n-text strong :style="{ color: currentRiderData?.currentOrders > 0 ? '#f0a020' : '#666' }">
                {{ currentRiderData?.currentOrders || 0 }} 单
              </n-text>
            </n-descriptions-item>
            <n-descriptions-item label="今日完成">
              <n-text :style="{ color: '#18a058' }">{{ currentRiderData?.todayCompleted || 0 }} 单</n-text>
            </n-descriptions-item>
            <n-descriptions-item label="平均配送时间">
              {{ currentRiderData?.avgDeliveryTime ? `${currentRiderData.avgDeliveryTime} 分钟` : '-' }}
            </n-descriptions-item>
            <n-descriptions-item label="位置信息">
              {{ currentRiderData?.location || '位置未知' }}
            </n-descriptions-item>
          </n-descriptions>
        </div>

        <!-- 当前分配订单列表 -->
        <div class="mb-4">
          <h4 class="text-gray-800 font-medium mb-3 flex items-center gap-2">
            <n-icon color="#409eff">
              <InformationCircleOutline />
            </n-icon>
            当前分配订单 ({{ currentRiderAssignments.length }} 单)
          </h4>
          
          <div v-if="currentRiderAssignments.length > 0" class="space-y-4">
            <div 
              v-for="(assignment, index) in currentRiderAssignments" 
              :key="assignment.orderId || assignment.OrderId || index"
              class="border border-gray-200 rounded-lg p-4 hover:bg-gray-50 transition-colors"
            >
              <div class="flex justify-between items-start mb-3">
                <div>
                  <h5 class="font-medium text-gray-800">
                    订单 #{{ assignment.orderId || assignment.OrderId || '未知' }}
                  </h5>
                  <n-text depth="3" class="text-sm">
                    分配时间: {{ formatTime(assignment.assignedAt) }}
                  </n-text>
                </div>
                <n-tag 
                  :type="getOrderStatus(assignment.orderStatus || assignment.status).type"
                >
                  {{ getOrderStatus(assignment.orderStatus || assignment.status).text }}
                </n-tag>
              </div>
              
              <!-- 分配详情信息 -->
              <div v-if="assignment.assignmentDetail" class="bg-blue-50 p-3 rounded">
                <n-descriptions :column="2" label-placement="left" size="small">
                  <n-descriptions-item label="订单金额">
                    {{ assignment.assignmentDetail.orderAmount ? `¥${assignment.assignmentDetail.orderAmount}` : '-' }}
                  </n-descriptions-item>
                  <n-descriptions-item label="配送费">
                    {{ assignment.assignmentDetail.deliveryFee ? `¥${assignment.assignmentDetail.deliveryFee}` : '-' }}
                  </n-descriptions-item>
                  <n-descriptions-item label="取餐地址">
                    {{ assignment.assignmentDetail.pickupAddress || '-' }}
                  </n-descriptions-item>
                  <n-descriptions-item label="送达地址">
                    {{ assignment.assignmentDetail.deliveryAddress || '-' }}
                  </n-descriptions-item>
                  <n-descriptions-item label="预计送达时间">
                    {{ formatTime(assignment.assignmentDetail.estimatedDeliveryTime) }}
                  </n-descriptions-item>
                  <n-descriptions-item label="备注信息">
                    {{ assignment.assignmentDetail.remark || '无' }}
                  </n-descriptions-item>
                </n-descriptions>
              </div>
              
              <!-- 没有详细信息时的提示 -->
              <div v-else class="bg-gray-100 p-3 rounded text-center">
                <n-text depth="3">
                  {{ assignment.error || '暂无详细分配信息' }}
                </n-text>
                <div v-if="assignment.error" class="mt-2">
                  <n-button 
                    size="small" 
                    type="primary" 
                    secondary
                    @click="retryGetAssignmentDetail(assignment)"
                  >
                    重试获取
                  </n-button>
                </div>
              </div>
            </div>
          </div>
          
          <!-- 无分配订单时的提示 -->
          <div v-else class="text-center py-8">
            <n-icon size="48" color="#d9d9d9">
              <InformationCircleOutline />
            </n-icon>
            <p class="text-gray-500 mt-2">该骑手当前无分配订单</p>
          </div>
        </div>
      </div>
      
      <!-- 加载状态 -->
      <div v-else class="flex justify-center items-center h-60">
        <n-spin size="large">
          <template #description>
            <div class="text-center mt-4">
              <p class="text-gray-600">正在加载分配详情...</p>
              <p class="text-sm text-gray-400 mt-1">请稍候片刻</p>
            </div>
          </template>
        </n-spin>
      </div>
      
      <template #action>
        <n-space justify="end">
          <n-button @click="showAssignmentModal = false">关闭</n-button>
          <n-button type="primary" @click="viewAssignmentDetails(currentRiderData)" :loading="assignmentDetailLoading">
            刷新数据
          </n-button>
        </n-space>
      </template>
    </n-modal>
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
  NModal,
  NDescriptions,
  NDescriptionsItem,
  NSpin,
  NSpace,
  NTag,
  useMessage 
} from 'naive-ui'
import { RefreshOutline, InformationCircleOutline, PersonOutline } from '@vicons/ionicons5'
import { getOrderAssignmentOverview, getOrderAssignment, getRiderAssignments } from '../api/order.ts'

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

// 分配详情弹窗相关
const showAssignmentModal = ref(false)
const assignmentDetailLoading = ref(false)
const currentRiderData = ref(null)
const currentRiderAssignments = ref([])

// 订单状态映射
const orderStatusMap = {
  0: { text: '待支付', type: 'warning' },
  1: { text: '已支付', type: 'info' },
  2: { text: '已分配', type: 'warning' },
  3: { text: '已接单', type: 'success' },
  4: { text: '准备中', type: 'info' },
  5: { text: '配送中', type: 'warning' },
  6: { text: '已送达', type: 'success' },
  7: { text: '已完成', type: 'success' },
  8: { text: '已取消', type: 'error' },
  9: { text: '已退款', type: 'error' }
}

// 格式化时间
const formatTime = (timeStr) => {
  if (!timeStr) return '未知时间'
  try {
    const date = new Date(timeStr)
    if (isNaN(date.getTime())) return '时间格式错误'
    return date.toLocaleString('zh-CN', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
    })
  } catch (error) {
    console.error('时间格式化错误:', error)
    return '时间格式错误'
  }
}

// 获取订单状态信息
const getOrderStatus = (status) => {
  const statusKey = typeof status === 'string' ? parseInt(status) : status
  return orderStatusMap[statusKey] || { text: status || '未知状态', type: 'default' }
}

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
    width: 200,
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
        ),
        h(NButton, 
          { 
            size: 'small', 
            type: 'info',
            secondary: true,
            onClick: () => viewAssignmentDetails(row)
          }, 
          { default: () => '分配详情', icon: () => h(NIcon, null, { default: () => h(InformationCircleOutline) }) }
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
    
    console.log('📊 API响应数据:', response)
    
    // 从response.data中获取数据
    const data = response.data || response
    orderStats.value = data.orderStats || {
      totalOrders: 0,
      assigned: 0,
      delivering: 0,
      completed: 0,
      assignmentRate: 0
    }
    ridersData.value = data.ridersOverview || []
    
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

// 查看分配详情
const viewAssignmentDetails = async (riderData) => {
  if (!riderData || !riderData.riderId) {
    message.error('骑手信息无效')
    return
  }
  
  console.log('查看骑手分配详情:', riderData)
  
  try {
    assignmentDetailLoading.value = true
    showAssignmentModal.value = true
    currentRiderData.value = riderData
    currentRiderAssignments.value = []
    
    // 获取骑手当前分配的订单详情
    const response = await getRiderAssignments(riderData.riderId)
    console.log('骑手分配订单响应:', response)
    
    if (response?.data && Array.isArray(response.data)) {
      if (response.data.length === 0) {
        console.log('骑手暂无分配订单')
        currentRiderAssignments.value = []
        return
      }
      
      // 为每个订单获取详细的分配信息（限制并发数量以避免服务器压力）
      const batchSize = 5 // 每批处理5个订单
      const detailedAssignments = []
      
      for (let i = 0; i < response.data.length; i += batchSize) {
        const batch = response.data.slice(i, i + batchSize)
        const batchResults = await Promise.allSettled(
          batch.map(async (assignment) => {
            try {
              const orderId = assignment.orderId || assignment.OrderId
              if (!orderId) {
                console.warn('订单ID为空，跳过获取详情:', assignment)
                return {
                  ...assignment,
                  assignmentDetail: null,
                  error: '订单ID为空'
                }
              }
              
              const assignmentDetail = await getOrderAssignment(orderId)
              return {
                ...assignment,
                assignmentDetail: assignmentDetail?.data || null,
                error: null
              }
            } catch (error) {
              console.warn(`获取订单 ${assignment.orderId || assignment.OrderId} 分配详情失败:`, error)
              return {
                ...assignment,
                assignmentDetail: null,
                error: error.message || '获取详情失败'
              }
            }
          })
        )
        
        // 处理批次结果
        batchResults.forEach((result, index) => {
          if (result.status === 'fulfilled') {
            detailedAssignments.push(result.value)
          } else {
            console.error(`批次处理失败:`, result.reason)
            detailedAssignments.push({
              ...batch[index],
              assignmentDetail: null,
              error: '处理失败'
            })
          }
        })
      }
      
      currentRiderAssignments.value = detailedAssignments
      console.log('详细分配信息:', detailedAssignments)
      
      // 显示加载结果统计
      const successCount = detailedAssignments.filter(a => a.assignmentDetail).length
      const totalCount = detailedAssignments.length
      if (successCount < totalCount) {
        message.warning(`已加载 ${successCount}/${totalCount} 个订单的详细信息`)
      } else {
        message.success(`成功加载 ${totalCount} 个订单的详细信息`)
      }
    } else {
      currentRiderAssignments.value = []
      console.log('响应数据格式不正确或骑手暂无分配订单')
    }
  } catch (error) {
    console.error('获取分配详情失败:', error)
    
    // 根据错误类型提供更具体的错误信息
    let errorMessage = '获取分配详情失败'
    if (error.response?.status === 404) {
      errorMessage = '骑手不存在或无分配订单'
    } else if (error.response?.status === 401) {
      errorMessage = '认证失败，请重新登录'
    } else if (error.response?.status >= 500) {
      errorMessage = '服务器内部错误，请稍后重试'
    } else if (error.code === 'NETWORK_ERROR') {
      errorMessage = '网络连接失败，请检查网络设置'
    } else if (error.message) {
      errorMessage = error.message
    }
    
    message.error(errorMessage)
    currentRiderAssignments.value = []
  } finally {
    assignmentDetailLoading.value = false
  }
}

// 重试获取单个订单的分配详情
const retryGetAssignmentDetail = async (assignment) => {
  const orderId = assignment.orderId || assignment.OrderId
  if (!orderId) {
    message.error('订单ID无效，无法重试')
    return
  }
  
  try {
    console.log(`重试获取订单 ${orderId} 的分配详情`)
    const assignmentDetail = await getOrderAssignment(orderId)
    
    // 更新当前分配列表中的对应项
    const index = currentRiderAssignments.value.findIndex(a => 
      (a.orderId || a.OrderId) === orderId
    )
    
    if (index !== -1) {
      currentRiderAssignments.value[index] = {
        ...assignment,
        assignmentDetail: assignmentDetail?.data || null,
        error: null
      }
      message.success(`订单 ${orderId} 详情获取成功`)
    }
  } catch (error) {
    console.error(`重试获取订单 ${orderId} 详情失败:`, error)
    message.error(`重试失败: ${error.message || '未知错误'}`)
  }
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

.space-y-4 > * + * {
  margin-top: 16px;
}

.font-medium {
  font-weight: 500;
}

.text-gray-800 {
  color: #1f2937;
}

.text-gray-600 {
  color: #4b5563;
}

.text-gray-500 {
  color: #6b7280;
}

.text-gray-400 {
  color: #9ca3af;
}

.text-sm {
  font-size: 14px;
}

.bg-gray-50 {
  background-color: #f9fafb;
}

.bg-gray-100 {
  background-color: #f3f4f6;
}

.bg-blue-50 {
  background-color: #eff6ff;
}

.border {
  border-width: 1px;
}

.border-gray-200 {
  border-color: #e5e7eb;
}

.rounded-lg {
  border-radius: 8px;
}

.rounded {
  border-radius: 4px;
}

.p-3 {
  padding: 12px;
}

.p-4 {
  padding: 16px;
}

.py-8 {
  padding-top: 32px;
  padding-bottom: 32px;
}

.mt-1 {
  margin-top: 4px;
}

.mt-2 {
  margin-top: 8px;
}

.mt-4 {
  margin-top: 16px;
}

.mb-3 {
  margin-bottom: 12px;
}

.text-center {
  text-align: center;
}

.hover\:bg-gray-50:hover {
  background-color: #f9fafb;
}

.transition-colors {
  transition-property: color, background-color, border-color, text-decoration-color, fill, stroke;
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
  transition-duration: 150ms;
}
</style>