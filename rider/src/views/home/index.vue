<script setup lang="ts">
import { computed, nextTick, onMounted, onUnmounted, ref, watch } from 'vue';
import { getRiderInfo } from '@/service/api/rider';
import {
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

// 监控绩效数据变化
watch(performanceData, (newData, oldData) => {
  console.log('绩效数据变化:', {
    old: oldData,
    new: newData
  });
}, { deep: true });
const performanceTrend = ref<Api.Rider.PerformanceTrendData[] | null>(null);
const performanceRanking = ref<Api.Rider.RankingData | null>(null);
const monthlyOverview = ref<Api.Rider.OverviewData | null>(null);

// 优秀骑手排行榜数据
const topPerformers = ref<Api.Rider.PerformanceTrendData[]>([]);
const topPerformersLoading = ref(false);


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
        { name: '总订单', value: performanceData.value?.totalOrders || 0, color: '#5da8ff' },
        { name: '准时率', value: performanceData.value?.onTimeRate || 0, color: '#8e9dff' },
        { name: '好评率', value: performanceData.value?.goodReviewRate || 0, color: '#fedc69' },
        { name: '差评率', value: performanceData.value?.badReviewRate || 0, color: '#26deca' }
      ]
    }
  ]
}));

// 直接使用原始数据，不做复杂计算
const onTimeRatePercent = computed(() => {
  return performanceData.value?.onTimeRate || 0;
});

const goodReviewRatePercent = computed(() => {
  return performanceData.value?.goodReviewRate || 0;
});

const badReviewRatePercent = computed(() => {
  return performanceData.value?.badReviewRate || 0;
});


// 统计卡片数据 - 直接使用原始数据
const cardData = computed(() => {
  const data = performanceData.value;
  if (!data) return [];
  
  return [
    {
      key: 'totalOrders',
      title: '本月总订单',
      value: data.totalOrders || 0,
      unit: '单',
      color: { start: '#ff6b6b', end: '#ee5a24' },
      icon: 'mdi:package-variant'
    },
    {
      key: 'onTimeRate',
      title: '准时率',
      value: data.onTimeRate || 0,
      unit: '%',
      color: { start: '#00d2ff', end: '#3a7bd5' },
      icon: 'mdi:clock-check'
    },
    {
      key: 'goodReviewRate',
      title: '好评率',
      value: data.goodReviewRate || 0,
      unit: '%',
      color: { start: '#11998e', end: '#38ef7d' },
      icon: 'mdi:thumb-up'
    },
    {
      key: 'income',
      title: '本月收入',
      value: data.income || 0,
      unit: '¥',
      color: { start: '#ffd700', end: '#ffb347' },
      icon: 'mdi:currency-cny'
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
    console.log('获取绩效数据参数:', params);

    const { data } = await getRiderPerformance(params);
    console.log('骑手绩效数据原始响应:', data);
    console.log('骑手绩效数据结构:', {
      code: data?.code,
      message: data?.message,
      data: data?.data,
      timestamp: data?.timestamp
    });
    if (data && data.data) {
      performanceData.value = data.data;
      console.log('绩效数据已赋值:', performanceData.value);
      console.log('原始数据:', {
        onTimeRate: performanceData.value.onTimeRate,
        goodReviewRate: performanceData.value.goodReviewRate,
        badReviewRate: performanceData.value.badReviewRate,
        totalOrders: performanceData.value.totalOrders,
        income: performanceData.value.income
      });
    } else if (data && !data.data) {
      // 如果 data.data 为 null，但 data 有值，直接使用 data
      performanceData.value = data as any;
      console.log('使用 data 作为绩效数据:', performanceData.value);
    } else {
      console.warn('绩效数据为空或格式不正确:', data);
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
    if (data && data.data) {
      performanceTrend.value = data.data;
    } else if (data && !data.data) {
      performanceTrend.value = data as any;
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
    if (data && data.data) {
      performanceRanking.value = data.data;
    } else if (data && !data.data) {
      performanceRanking.value = data as any;
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
    if (data && data.data) {
      monthlyOverview.value = data.data;
    } else if (data && !data.data) {
      monthlyOverview.value = data as any;
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
    if (data && data.data) {
      topPerformers.value = data.data || [];
    } else if (data && !data.data) {
      topPerformers.value = (data as any) || [];
    }
  } catch (error) {
    console.error('获取优秀骑手排行榜失败', error);
    topPerformers.value = [];
  } finally {
    topPerformersLoading.value = false;
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


// 获取渐变色
function getGradientColor(color: { start: string; end: string }) {
  return `linear-gradient(to bottom right, ${color.start}, ${color.end})`;
}

// 自动刷新定时器
let refreshTimer: NodeJS.Timeout | null = null;

// 刷新所有数据
async function refreshAllData() {
  if (loading.value) return;
  
  console.log('🔄 开始刷新首页数据...');
  loading.value = true;
  
  try {
    await Promise.all([
      fetchRiderDetail(),
      fetchRiderPerformance(),
      fetchPerformanceTrend(),
      fetchPerformanceRanking(),
      fetchMonthlyOverview(),
      fetchTopPerformers()
    ]);

    // 更新图表
    nextTick(() => {
      updateLineChart();
      updatePieChart();
    });
    
    console.log('✅ 首页数据刷新完成');
  } catch (error) {
    console.error('❌ 刷新首页数据失败:', error);
  } finally {
    loading.value = false;
  }
}

// 启动自动刷新
function startAutoRefresh() {
  // 每5分钟自动刷新一次数据
  refreshTimer = setInterval(() => {
    refreshAllData();
  }, 5 * 60 * 1000);
  console.log('🔄 已启动自动刷新，间隔: 5分钟');
}

// 停止自动刷新
function stopAutoRefresh() {
  if (refreshTimer) {
    clearInterval(refreshTimer);
    refreshTimer = null;
    console.log('⏹️ 已停止自动刷新');
  }
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
      fetchWeatherInfo()
    ]);

    // 更新图表
    nextTick(() => {
      updateLineChart();
      updatePieChart();
    });
    
    // 启动自动刷新
    startAutoRefresh();
  } finally {
    loading.value = false;
  }
});

// 页面卸载时清理定时器
onUnmounted(() => {
  stopAutoRefresh();
});
</script>

<template>
  <div class="min-h-full p-24px bg-gradient-to-br from-blue-50 to-indigo-100 dark:from-gray-900 dark:to-gray-800">
    <!-- 欢迎横幅 -->
    <NCard :bordered="false" class="mb-24px bg-white/80 dark:bg-gray-800/80 backdrop-blur-sm">
      <div class="flex items-center gap-3">
        <div class="w-12 h-12 bg-gradient-to-r from-blue-500 to-purple-600 rounded-xl flex items-center justify-center">
          <SvgIcon :local-icon="'home'" class="text-2xl text-white" />
        </div>
        <div class="flex-1">
          <h1 class="text-2xl text-gray-800 font-bold dark:text-gray-200">
            欢迎回来，{{ riderDetail?.name || '骑手' }}！
          </h1>
          <p class="mt-2px text-gray-600 dark:text-gray-400">
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
          <!-- 刷新按钮 -->
          <NButton 
            type="primary" 
            size="medium" 
            :loading="loading"
            @click="refreshAllData"
            class="bg-gradient-to-r from-blue-500 to-purple-600 hover:from-blue-600 hover:to-purple-700"
          >
            <template #icon>
              <SvgIcon icon="mdi:refresh" />
            </template>
            刷新数据
          </NButton>
        </div>
      </div>
    </NCard>

    <!-- 统计卡片 -->
    <NCard :bordered="false" size="small" class="mb-24px bg-white/90 dark:bg-gray-800/90 backdrop-blur-sm rounded-16px shadow-lg">
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
        <NCard :bordered="false" class="rounded-16px shadow-lg bg-white/90 dark:bg-gray-800/90 backdrop-blur-sm">
          <template #header>
            <span>本月绩效趋势</span>
          </template>
          <div ref="lineChartRef" class="h-360px overflow-hidden"></div>
        </NCard>
      </NGi>
      <NGi :span="8">
        <NCard title="订单类型分布" :bordered="false" class="rounded-16px shadow-lg bg-white/90 dark:bg-gray-800/90 backdrop-blur-sm">
          <div ref="pieChartRef" class="h-360px overflow-hidden"></div>
        </NCard>
      </NGi>
    </NGrid>

    <!-- 绩效指标和排行榜 -->
    <NGrid :cols="24" :x-gap="16" :y-gap="16" class="mb-24px">
      <!-- 绩效指标 -->
      <NGi :span="12">
        <NCard title="绩效指标" :bordered="false" class="rounded-16px shadow-lg bg-white/90 dark:bg-gray-800/90 backdrop-blur-sm">
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
      <NGi :span="12">
        <NCard title="优秀骑手排行榜" :bordered="false" class="rounded-16px shadow-lg bg-white/90 dark:bg-gray-800/90 backdrop-blur-sm">
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
              本月总订单
              {{ performanceData?.totalOrders || 0 }} 单，准时率
              {{ performanceData?.onTimeRate || 0 }}%
            </div>
          </div>
        </div>

        <div class="flex items-center rounded-lg bg-green-50 p-12px dark:bg-green-900/20">
          <SvgIcon icon="mdi:check-circle" class="mr-12px text-green-500" />
          <div>
            <div class="text-green-800 font-medium dark:text-green-200">完成情况</div>
            <div class="text-sm text-green-600 dark:text-green-300">
              本月总订单 {{ performanceData?.totalOrders || 0 }} 单，收入 {{ performanceData?.income || 0 }} 元
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

        <div class="flex items-center rounded-lg bg-purple-50 p-12px dark:bg-purple-900/20">
          <SvgIcon icon="mdi:chart-line" class="mr-12px text-purple-500" />
          <div>
            <div class="text-purple-800 font-medium dark:text-purple-200">月度概览</div>
            <div class="text-sm text-purple-600 dark:text-purple-300">
              本月共有 {{ monthlyOverview?.totalRiders || 0 }} 名骑手，平均完成率 {{ Math.round((monthlyOverview?.averageCompletionRate || 0) * 100) }}%
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

/* 页面整体动画 */
.min-h-full {
  animation: fadeIn 0.6s ease-out;
  min-height: 180vh;
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
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1), 0 0 0 1px rgba(255, 255, 255, 0.05);
}

/* 统计卡片动画 */
.grid > div,
.n-grid .n-gi > div {
  transition: all 0.3s ease;
  cursor: pointer;
}

.grid > div:hover,
.n-grid .n-gi > div:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.1);
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

/* 渐变按钮特殊样式 */
.n-button.bg-gradient-to-r {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border: none;
  color: white;
}

.n-button.bg-gradient-to-r:hover {
  background: linear-gradient(135deg, #5a67d8 0%, #6b46c1 100%);
}

/* 确保按钮内容居中 */
.n-button .n-button__content {
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
  width: 100% !important;
  gap: 8px;
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

/* 进度条动画 */
.n-progress .n-progress-line-fill {
  transition: width 0.8s ease-in-out;
}

/* 标签动画 */
.n-tag {
  transition: all 0.3s ease;
  border-radius: 20px;
  font-weight: 500;
  padding: 4px 12px;
}

.n-tag:hover {
  transform: scale(1.05);
}

/* 图表容器动画 */
.h-360px {
  transition: all 0.3s ease;
}

.h-360px:hover {
  transform: scale(1.02);
}

/* 排行榜项目动画 */
.space-y-8px > div {
  transition: all 0.3s ease;
  border-radius: 8px;
}

.space-y-8px > div:hover {
  transform: translateX(4px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  background-color: rgba(59, 130, 246, 0.05);
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

/* 数字动画 */
.text-28px,
.text-20px {
  transition: all 0.3s ease;
}

.text-28px:hover,
.text-20px:hover {
  transform: scale(1.1);
  color: #3b82f6;
}

/* 图标动画 */
.svg-icon {
  transition: all 0.3s ease;
}

.svg-icon:hover {
  transform: rotate(10deg) scale(1.1);
}

/* 天气信息动画 */
.text-center > div {
  transition: all 0.3s ease;
}

.text-center > div:hover {
  transform: translateY(-2px);
  color: #3b82f6;
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
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
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

  .text-30px {
    font-size: 24px;
  }

  .text-32px {
    font-size: 28px;
  }
}
</style>
