<template>
  <div class="login-container">
    <!-- 背景装饰 -->
    <div class="background-decoration">
      <div class="circle circle-1"></div>
      <div class="circle circle-2"></div>
      <div class="circle circle-3"></div>
    </div>

    <!-- 登录卡片 -->
    <div class="login-card">
      <!-- 头部Logo区域 -->
      <div class="login-header">
        <div class="logo-container">
          <div class="logo-icon">⚡</div>
          <h1 class="app-title">急速</h1>
        </div>
        <p class="welcome-text">欢迎回来</p>
      </div>

      <!-- 登录表单 -->
      <div class="login-form">
        <form @submit.prevent="handleLogin">
          <!-- 用户名输入框 -->
          <div class="input-group">
            <label class="input-label">用户名</label>
            <div class="input-wrapper">
              <i class="input-icon">👤</i>
              <input
                v-model="loginForm.username"
                type="text"
                class="form-input"
                placeholder="请输入用户名"
                required
              />
            </div>
          </div>

          <!-- 密码输入框 -->
          <div class="input-group">
            <label class="input-label">密码</label>
            <div class="input-wrapper">
              <i class="input-icon">🔒</i>
              <input
                v-model="loginForm.password"
                :type="showPassword ? 'text' : 'password'"
                class="form-input"
                placeholder="请输入密码"
                required
              />
              <button
                type="button"
                class="password-toggle"
                @click="togglePassword"
              >
                {{ showPassword ? '👁️' : '👁️‍🗨️' }}
              </button>
            </div>
          </div>

          <!-- 记住密码和忘记密码 -->
          <div class="form-options">
            <label class="checkbox-container">
              <input
                v-model="rememberMe"
                type="checkbox"
                class="checkbox-input"
              />
              <span class="checkbox-custom"></span>
              <span class="checkbox-text">记住密码</span>
            </label>
            <a href="#" class="forgot-password">忘记密码？</a>
          </div>

          <!-- 登录按钮 -->
          <button
            type="submit"
            class="login-button"
            :class="{ loading: isLoading }"
            :disabled="isLoading"
          >
            <span v-if="!isLoading">登录</span>
            <span v-else class="loading-text">
              <span class="loading-spinner"></span>
              登录中...
            </span>
          </button>
        </form>

        <!-- 分割线 -->
        <div class="divider">
          <span class="divider-text">或</span>
        </div>

        <!-- 注册链接 -->
        <div class="register-section">
          <p class="register-text">
            还没有账号？
            <router-link to="/register" class="register-link">立即注册</router-link>
          </p>
        </div>
      </div>
    </div>

    <!-- 提示消息 -->
    <div
      v-if="message"
      class="message-toast"
      :class="messageType"
    >
      {{ message }}
    </div>
  </div>
</template>

<script>
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { authAPI } from '@/api/user.js'

export default {
  name: 'Login',
  setup() {
    const router = useRouter()
    
    // 响应式数据
    const loginForm = reactive({
      username: '',
      password: ''
    })
    
    const showPassword = ref(false)
    const rememberMe = ref(false)
    const isLoading = ref(false)
    const message = ref('')
    const messageType = ref('')

    // 方法
    const togglePassword = () => {
      showPassword.value = !showPassword.value
    }

    const showMessage = (msg, type = 'info') => {
      message.value = msg
      messageType.value = type
      setTimeout(() => {
        message.value = ''
        messageType.value = ''
      }, 3000)
    }

    const handleLogin = async () => {
      if (!loginForm.username || !loginForm.password) {
        showMessage('请填写完整的登录信息', 'error')
        return
      }

      isLoading.value = true
      
      try {
        const response = await authAPI.login({
          username: loginForm.username,
          password: loginForm.password
        })
        
        console.log('登录响应:', response)
        
        if (response.success || response.code === 200) {
          showMessage('登录成功！', 'success')
          
          // 如果选择记住密码，可以在这里存储到localStorage
          if (rememberMe.value) {
            localStorage.setItem('rememberedUsername', loginForm.username)
          }
          
          // 存储用户信息和token
          if (response.data && response.data.token) {
            localStorage.setItem('token', response.data.token)
            localStorage.setItem('userInfo', JSON.stringify(response.data.user))
          }
          
          // 延迟跳转，让用户看到成功提示
          setTimeout(() => {
            router.push('/')
          }, 1500)
        } else {
          showMessage(response.message || '登录失败，请检查用户名和密码', 'error')
        }
      } catch (error) {
        console.error('登录错误:', error)
        showMessage('登录失败，请检查网络连接或联系管理员', 'error')
      } finally {
        isLoading.value = false
      }
    }

    // 组件挂载时检查是否有记住的用户名
    const initRememberedUsername = () => {
      const remembered = localStorage.getItem('rememberedUsername')
      if (remembered) {
        loginForm.username = remembered
        rememberMe.value = true
      }
    }

    // 初始化
    initRememberedUsername()

    return {
      loginForm,
      showPassword,
      rememberMe,
      isLoading,
      message,
      messageType,
      togglePassword,
      handleLogin
    }
  }
}
</script>

<style scoped>
/* 主容器 */
.login-container {
  min-height: 100vh;
  background: linear-gradient(135deg, #007BFF 0%, #00D4FF 50%, #40E0D0 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
  position: relative;
  overflow: hidden;
}

/* 背景装饰 */
.background-decoration {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
}

.circle {
  position: absolute;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.1);
  animation: float 6s ease-in-out infinite;
}

.circle-1 {
  width: 200px;
  height: 200px;
  top: 10%;
  left: 10%;
  animation-delay: 0s;
}

.circle-2 {
  width: 150px;
  height: 150px;
  top: 60%;
  right: 15%;
  animation-delay: 2s;
}

.circle-3 {
  width: 100px;
  height: 100px;
  bottom: 20%;
  left: 20%;
  animation-delay: 4s;
}

@keyframes float {
  0%, 100% { transform: translateY(0px); }
  50% { transform: translateY(-20px); }
}

/* 登录卡片 */
.login-card {
  background: white;
  border-radius: 20px;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
  padding: 40px;
  width: 100%;
  max-width: 420px;
  position: relative;
  z-index: 1;
}

/* 头部区域 */
.login-header {
  text-align: center;
  margin-bottom: 40px;
}

.logo-container {
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 10px;
}

.logo-icon {
  font-size: 32px;
  margin-right: 12px;
  background: linear-gradient(135deg, #007BFF, #00D4FF);
  background-clip: text;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

.app-title {
  font-size: 28px;
  font-weight: 700;
  color: #1E2A38;
  margin: 0;
  background: linear-gradient(135deg, #007BFF, #00D4FF);
  background-clip: text;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

.welcome-text {
  color: #5C6770;
  font-size: 16px;
  margin: 0;
}

/* 表单样式 */
.login-form {
  width: 100%;
}

.input-group {
  margin-bottom: 24px;
}

.input-label {
  display: block;
  color: #1E2A38;
  font-size: 14px;
  font-weight: 600;
  margin-bottom: 8px;
}

.input-wrapper {
  position: relative;
  display: flex;
  align-items: center;
}

.input-icon {
  position: absolute;
  left: 16px;
  font-size: 16px;
  color: #5C6770;
  z-index: 1;
}

.form-input {
  width: 100%;
  padding: 16px 16px 16px 48px;
  border: 2px solid #E1E5E9;
  border-radius: 12px;
  font-size: 16px;
  color: #1E2A38;
  background: #F5F7FA;
  transition: all 0.3s ease;
  box-sizing: border-box;
}

.form-input:focus {
  outline: none;
  border-color: #00D4FF;
  background: white;
  box-shadow: 0 0 0 3px rgba(0, 212, 255, 0.1);
}

.form-input::placeholder {
  color: #9CA3AF;
}

.password-toggle {
  position: absolute;
  right: 16px;
  background: none;
  border: none;
  cursor: pointer;
  font-size: 16px;
  color: #5C6770;
  padding: 0;
  z-index: 1;
  transition: color 0.3s ease;
}

.password-toggle:hover {
  color: #00D4FF;
}

/* 表单选项 */
.form-options {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 32px;
}

.checkbox-container {
  display: flex;
  align-items: center;
  cursor: pointer;
  user-select: none;
}

.checkbox-input {
  display: none;
}

.checkbox-custom {
  width: 18px;
  height: 18px;
  border: 2px solid #E1E5E9;
  border-radius: 4px;
  background: white;
  position: relative;
  margin-right: 8px;
  transition: all 0.3s ease;
}

.checkbox-input:checked + .checkbox-custom {
  background: linear-gradient(135deg, #007BFF, #00D4FF);
  border-color: #00D4FF;
}

.checkbox-input:checked + .checkbox-custom::after {
  content: '✓';
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  color: white;
  font-size: 12px;
  font-weight: bold;
}

.checkbox-text {
  color: #5C6770;
  font-size: 14px;
}

.forgot-password {
  color: #00D4FF;
  text-decoration: none;
  font-size: 14px;
  transition: color 0.3s ease;
}

.forgot-password:hover {
  color: #007BFF;
}

/* 登录按钮 */
.login-button {
  width: 100%;
  padding: 16px;
  background: linear-gradient(135deg, #007BFF, #00D4FF);
  color: white;
  border: none;
  border-radius: 12px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
}

.login-button:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0, 123, 255, 0.3);
}

.login-button:active:not(:disabled) {
  transform: translateY(0);
}

.login-button:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.loading-text {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
}

.loading-spinner {
  width: 16px;
  height: 16px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-radius: 50%;
  border-top-color: white;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

/* 分割线 */
.divider {
  position: relative;
  margin: 32px 0;
  text-align: center;
}

.divider::before {
  content: '';
  position: absolute;
  top: 50%;
  left: 0;
  right: 0;
  height: 1px;
  background: #E1E5E9;
}

.divider-text {
  background: white;
  color: #9CA3AF;
  padding: 0 16px;
  font-size: 14px;
  position: relative;
}

/* 注册区域 */
.register-section {
  text-align: center;
}

.register-text {
  color: #5C6770;
  font-size: 14px;
  margin: 0;
}

.register-link {
  color: #00D4FF;
  text-decoration: none;
  font-weight: 600;
  transition: color 0.3s ease;
}

.register-link:hover {
  color: #007BFF;
}

/* 消息提示 */
.message-toast {
  position: fixed;
  top: 20px;
  right: 20px;
  padding: 16px 24px;
  border-radius: 12px;
  color: white;
  font-weight: 500;
  z-index: 1000;
  animation: slideIn 0.3s ease;
  max-width: 300px;
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
}

.message-toast.success {
  background: linear-gradient(135deg, #10B981, #34D399);
}

.message-toast.error {
  background: linear-gradient(135deg, #EF4444, #F87171);
}

.message-toast.info {
  background: linear-gradient(135deg, #3B82F6, #60A5FA);
}

@keyframes slideIn {
  from {
    transform: translateX(100%);
    opacity: 0;
  }
  to {
    transform: translateX(0);
    opacity: 1;
  }
}

/* 响应式设计 */
@media (max-width: 480px) {
  .login-container {
    padding: 16px;
  }
  
  .login-card {
    padding: 24px;
    margin: 0;
  }
  
  .app-title {
    font-size: 24px;
  }
  
  .form-input {
    padding: 14px 14px 14px 44px;
    font-size: 16px;
  }
  
  .login-button {
    padding: 14px;
  }
}
</style>
