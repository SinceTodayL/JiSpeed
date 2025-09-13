<script setup lang="tsx">
import { ref, onMounted, computed } from 'vue';
import { 
  NButton, 
  NInput, 
  NSelect, 
  NSpace, 
  NCard, 
  NModal, 
  NForm, 
  NFormItem, 
  NText, 
  NSpin, 
  NBadge, 
  NGrid, 
  NGi, 
  NStatistic,
  NIcon,
  NAvatar,
  NDivider,
  NDataTable,
  NTag,
  useMessage 
} from 'naive-ui';
import type { DataTableColumns } from 'naive-ui';
import { 
  BicycleOutline,
  SearchOutline,
  RefreshOutline,
  PersonOutline,
  CallOutline,
  CarOutline,
  TrophyOutline,
  StatsChartOutline,
  EyeOutline,
  WalletOutline,
  FlashOutline,
  PauseCircleOutline,
  TimeOutline
} from '@vicons/ionicons5';
import { fetchRidersList, fetchRiderInfo, fetchRiderPerformanceRanking } from '@/api/rider';
import { getAllOrdersWithDetails } from '@/api/order';


defineOptions({
  name: 'RiderManage'
});

const message = useMessage();
const loading = ref(false);
const tableData = ref<any[]>([]);

// 订单相关数据
const ordersLoading = ref(false);
const ordersData = ref<any[]>([]);
const showOrdersModal = ref(false);
const totalOrdersCount = ref(0); // 总订单数

const pagination = ref({
  page: 1,
  pageSize: 20,
  totalCount: 0,
  totalPages: 0
});

// 搜索条件
const searchParams = ref({
  searchTerm: '',
  page: 1,
  pageSize: 20
});



// 骑手统计数据
const riderStats = computed(() => {
  const total = tableData.value.length;
  const onlineCount = tableData.value.filter(rider => rider.status === 1).length;
  
  return {
    total,
    onlineCount,
  };
});

// 过滤后的表格数据
const filteredTableData = computed(() => {
  return tableData.value.filter(rider => {
    if (searchParams.value.searchTerm) {
      const searchTerm = searchParams.value.searchTerm.toLowerCase();
      if (!rider.name?.toLowerCase().includes(searchTerm) && 
          !rider.phoneNumber?.toLowerCase().includes(searchTerm)) {
        return false;
      }
    }

    return true;
  });
});

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
  message.success(`找到 ${filteredTableData.value.length} 名骑手`);
}

// 重置搜索条件
function handleReset() {
  searchParams.value = {
    searchTerm: '',
    status: null,
    page: 1,
    pageSize: 20
  };
  message.info('搜索条件已重置');
}

// 刷新数据
function handleRefresh() {
  message.info('正在刷新骑手数据...');
  getRidersList();
}



// 分页变化
function handlePageChange(page: number) {
  searchParams.value.page = page;
  pagination.value.page = page;
  getRidersList();
}

// 订单表格列定义
const ordersColumns: DataTableColumns = [
  {
    key: 'index',
    title: '序号',
    align: 'center',
    width: 60,
    render: (row) => row.index
  },
  {
    key: 'OrderId',
    title: '订单ID',
    width: 100,
    render: (row) => (
      <n-text code style="font-size: 11px;">
        {row.OrderId?.slice(-8) || '-'}
      </n-text>
    )
  },
  {
    key: 'OrderStatus',
    title: '状态',
    align: 'center',
    width: 90,
    render: (row) => (
      <n-tag type={row.statusType} size="small">
        {row.statusText}
      </n-tag>
    )
  },
  {
    key: 'User',
    title: '用户信息',
    width: 120,
    render: (row) => (
      <div class="flex flex-col">
        <n-text strong style="font-size: 12px;">
          {row.User?.Nickname || '未知用户'}
        </n-text>
        <n-text depth="3" style="font-size: 10px;">
          ID: {row.User?.UserId?.slice(-6) || '-'}
        </n-text>
      </div>
    )
  },
  {
    key: 'Merchant',
    title: '商家信息',
    width: 140,
    render: (row) => (
      <div class="flex flex-col">
        <n-text strong style="font-size: 12px;">
          {row.Merchant?.MerchantName || '未知商家'}
        </n-text>
        <n-text depth="3" style="font-size: 10px; word-break: break-all;">
          {row.Merchant?.Location || '-'}
        </n-text>
      </div>
    )
  },
  {
    key: 'Assignment',
    title: '分配信息',
    width: 140,
    render: (row) => {
      if (!row.Assignment) {
        return (
          <n-tag type="warning" size="small">
            未分配
          </n-tag>
        );
      }
      return (
        <div class="flex flex-col">
          <n-text strong style="font-size: 12px;">
            {row.Assignment.RiderName || '未知骑手'}
          </n-text>
          <n-text depth="3" style="font-size: 10px;">
            {row.Assignment.RiderPhoneNumber || '-'}
          </n-text>
          <n-tag 
            type={row.Assignment.AcceptedStatus ? 'success' : 'warning'} 
            size="small"
            style="margin-top: 2px; font-size: 10px;"
          >
            {row.Assignment.AcceptedStatus ? '已接单' : '待接单'}
          </n-tag>
        </div>
      );
    }
  },
  {
    key: 'Address',
    title: '配送地址',
    width: 160,
    render: (row) => (
      <div class="flex flex-col">
        <n-text strong style="font-size: 12px;">
          {row.Address?.RecipientName || '未知收件人'}
        </n-text>
        <n-text depth="3" style="font-size: 10px; word-break: break-all; max-width: 150px;">
          {row.Address?.Address || '-'}
        </n-text>
      </div>
    )
  },
  {
    key: 'CreateAt',
    title: '创建时间',
    width: 120,
    render: (row) => (
      <n-text style="font-size: 11px;">
        {row.CreateAt ? new Date(row.CreateAt).toLocaleString() : '-'}
      </n-text>
    )
  }
];

const columns: DataTableColumns = [
  {
    key: 'index',
    title: '序号',
    align: 'center',
    width: 70,
    render: (row, index) => (
      <n-badge value={index + 1} type="warning" />
    )
  },
  {
    key: 'riderInfo',
    title: '骑手信息',
    width: 220,
    render: (row) => (
      <div class="flex items-center gap-12px">
        <n-avatar
          size="medium"
          style={{
            backgroundColor: row.status === 1 ? '#fa8c16' : row.status === 0 ? '#d9d9d9' : '#faad14'
          }}
        >
          <n-icon>
            <BicycleOutline />
          </n-icon>
        </n-avatar>
        <div class="flex flex-col">
          <n-text strong>{row.name || '未设置姓名'}</n-text>
          <n-text depth="3" style="font-size: 12px;">
            ID: {row.riderId?.slice(-8) || '-'}
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
        <n-icon color="#1890ff" size="14">
          <CallOutline />
        </n-icon>
        <n-text style="font-size: 12px;">{row.phoneNumber || '未提供'}</n-text>
      </div>
    )
  },
  {
    key: 'vehicleInfo',
    title: '车辆信息',
    align: 'center',
    width: 140,
    render: (row) => (
      <div class="flex items-center justify-center gap-4px">
        <n-icon color={row.vehicleNumber ? '#52c41a' : '#d9d9d9'} size="14">
          <CarOutline />
        </n-icon>
        <n-text style="font-size: 12px;">{row.vehicleNumber || '未登记'}</n-text>
      </div>
    )
  },
  {
    key: 'userAccount',
    title: '关联账户',
    align: 'center',
    width: 140,
    render: (row) => (
      <div class="flex items-center justify-center gap-4px">
        <n-icon color="#722ed1" size="14">
          <PersonOutline />
        </n-icon>
        <n-text style="font-size: 12px; word-break: break-all;" depth="3">
          {row.applicationUserId?.slice(-8) || '未关联'}
        </n-text>
      </div>
    )
  },
  {
    key: 'actions',
    title: '操作',
    align: 'center',
    width: 140,
    render: (row) => (
      <n-button 
        type="primary" 
        size="small" 
        onClick={() => handleViewDetail(row.riderId)}
        style="border-radius: 6px; background: linear-gradient(135deg, #fa8c16, #faad14);"
      >
        <div style="display: flex; align-items: center; gap: 4px;">
          <n-icon size="14">
            <TrophyOutline />
          </n-icon>
          查看绩效
        </div>
      </n-button>
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

// 订单状态映射（根据新的OrderStatus枚举）
function getOrderStatusText(status: number) {
  const statusMap = {
    0: '未支付',
    1: '已支付', 
    2: '用户确认收货',
    3: '已经评价',
    4: '售后中',
    5: '售后结束',
    6: '订单关闭',
    7: '已派单',
    8: '配送中'
  };
  return statusMap[status] || '未知状态';
}

// 订单状态颜色
function getOrderStatusType(status: number) {
  const typeMap = {
    0: 'error',    // 未支付 - 红色
    1: 'warning',  // 已支付 - 黄色
    2: 'success',  // 用户确认收货 - 绿色
    3: 'success',  // 已经评价 - 绿色
    4: 'warning',  // 售后中 - 橙色
    5: 'success',  // 售后结束 - 绿色
    6: 'error',    // 订单关闭 - 红色
    7: 'info',     // 已派单 - 蓝色
    8: 'warning'   // 配送中 - 橙色
  };
  return typeMap[status] || 'default';
}

// 获取派单相关订单数据（只显示状态7和8的订单）
async function getAllOrdersData() {
  try {
    ordersLoading.value = true;
    
    console.log('🚀 开始获取派单相关订单数据');
    const response = await getAllOrdersWithDetails();
    console.log('📥 收到订单数据响应:', response);
    
    if (response) {
      // API可能直接返回数组，也可能返回包含data属性的对象
      const orders = Array.isArray(response) ? response : response.data || [];

      if (Array.isArray(orders)) {
        // 保存总订单数
        totalOrdersCount.value = orders.length;

        // 只显示状态为7（已派单）和8（配送中）的订单
        const filteredOrders = orders.filter(order => {
          const status = order.OrderStatus || order.orderStatus; // 兼容大小写
          return status === 7 || status === 8;
        });

        ordersData.value = filteredOrders.map((order, index) => {
          // --- Deep Data Normalization ---
          const user = order.User || order.user || {};
          const merchant = order.Merchant || order.merchant || {};
          const address = order.Address || order.address || {};
          const assignment = order.Assignment || order.assignment; // Can be null/undefined

          const normalizedOrder = {
            // Keep other properties from original order
            ...order,

            // Normalize root level properties
            OrderId: order.OrderId || order.orderId,
            OrderStatus: order.OrderStatus || order.orderStatus,
            CreateAt: order.CreateAt || order.createAt,
            
            // Re-construct nested objects with consistent (PascalCase) keys
            User: {
              Nickname: user.Nickname || user.nickname,
              UserId: user.UserId || user.userId
            },
            Merchant: {
              MerchantName: merchant.MerchantName || merchant.merchantName,
              Location: merchant.Location || merchant.location
            },
            Address: {
              RecipientName: address.RecipientName || address.recipientName,
              Address: address.Address || address.address
            },
            // Only normalize assignment if it exists
            Assignment: assignment ? {
              RiderName: assignment.RiderName || assignment.riderName,
              RiderPhoneNumber: assignment.RiderPhoneNumber || assignment.riderPhoneNumber,
              AcceptedStatus: assignment.AcceptedStatus || assignment.acceptedStatus
            } : null,
          };

          return {
            ...normalizedOrder,
            index: index + 1,
            statusText: getOrderStatusText(normalizedOrder.OrderStatus),
            statusType: getOrderStatusType(normalizedOrder.OrderStatus)
          };
        });

        message.success(`成功获取 ${filteredOrders.length} 条派单订单（总订单数：${totalOrdersCount.value}）`);
      } else {
        message.error('获取到的订单数据格式不正确');
        ordersData.value = [];
        totalOrdersCount.value = 0;
      }
    } else {
      message.error('获取订单数据失败');
      ordersData.value = [];
      totalOrdersCount.value = 0;
    }
  } catch (error) {
    message.error('获取订单数据失败: ' + error.message);
    ordersData.value = [];
    totalOrdersCount.value = 0;
    console.error('Error fetching orders data:', error);
  } finally {
    ordersLoading.value = false;
  }
}

// 显示订单排单列表
function showOrdersAssignment() {
  showOrdersModal.value = true;
  getAllOrdersData();
}



// 格式化时间显示
function formatTime(timeStr: string) {
  if (!timeStr) return '未知时间';
  
  try {
    const date = new Date(timeStr);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMins = Math.floor(diffMs / (1000 * 60));
    
    if (diffMins < 1) return '刚刚';
    if (diffMins < 60) return `${diffMins}分钟前`;
    if (diffMins < 1440) return `${Math.floor(diffMins / 60)}小时前`;
    
    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString().slice(0, 5);
  } catch {
    return '时间格式错误';
  }
}

// 组件挂载时获取数据
onMounted(() => {
  getRidersList();
});
</script>

<template>
  <div class="p-6 min-h-full bg-gradient-to-br from-orange-50 to-yellow-50">
    <!-- 页面标题 -->
    <div class="mb-6">
      <h1 class="text-2xl font-bold text-gray-800 mb-2 flex items-center gap-2">
        <n-icon size="24" color="#fa8c16">
          <BicycleOutline />
        </n-icon>
        骑手管理中心
      </h1>
      <p class="text-gray-600">管理骑手信息和绩效排名</p>
    </div>

    <!-- 统计卡片区域 -->
    <n-grid :cols="2" :x-gap="16" :y-gap="16" class="mb-6">
      <n-gi>
        <n-card :bordered="false" class="shadow-sm hover:shadow-lg transition-shadow duration-300">
          <n-statistic
            label="总骑手数"
            :value="riderStats.total"
            value-style="color: #fa8c16; font-weight: bold;"
          >
            <template #prefix>
              <n-icon size="20" color="#fa8c16">
                <BicycleOutline />
              </n-icon>
            </template>
          </n-statistic>
        </n-card>
      </n-gi>
      <n-gi>
        <n-card :bordered="false" class="shadow-sm hover:shadow-lg transition-shadow duration-300">
          <n-statistic
            label="在线骑手数"
            :value="riderStats.onlineCount"
            value-style="color: #fa8c16; font-weight: bold;"
          >
            <template #prefix>
              <n-icon size="20" color="#fa8c16">
                <FlashOutline />
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
          <n-button size="small" @click="showOrdersAssignment" type="warning">
            <template #icon>
              <n-icon>
                <StatsChartOutline />
              </n-icon>
            </template>
            订单派送情况
          </n-button>
        </n-space>
      </template>
      
      <n-form :model="searchParams" inline label-placement="left" label-width="80">
        <n-form-item label="搜索条件">
          <n-input
            v-model:value="searchParams.searchTerm"
            placeholder="请输入骑手姓名或手机号"
            clearable
            style="width: 250px"
          />
        </n-form-item>
      </n-form>
    </n-card>

    <!-- 骑手列表 -->
    <n-card :bordered="false" class="shadow-sm">
      <template #header>
        <div class="flex items-center gap-2">
          <n-icon size="18" color="#fa8c16">
            <BicycleOutline />
          </n-icon>
          <span class="font-medium">骑手列表</span>
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
        :row-class-name="() => 'hover:bg-orange-50 transition-colors duration-200'"
      />
    </n-card>
    
    <!-- 骑手详情弹窗 -->
    <n-modal 
      v-model:show="showDetailModal" 
      preset="card" 
      style="width: 900px; max-height: 80vh;" 
      class="rounded-2xl"
      :mask-closable="false"
    >
      <template #header>
        <div class="flex items-center gap-3">
          <n-avatar 
            :size="40"
            style="background: linear-gradient(135deg, #fa8c16, #faad14)"
          >
            <n-icon size="20">
              <BicycleOutline />
            </n-icon>
          </n-avatar>
          <div>
            <h3 class="text-lg font-semibold text-gray-800">
              {{ riderDetail.name || riderDetail.Name || '骑手绩效信息' }}
            </h3>
            <p class="text-sm text-gray-500">
              ID: {{ riderDetail.riderId || riderDetail.RiderId || '-' }}
            </p>
          </div>
        </div>
      </template>

      <div v-if="!detailLoading" class="space-y-6">
        <!-- 绩效概览 -->
        <div class="bg-gradient-to-r from-orange-50 to-yellow-50 p-4 rounded-lg">
          <h4 class="text-gray-800 font-medium mb-4 flex items-center gap-2">
            <n-icon color="#fa8c16">
              <TrophyOutline />
            </n-icon>
            {{ currentYear }}年{{ currentMonth }}月绩效概览
          </h4>
          
          <n-grid :cols="4" :x-gap="20" v-if="Object.keys(riderRanking).length > 0">
            <n-gi v-for="(value, key) in riderRanking" :key="key">
              <div class="text-center">
                <div class="text-2xl font-bold" :class="{
                  'text-yellow-600': getRankingBadgeType(value) === 'success',
                  'text-orange-600': getRankingBadgeType(value) === 'warning',
                  'text-blue-600': getRankingBadgeType(value) === 'info'
                }">
                  {{ value }}
                </div>
                <div class="text-sm text-gray-600">{{ formatRankingKey(key) }}</div>
              </div>
            </n-gi>
          </n-grid>
          
          <div v-else class="text-center py-8">
            <n-icon size="48" color="#d9d9d9">
              <StatsChartOutline />
            </n-icon>
            <p class="text-gray-500 mt-2">暂无当月绩效数据</p>
          </div>
        </div>

        <n-divider />

        <!-- 详细信息 -->
        <n-grid :cols="2" :x-gap="24" :y-gap="20">
          <!-- 基本信息 -->
          <n-gi>
            <n-card title="基本信息" size="small" class="h-full">
              <template #header-extra>
                <n-icon color="#fa8c16">
                  <PersonOutline />
                </n-icon>
              </template>
              
              <div class="space-y-4">
                <div class="flex justify-between items-center py-2 border-b border-gray-100">
                  <span class="text-gray-600 font-medium">骑手ID</span>
                  <n-text code style="word-break: break-all; max-width: 200px;">
                    {{ riderDetail.riderId || riderDetail.RiderId || '-' }}
                  </n-text>
                </div>
                
                <div class="flex justify-between items-center py-2 border-b border-gray-100">
                  <span class="text-gray-600 font-medium">姓名</span>
                  <n-text strong>{{ riderDetail.name || riderDetail.Name || '未设置' }}</n-text>
                </div>
                
                <div class="flex justify-between items-center py-2 border-b border-gray-100">
                  <span class="text-gray-600 font-medium flex items-center gap-2">
                    <n-icon size="16" color="#1890ff">
                      <CallOutline />
                    </n-icon>
                    手机号
                  </span>
                  <n-text>{{ riderDetail.phoneNumber || riderDetail.PhoneNumber || '未设置' }}</n-text>
                </div>
                
                <div class="flex justify-between items-center py-2">
                  <span class="text-gray-600 font-medium flex items-center gap-2">
                    <n-icon size="16" color="#52c41a">
                      <CarOutline />
                    </n-icon>
                    车牌号
                  </span>
                  <n-text>{{ riderDetail.vehicleNumber || riderDetail.VehicleNumber || '未登记' }}</n-text>
                </div>
              </div>
            </n-card>
          </n-gi>
          
          <!-- 账户信息 -->
          <n-gi>
            <n-card title="账户关联" size="small" class="h-full">
              <template #header-extra>
                <n-icon color="#722ed1">
                  <PersonOutline />
                </n-icon>
              </template>
              
              <div class="space-y-4">
                <div class="flex justify-between items-start py-2 border-b border-gray-100">
                  <span class="text-gray-600 font-medium">关联用户ID</span>
                  <n-text style="word-break: break-all; max-width: 200px;">
                    {{ riderDetail.applicationUserId || riderDetail.ApplicationUserId || '未关联' }}
                  </n-text>
                </div>
                
                <div class="flex justify-between items-center py-2 border-b border-gray-100">
                  <span class="text-gray-600 font-medium">账户状态</span>
                  <n-badge 
                    :value="riderDetail.applicationUserId ? '已关联' : '未关联'" 
                    :type="riderDetail.applicationUserId ? 'success' : 'warning'"
                  />
                </div>
                
                <div class="flex justify-between items-center py-2">
                  <span class="text-gray-600 font-medium">车辆状态</span>
                  <n-badge 
                    :value="riderDetail.vehicleNumber ? '已登记' : '未登记'" 
                    :type="riderDetail.vehicleNumber ? 'success' : 'warning'"
                  />
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
              <p class="text-gray-600">正在加载骑手信息...</p>
              <p class="text-sm text-gray-400 mt-1">请稍候片刻</p>
            </div>
          </template>
        </n-spin>
      </div>
    </n-modal>

    <!-- 订单排单展示弹窗 -->
    <n-modal 
      v-model:show="showOrdersModal" 
      preset="card" 
      style="width: 1200px; max-height: 85vh;" 
      class="rounded-2xl"
      :mask-closable="false"
    >
      <template #header>
        <div class="flex items-center gap-3">
          <n-avatar 
            :size="40"
            style="background: linear-gradient(135deg, #fa8c16, #faad14)"
          >
            <n-icon size="20">
              <StatsChartOutline />
            </n-icon>
          </n-avatar>
          <div>
            <h3 class="text-lg font-semibold text-gray-800">
              派单状态展示
            </h3>
            <p class="text-sm text-gray-500">
              实时查看已派单和配送中的订单
            </p>
          </div>
        </div>
      </template>

      <div v-if="!ordersLoading" class="space-y-4">
        <!-- 订单统计概览 -->
        <div class="bg-gradient-to-r from-orange-50 to-yellow-50 p-4 rounded-lg">
          <h4 class="text-gray-800 font-medium mb-4 flex items-center gap-2">
            <n-icon color="#fa8c16">
              <StatsChartOutline />
            </n-icon>
            派单状态统计
          </h4>
          
          <n-grid :cols="3" :x-gap="16">
            <!-- 总订单数 -->
            <n-gi>
              <div class="text-center p-4 bg-white rounded-lg shadow-sm">
                <div class="text-2xl font-bold text-purple-600">
                  {{ totalOrdersCount }}
                </div>
                <div class="text-sm text-gray-600 mt-2">总订单数</div>
              </div>
            </n-gi>
            
            <!-- 派单状态统计 -->
            <n-gi v-for="status in [7, 8]" :key="status">
              <div class="text-center p-4 bg-white rounded-lg shadow-sm">
                <div class="text-2xl font-bold" :class="{
                  'text-blue-600': status === 7,
                  'text-orange-600': status === 8
                }">
                  {{ ordersData.filter(order => order.OrderStatus === status).length }}
                </div>
                <div class="text-sm text-gray-600 mt-2">{{ getOrderStatusText(status) }}</div>
              </div>
            </n-gi>
          </n-grid>
        </div>

        <!-- 订单列表 -->
        <n-data-table
          :columns="ordersColumns"
          :data="ordersData"
          :loading="ordersLoading"
          :pagination="{ pageSize: 15, showSizePicker: true, pageSizes: [10, 15, 20, 30] }"
          flex-height
          style="min-height: 400px;"
          :row-class-name="() => 'hover:bg-orange-50 transition-colors duration-200'"
        />
      </div>
      
      <!-- 加载状态 -->
      <div v-else class="flex justify-center items-center h-80">
        <n-spin size="large">
          <template #description>
            <div class="text-center mt-4">
              <p class="text-gray-600">正在加载订单数据...</p>
              <p class="text-sm text-gray-400 mt-1">请稍候片刻</p>
            </div>
          </template>
        </n-spin>
      </div>
    </n-modal>
  </div>
</template>

<style scoped>
/* 骑手管理页面样式 */
.rider-manage-container {
  background: linear-gradient(135deg, #fff7e6 0%, #fff2e8 100%);
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
  background-color: rgba(250, 140, 22, 0.05) !important;
}

/* 骑手状态指示器 */
.rider-status {
  position: relative;
}

.rider-status::before {
  content: '';
  position: absolute;
  top: 50%;
  left: -8px;
  width: 6px;
  height: 6px;
  border-radius: 50%;
  transform: translateY(-50%);
}

.status-online::before {
  background-color: #fa8c16;
  box-shadow: 0 0 0 2px rgba(250, 140, 22, 0.2);
  animation: pulse-orange 2s infinite;
}

.status-offline::before {
  background-color: #d9d9d9;
  box-shadow: 0 0 0 2px rgba(217, 217, 217, 0.2);
}

.status-busy::before {
  background-color: #ff4d4f;
  box-shadow: 0 0 0 2px rgba(255, 77, 79, 0.2);
  animation: pulse-red 2s infinite;
}

/* 骑手头像样式 */
.rider-avatar {
  background: linear-gradient(135deg, #fa8c16 0%, #faad14 100%);
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

/* 渐变按钮特殊样式 */
.gradient-button {
  background: linear-gradient(135deg, #fa8c16, #faad14);
  border: none;
  color: white;
}

.gradient-button:hover {
  background: linear-gradient(135deg, #e6780e, #e89f13);
  transform: translateY(-1px);
  box-shadow: 0 4px 8px rgba(250, 140, 22, 0.3);
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
  background-color: #fff7e6;
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
  color: #fa8c16;
}

/* 绩效排名样式 */
.performance-rank {
  transition: all 0.3s ease;
}

.performance-rank:hover {
  background-color: rgba(250, 140, 22, 0.1);
  transform: scale(1.02);
}

/* 排名徽章特殊样式 */
.rank-badge-gold {
  background: linear-gradient(135deg, #ffd700, #ffed4a);
  color: #8b5a00;
}

.rank-badge-silver {
  background: linear-gradient(135deg, #c0c0c0, #e5e5e5);
  color: #666;
}

.rank-badge-bronze {
  background: linear-gradient(135deg, #cd7f32, #daa520);
  color: #654321;
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

/* 动画效果 */
@keyframes pulse-orange {
  0% {
    box-shadow: 0 0 0 0 rgba(250, 140, 22, 0.4);
  }
  70% {
    box-shadow: 0 0 0 4px rgba(250, 140, 22, 0);
  }
  100% {
    box-shadow: 0 0 0 0 rgba(250, 140, 22, 0);
  }
}

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



/* 绩效卡片特效 */
.performance-card {
  position: relative;
  overflow: hidden;
  background: linear-gradient(135deg, #fff7e6, #fff2e8);
}

.performance-card::before {
  content: '';
  position: absolute;
  top: -50%;
  left: -50%;
  width: 200%;
  height: 200%;
  background: conic-gradient(from 0deg, transparent, rgba(250, 140, 22, 0.1), transparent);
  animation: rotate 4s linear infinite;
  pointer-events: none;
}

@keyframes rotate {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}
</style> 