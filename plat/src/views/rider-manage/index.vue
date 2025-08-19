<script setup lang="tsx">
import { ref, onMounted } from 'vue';
import { NButton, NInput, NSelect, NSpace, NCard, NModal, NForm, NFormItem, NText, NSpin, NBadge, NGrid, NGi, useMessage } from 'naive-ui';
import type { DataTableColumns } from 'naive-ui';
import { fetchRidersList, fetchRiderInfo, fetchRiderPerformanceRanking } from '@/api/rider';

defineOptions({
  name: 'RiderManage'
});

const message = useMessage();
const loading = ref(false);
const tableData = ref<any[]>([]);
const pagination = ref({
  page: 1,
  pageSize: 20,
  totalCount: 0,
  totalPages: 0
});

// 搜索条件
const searchParams = ref({
  searchTerm: '',
  status: null,
  page: 1,
  pageSize: 20
});

// 状态选项
const statusOptions = [
  { label: '全部状态', value: null },
  { label: '在线', value: 1 },
  { label: '离线', value: 0 },
  { label: '忙碌', value: 2 }
];

// 详情弹窗相关
const showDetailModal = ref(false);
const detailLoading = ref(false);
const riderDetail = ref<any>({});
const riderRanking = ref<any>({});

// 当前日期
const currentDate = new Date();
const currentYear = currentDate.getFullYear();
const currentMonth = currentDate.getMonth() + 1;

// 获取骑手列表数据
async function getRidersList() {
  try {
    loading.value = true;
    
    // 构建查询参数
    const params = {
      page: searchParams.value.page,
      pageSize: searchParams.value.pageSize
    };
    
    if (searchParams.value.searchTerm) {
      params.searchTerm = searchParams.value.searchTerm;
    }
    
    if (searchParams.value.status !== null) {
      params.status = searchParams.value.status;
    }
    
    console.log('🚀 开始请求骑手列表，参数:', params);
    const response = await fetchRidersList(params);
    console.log('📥 收到骑手列表响应:', response);
    
    if (response && response.data) {
      console.log('详细的响应数据结构:', response.data);
      
      // 尝试多种可能的数据结构
      let riders = null;
      let paginationInfo = null;
      
      // 情况1: 标准格式 { Riders, Pagination }
      if (response.data.Riders && Array.isArray(response.data.Riders)) {
        riders = response.data.Riders;
        paginationInfo = response.data.Pagination;
      }
      // 情况2: 小写格式 { riders, pagination }
      else if (response.data.riders && Array.isArray(response.data.riders)) {
        riders = response.data.riders;
        paginationInfo = response.data.pagination;
      }
      // 情况3: 直接是数组
      else if (Array.isArray(response.data)) {
        riders = response.data;
        paginationInfo = null;
      }
      
      console.log('解析出的骑手数据:', riders);
      console.log('解析出的分页信息:', paginationInfo);
      
      if (riders && Array.isArray(riders)) {
        tableData.value = riders.map((rider, index) => ({
          ...rider,
          index: (pagination.value.page - 1) * pagination.value.pageSize + index + 1,
          riderId: rider.riderId || rider.RiderId || '-',
          name: rider.name || rider.Name || '-',
          phoneNumber: rider.phoneNumber || rider.PhoneNumber || '-',
          vehicleNumber: rider.vehicleNumber || rider.VehicleNumber || '-',
          applicationUserId: rider.applicationUserId || rider.ApplicationUserId || '-'
        }));
        
        console.log('处理后的表格数据:', tableData.value);
        
        // 更新分页信息
        if (paginationInfo) {
          pagination.value = {
            page: paginationInfo.page || paginationInfo.Page || 1,
            pageSize: paginationInfo.pageSize || paginationInfo.PageSize || 20,
            totalCount: paginationInfo.totalCount || paginationInfo.TotalCount || 0,
            totalPages: paginationInfo.totalPages || paginationInfo.TotalPages || 0
          };
        } else {
          // 如果没有分页信息，使用数组长度
          pagination.value.totalCount = riders.length;
          pagination.value.totalPages = Math.ceil(riders.length / pagination.value.pageSize);
        }
      } else {
        console.log('没有找到有效的骑手数据数组');
        tableData.value = [];
      }
    } else {
      message.error(response?.message || '获取骑手列表失败');
      tableData.value = [];
    }
  } catch (error) {
    message.error('获取骑手列表失败: ' + error.message);
    tableData.value = [];
    console.error('Error fetching riders list:', error);
  } finally {
    loading.value = false;
  }
}

// 搜索骑手
function handleSearch() {
  searchParams.value.page = 1;
  pagination.value.page = 1;
  getRidersList();
}

// 重置搜索条件
function handleReset() {
  searchParams.value = {
    searchTerm: '',
    status: null,
    page: 1,
    pageSize: 20
  };
  pagination.value.page = 1;
  getRidersList();
}

// 分页变化
function handlePageChange(page: number) {
  searchParams.value.page = page;
  pagination.value.page = page;
  getRidersList();
}

const columns: DataTableColumns = [
  {
    key: 'index',
    title: '序号',
    align: 'center',
    width: 80
  },
  {
    key: 'riderId',
    title: '骑手ID',
    align: 'center',
    width: 150,
    ellipsis: {
      tooltip: true
    }
  },
  {
    key: 'name',
    title: '姓名',
    align: 'center',
    width: 120
  },
  {
    key: 'phoneNumber',
    title: '手机号',
    align: 'center',
    width: 130
  },
  {
    key: 'vehicleNumber',
    title: '车牌号',
    align: 'center',
    width: 120
  },
  {
    key: 'applicationUserId',
    title: '用户ID',
    align: 'center',
    width: 150,
    ellipsis: {
      tooltip: true
    }
  },
  {
    key: 'actions',
    title: '操作',
    align: 'center',
    width: 150,
    render: (row) => (
      <div class="flex-center gap-8px">
        <NButton type="primary" size="small" onClick={() => handleViewDetail(row.riderId)}>查看绩效</NButton>
      </div>
    )
  }
];

// 查看骑手详情
async function handleViewDetail(riderId: string) {
  try {
    detailLoading.value = true;
    showDetailModal.value = true;
    riderDetail.value = {};
    riderRanking.value = {};
    
    console.log(`获取骑手详情和绩效排名，ID: ${riderId}`);
    
    // 并发请求骑手详情和绩效排名
    const [detailResponse, rankingResponse] = await Promise.allSettled([
      fetchRiderInfo(riderId),
      fetchRiderPerformanceRanking(riderId, currentYear, currentMonth)
    ]);
    
    // 处理骑手详情
    if (detailResponse.status === 'fulfilled' && detailResponse.value?.data) {
      riderDetail.value = detailResponse.value.data;
      console.log('骑手详情:', riderDetail.value);
    } else {
      console.error('获取骑手详情失败:', detailResponse);
    }
    
    // 处理绩效排名
    if (rankingResponse.status === 'fulfilled' && rankingResponse.value?.data) {
      riderRanking.value = rankingResponse.value.data;
      console.log('骑手绩效排名:', riderRanking.value);
    } else {
      console.error('获取骑手绩效排名失败:', rankingResponse);
    }
    
  } catch (error) {
    console.error('获取骑手信息失败:', error);
    message.error('获取骑手信息失败: ' + (error.message || '未知错误'));
    showDetailModal.value = false;
  } finally {
    detailLoading.value = false;
  }
}

// 注释：原 handleViewPerformance 函数已删除，功能合并到 handleViewDetail

// 格式化排名键名
function formatRankingKey(key: string) {
  const keyMap = {
    'totalOrdersRanking': '总订单排名',
    'onTimeRateRanking': '准时率排名', 
    'goodReviewRateRanking': '好评率排名',
    'incomeRanking': '收入排名',
    'TotalOrdersRanking': '总订单排名',
    'OnTimeRateRanking': '准时率排名',
    'GoodReviewRateRanking': '好评率排名',
    'IncomeRanking': '收入排名',
    // 英文字段名翻译
    'IncomeRank': '收入排名',
    'OnTimeRateRank': '准时率排名',
    'GoodReviewRateRank': '好评率排名',
    'TotalOrdersRank': '总订单排名'
  };
  return keyMap[key] || key;
}

// 获取排名徽章类型
function getRankingBadgeType(rank: number) {
  if (rank <= 3) return 'success';  // 前3名 - 绿色
  if (rank <= 10) return 'warning'; // 4-10名 - 黄色
  return 'info';                    // 其他 - 蓝色
}

// 组件挂载时获取数据
onMounted(() => {
  getRidersList();
});
</script>

<template>
  <div class="m-16px">
    <!-- 搜索条件 -->
    <n-card title="搜索条件" class="mb-16px">
      <n-space :size="12" align="center">
        <n-input
          v-model:value="searchParams.searchTerm"
          placeholder="请输入骑手姓名或手机号"
          clearable
          style="width: 250px"
        />
        <n-select
          v-model:value="searchParams.status"
          :options="statusOptions"
          placeholder="选择状态"
          style="width: 120px"
          clearable
        />
        <n-button type="primary" @click="handleSearch">
          搜索
        </n-button>
        <n-button @click="handleReset">
          重置
        </n-button>
      </n-space>
    </n-card>

    <!-- 骑手列表 -->
    <n-card title="骑手管理" :bordered="false" class="h-full">
      <template #header-extra>
        <span class="text-gray-500">
          共 {{ pagination.totalCount }} 名骑手
        </span>
      </template>
      <n-data-table
        :columns="columns"
        :data="tableData"
        :loading="loading"
        :pagination="{
          page: pagination.page,
          pageSize: pagination.pageSize,
          itemCount: pagination.totalCount,
          showSizePicker: true,
          pageSizes: [10, 20, 50, 100],
          onUpdatePage: handlePageChange,
          onUpdatePageSize: (pageSize) => {
            searchParams.pageSize = pageSize;
            pagination.pageSize = pageSize;
            handleSearch();
          }
        }"
        flex-height
        class="h-full"
      />
    </n-card>
    
    <!-- 骑手详情弹窗 -->
    <n-modal 
      v-model:show="showDetailModal" 
      preset="card" 
      style="width: 800px" 
      title="骑手绩效信息"
      class="rounded-xl"
    >
      <div v-if="!detailLoading">
        <n-grid :cols="2" :x-gap="20" :y-gap="16">
          <!-- 基本信息 -->
          <n-gi>
            <n-card title="基本信息" size="small">
              <n-form label-placement="left" label-width="90">
                <n-form-item label="骑手ID">
                  <n-text>{{ riderDetail.riderId || riderDetail.RiderId || '-' }}</n-text>
                </n-form-item>
                <n-form-item label="姓名">
                  <n-text>{{ riderDetail.name || riderDetail.Name || '-' }}</n-text>
                </n-form-item>
                <n-form-item label="手机号">
                  <n-text>{{ riderDetail.phoneNumber || riderDetail.PhoneNumber || '-' }}</n-text>
                </n-form-item>
                <n-form-item label="车牌号">
                  <n-text>{{ riderDetail.vehicleNumber || riderDetail.VehicleNumber || '-' }}</n-text>
                </n-form-item>
                <n-form-item label="用户ID">
                  <n-text style="word-break: break-all;">{{ riderDetail.applicationUserId || riderDetail.ApplicationUserId || '-' }}</n-text>
                </n-form-item>
              </n-form>
            </n-card>
          </n-gi>
          
          <!-- 当月绩效排名 -->
          <n-gi>
            <n-card :title="`${currentYear}年${currentMonth}月绩效排名`" size="small">
              <div v-if="Object.keys(riderRanking).length > 0">
                <n-space vertical :size="12">
                  <div v-for="(value, key) in riderRanking" :key="key" class="flex justify-between items-center">
                    <span class="text-gray-600">{{ formatRankingKey(key) }}：</span>
                    <n-badge 
                      :value="value" 
                      :type="getRankingBadgeType(value)"
                      :max="999"
                      show-zero
                    />
                  </div>
                </n-space>
              </div>
              <div v-else class="text-center text-gray-500 py-8">
                暂无当月绩效排名数据
              </div>
            </n-card>
          </n-gi>
        </n-grid>
      </div>
      
      <div v-else class="flex-center p-40px">
        <n-spin size="large" />
      </div>
      
      <template #footer>
        <div class="flex-end">
          <n-button @click="showDetailModal = false">关闭</n-button>
        </div>
      </template>
    </n-modal>
  </div>
</template>

<style scoped></style> 