<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import { useFormRules, useNaiveForm } from '@/hooks/common/form';
import { ORDER_STATUS_MAP, getOrderStatusText } from '@/service/api';

defineOptions({
  name: 'OrderOperateDrawer'
});

interface Props {
  /** the type of operation */
  operateType: 'add' | 'edit';
  /** the edit row data */
  rowData?: Api.Order.OrderItem | null;
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
    add: '新增订单',
    edit: '查看订单详情'
  };
  return titles[props.operateType];
});

type Model = Pick<
  Api.Order.OrderItem,
  'orderId' | 'userId' | 'orderAmount' | 'orderStatus' | 'addressId' | 'couponId' | 'assignid' | 'reconId'
>;

const model = ref(createDefaultModel());

function createDefaultModel(): Model {
  return {
    orderId: '',
    userId: '',
    orderAmount: 0,
    orderStatus: 1,
    addressId: '',
    couponId: '',
    assignid: '',
    reconId: ''
  };
}

type RuleKey = Extract<keyof Model, 'orderId' | 'userId' | 'orderAmount'>;

const rules: Record<RuleKey, App.Global.FormRule> = {
  orderId: defaultRequiredRule,
  userId: defaultRequiredRule,
  orderAmount: defaultRequiredRule
};

// 订单状态选项
const orderStatusOptions = computed(() => {
  return Object.entries(ORDER_STATUS_MAP).map(([value, label]) => ({
    label,
    value: Number(value)
  }));
});

function handleInitModel() {
  Object.assign(model.value, createDefaultModel());

  if (props.operateType === 'edit' && props.rowData) {
    Object.assign(model.value, {
      orderId: props.rowData.orderId,
      userId: props.rowData.userId,
      orderAmount: props.rowData.orderAmount,
      orderStatus: props.rowData.orderStatus,
      addressId: props.rowData.addressId,
      couponId: props.rowData.couponId,
      assignid: props.rowData.assignid,
      reconId: props.rowData.reconId
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

// 格式化创建时间
const formatCreateTime = computed(() => {
  if (props.rowData?.createAt) {
    const date = new Date(props.rowData.createAt);
    return date.toLocaleString('zh-CN');
  }
  return '';
});

watch(visible, () => {
  if (visible.value) {
    handleInitModel();
    restoreValidation();
  }
});
</script>

<template>
  <NDrawer v-model:show="visible" display-directive="show" :width="420">
    <NDrawerContent :title="title" :native-scrollbar="false" closable>
      <NForm ref="formRef" :model="model" :rules="rules">
        <NFormItem label="订单ID" path="orderId">
          <NInput 
            v-model:value="model.orderId" 
            placeholder="请输入订单ID" 
            :readonly="operateType === 'edit'"
          />
        </NFormItem>
        <NFormItem label="用户ID" path="userId">
          <NInput 
            v-model:value="model.userId" 
            placeholder="请输入用户ID"
            :readonly="operateType === 'edit'"
          />
        </NFormItem>
        <NFormItem label="订单金额" path="orderAmount">
          <NInputNumber 
            v-model:value="model.orderAmount" 
            placeholder="请输入订单金额" 
            :precision="2" 
            :min="0" 
            class="w-full"
            :readonly="operateType === 'edit'"
          >
            <template #prefix>¥</template>
          </NInputNumber>
        </NFormItem>
        <NFormItem label="订单状态" path="orderStatus">
          <NSelect
            v-model:value="model.orderStatus"
            placeholder="请选择订单状态"
            :options="orderStatusOptions"
            :disabled="operateType === 'edit'"
          />
        </NFormItem>
        <NFormItem label="地址信息" path="addressId">
          <NInput 
            v-model:value="model.addressId" 
            placeholder="请输入地址信息"
            type="textarea"
            :autosize="{
              minRows: 2,
              maxRows: 4
            }"
            :readonly="operateType === 'edit'"
          />
        </NFormItem>
        <NFormItem label="优惠券ID" path="couponId">
          <NInput 
            v-model:value="model.couponId" 
            placeholder="请输入优惠券ID"
            :readonly="operateType === 'edit'"
          />
        </NFormItem>
        <NFormItem label="分配ID" path="assignid">
          <NInput 
            v-model:value="model.assignid" 
            placeholder="请输入分配ID"
            :readonly="operateType === 'edit'"
          />
        </NFormItem>
        <NFormItem label="对账ID" path="reconId">
          <NInput 
            v-model:value="model.reconId" 
            placeholder="请输入对账ID"
            :readonly="operateType === 'edit'"
          />
        </NFormItem>
        
        <!-- 只有在编辑模式下显示创建时间 -->
        <NFormItem v-if="operateType === 'edit'" label="创建时间">
          <NInput :value="formatCreateTime" readonly />
        </NFormItem>
      </NForm>
      
      <template #footer>
        <NSpace justify="end">
          <NButton @click="closeDrawer">{{ operateType === 'edit' ? '关闭' : '取消' }}</NButton>
          <NButton 
            v-if="operateType === 'add' || operateType === 'edit'" 
            type="primary" 
            @click="handleSubmit"
          >
            {{ operateType === 'edit' ? '更新状态' : '确定' }}
          </NButton>
        </NSpace>
      </template>
    </NDrawerContent>
  </NDrawer>
</template>

<style scoped></style> 