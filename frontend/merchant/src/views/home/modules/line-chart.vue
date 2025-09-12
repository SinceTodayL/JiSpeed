<script setup lang="ts">
import { watch, computed } from 'vue';
import { useAppStore } from '@/store/modules/app';
import { useMerchantStore } from '@/store/modules/merchant';
import { useEcharts } from '@/hooks/common/echarts';
import { $t } from '@/locales';

defineOptions({
  name: 'LineChart'
});

const appStore = useAppStore();
const merchantStore = useMerchantStore();

const { domRef, updateOptions } = useEcharts(() => ({
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
    data: ['销售数量', '销售金额']
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
    data: [] as string[]
  },
  yAxis: [
    {
      type: 'value',
      name: '销售数量 (份)',
      position: 'left'
    },
    {
      type: 'value',
      name: '销售金额 (¥)',
      position: 'right'
    }
  ],
  series: [
    {
      color: '#8e9dff',
      name: '销售数量',
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
            {
              offset: 0.25,
              color: '#8e9dff'
            },
            {
              offset: 1,
              color: '#fff'
            }
          ]
        }
      },
      emphasis: {
        focus: 'series'
      },
      data: [] as number[]
    },
    {
      color: '#26deca',
      name: '销售金额',
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
            {
              offset: 0.25,
              color: '#26deca'
            },
            {
              offset: 1,
              color: '#fff'
            }
          ]
        }
      },
      emphasis: {
        focus: 'series'
      },
      data: []
    }
  ]
}));

// 更新图表数据
function updateChartData() {
  const { salesStats } = merchantStore;
  
  if (salesStats.length === 0) {
    return;
  }

  // 格式化日期和数据
  const dates = salesStats.map(item => {
    try {
      // 直接处理标准日期格式 "2024-09-10"
      const date = new Date(item.statDate);
      
      // 检查日期是否有效
      if (isNaN(date.getTime())) {
        console.warn('无效的日期格式:', item.statDate);
        return '无效日期';
      }
      
      // 返回格式化的日期 (月/日)
      return `${date.getMonth() + 1}/${date.getDate()}`;
    } catch (error) {
      console.error('日期处理错误:', error, item.statDate);
      return '日期错误';
    }
  });
  
  const salesQtyData = salesStats.map(item => Number(item.salesQty) || 0);
  const salesAmountData = salesStats.map(item => Math.round(Number(item.salesAmount) || 0));



  updateOptions(opts => {
    opts.xAxis.data = dates;
    opts.series[0].data = salesQtyData;
    opts.series[1].data = salesAmountData;

    return opts;
  });
}

// 监听商家数据变化
watch(
  () => merchantStore.salesStats,
  () => {
    updateChartData();
  },
  { deep: true, immediate: true }
);

function updateLocale() {
  updateOptions((opts, factory) => {
    const originOpts = factory();

    opts.legend.data = originOpts.legend.data;
    opts.series[0].name = originOpts.series[0].name;
    opts.series[1].name = originOpts.series[1].name;

    return opts;
  });
}

async function init() {
  updateChartData();
}

watch(
  () => appStore.locale,
  () => {
    updateLocale();
  }
);

// init
init();
</script>

<template>
  <NCard :bordered="false" class="card-wrapper">
    <div ref="domRef" class="h-360px overflow-hidden"></div>
  </NCard>
</template>

<style scoped></style>
