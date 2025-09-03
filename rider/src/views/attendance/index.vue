<script setup lang="ts">
import { computed, h, onMounted, ref } from 'vue';
import { NButton, NCard, NDataTable, NDescriptions, NDescriptionsItem, NModal, NSpace, NTag } from 'naive-ui';
import { handleAttendanceError, handleCommonError } from '@/utils/rider-error-handler';

// 考勤数据类型
interface AttendanceData {
  id: string;
  riderId: string;
  date: string;
  checkInTime?: string;
  checkOutTime?: string;
  status: 'present' | 'absent' | 'late' | 'leave';
  location?: string;
  notes?: string;
}

// 骑手ID
const riderId = '1663b73718a54c65b32f5b6787972949';

// 响应式数据
const attendanceList = ref<AttendanceData[]>([]);
const loading = ref(false);
const showModal = ref(false);
const selectedRecord = ref<AttendanceData | null>(null);

// 考勤状态映射
const statusMap = {
  present: { text: '正常', type: 'success' },
  absent: { text: '缺勤', type: 'error' },
  late: { text: '迟到', type: 'warning' },
  leave: { text: '请假', type: 'info' }
};

// 今日考勤状态
const todayAttendance = ref<AttendanceData | null>(null);
const isCheckedIn = ref(false);
const isCheckedOut = ref(false);

// 获取考勤记录
const fetchAttendanceRecords = async () => {
  loading.value = true;
  try {
    // 替换为实际的API调用
    // const { data } = await getAttendanceRecords(riderId);
    // attendanceList.value = data;

    // 模拟数据
    attendanceList.value = [
      {
        id: '1',
        riderId,
        date: '2024-01-15',
        checkInTime: '08:30:00',
        checkOutTime: '18:00:00',
        status: 'present',
        location: '北京市朝阳区',
        notes: '正常出勤'
      },
      {
        id: '2',
        riderId,
        date: '2024-01-14',
        checkInTime: '08:45:00',
        checkOutTime: '18:30:00',
        status: 'late',
        location: '北京市朝阳区',
        notes: '迟到15分钟'
      }
    ];
  } catch (error) {
    handleCommonError(error, '获取考勤记录');
  } finally {
    loading.value = false;
  }
};

// 获取今日考勤状态
const fetchTodayAttendance = async () => {
  try {
    // 替换为实际的API调用
    // const { data } = await getTodayAttendance(riderId);
    // todayAttendance.value = data;

    // 模拟数据
    const today = new Date().toISOString().split('T')[0];
    const todayRecord = attendanceList.value.find(record => record.date === today);

    if (todayRecord) {
      todayAttendance.value = todayRecord;
      isCheckedIn.value = Boolean(todayRecord.checkInTime);
      isCheckedOut.value = Boolean(todayRecord.checkOutTime);
    }
  } catch (error) {
    handleCommonError(error, '获取今日考勤');
  }
};

// 打卡
const handleCheckIn = async () => {
  try {
    // 替换为实际的API调用
    // await checkIn(riderId);

    window.$message?.success('打卡成功！');
    isCheckedIn.value = true;
    await fetchTodayAttendance();
  } catch (error) {
    handleAttendanceError(error, '打卡失败，请稍后重试');
  }
};

// 签退
const handleCheckOut = async () => {
  try {
    // 替换为实际的API调用
    // await checkOut(riderId);

    window.$message?.success('签退成功！');
    isCheckedOut.value = true;
    await fetchTodayAttendance();
  } catch (error) {
    handleAttendanceError(error, '签退失败，请稍后重试');
  }
};

// 查看详情
const handleDetail = (record: AttendanceData) => {
  selectedRecord.value = record;
  showModal.value = true;
};

// 表格列配置
const columns = [
  {
    title: '日期',
    key: 'date',
    render: (row: AttendanceData) => row.date
  },
  {
    title: '上班时间',
    key: 'checkInTime',
    render: (row: AttendanceData) => row.checkInTime || '-'
  },
  {
    title: '下班时间',
    key: 'checkOutTime',
    render: (row: AttendanceData) => row.checkOutTime || '-'
  },
  {
    title: '状态',
    key: 'status',
    render: (row: AttendanceData) => {
      const statusInfo = statusMap[row.status];
      return h(NTag, { type: statusInfo.type as any, size: 'small' }, { default: () => statusInfo.text });
    }
  },
  {
    title: '位置',
    key: 'location',
    render: (row: AttendanceData) => row.location || '-'
  },
  {
    title: '备注',
    key: 'notes',
    render: (row: AttendanceData) => row.notes || '-'
  },
  {
    title: '操作',
    key: 'actions',
    render: (row: AttendanceData) => {
      return h(
        NSpace,
        {},
        {
          default: () => [
            h(NButton, { size: 'small', type: 'primary', onClick: () => handleDetail(row) }, { default: () => '详情' })
          ]
        }
      );
    }
  }
];

// 计算统计数据
const statistics = computed(() => {
  const total = attendanceList.value.length;
  const present = attendanceList.value.filter(record => record.status === 'present').length;
  const late = attendanceList.value.filter(record => record.status === 'late').length;
  const absent = attendanceList.value.filter(record => record.status === 'absent').length;

  return {
    total,
    present,
    late,
    absent,
    attendanceRate: total > 0 ? Math.round((present / total) * 100) : 0
  };
});

onMounted(() => {
  fetchAttendanceRecords();
  fetchTodayAttendance();
});
</script>

<template>
  <div class="h-full p-24px">
    <!-- 页面标题 -->
    <NCard :bordered="false" class="mb-24px">
      <div class="flex items-center justify-between">
        <div class="flex-1">
          <h1 class="text-2xl text-gray-800 font-bold dark:text-gray-200">考勤管理</h1>
          <p class="mt-8px text-gray-600 dark:text-gray-400">记录和管理您的出勤情况，保持工作纪律。</p>
        </div>
      </div>
    </NCard>

    <!-- 今日考勤卡片 -->
    <NCard title="今日考勤" class="mb-24px">
      <div class="flex items-center justify-between">
        <div class="flex items-center space-x-16px">
          <div class="text-center">
            <div class="text-2xl text-gray-800 font-bold dark:text-gray-200">
              {{ new Date().toLocaleDateString('zh-CN') }}
            </div>
            <div class="text-sm text-gray-500">今日日期</div>
          </div>

          <div class="text-center">
            <div class="text-lg text-gray-800 font-semibold dark:text-gray-200">
              {{ todayAttendance?.checkInTime || '未打卡' }}
            </div>
            <div class="text-sm text-gray-500">上班时间</div>
          </div>

          <div class="text-center">
            <div class="text-lg text-gray-800 font-semibold dark:text-gray-200">
              {{ todayAttendance?.checkOutTime || '未签退' }}
            </div>
            <div class="text-sm text-gray-500">下班时间</div>
          </div>
        </div>

        <div class="flex space-x-12px">
          <NButton type="primary" :disabled="isCheckedIn" @click="handleCheckIn">
            {{ isCheckedIn ? '已打卡' : '打卡' }}
          </NButton>

          <NButton type="info" :disabled="!isCheckedIn || isCheckedOut" @click="handleCheckOut">
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
    <NCard title="考勤记录" :bordered="false" class="rounded-16px shadow-sm">
      <NDataTable
        :columns="columns"
        :data="attendanceList"
        :loading="loading"
        :pagination="{ pageSize: 10 }"
        class="mt-16px"
      />
    </NCard>

    <!-- 详情弹窗 -->
    <NModal v-model:show="showModal" preset="card" title="考勤详情" style="width: 600px">
      <NDescriptions v-if="selectedRecord" :column="2" bordered>
        <NDescriptionsItem label="日期">{{ selectedRecord.date }}</NDescriptionsItem>
        <NDescriptionsItem label="状态">
          <NTag :type="statusMap[selectedRecord.status].type as any">
            {{ statusMap[selectedRecord.status].text }}
          </NTag>
        </NDescriptionsItem>
        <NDescriptionsItem label="上班时间">{{ selectedRecord.checkInTime || '-' }}</NDescriptionsItem>
        <NDescriptionsItem label="下班时间">{{ selectedRecord.checkOutTime || '-' }}</NDescriptionsItem>
        <NDescriptionsItem label="位置">{{ selectedRecord.location || '-' }}</NDescriptionsItem>
        <NDescriptionsItem label="备注">{{ selectedRecord.notes || '-' }}</NDescriptionsItem>
      </NDescriptions>
    </NModal>
  </div>
</template>
