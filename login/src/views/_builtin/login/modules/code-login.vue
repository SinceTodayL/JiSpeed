<script setup lang="ts">
import { computed, reactive, ref } from 'vue';
import { useRouterPush } from '@/hooks/common/router';
import { useFormRules, useNaiveForm } from '@/hooks/common/form';
import { useCaptcha } from '@/hooks/business/captcha';
import { $t } from '@/locales';

defineOptions({
  name: 'CodeLogin'
});

const { toggleLoginModule } = useRouterPush();
const { formRef, validate } = useNaiveForm();
const { label, isCounting, loading, getCaptcha } = useCaptcha();

// 登录方式选择
const loginMethod = ref<'phone' | 'email'>('phone');

interface FormModel {
  phone: string;
  email: string;
  code: string;
}

const model: FormModel = reactive({
  phone: '',
  email: '',
  code: ''
});

const rules = computed<Record<keyof FormModel, App.Global.FormRule[]>>(() => {
  const { formRules } = useFormRules();

  return {
    phone: formRules.phone,
    email: formRules.email,
    code: formRules.code
  };
});

// 获取验证码的联系方式
const contactValue = computed(() => {
  return loginMethod.value === 'phone' ? model.phone : model.email;
});

async function handleSubmit() {
  await validate();
  // request
  window.$message?.success($t('page.login.common.validateSuccess'));
}

// 登录方式选项
const loginMethodOptions = computed(() => [
  { label: '邮箱登录', value: 'email' as const },
  { label: '手机号登录', value: 'phone' as const }
]);
</script>

<template>
  <NForm ref="formRef" :model="model" :rules="rules" size="large" :show-label="false" @keyup.enter="handleSubmit">
    <!-- 登录方式选择 -->
    <NFormItem>
      <NRadioGroup v-model:value="loginMethod" class="w-full">
        <NSpace>
          <NRadio v-for="option in loginMethodOptions" :key="option.value" :value="option.value">
            {{ option.label }}
          </NRadio>
        </NSpace>
      </NRadioGroup>
    </NFormItem>
    
    <!-- 手机号输入 -->
    <NFormItem v-if="loginMethod === 'phone'" path="phone">
      <NInput v-model:value="model.phone" :placeholder="$t('page.login.common.phonePlaceholder')" />
    </NFormItem>
    
    <!-- 邮箱输入 -->
    <NFormItem v-if="loginMethod === 'email'" path="email">
      <NInput v-model:value="model.email" :placeholder="$t('page.login.common.emailPlaceholder')" />
    </NFormItem>
    
    <!-- 验证码输入 -->
    <NFormItem path="code">
      <div class="w-full flex-y-center gap-16px">
        <NInput v-model:value="model.code" :placeholder="$t('page.login.common.codePlaceholder')" />
        <NButton size="large" :disabled="isCounting" :loading="loading" @click="getCaptcha(contactValue)">
          {{ label }}
        </NButton>
      </div>
    </NFormItem>
    <NSpace vertical :size="18" class="w-full">
      <NButton type="primary" size="large" round block @click="handleSubmit">
        {{ $t('common.confirm') }}
      </NButton>
      <NButton size="large" round block @click="toggleLoginModule('pwd-login')">
        {{ $t('page.login.common.back') }}
      </NButton>
    </NSpace>
  </NForm>
</template>

<style scoped></style>
