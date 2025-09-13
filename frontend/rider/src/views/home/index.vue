<script setup lang="ts">
import { computed, nextTick, onMounted, ref } from 'vue';
import { getRiderInfo } from '@/service/api/rider';
import {
  generateRiderPerformance,
  getMonthlyPerformanceOverview,
  getRiderPerformance,
  getRiderPerformanceRanking,
  getRiderPerformanceTrend,
  getTopPerformers
} from '@/service/api/rider-performance';
import { useEcharts } from '@/hooks/common/echarts';
import SvgIcon from '@/components/custom/svg-icon.vue';
import { useAuthStore } from '../../store/modules/auth';
import { useRiderStore } from '../../store/modules/rider';

const authStore = useAuthStore();
const riderStore = useRiderStore();

// 骑手ID - 使用统一的 riderStore 获取
const riderId = computed(() => riderStore.riderId || authStore.userInfo.userId);

// 数据状态
const loading = ref(false);
const riderDetail = ref<Api.Rider.RiderInfoData | null>(null);

// 绩效数据
const performanceData = ref<Api.Rider.TimeData | null>(null);
const performanceTrend = ref<Api.Rider.PerformanceTrendData[] | null>(null);
const performanceRanking = ref<Api.Rider.RankingData | null>(null);
const monthlyOverview = ref<Api.Rider.OverviewData | null>(null);

// 优秀骑手排行榜数据
const topPerformers = ref<Api.Rider.PerformanceTrendData[]>([]);
const topPerformersLoading = ref(false);

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
    data: performanceTrend.value?.map((item: any) => item.statsMonth) || []
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
      data: performanceTrend.value?.map((item: any) => item.totalOrders) || []
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
      data: performanceTrend.value?.map((item: any) => Math.round(item.onTimeRate * 100)) || []
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
      data: performanceTrend.value?.map((item: any) => Math.round(item.goodReviewRate * 100)) || []
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
        { name: '已完成订单', value: performanceData.value?.totalOrders || 0, color: '#5da8ff' },
        {
          name: '准时订单',
          value: Math.round((performanceData.value?.totalOrders || 0) * (performanceData.value?.onTimeRate || 0)),
          color: '#8e9dff'
        },
        {
          name: '好评订单',
          value: Math.round((performanceData.value?.totalOrders || 0) * (performanceData.value?.goodReviewRate || 0)),
          color: '#fedc69'
        },
        {
          name: '其他',
          value: Math.round((performanceData.value?.totalOrders || 0) * (performanceData.value?.badReviewRate || 0)),
          color: '#26deca'
        }
      ]
    }
  ]
}));

// 计算属性
const completionRate = computed(() => {
  if (!performanceData.value) return 0;
  const total = performanceData.value.totalOrders || 0;
  // 由于API中没有completedOrders字段，我们使用onTimeRate作为完成率指标
  const completed = Math.round(total * (performanceData.value.onTimeRate || 0));
  return total > 0 ? Math.round((completed / total) * 100) : 0;
});

const onTimeRatePercent = computed(() => {
  return performanceData.value?.onTimeRate ? Math.round(performanceData.value.onTimeRate * 100) : 0;
});

const goodReviewRatePercent = computed(() => {
  return performanceData.value?.goodReviewRate ? Math.round(performanceData.value.goodReviewRate * 100) : 0;
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
      value: Math.round((performanceData.value?.totalOrders || 0) * (performanceData.value?.onTimeRate || 0)),
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
    const { data } = await getRiderInfo(riderId.value);
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
      performanceData.value = data.data;
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
      months: 7 // 获取最近7个月的趋势
    };

    const { data } = await getRiderPerformanceTrend(params);
    if (data) {
      performanceTrend.value = data.data;
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
      performanceRanking.value = data.data;
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
      monthlyOverview.value = data.data;
    }
  } catch (error) {
    console.error('获取月度概览失败', error);
  }
}

// 获取优秀骑手排行榜
async function fetchTopPerformers() {
  topPerformersLoading.value = true;
  try {
    const params = {
      year: currentYear,
      month: currentMonth,
      topCount: 10
    };

    const { data } = await getTopPerformers(params);
    if (data) {
      topPerformers.value = data.data || [];
    }
  } catch (error) {
    console.error('获取优秀骑手排行榜失败', error);
    topPerformers.value = [];
  } finally {
    topPerformersLoading.value = false;
  }
}

// 生成骑手月度绩效
async function generatePerformance() {
  try {
    const params = {
      riderId: riderId.value,
      year: currentYear,
      month: currentMonth
    };

    const { data } = await generateRiderPerformance(params);
    if (data) {
      window.$message?.success('绩效生成成功！');
      // 重新获取绩效数据
      await fetchRiderPerformance();
      await fetchPerformanceRanking();
    }
  } catch (error) {
    console.error('生成绩效失败', error);
    window.$message?.error('生成绩效失败，请稍后重试');
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
      fetchTopPerformers(),
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
              <SvgIcon :icon="item.icon" class="text-32px" />
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
        <NCard :bordered="false">
          <template #header>
            <div class="flex items-center justify-between">
              <span>本月绩效趋势</span>
              <NButton size="small" type="primary" @click="generatePerformance">
                <template #icon>
                  <SvgIcon icon="mdi:chart-line-variant" />
                </template>
                生成绩效
              </NButton>
            </div>
          </template>
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
                  <SvgIcon icon="mdi:clock-check" />
                </template>
                签到
              </NButton>
              <NButton v-if="attendanceStatus === '已签到'" type="info" size="small" block @click="handleCheckOut">
                <template #icon>
                  <SvgIcon icon="mdi:clock-out" />
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

      <!-- 优秀骑手排行榜 -->
      <NGi :span="8">
        <NCard title="优秀骑手排行榜" :bordered="false">
          <div v-if="topPerformersLoading" class="text-center py-16px">
            <NSpin size="small" />
            <div class="mt-8px text-sm text-gray-500">加载中...</div>
          </div>
          <div v-else-if="topPerformers.length > 0" class="space-y-8px">
            <div
              v-for="(rider, index) in topPerformers.slice(0, 5)"
              :key="rider.riderId || index"
              class="flex items-center justify-between p-8px rounded-lg bg-gray-50 dark:bg-gray-800"
            >
              <div class="flex items-center">
                <div
                  class="w-24px h-24px rounded-full flex items-center justify-center text-xs font-bold mr-8px"
                  :class="{
                    'bg-yellow-500 text-white': index === 0,
                    'bg-gray-400 text-white': index === 1,
                    'bg-orange-500 text-white': index === 2,
                    'bg-blue-500 text-white': index > 2
                  }"
                >
                  {{ index + 1 }}
                </div>
                <div>
                  <div class="text-sm font-medium">{{ rider.riderName || `骑手${index + 1}` }}</div>
                  <div class="text-xs text-gray-500">{{ rider.totalOrders || 0 }}单</div>
                </div>
              </div>
              <div class="text-right">
                <div class="text-sm font-semibold text-green-600">{{ Math.round((rider.onTimeRate || 0) * 100) }}%</div>
                <div class="text-xs text-gray-500">准时率</div>
              </div>
            </div>
            <div class="text-center pt-8px">
              <NButton size="small" type="primary" @click="fetchTopPerformers">
                <template #icon>
                  <SvgIcon icon="mdi:refresh" />
                </template>
                刷新排行榜
              </NButton>
            </div>
          </div>
          <div v-else class="text-center py-16px text-gray-500">
            <SvgIcon icon="mdi:trophy-outline" class="text-24px mb-8px" />
            <div class="text-sm">暂无排行榜数据</div>
            <NButton size="small" type="primary" class="mt-8px" @click="fetchTopPerformers">
              重新加载
            </NButton>
          </div>
        </NCard>
      </NGi>
    </NGrid>

    <!-- 今日工作提醒 -->
    <NCard title="今日工作提醒" :bordered="false">
      <div class="space-y-12px">
        <div class="flex items-center rounded-lg bg-blue-50 p-12px dark:bg-blue-900/20">
          <SvgIcon icon="mdi:information" class="mr-12px text-blue-500" />
          <div>
            <div class="text-blue-800 font-medium dark:text-blue-200">工作提醒</div>
            <div class="text-sm text-blue-600 dark:text-blue-300">
              本月已完成
              {{ Math.round((performanceData?.totalOrders || 0) * (performanceData?.onTimeRate || 0)) }} 个订单，完成率
              {{ completionRate }}%
            </div>
          </div>
        </div>

        <div class="flex items-center rounded-lg bg-green-50 p-12px dark:bg-green-900/20">
          <SvgIcon icon="mdi:check-circle" class="mr-12px text-green-500" />
          <div>
            <div class="text-green-800 font-medium dark:text-green-200">完成情况</div>
            <div class="text-sm text-green-600 dark:text-green-300">
              本月总订单 {{ performanceData?.totalOrders || 0 }} 单，收入 {{ Math.round(performanceData?.income || 0) }} 元
            </div>
          </div>
        </div>

        <div class="flex items-center rounded-lg bg-orange-50 p-12px dark:bg-orange-900/20">
          <SvgIcon icon="mdi:weather-sunny" class="mr-12px text-orange-500" />
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
