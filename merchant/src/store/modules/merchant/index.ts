import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { getUserId } from '@/store/modules/auth/shared';

export const useMerchantStore = defineStore('merchant', () => {
  // 添加响应式触发器，确保能响应localStorage变化
  const authUpdateTrigger = ref(0);
  
  // 商家ID - 直接从 AuthStorage 读取 userId
  const merchantId = computed(() => {
    // 依赖触发器以支持响应式更新
    authUpdateTrigger.value;
    return getUserId();
  });

  // 触发认证更新（URL参数解析后或登录后调用）
  function triggerAuthUpdate() {
    authUpdateTrigger.value++;
  }
  
  const merchantInfo = ref<any | null>(null);
  
  const salesStats = ref<any[]>([]);
  
  // 设置商家信息
  function setMerchantInfo(info: any) {
    merchantInfo.value = info;
  }
  
  // 设置销售统计数据
  function setSalesStats(stats: any[]) {
    salesStats.value = stats;
  }
  
  // 重置store
  function resetStore() {
    merchantInfo.value = null;
    salesStats.value = [];
  }
  
  return {
    merchantId,
    merchantInfo,
    salesStats,
    setMerchantInfo,
    setSalesStats,
    resetStore,
    triggerAuthUpdate
  };
}); 