<script setup lang="ts">
import { computed, ref } from 'vue';
import { useNaiveForm } from '@/hooks/common/form';
import SvgIcon from '@/components/custom/svg-icon.vue';
import { ORDER_STATUS_MAP } from '@/service/api';

defineOptions({
  name: 'OrderSearch'
});

interface Props {
  model: {
    orderId: string | null;
    orderStatus: number | null;
  };
}

const props = defineProps<Props>();

interface Emits {
  (e: 'reset'): void;
  (e: 'search'): void;
}

const emit = defineEmits<Emits>();

const { formRef } = useNaiveForm();

const model = defineModel<Props['model']>('model', {
  required: true
});

// 订单状态选项 - 只显示1-4的状态
const orderStatusOptions = computed(() => {
  return Object.entries(ORDER_STATUS_MAP)
    .filter(([value]) => [1, 2, 3, 4].includes(Number(value)))
    .map(([value, label]) => ({
      label,
      value: Number(value)
    }));
});

function reset() {
  // 清空搜索表单
  model.value.orderId = null;
  model.value.orderStatus = null;
  emit('reset');
}

function search() {
  emit('search');
}
</script>

<template>
  <NCard :bordered="false" size="small" class="card-wrapper">
    <NCollapse :default-expanded-names="['order-search']">
      <NCollapseItem title="搜索" name="order-search">
        <NForm ref="formRef" :model="model" label-placement="left" :label-width="80">
          <NGrid responsive="screen" item-responsive>
            <NFormItemGi span="24 s:12 m:6" label="订单ID" path="orderId" class="pr-24px">
              <NInput v-model:value="model.orderId" placeholder="请输入订单ID" />
            </NFormItemGi>
            <NFormItemGi span="24 s:12 m:6" label="订单状态" path="orderStatus" class="pr-24px">
              <NSelect
                v-model:value="model.orderStatus"
                placeholder="请选择订单状态"
                :options="orderStatusOptions"
                clearable
              />
            </NFormItemGi>
            <NFormItemGi span="24 s:12 m:6" class="pr-24px">
              <NSpace class="w-full" justify="end">
                <NButton @click="reset">
                  <template #icon>
                    <SvgIcon icon="ic:round-refresh" class="text-icon" />
                  </template>
                  重置
                </NButton>
                <NButton type="primary" ghost @click="search">
                  <template #icon>
                    <SvgIcon icon="ic:round-search" class="text-icon" />
                  </template>
                  搜索
                </NButton>
              </NSpace>
            </NFormItemGi>
          </NGrid>
        </NForm>
      </NCollapseItem>
    </NCollapse>
  </NCard>
</template>

<style scoped></style> 