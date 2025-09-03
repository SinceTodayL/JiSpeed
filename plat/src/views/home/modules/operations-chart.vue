<script setup lang="ts">
import { onMounted, ref, computed } from 'vue';
import { NDatePicker, NButton, NSpace, NTag, NStatistic, NGrid, NGi, NSpin } from 'naive-ui';
import { useEcharts } from '@/hooks/common/echarts';
import { getRecentOperations } from '@/api/rider';
import { formatDate } from '@/utils/common';

defineOptions({
  name: 'OperationsChart'
});

interface OperationsData {
  orderQuantity: number;
  cancelledOrderQuantity: number;
  aftersalesCompletedOrderQuantity: number;
  userQuantity: number;
  merchantQuantity: number;
  riderQuantity: number;
}

const loading = ref(true);
const dateRange = ref<[number, number]>([
  Date.now() - 30 * 24 * 60 * 60 * 1000, // 30天前
  Date.now()
]);

const operationsData = ref<OperationsData>({
  orderQuantity: 0,
  cancelledOrderQuantity: 0,
  aftersalesCompletedOrderQuantity: 0,
  userQuantity: 0,
  merchantQuantity: 0,
  riderQuantity: 0
});

// 计算统计数据
const statistics = computed(() => [
  {
    title: '总订单量',
    value: operationsData.value.orderQuantity,
    color: '#409eff',
    icon: '📦',
    suffix: '单'
  },
  {
    title: '取消订单',
    value: operationsData.value.cancelledOrderQuantity,
    color: '#f56c6c',
    icon: '❌',
    suffix: '单'
  },
  {
    title: '售后完成',
    value: operationsData.value.aftersalesCompletedOrderQuantity,
    color: '#67c23a',
    icon: '✅',
    suffix: '单'
  },
  {
    title: '用户数量',
    value: operationsData.value.userQuantity,
    color: '#909399',
    icon: '👤',
    suffix: '人'
  },
  {
    title: '商家数量',
    value: operationsData.value.merchantQuantity,
    color: '#e6a23c',
    icon: '🏪',
    suffix: '家'
  },
  {
    title: '骑手数量',
    value: operationsData.value.riderQuantity,
    color: '#606266',
    icon: '🚴',
    suffix: '人'
  }
]);

const { domRef, updateOptions } = useEcharts(() => ({
  tooltip: {
    trigger: 'item',
    backgroundColor: 'rgba(255,255,255,0.95)',
    borderColor: '#e4e7ed',
    borderWidth: 1,
    textStyle: {
      color: '#606266'
    },
    formatter: function(params: any) {
      const percent = ((params.value / operationsData.value.orderQuantity) * 100).toFixed(1);
      return `
        <div style="padding: 8px;">
          <div style="font-weight: bold; margin-bottom: 4px;">${params.name}</div>
          <div style="color: ${params.color};">
            <span style="display: inline-block; width: 10px; height: 10px; background: ${params.color}; border-radius: 50%; margin-right: 6px;"></span>
            数量: ${params.value.toLocaleString()} 单 (${percent}%)
          </div>
        </div>
      `;
    }
  },
  legend: {
    orient: 'vertical',
    left: 'left',
    top: 'middle',
    textStyle: {
      color: '#606266',
      fontSize: 12
    }
  },
  series: [
    {
      name: '订单分布',
      type: 'pie',
      radius: ['40%', '70%'],
      center: ['65%', '50%'],
      avoidLabelOverlap: false,
      itemStyle: {
        borderRadius: 8,
        borderColor: '#fff',
        borderWidth: 2
      },
      label: {
        show: false,
        position: 'center'
      },
      emphasis: {
        label: {
          show: true,
          fontSize: 20,
          fontWeight: 'bold'
        },
        itemStyle: {
          shadowBlur: 10,
          shadowOffsetX: 0,
          shadowColor: 'rgba(0, 0, 0, 0.5)'
        }
      },
      labelLine: {
        show: false
      },
      data: []
    }
  ]
}));

async function fetchOperationsData() {
  loading.value = true;
  try {
    const [start, end] = dateRange.value;
    const startDate = formatDate(new Date(start), 'YYYY-MM-DD');
    const endDate = formatDate(new Date(end), 'YYYY-MM-DD');
    
    console.log('🚀 正在调用API获取营业数据...', { startDate, endDate });
    
    const response = await getRecentOperations(startDate, endDate);
    console.log('📊 API响应数据:', response);
    
    if (response && response.data) {
      const apiData = response.data;
      console.log('✅ 成功获取营业数据:', apiData);
      
      operationsData.value = {
        orderQuantity: apiData.orderQuantity || 0,
        cancelledOrderQuantity: apiData.cancelledOrderQuantity || 0,
        aftersalesCompletedOrderQuantity: apiData.aftersalesCompletedOrderQuantity || 0,
        userQuantity: apiData.userQuantity || 0,
        merchantQuantity: apiData.merchantQuantity || 0,
        riderQuantity: apiData.riderQuantity || 0
      };

      console.log('📈 更新组件数据:', operationsData.value);

      // 更新图表数据
      const normalOrders = operationsData.value.orderQuantity - 
                          operationsData.value.cancelledOrderQuantity - 
                          operationsData.value.aftersalesCompletedOrderQuantity;

      updateOptions(opts => {
        opts.series[0].data = [
          { 
            value: Math.max(0, normalOrders), 
            name: '正常订单',
            itemStyle: { color: '#409eff' }
          },
          { 
            value: operationsData.value.cancelledOrderQuantity, 
            name: '取消订单',
            itemStyle: { color: '#f56c6c' }
          },
          { 
            value: operationsData.value.aftersalesCompletedOrderQuantity, 
            name: '售后完成',
            itemStyle: { color: '#67c23a' }
          }
        ];
        return opts;
      });
    } else {
      console.warn('⚠️ API返回的数据格式不正确:', response);
    }
  } catch (error) {
    console.error('❌ 加载营业数据失败:', error);
    console.error('错误详情:', {
      message: error.message,
      stack: error.stack,
      response: error.response
    });
  } finally {
    loading.value = false;
  }
}

function handleDateRangeChange() {
  fetchOperationsData();
}

onMounted(() => {
  fetchOperationsData();
});
</script>

<template>
  <div class="operations-chart">
    <!-- 时间选择器 -->
    <div class="mb-6 flex items-center justify-between">
      <h3 class="text-lg font-semibold text-gray-800 dark:text-gray-200">营业数据分析</h3>
      <NSpace>
        <NDatePicker
          v-model:value="dateRange"
          type="daterange"
          clearable
          placeholder="选择时间范围"
          @update:value="handleDateRangeChange"
          class="w-72"
        />
        <NButton 
          type="primary" 
          ghost
          @click="fetchOperationsData"
          :loading="loading"
        >
          刷新数据
        </NButton>
      </NSpace>
    </div>

    <!-- 统计卡片 -->
    <div class="mb-6">
      <NGrid :x-gap="16" :y-gap="16" :cols="3" responsive="screen">
        <NGi v-for="stat in statistics" :key="stat.title">
          <div class="bg-white dark:bg-gray-800 rounded-lg p-4 shadow-sm hover:shadow-md transition-shadow duration-200 border border-gray-100 dark:border-gray-700">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm text-gray-600 dark:text-gray-400 mb-1">{{ stat.title }}</p>
                <div class="flex items-baseline">
                  <span class="text-2xl font-bold" :style="{ color: stat.color }">
                    {{ stat.value.toLocaleString() }}
                  </span>
                  <span class="ml-1 text-sm text-gray-500">{{ stat.suffix }}</span>
                </div>
              </div>
              <div 
                class="w-10 h-10 rounded-full flex items-center justify-center text-lg"
                :style="{ backgroundColor: stat.color + '20' }"
              >
                {{ stat.icon }}
              </div>
            </div>
          </div>
        </NGi>
      </NGrid>
    </div>

    <!-- 图表区域 -->
    <div class="relative bg-white dark:bg-gray-800 rounded-lg p-6 shadow-sm">
      <div v-if="loading" class="absolute inset-0 flex items-center justify-center bg-white/80 dark:bg-gray-800/80 backdrop-blur-sm rounded-lg z-10">
        <NSpin size="medium" />
      </div>
      
      <div class="mb-4">
        <h4 class="text-base font-medium text-gray-800 dark:text-gray-200 mb-2">订单状态分布</h4>
        <p class="text-sm text-gray-600 dark:text-gray-400">查看所选时间范围内的订单状态分布情况</p>
      </div>
      
      <div ref="domRef" class="h-80 overflow-hidden"></div>
    </div>
  </div>
</template>

<style scoped>
.operations-chart {
  @apply space-y-4;
}

:deep(.n-statistic .n-statistic-value) {
  font-weight: 700;
}

:deep(.n-date-picker) {
  min-width: 280px;
}
</style>
