<script setup lang="ts">
import { computed, nextTick, onMounted, ref } from 'vue';
import { getRiderInfo } from '@/service/api/rider';
import {
  getMonthlyPerformanceOverview,
  getRiderPerformance,
  getRiderPerformanceRanking,
  getRiderPerformanceTrend
} from '@/service/api/rider-performance';
import { useEcharts } from '@/hooks/common/echarts';
import { useAuthStore } from '../../store/modules/auth';

const authStore = useAuthStore();

// 骑手ID - 使用登录用户的ID
const riderId = computed(() => authStore.userInfo.userId);

// 数据状态
const loading = ref(false);
const riderDetail = ref<Api.Rider.InfoData | null>(null);

// 绩效数据
const performanceData = ref<Api.Rider.TimeResponse | null>(null);
const performanceTrend = ref<Api.Rider.PerformanceTrendResponse | null>(null);
const performanceRanking = ref<Api.Rider.PerformanceRankingResponse | null>(null);
const monthlyOverview = ref<Api.Rider.PerformanceOverviewResponse | null>(null);

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

// 当前年月
const currentDate = new Date();
const currentYear = currentDate.getFullYear();
const currentMonth = currentDate.getMonth() + 1;

// 折线图配置
const { domRef: lineChartRef, updateOptions: updateLineChart } = useEcharts(() => ({
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
    data: ['完成订单', '准时率', '好评率']
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
    data: performanceTrend.value?.trendData?.map((item: any) => item.date) || []
  },
  yAxis: [
    {
      type: 'value',
      name: '订单数量',
      position: 'left'
    },
    {
      type: 'value',
      name: '百分比',
      position: 'right',
      min: 0,
      max: 100,
      axisLabel: {
        formatter: '{value}%'
      }
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
      data: performanceTrend.value?.trendData?.map((item: any) => item.completedOrders) || []
    },
    {
      color: '#26deca',
      name: '准时率',
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
      data: performanceTrend.value?.trendData?.map((item: any) => Math.round(item.onTimeRate * 100)) || []
    },
    {
      color: '#fedc69',
      name: '好评率',
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
            { offset: 0.25, color: '#fedc69' },
            { offset: 1, color: '#fff' }
          ]
        }
      },
      emphasis: { focus: 'series' },
      data: performanceTrend.value?.trendData?.map((item: any) => Math.round(item.goodReviewRate * 100)) || []
    }
  ]
}));

// 饼图配置
const { domRef: pieChartRef, updateOptions: updatePieChart } = useEcharts(() => ({
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
      data: [
        { name: '外卖配送', value: performanceData.value?.deliveryOrders || 0, color: '#5da8ff' },
        { name: '同城快递', value: performanceData.value?.expressOrders || 0, color: '#8e9dff' },
        { name: '生鲜配送', value: performanceData.value?.freshOrders || 0, color: '#fedc69' },
        { name: '其他', value: performanceData.value?.otherOrders || 0, color: '#26deca' }
      ]
    }
  ]
}));

// 计算属性
const completionRate = computed(() => {
  if (!performanceData.value) return 0;
  const total = performanceData.value.totalOrders || 0;
  const completed = performanceData.value.completedOrders || 0;
  return total > 0 ? Math.round((completed / total) * 100) : 0;
});

const onTimeRatePercent = computed(() => {
  return performanceData.value?.onTimeRate ? Math.round(performanceData.value.onTimeRate * 100) : 0;
});

const goodReviewRatePercent = computed(() => {
  return performanceData.value?.goodReviewRate
    ? Math.round(performanceData.value.goodReviewRate * 100)
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
  return [
    {
      key: 'completedOrders',
      title: '本月完成订单',
      value: performanceData.value?.completedOrders || 0,
      unit: '单',
      color: { start: '#ec4786', end: '#b955a4' },
      icon: 'mdi:truck-delivery'
    },
    {
      key: 'totalOrders',
      title: '本月总订单',
      value: performanceData.value?.totalOrders || 0,
      unit: '单',
      color: { start: '#865ec0', end: '#5144b4' },
      icon: 'mdi:clock-outline'
    },
    {
      key: 'completionRate',
      title: '完成率',
      value: completionRate.value,
      unit: '%',
      color: { start: '#56cdf3', end: '#719de3' },
      icon: 'mdi:chart-line'
    },
    {
      key: 'monthlyIncome',
      title: '本月收入',
      value: Math.round(performanceData.value?.income || 0),
      unit: '¥',
      color: { start: '#fcbc25', end: '#f68057' },
      icon: 'ant-design:money-collect-outlined'
    }
  ];
});

// 获取骑手详细信息
async function fetchRiderDetail() {
  try {
    const { data } = await getRiderInfo({ riderId: riderId.value });
    if (data) {
      riderDetail.value = data;
    }
  } catch (error) {
    console.error('获取骑手详细信息失败', error);
  }
}

// 获取骑手绩效数据
async function fetchRiderPerformance() {
  try {
    const params = {
      riderId: riderId.value,
      year: currentYear,
      month: currentMonth
    };

    const { data } = await getRiderPerformance(params);
    if (data) {
      performanceData.value = data;
    }
  } catch (error) {
    console.error('获取骑手绩效数据失败', error);
  }
}

// 获取骑手绩效趋势
async function fetchPerformanceTrend() {
  try {
    const params = {
      riderId: riderId.value,
      year: currentYear,
      month: currentMonth,
      days: 7 // 获取最近7天的趋势
    };

    const { data } = await getRiderPerformanceTrend(params);
    if (data) {
      performanceTrend.value = data;
      // 更新图表
      nextTick(() => {
        updateLineChart();
      });
    }
  } catch (error) {
    console.error('获取绩效趋势失败', error);
  }
}

// 获取骑手绩效排名
async function fetchPerformanceRanking() {
  try {
    const params = {
      riderId: riderId.value,
      year: currentYear,
      month: currentMonth
    };

    const { data } = await getRiderPerformanceRanking(params);
    if (data) {
      performanceRanking.value = data;
    }
  } catch (error) {
    console.error('获取绩效排名失败', error);
  }
}

// 获取月度绩效概览
async function fetchMonthlyOverview() {
  try {
    const params = {
      year: currentYear,
      month: currentMonth
    };

    const { data } = await getMonthlyPerformanceOverview(params);
    if (data) {
      monthlyOverview.value = data;
    }
  } catch (error) {
    console.error('获取月度概览失败', error);
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

  // 检查用户信息是否已初始化
  console.log('=== 骑手首页加载 ===');
  console.log('当前用户信息:', authStore.userInfo);
  console.log('当前token:', authStore.token);

  // 如果没有用户信息但有token，尝试初始化
  if (!authStore.userInfo.userId && authStore.token) {
    console.log('检测到token但用户信息为空，尝试初始化用户信息...');
    await authStore.initUserInfo();
    console.log('初始化后的用户信息:', authStore.userInfo);
  }

  // 如果仍然没有用户信息，使用模拟数据
  if (!authStore.userInfo.userId) {
    console.log('用户信息初始化失败，使用模拟数据');
    // 设置模拟用户信息
    Object.assign(authStore.userInfo, {
      userId: `rider_${Date.now()}`,
      userName: '测试骑手',
      roles: ['rider'],
      buttons: []
    });
    console.log('设置的模拟用户信息:', authStore.userInfo);
  }

  try {
    await Promise.all([
      fetchRiderDetail(),
      fetchRiderPerformance(),
      fetchPerformanceTrend(),
      fetchPerformanceRanking(),
      fetchMonthlyOverview(),
      fetchAttendanceStatus(),
      fetchWeatherInfo()
    ]);

    // 更新图表
    nextTick(() => {
      updateLineChart();
      updatePieChart();
    });
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
        <NCard title="本月绩效趋势" :bordered="false">
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
              <span class="font-semibold">{{ performanceData?.totalOrders || 0 }}单</span>
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
              本月已完成 {{ performanceData?.completedOrders || 0 }} 个订单，完成率 {{ completionRate }}%
            </div>
          </div>
        </div>

        <div class="flex items-center rounded-lg bg-green-50 p-12px dark:bg-green-900/20">
          <Icon name="mdi:check-circle" class="mr-12px text-green-500" />
          <div>
            <div class="text-green-800 font-medium dark:text-green-200">完成情况</div>
            <div class="text-sm text-green-600 dark:text-green-300">
              本月总订单 {{ performanceData?.totalOrders || 0 }} 单，收入 {{ Math.round(performanceData?.income || 0) }} 元
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
