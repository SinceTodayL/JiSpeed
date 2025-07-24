<script setup lang="ts">
import { computed } from 'vue';
import { createReusableTemplate } from '@vueuse/core';
import { useMerchantStore } from '@/store/modules/merchant';
import { $t } from '@/locales';

defineOptions({
  name: 'CardData'
});

const merchantStore = useMerchantStore();

interface CardData {
  key: string;
  title: string;
  value: number;
  unit: string;
  color: {
    start: string;
    end: string;
  };
  icon: string;
}

// 基于商家真实数据的卡片数据
const cardData = computed<CardData[]>(() => {
  const { salesStats } = merchantStore;
  
  if (!salesStats || salesStats.length === 0) {
    return [
      { key: 'totalSales', title: '总销量', value: 0, unit: '份', color: { start: '#ec4786', end: '#b955a4' }, icon: 'mdi:food' },
      { key: 'totalRevenue', title: '总营收', value: 0, unit: '¥', color: { start: '#865ec0', end: '#5144b4' }, icon: 'ant-design:money-collect-outlined' },
      { key: 'avgDailyRevenue', title: '日均营收', value: 0, unit: '¥', color: { start: '#56cdf3', end: '#719de3' }, icon: 'mdi:trending-up' },
      { key: 'maxDaySales', title: '单日最高销量', value: 0, unit: '份', color: { start: '#fcbc25', end: '#f68057' }, icon: 'mdi:crown' }
    ];
  }
  
  // 计算商家业务指标
  const totalSales = salesStats.reduce((sum, item) => sum + (Number(item.salesQty) || 0), 0);
  const totalAmount = salesStats.reduce((sum, item) => sum + (Number(item.salesAmount) || 0), 0);
  const avgDailyAmount = totalAmount / salesStats.length;
  const maxDaySales = salesStats.reduce((max, item) => Math.max(max, Number(item.salesQty) || 0), 0);
  
  return [
    {
      key: 'totalSales',
      title: '总销量',
      value: totalSales,
      unit: '份',
      color: {
        start: '#ec4786',
        end: '#b955a4'
      },
      icon: 'mdi:food'
    },
    {
      key: 'totalRevenue',
      title: '总营收',
      value: Math.round(totalAmount),
      unit: '¥',
      color: {
        start: '#865ec0',
        end: '#5144b4'
      },
      icon: 'ant-design:money-collect-outlined'
    },
    {
      key: 'avgDailyRevenue',
      title: '日均营收',
      value: Math.round(avgDailyAmount),
      unit: '¥',
      color: {
        start: '#56cdf3',
        end: '#719de3'
      },
      icon: 'mdi:trending-up'
    },
    {
      key: 'maxDaySales',
      title: '单日最高销量',
      value: maxDaySales,
      unit: '份',
      color: {
        start: '#fcbc25',
        end: '#f68057'
      },
      icon: 'mdi:crown'
    }
  ];
});

interface GradientBgProps {
  gradientColor: string;
}

const [DefineGradientBg, GradientBg] = createReusableTemplate<GradientBgProps>();

function getGradientColor(color: CardData['color']) {
  return `linear-gradient(to bottom right, ${color.start}, ${color.end})`;
}
</script>

<template>
  <NCard :bordered="false" size="small" class="card-wrapper">
    <!-- define component start: GradientBg -->
    <DefineGradientBg v-slot="{ $slots, gradientColor }">
      <div class="rd-8px px-16px pb-4px pt-8px text-white" :style="{ backgroundImage: gradientColor }">
        <component :is="$slots.default" />
      </div>
    </DefineGradientBg>
    <!-- define component end: GradientBg -->

    <NGrid cols="s:1 m:2 l:4" responsive="screen" :x-gap="16" :y-gap="16">
      <NGi v-for="item in cardData" :key="item.key">
        <GradientBg :gradient-color="getGradientColor(item.color)" class="flex-1">
          <h3 class="text-16px">{{ item.title }}</h3>
          <div class="flex justify-between pt-12px">
            <SvgIcon :icon="item.icon" class="text-32px" />
            <CountTo
              :prefix="item.unit === '¥' ? item.unit : ''"
              :suffix="item.unit === '¥' ? '' : item.unit"
              :start-value="1"
              :end-value="item.value"
              class="text-30px text-white dark:text-dark"
            />
          </div>
        </GradientBg>
      </NGi>
    </NGrid>
  </NCard>
</template>

<style scoped></style>
