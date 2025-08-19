<script setup lang="tsx">
import { ref, onMounted } from 'vue';
import { NButton, NTag, NModal, NForm, NFormItem, NSelect, NInput, NCard, NDataTable, NBadge, NCheckbox, NInputNumber, useMessage } from 'naive-ui';
import type { DataTableColumns } from 'naive-ui';
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
  adminId: '',
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

// 获取投诉列表数据
async function getComplaintList() {
  try {
    loading.value = true;
    
    // 构建查询参数，只传递非空值
    const params = {};
    if (searchForm.value.userId && searchForm.value.userId.trim()) params.userId = searchForm.value.userId.trim();
    if (searchForm.value.merchantId && searchForm.value.merchantId.trim()) params.merchantId = searchForm.value.merchantId.trim();
    if (searchForm.value.status !== null && searchForm.value.status !== undefined) params.status = searchForm.value.status;
    if (searchForm.value.adminId && searchForm.value.adminId.trim()) params.adminId = searchForm.value.adminId.trim();
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
  { key: 'complaintId', title: '投诉ID', align: 'center', width: 120 },
  { key: 'orderId', title: '订单ID', align: 'center', width: 120 },
  { 
    key: 'cmplRole',
    title: '投诉类型', 
    align: 'center',
    render: (row) => getCmplRoleText(row.cmplRole)
  },
  { key: 'cmplDescription', title: '投诉内容', align: 'center', width: 250 },
  { key: 'complainantId', title: '投诉人ID', align: 'center' },
  { 
    key: 'createdAt', 
    title: '投诉时间', 
    align: 'center',
    render: (row) => formatDateTime(row.createdAt)
  },
  {
    key: 'cmplStatus',
    title: '状态',
    align: 'center',
    render: (row) => {
      let tagType: 'warning' | 'success' | 'info' = 'warning';
      let statusText = getCmplStatusText(row.cmplStatus);
      if (row.cmplStatus === 2) tagType = 'success'; // 已处理
      if (row.cmplStatus === 1) tagType = 'info'; // 处理中
      return <NTag type={tagType}>{statusText}</NTag>;
    }
  },
  {
    key: 'actions',
    title: '操作',
    align: 'center',
    render: (row) => (
      <NButton type="primary" size="small" onClick={() => handleComplaint(row)}>
        {row.cmplStatus === 2 ? '查看详情' : '处理'}
      </NButton>
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
  getComplaintList();
}

// 重置搜索条件
function handleReset() {
  searchForm.value = {
    userId: '',
    merchantId: '',
    status: null,
    adminId: '',
    size: null,
    page: null
  };
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
  <div class="m-16px">
    <!-- 投诉处理界面 -->
    <NCard title="投诉处理" :bordered="false" class="h-full">
      <template #header-extra>
        <div class="flex items-center space-x-2">
          <NBadge :value="tableData.length" type="info" />
          <span class="text-sm text-gray-500">投诉记录</span>
        </div>
      </template>
      
      <!-- 搜索筛选区域 -->
      <div class="mb-4 p-4 bg-gray-50 rounded-lg">
        <NForm :model="searchForm" inline label-placement="left" label-width="80">
          <NFormItem label="用户ID">
            <NInput 
              v-model:value="searchForm.userId" 
              placeholder="输入用户ID"
              class="w-32"
            />
          </NFormItem>
          <NFormItem label="商家ID">
            <NInput 
              v-model:value="searchForm.merchantId" 
              placeholder="输入商家ID"
              class="w-32"
            />
          </NFormItem>
          <NFormItem label="状态">
            <NSelect 
              v-model:value="searchForm.status" 
              :options="complaintStatusFilterOptions"
              placeholder="选择状态"
              class="w-32"
            />
          </NFormItem>
          <NFormItem label="管理员ID">
            <NInput 
              v-model:value="searchForm.adminId" 
              placeholder="输入管理员ID"
              class="w-32"
            />
          </NFormItem>
          <NFormItem>
            <div class="flex space-x-2">
              <NButton type="primary" @click="handleSearch">
                <template #icon>
                  <SvgIcon icon="material-symbols:search" />
                </template>
                搜索
              </NButton>
              <NButton @click="handleReset">
                <template #icon>
                  <SvgIcon icon="material-symbols:refresh" />
                </template>
                重置
              </NButton>
            </div>
          </NFormItem>
        </NForm>
      </div>
      
      <NDataTable
        :columns="columns"
        :data="tableData"
        :loading="loading"
        :pagination="false"
        flex-height
        class="h-full"
      />
    </NCard>

    <!-- 投诉处理弹窗 -->
    <NModal 
      v-model:show="showModal" 
      preset="card" 
      style="width: 650px" 
      title="投诉处理"
      class="rounded-xl"
    >
      <div class="mb-4 p-4 bg-gray-50 rounded-lg">
        <h4 class="font-medium mb-2">投诉详情</h4>
        <div class="grid grid-cols-2 gap-3 text-sm">
          <p><strong>投诉ID：</strong>{{ currentItem.complaintId }}</p>
          <p><strong>订单ID：</strong>{{ currentItem.orderId }}</p>
          <p><strong>投诉类型：</strong>{{ getCmplRoleText(currentItem.cmplRole) }}</p>
          <p><strong>投诉人ID：</strong>{{ currentItem.complainantId }}</p>
          <p><strong>当前状态：</strong>{{ getCmplStatusText(currentItem.cmplStatus) }}</p>
          <p><strong>投诉时间：</strong>{{ formatDateTime(currentItem.createdAt) }}</p>
        </div>
        <div class="mt-3">
          <p><strong>投诉内容：</strong></p>
          <p class="mt-1 p-2 bg-white rounded border text-sm">{{ currentItem.cmplDescription || '无详细描述' }}</p>
        </div>
      </div>
      
      <NForm :model="auditForm" label-placement="left" label-width="90">
        <NFormItem label="处理结果" required>
          <NSelect 
            v-model:value="auditForm.status" 
            :options="statusOptions"
            placeholder="请选择处理结果"
            class="rounded-lg"
          />
        </NFormItem>
        
        <NFormItem label="处理原因">
          <NInput 
            v-model:value="auditForm.reason" 
            placeholder="请输入处理原因"
            class="rounded-lg"
          />
        </NFormItem>
        
        <NFormItem label="是否退款">
          <div class="flex items-center space-x-4">
            <NCheckbox v-model:checked="auditForm.needRefund">
              需要为用户退款
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
            class="w-full rounded-lg"
          >
            <template #prefix>¥</template>
          </NInputNumber>
        </NFormItem>
        
        <NFormItem label="备注说明">
          <NInput 
            v-model:value="auditForm.remark" 
            type="textarea" 
            placeholder="请输入备注说明"
            :rows="3"
            class="rounded-lg"
          />
        </NFormItem>
        
        <div class="flex space-x-3 mt-6">
          <NButton @click="showModal = false" class="flex-1 rounded-lg">
            取消
          </NButton>
          <NButton 
            type="primary" 
            @click="submitComplaintHandle"
            class="flex-1 bg-gradient-to-r from-blue-500 to-purple-500 border-none rounded-lg"
          >
            确认处理
          </NButton>
        </div>
      </NForm>
    </NModal>
  </div>
</template>

<style scoped></style> 