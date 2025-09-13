<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { getPerformanceOverview } from '@/api/rider';
import { $t } from '@/locales';

defineOptions({
  name: 'PerformanceOverview'
});

interface OverviewData {
  key: string;
  title: string;
  value: number;
  precision?: number;
  unit: string;
  color: {
    start: string;
    end: string;
  };
  icon: string;
}

const overviewData = ref<OverviewData[]>([]);
const loading = ref(true);

async function fetchPerformanceOverview() {
  loading.value = true;
  try {
    const today = new Date();
    const year = today.getFullYear();
    const month = today.getMonth() + 1;
    const { data } = await getPerformanceOverview(year, month);

    if (data) {
      overviewData.value = [
        {
          key: 'totalRiders',
          title: '总骑手数',
          value: data.TotalRiders || 0,
          unit: '人',
          color: { start: '#ec4786', end: '#b955a4' },
          icon: 'ant-design:team-outlined'
        },
        {
          key: 'totalOrders',
          title: '总订单量',
          value: data.TotalOrders || 0,
          unit: '单',
          color: { start: '#865ec0', end: '#5144b4' },
          icon: 'ant-design:profile-outlined'
        },
        {
          key: 'avgOnTimeRate',
          title: '平均准时率',
          value: (data.AverageOnTimeRate || 0) * 100,
          precision: 2,
          unit: '%',
          color: { start: '#56cdf3', end: '#719de3' },
          icon: 'ant-design:clock-circle-outlined'
        },
        {
          key: 'avgGoodReviewRate',
          title: '平均好评率',
          value: (data.AverageGoodReviewRate || 0) * 100,
          precision: 2,
          unit: '%',
          color: { start: '#fcbc25', end: '#f68057' },
          icon: 'ant-design:star-outlined'
        }
      ];
    }
  } catch (error) {
    console.error('Failed to fetch performance overview:', error);
    // 可选：设置默认或错误状态的数据
  } finally {
    loading.value = false;
  }
}

function getGradientColor(color: OverviewData['color']) {
  return `linear-gradient(135deg, ${color.start}, ${color.end})`;
}

function formatUpdateTime() {
  return new Date().toLocaleTimeString('zh-CN', { 
    hour: '2-digit', 
    minute: '2-digit' 
  });
}

onMounted(() => {
  fetchPerformanceOverview();
});
</script>

<template>
  <NCard 
    :title="$t('page.home.performanceOverview.title')" 
    :bordered="false" 
    size="small" 
    class="card-wrapper overflow-hidden"
  >


    <div v-if="loading" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
      <div v-for="i in 4" :key="i" class="h-32 rounded-xl">
        <NSkeleton height="128px" class="rounded-xl" />
      </div>
    </div>

    <div v-else-if="overviewData.length === 0" class="text-center py-12">
      <SvgIcon icon="material-symbols:error-outline" class="text-4xl text-gray-400 mb-3" />
      <p class="text-gray-500">暂无平台数据</p>
      <NButton size="small" @click="fetchPerformanceOverview" class="mt-3">
        重新加载
      </NButton>
    </div>

    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      <div 
        v-for="(item, index) in overviewData" 
        :key="item.key"
        class="relative overflow-hidden rounded-2xl p-6 text-white shadow-lg hover:shadow-xl transform hover:scale-105 transition-all duration-300"
        :style="{ backgroundImage: getGradientColor(item.color) }"
      >
        <!-- 装饰圆圈 -->
        <div class="absolute -top-4 -right-4 w-16 h-16 bg-white/10 rounded-full"></div>
        <div class="absolute -bottom-2 -left-2 w-8 h-8 bg-white/10 rounded-full"></div>
        
        <div class="relative z-10">
          <div class="flex items-center justify-between mb-4">
            <div class="p-2 bg-white/20 rounded-xl backdrop-blur-sm">
              <SvgIcon :icon="item.icon" class="text-2xl" />
            </div>
            <div class="text-right">
              <div class="text-xs opacity-80">{{ formatUpdateTime() }}</div>
            </div>
          </div>
          
          <div class="space-y-2">
            <h3 class="text-sm font-medium opacity-90">{{ item.title }}</h3>
            <div class="flex items-baseline space-x-1">
              <CountTo
                :suffix="item.unit"
                :start-value="0"
                :end-value="item.value"
                :precision="item.precision || 0"
                class="text-2xl font-bold"
                :duration="2000 + index * 300"
              />
            </div>
          </div>
        </div>
      </div>
    </div>
  </NCard>
</template>

<style scoped></style>
