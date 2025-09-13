<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue';
import { useRouterPush } from '@/hooks/common/router';
import { useFormRules, useNaiveForm } from '@/hooks/common/form';
import { useLoading } from '@sa/hooks';
import { $t } from '@/locales';
import { fetchRegister } from '@/service/api/auth';

defineOptions({
  name: 'Register'
});

interface Props {
  selectedRole?: 'user' | 'rider' | 'merchant' | 'admin';
}

const props = withDefaults(defineProps<Props>(), {
  selectedRole: 'user'
});

const { toggleLoginModule } = useRouterPush();
const { formRef, validate } = useNaiveForm();
const { loading: registerLoading, startLoading, endLoading } = useLoading();

// 直接使用从父组件传递的角色，不需要本地状态

// Contact type auto detection
const contactType = ref('email');

interface FormModel {
  userName: string;
  contact: string;
  password: string;
  confirmPassword: string;
}

const model: FormModel = reactive({
  userName: '',
  contact: '',
  password: '',
  confirmPassword: ''
});

const rules = computed<Record<keyof FormModel, App.Global.FormRule[]>>(() => {
  const { formRules, createConfirmPwdRule } = useFormRules();

  return {
    userName: formRules.userName,
    contact: [
      { required: true, message: '请输入邮箱或手机号', trigger: 'blur' }
    ],
    password: formRules.pwd,
    confirmPassword: createConfirmPwdRule(model.password)
  };
});

// 显示当前选中角色的标签
const currentRoleLabel = computed(() => {
  const roleLabels = {
    user: $t('page.login.pwdLogin.user'),
    rider: $t('page.login.pwdLogin.rider'),
    merchant: $t('page.login.pwdLogin.merchant'),
    admin: $t('page.login.pwdLogin.admin')
  };
  return roleLabels[props.selectedRole];
});

// Auto detect contact type based on input
const detectContactType = (value: string) => {
  if (value.includes('@')) {
    contactType.value = 'email';
  } else if (/^1[3-9]\d{9}$/.test(value)) {
    contactType.value = 'phone';
  }
};

// Watch contact input changes
watch(() => model.contact, (newValue: string) => {
  detectContactType(newValue);
});

async function handleSubmit() {
  // 防止重复提交
  if (registerLoading.value) {
    return;
  }

  try {
    await validate();
  } catch (error) {
    // 表单验证失败，直接返回
    return;
  }
  
  // 开始loading状态
  startLoading();
  
  try {
    const userTypeMap = {
      'user': 1,
      'merchant': 2, 
      'rider': 3,
      'admin': 4
    };
    
    const registerData = {
      UserName: model.userName,
      PassWord: model.password,
      Email: contactType.value === 'email' ? model.contact : '',
      PhoneNumber: contactType.value === 'phone' ? model.contact : undefined
    };
    
    // Call register API
    const { data: response, error } = await fetchRegister(registerData, userTypeMap[props.selectedRole]);
    
    // 检查是否有错误
    if (error) {
      window.$message?.error(error.message || '注册失败，请稍后重试');
      return;
    }
    
    // 检查后端响应
    if (response && response.code === 0) {
      // 注册成功，提示用户查收验证邮件
      window.$message?.success('注册成功！验证邮件已发送至您的邮箱，请点击邮件中的验证链接完成注册。', {
        duration: 6000
      });
      toggleLoginModule('pwd-login');
    } else {
      // 注册失败
      window.$message?.error(response?.message || '注册失败，请检查输入信息');
    }
    
  } catch (error) {
    console.error('Registration failed:', error);
    window.$message?.error('注册失败，请稍后重试');
  } finally {
    // 确保在所有情况下都结束loading状态
    endLoading();
  }
}
</script>

<template>
  <div class="register-container">
    <NForm ref="formRef" :model="model" :rules="rules" size="large" :show-label="false" @keyup.enter="handleSubmit">
      <!-- 显示当前注册角色 -->
      <div class="current-role-display">
        <span class="role-info">注册角色：<strong>{{ currentRoleLabel }}</strong></span>
      </div>

      <div class="form-row">
        <NFormItem path="userName" class="form-item">
          <NInput 
            v-model:value="model.userName" 
            placeholder="用户名"
          />
        </NFormItem>
        
        <NFormItem path="contact" class="form-item">
          <NInput 
            v-model:value="model.contact" 
            placeholder="邮箱或手机号"
          />
        </NFormItem>
      </div>

      <div class="form-row">
        <NFormItem path="password" class="form-item">
          <NInput
            v-model:value="model.password"
            type="password"
            show-password-on="click"
            placeholder="密码"
          />
        </NFormItem>

        <NFormItem path="confirmPassword" class="form-item">
          <NInput
            v-model:value="model.confirmPassword"
            type="password"
            show-password-on="click"
            placeholder="确认密码"
          />
        </NFormItem>
      </div>

      <NSpace vertical :size="18" class="w-full">
        <NButton 
          type="primary" 
          size="large" 
          round 
          block 
          :loading="registerLoading"
          :disabled="registerLoading"
          @click="handleSubmit"
        >
          {{ registerLoading ? '注册中...' : $t('common.confirm') }}
        </NButton>
        <NButton size="large" round block @click="toggleLoginModule('pwd-login')">
          {{ $t('page.login.common.back') }}
        </NButton>
      </NSpace>
    </NForm>
  </div>
</template>

<style scoped>
.register-container {
  width: 100%;
  max-width: 100%;
}

/* 当前角色显示样式 */
.current-role-display {
  margin-bottom: 24px;
  padding: 12px;
  background-color: var(--n-color-target);
  border-radius: 6px;
  text-align: center;
}

.role-info {
  font-size: 14px;
  color: var(--n-text-color);
}

.form-row {
  display: flex;
  gap: 16px;
  width: 100%;
}

.form-item {
  flex: 1;
  height: 48px;
}

:deep(.form-item .n-form-item-blank) {
  min-height: 48px;
  display: flex;
  align-items: center;
}

:deep(.n-input) {
  height: 40px;
}

:deep(.n-input__input-el) {
  height: 40px;
  line-height: 40px;
}

:deep(.n-select) {
  height: 40px;
}

:deep(.n-base-selection) {
  height: 40px;
}

:deep(.n-base-selection-input) {
  height: 40px;
}

:deep(.n-form-item) {
  margin-bottom: 16px;
}

:deep(.n-form-item:last-child) {
  margin-bottom: 0;
}
</style>
