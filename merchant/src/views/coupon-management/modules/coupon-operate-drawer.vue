<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue';
import type { FormInst, FormRules } from 'naive-ui';
import { $t } from '@/locales';
import { addCoupon, updateCoupon } from '@/service/api';

interface Props {
  operateType: 'add' | 'edit';
  rowData?: Api.Coupon.CouponItem | null;
}

interface Emits {
  (e: 'submitted'): void;
}

defineOptions({
  name: 'CouponOperateDrawer'
});

const props = defineProps<Props>();

const emit = defineEmits<Emits>();

const visible = defineModel<boolean>('visible', {
  default: false
});

const title = computed(() => {
  return props.operateType === 'add' ? '添加优惠券' : '编辑优惠券';
});

type Model = Pick<Api.Coupon.CouponItem, 'couponId' | 'faceValue' | 'threshold' | 'totalQty' | 'issuedQty'> & {
  startTime: string | null;
  endTime: string | null;
};

const model: Model = reactive({
  couponId: '',
  faceValue: 0,
  threshold: 0,
  totalQty: 0,
  issuedQty: 0,
  startTime: null,
  endTime: null
});

const rules: FormRules = {
  couponId: {
    required: true,
    message: '请输入优惠券ID',
    trigger: 'blur'
  },
  faceValue: {
    required: true,
    type: 'number',
    message: '请输入面值',
    trigger: 'blur'
  },
  threshold: {
    required: true,
    type: 'number', 
    message: '请输入使用门槛',
    trigger: 'blur'
  },
  totalQty: {
    required: true,
    type: 'number',
    message: '请输入总数量',
    trigger: 'blur'
  },
  startTime: {
    required: true,
    message: '请选择开始时间',
    trigger: 'blur'
  },
  endTime: {
    required: true,
    message: '请选择结束时间',
    trigger: 'blur'
  }
};

const formRef = ref<FormInst | null>(null);

function handleInitModel() {
  Object.assign(model, {
    couponId: '',
    faceValue: 0,
    threshold: 0,
    totalQty: 0,
    issuedQty: 0,
    startTime: null,
    endTime: null
  });

  if (props.operateType === 'edit' && props.rowData) {
    Object.assign(model, {
      couponId: props.rowData.couponId,
      faceValue: props.rowData.faceValue,
      threshold: props.rowData.threshold,
      totalQty: props.rowData.totalQty,
      issuedQty: props.rowData.issuedQty,
      startTime: props.rowData.startTime,
      endTime: props.rowData.endTime
    });
  }
}

function closeDrawer() {
  visible.value = false;
}

async function handleSubmit() {
  try {
    await formRef.value?.validate();
    
    // 处理日期字段的类型转换
    const submitData = {
      ...model,
      startTime: model.startTime || '',
      endTime: model.endTime || ''
    };
    
    if (props.operateType === 'add') {
      await addCoupon(submitData);
    } else {
      await updateCoupon(submitData);
    }
    
    window.$message?.success(`优惠券${props.operateType === 'add' ? '添加' : '编辑'}成功`);
    
    closeDrawer();
    emit('submitted');
  } catch (error) {
    console.error('表单验证失败:', error);
    window.$message?.error('请检查表单填写是否正确');
  }
}

watch(visible, () => {
  if (visible.value) {
    handleInitModel();
  }
});
</script>

<template>
  <NDrawer v-model:show="visible" display-directive="show" :width="640">
    <NDrawerContent :title="title" :native-scrollbar="false" closable>
      <NForm ref="formRef" :model="model" :rules="rules">
        <NFormItem label="优惠券ID" path="couponId">
          <NInput 
            v-model:value="model.couponId" 
            placeholder="请输入优惠券ID"
            :disabled="props.operateType === 'edit'"
          />
        </NFormItem>
        <NFormItem label="面值" path="faceValue">
          <NInputNumber 
            v-model:value="model.faceValue" 
            placeholder="请输入面值"
            :min="0"
            :precision="2"
            class="w-full"
          >
            <template #prefix>¥</template>
          </NInputNumber>
        </NFormItem>
        <NFormItem label="使用门槛" path="threshold">
          <NInputNumber 
            v-model:value="model.threshold" 
            placeholder="请输入使用门槛"
            :min="0"
            :precision="2"
            class="w-full"
          >
            <template #prefix>¥</template>
          </NInputNumber>
        </NFormItem>
        <NFormItem label="总数量" path="totalQty">
          <NInputNumber 
            v-model:value="model.totalQty" 
            placeholder="请输入总数量"
            :min="0"
            class="w-full"
          >
            <template #suffix>张</template>
          </NInputNumber>
        </NFormItem>
        <NFormItem label="已发放数量" path="issuedQty">
          <NInputNumber 
            v-model:value="model.issuedQty" 
            placeholder="请输入已发放数量"
            :min="0"
            :max="model.totalQty"
            class="w-full"
          >
            <template #suffix>张</template>
          </NInputNumber>
        </NFormItem>
        <NFormItem label="开始时间" path="startTime">
          <NDatePicker
            v-model:formatted-value="model.startTime"
            type="datetime"
            format="yyyy-MM-dd HH:mm:ss"
            placeholder="请选择开始时间"
            class="w-full"
          />
        </NFormItem>
        <NFormItem label="结束时间" path="endTime">
          <NDatePicker
            v-model:formatted-value="model.endTime"
            type="datetime"
            format="yyyy-MM-dd HH:mm:ss"
            placeholder="请选择结束时间"
            class="w-full"
          />
        </NFormItem>
      </NForm>
      <template #footer>
        <NSpace class="w-full pt-16px">
          <NButton class="flex-1" @click="closeDrawer">{{ $t('common.cancel') }}</NButton>
          <NButton class="flex-1" type="primary" @click="handleSubmit">{{ $t('common.confirm') }}</NButton>
        </NSpace>
      </template>
    </NDrawerContent>
  </NDrawer>
</template>

<style scoped></style> 