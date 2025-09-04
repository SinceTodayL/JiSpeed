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
  getAttendanceDetail,
  getRiderAttendanceRecordsList,
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

// 筛选条件
const filterForm = ref({
  startDate: null as number | null,
  endDate: null as number | null,
  status: undefined as 'all' | 'present' | 'late' | 'absent' | undefined
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
    const { data } = await getRiderInfo(riderId.value);
    if (data) {
      riderInfo.value = data;
    }
  } catch {
    // 获取骑手信息失败
  }
}

// 获取今日考勤状态
async function fetchTodayAttendance() {
  try {
    const { data } = await getTodayAttendance(riderId.value);

    if (data && data.data) {
      todayRecord.value = data.data;
      isCheckedIn.value = Boolean(data.data.checkInAt);
      isCheckedOut.value = Boolean(data.data.checkoutAt);
    } else {
      todayRecord.value = null;
      isCheckedIn.value = false;
      isCheckedOut.value = false;
    }
  } catch {
    // 获取今日考勤状态失败
    todayRecord.value = null;
    isCheckedIn.value = false;
    isCheckedOut.value = false;
  }
}

// 获取考勤记录列表
async function fetchAttendanceRecords() {
  loading.value = true;
  try {
    const params: Api.Attendance.GetRiderAttendanceRequest = {
      pageIndex: pagination.value.page,
      pageSize: pagination.value.pageSize,
      ...(filterForm.value.startDate && {
        startDate: new Date(filterForm.value.startDate).toISOString().split('T')[0]
      }),
      ...(filterForm.value.endDate && {
        endDate: new Date(filterForm.value.endDate).toISOString().split('T')[0]
      }),
      ...(filterForm.value.status === 'late' && { isLate: 1 }),
      ...(filterForm.value.status === 'absent' && { isAbsent: 1 })
    };

    const { data } = await getRiderAttendanceRecordsList(riderId.value, params);

    if (data && data.data) {
      attendanceList.value = data.data.records || [];
      pagination.value.itemCount = data.data.total || 0;
    } else {
      attendanceList.value = [];
      pagination.value.itemCount = 0;
    }
  } catch {
    // 获取考勤记录失败
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
    const currentDate = new Date();
    const firstDayOfMonth = new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);
    const lastDayOfMonth = new Date(currentDate.getFullYear(), currentDate.getMonth() + 1, 0);

    const params: Api.Attendance.GetRiderStatsRequest = {
      startDate: firstDayOfMonth.toISOString().split('T')[0],
      endDate: lastDayOfMonth.toISOString().split('T')[0]
    };

    const { data } = await getRiderAttendanceStats(riderId.value, params);
    if (data && data.data) {
      attendanceStats.value = data.data;
    }
  } catch {
    // 获取考勤统计失败
  }
}

// 打卡
async function handleCheckIn() {
  try {
    loading.value = true;
    const checkInData: Api.Attendance.CheckInRequest = {
      checkDate: new Date().toISOString().split('T')[0],
      checkInAt: new Date().toISOString(),
      isLate: 0,
      isAbsent: 0
    };
    console.log(checkInData);
    const { data } = await riderCheckInAttendance(riderId.value, checkInData);
    console.log('data',data);
    if (data) {
      console.log('okok');
      window.$message?.success('打卡成功！');
      isCheckedIn.value = true;
      await fetchTodayAttendance();
      await fetchAttendanceRecords();
    }
  } catch {
    // 打卡失败
    window.$message?.error('打卡失败，请稍后重试');
  } finally {
    loading.value = false;
  }
}

// 签退
async function handleCheckOut() {
  if (!todayRecord.value) return;

  try {
    loading.value = true;

    const checkOutData: Api.Attendance.CheckOutRequest = {
      checkoutAt: new Date().toISOString(),
      location: {
        longitude: 116.3974, // 默认位置，实际应该获取用户位置
        latitude: 39.9093,
        address: '北京市'
      }
    };

    const { data } = await riderCheckOutAttendance(riderId.value, checkOutData);

    if (data && data.data) {
      window.$message?.success('签退成功！');
      isCheckedOut.value = true;
      await fetchTodayAttendance();
      await fetchAttendanceRecords();
    }
  } catch {
    // 签退失败
    window.$message?.error('签退失败，请稍后重试');
  } finally {
    loading.value = false;
  }
}

// 查看详情
async function showDetail(record: Api.Attendance.AttendanceRecord) {
  try {
    const { data } = await getAttendanceDetail(record.attendanceId);
    if (data && data.data) {
      selectedRecord.value = data.data;
      showDetailModal.value = true;
    }
  } catch {
    // 获取考勤详情失败
    window.$message?.error('获取考勤详情失败');
  }
}

// 搜索
function handleSearch() {
  pagination.value.page = 1;
  fetchAttendanceRecords();
}

// 重置筛选
function handleReset() {
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

// 计算统计数据
const statistics = computed(() => {
  const total = attendanceList.value.length;
  const present = attendanceList.value.filter(record => record.isLate === 0 && record.isAbsent === 0).length;
  const late = attendanceList.value.filter(record => record.isLate === 1).length;
  const absent = attendanceList.value.filter(record => record.isAbsent === 1).length;

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

// 页面加载
onMounted(async () => {
  // 检查用户信息是否已初始化
  if (!authStore.userInfo.userId && authStore.token) {
    await authStore.initUserInfo();
  }

  // 如果仍然没有用户信息，使用模拟数据
  if (!authStore.userInfo.userId) {
    Object.assign(authStore.userInfo, {
      userId: `rider_${Date.now()}`,
      userName: '测试骑手',
      roles: ['rider'],
      buttons: []
    });
  }

  await Promise.all([fetchRiderInfo(), fetchTodayAttendance(), fetchAttendanceRecords(), fetchAttendanceStats()]);

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
              {{ riderInfo?.name || '骑手' }}
            </div>
            <div class="text-sm text-gray-500">车辆：{{ riderInfo?.vehicleNumber || '暂无' }}</div>
          </div>

          <!-- 今日状态 -->
          <div class="text-center">
            <div class="text-lg font-semibold text-gray-800 dark:text-gray-200">
              {{ todayRecord?.checkInAt ? '已打卡' : '未打卡' }}
            </div>
            <div class="text-sm text-gray-500">
              {{ todayRecord?.checkoutAt ? '已签退' : '未签退' }}
            </div>
          </div>
        </div>

        <!-- 打卡按钮 -->
        <div class="flex space-x-16px">
          <NButton
            class="px-32px"
            type="primary"
            size="large"
            :disabled="isCheckedIn || loading"
            :loading="loading"
            @click="handleCheckIn"
          >
            <template #icon>
              <Icon icon="mdi:clock-check" class="mr-8px" />
            </template>
            {{ isCheckedIn ? '已打卡' : '打卡' }}
          </NButton>

          <NButton
            class="px-32px"
            type="info"
            size="large"
            :disabled="!isCheckedIn || isCheckedOut || loading"
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
    <NGrid cols="s:2 m:4" responsive="screen" :x-gap="16" :y-gap="16" class="mb-24px">
      <NGi>
        <NCard class="text-center bg-gradient-to-r from-blue-500 to-blue-600 text-white">
          <NStatistic label="总出勤天数" :value="attendanceStats?.totalWorkDays || 0" />
        </NCard>
      </NGi>
      <NGi>
        <NCard class="text-center bg-gradient-to-r from-green-500 to-green-600 text-white">
          <NStatistic label="正常出勤" :value="statistics.present" />
        </NCard>
      </NGi>
      <NGi>
        <NCard class="text-center bg-gradient-to-r from-yellow-500 to-yellow-600 text-white">
          <NStatistic label="迟到次数" :value="attendanceStats?.lateCount || 0" />
        </NCard>
      </NGi>
      <NGi>
        <NCard class="text-center bg-gradient-to-r from-purple-500 to-purple-600 text-white">
          <NStatistic label="出勤率" :value="attendanceStats?.attendanceRate || 0" suffix="%" />
        </NCard>
      </NGi>
    </NGrid>

    <!-- 筛选表单 -->
    <NCard :bordered="false" class="mb-24px rounded-16px bg-white/90 shadow-lg backdrop-blur-sm dark:bg-gray-800/90">
      <NGrid :cols="24" :x-gap="16" :y-gap="16">
        <NGi :span="6">
          <div>
            <label class="mb-8px block text-sm text-gray-600 dark:text-gray-400">开始日期</label>
            <NDatePicker v-model:value="filterForm.startDate" type="date" placeholder="选择开始日期" clearable />
          </div>
        </NGi>
        <NGi :span="6">
          <div>
            <label class="mb-8px block text-sm text-gray-600 dark:text-gray-400">结束日期</label>
            <NDatePicker v-model:value="filterForm.endDate" type="date" placeholder="选择结束日期" clearable />
          </div>
        </NGi>
        <NGi :span="6">
          <div>
            <label class="mb-8px block text-sm text-gray-600 dark:text-gray-400">状态筛选</label>
            <NSelect v-model:value="filterForm.status" :options="statusOptions" placeholder="选择状态" clearable />
          </div>
        </NGi>
        <NGi :span="6">
          <div class="flex h-full items-end">
            <NSpace>
              <NButton type="primary" @click="handleSearch">搜索</NButton>
              <NButton @click="handleReset">重置</NButton>
            </NSpace>
          </div>
        </NGi>
      </NGrid>
    </NCard>

    <!-- 考勤记录表格 -->
    <NCard :bordered="false" class="rounded-16px bg-white/90 shadow-lg backdrop-blur-sm dark:bg-gray-800/90">
      <template #header>
        <div class="flex items-center justify-between">
          <span class="text-lg font-semibold text-gray-800 dark:text-gray-200">考勤记录</span>
          <span class="text-sm text-gray-500">本月共 {{ pagination.itemCount }} 条记录</span>
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
              onUpdatePage: handlePageChange
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
  min-height: 100vh;
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
