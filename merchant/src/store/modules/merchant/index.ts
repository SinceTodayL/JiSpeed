import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useMerchantStore = defineStore('merchant', () => {
  // 商家ID - 可以从环境变量、用户登录信息或配置中获取
  const merchantId = ref<string>('MER1234567890001');
  
  // 商家基本信息
  const merchantInfo = ref<any | null>(null);
  
  // 销售统计数据
  const salesStats = ref<any[]>([]);
  
  // 设置商家ID
  function setMerchantId(id: string) {
    merchantId.value = id;
  }
  
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
    setMerchantId,
    setMerchantInfo,
    setSalesStats,
    resetStore
  };
}); 