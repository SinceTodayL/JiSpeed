<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import { useFormRules, useNaiveForm } from '@/hooks/common/form';

defineOptions({
  name: 'GoodsOperateDrawer'
});

interface Props {
  /** the type of operation */
  operateType: 'add' | 'edit';
  /** the edit row data */
  rowData?: Api.Goods.DishItem | null;
}

const props = defineProps<Props>();

interface Emits {
  (e: 'submitted'): void;
}

const emit = defineEmits<Emits>();

const visible = defineModel<boolean>('visible', {
  default: false
});

const { formRef, validate, restoreValidation } = useNaiveForm();
const { defaultRequiredRule } = useFormRules();

const title = computed(() => {
  const titles: Record<'add' | 'edit', string> = {
    add: '新增商品',
    edit: '编辑商品'
  };
  return titles[props.operateType];
});

type Model = Pick<
  Api.Goods.DishItem,
  'dishName' | 'price' | 'originPrice' | 'coverUrl' | 'categoryId' | 'onSale' | 'quantity'
>;

const model = ref(createDefaultModel());

function createDefaultModel(): Model {
  return {
    dishName: '',
    price: 0,
    originPrice: 0,
    coverUrl: '',
    categoryId: '',
    onSale: 1,
    quantity: 0
  };
}

type RuleKey = Extract<keyof Model, 'dishName' | 'price' | 'categoryId'>;

const rules: Record<RuleKey, App.Global.FormRule> = {
  dishName: defaultRequiredRule,
  price: defaultRequiredRule,
  categoryId: defaultRequiredRule
};

// 销售状态选项
const saleStatusOptions = [
  { label: '在售', value: 1 },
  { label: '停售', value: 0 }
];

function handleInitModel() {
  Object.assign(model.value, createDefaultModel());

  if (props.operateType === 'edit' && props.rowData) {
    Object.assign(model.value, {
      dishName: props.rowData.dishName,
      price: props.rowData.price,
      originPrice: props.rowData.originPrice,
      coverUrl: props.rowData.coverUrl,
      categoryId: props.rowData.categoryId,
      onSale: props.rowData.onSale,
      quantity: props.rowData.quantity
    });
  }
}

function closeDrawer() {
  visible.value = false;
}

async function handleSubmit() {
  await validate();
  
  // TODO: 实现提交逻辑
  console.log('提交数据:', model.value);
  
  window.$message?.success(props.operateType === 'add' ? '新增成功' : '更新成功');
  closeDrawer();
  emit('submitted');
}

watch(visible, () => {
  if (visible.value) {
    handleInitModel();
    restoreValidation();
  }
});
</script>

<template>
  <NDrawer v-model:show="visible" display-directive="show" :width="360">
    <NDrawerContent :title="title" :native-scrollbar="false" closable>
      <NForm ref="formRef" :model="model" :rules="rules">
        <NFormItem label="菜品名称" path="dishName">
          <NInput v-model:value="model.dishName" placeholder="请输入菜品名称" />
        </NFormItem>
        <NFormItem label="分类ID" path="categoryId">
          <NInput v-model:value="model.categoryId" placeholder="请输入分类ID" />
        </NFormItem>
        <NFormItem label="价格" path="price">
          <NInputNumber v-model:value="model.price" placeholder="请输入价格" :precision="2" :min="0" class="w-full" />
        </NFormItem>
        <NFormItem label="原价" path="originPrice">
          <NInputNumber v-model:value="model.originPrice" placeholder="请输入原价" :precision="2" :min="0" class="w-full" />
        </NFormItem>
        <NFormItem label="库存数量" path="quantity">
          <NInputNumber v-model:value="model.quantity" placeholder="请输入库存数量" :min="0" class="w-full" />
        </NFormItem>
        <NFormItem label="商品图片" path="coverUrl">
          <NInput v-model:value="model.coverUrl" placeholder="请输入图片URL" />
        </NFormItem>
        <NFormItem label="销售状态" path="onSale">
          <NRadioGroup v-model:value="model.onSale">
            <NRadio v-for="item in saleStatusOptions" :key="item.value" :value="item.value" :label="item.label" />
          </NRadioGroup>
        </NFormItem>
      </NForm>
      <template #footer>
        <NSpace justify="end">
          <NButton @click="closeDrawer">取消</NButton>
          <NButton type="primary" @click="handleSubmit">确定</NButton>
        </NSpace>
      </template>
    </NDrawerContent>
  </NDrawer>
</template>

<style scoped></style> 