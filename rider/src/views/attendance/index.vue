<script setup lang="ts">
import { computed, h, onMounted, ref } from 'vue';
import {
  NButton,
  NCard,
  NDataTable,
  NDatePicker,
  NDescriptions,
  NDescriptionsItem,
  NEmpty,
  NGi,
  NGrid,
  NModal,
  NSelect,
  NSpace,
  NSpin,
  NStatistic,
  NTag
} from 'naive-ui';
import { Icon } from '@iconify/vue';
import { getRiderInfo } from '@/service/api/rider';
import {
  createAttendanceRecord,
  getAttendanceDetail,
  getRiderAttendanceRecordsList,
  getRiderAttendanceByDateRange,
  getRiderAttendanceStats,
  getTodayAttendance,
  riderCheckInAttendance,
  riderCheckOutAttendance
} from '@/service/api/attendance';
import { useAuthStore } from '@/store/modules/auth';
import { useRiderStore } from '@/store/modules/rider';

const authStore = useAuthStore();
const riderStore = useRiderStore();

// 骑手ID - 使用统一的 riderStore 获取
const riderId = computed(() => riderStore.riderId || authStore.userInfo.userId);

// 响应式数据
const attendanceList = ref<Api.Attendance.AttendanceRecord[]>([]);
const riderInfo = ref<Api.Rider.RiderInfoData | null>(null);
const todayRecord = ref<Api.Attendance.AttendanceRecord | null>(null);
const attendanceStats = ref<Api.Attendance.RiderStatsData | null>(null);
const loading = ref(false);
const showDetailModal = ref(false);
const selectedRecord = ref<Api.Attendance.AttendanceRecord | null>(null);

// 考勤状态
const isCheckedIn = ref(false);
const isCheckedOut = ref(false);
const currentTime = ref(new Date());

// 防抖和重试机制
const checkInRetryCount = ref(0);
const checkOutRetryCount = ref(0);
const maxRetryCount = 3;

// 筛选条件
const filterForm = ref({
  startDate: null as number | null,
  endDate: null as number | null,
  status: undefined as 'present' | 'late' | 'absent' | undefined
});

// 分页
const pagination = ref({
  page: 1,
  pageSize: 15,
  itemCount: 0
});

// 实时更新时间
const updateTime = () => {
  currentTime.value = new Date();
};

// 获取骑手信息
async function fetchRiderInfo() {
  try {
    console.log('开始获取骑手信息, riderId:', riderId.value);
    const { data } = await getRiderInfo(riderId.value);
    console.log('骑手信息获取成功:', data);
    if (data) {
      // 兼容两种数据格式：标准格式(data.data)和简单格式(data)
      const infoData = (data as any).data || data;
      riderInfo.value = infoData;
    }
  } catch (error: any) {
    console.error('获取骑手信息失败:', error);
    console.error('错误详情:', {
      message: error?.message,
      response: error?.response?.data,
      status: error?.response?.status
    });
    // 获取骑手信息失败，使用默认信息
    riderInfo.value = {
      riderId: riderId.value,
      name: '骑手',
      phoneNumber: '',
      vehicleNumber: '',
      applicationUserId: riderId.value
    };
  }
}

// 获取今日考勤状态
async function fetchTodayAttendance() {
  try {
    const { data } = await getTodayAttendance(riderId.value);

    if (data) {
      // 兼容两种数据格式：标准格式(data.data)和简单格式(data)
      const recordData = data.data || data;
      todayRecord.value = recordData;
      isCheckedIn.value = Boolean(recordData.checkInAt);
      isCheckedOut.value = Boolean(recordData.checkoutAt);
    } else {
      todayRecord.value = null;
      isCheckedIn.value = false;
      isCheckedOut.value = false;
    }
  } catch (error) {
    console.error('获取今日考勤状态失败:', error);
    // 获取今日考勤状态失败，重置状态
    todayRecord.value = null;
    isCheckedIn.value = false;
    isCheckedOut.value = false;
  }
}

// 获取考勤记录列表
async function fetchAttendanceRecords() {
  loading.value = true;
  try {
    console.log('开始获取考勤记录, riderId:', riderId.value);
    
    // 构建请求参数
    const params: Api.Attendance.GetRiderAttendanceRequest = {
      pageIndex: pagination.value.page,
      pageSize: pagination.value.pageSize
    };

    // 处理日期筛选
    if (filterForm.value.startDate && filterForm.value.startDate !== null) {
      // NDatePicker 返回的是时间戳（毫秒），直接转换为日期字符串
      const startDate = new Date(filterForm.value.startDate);
      params.startDate = startDate.toISOString().split('T')[0];
    }
    if (filterForm.value.endDate && filterForm.value.endDate !== null) {
      // NDatePicker 返回的是时间戳（毫秒），直接转换为日期字符串
      const endDate = new Date(filterForm.value.endDate);
      params.endDate = endDate.toISOString().split('T')[0];
    }

    // 处理状态筛选
    if (filterForm.value.status === 'late') {
      params.isLate = 1;
    } else if (filterForm.value.status === 'absent') {
      params.isAbsent = 1;
    } else if (filterForm.value.status === 'present') {
      // 正常出勤：既不是迟到也不是缺勤
      params.isLate = 0;
      params.isAbsent = 0;
    }

    console.log('考勤记录请求参数:', params);
    console.log('筛选条件:', {
      startDate: filterForm.value.startDate,
      endDate: filterForm.value.endDate,
      status: filterForm.value.status,
      processedStartDate: params.startDate,
      processedEndDate: params.endDate
    });
    
    // 只有当开始日期和结束日期都有值且不为空字符串时才使用日期范围查询API
    let data;
    if (params.startDate && params.endDate && params.startDate.trim() !== '' && params.endDate.trim() !== '') {
      console.log('使用日期范围查询API');
      const dateRangeParams = {
        startDate: params.startDate,
        endDate: params.endDate,
        pageIndex: params.pageIndex,
        pageSize: params.pageSize,
        // 添加状态筛选参数
        ...(params.isLate !== undefined && { isLate: params.isLate }),
        ...(params.isAbsent !== undefined && { isAbsent: params.isAbsent })
      };
      console.log('日期范围查询参数:', dateRangeParams);
      const response = await getRiderAttendanceByDateRange(riderId.value, dateRangeParams);
      data = response.data;
    } else {
      console.log('使用普通查询API');
      const response = await getRiderAttendanceRecordsList(riderId.value, params);
      data = response.data;
    }
    console.log('考勤记录API响应:', data);
    console.log('API响应类型:', typeof data);
    console.log('API响应是否为数组:', Array.isArray(data));
    if (data && typeof data === 'object') {
      console.log('API响应键:', Object.keys(data));
      if (data.data) {
        console.log('data.data类型:', typeof data.data);
        console.log('data.data是否为数组:', Array.isArray(data.data));
        console.log('data.data内容:', data.data);
      }
      if (data && typeof data === 'object' && 'records' in data) {
        console.log('data.records类型:', typeof (data as any).records);
        console.log('data.records是否为数组:', Array.isArray((data as any).records));
        console.log('data.records内容:', (data as any).records);
      }
    }

    if (data) {
      // 兼容两种数据格式：标准格式(data.data)和简单格式(data)
      const responseData = data.data || data;
      
      // 根据API类型定义，处理分页响应格式
      if (Array.isArray(responseData)) {
        // 如果返回的是数组，说明没有分页信息，使用数组长度作为总数
        attendanceList.value = responseData;
        pagination.value.itemCount = responseData.length;
      } else if (responseData && typeof responseData === 'object' && 'records' in responseData) {
        // 如果返回的是分页对象，使用分页信息
        attendanceList.value = (responseData as any).records || [];
        pagination.value.itemCount = (responseData as any).total || 0;
      } else {
        // 其他情况，设置为空
        attendanceList.value = [];
        pagination.value.itemCount = 0;
      }
    } else {
      attendanceList.value = [];
      pagination.value.itemCount = 0;
    }
  } catch (error: any) {
    console.error('获取考勤记录失败:', error);
    console.error('错误详情:', {
      message: error?.message,
      response: error?.response?.data,
      status: error?.response?.status
    });
    window.$message?.error('获取考勤记录失败');
    attendanceList.value = [];
    pagination.value.itemCount = 0;
  } finally {
    loading.value = false;
  }
}

// 获取考勤统计
async function fetchAttendanceStats() {
  try {
    console.log('开始获取考勤统计, riderId:', riderId.value);
    const currentDate = new Date();
    const firstDayOfMonth = new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);
    const lastDayOfMonth = new Date(currentDate.getFullYear(), currentDate.getMonth() + 1, 0);

    const params: Api.Attendance.GetRiderStatsRequest = {
      startDate: firstDayOfMonth.toISOString().split('T')[0],
      endDate: lastDayOfMonth.toISOString().split('T')[0]
    };

    console.log('考勤统计请求参数:', params);
    const { data } = await getRiderAttendanceStats(riderId.value, params);
    console.log('考勤统计获取成功:', data);
    
    if (data) {
      // 兼容两种数据格式：标准格式(data.data)和简单格式(data)
      const statsData = data.data || data;
      attendanceStats.value = statsData;
    } else {
      // 如果没有数据，设置默认值
      attendanceStats.value = {
        totalWorkDays: 0,
        totalWorkHours: 0,
        averageWorkHours: 0,
        lateCount: 0,
        absentCount: 0,
        attendanceRate: 0
      };
    }
  } catch (error: any) {
    console.error('获取考勤统计失败:', error);
    console.error('错误详情:', {
      message: error?.message,
      response: error?.response?.data,
      status: error?.response?.status
    });
    // 获取考勤统计失败，使用默认值
    attendanceStats.value = {
      totalWorkDays: 0,
      totalWorkHours: 0,
      averageWorkHours: 0,
      lateCount: 0,
      absentCount: 0,
      attendanceRate: 0
    };
    // 不显示错误消息，因为统计数据不是关键功能
  }
}

// 带重试机制的打卡函数
async function handleCheckInWithRetry() {
  // 防止重复点击 - 如果已经打卡但未签退，则不允许再次打卡
  if (loading.value || isCheckedIn.value) {
    if (isCheckedIn.value && !isCheckedOut.value) {
      window.$message?.warning('今日已打卡，请先签退后再打卡');
    }
    console.log('已经防止重复点击', loading.value, isCheckedIn.value);
    return;
  }

  try {
    loading.value = true;
    checkInRetryCount.value = 0;
    
    // 使用签到接口而不是创建考勤记录接口
    const { data } = await riderCheckInAttendance(riderId.value);
    console.log('if', data);
    if (data) {
      window.$message?.success(data.message || '打卡成功！');
      isCheckedIn.value = true;
      checkInRetryCount.value = 0; // 重置重试计数
      // 刷新考勤记录
      await fetchAttendanceRecords();
    } else {
      window.$message?.warning('打卡响应异常，请刷新页面查看状态');
    }
  } catch (error: any) {
    // 处理具体的错误信息
    console.error('打卡失败:', error);
    console.error('请求详情:', {
      riderId: riderId.value,
      url: error?.config?.url,
      method: error?.config?.method,
      data: error?.config?.data,
      headers: error?.config?.headers
    });
    
    if (error?.response?.data?.message) {
      const errorMessage = error.response.data.message;
      // 处理乐观并发错误和重复操作
      if (errorMessage.includes('已有考勤记录') || 
          errorMessage.includes('已签到') || 
          errorMessage.includes('重复') ||
          errorMessage.includes('already') ||
          errorMessage.includes('expected to affect 1 row') ||
          errorMessage.includes('actually affected 0 row') ||
          errorMessage.includes('乐观并发') ||
          errorMessage.includes('concurrency')) {
        // 数据库并发异常，可能是重复操作，刷新页面状态
        window.$message?.warning('检测到重复操作，正在刷新状态...');
        // 刷新考勤记录和状态
        await Promise.all([
          fetchAttendanceRecords(),
          fetchTodayAttendance()
        ]);
        // 检查是否已经打卡成功
        if (todayRecord.value?.checkInAt) {
          isCheckedIn.value = true;
          window.$message?.success('今日已成功打卡！');
        }
      } else {
        window.$message?.error(`打卡失败：${errorMessage}`);
      }
    } else if (error?.response?.status) {
      // 处理HTTP状态码错误
      switch (error.response.status) {
        case 400:
          window.$message?.error('请求参数错误，请检查网络连接');
          break;
        case 401:
          window.$message?.error('登录已过期，请重新登录');
          break;
        case 403:
          window.$message?.error('没有权限进行此操作');
          break;
        case 409:
          // 409 Conflict 通常表示乐观并发冲突，尝试重试
          if (checkInRetryCount.value < maxRetryCount) {
            checkInRetryCount.value++;
            window.$message?.warning(`操作冲突，正在重试 (${checkInRetryCount.value}/${maxRetryCount})...`);
            // 等待一段时间后重试
            await new Promise(resolve => setTimeout(resolve, 1000 * checkInRetryCount.value));
            await handleCheckInWithRetry();
            return;
          } else {
            window.$message?.warning('操作冲突，正在刷新状态...');
            await Promise.all([
              fetchAttendanceRecords(),
              fetchTodayAttendance()
            ]);
          }
          break;
        case 500:
          window.$message?.error('服务器错误，请稍后重试');
          break;
        default:
          window.$message?.error('打卡失败，请稍后重试');
      }
    } else {
      window.$message?.error('网络连接失败，请检查网络后重试');
    }
  } finally {
    loading.value = false;
  }
}

// 打卡
async function handleCheckIn() {
  await handleCheckInWithRetry();
}

// 带重试机制的签退函数
async function handleCheckOutWithRetry() {
  // 防止重复点击
  if (loading.value || isCheckedOut.value || !isCheckedIn.value) {
    return;
  }

  try {
    loading.value = true;
    checkOutRetryCount.value = 0;

    const { data } = await riderCheckOutAttendance(riderId.value);

    if (data) {
      window.$message?.success(data.message || '签退成功！');
      isCheckedOut.value = true;
      checkOutRetryCount.value = 0; // 重置重试计数
      // 刷新考勤记录
      await fetchAttendanceRecords();
    } else {
      window.$message?.warning('签退响应异常，请刷新页面查看状态');
    }
  } catch (error: any) {
    console.error('签退失败:', error);
    console.error('请求详情:', {
      riderId: riderId.value,
      url: error?.config?.url,
      method: error?.config?.method,
      data: error?.config?.data,
      headers: error?.config?.headers
    });
    
    if (error?.response?.data?.message) {
      const errorMessage = error.response.data.message;
      if (errorMessage.includes('未签到') || 
          errorMessage.includes('not checked in') ||
          errorMessage.includes('需要先签到')) {
        window.$message?.error('请先完成签到再进行签退');
      } else if (errorMessage.includes('已签退') || 
                 errorMessage.includes('already checked out') ||
                 errorMessage.includes('expected to affect 1 row') ||
                 errorMessage.includes('actually affected 0 row') ||
                 errorMessage.includes('乐观并发') ||
                 errorMessage.includes('concurrency')) {
        // 数据库并发异常，可能是重复操作，刷新页面状态
        window.$message?.warning('检测到重复操作，正在刷新状态...');
        // 刷新考勤记录和状态
        await Promise.all([
          fetchAttendanceRecords(),
          fetchTodayAttendance()
        ]);
        // 检查是否已经签退成功
        if (todayRecord.value?.checkoutAt) {
          isCheckedOut.value = true;
          window.$message?.success('今日已成功签退！');
        }
      } else {
        window.$message?.error(`签退失败：${errorMessage}`);
      }
    } else if (error?.response?.status) {
      // 处理HTTP状态码错误
      switch (error.response.status) {
        case 400:
          window.$message?.error('请求参数错误，请检查网络连接');
          break;
        case 401:
          window.$message?.error('登录已过期，请重新登录');
          break;
        case 403:
          window.$message?.error('没有权限进行此操作');
          break;
        case 409:
          // 409 Conflict 通常表示乐观并发冲突，尝试重试
          if (checkOutRetryCount.value < maxRetryCount) {
            checkOutRetryCount.value++;
            window.$message?.warning(`操作冲突，正在重试 (${checkOutRetryCount.value}/${maxRetryCount})...`);
            // 等待一段时间后重试
            await new Promise(resolve => setTimeout(resolve, 1000 * checkOutRetryCount.value));
            await handleCheckOutWithRetry();
            return;
          } else {
            window.$message?.warning('操作冲突，正在刷新状态...');
            await Promise.all([
              fetchAttendanceRecords(),
              fetchTodayAttendance()
            ]);
          }
          break;
        case 500:
          window.$message?.error('服务器错误，请稍后重试');
          break;
        default:
          window.$message?.error('签退失败，请稍后重试');
      }
    } else {
      window.$message?.error('网络连接失败，请检查网络后重试');
    }
  } finally {
    loading.value = false;
  }
}

// 签退
async function handleCheckOut() {
  await handleCheckOutWithRetry();
}

// 查看详情
async function showDetail(record: Api.Attendance.AttendanceRecord) {
  try {
    const { data } = await getAttendanceDetail(record.attendanceId);
    if (data && data.data) {
      selectedRecord.value = data.data;
      showDetailModal.value = true;
    }
  } catch (error) {
    console.error('获取考勤详情失败:', error);
    // 获取考勤详情失败
    window.$message?.error('获取考勤详情失败');
  }
}

// 搜索
function handleSearch() {
  console.log('执行搜索，筛选条件:', filterForm.value);
  pagination.value.page = 1;
  fetchAttendanceRecords();
}

// 实时筛选（当筛选条件改变时自动搜索）
function handleFilterChange() {
  console.log('筛选条件改变，自动触发搜索:', filterForm.value);
  pagination.value.page = 1;
  fetchAttendanceRecords();
}

// 清除单个筛选条件
function clearFilter(type: 'startDate' | 'endDate' | 'status') {
  if (type === 'startDate') {
    filterForm.value.startDate = null;
  } else if (type === 'endDate') {
    filterForm.value.endDate = null;
  } else if (type === 'status') {
    filterForm.value.status = undefined;
  }
  handleFilterChange();
}

// 重置筛选
function handleReset() {
  console.log('重置筛选条件');
  filterForm.value = {
    startDate: null,
    endDate: null,
    status: undefined
  };
  pagination.value.page = 1;
  fetchAttendanceRecords();
}

// 分页变化
function handlePageChange(page: number) {
  pagination.value.page = page;
  fetchAttendanceRecords();
}

// 页面大小变化
function handlePageSizeChange(pageSize: number) {
  pagination.value.pageSize = pageSize;
  pagination.value.page = 1; // 重置到第一页
  fetchAttendanceRecords();
}

// 计算统计数据
const statistics = computed(() => {
  if (!attendanceList.value || !Array.isArray(attendanceList.value)) {
    return {
      total: 0,
      present: 0,
      late: 0,
      absent: 0,
      attendanceRate: 0
    };
  }

  const total = attendanceList.value.length;
  const present = attendanceList.value.filter(record => 
    record.isLate === 0 && record.isAbsent === 0
  ).length;
  const late = attendanceList.value.filter(record => record.isLate === 1).length;
  const absent = attendanceList.value.filter(record => record.isAbsent === 1).length;

  console.log('统计数据计算:', {
    total,
    present,
    late,
    absent,
    attendanceRate: total > 0 ? Math.round((present / total) * 100) : 0
  });

  return {
    total,
    present,
    late,
    absent,
    attendanceRate: total > 0 ? Math.round((present / total) * 100) : 0
  };
});

// 考勤状态映射
const statusMap = {
  present: { text: '正常', type: 'success' },
  absent: { text: '缺勤', type: 'error' },
  late: { text: '迟到', type: 'warning' }
};

// 计算今日考勤状态
const todayAttendanceStatus = computed(() => {
  if (!todayRecord.value) {
    return {
      canCheckIn: true,
      canCheckOut: false,
      statusText: '未打卡',
      statusType: 'default'
    };
  }

  const hasCheckedIn = Boolean(todayRecord.value.checkInAt);
  const hasCheckedOut = Boolean(todayRecord.value.checkoutAt);

  if (hasCheckedIn && hasCheckedOut) {
    return {
      canCheckIn: false,
      canCheckOut: false,
      statusText: '已完成',
      statusType: 'success'
    };
  } else if (hasCheckedIn && !hasCheckedOut) {
    return {
      canCheckIn: false,
      canCheckOut: true,
      statusText: '已打卡(未签退)',
      statusType: 'warning'
    };
  } else {
    return {
      canCheckIn: true,
      canCheckOut: false,
      statusText: '未打卡',
      statusType: 'default'
    };
  }
});

// 获取考勤状态
function getAttendanceStatus(record: Api.Attendance.AttendanceRecord) {
  if (record.isAbsent === 1) return 'absent';
  if (record.isLate === 1) return 'late';
  return 'present';
}

// 计算工作时长
function calculateWorkHours(record: Api.Attendance.AttendanceRecord) {
  if (!record.checkInAt || !record.checkoutAt) return 0;

  const start = new Date(record.checkInAt);
  const end = new Date(record.checkoutAt);
  const hours = Math.round(((end.getTime() - start.getTime()) / (1000 * 60 * 60)) * 10) / 10;

  return hours;
}

// 表格列配置
const columns = [
  {
    title: '日期',
    key: 'checkDate',
    width: 120,
    render: (row: Api.Attendance.AttendanceRecord) => {
      const date = new Date(row.checkDate);
      const today = new Date();
      const isToday = date.toDateString() === today.toDateString();

      return h('div', { class: 'flex items-center' }, [
        h(
          'span',
          { class: isToday ? 'text-blue-600 font-medium' : '' },
          date.toLocaleDateString('zh-CN', { month: '2-digit', day: '2-digit' })
        ),
        isToday && h(NTag, { type: 'primary', size: 'tiny' }, { default: () => '今日' })
      ]);
    }
  },
  {
    title: '上班时间',
    key: 'checkInAt',
    width: 100,
    render: (row: Api.Attendance.AttendanceRecord) => {
      if (!row.checkInAt) return h('span', { class: 'text-gray-400' }, '未打卡');
      const time = new Date(row.checkInAt).toLocaleTimeString('zh-CN', {
        hour: '2-digit',
        minute: '2-digit'
      });
      return h('span', { class: 'font-medium' }, time);
    }
  },
  {
    title: '下班时间',
    key: 'checkoutAt',
    width: 100,
    render: (row: Api.Attendance.AttendanceRecord) => {
      if (!row.checkoutAt) return h('span', { class: 'text-gray-400' }, '未签退');
      const time = new Date(row.checkoutAt).toLocaleTimeString('zh-CN', {
        hour: '2-digit',
        minute: '2-digit'
      });
      return h('span', { class: 'font-medium' }, time);
    }
  },
  {
    title: '工作时长',
    key: 'workHours',
    width: 100,
    render: (row: Api.Attendance.AttendanceRecord) => {
      const hours = calculateWorkHours(row);
      if (hours === 0) return '-';
      return h('span', { class: 'text-green-600 font-medium' }, `${hours}h`);
    }
  },
  {
    title: '状态',
    key: 'status',
    width: 100,
    render: (row: Api.Attendance.AttendanceRecord) => {
      const status = getAttendanceStatus(row);
      const statusInfo = statusMap[status];
      return h(
        NTag,
        {
          type: statusInfo.type as any,
          size: 'small'
        },
        { default: () => statusInfo.text }
      );
    }
  },
  {
    title: '操作',
    key: 'actions',
    width: 80,
    render: (row: Api.Attendance.AttendanceRecord) => {
      return h(
        NButton,
        {
          size: 'small',
          type: 'primary',
          ghost: true,
          onClick: () => showDetail(row)
        },
        { default: () => '详情' }
      );
    }
  }
];

// 状态选项
const statusOptions = [
  { label: '全部状态', value: undefined },
  { label: '正常', value: 'present' },
  { label: '迟到', value: 'late' },
  { label: '缺勤', value: 'absent' }
];

// 检查是否有活跃的筛选条件
const hasActiveFilters = computed(() => {
  return !!(filterForm.value.startDate || filterForm.value.endDate || filterForm.value.status);
});

// 页面加载
onMounted(async () => {
  console.log('考勤页面开始初始化...');
  console.log('当前认证状态:', {
    token: authStore.token,
    userId: authStore.userInfo.userId,
    userName: authStore.userInfo.userName
  });

  // 检查用户信息是否已初始化
  if (!authStore.userInfo.userId && authStore.token) {
    console.log('用户信息未初始化，开始初始化...');
    await authStore.initUserInfo();
  }

  // 如果仍然没有用户信息，使用模拟数据
  if (!authStore.userInfo.userId) {
    console.log('使用模拟用户数据');
    Object.assign(authStore.userInfo, {
      userId: `rider_${Date.now()}`,
      userName: '测试骑手',
      roles: ['rider'],
      buttons: []
    });
  }

  console.log('初始化后的用户信息:', authStore.userInfo);
  console.log('骑手ID:', riderId.value);

  try {
    console.log('开始并行获取数据...');
    await Promise.all([fetchRiderInfo(), fetchTodayAttendance(), fetchAttendanceRecords(), fetchAttendanceStats()]);
    console.log('所有数据获取完成');
  } catch (error) {
    console.error('数据获取过程中出现错误:', error);
  }

  // 启动实时时钟
  setInterval(updateTime, 1000);
});
</script>

<template>
  <div class="min-h-full bg-gradient-to-br from-blue-50 to-indigo-100 p-24px dark:from-gray-900 dark:to-gray-800">
    <!-- 页面标题区域 -->
    <NCard :bordered="false" class="mb-24px bg-white/80 backdrop-blur-sm dark:bg-gray-800/80">
      <div class="flex items-center gap-3">
        <div class="flex h-12 w-12 items-center justify-center rounded-xl bg-gradient-to-r from-blue-500 to-purple-600">
          <Icon icon="mdi:clock-check" class="text-2xl text-white" />
        </div>
        <div>
          <h1 class="text-2xl font-bold text-gray-800 dark:text-gray-200">考勤管理</h1>
          <p class="mt-2px text-gray-600 dark:text-gray-400">管理您的出勤记录，保持工作纪律</p>
        </div>
      </div>
    </NCard>

    <!-- 今日考勤卡片 -->
    <NCard :bordered="false" class="mb-24px rounded-16px bg-white/90 shadow-lg backdrop-blur-sm dark:bg-gray-800/90">
      <div class="flex items-center justify-between">
        <div class="flex items-center space-x-32px">
          <!-- 当前时间 -->
          <div class="text-center">
            <div class="text-4xl font-bold text-blue-600 dark:text-blue-400">
              {{ currentTime.toLocaleTimeString('zh-CN', { hour: '2-digit', minute: '2-digit', second: '2-digit' }) }}
            </div>
            <div class="mt-4px text-sm text-gray-500">
              {{
                currentTime.toLocaleDateString('zh-CN', {
                  year: 'numeric',
                  month: 'long',
                  day: 'numeric',
                  weekday: 'long'
                })
              }}
            </div>
          </div>

          <!-- 骑手信息 -->
          <div class="text-center">
            <div class="text-lg font-semibold text-gray-800 dark:text-gray-200">
              {{ authStore.userInfo.userName || riderInfo?.name || '骑手' }}
            </div>
            <div class="text-sm text-gray-500">车辆：{{ riderInfo?.vehicleNumber || '暂无' }}</div>
          </div>

          <!-- 今日状态 -->
          <div class="text-center">
            <div class="text-lg font-semibold text-gray-800 dark:text-gray-200">
              {{ todayAttendanceStatus.statusText }}
            </div>
            <div class="text-sm text-gray-500">
              {{ 
                todayRecord?.checkInAt ? `打卡时间: ${new Date(todayRecord.checkInAt).toLocaleTimeString('zh-CN', { hour: '2-digit', minute: '2-digit' })}` :
                '今日未打卡'
              }}
            </div>
            <div v-if="todayRecord?.checkoutAt" class="text-sm text-gray-500">
              签退时间: {{ new Date(todayRecord.checkoutAt).toLocaleTimeString('zh-CN', { hour: '2-digit', minute: '2-digit' }) }}
            </div>
          </div>
        </div>

        <!-- 打卡按钮 -->
        <div class="flex space-x-16px">
          <NButton
            class="px-32px"
            type="primary"
            size="large"
            :disabled="!todayAttendanceStatus.canCheckIn || loading"
            :loading="loading"
            @click="handleCheckIn"
          >
            <template #icon>
              <Icon icon="mdi:clock-check" class="mr-8px" />
            </template>
            {{ todayAttendanceStatus.statusText }}
          </NButton>

          <NButton
            class="px-32px"
            type="info"
            size="large"
            :disabled="!todayAttendanceStatus.canCheckOut || loading"
            :loading="loading"
            @click="handleCheckOut"
          >
            <template #icon>
              <Icon icon="mdi:clock-out" class="mr-8px" />
            </template>
            {{ isCheckedOut ? '已签退' : '签退' }}
          </NButton>
        </div>
      </div>
    </NCard>

     <!-- 统计卡片 -->
     <NGrid cols="s:2 m:4" responsive="screen" :x-gap="20" :y-gap="20" class="mb-24px">
        <NGi>
          <NCard 
            :bordered="false" 
            class="stat-card stat-card-blue text-center bg-gradient-to-br from-blue-50 to-blue-100 dark:from-blue-900/30 dark:to-blue-800/30 backdrop-blur-sm shadow-lg hover:shadow-xl transition-all duration-300 cursor-pointer rounded-16px"
          >
           <div class="stat-card-content">
             <div class="stat-icon-wrapper">
               <Icon icon="mdi:calendar-check" class="stat-icon" />
             </div>
             <NStatistic 
               label="出勤天数" 
               :value="statistics.total" 
               class="stat-value"
               :label-style="{ color: '#6B7280', fontSize: '14px', fontWeight: '500' }"
               :value-style="{ color: '#1F2937', fontSize: '32px', fontWeight: '700' }"
             />
           </div>
           <div class="stat-card-decoration"></div>
         </NCard>
       </NGi>
        <NGi>
          <NCard 
            :bordered="false" 
            class="stat-card stat-card-green text-center bg-gradient-to-br from-green-50 to-green-100 dark:from-green-900/30 dark:to-green-800/30 backdrop-blur-sm shadow-lg hover:shadow-xl transition-all duration-300 cursor-pointer rounded-16px"
          >
           <div class="stat-card-content">
             <div class="stat-icon-wrapper">
               <Icon icon="mdi:check-circle" class="stat-icon" />
             </div>
             <NStatistic 
               label="正常出勤" 
               :value="statistics.present" 
               class="stat-value"
               :label-style="{ color: '#6B7280', fontSize: '14px', fontWeight: '500' }"
               :value-style="{ color: '#1F2937', fontSize: '32px', fontWeight: '700' }"
             />
           </div>
           <div class="stat-card-decoration"></div>
         </NCard>
       </NGi>
        <NGi>
          <NCard 
            :bordered="false" 
            class="stat-card stat-card-orange text-center bg-gradient-to-br from-orange-50 to-orange-100 dark:from-orange-900/30 dark:to-orange-800/30 backdrop-blur-sm shadow-lg hover:shadow-xl transition-all duration-300 cursor-pointer rounded-16px"
          >
           <div class="stat-card-content">
             <div class="stat-icon-wrapper">
               <Icon icon="mdi:clock-alert" class="stat-icon" />
             </div>
             <NStatistic 
               label="迟到次数" 
               :value="statistics.late" 
               class="stat-value"
               :label-style="{ color: '#6B7280', fontSize: '14px', fontWeight: '500' }"
               :value-style="{ color: '#1F2937', fontSize: '32px', fontWeight: '700' }"
             />
           </div>
           <div class="stat-card-decoration"></div>
         </NCard>
       </NGi>
        <NGi>
          <NCard 
            :bordered="false" 
            class="stat-card stat-card-purple text-center bg-gradient-to-br from-purple-50 to-purple-100 dark:from-purple-900/30 dark:to-purple-800/30 backdrop-blur-sm shadow-lg hover:shadow-xl transition-all duration-300 cursor-pointer rounded-16px"
          >
           <div class="stat-card-content">
             <div class="stat-icon-wrapper">
               <Icon icon="mdi:percent" class="stat-icon" />
             </div>
             <NStatistic 
               label="出勤率" 
               :value="statistics.attendanceRate" 
               suffix="%" 
               class="stat-value"
               :label-style="{ color: '#6B7280', fontSize: '14px', fontWeight: '500' }"
               :value-style="{ color: '#1F2937', fontSize: '32px', fontWeight: '700' }"
             />
           </div>
           <div class="stat-card-decoration"></div>
         </NCard>
       </NGi>
     </NGrid>

    <!-- 筛选表单 -->
    <NCard :bordered="false" class="mb-24px rounded-16px bg-white/90 shadow-lg backdrop-blur-sm dark:bg-gray-800/90">
      <NGrid :cols="24" :x-gap="16" :y-gap="16">
        <NGi :span="8">
          <div>
            <label class="mb-8px block text-sm text-gray-600 dark:text-gray-400">开始日期</label>
            <NDatePicker 
              v-model:value="filterForm.startDate" 
              type="date" 
              placeholder="选择开始日期" 
              clearable 
              @update:value="handleFilterChange"
              class="w-full"
            />
          </div>
        </NGi>
        <NGi :span="8">
          <div>
            <label class="mb-8px block text-sm text-gray-600 dark:text-gray-400">结束日期</label>
            <NDatePicker 
              v-model:value="filterForm.endDate" 
              type="date" 
              placeholder="选择结束日期" 
              clearable 
              @update:value="handleFilterChange"
              class="w-full"
            />
          </div>
        </NGi>
        <NGi :span="8">
          <div class="flex h-full items-end">
            <NSpace>
              <NButton type="primary" @click="handleReset">重置</NButton>
            </NSpace>
          </div>
        </NGi>
      </NGrid>
    </NCard>

    <!-- 考勤记录表格 -->
    <NCard :bordered="false" class="rounded-16px bg-white/90 shadow-lg backdrop-blur-sm dark:bg-gray-800/90">
      <template #header>
        <div class="flex items-center justify-between">
          <div>
            <span class="text-lg font-semibold text-gray-800 dark:text-gray-200">考勤记录</span>
            <!-- 显示当前筛选条件 -->
            <div v-if="hasActiveFilters" class="mt-1 flex flex-wrap gap-2">
              <NTag v-if="filterForm.startDate" size="small" type="info" closable @close="clearFilter('startDate')">
                开始: {{ new Date(filterForm.startDate).toLocaleDateString('zh-CN') }}
              </NTag>
              <NTag v-if="filterForm.endDate" size="small" type="info" closable @close="clearFilter('endDate')">
                结束: {{ new Date(filterForm.endDate).toLocaleDateString('zh-CN') }}
              </NTag>
              <NTag v-if="filterForm.status" size="small" type="warning" closable @close="clearFilter('status')">
                状态: {{ statusOptions.find(opt => opt.value === filterForm.status)?.label }}
              </NTag>
            </div>
          </div>
          <span class="text-sm text-gray-500">共 {{ pagination.itemCount }} 条记录</span>
        </div>
      </template>

      <NSpin :show="loading">
        <template #default>
          <NDataTable
            :columns="columns"
            :data="attendanceList"
              :pagination="{
                page: pagination.page,
                pageSize: pagination.pageSize,
                itemCount: pagination.itemCount,
                showSizePicker: true,
                pageSizes: [10, 15, 20, 30],
                onUpdatePage: handlePageChange,
                onUpdatePageSize: handlePageSizeChange
              }"
            :bordered="false"
            :single-line="false"
            class="mt-16px"
          />

          <NEmpty v-if="!loading && attendanceList.length === 0" description="暂无考勤记录" />
        </template>
      </NSpin>
    </NCard>

    <!-- 详情弹窗 -->
    <NModal v-model:show="showDetailModal" preset="card" title="考勤详情" class="w-[600px]">
      <div v-if="selectedRecord" class="space-y-16px">
        <!-- 基本信息 -->
        <div class="grid grid-cols-2 gap-16px">
          <div class="rounded-lg bg-gray-50 p-16px text-center dark:bg-gray-800">
            <div class="text-sm text-gray-500">日期</div>
            <div class="text-lg font-semibold text-gray-800 dark:text-gray-200">
              {{ new Date(selectedRecord.checkDate).toLocaleDateString('zh-CN') }}
            </div>
          </div>

          <div class="rounded-lg bg-gray-50 p-16px text-center dark:bg-gray-800">
            <div class="text-sm text-gray-500">状态</div>
            <NTag :type="statusMap[getAttendanceStatus(selectedRecord)].type as any" size="medium">
              {{ statusMap[getAttendanceStatus(selectedRecord)].text }}
            </NTag>
          </div>
        </div>

        <!-- 时间信息 -->
        <div class="grid grid-cols-2 gap-16px">
          <div class="rounded-lg bg-blue-50 p-16px text-center dark:bg-blue-900/20">
            <div class="text-sm text-blue-600 dark:text-blue-400">上班时间</div>
            <div class="text-lg font-semibold text-blue-800 dark:text-blue-200">
              {{ selectedRecord.checkInAt ? new Date(selectedRecord.checkInAt).toLocaleTimeString('zh-CN') : '未打卡' }}
            </div>
          </div>

          <div class="rounded-lg bg-green-50 p-16px text-center dark:bg-green-900/20">
            <div class="text-sm text-green-600 dark:text-green-400">下班时间</div>
            <div class="text-lg font-semibold text-green-800 dark:text-green-200">
              {{
                selectedRecord.checkoutAt
                  ? new Date(selectedRecord.checkoutAt).toLocaleTimeString('zh-CN')
                  : '未签退'
              }}
            </div>
          </div>
        </div>

        <!-- 工作时长 -->
        <div
          v-if="selectedRecord.checkInAt && selectedRecord.checkoutAt"
          class="rounded-lg bg-purple-50 p-16px text-center dark:bg-purple-900/20"
        >
          <div class="text-sm text-purple-600 dark:text-purple-400">工作时长</div>
          <div class="text-lg font-semibold text-purple-800 dark:text-purple-200">
            {{ calculateWorkHours(selectedRecord) }} 小时
          </div>
        </div>

        <!-- 详细信息 -->
        <NDescriptions label-placement="left" bordered :column="1">
          <NDescriptionsItem label="考勤ID">
            {{ selectedRecord.attendanceId }}
          </NDescriptionsItem>
          <NDescriptionsItem label="骑手ID">
            {{ selectedRecord.riderId }}
          </NDescriptionsItem>
          <NDescriptionsItem label="是否迟到">
            <NTag :type="selectedRecord.isLate ? 'warning' : 'success'" size="small">
              {{ selectedRecord.isLate ? '是' : '否' }}
            </NTag>
          </NDescriptionsItem>
          <NDescriptionsItem label="是否缺勤">
            <NTag :type="selectedRecord.isAbsent ? 'error' : 'success'" size="small">
              {{ selectedRecord.isAbsent ? '是' : '否' }}
            </NTag>
          </NDescriptionsItem>
        </NDescriptions>
      </div>
    </NModal>
  </div>
</template>

<style scoped>
/* 页面整体动画 */
.min-h-full {
  animation: fadeIn 0.6s ease-out;
  min-height: 150vh;
  overflow-y: auto;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* 卡片样式增强 */
.n-card {
  background-color: var(--n-color);
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.n-card:hover {
  transform: translateY(-4px) scale(1.02);
  box-shadow:
    0 20px 40px rgba(0, 0, 0, 0.1),
    0 0 0 1px rgba(255, 255, 255, 0.05);
}

/* 按钮样式增强 */
.n-button {
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
  border-radius: 12px;
  font-weight: 600;
  position: relative;
  overflow: hidden;
}

.n-button::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.2), transparent);
  transition: left 0.5s;
}

.n-button:hover::before {
  left: 100%;
}

.n-button:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.15);
}

.n-button:active {
  transform: translateY(0);
}

/* 表格行悬停效果 */
.n-data-table .n-data-table-tbody .n-data-table-tr:hover {
  background-color: rgba(59, 130, 246, 0.05);
  transform: scale(1.01);
  transition: all 0.2s ease;
}

/* 统计卡片样式 */
.stat-card {
  position: relative;
  padding: 24px;
  border-radius: 16px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  overflow: hidden;
  border: 1px solid rgba(255, 255, 255, 0.3);
  backdrop-filter: blur(10px);
}

.stat-card:hover {
  transform: translateY(-8px) scale(1.02);
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.15);
}

.stat-card-content {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 16px;
}

.stat-icon-wrapper {
  width: 56px;
  height: 56px;
  border-radius: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 8px;
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.15);
}

.stat-icon {
  font-size: 28px;
  color: white;
}

.stat-value {
  width: 100%;
}

.stat-card-decoration {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: linear-gradient(90deg, transparent, currentColor, transparent);
  opacity: 0.3;
}

/* 蓝色卡片 */
.stat-card-blue .stat-icon-wrapper {
  background: linear-gradient(135deg, #3B82F6 0%, #1D4ED8 100%);
}

.stat-card-blue .stat-card-decoration {
  background: linear-gradient(90deg, transparent, #3B82F6, transparent);
}

/* 绿色卡片 */
.stat-card-green .stat-icon-wrapper {
  background: linear-gradient(135deg, #10B981 0%, #047857 100%);
}

.stat-card-green .stat-card-decoration {
  background: linear-gradient(90deg, transparent, #10B981, transparent);
}

/* 橙色卡片 */
.stat-card-orange .stat-icon-wrapper {
  background: linear-gradient(135deg, #F59E0B 0%, #D97706 100%);
}

.stat-card-orange .stat-card-decoration {
  background: linear-gradient(90deg, transparent, #F59E0B, transparent);
}

/* 紫色卡片 */
.stat-card-purple .stat-icon-wrapper {
  background: linear-gradient(135deg, #8B5CF6 0%, #7C3AED 100%);
}

.stat-card-purple .stat-card-decoration {
  background: linear-gradient(90deg, transparent, #8B5CF6, transparent);
}

/* 暗色模式适配 */
.dark .stat-card {
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.dark .stat-card-blue .stat-icon-wrapper {
  background: linear-gradient(135deg, #1E40AF 0%, #1E3A8A 100%);
}

.dark .stat-card-green .stat-icon-wrapper {
  background: linear-gradient(135deg, #059669 0%, #047857 100%);
}

.dark .stat-card-orange .stat-icon-wrapper {
  background: linear-gradient(135deg, #D97706 0%, #B45309 100%);
}

.dark .stat-card-purple .stat-icon-wrapper {
  background: linear-gradient(135deg, #7C3AED 0%, #6D28D9 100%);
}

/* 统计卡片动画 */
.n-grid .n-gi > div {
  transition: all 0.3s ease;
  cursor: pointer;
}

.n-grid .n-gi > div:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.1);
}

/* 渐变背景特殊效果 */
.bg-gradient-to-br {
  position: relative;
  overflow: hidden;
}

.bg-gradient-to-br::before {
  content: '';
  position: absolute;
  top: -50%;
  left: -50%;
  width: 200%;
  height: 200%;
  background: linear-gradient(45deg, transparent, rgba(255, 255, 255, 0.1), transparent);
  animation: shimmer 3s infinite;
}

@keyframes shimmer {
  0% {
    transform: translateX(-100%) translateY(-100%) rotate(45deg);
  }
  100% {
    transform: translateX(100%) translateY(100%) rotate(45deg);
  }
}

/* 自定义滚动条 */
::-webkit-scrollbar {
  width: 8px;
}

::-webkit-scrollbar-track {
  background: rgba(0, 0, 0, 0.05);
  border-radius: 4px;
}

::-webkit-scrollbar-thumb {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 4px;
}

::-webkit-scrollbar-thumb:hover {
  background: linear-gradient(135deg, #5a67d8 0%, #6b46c1 100%);
}

/* 暗色模式滚动条 */
.dark ::-webkit-scrollbar-track {
  background: rgba(255, 255, 255, 0.05);
}

.dark ::-webkit-scrollbar-thumb {
  background: linear-gradient(135deg, #4c51bf 0%, #553c9a 100%);
}

.dark ::-webkit-scrollbar-thumb:hover {
  background: linear-gradient(135deg, #434190 0%, #4c1d95 100%);
}

/* 响应式设计 */
@media (max-width: 768px) {
  .n-grid {
    grid-template-columns: 1fr !important;
  }

  .n-gi {
    grid-column: span 24 !important;
  }

  .flex.gap-24px {
    flex-direction: column;
    gap: 16px;
  }

  .text-4xl {
    font-size: 2rem;
  }

  .space-x-32px > * + * {
    margin-left: 1rem;
  }
}

/* 加载状态动画 */
.n-button[loading] {
  position: relative;
}

.n-button[loading]::after {
  content: '';
  position: absolute;
  width: 16px;
  height: 16px;
  border: 2px solid transparent;
  border-top: 2px solid currentColor;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}

/* 模态框动画增强 */
.n-modal .n-card {
  animation: modalSlideIn 0.3s ease-out;
}

@keyframes modalSlideIn {
  from {
    opacity: 0;
    transform: translateY(-20px) scale(0.95);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}
</style>
