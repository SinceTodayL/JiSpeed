<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue';
import { useAuthStore } from '@/store/modules/auth';
import { useRouterPush } from '@/hooks/common/router';
import { useFormRules, useNaiveForm } from '@/hooks/common/form';
import { useAppStore } from '@/store/modules/app';
import { $t } from '@/locales';

defineOptions({
  name: 'PwdLogin'
});

const authStore = useAuthStore();
const appStore = useAppStore();
const { toggleLoginModule } = useRouterPush();
const { formRef, validate } = useNaiveForm();

// 登录角色选择
const loginRole = ref<'user' | 'rider' | 'merchant' | 'admin'>('user');

// 登录方式选择
const loginMethod = ref<'username' | 'email' | 'phone'>('username');

interface FormModel {
  loginKey: string;  // 可以是用户名或邮箱
  password: string;
}

const model: FormModel = reactive({
  loginKey: 'TongJi',
  password: '6547265472'
});

const rules = computed<Record<keyof FormModel, App.Global.FormRule[]>>(() => {
  const { formRules } = useFormRules();

  return {
    loginKey: [
      { required: true, message: '请输入用户名或邮箱', trigger: 'blur' }
    ],
    password: formRules.pwd
  };
});

async function handleSubmit() {
  if (loginMethod.value !== 'username') {
    // 如果选择的不是用户名密码登录，跳转到code-login页面
    toggleLoginModule('code-login');
    return;
  }
  
  await validate();
  
  // 根据选择的角色确定userType
  const userTypeMap = {
    'user': 1,
    'merchant': 2,
    'rider': 3,
    'admin': 4
  };
  
  const userType = userTypeMap[loginRole.value];
  await authStore.login(model.loginKey, model.password, userType);
}

// 登录角色选项
const loginRoleOptions = computed(() => {
  // 使用现有的翻译键和手动映射相结合
  return [
    { label: $t('page.login.pwdLogin.user'), value: 'user' as const },
    { 
      label: $t('page.login.pwdLogin.rider'), 
      value: 'rider' as const 
    },
    { 
      label: $t('page.login.pwdLogin.merchant'), 
      value: 'merchant' as const 
    },
    { label: $t('page.login.pwdLogin.admin'), value: 'admin' as const },
  ];
});

// 登录方式选项  
const loginMethodOptions = computed(() => {
  return [
    { 
      label: $t('page.login.pwdLogin.usernameLogin'), 
      value: 'username' as const 
    },
    { 
      label: $t('page.login.pwdLogin.emailLogin'), 
      value: 'email' as const 
    },
    { 
      label: $t('page.login.pwdLogin.phoneLogin'), 
      value: 'phone' as const 
    }
  ];
});

// 监听登录方式变化，自动跳转
watch(loginMethod, (newMethod) => {
  if (newMethod === 'email' || newMethod === 'phone') {
    setTimeout(() => {
      toggleLoginModule('code-login');
    }, 0);
  }
});
</script>

<template>
  <NForm ref="formRef" :model="model" :rules="rules" size="large" :show-label="false" @keyup.enter="handleSubmit">

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

    <NFormItem>
      <div class="w-full">
        <label class="form-label">{{ $t('page.login.pwdLogin.loginMethod') }}</label>
        <NSelect
          v-model:value="loginMethod"
          :options="loginMethodOptions"
          :placeholder="$t('page.login.common.selectMethodPlaceholder')"
        />
      </div>
    </NFormItem>

    <template v-if="loginMethod === 'username'">
      <NFormItem path="loginKey">
        <div class="w-full">
          <label class="form-label">
            {{ appStore.locale === 'zh-CN' ? '用户名或邮箱' : 'Username or Email' }}
          </label>
          <NInput 
            v-model:value="model.loginKey" 
            :placeholder="appStore.locale === 'zh-CN' ? '请输入用户名或邮箱' : 'Enter username or email'"
          />
        </div>
      </NFormItem>
      <NFormItem path="password">
        <div class="w-full">
          <label class="form-label">
            {{ appStore.locale === 'zh-CN' ? '密码' : 'Password' }}
          </label>
          <NInput
            v-model:value="model.password"
            type="password"
            show-password-on="click"
            :placeholder="appStore.locale === 'zh-CN' ? '请输入密码' : 'Enter password'"
          />
        </div>
      </NFormItem>
    </template>

    <template v-else>
      <NFormItem>
        <div class="w-full">
          <NAlert type="info">
            {{ loginMethod === 'email' 
              ? $t('page.login.common.redirectToEmailLogin')
              : $t('page.login.common.redirectToPhoneLogin')
            }}
          </NAlert>
        </div>
      </NFormItem>
    </template>

    <NSpace vertical :size="18">
      <div v-if="loginMethod === 'username'" class="flex-y-center justify-between">
        <NButton quaternary @click="toggleLoginModule('register')">
          {{ $t('page.login.pwdLogin.noAccountRegister') }}
        </NButton>
        <NButton quaternary @click="toggleLoginModule('reset-pwd')">
          {{ $t('page.login.pwdLogin.forgetPassword') }}
        </NButton>
      </div>
      
      <NButton 
        type="primary" 
        size="large" 
        round 
        block 
        :loading="authStore.loginLoading" 
        @click="handleSubmit"
      >
        {{ loginMethod === 'username' 
          ? $t('common.confirm') 
          : $t('page.login.common.continue')
        }}
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

/* 确保所有表单项有统一的间距 */
:deep(.n-form-item) {
  margin-bottom: 4px;
}

/* 最后一个表单项不需要底部边距 */
:deep(.n-form-item:last-child) {
  margin-bottom: 0;
}

/* 提示框样式优化 */
:deep(.n-alert) {
  border-radius: 6px;
}
</style>
