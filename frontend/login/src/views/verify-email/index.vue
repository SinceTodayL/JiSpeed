<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { $t } from '@/locales';
import { fetchVerifyEmail } from '@/service/api';
import WaveBg from '@/components/custom/wave-bg.vue';

/**
 * Email verification page
 * Handles email verification via link from registration email
 */

const route = useRoute();
const router = useRouter();

// Loading state for verification button
const loading = ref(false);
// Verification status
const verificationStatus = ref<'pending' | 'success' | 'error'>('pending');
const message = ref('');

// Get userId and token from URL parameters
const userId = ref('');
const token = ref('');

onMounted(() => {
  // Extract parameters from URL
  userId.value = route.query.userId as string || '';
  token.value = route.query.token as string || '';
  
  // Validate required parameters
  if (!userId.value || !token.value) {
    verificationStatus.value = 'error';
    message.value = '验证链接无效，缺少必要参数';
  }
});

/**
 * Handle verification button click
 * Calls backend verification API and processes response
 */
async function handleVerifyClick() {
  if (!userId.value || !token.value) {
    message.value = '验证参数不完整，请重新点击邮件中的验证链接';
    return;
  }
  
  try {
    // Show loading state
    loading.value = true;
    verificationStatus.value = 'pending';
    
    // Call backend verification API using unified request
    const { data: result, error } = await fetchVerifyEmail(userId.value, token.value);    
    
    // Check if there's an error
    if (!result) {
      verificationStatus.value = 'error';
      message.value = '验证失败，请重试 (backend error, fetchVerifyEmail() return None)';
      return;
    }
    
    // Process verification result
    if (result.code === 0) {
      // Verification successful
      verificationStatus.value = 'success';
      message.value = '邮箱验证成功！正在跳转到登录页面...';
      
      // Redirect to login page
      setTimeout(() => {
        router.push('/login');
      }, 50);
    } else {
      // Verification failed
      verificationStatus.value = 'error';
      message.value = result?.message || '验证失败，请重试';
    }
    
  } catch (error) {
    // Network error
    verificationStatus.value = 'error';
    message.value = '网络错误，请稍后重试';
    console.error('Verification error:', error);
  } finally {
    loading.value = false;
  }
}
</script>

<template>
  <div class="verify-email-container">
    <!-- Wave background pattern from the project -->
    <WaveBg theme-color="#1890ff" />
    
    <div class="verify-content">
      <!-- Verification icon -->
      <div class="verify-icon">
        <svg v-if="verificationStatus === 'pending'" width="80" height="80" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-2 15l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z" fill="#1890ff"/>
        </svg>
        
        <svg v-if="verificationStatus === 'success'" width="80" height="80" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-2 15l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z" fill="#52c41a"/>
        </svg>
        
        <svg v-if="verificationStatus === 'error'" width="80" height="80" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-2h2v2zm0-4h-2V7h2v6z" fill="#ff4d4f"/>
        </svg>
      </div>

      <!-- Verification text -->
      <div class="verify-text">
        <h2>邮箱验证</h2>
        <p v-if="verificationStatus === 'pending'">
          请点击下方按钮完成邮箱验证，验证成功后即可正常使用济时达外卖服务。
        </p>
        <p v-if="message" :class="{ 'success-text': verificationStatus === 'success', 'error-text': verificationStatus === 'error' }">
          {{ message }}
        </p>
      </div>

      <!-- Verification button -->
      <div class="verify-button">
        <NButton 
          v-if="verificationStatus === 'pending'" 
          type="primary" 
          size="large" 
          :loading="loading"
          @click="handleVerifyClick"
        >
          {{ loading ? '验证中...' : '验证邮箱' }}
        </NButton>
        
        <NButton 
          v-if="verificationStatus === 'error'" 
          type="primary" 
          size="large" 
          @click="handleVerifyClick"
        >
          重新验证
        </NButton>
      </div>
    </div>
  </div>
</template>

<style scoped>
.verify-email-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
  background: linear-gradient(135deg, #e16b4b 0%, #1d2be6 100%);
}

.verify-content {
  background: white;
  border-radius: 16px;
  padding: 48px 40px;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
  text-align: center;
  max-width: 480px;
  width: 90%;
  position: relative;
  z-index: 10;
}

.verify-icon {
  margin-bottom: 24px;
  display: flex;
  justify-content: center;
}

.verify-text {
  margin-bottom: 32px;
}

.verify-text h2 {
  font-size: 28px;
  font-weight: 600;
  color: #1a1a1a;
  margin-bottom: 16px;
}

.verify-text p {
  font-size: 16px;
  color: #666;
  line-height: 1.6;
  margin: 12px 0;
}

.success-text {
  color: #52c41a !important;
  font-weight: 500;
}

.error-text {
  color: #ff4d4f !important;
  font-weight: 500;
}

.verify-button {
  display: flex;
  justify-content: center;
}

.verify-button .n-button {
  min-width: 200px;
  height: 48px;
  font-size: 16px;
  font-weight: 500;
}
</style>