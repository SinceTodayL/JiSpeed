<script setup lang="tsx">
import { ref, onMounted, computed } from 'vue';
import { 
  NButton, 
  NTag, 
  NModal, 
  NForm, 
  NFormItem, 
  NSelect, 
  NInput, 
  NCard, 
  NDataTable, 
  NBadge, 
  NCheckbox, 
  NInputNumber, 
  NStatistic,
  NIcon,
  NAvatar,
  NDivider,
  NGrid,
  NGi,
  NSpace,
  useMessage 
} from 'naive-ui';
import type { DataTableColumns } from 'naive-ui';
import { 
  WarningOutline,
  SearchOutline,
  RefreshOutline,
  DocumentTextOutline,
  PersonOutline,
  BusinessOutline,
  BicycleOutline,
  TimeOutline,
  CheckmarkCircleOutline,
  CloseCircleOutline,
  InformationCircleOutline,
  WalletOutline,
  ChatbubbleOutline,
  EyeOutline,
  SettingsOutline
} from '@vicons/ionicons5';
import { fetchComplaintList, fetchComplaintDetail, auditComplaint } from '@/api/complaint';

defineOptions({
  name: 'ComplaintHandle'
});

const message = useMessage();
const loading = ref(false);
const tableData = ref<any[]>([]);
const showModal = ref(false);
const currentItem = ref<any>({});

// 筛选表单
const searchForm = ref({
  userId: '',
  merchantId: '',
  status: null,
  size: null, // 不设置默认分页，获取所有数据
  page: null
});

// 审批表单
const auditForm = ref({
  status: '',
  reason: '',
  remark: '',
  needRefund: false,
  refundAmount: 0
});

// 状态选项
const statusOptions = [
  { label: '投诉成立', value: 'approved' },
  { label: '投诉不成立', value: 'rejected' },
  { label: '需要进一步调查', value: 'investigating' }
];

// 投诉状态筛选选项
const complaintStatusFilterOptions = [
  { label: '全部状态', value: null },
  { label: '待处理', value: 0 },
  { label: '处理中', value: 1 },
  { label: '已处理', value: 2 }
];

// 投诉统计数据
const complaintStats = computed(() => {
  const total = tableData.value.length;
  const pendingCount = tableData.value.filter(complaint => complaint.cmplStatus === 0).length;
  const processingCount = tableData.value.filter(complaint => complaint.cmplStatus === 1).length;
  const completedCount = tableData.value.filter(complaint => complaint.cmplStatus === 2).length;
  const merchantComplaints = tableData.value.filter(complaint => complaint.cmplRole === 1).length;
  const riderComplaints = tableData.value.filter(complaint => complaint.cmplRole === 2).length;
  
  return {
    total,
    pendingCount,
    processingCount,
    completedCount,
    merchantComplaints,
    riderComplaints,
    completionRate: total > 0 ? Math.round((completedCount / total) * 100) : 0,
    urgentCount: pendingCount + processingCount
  };
});

// 过滤后的表格数据
const filteredTableData = computed(() => {
  return tableData.value.filter(complaint => {
    if (searchForm.value.userId && !complaint.complainantId?.includes(searchForm.value.userId)) {
      return false;
    }
    if (searchForm.value.merchantId && !complaint.merchantId?.includes(searchForm.value.merchantId)) {
      return false;
    }
    if (searchForm.value.status !== null && complaint.cmplStatus !== searchForm.value.status) {
      return false;
    }
    return true;
  });
});

// 获取投诉列表数据
async function getComplaintList() {
  try {
    loading.value = true;
    
    // 构建查询参数，只传递非空值
    const params = {};
    if (searchForm.value.userId && searchForm.value.userId.trim()) params.userId = searchForm.value.userId.trim();
    if (searchForm.value.merchantId && searchForm.value.merchantId.trim()) params.merchantId = searchForm.value.merchantId.trim();
    if (searchForm.value.status !== null && searchForm.value.status !== undefined) params.status = searchForm.value.status;
    if (searchForm.value.size && searchForm.value.size > 0) params.size = searchForm.value.size;
    if (searchForm.value.page && searchForm.value.page > 0) params.page = searchForm.value.page;
    
    const response = await fetchComplaintList(params);
    
    console.log('API响应:', response);
    
    if (response && response.data) {
      // 后端返回的是投诉ID列表，需要逐个获取详情
      if (Array.isArray(response.data)) {
        console.log('获取到投诉ID列表，共', response.data.length, '条:', response.data);
        
        if (response.data.length === 0) {
          console.log('投诉列表为空');
          tableData.value = [];
        } else {
          // 批量获取投诉详情
          try {
            const detailPromises = response.data.map(async (id) => {
              try {
                console.log('正在获取投诉详情，ID:', id);
                const detail = await fetchComplaintDetail(id);
                console.log(`投诉 ${id} 详情:`, detail);
                return detail?.data || null;
              } catch (error) {
                console.error(`获取投诉详情失败 ${id}:`, error);
                return null;
              }
            });
            
            const details = await Promise.all(detailPromises);
            const validDetails = details.filter(Boolean);
            console.log('成功获取投诉详情数量:', validDetails.length, '详情:', validDetails);
            tableData.value = validDetails;
          } catch (error) {
            console.error('批量获取投诉详情失败:', error);
            message.error('获取投诉详情失败');
            tableData.value = [];
          }
        }
      } else {
        // 如果返回的不是数组，可能是单个对象或其他格式
        console.log('获取到非数组投诉数据:', response.data);
        tableData.value = [response.data];
      }
    } else {
      console.log('没有获取到投诉数据，响应:', response);
      tableData.value = [];
    }
  } catch (error) {
    message.error('获取投诉列表失败: ' + error.message);
    console.error('Error fetching complaint list:', error);
    tableData.value = [];
  } finally {
    loading.value = false;
  }
}

// 投诉列表列配置
const columns: DataTableColumns = [
  {
    key: 'index',
    title: '序号',
    align: 'center',
    width: 70,
    render: (row, index) => (
      <n-badge value={index + 1} type="error" />
    )
  },
  {
    key: 'complaintInfo',
    title: '投诉信息',
    width: 200,
    render: (row) => (
      <div class="flex items-center gap-12px">
        <n-avatar
          size="medium"
          style={{
            backgroundColor: row.cmplStatus === 0 ? '#ff4d4f' : row.cmplStatus === 1 ? '#faad14' : '#52c41a'
          }}
        >
          <n-icon>
            <WarningOutline />
          </n-icon>
        </n-avatar>
        <div class="flex flex-col">
          <n-text strong style="font-size: 12px;">ID: {row.complaintId?.slice(-8) || '-'}</n-text>
          <n-text depth="3" style="font-size: 11px;">
            订单: {row.orderId?.slice(-8) || '-'}
          </n-text>
        </div>
      </div>
    )
  },
  {
    key: 'cmplRole',
    title: '投诉类型',
    align: 'center',
    width: 120,
    render: (row) => {
      const roleConfig = {
        1: { type: 'warning', icon: BusinessOutline, color: '#fa8c16' },
        2: { type: 'info', icon: BicycleOutline, color: '#1890ff' }
      };
      const config = roleConfig[row.cmplRole] || roleConfig[1];
      return (
        <div class="flex items-center justify-center gap-4px">
          <n-icon color={config.color} size="14">
            <config.icon />
          </n-icon>
          <n-tag type={config.type} size="small">
            {getCmplRoleText(row.cmplRole)}
          </n-tag>
        </div>
      );
    }
  },
  {
    key: 'cmplDescription',
    title: '投诉内容',
    width: 200,
    render: (row) => (
      <div class="flex items-center gap-4px">
        <n-icon color="#722ed1" size="14">
          <ChatbubbleOutline />
        </n-icon>
        <n-text 
          style="font-size: 12px; max-width: 150px; word-break: break-all;" 
          depth="2"
        >
          {row.cmplDescription ? row.cmplDescription.slice(0, 30) + (row.cmplDescription.length > 30 ? '...' : '') : '无描述'}
        </n-text>
      </div>
    )
  },
  {
    key: 'complainantInfo',
    title: '投诉人',
    align: 'center',
    width: 140,
    render: (row) => (
      <div class="flex items-center justify-center gap-4px">
        <n-icon color="#722ed1" size="14">
          <PersonOutline />
        </n-icon>
        <n-text style="font-size: 12px;" depth="3">
          {row.complainantId?.slice(-8) || '未知'}
        </n-text>
      </div>
    )
  },
  {
    key: 'createdAt',
    title: '投诉时间',
    align: 'center',
    width: 140,
    render: (row) => (
      <div class="flex items-center justify-center gap-4px">
        <n-icon color="#52c41a" size="14">
          <TimeOutline />
        </n-icon>
        <n-text style="font-size: 11px;" depth="3">
          {formatDateTime(row.createdAt)}
        </n-text>
      </div>
    )
  },
  {
    key: 'cmplStatus',
    title: '处理状态',
    align: 'center',
    width: 120,
    render: (row) => {
      const statusConfig = {
        0: { type: 'error', icon: CloseCircleOutline, color: '#ff4d4f' },
        1: { type: 'warning', icon: InformationCircleOutline, color: '#faad14' },
        2: { type: 'success', icon: CheckmarkCircleOutline, color: '#52c41a' }
      };
      const config = statusConfig[row.cmplStatus] || statusConfig[0];
      return (
        <div class="flex items-center justify-center gap-4px">
          <n-icon color={config.color} size="16">
            <config.icon />
          </n-icon>
          <n-tag type={config.type} size="small">
            {getCmplStatusText(row.cmplStatus)}
          </n-tag>
        </div>
      );
    }
  },
  {
    key: 'actions',
    title: '操作',
    align: 'center',
    width: 120,
    render: (row) => (
      <n-button 
        type={row.cmplStatus === 2 ? 'default' : 'error'} 
        size="small" 
        onClick={() => handleComplaint(row)}
        style="border-radius: 6px;"
      >
        <div style="display: flex; align-items: center; gap: 4px;">
          <n-icon size="14">
            {row.cmplStatus === 2 ? <EyeOutline /> : <SettingsOutline />}
          </n-icon>
          {row.cmplStatus === 2 ? '查看详情' : '处理投诉'}
        </div>
      </n-button>
    )
  }
];

// 根据 CmplRole 获取投诉对象文本
function getCmplRoleText(cmplRole: number) {
  const roleMap = { 1: '投诉商家', 2: '投诉骑手' };
  return roleMap[cmplRole] || '未知投诉';
}

// 根据 CmplStatus 获取状态文本
function getCmplStatusText(cmplStatus: number) {
  const statusMap = { 0: '待处理', 1: '处理中', 2: '已处理' };
  return statusMap[cmplStatus] || '未知状态';
}

// 格式化日期时间
function formatDateTime(dateTime: string) {
  if (!dateTime) return '-';
  try {
    return new Date(dateTime).toLocaleString('zh-CN', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    });
  } catch {
    return dateTime;
  }
}

// 处理投诉
function handleComplaint(complaint: any) {
  currentItem.value = complaint;
  auditForm.value = {
    status: '',
    reason: '',
    remark: '',
    needRefund: false,
    refundAmount: 0
  };
  showModal.value = true;
}

// 搜索投诉
function handleSearch() {
  message.success(`找到 ${filteredTableData.value.length} 条投诉记录`);
}

// 重置搜索条件
function handleReset() {
  searchForm.value = {
    userId: '',
    merchantId: '',
    status: null,
    size: null,
    page: null
  };
  message.info('搜索条件已重置');
}

// 刷新数据
function handleRefresh() {
  getComplaintList();
}

// 提交投诉处理
async function submitComplaintHandle() {
  if (!auditForm.value.status) {
    message.warning('请选择处理结果');
    return;
  }

  if (auditForm.value.needRefund && auditForm.value.refundAmount <= 0) {
    message.warning('请输入有效的退款金额');
    return;
  }

  try {
    const adminId = '6f7af74d972c481c91f19596e07aae3a';
    const auditData = {
      Status: auditForm.value.status,
      Reason: auditForm.value.reason,
      Remark: auditForm.value.remark,
      NeedRefund: auditForm.value.needRefund,
      RefundAmount: auditForm.value.needRefund ? auditForm.value.refundAmount : 0
    };

    await auditComplaint(adminId, currentItem.value.complaintId, auditData);
    
    if (auditForm.value.needRefund) {
      message.success(`投诉处理完成，已为用户退款 ¥${auditForm.value.refundAmount}`);
    } else {
      message.success('投诉处理完成');
    }
    
    showModal.value = false;
    getComplaintList(); // 刷新列表
  } catch (error) {
    message.error('处理失败: ' + error.message);
  }
}

// 组件挂载时获取数据
onMounted(() => {
  getComplaintList();
});
</script>

<template>
  <div class="p-6 min-h-full bg-gradient-to-br from-red-50 to-purple-50">
    <!-- 页面标题 -->
    <div class="mb-6">
      <h1 class="text-2xl font-bold text-gray-800 mb-2 flex items-center gap-2">
        <n-icon size="24" color="#ff4d4f">
          <WarningOutline />
        </n-icon>
        投诉处理中心
      </h1>
      <p class="text-gray-600">处理用户投诉，维护平台服务质量</p>
    </div>

    <!-- 统计卡片区域 -->
    <n-grid :cols="6" :x-gap="16" :y-gap="16" class="mb-6">
      <n-gi>
        <n-card :bordered="false" class="shadow-sm hover:shadow-lg transition-shadow duration-300">
          <n-statistic
            label="总投诉数"
            :value="complaintStats.total"
            value-style="color: #ff4d4f; font-weight: bold;"
          >
            <template #prefix>
              <n-icon size="20" color="#ff4d4f">
                <WarningOutline />
              </n-icon>
            </template>
          </n-statistic>
        </n-card>
      </n-gi>
      <n-gi>
        <n-card :bordered="false" class="shadow-sm hover:shadow-lg transition-shadow duration-300">
          <n-statistic
            label="待处理"
            :value="complaintStats.pendingCount"
            value-style="color: #ff4d4f; font-weight: bold;"
          >
            <template #prefix>
              <n-icon size="20" color="#ff4d4f">
                <CloseCircleOutline />
              </n-icon>
            </template>
          </n-statistic>
        </n-card>
      </n-gi>
      <n-gi>
        <n-card :bordered="false" class="shadow-sm hover:shadow-lg transition-shadow duration-300">
          <n-statistic
            label="处理中"
            :value="complaintStats.processingCount"
            value-style="color: #faad14; font-weight: bold;"
          >
            <template #prefix>
              <n-icon size="20" color="#faad14">
                <InformationCircleOutline />
              </n-icon>
            </template>
          </n-statistic>
        </n-card>
      </n-gi>
      <n-gi>
        <n-card :bordered="false" class="shadow-sm hover:shadow-lg transition-shadow duration-300">
          <n-statistic
            label="已处理"
            :value="complaintStats.completedCount"
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
            label="商家投诉"
            :value="complaintStats.merchantComplaints"
            value-style="color: #fa8c16; font-weight: bold;"
          >
            <template #prefix>
              <n-icon size="20" color="#fa8c16">
                <BusinessOutline />
              </n-icon>
            </template>
          </n-statistic>
        </n-card>
      </n-gi>
      <n-gi>
        <n-card :bordered="false" class="shadow-sm hover:shadow-lg transition-shadow duration-300">
          <n-statistic
            label="处理率"
            :value="complaintStats.completionRate"
            value-style="color: #722ed1; font-weight: bold;"
          >
            <template #suffix>
              <span class="text-sm text-gray-500">%</span>
            </template>
            <template #prefix>
              <n-icon size="20" color="#722ed1">
                <CheckmarkCircleOutline />
              </n-icon>
            </template>
          </n-statistic>
        </n-card>
      </n-gi>
    </n-grid>

    <!-- 搜索筛选区域 -->
    <n-card title="筛选条件" class="mb-6 shadow-sm" :bordered="false">
      <template #header-extra>
        <n-space>
          <n-button size="small" @click="handleSearch" type="primary">
            <template #icon>
              <n-icon>
                <SearchOutline />
              </n-icon>
            </template>
            搜索
          </n-button>
          <n-button size="small" @click="handleReset">
            <template #icon>
              <n-icon>
                <RefreshOutline />
              </n-icon>
            </template>
            重置
          </n-button>
        </n-space>
      </template>
      
      <n-form :model="searchForm" inline label-placement="left" label-width="80">
        <n-form-item label="投诉人ID">
          <n-input 
            v-model:value="searchForm.userId" 
            placeholder="输入用户ID"
            clearable
            style="width: 150px"
          />
        </n-form-item>
        <n-form-item label="商家ID">
          <n-input 
            v-model:value="searchForm.merchantId" 
            placeholder="输入商家ID"
            clearable
            style="width: 150px"
          />
        </n-form-item>
        <n-form-item label="处理状态">
          <n-select 
            v-model:value="searchForm.status" 
            :options="complaintStatusFilterOptions"
            placeholder="选择状态"
            clearable
            style="width: 120px"
          />
        </n-form-item>
      </n-form>
    </n-card>

    <!-- 投诉列表 -->
    <n-card :bordered="false" class="shadow-sm">
      <template #header>
        <div class="flex items-center gap-2">
          <n-icon size="18" color="#ff4d4f">
            <WarningOutline />
          </n-icon>
          <span class="font-medium">投诉列表</span>
          <n-badge 
            v-if="complaintStats.urgentCount > 0"
            :value="complaintStats.urgentCount" 
            type="error" 
            style="margin-left: 8px;"
          />
        </div>
      </template>
      
      <template #header-extra>
        <n-space align="center">
          <n-text depth="3">
            显示 {{ filteredTableData.length }} / {{ tableData.length }} 条记录
          </n-text>
          <n-button size="small" @click="handleRefresh" :loading="loading">
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
        :columns="columns"
        :data="filteredTableData"
        :loading="loading"
        :pagination="{ pageSize: 10, showSizePicker: true, pageSizes: [10, 20, 50] }"
        flex-height
        class="min-h-400px"
        :row-class-name="(row) => {
          if (row.cmplStatus === 0) return 'urgent-row';
          if (row.cmplStatus === 1) return 'processing-row';
          return 'completed-row';
        }"
      />
    </n-card>

    <!-- 投诉处理弹窗 -->
    <NModal 
      v-model:show="showModal" 
      preset="card" 
      style="width: 800px; max-height: 80vh; background-color: #ffffff;" 
      class="rounded-2xl complaint-modal"
      :mask-closable="false"
    >
      <template #header>
        <div class="flex items-center gap-3">
          <n-avatar 
            :size="40"
            :style="{
              backgroundColor: currentItem.cmplStatus === 0 ? '#ff4d4f' : currentItem.cmplStatus === 1 ? '#faad14' : '#52c41a'
            }"
          >
            <n-icon size="20">
              <WarningOutline />
            </n-icon>
          </n-avatar>
          <div>
            <h3 class="text-lg font-semibold text-gray-800">
              投诉处理 - {{ getCmplRoleText(currentItem.cmplRole) }}
            </h3>
            <p class="text-sm text-gray-500">
              ID: {{ currentItem.complaintId || '-' }}
            </p>
          </div>
        </div>
      </template>

      <div class="space-y-6">
        <!-- 投诉详情概览 -->
        <div class="bg-white p-4 rounded-lg border border-red-200 shadow-sm">
          <h4 class="text-gray-800 font-medium mb-4 flex items-center gap-2">
            <n-icon color="#ff4d4f">
              <DocumentTextOutline />
            </n-icon>
            投诉详情
          </h4>
          
          <n-grid :cols="3" :x-gap="20" :y-gap="12">
            <n-gi>
              <div class="text-center p-3 bg-purple-50 rounded-lg border border-purple-200">
                <div class="text-lg font-bold text-purple-700">
                  {{ getCmplStatusText(currentItem.cmplStatus) }}
                </div>
                <div class="text-sm text-gray-700 font-medium">当前状态</div>
              </div>
            </n-gi>
            <n-gi>
              <div class="text-center p-3 bg-blue-50 rounded-lg border border-blue-200">
                <div class="text-lg font-bold text-blue-700">
                  {{ getCmplRoleText(currentItem.cmplRole) }}
                </div>
                <div class="text-sm text-gray-700 font-medium">投诉类型</div>
              </div>
            </n-gi>
            <n-gi>
              <div class="text-center p-3 bg-green-50 rounded-lg border border-green-200">
                <div class="text-lg font-bold text-green-700">
                  {{ formatDateTime(currentItem.createdAt).split(' ')[0] }}
                </div>
                <div class="text-sm text-gray-700 font-medium">投诉时间</div>
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
                <n-icon color="#ff4d4f">
                  <InformationCircleOutline />
                </n-icon>
              </template>
              
              <div class="space-y-4">
                <div class="flex justify-between items-center py-2 border-b border-gray-100">
                  <span class="text-gray-700 font-medium">投诉ID</span>
                  <n-text code style="word-break: break-all; max-width: 200px; background-color: #f7fafc; color: #2d3748; padding: 2px 6px; border-radius: 4px;">
                    {{ currentItem.complaintId || '-' }}
                  </n-text>
                </div>
                
                <div class="flex justify-between items-center py-2 border-b border-gray-100">
                  <span class="text-gray-700 font-medium">订单ID</span>
                  <n-text code style="word-break: break-all; max-width: 200px; background-color: #f7fafc; color: #2d3748; padding: 2px 6px; border-radius: 4px;">
                    {{ currentItem.orderId || '-' }}
                  </n-text>
                </div>
                
                <div class="flex justify-between items-center py-2 border-b border-gray-100">
                  <span class="text-gray-700 font-medium">投诉人ID</span>
                  <n-text code style="word-break: break-all; max-width: 200px; background-color: #f7fafc; color: #2d3748; padding: 2px 6px; border-radius: 4px;">
                    {{ currentItem.complainantId || '-' }}
                  </n-text>
                </div>
                
                <div class="flex justify-between items-center py-2">
                  <span class="text-gray-700 font-medium">投诉时间</span>
                  <n-text style="color: #2d3748; font-weight: 500;">{{ formatDateTime(currentItem.createdAt) || '-' }}</n-text>
                </div>
              </div>
            </n-card>
          </n-gi>
          
          <!-- 投诉内容 -->
          <n-gi>
            <n-card title="投诉内容" size="small" class="h-full">
              <template #header-extra>
                <n-icon color="#722ed1">
                  <ChatbubbleOutline />
                </n-icon>
              </template>
              
              <div class="p-4 bg-white rounded-lg min-h-32 border border-gray-200 shadow-sm">
                <n-text style="color: #2d3748; font-weight: 500; line-height: 1.6;">
                  {{ currentItem.cmplDescription || '无详细投诉描述' }}
                </n-text>
              </div>
            </n-card>
          </n-gi>
        </n-grid>

        <n-divider />

        <!-- 处理表单 -->
        <div v-if="currentItem.cmplStatus !== 2">
          <h4 class="text-gray-800 font-medium mb-4 flex items-center gap-2">
            <n-icon color="#1890ff">
              <SettingsOutline />
            </n-icon>
            处理操作
          </h4>
          
          <NForm :model="auditForm" label-placement="top">
            <n-grid :cols="2" :x-gap="16">
              <n-gi>
                <NFormItem label="处理结果" required>
                  <NSelect 
                    v-model:value="auditForm.status" 
                    :options="statusOptions"
                    placeholder="请选择处理结果"
                    size="large"
                  />
                </NFormItem>
              </n-gi>
              <n-gi>
                <NFormItem label="处理原因">
                  <NInput 
                    v-model:value="auditForm.reason" 
                    placeholder="请输入处理原因"
                    size="large"
                  />
                </NFormItem>
              </n-gi>
            </n-grid>
            
            <NFormItem label="退款设置">
              <div class="flex items-center space-x-4">
                <NCheckbox v-model:checked="auditForm.needRefund">
                  <span class="flex items-center gap-2">
                    <n-icon color="#fa8c16">
                      <WalletOutline />
                    </n-icon>
                    需要为用户退款
                  </span>
                </NCheckbox>
              </div>
            </NFormItem>
            
            <NFormItem v-if="auditForm.needRefund" label="退款金额" required>
              <NInputNumber 
                v-model:value="auditForm.refundAmount"
                placeholder="请输入退款金额"
                :min="0.01"
                :max="10000"
                :precision="2"
                size="large"
                style="width: 100%"
              >
                <template #prefix>¥</template>
              </NInputNumber>
            </NFormItem>
            
            <NFormItem label="备注说明">
              <NInput 
                v-model:value="auditForm.remark" 
                type="textarea" 
                placeholder="请输入处理备注或说明"
                :rows="4"
                show-count
                maxlength="200"
              />
            </NFormItem>
          </NForm>

          <div class="flex justify-end gap-4 mt-6 pt-4 border-t border-gray-200">
            <n-button @click="showModal = false" size="large">
              取消
            </n-button>
            <n-button 
              type="primary" 
              @click="submitComplaintHandle"
              size="large"
              style="background: linear-gradient(135deg, #ff4d4f, #ff7875); border: none;"
            >
              <template #icon>
                <n-icon>
                  <CheckmarkCircleOutline />
                </n-icon>
              </template>
              确认处理
            </n-button>
          </div>
        </div>
        
        <!-- 已处理状态显示 -->
        <div v-else class="text-center py-8">
          <n-icon size="48" color="#52c41a">
            <CheckmarkCircleOutline />
          </n-icon>
          <p class="text-lg font-medium text-gray-700 mt-4">此投诉已处理完成</p>
          <p class="text-sm text-gray-500 mt-2">如需重新处理，请联系系统管理员</p>
        </div>
      </div>
    </NModal>
  </div>
</template>

<style scoped>
/* 投诉处理页面样式 */
.complaint-manage-container {
  background: linear-gradient(135deg, #fff5f5 0%, #f3e8ff 100%);
  min-height: 100vh;
}

/* 统计卡片动画 */
.n-card {
  transition: all 0.3s ease;
}

.n-card:hover {
  transform: translateY(-2px);
}

/* 紧急投诉行样式 */
:deep(.urgent-row) {
  background-color: rgba(255, 77, 79, 0.05) !important;
  border-left: 3px solid #ff4d4f;
}

:deep(.urgent-row:hover) {
  background-color: rgba(255, 77, 79, 0.1) !important;
}

/* 处理中投诉行样式 */
:deep(.processing-row) {
  background-color: rgba(250, 173, 20, 0.05) !important;
  border-left: 3px solid #faad14;
}

:deep(.processing-row:hover) {
  background-color: rgba(250, 173, 20, 0.1) !important;
}

/* 已完成投诉行样式 */
:deep(.completed-row:hover) {
  background-color: rgba(82, 196, 26, 0.05) !important;
}

/* 投诉状态指示器 */
.complaint-status {
  position: relative;
}

.complaint-status::before {
  content: '';
  position: absolute;
  top: 50%;
  left: -8px;
  width: 6px;
  height: 6px;
  border-radius: 50%;
  transform: translateY(-50%);
}

.status-urgent::before {
  background-color: #ff4d4f;
  box-shadow: 0 0 0 2px rgba(255, 77, 79, 0.2);
  animation: pulse-red 2s infinite;
}

.status-processing::before {
  background-color: #faad14;
  box-shadow: 0 0 0 2px rgba(250, 173, 20, 0.2);
  animation: pulse-orange 2s infinite;
}

.status-completed::before {
  background-color: #52c41a;
  box-shadow: 0 0 0 2px rgba(82, 196, 26, 0.2);
}

/* 投诉头像样式 */
.complaint-avatar {
  background: linear-gradient(135deg, #ff4d4f 0%, #ff7875 100%);
  border: 2px solid #fff;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

/* 搜索表单优化 */
:deep(.n-form-item-label__text) {
  font-weight: 500;
  color: #4a5568;
}

/* 按钮圆角优化 */
:deep(.n-button) {
  border-radius: 8px;
  font-weight: 500;
}

/* 输入框圆角优化 */
:deep(.n-input) {
  border-radius: 8px;
}

:deep(.n-select) {
  border-radius: 8px;
}

/* 数据表格优化 */
:deep(.n-data-table) {
  border-radius: 12px;
  overflow: hidden;
}

:deep(.n-data-table-th) {
  background-color: #fff5f5;
  font-weight: 600;
  color: #4a5568;
}

/* 状态标签优化 */
:deep(.n-tag) {
  border-radius: 12px;
  font-weight: 500;
}

/* 模态框优化 */
:deep(.n-modal) {
  border-radius: 16px;
  overflow: hidden;
}

/* 投诉详情模态框样式增强 */
:deep(.complaint-modal .n-card) {
  background-color: #ffffff !important;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1) !important;
}

:deep(.complaint-modal .n-card-header) {
  background-color: #ffffff !important;
  border-bottom: 1px solid #e2e8f0 !important;
  padding: 20px 24px !important;
}

:deep(.complaint-modal .n-card__content) {
  background-color: #ffffff !important;
  padding: 24px !important;
}

/* 确保所有文字都有足够的对比度 */
:deep(.complaint-modal .n-text) {
  color: #2d3748 !important;
}

:deep(.complaint-modal .n-card .n-card-header__main) {
  color: #1a202c !important;
  font-weight: 600 !important;
}

:deep(.complaint-modal .n-form-item-label__text) {
  color: #4a5568 !important;
  font-weight: 500 !important;
}

/* 加载动画优化 */
:deep(.n-spin) {
  color: #ff4d4f;
}

/* 紧急处理样式 */
.urgent-processing {
  background: linear-gradient(135deg, #ff4d4f, #ff7875);
  color: white;
  border: none;
}

.urgent-processing:hover {
  background: linear-gradient(135deg, #f5222d, #ff4d4f);
  transform: translateY(-1px);
  box-shadow: 0 4px 8px rgba(255, 77, 79, 0.3);
}

/* 投诉内容展示优化 */
.complaint-content {
  background: linear-gradient(135deg, #fff5f5, #fef2f2);
  border: 1px solid #fed7d7;
  border-radius: 8px;
  padding: 12px;
  line-height: 1.6;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .p-6 {
    padding: 1rem;
  }
  
  .n-grid[cols="6"] {
    grid-template-columns: repeat(2, 1fr);
  }
  
  .n-modal {
    width: 95vw !important;
    max-width: none !important;
  }
}

/* 动画效果 */
@keyframes pulse-red {
  0% {
    box-shadow: 0 0 0 0 rgba(255, 77, 79, 0.4);
  }
  70% {
    box-shadow: 0 0 0 4px rgba(255, 77, 79, 0);
  }
  100% {
    box-shadow: 0 0 0 0 rgba(255, 77, 79, 0);
  }
}

@keyframes pulse-orange {
  0% {
    box-shadow: 0 0 0 0 rgba(250, 173, 20, 0.4);
  }
  70% {
    box-shadow: 0 0 0 4px rgba(250, 173, 20, 0);
  }
  100% {
    box-shadow: 0 0 0 0 rgba(250, 173, 20, 0);
  }
}

/* 页面进入动画 */
@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.fade-in-up {
  animation: fadeInUp 0.6s ease-out;
}

/* 紧急提醒效果 */
.urgent-alert {
  position: relative;
  overflow: hidden;
}

.urgent-alert::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 77, 79, 0.3), transparent);
  animation: slide 2s infinite;
}

@keyframes slide {
  0% {
    left: -100%;
  }
  100% {
    left: 100%;
  }
}

/* 处理表单特效 */
.processing-form {
  background: linear-gradient(135deg, #fff7e6, #fff2e8);
  border-radius: 12px;
  padding: 20px;
  border: 1px solid #ffd591;
}

/* 投诉类型图标动画 */
.complaint-type-icon {
  transition: transform 0.3s ease;
}

.complaint-type-icon:hover {
  transform: scale(1.2) rotate(5deg);
}
</style> 