<script setup lang="tsx">
import { ref, onMounted, computed } from 'vue';
import { 
  NButton, 
  NPopconfirm, 
  NTag, 
  NInput, 
  NSelect, 
  NSpace, 
  NCard, 
  NModal, 
  NForm, 
  NFormItem, 
  NText, 
  NSpin, 
  NStatistic,
  NIcon,
  NAvatar,
  NBadge,
  NDivider,
  NGrid,
  NGi,
  NDataTable,
  useMessage 
} from 'naive-ui';
import type { DataTableColumns } from 'naive-ui';
import { 
  StorefrontOutline,
  SearchOutline,
  RefreshOutline,
  BusinessOutline,
  LocationOutline,
  CallOutline,
  CheckmarkCircleOutline,
  CloseCircleOutline,
  PauseCircleOutline,
  EyeOutline,
  BanOutline,
  DocumentTextOutline,
  TimeOutline,
  StatsChartOutline
} from '@vicons/ionicons5';
import { fetchMerchantList, fetchMerchantInfo, formatMerchantStatus, getMerchantStatusType, banMerchant, unbanMerchant, fetchApplications, auditApplication, formatApplicationStatus, getApplicationStatusType } from '@/api/merchant';

defineOptions({
  name: 'MerchantManage'
});

const message = useMessage();
const loading = ref(false);
const tableData = ref<any[]>([]);

// 商家详情弹窗
const showDetailModal = ref(false);
const detailLoading = ref(false);
const merchantDetail = ref<any>({});

// 申请审核相关
const showApplicationModal = ref(false);
const applicationLoading = ref(false);
const applicationData = ref<any[]>([]);
const currentAdminId = ref('6f7af74d972c481c91f19596e07aae3a'); // 管理员ID

// 审核弹窗
const showAuditModal = ref(false);
const auditLoading = ref(false);
const currentApplication = ref<any>({});
const auditForm = ref({
  decision: '',
  reason: ''
});

// 筛选条件
const searchParams = ref({
  merchantName: '',
  status: null,
  location: ''
});

// 状态选项
const statusOptions = [
  { label: '全部状态', value: null },
  { label: '正常营业', value: 1 },
  { label: '已封禁', value: 0 },
  { label: '暂停营业', value: 2 }
];

// 商家统计数据
const merchantStats = computed(() => {
  const total = tableData.value.length;
  const activeCount = tableData.value.filter(merchant => merchant.status === 1).length;
  const bannedCount = tableData.value.filter(merchant => merchant.status === 0).length;
  const pausedCount = tableData.value.filter(merchant => merchant.status === 2).length;
  const withLocation = tableData.value.filter(merchant => merchant.location).length;
  
  return {
    total,
    activeCount,
    bannedCount,
    pausedCount,
    withLocation,
    activeRate: total > 0 ? Math.round((activeCount / total) * 100) : 0,
    locationRate: total > 0 ? Math.round((withLocation / total) * 100) : 0
  };
});

// 过滤后的表格数据
const filteredTableData = computed(() => {
  return tableData.value.filter(merchant => {
    if (searchParams.value.merchantName && !merchant.merchantName?.toLowerCase().includes(searchParams.value.merchantName.toLowerCase())) {
      return false;
    }
    if (searchParams.value.status !== null && merchant.status !== searchParams.value.status) {
      return false;
    }
    if (searchParams.value.location && !merchant.location?.toLowerCase().includes(searchParams.value.location.toLowerCase())) {
      return false;
    }
    return true;
  });
});

// 获取商家列表数据
async function getMerchantList() {
  try {
    loading.value = true;
    // 传递筛选参数 - 只传递非空值，移除undefined参数
    const params = {};
    if (searchParams.value.merchantName) {
      params.merchantName = searchParams.value.merchantName;
    }
    if (searchParams.value.location) {
      params.location = searchParams.value.location;
    }
    // 注意：后端不支持status参数，暂时移除
    
    console.log('🚀 开始请求商家列表，参数:', params);
    const response = await fetchMerchantList(params);
    console.log('📥 收到商家列表响应:', response);
    
    // 根据Apifox接口实际返回格式处理数据
    // 接口返回格式: { code: 0, message: "商家信息获取成功", data: [...], timestamp: 1754444437577 }
    if (response && response.data) {
      tableData.value = response.data.map((merchant, index) => ({
        ...merchant,
        index: index + 1,
        statusText: formatMerchantStatus(merchant.status),
        hasLocation: merchant.location ? '是' : '否'
      }));
    } else {
      message.error(response.message || '获取商家列表失败');
      tableData.value = []; // 确保在出错时清空表格
    }
  } catch (error) {
    message.error('获取商家列表失败: ' + error.message);
    console.error('Error fetching merchant list:', error);
  } finally {
    loading.value = false;
  }
}

// 搜索功能
function handleSearch() {
  message.success(`找到 ${filteredTableData.value.length} 个商家`);
}

// 重置搜索条件
function handleReset() {
  searchParams.value = {
    merchantName: '',
    status: null,
    location: ''
  };
  message.info('搜索条件已重置');
}

// 刷新数据
function handleRefresh() {
  getMerchantList();
}

// 申请列表表格配置
const applicationColumns: DataTableColumns = [
  {
    key: 'id',
    title: '申请ID',
    align: 'center',
    width: 150,
    ellipsis: {
      tooltip: true
    }
  },

  {
    key: 'businessName',
    title: '商家名称',
    align: 'center',
    width: 150,
    render: (row) => row.businessName || row.name || '-'
  },

  {
    key: 'status',
    title: '状态',
    align: 'center',
    width: 100,
    render: (row) => {
      const statusType = getApplicationStatusType(row.status);
      const statusText = formatApplicationStatus(row.status);
      return <n-tag type={statusType}>{statusText}</n-tag>;
    }
  },

  {
    key: 'actions',
    title: '操作',
    align: 'center',
    width: 120,
    render: (row) => {
      console.log('🔍 申请状态检查:', { 
        id: row.id, 
        status: row.status, 
        type: typeof row.status,
        formatStatus: formatApplicationStatus(row.status)
      });
      
      // 根据格式化后的状态文本判断是否可审核
      const statusText = formatApplicationStatus(row.status);
      const isPending = statusText === '待审核';
      
      if (isPending) {
        return (
          <n-button type="primary" size="small" onClick={() => handleAuditApplication(row)}>
            审核
          </n-button>
        );
      } else {
        return (
          <n-button type="default" size="small" disabled>
            已审核
          </n-button>
        );
      }
    }
  }
];

const columns: DataTableColumns = [
  {
    key: 'index',
    title: '序号',
    align: 'center',
    width: 70,
    render: (row, index) => (
      <n-badge value={index + 1} type="info" />
    )
  },
  {
    key: 'merchantInfo',
    title: '商家信息',
    width: 220,
    render: (row) => (
      <div class="flex items-center gap-12px">
        <n-avatar
          size="medium"
          style={{
            backgroundColor: row.status === 1 ? '#52c41a' : row.status === 0 ? '#f5222d' : '#faad14'
          }}
        >
          <n-icon>
            <StorefrontOutline />
          </n-icon>
        </n-avatar>
        <div class="flex flex-col">
          <n-text strong>{row.merchantName || '未设置店名'}</n-text>
          <n-text depth="3" style="font-size: 12px;">
            ID: {row.merchantId?.slice(-8) || '-'}
          </n-text>
        </div>
      </div>
    )
  },
  {
    key: 'contactInfo',
    title: '联系方式',
    align: 'center',
    width: 140,
    render: (row) => (
      <div class="flex items-center justify-center gap-4px">
        <n-icon color="#409eff" size="14">
          <CallOutline />
        </n-icon>
        <n-text style="font-size: 12px;">{row.contactInfo || '未提供'}</n-text>
      </div>
    )
  },
  {
    key: 'location',
    title: '地址信息',
    align: 'center',
    width: 160,
    render: (row) => (
      <div class="flex items-center justify-center gap-4px">
        <n-icon color={row.location ? '#67c23a' : '#dcdfe6'} size="14">
          <LocationOutline />
        </n-icon>
        <div class="flex flex-col">
          <n-text style="font-size: 12px;">{row.location || '未设置地址'}</n-text>
          <n-tag size="small" type={row.location ? 'success' : 'warning'}>
            {row.hasLocation}
          </n-tag>
        </div>
      </div>
    )
  },
  {
    key: 'status',
    title: '营业状态',
    align: 'center',
    width: 120,
    render: (row) => {
      const statusConfig = {
        1: { type: 'success', icon: CheckmarkCircleOutline, color: '#52c41a' },
        0: { type: 'error', icon: CloseCircleOutline, color: '#f5222d' },
        2: { type: 'warning', icon: PauseCircleOutline, color: '#faad14' }
      };
      const config = statusConfig[row.status] || statusConfig[2];
      return (
        <div class="flex items-center justify-center gap-4px">
          <n-icon color={config.color} size="16">
            <config.icon />
          </n-icon>
          <n-tag type={config.type} size="small">
            {row.statusText}
          </n-tag>
        </div>
      );
    }
  },
  {
    key: 'actions',
    title: '操作',
    align: 'center',
    width: 180,
    render: (row) => (
      <div class="flex items-center justify-center gap-6px">
        <n-button 
          type="primary" 
          size="small" 
          onClick={() => handleViewDetail(row.merchantId)}
          style="border-radius: 6px;"
        >
          <div style="display: flex; align-items: center; gap: 4px;">
            <n-icon size="14">
              <EyeOutline />
            </n-icon>
            详情
          </div>
        </n-button>
        
        {row.status === 1 ? (
          <NPopconfirm onPositiveClick={() => handleBanMerchant(row.merchantId)}>
            {{
              default: () => '确定要封禁该商家吗？封禁后商家将无法正常营业。',
              trigger: () => (
                <n-button type="error" size="small" style="border-radius: 6px;">
                  <div style="display: flex; align-items: center; gap: 4px;">
                    <n-icon size="14">
                      <BanOutline />
                    </n-icon>
                    封禁
                  </div>
                </n-button>
              ),
            }}
          </NPopconfirm>
        ) : row.status === 0 ? (
          <NPopconfirm onPositiveClick={() => handleUnbanMerchant(row.merchantId)}>
            {{
              default: () => '确定要解封该商家吗？解封后商家可恢复正常营业。',
              trigger: () => (
                <n-button type="success" size="small" style="border-radius: 6px;">
                  <div style="display: flex; align-items: center; gap: 4px;">
                    <n-icon size="14">
                      <CheckmarkCircleOutline />
                    </n-icon>
                    解封
                  </div>
                </n-button>
              ),
            }}
          </NPopconfirm>
        ) : (
          <n-button type="warning" size="small" disabled style="border-radius: 6px;">
            <div style="display: flex; align-items: center; gap: 4px;">
              <n-icon size="14">
                <PauseCircleOutline />
              </n-icon>
              暂停
            </div>
          </n-button>
        )}
      </div>
    )
  }
];

// 封禁商家
async function handleBanMerchant(merchantId: string) {
  try {
    loading.value = true;
    await banMerchant(merchantId, '管理员手动封禁');
    message.success('商家封禁成功');
    // 刷新列表
    await getMerchantList();
  } catch (error) {
    console.error('封禁商家失败:', error);
    message.error('封禁商家失败: ' + (error.message || '未知错误'));
  } finally {
    loading.value = false;
  }
}

// 解封商家
async function handleUnbanMerchant(merchantId: string) {
  try {
    loading.value = true;
    await unbanMerchant(merchantId);
    message.success('商家解封成功');
    // 刷新列表
    await getMerchantList();
  } catch (error) {
    console.error('解封商家失败:', error);
    message.error('解封商家失败: ' + (error.message || '未知错误'));
  } finally {
    loading.value = false;
  }
}

// 获取申请列表
async function getApplicationList() {
  try {
    applicationLoading.value = true;
    
    console.log(`获取申请列表，管理员ID: ${currentAdminId.value}`);
    const response = await fetchApplications(currentAdminId.value);
    
    if (response && response.data) {
      const rawList = Array.isArray(response.data) ? response.data : [response.data];
      // 统一后端字段到表格使用的字段
      applicationData.value = rawList.map((item: any) => ({
        ...item,
        id: item.id || item.applyId || item.applicationId || item.ApplyId,
        applicantName: item.applicantName || item.contactName || item.applicant || item.userName || item.name,
        businessName: item.businessName || item.companyName || item.merchantName || item.shopName || item.name,
        contactInfo: item.contactInfo || item.phone || item.mobile || item.contactPhone || item.contact || item.tel,
        status: item.status ?? item.applyStatus ?? item.auditStatus ?? item.state ?? 'pending',
        _rawStatus: item.status,  // 保留原始状态用于调试
        createdAt: item.createdAt || item.createdTime || item.applyTime || item.createTime
      }));
      console.log('申请列表:', applicationData.value);
    } else {
      message.error('获取申请列表失败：返回数据为空');
      applicationData.value = [];
    }
  } catch (error) {
    console.error('获取申请列表失败:', error);
    message.error('获取申请列表失败: ' + (error.message || '未知错误'));
    applicationData.value = [];
  } finally {
    applicationLoading.value = false;
  }
}

// 显示申请审核弹窗
function handleShowApplications() {
  showApplicationModal.value = true;
  getApplicationList();
}

// 查看商家详情
async function handleViewDetail(merchantId: string) {
  try {
    detailLoading.value = true;
    showDetailModal.value = true;
    
    console.log(`获取商家详情，ID: ${merchantId}`);
    const response = await fetchMerchantInfo(merchantId);
    
    if (response && response.data) {
      merchantDetail.value = response.data;
      console.log('商家详情:', merchantDetail.value);
    } else {
      message.error('获取商家详情失败：返回数据为空');
    }
  } catch (error) {
    console.error('获取商家详情失败:', error);
    message.error('获取商家详情失败: ' + (error.message || '未知错误'));
    showDetailModal.value = false;
  } finally {
    detailLoading.value = false;
  }
}

// 处理审核申请
function handleAuditApplication(application: any) {
  currentApplication.value = application;
  auditForm.value = {
    decision: '',
    reason: ''
  };
  showAuditModal.value = true;
}

// 提交审核结果
async function submitAudit() {
  if (!auditForm.value.decision) {
    message.warning('请选择审核结果');
    return;
  }
  
  try {
    auditLoading.value = true;
    
    const auditData = {
      decision: auditForm.value.decision,
      reason: auditForm.value.reason || ''
    };
    
    await auditApplication(currentAdminId.value, currentApplication.value.id, auditData);
    
    message.success(`申请${auditForm.value.decision === 'approve' ? '同意' : '拒绝'}成功`);
    showAuditModal.value = false;
    
    // 刷新申请列表
    await getApplicationList();
  } catch (error) {
    console.error('审核失败:', error);
    message.error('审核失败: ' + (error.message || '未知错误'));
  } finally {
    auditLoading.value = false;
  }
}

// 组件挂载时获取数据
onMounted(() => {
  getMerchantList();
});
</script>

<template>
  <div class="p-6 min-h-full bg-gray-50">
    <!-- 页面标题 -->
    <div class="mb-6">
      <h1 class="text-2xl font-bold text-gray-800 mb-2 flex items-center gap-2">
        <n-icon size="24" color="#52c41a">
          <StorefrontOutline />
        </n-icon>
        商家管理中心
      </h1>
      <p class="text-gray-600">管理商家入驻、营业状态和基本信息</p>
    </div>

    <!-- 统计卡片区域 -->
    <n-grid :cols="5" :x-gap="16" :y-gap="16" class="mb-6">
      <n-gi>
        <n-card :bordered="false" class="shadow-sm hover:shadow-lg transition-shadow duration-300">
          <n-statistic
            label="总商家数"
            :value="merchantStats.total"
            value-style="color: #52c41a; font-weight: bold;"
          >
            <template #prefix>
              <n-icon size="20" color="#52c41a">
                <BusinessOutline />
              </n-icon>
            </template>
          </n-statistic>
        </n-card>
      </n-gi>
      <n-gi>
        <n-card :bordered="false" class="shadow-sm hover:shadow-lg transition-shadow duration-300">
          <n-statistic
            label="正常营业"
            :value="merchantStats.activeCount"
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
            label="已封禁"
            :value="merchantStats.bannedCount"
            value-style="color: #f5222d; font-weight: bold;"
          >
            <template #prefix>
              <n-icon size="20" color="#f5222d">
                <CloseCircleOutline />
              </n-icon>
            </template>
          </n-statistic>
        </n-card>
      </n-gi>
      <n-gi>
        <n-card :bordered="false" class="shadow-sm hover:shadow-lg transition-shadow duration-300">
          <n-statistic
            label="暂停营业"
            :value="merchantStats.pausedCount"
            value-style="color: #faad14; font-weight: bold;"
          >
            <template #prefix>
              <n-icon size="20" color="#faad14">
                <PauseCircleOutline />
              </n-icon>
            </template>
          </n-statistic>
        </n-card>
      </n-gi>
      <n-gi>
        <n-card :bordered="false" class="shadow-sm hover:shadow-lg transition-shadow duration-300">
          <n-statistic
            label="地址完整率"
            :value="merchantStats.locationRate"
            value-style="color: #722ed1; font-weight: bold;"
          >
            <template #suffix>
              <span class="text-sm text-gray-500">%</span>
            </template>
            <template #prefix>
              <n-icon size="20" color="#722ed1">
                <LocationOutline />
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
      
      <n-form :model="searchParams" inline label-placement="left" label-width="80">
        <n-form-item label="商家名称">
          <n-input
            v-model:value="searchParams.merchantName"
            placeholder="请输入商家名称"
            clearable
            style="width: 200px"
          />
        </n-form-item>
        
        <n-form-item label="营业状态">
          <n-select
            v-model:value="searchParams.status"
            :options="statusOptions"
            placeholder="选择状态"
            clearable
            style="width: 150px"
          />
        </n-form-item>
        
        <n-form-item label="所在地区">
          <n-input
            v-model:value="searchParams.location"
            placeholder="请输入地区"
            clearable
            style="width: 200px"
          />
        </n-form-item>
      </n-form>
    </n-card>

    <!-- 商家列表 -->
    <n-card :bordered="false" class="shadow-sm">
      <template #header>
        <div class="flex items-center gap-2">
          <n-icon size="18" color="#52c41a">
            <StorefrontOutline />
          </n-icon>
          <span class="font-medium">商家列表</span>
        </div>
      </template>
      
      <template #header-extra>
        <n-space align="center">
          <n-text depth="3">
            显示 {{ filteredTableData.length }} / {{ tableData.length }} 条记录
          </n-text>
          <n-button type="primary" @click="handleShowApplications">
            <template #icon>
              <n-icon>
                <DocumentTextOutline />
              </n-icon>
            </template>
            审核入驻申请
          </n-button>
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
        :row-class-name="() => 'hover:bg-green-50 transition-colors duration-200'"
      />
    </n-card>
    
    <!-- 商家详情弹窗 -->
    <n-modal 
      v-model:show="showDetailModal" 
      preset="card" 
      style="width: 800px; max-height: 80vh;" 
      class="rounded-2xl"
      :mask-closable="false"
    >
      <template #header>
        <div class="flex items-center gap-3">
          <n-avatar 
            :size="40"
            :style="{
              backgroundColor: merchantDetail.status === 1 ? '#52c41a' : merchantDetail.status === 0 ? '#f5222d' : '#faad14'
            }"
          >
            <n-icon size="20">
              <StorefrontOutline />
            </n-icon>
          </n-avatar>
          <div>
            <h3 class="text-lg font-semibold text-gray-800">
              {{ merchantDetail.merchantName || '商家详情' }}
            </h3>
            <p class="text-sm text-gray-500">
              ID: {{ merchantDetail.merchantId || '-' }}
            </p>
          </div>
        </div>
      </template>

      <div v-if="!detailLoading" class="space-y-6">
        <!-- 商家概览 -->
        <div class="bg-gradient-to-r from-green-50 to-emerald-50 p-4 rounded-lg">
          <n-grid :cols="3" :x-gap="20">
            <n-gi>
              <div class="text-center">
                <div class="text-2xl font-bold text-green-600">
                  {{ formatMerchantStatus(merchantDetail.status) }}
                </div>
                <div class="text-sm text-gray-600">营业状态</div>
              </div>
            </n-gi>
            <n-gi>
              <div class="text-center">
                <div class="text-2xl font-bold text-blue-600">
                  {{ merchantDetail.location ? '已设置' : '未设置' }}
                </div>
                <div class="text-sm text-gray-600">地址信息</div>
              </div>
            </n-gi>
            <n-gi>
              <div class="text-center">
                <div class="text-2xl font-bold text-purple-600">
                  {{ merchantDetail.contactInfo ? '已设置' : '未设置' }}
                </div>
                <div class="text-sm text-gray-600">联系方式</div>
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
                <n-icon color="#52c41a">
                  <BusinessOutline />
                </n-icon>
              </template>
              
              <div class="space-y-4">
                <div class="flex justify-between items-center py-2 border-b border-gray-100">
                  <span class="text-gray-600 font-medium">商家ID</span>
                  <n-text code style="word-break: break-all; max-width: 200px;">{{ merchantDetail.merchantId || '-' }}</n-text>
                </div>
                
                <div class="flex justify-between items-center py-2 border-b border-gray-100">
                  <span class="text-gray-600 font-medium">店铺名称</span>
                  <n-text strong>{{ merchantDetail.merchantName || '未设置' }}</n-text>
                </div>
                
                <div class="flex justify-between items-center py-2 border-b border-gray-100">
                  <span class="text-gray-600 font-medium">营业状态</span>
                  <n-tag 
                    :type="getMerchantStatusType(merchantDetail.status)"
                    size="small"
                  >
                    <template #icon>
                      <n-icon>
                        <CheckmarkCircleOutline v-if="merchantDetail.status === 1" />
                        <CloseCircleOutline v-else-if="merchantDetail.status === 0" />
                        <PauseCircleOutline v-else />
                      </n-icon>
                    </template>
                    {{ formatMerchantStatus(merchantDetail.status) }}
                  </n-tag>
                </div>
                
                <div class="flex justify-between items-center py-2">
                  <span class="text-gray-600 font-medium">备注描述</span>
                  <n-text>{{ merchantDetail.description || '暂无描述' }}</n-text>
                </div>
              </div>
            </n-card>
          </n-gi>
          
          <!-- 联系信息 -->
          <n-gi>
            <n-card title="联系信息" size="small" class="h-full">
              <template #header-extra>
                <n-icon color="#409eff">
                  <CallOutline />
                </n-icon>
              </template>
              
              <div class="space-y-4">
                <div class="flex justify-between items-center py-2 border-b border-gray-100">
                  <span class="text-gray-600 font-medium flex items-center gap-2">
                    <n-icon size="16" color="#409eff">
                      <CallOutline />
                    </n-icon>
                    联系方式
                  </span>
                  <n-text>{{ merchantDetail.contactInfo || '未设置' }}</n-text>
                </div>
                
                <div class="flex justify-between items-start py-2 border-b border-gray-100">
                  <span class="text-gray-600 font-medium flex items-center gap-2">
                    <n-icon size="16" color="#67c23a">
                      <LocationOutline />
                    </n-icon>
                    商家地址
                  </span>
                  <n-text style="max-width: 200px; word-break: break-word;">
                    {{ merchantDetail.location || '暂无地址信息' }}
                  </n-text>
                </div>
                
                <div class="flex justify-between items-center py-2">
                  <span class="text-gray-600 font-medium">地址状态</span>
                  <n-tag 
                    :type="merchantDetail.location ? 'success' : 'warning'" 
                    size="small"
                  >
                    {{ merchantDetail.location ? '已完善' : '待完善' }}
                  </n-tag>
                </div>
              </div>
            </n-card>
          </n-gi>
        </n-grid>
      </div>
      
      <!-- 加载状态 -->
      <div v-else class="flex justify-center items-center h-80">
        <n-spin size="large">
          <template #description>
            <div class="text-center mt-4">
              <p class="text-gray-600">正在加载商家详情...</p>
              <p class="text-sm text-gray-400 mt-1">请稍候片刻</p>
            </div>
          </template>
        </n-spin>
      </div>
    </n-modal>
    
    <!-- 申请审核弹窗 -->
    <n-modal 
      v-model:show="showApplicationModal" 
      preset="card" 
      style="width: 1000px; max-height: 80vh;" 
      class="rounded-2xl"
      :mask-closable="false"
    >
      <template #header>
        <div class="flex items-center gap-3">
          <n-icon size="24" color="#722ed1">
            <DocumentTextOutline />
          </n-icon>
          <div>
            <h3 class="text-lg font-semibold text-gray-800">商家入驻申请审核</h3>
            <p class="text-sm text-gray-500">管理和审核待处理的商家入驻申请</p>
          </div>
        </div>
      </template>

      <div v-if="!applicationLoading" class="space-y-4">
        <div class="bg-purple-50 p-3 rounded-lg mb-4">
          <div class="flex items-center justify-between">
            <div class="flex items-center gap-2">
              <n-icon color="#722ed1">
                <StatsChartOutline />
              </n-icon>
              <span class="text-gray-700 font-medium">申请统计</span>
            </div>
            <n-badge :value="applicationData.length" type="info" />
          </div>
        </div>
        
        <n-data-table
          :columns="applicationColumns"
          :data="applicationData"
          :pagination="{ pageSize: 8, showSizePicker: true }"
          style="height: 400px;"
          :row-class-name="() => 'hover:bg-purple-50 transition-colors duration-200'"
        />
      </div>
      
      <div v-else class="flex justify-center items-center h-60">
        <n-spin size="large">
          <template #description>
            <div class="text-center mt-4">
              <p class="text-gray-600">正在加载申请列表...</p>
            </div>
          </template>
        </n-spin>
      </div>
    </n-modal>
    
    <!-- 审核弹窗 -->
    <n-modal 
      v-model:show="showAuditModal" 
      preset="card" 
      style="width: 600px" 
      class="rounded-2xl"
      :mask-closable="false"
    >
      <template #header>
        <div class="flex items-center gap-3">
          <n-icon size="24" color="#fa8c16">
            <TimeOutline />
          </n-icon>
          <div>
            <h3 class="text-lg font-semibold text-gray-800">审核申请</h3>
            <p class="text-sm text-gray-500">请仔细审核申请信息</p>
          </div>
        </div>
      </template>

      <div v-if="!auditLoading" class="space-y-6">
        <!-- 申请信息展示 -->
        <div class="bg-orange-50 p-4 rounded-lg">
          <h4 class="text-gray-800 font-medium mb-3 flex items-center gap-2">
            <n-icon color="#fa8c16">
              <DocumentTextOutline />
            </n-icon>
            申请详情
          </h4>
          <div class="grid grid-cols-2 gap-4">
            <div>
              <span class="text-gray-600 text-sm">申请ID</span>
              <p class="text-gray-800 font-medium">{{ currentApplication.id || '-' }}</p>
            </div>
            <div>
              <span class="text-gray-600 text-sm">申请类型</span>
              <p class="text-gray-800 font-medium">{{ currentApplication.type || '商家入驻' }}</p>
            </div>

          </div>
        </div>

        <n-divider />

        <!-- 审核表单 -->
        <n-form :model="auditForm" label-placement="top">
          <n-form-item label="审核结果" required>
            <n-select 
              v-model:value="auditForm.decision" 
              :options="[
                { label: '✅ 同意申请', value: 'approve' },
                { label: '❌ 拒绝申请', value: 'reject' }
              ]"
              placeholder="请选择审核结果"
              size="large"
            />
          </n-form-item>
          
          <n-form-item label="审核备注">
            <n-input 
              v-model:value="auditForm.reason" 
              type="textarea" 
              placeholder="请输入审核理由或备注（可选）"
              :rows="4"
              show-count
              maxlength="200"
            />
          </n-form-item>
        </n-form>
      </div>
      
      <div v-else class="flex justify-center items-center h-60">
        <n-spin size="large">
          <template #description>
            <div class="text-center mt-4">
              <p class="text-gray-600">正在处理审核...</p>
            </div>
          </template>
        </n-spin>
      </div>
      
      <template #footer>
        <div class="flex justify-end gap-3">
          <n-button @click="showAuditModal = false" size="large">
            取消
          </n-button>
          <n-button 
            type="primary" 
            @click="submitAudit" 
            :loading="auditLoading"
            size="large"
          >
            <template #icon>
              <n-icon>
                <CheckmarkCircleOutline />
              </n-icon>
            </template>
            提交审核
          </n-button>
        </div>
      </template>
    </n-modal>
  </div>
</template>

<style scoped>
/* 商家管理页面样式 */
.merchant-manage-container {
  background: linear-gradient(135deg, #f6ffed 0%, #f0f9ff 100%);
  min-height: 100vh;
}

/* 统计卡片动画 */
.n-card {
  transition: all 0.3s ease;
}

.n-card:hover {
  transform: translateY(-2px);
}

/* 表格行悬停效果 */
:deep(.n-data-table-tr:hover) {
  background-color: rgba(82, 196, 26, 0.05) !important;
}

/* 商家状态指示器 */
.merchant-status {
  position: relative;
}

.merchant-status::before {
  content: '';
  position: absolute;
  top: 50%;
  left: -8px;
  width: 6px;
  height: 6px;
  border-radius: 50%;
  transform: translateY(-50%);
}

.status-active::before {
  background-color: #52c41a;
  box-shadow: 0 0 0 2px rgba(82, 196, 26, 0.2);
}

.status-banned::before {
  background-color: #f5222d;
  box-shadow: 0 0 0 2px rgba(245, 34, 45, 0.2);
}

.status-paused::before {
  background-color: #faad14;
  box-shadow: 0 0 0 2px rgba(250, 173, 20, 0.2);
}

/* 商家头像样式 */
.merchant-avatar {
  background: linear-gradient(135deg, #52c41a 0%, #389e0d 100%);
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
  background-color: #f6ffed;
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

/* 加载动画优化 */
:deep(.n-spin) {
  color: #52c41a;
}

/* 审核弹窗特殊样式 */
.audit-modal {
  background: linear-gradient(135deg, #fff7e6 0%, #fff2e8 100%);
}

/* 申请列表特殊样式 */
.application-modal {
  background: linear-gradient(135deg, #f9f0ff 0%, #efdbff 100%);
}

/* 信息卡片样式 */
.info-card {
  transition: all 0.3s ease;
  border-radius: 12px;
  overflow: hidden;
}

.info-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.1);
}

/* 渐变背景 */
.gradient-green {
  background: linear-gradient(135deg, #52c41a 0%, #389e0d 100%);
}

.gradient-orange {
  background: linear-gradient(135deg, #fa8c16 0%, #d46b08 100%);
}

.gradient-purple {
  background: linear-gradient(135deg, #722ed1 0%, #531dab 100%);
}

/* 响应式设计 */
@media (max-width: 768px) {
  .p-6 {
    padding: 1rem;
  }
  
  .n-grid[cols="5"] {
    grid-div-columns: repeat(2, 1fr);
  }
  
  .n-modal {
    width: 95vw !important;
    max-width: none !important;
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

/* 操作按钮组样式 */
.action-buttons {
  display: flex;
  gap: 4px;
  align-items: center;
}

.action-buttons .n-button {
  min-width: 60px;
  height: 28px;
}

/* 状态指示灯动画 */
@keyframes pulse {
  0% {
    box-shadow: 0 0 0 0 rgba(82, 196, 26, 0.4);
  }
  70% {
    box-shadow: 0 0 0 4px rgba(82, 196, 26, 0);
  }
  100% {
    box-shadow: 0 0 0 0 rgba(82, 196, 26, 0);
  }
}

.status-active::before {
  animation: pulse 2s infinite;
}
</style> 