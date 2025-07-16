<template>
  <div class="login-container">
    <n-card class="login-card" :bordered="false" content-style="padding: 0;">
      <div class="login-wrapper">
        <div class="branding-panel">
          <div class="branding-content">
            <h2>商家管理后台</h2>
          </div>
        </div>
        <div class="form-panel">
          <h3>欢迎回来</h3>
          <n-form ref="formRef" :model="formValue" :rules="rules" @submit.prevent="handleLogin">
            <n-form-item path="username" label="用户名">
              <n-input v-model:value="formValue.username" placeholder="输入任意用户名" />
            </n-form-item>
            <n-form-item path="password" label="密码">
              <n-input
                v-model:value="formValue.password"
                type="password"
                placeholder="输入任意密码"
                show-password-on="mousedown"
              />
            </n-form-item>
            <n-button type="primary" attr-type="submit" block size="large">
              登 录
            </n-button>
          </n-form>
        </div>
      </div>
    </n-card>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useMessage, NCard, NForm, NFormItem, NInput, NButton } from 'naive-ui';

const router = useRouter();
const message = useMessage();
const formRef = ref(null);

const formValue = ref({
  username: '',
  password: '',
});

const rules = {
  username: {
    required: true,
    message: '请输入用户名',
    trigger: 'blur',
  },
  password: {
    required: true,
    message: '请输入密码',
    trigger: 'blur',
  },
};

const handleLogin = (e) => {
  formRef.value?.validate((errors) => {
    if (!errors) {
      message.success('登录成功');
      router.push('/merchant/main');
    } else {
      message.error('请输入用户名和密码');
    }
  });
};
</script>

<style scoped>
.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background-color: #f0f2f5;
}

.login-card {
  width: 800px;
  height: 500px;
  border-radius: 8px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.login-wrapper {
  display: flex;
  width: 100%;
  height: 100%;
}

.branding-panel {
  width: 50%;
  background-color: #fafafa;
  color: #333;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 40px;
  text-align: center;
}

.branding-content h2 {
  font-size: 2.5rem;
  margin: 0 0 16px 0;
  font-weight: 300;
}

.branding-content p {
  font-size: 1rem;
  opacity: 0.8;
}

.form-panel {
  width: 50%;
  padding: 40px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  background-color: #ffffff;
}

.form-panel h3 {
  font-size: 24px;
  font-weight: 600;
  margin-bottom: 24px;
  color: #333;
}
</style> 