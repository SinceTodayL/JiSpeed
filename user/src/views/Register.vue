<template>
  <div class="register-container">
    <!-- 背景装饰 -->
    <div class="background-decoration">
      <div class="circle circle-1"></div>
      <div class="circle circle-2"></div>
      <div class="circle circle-3"></div>
    </div>

    <!-- 注册卡片 -->
    <div class="register-card">
      <!-- 头部Logo区域 -->
      <div class="register-header">
        <div class="logo-container">
          <div class="logo-icon">⚡</div>
          <h1 class="app-title">急速</h1>
        </div>
        <p class="welcome-text">创建您的账号</p>
      </div>

      <!-- 注册表单 -->
      <div class="register-form">
        <form @submit.prevent="handleRegister">
          <!-- 用户名输入框 -->
          <div class="input-group">
            <label class="input-label">用户名</label>
            <div class="input-wrapper">
              <i class="input-icon">👤</i>
              <input
                v-model="registerForm.username"
                type="text"
                class="form-input"
                placeholder="请输入用户名"
                required
              />
            </div>
          </div>

          <!-- 邮箱输入框 -->
          <div class="input-group">
            <label class="input-label">邮箱</label>
            <div class="input-wrapper">
              <i class="input-icon">📧</i>
              <input
                v-model="registerForm.email"
                type="email"
                class="form-input"
                placeholder="请输入邮箱地址"
                required
              />
            </div>
          </div>

          <!-- 手机号输入框 -->
          <div class="input-group">
            <label class="input-label">手机号</label>
            <div class="input-wrapper">
              <i class="input-icon">📱</i>
              <input
                v-model="registerForm.phone"
                type="tel"
                class="form-input"
                placeholder="请输入手机号"
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
                v-model="registerForm.password"
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

          <!-- 确认密码输入框 -->
          <div class="input-group">
            <label class="input-label">确认密码</label>
            <div class="input-wrapper">
              <i class="input-icon">🔒</i>
              <input
                v-model="confirmPassword"
                :type="showConfirmPassword ? 'text' : 'password'"
                class="form-input"
                placeholder="请再次输入密码"
                required
              />
              <button
                type="button"
                class="password-toggle"
                @click="toggleConfirmPassword"
              >
                {{ showConfirmPassword ? '👁️' : '👁️‍🗨️' }}
              </button>
            </div>
          </div>

          <!-- 服务条款 -->
          <div class="form-options">
            <label class="checkbox-container">
              <input
                v-model="agreeTerms"
                type="checkbox"
                class="checkbox-input"
                required
              />
              <span class="checkbox-custom"></span>
              <span class="checkbox-text">
                我已阅读并同意
                <a href="#" class="terms-link">服务条款</a>
                和
                <a href="#" class="terms-link">隐私政策</a>
              </span>
            </label>
          </div>

          <!-- 注册按钮 -->
          <button
            type="submit"
            class="register-button"
            :class="{ loading: isLoading }"
            :disabled="isLoading || !agreeTerms"
          >
            <span v-if="!isLoading">注册</span>
            <span v-else class="loading-text">
              <span class="loading-spinner"></span>
              注册中...
            </span>
          </button>
        </form>

        <!-- 分割线 -->
        <div class="divider">
          <span class="divider-text">或</span>
        </div>

        <!-- 登录链接 -->
        <div class="login-section">
          <p class="login-text">
            已有账号？
            <router-link to="/login" class="login-link">立即登录</router-link>
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
  name: 'Register',
  setup() {
    const router = useRouter()
    
    // 响应式数据
    const registerForm = reactive({
      username: '',
      email: '',
      phone: '',
      password: ''
    })
    
    const confirmPassword = ref('')
    const showPassword = ref(false)
    const showConfirmPassword = ref(false)
    const agreeTerms = ref(false)
    const isLoading = ref(false)
    const message = ref('')
    const messageType = ref('')

    // 方法
    const togglePassword = () => {
      showPassword.value = !showPassword.value
    }

    const toggleConfirmPassword = () => {
      showConfirmPassword.value = !showConfirmPassword.value
    }

    const showMessage = (msg, type = 'info') => {
      message.value = msg
      messageType.value = type
      setTimeout(() => {
        message.value = ''
        messageType.value = ''
      }, 3000)
    }

    const validateForm = () => {
      if (!registerForm.username || registerForm.username.length < 3) {
        showMessage('用户名至少需要3个字符', 'error')
        return false
      }

      if (!registerForm.email || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(registerForm.email)) {
        showMessage('请输入有效的邮箱地址', 'error')
        return false
      }

      if (!registerForm.phone || !/^1[3-9]\d{9}$/.test(registerForm.phone)) {
        showMessage('请输入有效的手机号', 'error')
        return false
      }

      if (!registerForm.password || registerForm.password.length < 6) {
        showMessage('密码至少需要6个字符', 'error')
        return false
      }

      if (registerForm.password !== confirmPassword.value) {
        showMessage('两次输入的密码不一致', 'error')
        return false
      }

      if (!agreeTerms.value) {
        showMessage('请阅读并同意服务条款', 'error')
        return false
      }

      return true
    }

    const handleRegister = async () => {
      if (!validateForm()) {
        return
      }

      isLoading.value = true
      
      try {
        const response = await authAPI.register(registerForm)
        
        console.log('注册响应:', response)
        
        if (response.success || response.code === 200) {
          showMessage('注册成功！请前往登录', 'success')
          
          // 延迟跳转到登录页面
          setTimeout(() => {
            router.push('/login')
          }, 2000)
        } else {
          showMessage(response.message || '注册失败，请重试', 'error')
        }
      } catch (error) {
        console.error('注册错误:', error)
        if (error.response && error.response.data && error.response.data.message) {
          showMessage(error.response.data.message, 'error')
        } else {
          showMessage('注册失败，请检查网络连接或联系管理员', 'error')
        }
      } finally {
        isLoading.value = false
      }
    }

    return {
      registerForm,
      confirmPassword,
      showPassword,
      showConfirmPassword,
      agreeTerms,
      isLoading,
      message,
      messageType,
      togglePassword,
      toggleConfirmPassword,
      handleRegister
    }
  }
}
</script>

<style scoped>
/* 主容器 */
.register-container {
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
}

.circle-1 {
  width: 200px;
  height: 200px;
  top: 10%;
  left: 10%;
}

.circle-2 {
  width: 150px;
  height: 150px;
  top: 60%;
  right: 15%;
}

.circle-3 {
  width: 100px;
  height: 100px;
  bottom: 20%;
  left: 20%;
}

/* 注册卡片 */
.register-card {
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
.register-header {
  text-align: center;
  margin-bottom: 32px;
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
.register-form {
  width: 100%;
}

.input-group {
  margin-bottom: 20px;
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
  margin-bottom: 24px;
}

.checkbox-container {
  display: flex;
  align-items: flex-start;
  cursor: pointer;
  user-select: none;
}

.checkbox-input {
  display: none;
}

.checkbox-custom {
  width: 18px;
  height: 18px;
  min-width: 18px;
  border: 2px solid #E1E5E9;
  border-radius: 4px;
  background: white;
  position: relative;
  margin-right: 8px;
  margin-top: 2px;
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
  line-height: 1.4;
}

.terms-link {
  color: #00D4FF;
  text-decoration: none;
  transition: color 0.3s ease;
}

.terms-link:hover {
  color: #007BFF;
}

/* 注册按钮 */
.register-button {
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

.register-button:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0, 123, 255, 0.3);
}

.register-button:active:not(:disabled) {
  transform: translateY(0);
}

.register-button:disabled {
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
  margin: 24px 0;
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

/* 登录区域 */
.login-section {
  text-align: center;
}

.login-text {
  color: #5C6770;
  font-size: 14px;
  margin: 0;
}

.login-link {
  color: #00D4FF;
  text-decoration: none;
  font-weight: 600;
  transition: color 0.3s ease;
}

.login-link:hover {
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
  .register-container {
    padding: 16px;
  }
  
  .register-card {
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
  
  .register-button {
    padding: 14px;
  }

  .input-group {
    margin-bottom: 18px;
  }
}
</style>
