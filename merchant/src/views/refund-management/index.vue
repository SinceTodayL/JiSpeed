<script setup lang="tsx">
import { reactive, ref, computed, onMounted } from 'vue';
import { NButton, NPopconfirm, NTag, NDataTable, NCard, NSpace, NSelect } from 'naive-ui';
import type { DataTableColumns, DataTableRowKey } from 'naive-ui';
import { fetchMerchantRefunds, fetchRefundDetailsBatch, approveRefund, rejectRefund, getRefundStatusText, REFUND_STATUS_MAP } from '@/service/api';
import { useMerchantStore } from '@/store/modules/merchant';
import { useAppStore } from '@/store/modules/app';
import { $t } from '@/locales';

const appStore = useAppStore();
const merchantStore = useMerchantStore();

type Model = Api.Refund.RefundItem;

const data = ref<Model[]>([]);
const loading = ref(false);
const originalData = ref<Model[]>([]);

// 搜索参数 - 默认显示待处理的退款申请
const searchParams = reactive({
  auditStatus: 0 as number | null // 默认显示待处理状态，商家可以操作
});

// 表格操作相关
const checkedRowKeys = ref<DataTableRowKey[]>([]);

// 状态选项（基于RefundStatus枚举）
const statusOptions = [
  { label: '全部', value: null as any },
  { label: '待处理', value: 0 },      // Default - 商家可操作
  { label: '商家拒绝', value: 1 },     // Rejected  
  { label: '商家退款', value: 2 },     // Refunded
  { label: '等待管理员处理', value: 3 }, // DefaultForAdmin
  { label: '管理员拒绝', value: 4 },    // RejectedForAdmin
  { label: '管理员同意', value: 5 }     // RefundedForAdmin
];

// 获取数据
const getData = async () => {
  loading.value = true;
  try {
    console.log('开始获取退款申请数据，商家ID:', merchantStore.merchantId);
    
    if (!merchantStore.merchantId) {
      console.warn('商家ID为空，无法获取退款数据');
      loading.value = false;
      return;
    }
    
    // 获取退款申请ID列表
    const result = await fetchMerchantRefunds(merchantStore.merchantId, {
      auditStatus: searchParams.auditStatus || undefined
    });
    
    console.log('获取到退款申请ID列表:', result);
    
    const refundIds = (result as any)?.data;
    if (Array.isArray(refundIds) && refundIds.length > 0) {
      console.log('获取到退款申请ID:', refundIds.length, '个');
      
      // 批量获取退款详情
      const refundDetails = await fetchRefundDetailsBatch(refundIds);
      
      console.log('获取到退款详情:', refundDetails.length, '条记录');
      
      if (refundDetails.length > 0) {
        console.log('=== 退款数据状态分析 ===');
        refundDetails.forEach((refund, index) => {
          console.log(`退款${index + 1}:`, {
            refundId: refund.refundId,
            auditStatus: refund.auditStatus,
            statusText: REFUND_STATUS_MAP[refund.auditStatus] || '未知状态',
            canOperate: refund.auditStatus === 0 ? '✅ 可操作' : '❌ 不可操作'
          });
        });
        
        const operableCount = refundDetails.filter(r => r.auditStatus === 0).length;
        console.log(`总计 ${refundDetails.length} 条退款，其中 ${operableCount} 条可操作`);
        
        originalData.value = refundDetails;
        data.value = [...refundDetails];
        
        if (operableCount === 0 && searchParams.auditStatus === 0) {
          window.$message?.info('当前没有待处理的退款申请');
        }
      } else {
        console.log('所有退款详情获取失败或无有效退款');
        window.$message?.warning('未能获取到有效的退款详情信息');
        originalData.value = [];
        data.value = [];
      }
    } else {
      console.log('没有找到退款申请数据');
      originalData.value = [];
      data.value = [];
    }
  } catch (error) {
    console.error('获取退款申请数据失败:', error);
    
    let errorMessage = '获取退款数据失败，请稍后重试';
    
    const errorAny = error as any;
    if (errorAny?.response?.status === 401) {
      errorMessage = '认证失败，请重新登录';
    } else if (errorAny?.response?.status === 403) {
      errorMessage = '权限不足，无法访问退款数据';
    } else if (errorAny?.response?.status === 404) {
      errorMessage = 'API端点不存在，请检查后端服务状态';
    } else if (errorAny?.code === 'ERR_NETWORK') {
      errorMessage = '网络连接失败，请检查后端服务是否运行';
    }
    
    window.$message?.error(errorMessage);
    originalData.value = [];
    data.value = [];
  } finally {
    loading.value = false;
  }
};

// 处理退款申请 - 同意
const handleApproveRefund = async (refundId: string) => {
  if (!merchantStore.merchantId) {
    window.$message?.error('商家信息未找到，无法操作');
    return;
  }

  try {
    console.log(`=== 商家同意退款申请 ===`);
    console.log('商家ID:', merchantStore.merchantId);
    console.log('退款ID:', refundId);
    
    const result = await approveRefund(merchantStore.merchantId, refundId);
    console.log('同意退款API响应:', result);
    
    // 解析API响应
    const resultAny = result as any;
    let success = false;
    let message = '';
    
    if (resultAny?.data === true || resultAny?.code === 0) {
      success = true;
      message = resultAny?.message || '退款申请已同意';
    } else if (resultAny === true) {
      success = true;
      message = '退款申请已同意';
    }
    
    if (success) {
      window.$message?.success(message);
      console.log('同意退款成功，开始刷新数据');
      await getData(); // 刷新数据
    } else {
      console.warn('同意退款失败，API返回:', result);
      window.$message?.error(message || '同意退款失败');
    }
    
  } catch (error) {
    console.error('同意退款API调用失败:', error);
    const errorAny = error as any;
    const errorMessage = errorAny?.response?.data?.message || errorAny?.message || '同意退款失败，请重试';
    window.$message?.error(errorMessage);
  }
};

// 处理退款申请 - 拒绝
const handleRejectRefund = async (refundId: string) => {
  if (!merchantStore.merchantId) {
    window.$message?.error('商家信息未找到，无法操作');
    return;
  }

  try {
    console.log(`=== 商家拒绝退款申请 ===`);
    console.log('商家ID:', merchantStore.merchantId);
    console.log('退款ID:', refundId);
    
    const result = await rejectRefund(merchantStore.merchantId, refundId);
    console.log('拒绝退款API响应:', result);
    
    // 解析API响应
    const resultAny = result as any;
    let success = false;
    let message = '';
    
    if (resultAny?.data === true || resultAny?.code === 0) {
      success = true;
      message = resultAny?.message || '退款申请已拒绝';
    } else if (resultAny === true) {
      success = true;
      message = '退款申请已拒绝';
    }
    
    if (success) {
      window.$message?.success(message);
      console.log('拒绝退款成功，开始刷新数据');
      await getData(); // 刷新数据
    } else {
      console.warn('拒绝退款失败，API返回:', result);
      window.$message?.error(message || '拒绝退款失败');
    }
    
  } catch (error) {
    console.error('拒绝退款API调用失败:', error);
    const errorAny = error as any;
    const errorMessage = errorAny?.response?.data?.message || errorAny?.message || '拒绝退款失败，请重试';
    window.$message?.error(errorMessage);
  }
};

// 搜索功能
const handleSearch = () => {
  getData();
};

// 重置搜索 - 重置为默认的待处理状态
const handleReset = () => {
  searchParams.auditStatus = 0; // 重置为待处理状态，方便商家操作
  getData();
};

// 表格列定义
const columns: DataTableColumns<Model> = [
  {
    type: 'selection',
    disabled(row: Model) {
      return row.auditStatus !== 1; // 只有待处理状态才能选择
    }
  },
  {
    key: 'index',
    title: '序号',
    width: 60,
    render: (_, index) => index + 1
  },
  {
    key: 'refundId',
    title: '退款申请ID',
    width: 140,
    ellipsis: {
      tooltip: true
    },
    render: (row) => (
      <span class="text-blue-600">
        {row.refundId.slice(-8)}
      </span>
    )
  },
  {
    key: 'orderId',
    title: '关联订单ID',
    width: 140,
    ellipsis: {
      tooltip: true
    },
    render: (row) => (
      <span class="text-purple-600">
        {row.orderId.slice(-8)}
      </span>
    )
  },
  {
    key: 'refundAmount',
    title: '退款金额',
    width: 120,
    render: (row) => (
      <span class="text-green-600 font-semibold">
        ¥{(row.refundAmount / 100).toFixed(2)}
      </span>
    )
  },
  {
    key: 'reason',
    title: '退款理由',
    width: 150,
    ellipsis: {
      tooltip: true
    },
    render: (row) => (
      <span>
        {row.reason || '无'}
      </span>
    )
  },
  {
    key: 'auditStatus',
    title: '审核状态',
    width: 100,
    render: (row) => {
      const statusText = getRefundStatusText(row.auditStatus);
      let type: 'default' | 'success' | 'warning' | 'error' | 'info' = 'default';
      
      switch (row.auditStatus) {
        case 0:
          type = 'warning'; // 默认状态，需要处理
          break;
        case 1:
          type = 'error'; // 商家拒绝
          break;
        case 2:
          type = 'success'; // 商家退款
          break;
        case 3:
          type = 'info'; // 等待管理员处理
          break;
        case 4:
          type = 'error'; // 管理员拒绝
          break;
        case 5:
          type = 'success'; // 管理员同意退款
          break;
        default:
          type = 'default';
      }
      
      return <NTag type={type}>{statusText}</NTag>;
    }
  },
  {
    key: 'applyAt',
    title: '申请时间',
    width: 160,
    render: (row) => (
      <span class="text-gray-600">
        {new Date(row.applyAt).toLocaleString('zh-CN', {
          month: '2-digit',
          day: '2-digit',
          hour: '2-digit',
          minute: '2-digit'
        })}
      </span>
    )
  },
  {
    key: 'finishAt',
    title: '处理时间',
    width: 160,
    render: (row) => (
      <span class="text-gray-600">
        {row.finishAt ? new Date(row.finishAt).toLocaleString('zh-CN', {
          month: '2-digit',
          day: '2-digit',
          hour: '2-digit',
          minute: '2-digit'
        }) : '未处理'}
      </span>
    )
  },
  {
    key: 'operate',
    title: '操作',
    width: 180,
    fixed: 'right' as const,
    render: (row) => {
      console.log(`渲染操作列 - 退款ID: ${row.refundId}, 状态: ${row.auditStatus}, 可操作: ${row.auditStatus === 0}`);
      
      // 只有默认状态(0)的退款申请可以被商家处理
      if (row.auditStatus !== 0) {
        return <span class="text-gray-400">已处理</span>;
      }
      
      return (
        <NSpace size={8}>
          <NPopconfirm
            onPositiveClick={() => handleApproveRefund(row.refundId)}
            positiveText="确认同意"
            negativeText="取消"
          >
            {{
              default: () => '确认同意此退款申请？',
              trigger: () => (
                <NButton size="small" type="success" ghost>
                  同意退款
                </NButton>
              )
            }}
          </NPopconfirm>
          
          <NPopconfirm
            onPositiveClick={() => handleRejectRefund(row.refundId)}
            positiveText="确认拒绝"
            negativeText="取消"
          >
            {{
              default: () => '确认拒绝此退款申请？',
              trigger: () => (
                <NButton size="small" type="error" ghost>
                  拒绝退款
                </NButton>
              )
            }}
          </NPopconfirm>
        </NSpace>
      );
    }
  }
];

// 组件挂载时获取数据
onMounted(() => {
  getData();
});
</script>

<template>
  <div class="min-h-500px flex-col-stretch gap-16px overflow-hidden <sm:overflow-auto">
    <NCard :title="'退款管理'" :bordered="false" size="small" class="sm:flex-1-hidden card-wrapper">
      <!-- 搜索区域 -->
      <div class="mb-4">
        <NCard :bordered="false" size="small">
          <div class="flex items-center gap-4">
            <NSelect
              v-model:value="searchParams.auditStatus"
              :options="statusOptions"
              placeholder="审核状态"
              clearable
              class="w-200px"
            />
            <NButton type="primary" @click="handleSearch">
              搜索
            </NButton>
            <NButton @click="handleReset">
              重置
            </NButton>
          </div>
        </NCard>
      </div>

      <!-- API 缺失提示 -->
       <!--
      <div class="mb-4">
        <NCard :bordered="true" size="small">
          <div class="text-orange-600">
            <div class="font-semibold mb-2">⚠️ API 缺失提醒：</div>
            <ul class="list-disc list-inside space-y-1 text-sm">
              <li><strong>缺少API：</strong>GET /api/refunds/{refundId} - 获取单个退款申请详情</li>
              <li><strong>或者：</strong>POST /api/refunds/batch - 批量获取退款申请详情</li>
              <li><strong>影响：</strong>无法显示退款金额、申请理由、关联订单、申请时间等信息</li>
              <li><strong>建议：</strong>请联系后端开发人员添加获取退款详情的API接口</li>
            </ul>
          </div>
        </NCard>
      </div>
      -->
      
      <!-- 数据表格 -->
      <NDataTable
        v-model:checked-row-keys="checkedRowKeys"
        :columns="columns"
        :data="data"
        size="small"
        :loading="loading"
        :scroll-x="1170"
        :flex-height="!appStore.isMobile"
        :row-key="(row: Model) => row.refundId"
        :pagination="{
          pageSize: 20,
          showSizePicker: true,
          showQuickJumper: true,
          pageSizes: [10, 20, 50, 100]
        }"
        :single-line="false"
        class="sm:h-full"
      />
    </NCard>
  </div>
</template>

<style scoped>
.card-wrapper {
  flex: 1;
  overflow: hidden;
}
</style>
