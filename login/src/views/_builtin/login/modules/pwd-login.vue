<script setup lang="ts">
import { computed, onMounted, reactive, watch } from 'vue';
import { useAppStore } from '@/store/modules/app';
import { useAuthStore } from '@/store/modules/auth';
import { useFormRules, useNaiveForm } from '@/hooks/common/form';
import { useRouterPush } from '@/hooks/common/router';
import { $t } from '@/locales';

defineOptions({
  name: 'PwdLogin'
});

interface Props {
  selectedRole?: 'user' | 'rider' | 'merchant' | 'admin';
}

const props = withDefaults(defineProps<Props>(), {
  selectedRole: 'user'
});

const authStore = useAuthStore();
const appStore = useAppStore();
const { toggleLoginModule } = useRouterPush();
const { formRef, validate } = useNaiveForm();

interface FormModel {
  loginKey: string;  // 用户名或邮箱
  password: string;
}

// By default
const model: FormModel = reactive({
<<<<<<< HEAD
  loginKey: roleDefaultCredentials[props.selectedRole].loginKey,
  password: roleDefaultCredentials[props.selectedRole].password
=======
  loginKey: 'TongJi',
  password: '6547265472'
>>>>>>> 7080accb4b1095753f1e0ab7f755de7175bca229
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
  await validate();

  // 根据选择的角色确定userType
<<<<<<< HEAD
  const userType = ROLE_CONFIG[props.selectedRole].userType;
  const loginResult = await authStore.login(model.loginKey, model.password, userType);
=======
  const userTypeMap = {
    'user': 1,
    'merchant': 2,
    'rider': 3,
    'admin': 4
  };
>>>>>>> 7080accb4b1095753f1e0ab7f755de7175bca229

  const userType = userTypeMap[loginRole.value];
  await authStore.login(model.loginKey, model.password, userType);

  // 登录成功后跳转到相应的模块
  await redirectToModule(loginRole.value);
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

<<<<<<< HEAD
// 监听角色变化，自动更新默认账号密码
watch(() => props.selectedRole, (newRole) => {
  const defaultCredentials = roleDefaultCredentials[newRole];
  model.loginKey = defaultCredentials.loginKey;
  model.password = defaultCredentials.password;
}, { immediate: true });
=======
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
>>>>>>> 7080accb4b1095753f1e0ab7f755de7175bca229

// 检查登录前状态
onMounted(async () => {
  // 注释掉自动检查认证状态的逻辑，防止退出登录后自动跳转
  // const isAuthenticated = await authStore.checkAuthBeforeLogin();
  // if (isAuthenticated) {
  //   // checkAuthBeforeLogin方法已经处理了跳转逻辑
  //   console.log('User already authenticated, redirecting...');
  // }

  // 直接显示登录页面，不进行自动认证检查
  console.log('Showing login page without auto auth check');
});
</script>

<template>
  <NForm ref="formRef" :model="model" :rules="rules" size="large" :show-label="false" @keyup.enter="handleSubmit">
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

    <NSpace vertical :size="18">
      <div class="flex-y-center justify-between">
        <NButton quaternary @click="toggleLoginModule('register')">
          {{ $t('page.login.pwdLogin.noAccountRegister') }}
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
        {{ $t('common.confirm') }}
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

:deep(.n-alert) {
  border-radius: 6px;
}
</style>
