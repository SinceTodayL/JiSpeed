<script setup lang="ts">
import { computed, h, onMounted, ref } from 'vue';
import {
  NButton,
  NCard,
  NDataTable,
  NEmpty,
  NModal,
  NSpin,
  NTag
} from 'naive-ui';
import { Icon } from '@iconify/vue';
import { getRiderInfo, riderCheckIn, riderCheckOut } from '@/service/api/rider';
import {
  getAttendanceRecords,
  getTodayAttendance
} from '@/service/api/attendance';
import { useAuthStore } from '@/store/modules/auth';

const authStore = useAuthStore();

// 骑手ID - 使用登录用户的ID
const riderId = computed(() => authStore.userInfo.userId);

// 响应式数据
const attendanceList = ref<Api.Attendance.AttendanceRecord[]>([]);
const riderInfo = ref<Api.Rider.InfoData | null>(null);
const todayRecord = ref<Api.Attendance.AttendanceRecord | null>(null);
const loading = ref(false);
const showDetailModal = ref(false);
const selectedRecord = ref<Api.Attendance.AttendanceRecord | null>(null);

// 考勤状态
const isCheckedIn = ref(false);
const isCheckedOut = ref(false);
const currentTime = ref(new Date());

// 实时更新时间
const updateTime = () => {
  currentTime.value = new Date();
};

// 获取骑手信息
async function fetchRiderInfo() {
  try {
    const { data } = await getRiderInfo({ riderId: riderId.value });
    if (data) {
      riderInfo.value = data;
    }
  } catch (error) {
    console.error('获取骑手信息失败', error);
  }
}

// 获取考勤记录
async function fetchAttendanceRecords() {
  loading.value = true;
  try {
    // 使用更标准的日期格式，确保后端能正确解析
    const currentDate = new Date();
    const firstDayOfMonth = new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);

    const params = {
      riderId: riderId.value,
      startDate: firstDayOfMonth.toISOString().split('T')[0], // YYYY-MM-DD
      endDate: currentDate.toISOString().split('T')[0] // YYYY-MM-DD
    };

    // 确保日期格式正确
    console.log('开始日期:', params.startDate);
    console.log('结束日期:', params.endDate);
    console.log('骑手ID:', params.riderId);

    console.log('获取考勤记录参数:', params);
    const { data } = await getAttendanceRecords(params);
    console.log('获取考勤记录响应:', data);

    if (data && data.data) {
      // 后端返回的是考勤记录数组，需要转换为前端格式
      const backendRecords = (data.data as any) || [];
      attendanceList.value = backendRecords.map((record: any) => ({
        id: record.attendanceId || '',
        riderId: record.riderId || '',
        date: record.checkDate ? new Date(record.checkDate).toISOString().split('T')[0] : '',
        checkInTime: record.checkInAt ? new Date(record.checkInAt).toISOString() : undefined,
        checkOutTime: record.checkoutAt ? new Date(record.checkoutAt).toISOString() : undefined,
        status: record.isLate > 0 ? 'late' : 'present',
        location: '',
        notes: '',
        workHours: 0
      }));

      // 查找今日记录
      const today = new Date().toISOString().split('T')[0];
      todayRecord.value = attendanceList.value.find(record => record.date === today) || null;

      // 更新打卡状态
      if (todayRecord.value) {
        isCheckedIn.value = Boolean(todayRecord.value.checkInTime);
        isCheckedOut.value = Boolean(todayRecord.value.checkOutTime);
      }
    }
  } catch (error: any) {
    console.error('获取考勤记录失败', error);
    // 详细记录错误信息
    if (error.response) {
      console.error('错误状态码:', error.response.status);
      console.error('错误数据:', error.response.data);
      console.error('错误头:', error.response.headers);
    } else if (error.request) {
      console.error('请求错误:', error.request);
    } else {
      console.error('错误信息:', error.message);
    }

    window.$message?.error('获取考勤记录失败');
    attendanceList.value = [];
    todayRecord.value = null;
    isCheckedIn.value = false;
    isCheckedOut.value = false;
  } finally {
    loading.value = false;
  }
}

// 获取今日考勤状态
async function fetchTodayAttendance() {
  try {
    const { data } = await getTodayAttendance(riderId.value);
    console.log('今日考勤状态:', data);

    if (data && data.data) {
      // 使用类型断言处理后端数据
      const backendData = data.data as any;

      // 将后端数据转换为前端格式
      todayRecord.value = {
        id: backendData.attendanceId || '',
        riderId: riderId.value,
        date: backendData.checkDate ? new Date(backendData.checkDate).toISOString().split('T')[0] : '',
        checkInTime: backendData.checkInAt ? new Date(backendData.checkInAt).toISOString() : undefined,
        checkOutTime: backendData.checkoutAt ? new Date(backendData.checkoutAt).toISOString() : undefined,
        status: backendData.isLate > 0 ? 'late' : 'present',
        location: '',
        notes: '',
        workHours: 0
      };
      isCheckedIn.value = Boolean(backendData.checkInAt);
      isCheckedOut.value = Boolean(backendData.checkoutAt);
    } else {
      // 今日无考勤记录
      todayRecord.value = null;
      isCheckedIn.value = false;
      isCheckedOut.value = false;
    }
  } catch (error: any) {
    console.error('获取今日考勤状态失败', error);
    // 如果获取失败，重置状态
    todayRecord.value = null;
    isCheckedIn.value = false;
    isCheckedOut.value = false;
  }
}

// 打卡
async function handleCheckIn() {
  try {
    loading.value = true;
    console.log('开始打卡，骑手ID:', riderId.value);

    const { data } = await riderCheckIn(riderId.value, {
      checkDate: new Date().toISOString().split('T')[0],
      checkInAt: new Date().toISOString(),
      isLate: 0,
      isAbsent: 0
    });
    console.log('打卡响应:', data);

    if (data && data.data) {
      window.$message?.success('打卡成功！');
      isCheckedIn.value = true;
      await fetchTodayAttendance(); // 刷新今日考勤状态
    }
  } catch (error) {
    console.error('打卡失败', error);
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
    console.log('开始签退，骑手ID:', riderId.value);

    const { data } = await riderCheckOut(riderId.value, {
      checkDate: new Date().toISOString().split('T')[0],
      checkInAt: new Date().toISOString(),
      isLate: 0,
      isAbsent: 0
    });
    console.log('签退响应:', data);

    if (data && data.data) {
      window.$message?.success('签退成功！');
      isCheckedOut.value = true;
      await fetchTodayAttendance(); // 刷新今日考勤状态
    }
  } catch (error) {
    console.error('签退失败', error);
    window.$message?.error('签退失败，请稍后重试');
  } finally {
    loading.value = false;
  }
}

// 查看详情
function showDetail(record: Api.Attendance.AttendanceRecord) {
  selectedRecord.value = record;
  showDetailModal.value = true;
}

// 计算统计数据
const statistics = computed(() => {
  const total = attendanceList.value.length;
  const present = attendanceList.value.filter(record => record.status === 'present').length;
  const late = attendanceList.value.filter(record => record.status === 'late').length;
  const absent = attendanceList.value.filter(record => record.status === 'absent').length;
  const leave = attendanceList.value.filter(record => record.status === 'leave').length;

  return {
    total,
    present,
    late,
    absent,
    leave,
    attendanceRate: total > 0 ? Math.round((present / total) * 100) : 0
  };
});

// 考勤状态映射
const statusMap = {
  present: { text: '正常', type: 'success' },
  absent: { text: '缺勤', type: 'error' },
  late: { text: '迟到', type: 'warning' },
  leave: { text: '请假', type: 'info' }
};

// 表格列配置
const columns = [
  {
    title: '日期',
    key: 'date',
    width: 120,
    render: (row: Api.Attendance.AttendanceRecord) => {
      const date = new Date(row.date);
      const today = new Date();
      const isToday = date.toDateString() === today.toDateString();

      return h('div', { class: 'flex items-center' }, [
        h('span', { class: isToday ? 'text-blue-600 font-medium' : '' },
          date.toLocaleDateString('zh-CN', { month: '2-digit', day: '2-digit' })
        ),
        isToday && h(NTag, { type: 'primary', size: 'tiny' }, { default: () => '今日' })
      ]);
    }
  },
  {
    title: '上班时间',
    key: 'checkInTime',
    width: 100,
    render: (row: Api.Attendance.AttendanceRecord) => {
      if (!row.checkInTime) return h('span', { class: 'text-gray-400' }, '未打卡');
      const time = new Date(row.checkInTime).toLocaleTimeString('zh-CN', {
        hour: '2-digit',
        minute: '2-digit'
      });
      return h('span', { class: 'font-medium' }, time);
    }
  },
  {
    title: '下班时间',
    key: 'checkOutTime',
    width: 100,
    render: (row: Api.Attendance.AttendanceRecord) => {
      if (!row.checkOutTime) return h('span', { class: 'text-gray-400' }, '未签退');
      const time = new Date(row.checkOutTime).toLocaleTimeString('zh-CN', {
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
      if (!row.checkInTime || !row.checkOutTime) return '-';

      const start = new Date(row.checkInTime);
      const end = new Date(row.checkOutTime);
      const hours = Math.round((end.getTime() - start.getTime()) / (1000 * 60 * 60) * 10) / 10;

      return h('span', { class: 'text-green-600 font-medium' }, `${hours}h`);
    }
  },
  {
    title: '状态',
    key: 'status',
    width: 100,
    render: (row: Api.Attendance.AttendanceRecord) => {
      const statusInfo = statusMap[row.status];
      return h(NTag, {
        type: statusInfo.type as any,
        size: 'small'
      }, { default: () => statusInfo.text });
    }
  },
  {
    title: '操作',
    key: 'actions',
    width: 80,
    render: (row: Api.Attendance.AttendanceRecord) => {
      return h(NButton, {
        size: 'small',
        type: 'primary',
        ghost: true,
        onClick: () => showDetail(row)
      }, { default: () => '详情' });
    }
  }
];

// 页面加载
onMounted(async () => {
  // 检查用户信息是否已初始化
  if (!authStore.userInfo.userId && authStore.token) {
    await authStore.initUserInfo();
  }



  await Promise.all([
    fetchRiderInfo(),
    fetchTodayAttendance(), // 先获取今日考勤状态
    fetchAttendanceRecords() // 再获取考勤记录列表
  ]);

  // 启动实时时钟
  setInterval(updateTime, 1000);
});
</script>

<template>
  <div class="h-full p-24px bg-gray-50 dark:bg-gray-900">
    <!-- 页面头部 -->
    <div class="mb-24px">
      <h1 class="text-3xl font-bold text-gray-800 dark:text-gray-200 mb-8px">
        考勤管理
      </h1>
      <p class="text-gray-600 dark:text-gray-400">
        管理您的出勤记录，保持工作纪律
      </p>
    </div>

    <!-- 今日考勤卡片 -->
    <NCard :bordered="false" class="mb-24px shadow-sm">
      <div class="flex items-center justify-between">
        <div class="flex items-center space-x-32px">
          <!-- 当前时间 -->
          <div class="text-center">
            <div class="text-4xl font-bold text-blue-600 dark:text-blue-400">
              {{ currentTime.toLocaleTimeString('zh-CN', { hour: '2-digit', minute: '2-digit', second: '2-digit' }) }}
            </div>
            <div class="text-sm text-gray-500 mt-4px">
              {{ currentTime.toLocaleDateString('zh-CN', { year: 'numeric', month: 'long', day: 'numeric', weekday: 'long' }) }}
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
              {{ todayRecord?.checkInTime ? '已打卡' : '未打卡' }}
            </div>
            <div class="text-sm text-gray-500">
              {{ todayRecord?.checkOutTime ? '已签退' : '未签退' }}
            </div>
          </div>
        </div>

        <!-- 打卡按钮 -->
        <div class="flex space-x-16px">
          <NButton
            type="primary"
            size="large"
            :disabled="isCheckedIn || loading"
            :loading="loading"
            @click="handleCheckIn"
            class="px-32px"
          >
            <template #icon>
              <Icon icon="mdi:clock-check" class="mr-8px" />
            </template>
            {{ isCheckedIn ? '已打卡' : '打卡' }}
          </NButton>

          <NButton
            type="info"
            size="large"
            :disabled="!isCheckedIn || isCheckedOut || loading"
            :loading="loading"
            @click="handleCheckOut"
            class="px-32px"
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
    <div class="grid grid-cols-4 mb-24px gap-16px">
      <NCard>
        <div class="text-center">
          <div class="text-2xl text-blue-600 font-bold">{{ statistics.total }}</div>
          <div class="text-sm text-gray-500">总出勤天数</div>
        </div>
      </NCard>

      <NCard>
        <div class="text-center">
          <div class="text-2xl text-green-600 font-bold">{{ statistics.present }}</div>
          <div class="text-sm text-gray-500">正常出勤</div>
        </div>
      </NCard>

      <NCard>
        <div class="text-center">
          <div class="text-2xl text-yellow-600 font-bold">{{ statistics.late }}</div>
          <div class="text-sm text-gray-500">迟到次数</div>
        </div>
      </NCard>

      <NCard>
        <div class="text-center">
          <div class="text-2xl text-red-600 font-bold">{{ statistics.attendanceRate }}%</div>
          <div class="text-sm text-gray-500">出勤率</div>
        </div>
      </NCard>
    </div>

    <!-- 考勤记录表格 -->
    <NCard :bordered="false" class="shadow-sm">
      <template #header>
        <div class="flex items-center justify-between">
          <span class="text-lg font-semibold text-gray-800 dark:text-gray-200">考勤记录</span>
          <span class="text-sm text-gray-500">本月共 {{ statistics.total }} 条记录</span>
        </div>
      </template>

      <NSpin :show="loading">
        <template #default>
          <NDataTable
            :columns="columns"
            :data="attendanceList"
            :pagination="{
              pageSize: 15,
              showSizePicker: true,
              pageSizes: [10, 15, 20, 30]
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
    <NModal v-model:show="showDetailModal" preset="card" title="考勤详情" style="width: 600px">
      <div v-if="selectedRecord" class="space-y-16px">
        <!-- 基本信息 -->
        <div class="grid grid-cols-2 gap-16px">
          <div class="text-center p-16px bg-gray-50 dark:bg-gray-800 rounded-lg">
            <div class="text-sm text-gray-500">日期</div>
            <div class="text-lg font-semibold text-gray-800 dark:text-gray-200">
              {{ new Date(selectedRecord.date).toLocaleDateString('zh-CN') }}
            </div>
          </div>

          <div class="text-center p-16px bg-gray-50 dark:bg-gray-800 rounded-lg">
            <div class="text-sm text-gray-500">状态</div>
            <NTag :type="statusMap[selectedRecord.status].type as any" size="medium">
              {{ statusMap[selectedRecord.status].text }}
            </NTag>
          </div>
        </div>

        <!-- 时间信息 -->
        <div class="grid grid-cols-2 gap-16px">
          <div class="text-center p-16px bg-blue-50 dark:bg-blue-900/20 rounded-lg">
            <div class="text-sm text-blue-600 dark:text-blue-400">上班时间</div>
            <div class="text-lg font-semibold text-blue-800 dark:text-blue-200">
              {{ selectedRecord.checkInTime ? new Date(selectedRecord.checkInTime).toLocaleTimeString('zh-CN') : '未打卡' }}
            </div>
          </div>

          <div class="text-center p-16px bg-green-50 dark:bg-green-900/20 rounded-lg">
            <div class="text-sm text-green-600 dark:text-green-400">下班时间</div>
            <div class="text-lg font-semibold text-green-800 dark:text-green-200">
              {{ selectedRecord.checkOutTime ? new Date(selectedRecord.checkOutTime).toLocaleTimeString('zh-CN') : '未签退' }}
            </div>
          </div>
        </div>

        <!-- 工作时长 -->
        <div v-if="selectedRecord.checkInTime && selectedRecord.checkOutTime" class="text-center p-16px bg-purple-50 dark:bg-purple-900/20 rounded-lg">
          <div class="text-sm text-purple-600 dark:text-purple-400">工作时长</div>
          <div class="text-lg font-semibold text-purple-800 dark:text-purple-200">
            {{ Math.round((new Date(selectedRecord.checkOutTime).getTime() - new Date(selectedRecord.checkInTime).getTime()) / (1000 * 60 * 60) * 10) / 10 }} 小时
          </div>
        </div>

        <!-- 备注信息 -->
        <div v-if="selectedRecord.notes" class="p-16px bg-gray-50 dark:bg-gray-800 rounded-lg">
          <div class="text-sm text-gray-500 mb-8px">备注</div>
          <div class="text-gray-800 dark:text-gray-200">{{ selectedRecord.notes }}</div>
        </div>
      </div>
    </NModal>
  </div>
</template>

<style scoped>
.n-card {
  transition: all 0.3s ease;
}

.n-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.1);
}

.n-button {
  transition: all 0.3s ease;
}

.n-button:hover:not(:disabled) {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.n-button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* 表格行悬停效果 */
.n-data-table .n-data-table-tbody .n-data-table-tr:hover {
  background-color: rgba(59, 130, 246, 0.05);
}

/* 响应式设计 */
@media (max-width: 768px) {
  .text-4xl {
    font-size: 2rem;
  }

  .space-x-32px > * + * {
    margin-left: 1rem;
  }
}
</style>
