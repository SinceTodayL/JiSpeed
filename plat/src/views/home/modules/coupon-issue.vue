<script setup lang="ts">
import { ref } from 'vue';
import { issueCoupon } from '@/api/coupon';
import { useMessage, NRadioGroup, NRadio, NSpace } from 'naive-ui';

defineOptions({ name: 'CouponIssue' });

const message = useMessage();
const show = ref<boolean>(false);
const form = ref({
  distributionType: 'all', // 'all' 所有用户, 'specific' 指定用户
  userId: '',
  faceValue: 10,
  threshold: 0,
  timeRange: null as null | [number, number]
});

const distributionOptions = [
  { label: '发放给所有用户', value: 'all' },
  { label: '发放给指定用户', value: 'specific' }
];

async function submit() {
  if (!form.value.timeRange) {
    message.warning('请选择有效期');
    return;
  }
  
  if (form.value.distributionType === 'specific' && !form.value.userId) {
    message.warning('请输入用户ID');
    return;
  }

  const [start, end] = form.value.timeRange as [number, number];
  const payload = {
    StartTime: new Date(start).toISOString(),
    EndTime: new Date(end).toISOString(),
    FaceValue: Number(form.value.faceValue) || 0,
    Threshold: Number(form.value.threshold) || 0,
    UserId: form.value.distributionType === 'all' ? null : form.value.userId
  };

  try {
    const adminId = '6f7af74d972c481c91f19596e07aae3a'; // 使用真实管理员ID
    await issueCoupon(adminId, payload);
    message.success(form.value.distributionType === 'all' ? '已向所有用户发放优惠券' : '已发放给指定用户');
    show.value = false;
    // 重置表单
    form.value = {
      distributionType: 'all',
      userId: '',
      faceValue: 10,
      threshold: 0,
      timeRange: null
    };
  } catch (err) {
    message.error('发放失败');
    console.error('优惠券发放失败:', err);
  }
}
</script>

<template>
  <NCard :title="$t('page.home.coupon.title')" :bordered="false" class="card-wrapper">
    <div class="text-center py-6">
      <div class="mb-4">
        <div class="mx-auto w-16 h-16 bg-gradient-to-r from-yellow-400 to-orange-500 rounded-full flex items-center justify-center mb-3">
          <SvgIcon icon="material-symbols:local-offer" class="text-2xl text-white" />
        </div>
        <p class="text-gray-600 text-sm">为所有用户或指定用户发放优惠券，提升用户体验</p>
      </div>
      
      <NButton 
        type="primary" 
        size="medium"
        @click="show = true"
        class="bg-gradient-to-r from-yellow-500 to-orange-500 border-none hover:from-yellow-600 hover:to-orange-600 rounded-lg"
      >
        <template #icon>
          <SvgIcon icon="material-symbols:redeem" />
        </template>
        {{ $t('page.home.coupon.issue') }}
      </NButton>
    </div>

    <NModal 
      v-model:show="show" 
      preset="card" 
      style="width: 520px" 
      title="发放优惠券"
      class="rounded-xl"
    >
      <NForm :model="form" label-placement="left" label-width="100">
        <NFormItem label="发放方式" required>
          <NRadioGroup v-model:value="form.distributionType" class="w-full">
            <NSpace>
              <NRadio value="all">发放给所有用户</NRadio>
              <NRadio value="specific">发放给指定用户</NRadio>
            </NSpace>
          </NRadioGroup>
        </NFormItem>
        
        <NFormItem 
          v-if="form.distributionType === 'specific'" 
          label="用户ID" 
          required
        >
          <NInput 
            v-model:value="form.userId" 
            placeholder="请输入用户ID"
            class="rounded-lg"
          />
        </NFormItem>
        <NFormItem label="面值（元）" required>
          <NInputNumber 
            v-model:value="form.faceValue"
            placeholder="请输入面值"
            :min="1"
            :max="1000"
            class="w-full rounded-lg"
          />
        </NFormItem>
        <NFormItem label="门槛（元）">
          <NInputNumber 
            v-model:value="form.threshold"
            placeholder="请输入使用门槛"
            :min="0"
            :max="10000"
            class="w-full rounded-lg"
          />
        </NFormItem>
        <NFormItem label="有效期" required>
          <NDatePicker 
            v-model:value="form.timeRange" 
            type="datetimerange" 
            clearable
            class="w-full rounded-lg"
          />
        </NFormItem>
        
        <div class="bg-gray-50 p-4 rounded-lg mb-4">
          <h4 class="text-sm font-medium text-gray-700 mb-2">优惠券预览</h4>
          <div class="bg-gradient-to-r from-yellow-400 to-orange-500 text-white p-3 rounded-lg">
            <div class="flex justify-between items-center">
              <div>
                <div class="text-lg font-bold">¥{{ form.faceValue || 0 }}</div>
                <div class="text-xs opacity-90">
                  {{ form.threshold > 0 ? `满${form.threshold}可用` : '无门槛' }}
                </div>
              </div>
              <SvgIcon icon="material-symbols:local-offer" class="text-xl" />
            </div>
          </div>
        </div>

        <div class="flex space-x-3 mt-4 pt-4">
          <NButton @click="show = false" class="flex-1 rounded-lg">
            取消
          </NButton>
          <NButton 
            type="primary" 
            @click="submit"
            class="flex-1 bg-gradient-to-r from-yellow-500 to-orange-500 border-none rounded-lg"
          >
            确认发放
          </NButton>
        </div>
      </NForm>
    </NModal>
  </NCard>
</template>

<style scoped></style>


