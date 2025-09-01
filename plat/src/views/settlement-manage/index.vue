<template>
  <div class="settlement-manage-container min-h-screen p-6">
    <!-- 页面标题 -->
    <div class="mb-8">
      <div class="flex items-center gap-4 mb-2">
        <n-avatar 
          :size="48"
          style="background: linear-gradient(135deg, #722ed1 0%, #531dab 100%);"
        >
          <n-icon size="24">
            <WalletOutline />
          </n-icon>
        </n-avatar>
        <div>
          <h1 class="text-3xl font-bold text-gray-800">结算管理</h1>
          <p class="text-gray-600 mt-1">管理和查看平台所有结算记录</p>
        </div>
      </div>
    </div>

    <!-- 结算统计卡片区域 -->
    <n-grid :cols="6" :x-gap="16" :y-gap="16" class="mb-6">
      <n-gi>
        <n-card :bordered="false" class="shadow-sm hover:shadow-lg transition-shadow duration-300">
          <n-statistic
            label="结算单总数"
            :value="settlementStats.total"
            value-style="color: #722ed1; font-weight: bold;"
          >
            <template #prefix>
              <n-icon size="20" color="#722ed1">
                <ReceiptOutline />
              </n-icon>
            </template>
          </n-statistic>
        </n-card>
      </n-gi>
      <n-gi>
        <n-card :bordered="false" class="shadow-sm hover:shadow-lg transition-shadow duration-300">
          <n-statistic
            label="已结算"
            :value="settlementStats.settledCount"
            value-style="color: #52c41a; font-weight: bold;"
          >
            <template #prefix>
              <n-icon size="20" color="#52c41a">
                <CheckmarkCircleOutline />
              </n-icon>
            </template>
          </n-statistic>
        </n-card>
      </n-gi>
      <n-gi>
        <n-card :bordered="false" class="shadow-sm hover:shadow-lg transition-shadow duration-300">
          <n-statistic
            label="待结算"
            :value="settlementStats.pendingCount"
            value-style="color: #faad14; font-weight: bold;"
          >
            <template #prefix>
              <n-icon size="20" color="#faad14">
                <TimeOutline />
              </n-icon>
            </template>
          </n-statistic>
        </n-card>
      </n-gi>
      <n-gi>
        <n-card :bordered="false" class="shadow-sm hover:shadow-lg transition-shadow duration-300">
          <n-statistic
            label="总毛收入"
            :value="formatAmount(settlementStats.totalGrossAmount)"
            value-style="color: #52c41a; font-weight: bold;"
          >
            <template #prefix>
              <n-icon size="20" color="#52c41a">
                <WalletOutline />
              </n-icon>
            </template>
          </n-statistic>
        </n-card>
      </n-gi>
      <n-gi>
        <n-card :bordered="false" class="shadow-sm hover:shadow-lg transition-shadow duration-300">
          <n-statistic
            label="总抽佣"
            :value="formatAmount(settlementStats.totalCommissionFee)"
            value-style="color: #fa8c16; font-weight: bold;"
          >
            <template #prefix>
              <n-icon size="20" color="#fa8c16">
                <CardOutline />
              </n-icon>
            </template>
          </n-statistic>
        </n-card>
      </n-gi>
      <n-gi>
        <n-card :bordered="false" class="shadow-sm hover:shadow-lg transition-shadow duration-300">
          <n-statistic
            label="平均抽佣率"
            :value="settlementStats.avgCommissionRate"
            value-style="color: #1890ff; font-weight: bold;"
          >
            <template #suffix>
              <span class="text-sm text-gray-500">%</span>
            </template>
            <template #prefix>
              <n-icon size="20" color="#1890ff">
                <StatsChartOutline />
              </n-icon>
            </template>
          </n-statistic>
        </n-card>
      </n-gi>
    </n-grid>

    <!-- 结算筛选区域 -->
    <n-card title="筛选条件" class="mb-6 shadow-sm" :bordered="false">
      <template #header-extra>
        <n-space>
          <n-button size="small" @click="handleSettlementSearch" type="primary">
            <template #icon>
              <n-icon>
                <SearchOutline />
              </n-icon>
            </template>
            搜索
          </n-button>
          <n-button size="small" @click="handleSettlementReset">
            <template #icon>
              <n-icon>
                <RefreshOutline />
              </n-icon>
            </template>
            重置
          </n-button>
        </n-space>
      </template>
      
      <n-form :model="settlementSearchParams" inline label-placement="left" label-width="80">
        <n-form-item label="商家名称">
          <n-input
            v-model:value="settlementSearchParams.merchantName"
            placeholder="输入商家名称"
            clearable
            style="width: 200px"
          />
        </n-form-item>
        
        <n-form-item label="起始日期">
          <n-date-picker
            v-model:value="settlementSearchParams.startDate"
            type="date"
            placeholder="选择起始日期"
            clearable
            style="width: 200px"
          />
        </n-form-item>
        
        <n-form-item label="结束日期">
          <n-date-picker
            v-model:value="settlementSearchParams.endDate"
            type="date"
            placeholder="选择结束日期"
            clearable
            style="width: 200px"
          />
        </n-form-item>
        
        <n-form-item label="结算状态">
          <n-select
            v-model:value="settlementSearchParams.isSettled"
            :options="settlementStatusOptions"
            placeholder="选择状态"
            clearable
            style="width: 150px"
          />
        </n-form-item>

        <n-form-item label="抽佣范围">
          <n-select
            v-model:value="settlementSearchParams.commissionRange"
            :options="commissionRangeOptions"
            placeholder="选择抽佣范围"
            clearable
            style="width: 150px"
          />
        </n-form-item>
      </n-form>
    </n-card>

    <!-- 结算单列表 -->
    <n-card :bordered="false" class="shadow-sm">
      <template #header>
        <div class="flex items-center gap-2">
          <n-icon size="18" color="#722ed1">
            <WalletOutline />
          </n-icon>
          <span class="font-medium">结算单列表</span>
        </div>
      </template>
      
      <template #header-extra>
        <n-space align="center">
          <n-text depth="3">
            显示 {{ filteredSettlementData.length }} / {{ settlementData.length }} 条记录
          </n-text>
          <n-button size="small" @click="exportSettlements" :loading="exportLoading">
            <template #icon>
              <n-icon>
                <DownloadOutline />
              </n-icon>
            </template>
            导出Excel
          </n-button>
          <n-button size="small" @click="handleSettlementRefresh" :loading="settlementLoading">
            <template #icon>
              <n-icon>
                <RefreshOutline />
              </n-icon>
            </template>
            刷新
          </n-button>
        </n-space>
      </template>
      
      <n-data-table
        :columns="settlementColumns"
        :data="filteredSettlementData"
        :loading="settlementLoading"
        :pagination="{ pageSize: 15, showSizePicker: true, pageSizes: [10, 15, 20, 50] }"
        flex-height
        class="min-h-500px"
        :row-class-name="() => 'hover:bg-purple-50 transition-colors duration-200'"
      />
    </n-card>

    <!-- 结算单详情弹窗 -->
    <n-modal 
      v-model:show="showSettlementDetailModal" 
      preset="card" 
      style="width: 900px; max-height: 85vh;" 
      class="rounded-2xl"
      :mask-closable="false"
    >
      <template #header>
        <div class="flex items-center gap-3">
          <n-avatar 
            :size="40"
            style="background: linear-gradient(135deg, #722ed1 0%, #531dab 100%);"
          >
            <n-icon size="20">
              <WalletOutline />
            </n-icon>
          </n-avatar>
          <div>
            <h3 class="text-lg font-semibold text-gray-800">
              结算单详情
            </h3>
            <p class="text-sm text-gray-500">
              单号: {{ currentSettlement.settleId || '-' }}
            </p>
          </div>
        </div>
      </template>

      <div v-if="!settlementDetailLoading" class="space-y-6">
        <!-- 结算概览 -->
        <div class="bg-gradient-to-r from-purple-50 to-indigo-50 p-4 rounded-lg">
          <n-grid :cols="4" :x-gap="20">
            <n-gi>
              <div class="text-center">
                <div class="text-2xl font-bold text-purple-600">
                  {{ currentSettlement.statusText || '待结算' }}
                </div>
                <div class="text-sm text-gray-600">结算状态</div>
              </div>
            </n-gi>
            <n-gi>
              <div class="text-center">
                <div class="text-2xl font-bold text-blue-600">
                  {{ currentSettlement.periodText || '-' }}
                </div>
                <div class="text-sm text-gray-600">结算周期</div>
              </div>
            </n-gi>
            <n-gi>
              <div class="text-center">
                <div class="text-2xl font-bold text-orange-600">
                  {{ currentSettlement.commissionRate || 0 }}%
                </div>
                <div class="text-sm text-gray-600">抽佣率</div>
              </div>
            </n-gi>
            <n-gi>
              <div class="text-center">
                <div class="text-xl font-bold text-green-600">
                  {{ currentSettlement.merchantName || '-' }}
                </div>
                <div class="text-sm text-gray-600">商家名称</div>
              </div>
            </n-gi>
          </n-grid>
        </div>

        <n-divider />

        <!-- 详细信息 -->
        <n-grid :cols="2" :x-gap="24" :y-gap="20">
          <!-- 基本信息 -->
          <n-gi>
            <n-card title="基本信息" size="small" class="h-full">
              <template #header-extra>
                <n-icon color="#722ed1">
                  <ReceiptOutline />
                </n-icon>
              </template>
              
              <div class="space-y-4">
                <div class="flex justify-between items-center py-2 border-b border-gray-100">
                  <span class="text-gray-600 font-medium">结算单号</span>
                  <n-text code style="word-break: break-all; max-width: 200px;">{{ currentSettlement.settleId || '-' }}</n-text>
                </div>
                
                <div class="flex justify-between items-center py-2 border-b border-gray-100">
                  <span class="text-gray-600 font-medium">商家名称</span>
                  <n-text strong>{{ currentSettlement.merchantName || '-' }}</n-text>
                </div>
                
                <div class="flex justify-between items-center py-2 border-b border-gray-100">
                  <span class="text-gray-600 font-medium">结算周期</span>
                  <div class="text-right">
                    <div class="text-sm font-medium">{{ currentSettlement.periodText || '-' }}</div>
                    <div class="text-xs text-gray-500">
                      {{ currentSettlement.periodStart ? new Date(currentSettlement.periodStart).toLocaleDateString('zh-CN') : '' }} - 
                      {{ currentSettlement.periodEnd ? new Date(currentSettlement.periodEnd).toLocaleDateString('zh-CN') : '' }}
                    </div>
                  </div>
                </div>
                
                <div class="flex justify-between items-center py-2 border-b border-gray-100">
                  <span class="text-gray-600 font-medium">结算状态</span>
                  <n-tag 
                    :type="getSettlementStatusType(currentSettlement.settledAt)"
                    size="small"
                  >
                    <template #icon>
                      <n-icon>
                        <CheckmarkCircleOutline v-if="currentSettlement.settledAt" />
                        <TimeOutline v-else />
                      </n-icon>
                    </template>
                    {{ currentSettlement.statusText || '待结算' }}
                  </n-tag>
                </div>
                
                <div class="flex justify-between items-center py-2">
                  <span class="text-gray-600 font-medium">结算时间</span>
                  <n-text>
                    {{ currentSettlement.settledAt ? 
                      new Date(currentSettlement.settledAt).toLocaleString('zh-CN') : 
                      '待结算' 
                    }}
                  </n-text>
                </div>
              </div>
            </n-card>
          </n-gi>
          
          <!-- 金额信息 -->
          <n-gi>
            <n-card title="金额明细" size="small" class="h-full">
              <template #header-extra>
                <n-icon color="#fa8c16">
                  <WalletOutline />
                </n-icon>
              </template>
              
              <div class="space-y-4">
                <div class="flex justify-between items-center py-2 border-b border-gray-100">
                  <span class="text-gray-600 font-medium flex items-center gap-2">
                    <n-icon size="16" color="#52c41a">
                      <WalletOutline />
                    </n-icon>
                    毛收入
                  </span>
                  <n-text strong style="color: #52c41a; font-size: 16px;">
                    ¥{{ formatAmount(currentSettlement.grossAmount) }}
                  </n-text>
                </div>
                
                <div class="flex justify-between items-center py-2 border-b border-gray-100">
                  <span class="text-gray-600 font-medium flex items-center gap-2">
                    <n-icon size="16" color="#fa8c16">
                      <CardOutline />
                    </n-icon>
                    平台抽佣
                  </span>
                  <div class="text-right">
                    <n-text strong style="color: #fa8c16; font-size: 16px;">
                      ¥{{ formatAmount(currentSettlement.commissionFee) }}
                    </n-text>
                    <div class="text-xs text-gray-500">
                      ({{ currentSettlement.commissionRate || 0 }}%)
                    </div>
                  </div>
                </div>
                
                <div class="flex justify-between items-center py-2 border-b border-gray-100">
                  <span class="text-gray-600 font-medium flex items-center gap-2">
                    <n-icon size="16" color="#1890ff">
                      <ReceiptOutline />
                    </n-icon>
                    应结金额
                  </span>
                  <n-text strong style="color: #1890ff; font-size: 18px;">
                    ¥{{ formatAmount(currentSettlement.netAmount) }}
                  </n-text>
                </div>
                
                <div class="bg-blue-50 p-3 rounded-lg mt-4">
                  <div class="text-center">
                    <div class="text-sm text-gray-600 mb-1">计算公式</div>
                    <div class="text-xs text-gray-500">
                      应结金额 = 毛收入 - 平台抽佣
                    </div>
                  </div>
                </div>
              </div>
            </n-card>
          </n-gi>
        </n-grid>

        <!-- 操作按钮 -->
        <div class="flex justify-end gap-3 pt-4 border-t">
          <n-button size="medium" @click="showSettlementDetailModal = false">
            关闭
          </n-button>
          <n-button 
            v-if="!currentSettlement.settledAt" 
            type="primary" 
            size="medium" 
            @click="handleProcessSettlement"
            :loading="processLoading"
          >
            <template #icon>
              <n-icon>
                <CheckmarkCircleOutline />
              </n-icon>
            </template>
            确认结算
          </n-button>
        </div>
      </div>
      
      <!-- 加载状态 -->
      <div v-else class="flex justify-center items-center h-80">
        <n-spin size="large">
          <template #description>
            <div class="text-center mt-4">
              <p class="text-gray-600">正在加载结算单详情...</p>
              <p class="text-sm text-gray-400 mt-1">请稍候片刻</p>
            </div>
          </template>
        </n-spin>
      </div>
    </n-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useMessage } from 'naive-ui'
import type { DataTableColumns } from 'naive-ui'
import {
  WalletOutline,
  ReceiptOutline,
  CheckmarkCircleOutline,
  TimeOutline,
  SearchOutline,
  RefreshOutline,
  StatsChartOutline,
  CardOutline,
  DownloadOutline
} from '@vicons/ionicons5'
import { getSettlements, getSettlementDetail } from '@/api'

// 结算单接口定义
interface SettlementItem {
  settleId: string
  merchantId: string
  merchantName: string
  startDate: string
  endDate: string
  grossAmount: number
  commissionFee: number
  netAmount: number
  settledAt?: string
  statusText?: string
  commissionRate?: number
}

const message = useMessage()

// 响应式数据
const settlementLoading = ref(false)
const settlementDetailLoading = ref(false)
const exportLoading = ref(false)
const processLoading = ref(false)
const showSettlementDetailModal = ref(false)

// 结算单数据
const settlementData = ref<SettlementItem[]>([])
const currentSettlement = ref<Partial<SettlementItem>>({})

// 搜索参数
const settlementSearchParams = reactive({
  merchantName: '',
  startDate: null as number | null,
  endDate: null as number | null,
  isSettled: null as boolean | null,
  commissionRange: null as string | null
})

// 筛选选项
const settlementStatusOptions = [
  { label: '全部', value: null },
  { label: '已结算', value: true },
  { label: '待结算', value: false }
]

const commissionRangeOptions = [
  { label: '全部', value: null },
  { label: '0-5%', value: '0-5' },
  { label: '5-10%', value: '5-10' },
  { label: '10-15%', value: '10-15' },
  { label: '15%以上', value: '15+' }
]

// 计算属性
const filteredSettlementData = computed(() => {
  let filtered = [...settlementData.value]
  
  // 商家名称筛选
  if (settlementSearchParams.merchantName) {
    const keyword = settlementSearchParams.merchantName.toLowerCase()
    filtered = filtered.filter(item => 
      item.merchantName?.toLowerCase().includes(keyword)
    )
  }
  
  // 日期范围筛选
  if (settlementSearchParams.startDate) {
    filtered = filtered.filter(item => 
      new Date(item.periodStart || 0).getTime() >= settlementSearchParams.startDate!
    )
  }
  
  if (settlementSearchParams.endDate) {
    filtered = filtered.filter(item => 
      new Date(item.periodEnd || 0).getTime() <= settlementSearchParams.endDate!
    )
  }
  
  // 结算状态筛选
  if (settlementSearchParams.isSettled !== null) {
    filtered = filtered.filter(item => 
      Boolean(item.settledAt) === settlementSearchParams.isSettled
    )
  }
  
  // 抽佣范围筛选
  if (settlementSearchParams.commissionRange) {
    const range = settlementSearchParams.commissionRange
    filtered = filtered.filter(item => {
      const rate = item.commissionRate || 0
      switch (range) {
        case '0-5': return rate >= 0 && rate <= 5
        case '5-10': return rate > 5 && rate <= 10
        case '10-15': return rate > 10 && rate <= 15
        case '15+': return rate > 15
        default: return true
      }
    })
  }
  
  return filtered
})

const settlementStats = computed(() => {
  const data = filteredSettlementData.value
  const settled = data.filter(item => item.settledAt)
  const pending = data.filter(item => !item.settledAt)
  
  const totalGross = data.reduce((sum, item) => sum + (item.grossAmount || 0), 0)
  const totalCommission = data.reduce((sum, item) => sum + (item.commissionFee || 0), 0)
  const avgRate = data.length > 0 
    ? Number((data.reduce((sum, item) => sum + (item.commissionRate || 0), 0) / data.length).toFixed(2))
    : 0
  
  return {
    total: data.length,
    settledCount: settled.length,
    pendingCount: pending.length,
    totalGrossAmount: totalGross,
    totalCommissionFee: totalCommission,
    avgCommissionRate: avgRate
  }
})

// 表格列定义
const settlementColumns: DataTableColumns<SettlementItem> = [
  {
    title: '序号',
    key: 'index',
    width: 80,
    render: (_, index) => index + 1
  },
  {
    title: '结算单号',
    key: 'settleId',
    width: 200,
    ellipsis: {
      tooltip: true
    }
  },
  {
    title: '商家名称',
    key: 'merchantName',
    width: 150,
    ellipsis: {
      tooltip: true
    }
  },
  {
    title: '结算周期',
    key: 'period',
    width: 180,
    render: (row) => row.periodText || '-'
  },
  {
    title: '毛收入(¥)',
    key: 'grossAmount',
    width: 120,
    align: 'right',
    render: (row) => formatAmount(row.grossAmount || 0)
  },
  {
    title: '抽佣(¥)',
    key: 'commissionFee',
    width: 120,
    align: 'right',
    render: (row) => formatAmount(row.commissionFee || 0)
  },
  {
    title: '抽佣率',
    key: 'commissionRate',
    width: 100,
    align: 'center',
    render: (row) => `${row.commissionRate || 0}%`
  },
  {
    title: '应结金额(¥)',
    key: 'netAmount',
    width: 120,
    align: 'right',
    render: (row) => formatAmount(row.netAmount || 0)
  },
  {
    title: '结算状态',
    key: 'status',
    width: 100,
    align: 'center',
    render: (row) => {
      const isSettled = Boolean(row.settledAt)
      return h(
        'n-tag',
        {
          type: isSettled ? 'success' : 'warning',
          size: 'small'
        },
        () => isSettled ? '已结算' : '待结算'
      )
    }
  },
  {
    title: '结算时间',
    key: 'settledAt',
    width: 180,
    render: (row) => {
      return row.settledAt 
        ? new Date(row.settledAt).toLocaleString('zh-CN')
        : '-'
    }
  },
  {
    title: '操作',
    key: 'actions',
    width: 100,
    align: 'center',
    fixed: 'right',
    render: (row) => {
      return h(
        'n-button',
        {
          size: 'small',
          type: 'primary',
          ghost: true,
          onClick: () => handleViewSettlementDetail(row)
        },
        () => '详情'
      )
    }
  }
]

// 工具函数
const formatAmount = (amount: number): string => {
  return new Intl.NumberFormat('zh-CN', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

const getSettlementStatusType = (settledAt?: string) => {
  return settledAt ? 'success' : 'warning'
}

// 事件处理函数
const handleSettlementSearch = () => {
  // 筛选逻辑已在计算属性中实现
  message.success('搜索完成')
}

const handleSettlementReset = () => {
  Object.assign(settlementSearchParams, {
    merchantName: '',
    startDate: null,
    endDate: null,
    isSettled: null,
    commissionRange: null
  })
  message.info('筛选条件已重置')
}

const handleSettlementRefresh = async () => {
  await loadSettlements()
  message.success('数据已刷新')
}

const handleViewSettlementDetail = async (settlement: SettlementItem) => {
  try {
    settlementDetailLoading.value = true
    showSettlementDetailModal.value = true
    
    // 获取详细信息
    const detail = await getSettlementDetail(settlement.settleId!)
    currentSettlement.value = {
      ...settlement,
      ...detail,
      statusText: settlement.settledAt ? '已结算' : '待结算'
    }
  } catch (error) {
    message.error('获取结算单详情失败')
    console.error('获取结算单详情失败:', error)
  } finally {
    settlementDetailLoading.value = false
  }
}

const handleProcessSettlement = async () => {
  try {
    processLoading.value = true
    
    // 这里调用确认结算的API
    // await processSettlement(currentSettlement.value.settleId!)
    
    message.success('结算处理成功')
    showSettlementDetailModal.value = false
    await loadSettlements()
  } catch (error) {
    message.error('结算处理失败')
    console.error('结算处理失败:', error)
  } finally {
    processLoading.value = false
  }
}

const exportSettlements = async () => {
  try {
    exportLoading.value = true
    
    // 这里实现导出Excel功能
    // const blob = await exportSettlementsToExcel(filteredSettlementData.value)
    // downloadFile(blob, `结算单列表_${new Date().toLocaleDateString()}.xlsx`)
    
    message.success('导出成功')
  } catch (error) {
    message.error('导出失败')
    console.error('导出失败:', error)
  } finally {
    exportLoading.value = false
  }
}

// 数据加载
const loadSettlements = async () => {
  try {
    settlementLoading.value = true
    
    const response = await getSettlements({})
    settlementData.value = response.settlements.map(item => ({
      ...item,
      periodText: formatPeriodText(item.periodStart, item.periodEnd)
    }))
  } catch (error) {
    message.error('加载结算数据失败')
    console.error('加载结算数据失败:', error)
  } finally {
    settlementLoading.value = false
  }
}

const formatPeriodText = (start?: string, end?: string): string => {
  if (!start || !end) return '-'
  
  const startDate = new Date(start)
  const endDate = new Date(end)
  
  const startStr = startDate.toLocaleDateString('zh-CN')
  const endStr = endDate.toLocaleDateString('zh-CN')
  
  return `${startStr} ~ ${endStr}`
}

// 生命周期
onMounted(() => {
  loadSettlements()
})
</script>

<style scoped>
.settlement-manage-container {
  background: linear-gradient(135deg, #f6ffed 0%, #f0f9ff 100%);
  min-height: 100vh;
}

/* 统计卡片悬停效果 */
:deep(.n-statistic) {
  .n-statistic__value {
    transition: all 0.3s ease;
  }
}

:deep(.n-card:hover .n-statistic__value) {
  transform: scale(1.05);
}

/* 表格行悬停效果 */
:deep(.n-data-table-tbody .hover\:bg-purple-50:hover) {
  background-color: rgb(250 245 255) !important;
}

/* 搜索表单样式 */
:deep(.n-form-item) {
  margin-bottom: 0;
}

/* 弹窗样式 */
:deep(.n-modal-card) {
  border-radius: 16px;
}

/* 渐变背景 */
.settlement-manage-container::before {
  content: '';
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: 
    radial-gradient(circle at 20% 20%, rgba(114, 46, 209, 0.05) 0%, transparent 50%),
    radial-gradient(circle at 80% 80%, rgba(16, 185, 129, 0.05) 0%, transparent 50%);
  pointer-events: none;
  z-index: -1;
}
</style>
