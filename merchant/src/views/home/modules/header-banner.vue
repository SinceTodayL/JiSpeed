<script setup lang="ts">
import { computed, onMounted } from 'vue';
import { useAppStore } from '@/store/modules/app';
import { useAuthStore } from '@/store/modules/auth';
import { useMerchantStore } from '@/store/modules/merchant';
import { fetchMerchantInfo, fetchMerchantSalesStats } from '@/service/api';
import { localStg } from '@/utils/storage';
import { $t } from '@/locales';

defineOptions({
  name: 'HeaderBanner'
});

const appStore = useAppStore();
const merchantStore = useMerchantStore();

const gap = computed(() => (appStore.isMobile ? 0 : 16));

interface StatisticData {
  id: number;
  label: string;
  value: string;
}

// 基于商家真实数据的统计信息
const statisticData = computed<StatisticData[]>(() => {
  const { salesStats } = merchantStore;
  
  if (!salesStats || salesStats.length === 0) {
    return [
      { id: 0, label: '统计天数', value: '0天' },
      { id: 1, label: '总销售额', value: '¥0.00' },
      { id: 2, label: '日均销量', value: '0份' }
    ];
  }
  
  // 计算统计指标
  const totalSales = salesStats.reduce((sum, item) => sum + (Number(item.salesQty) || 0), 0);
  const totalAmount = salesStats.reduce((sum, item) => sum + (Number(item.salesAmount) || 0), 0);
  const avgSales = Math.round(totalSales / salesStats.length);
  
  const statDays = 7;

  return [
    {
      id: 0,
      label: '统计天数',
      value: `${statDays}天`
    },
    {
      id: 1,
      label: '总销售额',
      value: `¥${totalAmount.toFixed(2)}`
    },
    {
      id: 2,
      label: '日均销量',
      value: `${avgSales}份`
    }
  ];
});

// 商家问候语
const merchantGreeting = computed(() => {
  const merchantName = merchantStore.merchantInfo?.data.merchantName || '商家';
  return `你好，${merchantName} !`;
});

// 商家状态描述
const merchantStatusDesc = computed(() => {
  const status = merchantStore.merchantInfo?.data.status;
  const location = merchantStore.merchantInfo?.data.location || '';
  
  let statusText = '🟢 营业中'; // 默认显示营业中
  
  // 如果有明确的状态值，可以根据业务需要调整
  if (status === 0) {
    statusText = '🔴 暂停营业';
  } else if (status && status > 0) {
    statusText = '🟢 营业中';
  }
  
  return `${statusText} | ${location}`;
});

// 获取商家数据
const loadMerchantData = async () => {
  const { merchantId } = merchantStore;
  
  if (!merchantId) {
    console.log("merchantId is empty");
    return;
  }

  try {
    const result = await fetchMerchantInfo(merchantId);
    // console.log("result", result);
    // Unpack the real data from the wrapper object before setting it to the store.
    if (result && result.data) {
      merchantStore.setMerchantInfo(result.data);
      console.log("merchantStore.merchantInfo", merchantStore.merchantInfo?.data);
    }
  } catch (error) {
    console.error('加载商家基本信息失败:', error);
    window.$message?.error('获取商家基本信息失败');
  }

  try {
    const result = await fetchMerchantSalesStats(merchantId);
    // Unpack the real data from the wrapper object.
    if (result && Array.isArray(result.response.data.data)) {
      merchantStore.setSalesStats(result.response.data.data);
      console.log('成功加载销售统计数据:', result.response.data.data);
    } else {
      console.warn('销售统计数据格式异常:', result.response.data.data);
    }
  } catch (error) {
    console.error('加载商家销售数据失败:', error);
    window.$message?.error('获取商家销售数据失败');
  }
};

onMounted(() => {
  merchantStore.triggerAuthUpdate();
  loadMerchantData();
});
</script>

<template>
  <NCard :bordered="false" class="card-wrapper">
    <NGrid :x-gap="gap" :y-gap="16" responsive="screen" item-responsive>
      <NGi span="24 s:24 m:18">
        <div class="flex-y-center">
          <div class="size-72px shrink-0 overflow-hidden rd-1/2">
            <img src="@/assets/svg-icon/avatar.svg" class="size-full" />
          </div>
          <div class="pl-12px">
            <h3 class="text-18px font-semibold">
              {{ merchantGreeting }}
            </h3>
            <p class="text-#999 leading-30px">{{ merchantStatusDesc }}</p>
            <p v-if="merchantStore.merchantInfo?.contactInfo" class="text-#666 text-12px mt-1">
              📞 {{ merchantStore.merchantInfo.contactInfo }}
            </p>
          </div>
        </div>
      </NGi>
      <NGi span="24 s:24 m:6">
        <NSpace :size="24" justify="end">
          <NStatistic v-for="item in statisticData" :key="item.id" class="whitespace-nowrap" v-bind="item" />
        </NSpace>
      </NGi>
    </NGrid>
  </NCard>
</template>

<style scoped></style>
