<script setup lang="ts">
import { computed, ref } from 'vue';
import { useNaiveForm } from '@/hooks/common/form';
import SvgIcon from '@/components/custom/svg-icon.vue';

defineOptions({
  name: 'GoodsSearch'
});

interface Props {
  model: {
    dishName: string | null;
    categoryId: string | null;
    onSale: number | null;
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

// 销售状态选项
const saleStatusOptions = [
  { label: '在售', value: 1 },
  { label: '停售', value: 0 }
];

function reset() {
  // 清空搜索表单
  model.value.dishName = null;
  model.value.categoryId = null;
  model.value.onSale = null;
  emit('reset');
}

function search() {
  emit('search');
}
</script>

<template>
  <NCard :bordered="false" size="small" class="card-wrapper">
    <NCollapse :default-expanded-names="['goods-search']">
      <NCollapseItem title="搜索" name="goods-search">
        <NForm ref="formRef" :model="model" label-placement="left" :label-width="80">
          <NGrid responsive="screen" item-responsive>
            <NFormItemGi span="24 s:12 m:6" label="菜品名称" path="dishName" class="pr-24px">
              <NInput v-model:value="model.dishName" placeholder="请输入菜品名称" />
            </NFormItemGi>
            <NFormItemGi span="24 s:12 m:6" label="分类ID" path="categoryId" class="pr-24px">
              <NInput v-model:value="model.categoryId" placeholder="请输入分类ID" />
            </NFormItemGi>
            <NFormItemGi span="24 s:12 m:6" label="销售状态" path="onSale" class="pr-24px">
              <NSelect
                v-model:value="model.onSale"
                placeholder="请选择销售状态"
                :options="saleStatusOptions"
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