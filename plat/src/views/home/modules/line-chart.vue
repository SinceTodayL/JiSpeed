<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useEcharts } from '@/hooks/common/echarts';
import { getOrderTrends } from '@/api/rider';

defineOptions({
  name: 'OrderTrendChart'
});

const loading = ref(true);

const { domRef, updateOptions } = useEcharts(() => ({
  tooltip: {
    trigger: 'axis',
    backgroundColor: 'rgba(255,255,255,0.95)',
    borderColor: '#e4e7ed',
    borderWidth: 1,
    textStyle: {
      color: '#606266'
    },
    formatter: function(params: any) {
      const data = params[0];
      return `
        <div style="padding: 8px;">
          <div style="font-weight: bold; margin-bottom: 4px;">${data.name}</div>
          <div style="color: #409eff;">
            <span style="display: inline-block; width: 10px; height: 10px; background: #409eff; border-radius: 50%; margin-right: 6px;"></span>
            订单量: ${data.value.toLocaleString()} 单
          </div>
        </div>
      `;
    }
  },
  grid: {
    left: '3%',
    right: '4%',
    bottom: '10%',
    top: '10%',
    containLabel: true
  },
  xAxis: {
    type: 'category',
    boundaryGap: false,
    data: [] as string[],
    axisLine: {
      lineStyle: {
        color: '#e4e7ed'
      }
    },
    axisLabel: {
      color: '#606266',
      fontSize: 12
    }
  },
  yAxis: {
    type: 'value',
    axisLine: {
      show: false
    },
    axisTick: {
      show: false
    },
    axisLabel: {
      color: '#606266',
      fontSize: 12,
      formatter: '{value}'
    },
    splitLine: {
      lineStyle: {
        color: '#f5f7fa',
        type: 'dashed'
      }
    }
  },
  series: [
    {
      name: '订单量',
      data: [] as number[],
      type: 'line',
      smooth: true,
      symbol: 'circle',
      symbolSize: 6,
      lineStyle: {
        width: 3,
        color: {
          type: 'linear',
          x: 0,
          y: 0,
          x2: 1,
          y2: 0,
          colorStops: [
            { offset: 0, color: '#409eff' },
            { offset: 1, color: '#67c23a' }
          ]
        }
      },
      areaStyle: {
        color: {
          type: 'linear',
          x: 0,
          y: 0,
          x2: 0,
          y2: 1,
          colorStops: [
            { offset: 0, color: 'rgba(64, 158, 255, 0.3)' },
            { offset: 1, color: 'rgba(64, 158, 255, 0.05)' }
          ]
        }
      },
      itemStyle: {
        color: '#409eff',
        borderColor: '#fff',
        borderWidth: 2
      }
    }
  ]
}));

async function initChart() {
  loading.value = true;
  try {
    const { data: trends } = await getOrderTrends(6);
    
    const months = trends.map((item: any) => item.month);
    const orders = trends.map((item: any) => item.orders);
    
    updateOptions(opts => {
      opts.xAxis.data = months;
      opts.series[0].data = orders;
      return opts;
    });
  } catch (error) {
    console.error('Failed to load order trends:', error);
    // 删除静态默认数据
    opts.xAxis.data = [];
    opts.series[0].data = [];
    return opts;
  } finally {
    loading.value = false;
  }
}

onMounted(() => {
  initChart();
});
</script>

<template>
  <div class="relative">
    <div v-if="loading" class="absolute inset-0 flex items-center justify-center bg-white/80 backdrop-blur-sm rounded-lg">
      <NSpin size="medium" />
    </div>
    <div ref="domRef" class="h-360px overflow-hidden"></div>
  </div>
</template>

<style scoped></style>