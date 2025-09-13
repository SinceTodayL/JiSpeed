<script setup lang="ts">
import { computed, onMounted, reactive, watch } from 'vue';
import { useAppStore } from '@/store/modules/app';
import { useAuthStore } from '@/store/modules/auth';
import { useFormRules, useNaiveForm } from '@/hooks/common/form';
import { useRouterPush } from '@/hooks/common/router';
import { $t } from '@/locales';
import { getDefaultCredentials, ROLE_CONFIG } from '@/constants/login-defaults';

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

// 从配置文件获取默认账号密码
const roleDefaultCredentials = getDefaultCredentials();

// 响应式表单模型
const model: FormModel = reactive({
  loginKey: roleDefaultCredentials[props.selectedRole].loginKey,
  password: roleDefaultCredentials[props.selectedRole].password
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
  const userType = ROLE_CONFIG[props.selectedRole].userType;
  const loginResult = await authStore.login(model.loginKey, model.password, userType);

  // 只有登录成功时才执行跳转（auth store内部已经处理了跳转逻辑）
  if (!loginResult.success) {
    // 登录失败，不做任何跳转，错误信息已在auth store中显示
    console.log('登录失败:', loginResult.error);
    return;
  }
  
  // 登录成功，auth store已经处理了跳转逻辑，这里不需要额外操作
  console.log('登录成功');
}

// 这个函数现在不再需要，因为auth store内部已经处理了跳转逻辑
// 保留函数定义以防其他地方引用，但不执行任何操作
async function redirectToModule(role: 'user' | 'rider' | 'merchant' | 'admin') {
  // 跳转逻辑已移至auth store中处理
  console.log('跳转逻辑已由auth store处理');
}

// 监听角色变化，自动更新默认账号密码
watch(() => props.selectedRole, (newRole) => {
  const defaultCredentials = roleDefaultCredentials[newRole];
  model.loginKey = defaultCredentials.loginKey;
  model.password = defaultCredentials.password;
}, { immediate: true });

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
