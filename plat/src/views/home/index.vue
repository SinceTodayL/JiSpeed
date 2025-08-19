<script setup lang="ts">
import { computed } from 'vue';
import { useAppStore } from '@/store/modules/app';
import HeaderBanner from './modules/header-banner.vue';
import CardData from './modules/card-data.vue';
import Announcement from './modules/announcement.vue';
import Reconciliation from './modules/reconciliation.vue';
import CouponIssue from './modules/coupon-issue.vue';
import LineChart from './modules/line-chart.vue';

const appStore = useAppStore();

const gap = computed(() => (appStore.isMobile ? 0 : 16));
</script>

<template>
  <div class="min-h-full bg-gradient-to-br from-blue-50 to-purple-50 dark:from-gray-900 dark:to-gray-800">
    <NSpace vertical :size="20" class="p-6">
      <!-- 欢迎横幅 -->
      <div class="relative overflow-hidden rounded-2xl bg-gradient-to-r from-blue-600 to-purple-600 p-8 text-white shadow-2xl">
        <div class="absolute top-0 right-0 h-32 w-32 translate-x-16 translate-y-[-64px] transform rounded-full bg-white/10"></div>
        <div class="absolute bottom-0 left-0 h-24 w-24 translate-x-[-48px] translate-y-12 transform rounded-full bg-white/10"></div>
        <HeaderBanner />
      </div>

      <!-- 平台成绩卡片 -->
      <div class="rounded-2xl bg-white/90 backdrop-blur-sm shadow-xl dark:bg-gray-800/90">
        <CardData />
      </div>

      <!-- 订单趋势图表区域 -->
      <div class="rounded-2xl bg-white/95 backdrop-blur-sm shadow-lg hover:shadow-xl transition-shadow duration-300">
        <NCard 
          title="订单趋势分析" 
          :bordered="false" 
          class="rounded-2xl"
        >

          <LineChart />
        </NCard>
      </div>

      <!-- 功能模块区域 -->
      <NGrid :x-gap="gap" :y-gap="20" responsive="screen" item-responsive>
        <NGi span="24 s:24 m:16">
          <div class="rounded-2xl bg-white/95 backdrop-blur-sm shadow-lg hover:shadow-xl transition-shadow duration-300">
            <Announcement />
          </div>
        </NGi>
        <NGi span="24 s:24 m:8">
          <div class="rounded-2xl bg-white/95 backdrop-blur-sm shadow-lg hover:shadow-xl transition-shadow duration-300">
            <CouponIssue />
          </div>
        </NGi>
      </NGrid>

      <!-- 对账处理区域 -->
      <div class="rounded-2xl bg-white/95 backdrop-blur-sm shadow-lg hover:shadow-xl transition-shadow duration-300">
        <Reconciliation />
      </div>
    </NSpace>
  </div>
</template>

<style scoped></style>
