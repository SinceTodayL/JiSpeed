<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { getReconciliations, resolveReconciliation } from '@/api/reconciliation';
import { useMessage } from 'naive-ui';

defineOptions({ name: 'Reconciliation' });

const message = useMessage();
const loading = ref<boolean>(false);
const items = ref<any[]>([]);

async function fetchList() {
  loading.value = true;
  try {
    const { data } = await getReconciliations();
    items.value = Array.isArray(data) ? data : [];
  } catch (err) {
    console.error('获取对账列表失败', err);
  } finally {
    loading.value = false;
  }
}

function formatPeriod(start: string, end: string) {
  const startDate = new Date(start).toLocaleDateString('zh-CN', { month: 'short', day: 'numeric' });
  const endDate = new Date(end).toLocaleDateString('zh-CN', { month: 'short', day: 'numeric' });
  return `${startDate} ~ ${endDate}`;
}

function formatAmount(amount: number) {
  const abs = Math.abs(amount);
  const sign = amount >= 0 ? '+' : '-';
  return `${sign}¥${abs.toFixed(2)}`;
}

function getDiffAmountColor(amount: number) {
  if (amount > 0) return 'text-green-600';
  if (amount < 0) return 'text-red-600';
  return 'text-gray-600';
}

function getReconType(type: number) {
  const typeMap = {
    1: '系统对账',
    2: '人工对账',
    3: '异常对账'
  };
  return typeMap[type] || '未知类型';
}

async function handleResolve(reconId: string) {
  try {
    await resolveReconciliation(reconId);
    message.success('已处理该异常');
    fetchList();
  } catch (err) {
    message.error('处理失败');
    console.error('处理失败', err);
  }
}

onMounted(() => fetchList());
</script>

<template>
  <NCard :title="$t('page.home.reconciliation.title')" :bordered="false" class="card-wrapper">
    <div v-if="loading" class="space-y-3">
      <NSkeleton text :repeat="3" />
    </div>
    
    <div v-else-if="items.length === 0" class="text-center py-8 text-gray-500">
      <SvgIcon icon="material-symbols:account-balance-wallet-outline" class="text-4xl mb-2" />
      <p>{{ $t('page.home.reconciliation.noReconciliations') }}</p>
    </div>
    
    <div v-else class="space-y-4">
      <div 
        v-for="item in items" 
        :key="item.reconId"
        class="p-4 rounded-lg border border-gray-200 hover:border-blue-300 hover:shadow-md transition-all duration-200"
        :class="[
          item.isResolved 
            ? 'bg-gradient-to-r from-green-50 to-emerald-50 border-green-200' 
            : 'bg-gradient-to-r from-orange-50 to-red-50 border-orange-200'
        ]"
      >
        <div class="flex items-center justify-between">
          <div class="flex-1">
            <div class="flex items-center space-x-3 mb-2">
              <SvgIcon 
                :icon="item.isResolved ? 'material-symbols:check-circle' : 'material-symbols:warning'" 
                :class="[
                  'text-lg',
                  item.isResolved ? 'text-green-500' : 'text-orange-500'
                ]"
              />
              <h4 class="font-medium text-gray-900">
                对账单 {{ item.reconId.slice(0, 8) }}...
              </h4>
              <NBadge 
                :value="item.isResolved ? '已处理' : '待处理'" 
                :type="item.isResolved ? 'success' : 'warning'"
              />
            </div>
            
            <div class="grid grid-cols-2 md:grid-cols-4 gap-4 text-sm">
              <div>
                <span class="text-gray-500">对账周期</span>
                <p class="font-medium">{{ formatPeriod(item.periodStart, item.periodEnd) }}</p>
              </div>
              <div>
                <span class="text-gray-500">差额金额</span>
                <p class="font-medium" :class="getDiffAmountColor(item.diffAmount)">
                  {{ formatAmount(item.diffAmount) }}
                </p>
              </div>
              <div>
                <span class="text-gray-500">影响订单</span>
                <p class="font-medium">{{ item.affectedOrders }} 单</p>
              </div>
              <div>
                <span class="text-gray-500">对账类型</span>
                <p class="font-medium">{{ getReconType(item.reconType) }}</p>
              </div>
            </div>
          </div>
          
          <div v-if="!item.isResolved" class="ml-4">
            <NButton 
              type="primary" 
              size="small" 
              @click="handleResolve(item.reconId)"
              class="bg-gradient-to-r from-blue-500 to-purple-500 border-none rounded-lg"
            >
              <template #icon>
                <SvgIcon icon="material-symbols:done" />
              </template>
              处理
            </NButton>
          </div>
        </div>
      </div>
    </div>
  </NCard>
</template>

<style scoped></style>


