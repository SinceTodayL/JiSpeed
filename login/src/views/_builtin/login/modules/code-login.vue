<script setup lang="ts">
import { computed, reactive, ref } from 'vue';
import { useAuthStore } from '@/store/modules/auth';
import { useCaptcha } from '@/hooks/business/captcha';
import { useFormRules, useNaiveForm } from '@/hooks/common/form';
import { useRouterPush } from '@/hooks/common/router';
import { $t } from '@/locales';

defineOptions({
  name: 'CodeLogin'
});

interface Props {
  selectedRole?: 'user' | 'rider' | 'merchant' | 'admin';
}

const props = withDefaults(defineProps<Props>(), {
  selectedRole: 'user'
});

const { toggleLoginModule } = useRouterPush();
const { formRef, validate } = useNaiveForm();
const { label, isCounting, loading, getCaptcha } = useCaptcha();
const authStore = useAuthStore();

// 登录角色选择 - 现在从 props 获取
const loginRole = ref<'user' | 'rider' | 'merchant' | 'admin'>(props.selectedRole);

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

  // 根据选择的角色确定userType
  const userTypeMap = {
    'user': 1,
    'merchant': 2,
    'rider': 3,
    'admin': 4
  };

  const userType = userTypeMap[loginRole.value];

  // 调用验证码登录接口
  const loginResult = await authStore.codeLogin(contactValue.value, model.code, userType);

  // 如果登录成功，根据角色跳转到相应的模块
  if (loginResult && loginResult.success) {
    await redirectToModule(loginRole.value);
  }
}

// 根据用户角色跳转到相应的模块
async function redirectToModule(role: 'user' | 'rider' | 'merchant' | 'admin') {
  const moduleUrls = {
    'user': import.meta.env.VITE_USER_FRONTEND_URL,
    'merchant': import.meta.env.VITE_MERCHANT_FRONTEND_URL,
    'rider': import.meta.env.VITE_RIDER_FRONTEND_URL,
    'admin': import.meta.env.VITE_ADMIN_FRONTEND_URL
  };

  const targetUrl = moduleUrls[role];

  if (targetUrl) {
    // 延迟跳转，确保登录状态已保存
    setTimeout(() => {
      window.location.href = targetUrl;
    }, 500);
  } else {
    console.warn(`No frontend URL configured for role: ${role}`);
  }
}

// 登录角色选项
const loginRoleOptions = computed(() => [
  { label: $t('page.login.pwdLogin.user'), value: 'user' as const },
  { label: $t('page.login.pwdLogin.rider'), value: 'rider' as const },
  { label: $t('page.login.pwdLogin.merchant'), value: 'merchant' as const },
  { label: $t('page.login.pwdLogin.admin'), value: 'admin' as const }
]);

// 登录方式选项
const loginMethodOptions = computed(() => [
  { label: '邮箱登录', value: 'email' as const },
  { label: '手机号登录', value: 'phone' as const }
]);
</script>

<template>
  <NForm ref="formRef" :model="model" :rules="rules" size="large" :show-label="false" @keyup.enter="handleSubmit">
    <!-- 登录角色选择 -->
    <NFormItem>
      <div class="w-full">
        <label class="form-label">{{ $t('page.login.pwdLogin.loginRole') }}</label>
        <NSelect
          v-model:value="loginRole"
          :options="loginRoleOptions"
          :placeholder="$t('page.login.common.selectRolePlaceholder')"
        />
      </div>
    </NFormItem>

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
      <div class="w-full">
        <label class="form-label">手机号</label>
        <NInput v-model:value="model.phone" placeholder="请输入手机号" />
      </div>
    </NFormItem>

    <!-- 邮箱输入 -->
    <NFormItem v-if="loginMethod === 'email'" path="email">
      <div class="w-full">
        <label class="form-label">邮箱</label>
        <NInput v-model:value="model.email" placeholder="请输入邮箱" />
      </div>
    </NFormItem>

    <!-- 验证码输入 -->
    <NFormItem path="code">
      <div class="w-full">
        <label class="form-label">验证码</label>
        <div class="w-full flex-y-center gap-16px">
          <NInput v-model:value="model.code" placeholder="请输入验证码" />
          <NButton size="large" :disabled="isCounting" :loading="loading" @click="getCaptcha(contactValue)">
            {{ label }}
          </NButton>
        </div>
      </div>
    </NFormItem>

    <NSpace vertical :size="18" class="w-full">
      <NButton
        type="primary"
        size="large"
        round
        block
        :loading="authStore.loginLoading"
        @click="handleSubmit"
      >
        {{ $t('common.confirm') }}
      </NButton>
      <NButton size="large" round block @click="toggleLoginModule('pwd-login')">
        {{ $t('page.login.common.back') }}
      </NButton>
    </NSpace>
  </NForm>
</template>

<style scoped>
.form-label {
  display: block;
  margin-bottom: 8px;
  font-size: 14px;
  font-weight: 500;
  color: #333;
}

:deep(.n-form-item) {
  margin-bottom: 4px;
}

:deep(.n-form-item:last-child) {
  margin-bottom: 0;
}
</style>
