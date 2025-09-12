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
  userName: string;
  password: string;
}

const model: FormModel = reactive({
  userName: 'Soybean',
  password: '123456'
});

// 不同角色的默认登录信息
const roleDefaults = {
  user: { userName: 'Soybean', password: '123456' },
  // rider: { userName: 'rider_demo', password: '123456' },
  merchant: { userName: 'test_1226', password: '123456' },
  // admin: { userName: 'admin', password: '123456' }
};

// 监听登录角色变化，更新默认值
watch(loginRole, (newRole) => {
  const defaults = roleDefaults[newRole];
  model.userName = defaults.userName;
  model.password = defaults.password;
}, { immediate: true });

const rules = computed<Record<keyof FormModel, App.Global.FormRule[]>>(() => {
  const { formRules } = useFormRules();

  return {
    userName: formRules.userName,
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
  await authStore.login(model.userName, model.password);
}

// 登录角色选项
const loginRoleOptions = computed(() => {
  // 使用现有的翻译键和手动映射相结合
  return [
    { label: $t('page.login.pwdLogin.user'), value: 'user' as const },
    { 
      label: appStore.locale === 'zh-CN' ? '骑手' : 'Rider', 
      value: 'rider' as const 
    },
    { 
      label: appStore.locale === 'zh-CN' ? '商家' : 'Merchant', 
      value: 'merchant' as const 
    },
    { label: $t('page.login.pwdLogin.admin'), value: 'admin' as const },
  ];
});

// 登录方式选项  
const loginMethodOptions = computed(() => {
  return [
    { 
      label: appStore.locale === 'zh-CN' ? '用户名登录' : 'Username Login', 
      value: 'username' as const 
    },
    { 
      label: appStore.locale === 'zh-CN' ? '邮箱登录' : 'Email Login', 
      value: 'email' as const 
    },
    { 
      label: appStore.locale === 'zh-CN' ? '手机号登录' : 'Phone Login', 
      value: 'phone' as const 
    }
  ];
});

// 监听登录方式变化，自动跳转
watch(loginMethod, (newMethod) => {
  if (newMethod === 'email' || newMethod === 'phone') {
    setTimeout(() => {
      toggleLoginModule('code-login');
    }, 300);
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
      <NFormItem path="userName">
        <div class="w-full">
          <label class="form-label">
            {{ appStore.locale === 'zh-CN' ? '用户名' : 'Username' }}
          </label>
          <NInput 
            v-model:value="model.userName" 
            :placeholder="$t('page.login.common.userNamePlaceholder')"
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
            :placeholder="$t('page.login.common.passwordPlaceholder')"
          />
        </div>
      </NFormItem>
    </template>

    <template v-else>
      <NFormItem>
        <div class="w-full">
          <NAlert type="info">
            {{ loginMethod === 'email' 
              ? (appStore.locale === 'zh-CN' ? '将跳转到邮箱验证码登录页面' : 'Redirecting to email verification login page')
              : (appStore.locale === 'zh-CN' ? '将跳转到手机验证码登录页面' : 'Redirecting to phone verification login page') 
            }}
          </NAlert>
        </div>
      </NFormItem>
    </template>

    <NSpace vertical :size="18">
      <div v-if="loginMethod === 'username'" class="flex-y-center justify-between">
        <NCheckbox>{{ $t('page.login.pwdLogin.rememberMe') }}</NCheckbox>
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
          : (appStore.locale === 'zh-CN' ? '继续' : 'Continue') 
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
