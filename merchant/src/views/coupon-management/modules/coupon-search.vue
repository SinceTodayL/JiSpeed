<script setup lang="ts">
import { $t } from '@/locales';
import SvgIcon from '@/components/custom/svg-icon.vue';

interface Emits {
  (e: 'reset'): void;
  (e: 'search'): void;
}

defineOptions({
  name: 'CouponSearch'
});

const emit = defineEmits<Emits>();

const couponId = defineModel<string | null>('couponId');
const status = defineModel<string | null>('status');
const faceValueRange = defineModel<[number, number] | null>('faceValueRange');

// 状态选项
const statusOptions = [
  { label: '全部状态', value: '' },
  { label: '未开始', value: 'not_started' },
  { label: '进行中', value: 'active' },
  { label: '已发完', value: 'sold_out' },
  { label: '已过期', value: 'expired' }
];

function reset() {
  // 清空搜索表单
  couponId.value = null;
  status.value = '';
  faceValueRange.value = null;
  emit('reset');
}

function search() {
  emit('search');
}
</script>

<template>
  <NCard :title="$t('common.search')" :bordered="false" size="small" class="card-wrapper">
    <div class="grid grid-cols-1 gap-16px sm:grid-cols-2 lg:grid-cols-4">
      <div>
        <NFormItem label="优惠券ID">
          <NInput 
            v-model:value="couponId" 
            placeholder="请输入优惠券ID"
            clearable
          />
        </NFormItem>
      </div>
      <div>
        <NFormItem label="状态">
          <NSelect
            v-model:value="status"
            placeholder="请选择状态"
            :options="statusOptions"
            clearable
          />
        </NFormItem>
      </div>
      <div class="col-span-2">
        <div class="grid grid-cols-2 gap-12px">
          <NButton @click="reset">
            <template #icon>
              <SvgIcon icon="ic:round-refresh" class="text-icon" />
            </template>
            {{ $t('common.reset') }}
          </NButton>
          <NButton type="primary" ghost @click="search">
            <template #icon>
              <SvgIcon icon="ic:round-search" class="text-icon" />
            </template>
            {{ $t('common.search') }}
          </NButton>
        </div>
      </div>
    </div>
  </NCard>
</template>

<style scoped></style> 