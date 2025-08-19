<script setup lang="tsx">
import { ref, onMounted } from 'vue';
import { NButton, NPopconfirm, NTag, NInput, NSelect, NSpace, NCard, NModal, NForm, NFormItem, NText, NSpin, useMessage } from 'naive-ui';
import type { DataTableColumns } from 'naive-ui';
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
  { label: '全部', value: null },
  { label: '正常', value: 1 },
  { label: '已封禁', value: 0 },
  { label: '暂停', value: 2 }
];

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
async function handleSearch() {
  await getMerchantList();
}

// 重置搜索条件
function handleReset() {
  searchParams.value = {
    merchantName: '',
    status: null,
    location: ''
  };
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
    key: 'applicantName',
    title: '申请人',
    align: 'center',
    width: 120,
    render: (row) => row.applicantName || row.merchantName || '-'
  },
  {
    key: 'businessName',
    title: '商家名称',
    align: 'center',
    width: 150,
    render: (row) => row.businessName || row.name || '-'
  },
  {
    key: 'contactInfo',
    title: '联系方式',
    align: 'center',
    width: 120,
    render: (row) => row.contactInfo || row.phone || '-'
  },
  {
    key: 'status',
    title: '状态',
    align: 'center',
    width: 100,
    render: (row) => {
      const statusType = getApplicationStatusType(row.status);
      const statusText = formatApplicationStatus(row.status);
      return <NTag type={statusType}>{statusText}</NTag>;
    }
  },
  {
    key: 'createdAt',
    title: '申请时间',
    align: 'center',
    width: 150,
    render: (row) => {
      if (!row.createdAt) return '-';
      try {
        return new Date(row.createdAt).toLocaleString('zh-CN');
      } catch {
        return row.createdAt;
      }
    }
  },
  {
    key: 'actions',
    title: '操作',
    align: 'center',
    width: 120,
    render: (row) => {
      if (row.status === 0 || row.status === 'pending') {
        return (
          <NButton type="primary" size="small" onClick={() => handleAuditApplication(row)}>
            审核
          </NButton>
        );
      } else {
        return (
          <NButton type="default" size="small" disabled>
            已审核
          </NButton>
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
    width: 80
  },
  {
    key: 'merchantId',
    title: '商家ID',
    align: 'center',
    width: 150,
    ellipsis: {
      tooltip: true
    }
  },
  {
    key: 'merchantName',
    title: '店铺名称',
    align: 'center',
    width: 150
  },
  {
    key: 'contactInfo',
    title: '联系方式',
    align: 'center',
    width: 150
  },
  {
    key: 'hasLocation',
    title: '地址信息',
    align: 'center',
    width: 100,
    render: (row) => {
      const tagType = row.hasLocation === '是' ? 'success' : 'warning';
      return <NTag type={tagType}>{row.hasLocation}</NTag>;
    }
  },
  {
    key: 'statusText',
    title: '营业状态',
    align: 'center',
    width: 100,
    render: (row) => {
      const tagType = getMerchantStatusType(row.status);
      return <NTag type={tagType}>{row.statusText}</NTag>;
    }
  },
  {
    key: 'actions',
    title: '操作',
    align: 'center',
    render: (row) => (
      <div class="flex-center gap-10px">
        <NButton type="primary" size="small" onClick={() => handleViewDetail(row.merchantId)}>查看详情</NButton>
        {row.status === 1 ? (
          <NPopconfirm onPositiveClick={() => handleBanMerchant(row.merchantId)}>
            {{
              default: () => '确定要封禁该商家吗？封禁后商家将无法正常营业。',
              trigger: () => <NButton type="error" size="small">封禁</NButton>,
            }}
          </NPopconfirm>
        ) : row.status === 0 ? (
          <NPopconfirm onPositiveClick={() => handleUnbanMerchant(row.merchantId)}>
            {{
              default: () => '确定要解封该商家吗？解封后商家可恢复正常营业。',
              trigger: () => <NButton type="success" size="small">解封</NButton>,
            }}
          </NPopconfirm>
        ) : (
          <NButton type="warning" size="small" disabled>暂停中</NButton>
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
      applicationData.value = Array.isArray(response.data) ? response.data : [response.data];
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
  <div class="m-16px">
    <!-- 筛选条件 -->
    <n-card title="筛选条件" class="mb-16px">
      <n-space :size="12" align="center">
        <n-input
          v-model:value="searchParams.merchantName"
          placeholder="请输入商家名称"
          clearable
          style="width: 200px"
        />
        <n-select
          v-model:value="searchParams.status"
          :options="statusOptions"
          placeholder="营业状态"
          style="width: 150px"
          clearable
        />
        <n-input
          v-model:value="searchParams.location"
          placeholder="请输入地区"
          clearable
          style="width: 200px"
        />
        <n-button type="primary" @click="handleSearch">
          搜索
        </n-button>
        <n-button @click="handleReset">
          重置
        </n-button>
      </n-space>
    </n-card>

    <!-- 商家列表 -->
    <n-card title="商家管理" :bordered="false" class="h-full">
      <template #header-extra>
        <n-button type="primary" @click="handleShowApplications">
          审核入驻申请
        </n-button>
      </template>
      <n-data-table
        :columns="columns"
        :data="tableData"
        :loading="loading"
        :pagination="{ pageSize: 10 }"
        flex-height
        class="h-full"
      />
    </n-card>
    
    <!-- 商家详情弹窗 -->
    <n-modal 
      v-model:show="showDetailModal" 
      preset="card" 
      style="width: 600px" 
      title="商家详情"
      class="rounded-xl"
    >
      <n-form v-if="!detailLoading" label-placement="left" label-width="120">
        <n-form-item label="商家ID">
          <n-text>{{ merchantDetail.merchantId || '-' }}</n-text>
        </n-form-item>
        <n-form-item label="店铺名称">
          <n-text>{{ merchantDetail.merchantName || '-' }}</n-text>
        </n-form-item>
        <n-form-item label="营业状态">
          <n-tag :type="getMerchantStatusType(merchantDetail.status)">
            {{ formatMerchantStatus(merchantDetail.status) }}
          </n-tag>
        </n-form-item>
        <n-form-item label="联系方式">
          <n-text>{{ merchantDetail.contactInfo || '-' }}</n-text>
        </n-form-item>
        <n-form-item label="商家地址">
          <n-text>{{ merchantDetail.location || '暂无地址信息' }}</n-text>
        </n-form-item>
        <n-form-item label="备注描述">
          <n-text>{{ merchantDetail.description || '暂无描述' }}</n-text>
        </n-form-item>
      </n-form>
      
      <div v-else class="flex-center p-40px">
        <n-spin size="large" />
      </div>
      
      <template #footer>
        <div class="flex-end">
          <n-button @click="showDetailModal = false">关闭</n-button>
        </div>
      </template>
    </n-modal>
    
    <!-- 申请审核弹窗 -->
    <n-modal 
      v-model:show="showApplicationModal" 
      preset="card" 
      style="width: 900px" 
      title="商家入驻申请审核"
      class="rounded-xl"
    >
      <div v-if="!applicationLoading">
        <n-data-table
          :columns="applicationColumns"
          :data="applicationData"
          :pagination="{ pageSize: 10 }"
          flex-height
          max-height="500px"
        />
      </div>
      
      <div v-else class="flex-center p-40px">
        <n-spin size="large" />
      </div>
      
      <template #footer>
        <div class="flex-end">
          <n-button @click="showApplicationModal = false">关闭</n-button>
        </div>
      </template>
    </n-modal>
    
    <!-- 审核弹窗 -->
    <n-modal 
      v-model:show="showAuditModal" 
      preset="card" 
      style="width: 500px" 
      title="审核申请"
      class="rounded-xl"
    >
      <div v-if="!auditLoading">
        <n-form :model="auditForm" label-placement="left" label-width="80">
          <n-form-item label="申请信息">
            <div>
              <p><strong>申请ID:</strong> {{ currentApplication.id || '-' }}</p>
              <p><strong>申请类型:</strong> {{ currentApplication.type || '商家入驻' }}</p>
              <p><strong>申请时间:</strong> {{ currentApplication.createdAt || '-' }}</p>
            </div>
          </n-form-item>
          
          <n-form-item label="审核结果" required>
            <n-select 
              v-model:value="auditForm.decision" 
              :options="[
                { label: '同意', value: 'approve' },
                { label: '拒绝', value: 'reject' }
              ]"
              placeholder="选择审核结果"
            />
          </n-form-item>
          
          <n-form-item label="审核备注">
            <n-input 
              v-model:value="auditForm.reason" 
              type="textarea" 
              placeholder="请输入审核备注（可选）"
              :rows="3"
            />
          </n-form-item>
        </n-form>
      </div>
      
      <div v-else class="flex-center p-40px">
        <n-spin size="large" />
      </div>
      
      <template #footer>
        <div class="flex-end gap-8px">
          <n-button @click="showAuditModal = false">取消</n-button>
          <n-button type="primary" @click="submitAudit" :loading="auditLoading">
            提交审核
          </n-button>
        </div>
      </template>
    </n-modal>
  </div>
</template>

<style scoped></style> 