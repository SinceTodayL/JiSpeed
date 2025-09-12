import { computed, ref } from 'vue';
import { defineStore } from 'pinia';
import { getUserId } from '@/store/modules/auth/shared';

export const useRiderStore = defineStore('rider', () => {
  // 添加响应式触发器，确保能响应localStorage变化
  const authUpdateTrigger = ref(0);

  // 骑手ID - 直接从 AuthStorage 读取 userId
  const riderId = computed(() => {
    // 依赖触发器以支持响应式更新
    authUpdateTrigger.value;
    return getUserId();
  });

  // 触发认证更新（URL参数解析后或登录后调用）
  function triggerAuthUpdate() {
    authUpdateTrigger.value++;
  }

  const riderInfo = ref<any | null>(null);

  const performanceStats = ref<any[]>([]);

  // 设置骑手信息
  function setRiderInfo(info: any) {
    riderInfo.value = info;
  }

  // 设置绩效统计数据
  function setPerformanceStats(stats: any[]) {
    performanceStats.value = stats;
  }

  // 重置store
  function resetStore() {
    riderInfo.value = null;
    performanceStats.value = [];
  }

  return {
    riderId,
    riderInfo,
    performanceStats,
    setRiderInfo,
    setPerformanceStats,
    resetStore,
    triggerAuthUpdate
  };
});
