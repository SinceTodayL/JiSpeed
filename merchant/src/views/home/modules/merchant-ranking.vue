<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { NCard, NSpace, NTag, NSpin, NEmpty } from 'naive-ui';
import { useAppStore } from '@/store/modules/app';
import { fetchMerchantsRanking } from '@/service/api';
import { $t } from '@/locales';

defineOptions({
  name: 'MerchantRanking'
});

const appStore = useAppStore();

// 商家排行数据
const merchantsData = ref<Api.Merchant.MerchantRankingItem[]>([]);
const loading = ref(false);

// 商家状态映射
const MERCHANT_STATUS_MAP: Record<number, { label: string; type: 'success' | 'warning' | 'error' | 'info' }> = {
  0: { label: '未审核', type: 'warning' },
  1: { label: '正常营业', type: 'success' },
  2: { label: '暂停营业', type: 'info' },
  3: { label: '停业', type: 'error' }
};

// 获取商家排行数据
const loadMerchantsRanking = async () => {
  loading.value = true;
  try {
    console.log('=== 开始获取商家排行榜 ===');
    
    const result = await fetchMerchantsRanking({
      size: 10
    });
    
    console.log('商家排行榜API响应:', result);
    
    // 解析数据结构
    const resultAny = result as any;
    let merchantsArray: Api.Merchant.MerchantRankingItem[] = [];
    
    if (resultAny?.data && Array.isArray(resultAny.data)) {
      merchantsArray = resultAny.data;
    } else if (Array.isArray(resultAny)) {
      merchantsArray = resultAny;
    } else if (result && Array.isArray(result)) {
      merchantsArray = result;
    }
    
    console.log('解析后的商家数据:', merchantsArray);
    
    merchantsData.value = merchantsArray;
    
  } catch (error) {
    console.error('获取商家排行榜失败:', error);
    window.$message?.error('获取商家排行榜失败');
    merchantsData.value = [];
  } finally {
    loading.value = false;
  }
};



// 计算排行榜数据
const rankedMerchants = computed(() => {
  if (!merchantsData.value || merchantsData.value.length === 0) {
    return [];
  }
  
  // 按订单量降序排序
  const sortedMerchants = [...merchantsData.value].sort((a, b) => (b.ordersCount || 0) - (a.ordersCount || 0));
  
  // 添加排名信息
  return sortedMerchants.map((merchant, index) => ({
    ...merchant,
    rank: index + 1
  }));
});

// 获取状态标签
const getStatusTag = (status: number) => {
  const statusInfo = MERCHANT_STATUS_MAP[status] || { label: '未知', type: 'error' as const };
  return statusInfo;
};

// 格式化订单量
const formatOrdersCount = (count: number) => {
  if (count === null) {
    console.log('ordersCount is null');
    return 'No data';
  }
  if (count === 0) return '暂无订单';
  if (count >= 10000) {
    return `${(count / 10000).toFixed(1)}万单`;
  } else if (count >= 1000) {
    return `${(count / 1000).toFixed(1)}K单`;
  } else {
    return `${count}单`;
  }
};

// 获取排名样式
const getRankClass = (rank: number) => {
  switch (rank) {
    case 1:
      return 'text-yellow-500 font-bold text-lg'; // 金色
    case 2:
      return 'text-gray-400 font-bold text-lg'; // 银色  
    case 3:
      return 'text-orange-600 font-bold text-lg'; // 铜色
    default:
      return 'text-gray-600 font-semibold';
  }
};

// 获取排名图标
const getRankIcon = (rank: number) => {
  switch (rank) {
    case 1:
      return '🥇';
    case 2:
      return '🥈';
    case 3:
      return '🥉';
    default:
      return `${rank}`;
  }
};

// 组件挂载时获取数据
onMounted(() => {
  loadMerchantsRanking();
});
</script>

<template>
  <NCard :bordered="false" class="card-wrapper" title="商家排行榜">
    <template #header-extra>
      <NTag type="info" size="small">Congratulations!</NTag>
    </template>
    
    <NSpin :show="loading">
      <div v-if="rankedMerchants.length > 0" class="space-y-3">
        <div 
          v-for="merchant in rankedMerchants" 
          :key="merchant.merchantId"
          class="flex items-center p-3 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors"
        >
          <!-- 排名 -->
          <div class="flex-shrink-0 w-12 text-center">
            <span :class="getRankClass(merchant.rank)">
              {{ getRankIcon(merchant.rank) }}
            </span>
          </div>
          
          <!-- 商家信息 -->
          <div class="flex-1 ml-3">
            <div class="flex items-center justify-between">
              <div>
                <h4 class="font-semibold text-gray-900 truncate max-w-32">
                  {{ merchant.merchantName }}
                </h4>
                <p class="text-sm text-gray-500 truncate max-w-40">
                  📍 {{ merchant.location }}
                </p>
              </div>
              
              <!-- 订单量和状态 -->
              <div class="text-right flex-shrink-0">
                <div class="text-blue-600 font-semibold">
                  {{ formatOrdersCount(merchant.ordersCount || 0) }}
                </div>
                <div class="mt-1">
                  <NTag 
                    :type="getStatusTag(merchant.status).type" 
                    size="small"
                  >
                    {{ getStatusTag(merchant.status).label }}
                  </NTag>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <!-- 空状态 -->
      <div v-else-if="!loading" class="h-60 flex items-center justify-center">
        <NEmpty description="暂无排行榜数据">
          <template #icon>
            <div class="text-6xl">📊</div>
          </template>
        </NEmpty>
      </div>
      
      <!-- Loading 占位 -->
      <div v-else class="h-60 flex items-center justify-center">
        <div class="text-gray-400">正在加载排行榜...</div>
      </div>
    </NSpin>
  </NCard>
</template>

<style scoped>
.card-wrapper {
  height: 100%;
}

/* 确保卡片内容可滚动 */
.card-wrapper :deep(.n-card__content) {
  max-height: 400px;
  overflow-y: auto;
}
</style>
