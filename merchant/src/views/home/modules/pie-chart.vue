<script setup lang="ts">
import { watch, computed, onMounted, ref } from 'vue';
import { useAppStore } from '@/store/modules/app';
import { useMerchantStore } from '@/store/modules/merchant';
import { useEcharts } from '@/hooks/common/echarts';
import { fetchGetAllDishes } from '@/service/api';
import { $t } from '@/locales';

defineOptions({
  name: 'PieChart'
});

const appStore = useAppStore();
const merchantStore = useMerchantStore();

// 菜品数据
const dishesData = ref<any[]>([]);

const { domRef, updateOptions } = useEcharts(() => ({
  tooltip: {
    trigger: 'item',
    formatter: '{a} <br/>{b}: {c}份 ({d}%)'
  },
  legend: {
    bottom: '1%',
    left: 'center',
    itemStyle: {
      borderWidth: 0
    }
  },
  series: [
    {
      color: ['#5da8ff', '#8e9dff', '#fedc69', '#26deca', '#ff7875', '#52c41a', '#1890ff', '#722ed1'],
      name: '分类销量',
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
      labelLine: {
        show: false
      },
      data: [] as { name: string; value: number }[]
    }
  ]
}));

// 获取菜品数据
const loadDishesData = async () => {
  try {
    const dishes = await fetchGetAllDishes(merchantStore.merchantId);
    dishesData.value = dishes || [];
    updateDishChart();
  } catch (error) {
    console.error('获取菜品数据失败:', error);
    dishesData.value = [];
    updateDishChart();
  }
};

// 更新分类占比图表
function updateDishChart() {
  if (!dishesData.value || dishesData.value.length === 0) {
    updateOptions(opts => {
      opts.series[0].data = [
        { name: '暂无数据', value: 1 }
      ];
      return opts;
    });
    return;
  }

  // 按分类ID分组统计销量
  const categoryMap = new Map<string, number>();
  
  dishesData.value.forEach(dish => {
    const categoryId = dish.categoryId || '未知分类';
    const sales = Number(dish.monthlySales) || 0;
    
    if (categoryMap.has(categoryId)) {
      categoryMap.set(categoryId, categoryMap.get(categoryId)! + sales);
    } else {
      categoryMap.set(categoryId, sales);
    }
  });

  // 转换为数组并按销量排序
  const categorySales = Array.from(categoryMap.entries()).map(([categoryId, totalSales]) => ({
    name: getCategoryName(categoryId),
    value: totalSales,
    categoryId: categoryId
  }));

  // 按销量从高到低排序
  const sortedCategories = categorySales.sort((a, b) => b.value - a.value);

  updateOptions(opts => {
    opts.series[0].data = sortedCategories;
    return opts;
  });
}

// 根据分类ID获取分类名称 (可以根据实际业务需求调整)
function getCategoryName(categoryId: string): string {
  const categoryNames: Record<string, string> = {
    'CAT10001': '主菜类',
    'CAT10002': '素菜类', 
    'CAT10003': '汤类',
    'CAT10004': '小食类',
    'CAT10005': '饮料类',
    'CAT10006': '甜品类',
    'CAT10007': '酒水类',
    'CAT10008': '特色菜',
    // 可以继续添加更多分类映射
  };
  
  return categoryNames[categoryId] || `分类${categoryId}`;
}

// 监听菜品数据变化
watch(
  () => dishesData.value,
  () => {
    updateDishChart();
  },
  { deep: true }
);

// 组件挂载时获取数据
onMounted(() => {
  loadDishesData();
});
</script>

<template>
  <NCard :bordered="false" class="card-wrapper" title="分类销量占比">
    <div ref="domRef" class="h-360px overflow-hidden"></div>
  </NCard>
</template>

<style scoped></style>
