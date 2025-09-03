<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { getRiderInfo } from '@/service/api/rider';
import { useEcharts } from '@/hooks/common/echarts';

// 骑手ID
const riderId = '1663b73718a54c65b32f5b6787972949';

// 数据状态
const loading = ref(false);
const riderDetail = ref<Api.Rider.DetailData | null>(null);

// 考勤状态
const attendanceStatus = ref<'未签到' | '已签到' | '已签退'>('未签到');
const checkInTime = ref<string>('');
const checkOutTime = ref<string>('');

// 天气信息
const weatherInfo = ref({
  temperature: 25,
  condition: '晴天',
  humidity: 65,
  windSpeed: 3.2
});

// 模拟订单数据（用于图表）
const orderStats = ref([
  { date: '周一', completed: 8, pending: 3, income: 120 },
  { date: '周二', completed: 12, pending: 2, income: 180 },
  { date: '周三', completed: 10, pending: 4, income: 150 },
  { date: '周四', completed: 15, pending: 1, income: 225 },
  { date: '周五', completed: 18, pending: 2, income: 270 },
  { date: '周六', completed: 22, pending: 0, income: 330 },
  { date: '周日', completed: 16, pending: 3, income: 240 }
]);

// 模拟订单类型分布
const orderTypeStats = ref([
  { name: '外卖配送', value: 65, color: '#5da8ff' },
  { name: '同城快递', value: 20, color: '#8e9dff' },
  { name: '生鲜配送', value: 10, color: '#fedc69' },
  { name: '其他', value: 5, color: '#26deca' }
]);

// 折线图配置
const { domRef: lineChartRef, updateOptions: _updateLineChart } = useEcharts(() => ({
  tooltip: {
    trigger: 'axis',
    axisPointer: {
      type: 'cross',
      label: {
        backgroundColor: '#6a7985'
      }
    }
  },
  legend: {
    data: ['完成订单', '待处理订单', '收入']
  },
  grid: {
    left: '3%',
    right: '4%',
    bottom: '3%',
    containLabel: true
  },
  xAxis: {
    type: 'category',
    boundaryGap: false,
    data: orderStats.value.map(item => item.date)
  },
  yAxis: [
    {
      type: 'value',
      name: '订单数量',
      position: 'left'
    },
    {
      type: 'value',
      name: '收入 (¥)',
      position: 'right'
    }
  ],
  series: [
    {
      color: '#8e9dff',
      name: '完成订单',
      type: 'line',
      smooth: true,
      areaStyle: {
        color: {
          type: 'linear',
          x: 0,
          y: 0,
          x2: 0,
          y2: 1,
          colorStops: [
            { offset: 0.25, color: '#8e9dff' },
            { offset: 1, color: '#fff' }
          ]
        }
      },
      emphasis: { focus: 'series' },
      data: orderStats.value.map(item => item.completed)
    },
    {
      color: '#ff7875',
      name: '待处理订单',
      type: 'line',
      smooth: true,
      areaStyle: {
        color: {
          type: 'linear',
          x: 0,
          y: 0,
          x2: 0,
          y2: 1,
          colorStops: [
            { offset: 0.25, color: '#ff7875' },
            { offset: 1, color: '#fff' }
          ]
        }
      },
      emphasis: { focus: 'series' },
      data: orderStats.value.map(item => item.pending)
    },
    {
      color: '#26deca',
      name: '收入',
      type: 'line',
      smooth: true,
      yAxisIndex: 1,
      areaStyle: {
        color: {
          type: 'linear',
          x: 0,
          y: 0,
          x2: 0,
          y2: 1,
          colorStops: [
            { offset: 0.25, color: '#26deca' },
            { offset: 1, color: '#fff' }
          ]
        }
      },
      emphasis: { focus: 'series' },
      data: orderStats.value.map(item => item.income)
    }
  ]
}));

// 饼图配置
const { domRef: pieChartRef, updateOptions: _updatePieChart } = useEcharts(() => ({
  tooltip: {
    trigger: 'item',
    formatter: '{a} <br/>{b}: {c}单 ({d}%)'
  },
  legend: {
    bottom: '1%',
    left: 'center',
    itemStyle: { borderWidth: 0 }
  },
  series: [
    {
      color: ['#5da8ff', '#8e9dff', '#fedc69', '#26deca'],
      name: '订单类型',
      type: 'pie',
      radius: ['45%', '75%'],
      avoidLabelOverlap: false,
      itemStyle: {
        borderRadius: 10,
        borderColor: '#fff',
        borderWidth: 1
      },
      label: {
        show: false,
        position: 'center'
      },
      emphasis: {
        label: {
          show: true,
          fontSize: '12'
        }
      },
      labelLine: { show: false },
      data: orderTypeStats.value
    }
  ]
}));

// 计算属性
const completionRate = computed(() => {
  if (!riderDetail.value) return 0;
  const total = riderDetail.value.currentTaskCount + riderDetail.value.todayCompletedOrders;
  return total > 0 ? Math.round((riderDetail.value.todayCompletedOrders / total) * 100) : 0;
});

const onTimeRatePercent = computed(() => {
  return riderDetail.value?.performance?.onTimeRate ? Math.round(riderDetail.value.performance.onTimeRate * 100) : 0;
});

const goodReviewRatePercent = computed(() => {
  return riderDetail.value?.performance?.goodReviewRate
    ? Math.round(riderDetail.value.performance.goodReviewRate * 100)
    : 0;
});

const attendanceStatusColor = computed(() => {
  switch (attendanceStatus.value) {
    case '已签到':
      return 'success';
    case '已签退':
      return 'info';
    default:
      return 'warning';
  }
});

// 统计卡片数据
const cardData = computed(() => {
  const completionRateValue = completionRate.value;
  return [
    {
      key: 'todayCompleted',
      title: '今日完成订单',
      value: riderDetail.value?.todayCompletedOrders || 0,
      unit: '单',
      color: { start: '#ec4786', end: '#b955a4' },
      icon: 'mdi:truck-delivery'
    },
    {
      key: 'currentTask',
      title: '当前任务',
      value: riderDetail.value?.currentTaskCount || 0,
      unit: '单',
      color: { start: '#865ec0', end: '#5144b4' },
      icon: 'mdi:clock-outline'
    },
    {
      key: 'completionRate',
      title: '完成率',
      value: completionRateValue,
      unit: '%',
      color: { start: '#56cdf3', end: '#719de3' },
      icon: 'mdi:chart-line'
    },
    {
      key: 'monthlyIncome',
      title: '本月收入',
      value: Math.round(riderDetail.value?.performance?.income || 0),
      unit: '¥',
      color: { start: '#fcbc25', end: '#f68057' },
      icon: 'ant-design:money-collect-outlined'
    }
  ];
});

// 获取骑手详细信息
async function fetchRiderDetail() {
  try {
    const { data } = await getRiderInfo({ riderId });
    if (data) {
      // 使用基本骑手信息，添加模拟的绩效数据
      riderDetail.value = {
        ...data,
        currentTaskCount: 3,
        todayCompletedOrders: 8,
        performance: {
          statsMonth: '2024-01',
          totalOrders: 156,
          onTimeRate: 0.95,
          goodReviewRate: 0.92,
          badReviewRate: 0.03,
          income: 4560.5
        }
      };
    }
  } catch (error) {
    console.error('获取骑手详细信息失败', error);
    // 使用模拟数据
    riderDetail.value = {
      applicationUserId: 'mock_user_001',
      name: '测试骑手',
      phoneNumber: '13800138000',
      riderId,
      vehicleNumber: '宁A12345',
      currentTaskCount: 3,
      todayCompletedOrders: 8,
      performance: {
        statsMonth: '2024-01',
        totalOrders: 156,
        onTimeRate: 0.95,
        goodReviewRate: 0.92,
        badReviewRate: 0.03,
        income: 4560.5
      }
    };
  }
}

// 模拟获取考勤状态
function fetchAttendanceStatus() {
  const now = new Date();
  const currentHour = now.getHours();

  // 模拟考勤逻辑
  if (currentHour < 8) {
    attendanceStatus.value = '未签到';
    checkInTime.value = '';
    checkOutTime.value = '';
  } else if (currentHour >= 8 && currentHour < 18) {
    attendanceStatus.value = '已签到';
    checkInTime.value = '08:30';
    checkOutTime.value = '';
  } else {
    attendanceStatus.value = '已签退';
    checkInTime.value = '08:30';
    checkOutTime.value = '18:30';
  }
}

// 模拟获取天气信息
function fetchWeatherInfo() {
  // 这里可以接入真实的天气API
  weatherInfo.value = {
    temperature: Math.floor(Math.random() * 15) + 20, // 20-35度
    condition: ['晴天', '多云', '阴天', '小雨'][Math.floor(Math.random() * 4)],
    humidity: Math.floor(Math.random() * 30) + 50, // 50-80%
    windSpeed: Math.random() * 5 + 1 // 1-6级风
  };
}

// 签到功能
function handleCheckIn() {
  if (attendanceStatus.value === '未签到') {
    attendanceStatus.value = '已签到';
    checkInTime.value = new Date().toLocaleTimeString('zh-CN', {
      hour: '2-digit',
      minute: '2-digit'
    });
    window.$message?.success('签到成功！');
  } else {
    window.$message?.warning('您已经签到过了！');
  }
}

// 签退功能
function handleCheckOut() {
  if (attendanceStatus.value === '已签到') {
    attendanceStatus.value = '已签退';
    checkOutTime.value = new Date().toLocaleTimeString('zh-CN', {
      hour: '2-digit',
      minute: '2-digit'
    });
    window.$message?.success('签退成功！');
  } else if (attendanceStatus.value === '未签到') {
    window.$message?.warning('请先签到！');
  } else {
    window.$message?.warning('您已经签退过了！');
  }
}

// 获取渐变色
function getGradientColor(color: { start: string; end: string }) {
  return `linear-gradient(to bottom right, ${color.start}, ${color.end})`;
}

// 页面加载
onMounted(async () => {
  loading.value = true;
  try {
    await Promise.all([fetchRiderDetail(), fetchAttendanceStatus(), fetchWeatherInfo()]);
  } finally {
    loading.value = false;
  }
});
</script>

<template>
  <div class="h-full p-24px">
    <!-- 欢迎横幅 -->
    <NCard :bordered="false" class="mb-24px">
      <div class="flex items-center justify-between">
        <div class="flex-1">
          <h1 class="text-2xl text-gray-800 font-bold dark:text-gray-200">
            欢迎回来，{{ riderDetail?.name || '骑手' }}！
          </h1>
          <p class="mt-8px text-gray-600 dark:text-gray-400">
            今天是 {{ new Date().toLocaleDateString('zh-CN') }}，祝您工作顺利！
          </p>
        </div>
        <div class="flex items-center space-x-16px">
          <!-- 天气信息 -->
          <div class="text-center">
            <div class="text-sm text-gray-500">天气</div>
            <div class="text-lg font-semibold">{{ weatherInfo.temperature }}°C</div>
            <div class="text-xs text-gray-400">{{ weatherInfo.condition }}</div>
          </div>
          <!-- 车辆信息 -->
          <div class="text-center">
            <div class="text-sm text-gray-500">车辆编号</div>
            <div class="text-lg font-semibold">{{ riderDetail?.vehicleNumber || '暂无' }}</div>
          </div>
        </div>
      </div>
    </NCard>

    <!-- 统计卡片 -->
    <NCard :bordered="false" size="small" class="mb-24px">
      <NGrid cols="s:1 m:2 l:4" responsive="screen" :x-gap="16" :y-gap="16">
        <NGi v-for="item in cardData" :key="item.key">
          <div
            class="flex-1 rd-8px px-16px pb-4px pt-8px text-white"
            :style="{ backgroundImage: getGradientColor(item.color) }"
          >
            <h3 class="text-20px font-semibold">{{ item.title }}</h3>
            <div class="flex justify-between pt-12px">
              <Icon :name="item.icon" class="text-32px" />
              <div class="text-28px text-white dark:text-dark">
                <span v-if="item.unit === '¥'">{{ item.unit }}</span>
                {{ item.value }}
                <span v-if="item.unit !== '¥'">{{ item.unit }}</span>
              </div>
            </div>
          </div>
        </NGi>
      </NGrid>
    </NCard>

    <!-- 图表区域 -->
    <NGrid :cols="24" :x-gap="16" :y-gap="16" class="mb-24px">
      <NGi :span="16">
        <NCard title="本周订单趋势" :bordered="false">
          <div ref="lineChartRef" class="h-360px overflow-hidden"></div>
        </NCard>
      </NGi>
      <NGi :span="8">
        <NCard title="订单类型分布" :bordered="false">
          <div ref="pieChartRef" class="h-360px overflow-hidden"></div>
        </NCard>
      </NGi>
    </NGrid>

    <!-- 考勤状态和绩效指标 -->
    <NGrid :cols="24" :x-gap="16" :y-gap="16" class="mb-24px">
      <!-- 考勤状态 -->
      <NGi :span="8">
        <NCard title="今日考勤" :bordered="false">
          <div class="text-center">
            <NTag :type="attendanceStatusColor" size="large" class="mb-16px">
              {{ attendanceStatus }}
            </NTag>
            <div class="text-sm space-y-8px">
              <div v-if="checkInTime" class="flex justify-between">
                <span class="text-gray-500">签到时间：</span>
                <span class="font-medium">{{ checkInTime }}</span>
              </div>
              <div v-if="checkOutTime" class="flex justify-between">
                <span class="text-gray-500">签退时间：</span>
                <span class="font-medium">{{ checkOutTime }}</span>
              </div>
            </div>
            <div class="mt-16px space-y-8px">
              <NButton v-if="attendanceStatus === '未签到'" type="success" size="small" block @click="handleCheckIn">
                <template #icon>
                  <Icon name="mdi:clock-check" />
                </template>
                签到
              </NButton>
              <NButton v-if="attendanceStatus === '已签到'" type="info" size="small" block @click="handleCheckOut">
                <template #icon>
                  <Icon name="mdi:clock-out" />
                </template>
                签退
              </NButton>
            </div>
          </div>
        </NCard>
      </NGi>

      <!-- 绩效指标 -->
      <NGi :span="8">
        <NCard title="绩效指标" :bordered="false">
          <NSpace vertical :size="16">
            <div class="flex items-center justify-between">
              <span>准时率</span>
              <span class="text-green-500 font-semibold">{{ onTimeRatePercent }}%</span>
            </div>
            <NProgress type="line" :percentage="onTimeRatePercent" color="#10b981" :show-indicator="false" />

            <div class="flex items-center justify-between">
              <span>好评率</span>
              <span class="text-blue-500 font-semibold">{{ goodReviewRatePercent }}%</span>
            </div>
            <NProgress type="line" :percentage="goodReviewRatePercent" color="#3b82f6" :show-indicator="false" />

            <div class="flex items-center justify-between">
              <span>本月总订单</span>
              <span class="font-semibold">{{ riderDetail?.performance?.totalOrders || 0 }}单</span>
            </div>
          </NSpace>
        </NCard>
      </NGi>

      <!-- 快速操作 -->
      <NGi :span="8">
        <NCard title="快速操作" :bordered="false">
          <NSpace vertical :size="12">
            <NButton type="primary" block @click="$router.push('/delivery')">
              <template #icon>
                <Icon name="mdi:truck-delivery" />
              </template>
              查看配送订单
            </NButton>
            <NButton type="info" block @click="$router.push('/profile')">
              <template #icon>
                <Icon name="mdi:account-edit" />
              </template>
              编辑个人信息
            </NButton>
            <NButton type="success" block @click="$router.push('/attendance')">
              <template #icon>
                <Icon name="mdi:calendar-clock" />
              </template>
              考勤记录
            </NButton>
            <NButton type="warning" block @click="$router.push('/performance')">
              <template #icon>
                <Icon name="mdi:chart-line" />
              </template>
              绩效统计
            </NButton>
          </NSpace>
        </NCard>
      </NGi>
    </NGrid>

    <!-- 今日工作提醒 -->
    <NCard title="今日工作提醒" :bordered="false">
      <div class="space-y-12px">
        <div class="flex items-center rounded-lg bg-blue-50 p-12px dark:bg-blue-900/20">
          <Icon name="mdi:information" class="mr-12px text-blue-500" />
          <div>
            <div class="text-blue-800 font-medium dark:text-blue-200">工作提醒</div>
            <div class="text-sm text-blue-600 dark:text-blue-300">
              今日有 {{ riderDetail?.currentTaskCount || 0 }} 个待处理订单，请及时处理
            </div>
          </div>
        </div>

        <div class="flex items-center rounded-lg bg-green-50 p-12px dark:bg-green-900/20">
          <Icon name="mdi:check-circle" class="mr-12px text-green-500" />
          <div>
            <div class="text-green-800 font-medium dark:text-green-200">完成情况</div>
            <div class="text-sm text-green-600 dark:text-green-300">
              今日已完成 {{ riderDetail?.todayCompletedOrders || 0 }} 个订单，完成率 {{ completionRate }}%
            </div>
          </div>
        </div>

        <div class="flex items-center rounded-lg bg-orange-50 p-12px dark:bg-orange-900/20">
          <Icon name="mdi:weather-sunny" class="mr-12px text-orange-500" />
          <div>
            <div class="text-orange-800 font-medium dark:text-orange-200">天气提醒</div>
            <div class="text-sm text-orange-600 dark:text-orange-300">
              今日{{ weatherInfo.condition }}，温度{{ weatherInfo.temperature }}°C，注意防晒和保暖
            </div>
          </div>
        </div>
      </div>
    </NCard>
  </div>
</template>

<style scoped>
.n-card {
  background-color: var(--n-color);
  transition: all 0.3s ease;
}

.n-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.1);
}

.n-button {
  transition: all 0.3s ease;
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
}

.n-button:hover {
  transform: translateY(-1px);
}

/* 确保按钮内容居中 */
.n-button .n-button__content {
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
  width: 100% !important;
}

/* 更具体的按钮样式覆盖 */
.n-button .n-button__content .n-button__text {
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
}

/* 确保图标和文字都居中 */
.n-button .n-button__content .n-button__icon {
  margin-right: 4px !important;
}

/* 提醒卡片样式 */
.reminder-card {
  border-left: 4px solid;
  transition: all 0.3s ease;
}

.reminder-card:hover {
  transform: translateX(4px);
}

/* 统计卡片渐变效果 */
.rd-8px {
  border-radius: 8px;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .text-30px {
    font-size: 24px;
  }

  .text-32px {
    font-size: 28px;
  }
}
</style>
