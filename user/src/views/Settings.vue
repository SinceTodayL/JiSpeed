<template>
  <div class="settings-page">
    <!-- 页面头部 -->
    <div class="page-header">
      <div class="header-content">
        <button @click="goBack" class="back-btn">
          <i class="back-icon">←</i>
        </button>
        <h1 class="page-title">设置</h1>
      </div>
    </div>

    <!-- 设置内容 -->
    <div class="settings-content">
      <!-- 账户设置 -->
      <div class="settings-section">
        <h3 class="section-title">账户设置</h3>
        <div class="settings-items">
          <div @click="editProfile" class="setting-item">
            <div class="setting-info">
              <div class="setting-icon">👤</div>
              <div class="setting-details">
                <span class="setting-title">个人信息</span>
                <span class="setting-subtitle">修改昵称、性别、生日等</span>
              </div>
            </div>
            <div class="setting-arrow">></div>
          </div>
          
          <div @click="changePassword" class="setting-item">
            <div class="setting-info">
              <div class="setting-icon">🔒</div>
              <div class="setting-details">
                <span class="setting-title">修改密码</span>
                <span class="setting-subtitle">定期更换密码保障账户安全</span>
              </div>
            </div>
            <div class="setting-arrow">></div>
          </div>
          
          <div @click="manageAddresses" class="setting-item">
            <div class="setting-info">
              <div class="setting-icon">📍</div>
              <div class="setting-details">
                <span class="setting-title">收货地址</span>
                <span class="setting-subtitle">管理您的收货地址</span>
              </div>
            </div>
            <div class="setting-arrow">></div>
          </div>
        </div>
      </div>

      <!-- 通知设置 -->
      <div class="settings-section">
        <h3 class="section-title">通知设置</h3>
        <div class="settings-items">
          <div class="setting-item">
            <div class="setting-info">
              <div class="setting-icon">📱</div>
              <div class="setting-details">
                <span class="setting-title">推送通知</span>
                <span class="setting-subtitle">接收订单状态、优惠活动等通知</span>
              </div>
            </div>
            <label class="setting-switch">
              <input type="checkbox" v-model="notificationSettings.push" />
              <span class="switch-slider"></span>
            </label>
          </div>
          
          <div class="setting-item">
            <div class="setting-info">
              <div class="setting-icon">📧</div>
              <div class="setting-details">
                <span class="setting-title">邮件通知</span>
                <span class="setting-subtitle">接收订单确认、促销信息等邮件</span>
              </div>
            </div>
            <label class="setting-switch">
              <input type="checkbox" v-model="notificationSettings.email" />
              <span class="switch-slider"></span>
            </label>
          </div>
          
          <div class="setting-item">
            <div class="setting-info">
              <div class="setting-icon">📞</div>
              <div class="setting-details">
                <span class="setting-title">短信通知</span>
                <span class="setting-subtitle">接收订单状态变更短信</span>
              </div>
            </div>
            <label class="setting-switch">
              <input type="checkbox" v-model="notificationSettings.sms" />
              <span class="switch-slider"></span>
            </label>
          </div>
        </div>
      </div>

      <!-- 隐私设置 -->
      <div class="settings-section">
        <h3 class="section-title">隐私设置</h3>
        <div class="settings-items">
          <div class="setting-item">
            <div class="setting-info">
              <div class="setting-icon">📊</div>
              <div class="setting-details">
                <span class="setting-title">数据分析</span>
                <span class="setting-subtitle">允许收集使用数据以改善服务</span>
              </div>
            </div>
            <label class="setting-switch">
              <input type="checkbox" v-model="privacySettings.analytics" />
              <span class="switch-slider"></span>
            </label>
          </div>
          
          <div class="setting-item">
            <div class="setting-info">
              <div class="setting-icon">🎯</div>
              <div class="setting-details">
                <span class="setting-title">个性化推荐</span>
                <span class="setting-subtitle">基于购买历史推荐相关商品</span>
              </div>
            </div>
            <label class="setting-switch">
              <input type="checkbox" v-model="privacySettings.personalization" />
              <span class="switch-slider"></span>
            </label>
          </div>
        </div>
      </div>

      <!-- 应用设置 -->
      <div class="settings-section">
        <h3 class="section-title">应用设置</h3>
        <div class="settings-items">
          <div @click="clearCache" class="setting-item">
            <div class="setting-info">
              <div class="setting-icon">🗑️</div>
              <div class="setting-details">
                <span class="setting-title">清理缓存</span>
                <span class="setting-subtitle">清理应用缓存释放存储空间</span>
              </div>
            </div>
            <div class="setting-value">{{ cacheSize }}MB</div>
          </div>
          
          <div @click="checkUpdate" class="setting-item">
            <div class="setting-info">
              <div class="setting-icon">🔄</div>
              <div class="setting-details">
                <span class="setting-title">检查更新</span>
                <span class="setting-subtitle">当前版本 v1.0.0</span>
              </div>
            </div>
            <div class="setting-arrow">></div>
          </div>
        </div>
      </div>

      <!-- 帮助与反馈 -->
      <div class="settings-section">
        <h3 class="section-title">帮助与反馈</h3>
        <div class="settings-items">
          <div @click="showHelp" class="setting-item">
            <div class="setting-info">
              <div class="setting-icon">❓</div>
              <div class="setting-details">
                <span class="setting-title">帮助中心</span>
                <span class="setting-subtitle">常见问题和使用指南</span>
              </div>
            </div>
            <div class="setting-arrow">></div>
          </div>
          
          <div @click="contactSupport" class="setting-item">
            <div class="setting-info">
              <div class="setting-icon">💬</div>
              <div class="setting-details">
                <span class="setting-title">联系客服</span>
                <span class="setting-subtitle">在线客服为您答疑解惑</span>
              </div>
            </div>
            <div class="setting-arrow">></div>
          </div>
          
          <div @click="submitFeedback" class="setting-item">
            <div class="setting-info">
              <div class="setting-icon">📝</div>
              <div class="setting-details">
                <span class="setting-title">意见反馈</span>
                <span class="setting-subtitle">您的建议是我们改进的动力</span>
              </div>
            </div>
            <div class="setting-arrow">></div>
          </div>
        </div>
      </div>

      <!-- 关于 -->
      <div class="settings-section">
        <h3 class="section-title">关于</h3>
        <div class="settings-items">
          <div @click="showAbout" class="setting-item">
            <div class="setting-info">
              <div class="setting-icon">ℹ️</div>
              <div class="setting-details">
                <span class="setting-title">关于极速外卖</span>
                <span class="setting-subtitle">了解我们的愿景和团队</span>
              </div>
            </div>
            <div class="setting-arrow">></div>
          </div>
          
          <div @click="showPrivacyPolicy" class="setting-item">
            <div class="setting-info">
              <div class="setting-icon">📄</div>
              <div class="setting-details">
                <span class="setting-title">隐私政策</span>
                <span class="setting-subtitle">我们如何保护您的隐私</span>
              </div>
            </div>
            <div class="setting-arrow">></div>
          </div>
          
          <div @click="showTerms" class="setting-item">
            <div class="setting-info">
              <div class="setting-icon">📋</div>
              <div class="setting-details">
                <span class="setting-title">服务条款</span>
                <span class="setting-subtitle">使用条款和服务协议</span>
              </div>
            </div>
            <div class="setting-arrow">></div>
          </div>
        </div>
      </div>

      <!-- 退出登录 -->
      <div class="logout-section">
        <button @click="logout" class="logout-btn">
          退出登录
        </button>
      </div>
    </div>

    <!-- 确认弹窗 -->
    <div v-if="showConfirmModal" class="confirm-modal" @click="closeModal">
      <div class="modal-content" @click.stop>
        <h3 class="modal-title">{{ confirmTitle }}</h3>
        <p class="modal-message">{{ confirmMessage }}</p>
        <div class="modal-actions">
          <button @click="closeModal" class="cancel-btn">取消</button>
          <button @click="confirmAction" class="confirm-btn">确认</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { clearUserAuth } from '@/utils/urlParams.js'

export default {
  name: 'SettingsPage',
  setup() {
    const router = useRouter()
    
    const showConfirmModal = ref(false)
    const confirmTitle = ref('')
    const confirmMessage = ref('')
    const confirmCallback = ref(null)
    const cacheSize = ref(12.5)
    
    // 通知设置
    const notificationSettings = reactive({
      push: true,
      email: false,
      sms: true
    })
    
    // 隐私设置
    const privacySettings = reactive({
      analytics: true,
      personalization: true
    })
    
    // 方法
    const editProfile = () => {
      router.push('/profile/edit')
    }
    
    const changePassword = () => {
      router.push('/profile/password')
    }
    
    const manageAddresses = () => {
      router.push('/addresses')
    }
    
    const clearCache = () => {
      confirmTitle.value = '清理缓存'
      confirmMessage.value = '确定要清理应用缓存吗？这将清除所有临时文件。'
      confirmCallback.value = () => {
        console.log('清理缓存')
        cacheSize.value = 0
        closeModal()
      }
      showConfirmModal.value = true
    }
    
    const checkUpdate = () => {
      console.log('检查更新')
      alert('当前已是最新版本')
    }
    
    const showHelp = () => {
      router.push('/help')
    }
    
    const contactSupport = () => {
      // 打开客服聊天窗口或跳转到客服页面
      alert('客服功能开发中...')
    }
    
    const submitFeedback = () => {
      router.push('/feedback')
    }
    
    const showAbout = () => {
      router.push('/about')
    }
    
    const showPrivacyPolicy = () => {
      router.push('/privacy-policy')
    }
    
    const showTerms = () => {
      router.push('/terms')
    }
    
    const logout = () => {
      confirmTitle.value = '退出登录'
      confirmMessage.value = '确定要退出当前账户吗？'
      confirmCallback.value = () => {
        // 清除所有用户认证信息
        clearUserAuth()
        
        // 跳转到统一登录页面
        window.location.href = 'http://localhost:9527/login'
        closeModal()
      }
      showConfirmModal.value = true
    }
    
    const confirmAction = () => {
      if (confirmCallback.value) {
        confirmCallback.value()
      }
    }
    
    const closeModal = () => {
      showConfirmModal.value = false
      confirmTitle.value = ''
      confirmMessage.value = ''
      confirmCallback.value = null
    }
    
    const goBack = () => {
      router.back()
    }
    
    // 保存设置（当设置发生变化时）
    const saveSettings = () => {
      const settings = {
        notifications: notificationSettings,
        privacy: privacySettings
      }
      localStorage.setItem('userSettings', JSON.stringify(settings))
      console.log('设置已保存')
    }
    
    // 加载设置
    const loadSettings = () => {
      try {
        const savedSettings = localStorage.getItem('userSettings')
        if (savedSettings) {
          const settings = JSON.parse(savedSettings)
          Object.assign(notificationSettings, settings.notifications || {})
          Object.assign(privacySettings, settings.privacy || {})
        }
      } catch (error) {
        console.error('加载设置失败:', error)
      }
    }
    
    onMounted(() => {
      loadSettings()
    })
    
    return {
      showConfirmModal,
      confirmTitle,
      confirmMessage,
      cacheSize,
      notificationSettings,
      privacySettings,
      editProfile,
      changePassword,
      manageAddresses,
      clearCache,
      checkUpdate,
      showHelp,
      contactSupport,
      submitFeedback,
      showAbout,
      showPrivacyPolicy,
      showTerms,
      logout,
      confirmAction,
      closeModal,
      goBack,
      saveSettings
    }
  }
}
</script>

<style scoped>
.settings-page {
  min-height: 100vh;
  background: #f8f9fa;
  padding-bottom: 80px;
}

/* 页面头部 */
.page-header {
  background: white;
  border-bottom: 1px solid #e9ecef;
  position: sticky;
  top: 0;
  z-index: 100;
}

.header-content {
  display: flex;
  align-items: center;
  padding: 16px 20px;
  max-width: 1200px;
  margin: 0 auto;
}

.back-btn {
  width: 40px;
  height: 40px;
  background: none;
  border: none;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  font-size: 18px;
  color: #333;
  transition: all 0.3s ease;
  margin-right: 12px;
}

.back-btn:hover {
  background: #f8f9fa;
}

.page-title {
  font-size: 20px;
  font-weight: 600;
  color: #333;
  margin: 0;
}

/* 设置内容 */
.settings-content {
  padding: 20px;
  max-width: 1200px;
  margin: 0 auto;
}

.settings-section {
  background: white;
  border-radius: 12px;
  margin-bottom: 16px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.section-title {
  font-size: 16px;
  font-weight: 600;
  color: #333;
  margin: 0;
  padding: 16px 20px;
  background: #f8f9fa;
  border-bottom: 1px solid #e9ecef;
}

.settings-items {
  display: flex;
  flex-direction: column;
}

.setting-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 16px 20px;
  border-bottom: 1px solid #f0f0f0;
  cursor: pointer;
  transition: all 0.3s ease;
}

.setting-item:last-child {
  border-bottom: none;
}

.setting-item:hover {
  background: #f8f9fa;
}

.setting-info {
  display: flex;
  align-items: center;
  flex: 1;
}

.setting-icon {
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 20px;
  margin-right: 12px;
}

.setting-details {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.setting-title {
  font-size: 16px;
  font-weight: 500;
  color: #333;
}

.setting-subtitle {
  font-size: 13px;
  color: #666;
  line-height: 1.3;
}

.setting-arrow {
  font-size: 16px;
  color: #999;
  margin-left: 12px;
}

.setting-value {
  font-size: 14px;
  color: #666;
  margin-left: 12px;
}

/* 开关样式 */
.setting-switch {
  position: relative;
  display: inline-block;
  width: 50px;
  height: 28px;
  margin-left: 12px;
}

.setting-switch input {
  opacity: 0;
  width: 0;
  height: 0;
}

.switch-slider {
  position: absolute;
  cursor: pointer;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: #ccc;
  transition: 0.4s;
  border-radius: 28px;
}

.switch-slider:before {
  position: absolute;
  content: "";
  height: 20px;
  width: 20px;
  left: 4px;
  bottom: 4px;
  background-color: white;
  transition: 0.4s;
  border-radius: 50%;
}

.setting-switch input:checked + .switch-slider {
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
}

.setting-switch input:checked + .switch-slider:before {
  transform: translateX(22px);
}

/* 退出登录 */
.logout-section {
  margin-top: 32px;
  text-align: center;
}

.logout-btn {
  width: 100%;
  max-width: 300px;
  padding: 16px;
  background: #dc3545;
  color: white;
  border: none;
  border-radius: 12px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.logout-btn:hover {
  background: #c82333;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(220, 53, 69, 0.3);
}

/* 确认弹窗 */
.confirm-modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 2000;
  padding: 20px;
}

.modal-content {
  background: white;
  border-radius: 12px;
  padding: 24px;
  max-width: 400px;
  width: 100%;
  text-align: center;
}

.modal-title {
  font-size: 18px;
  font-weight: 600;
  color: #333;
  margin: 0 0 16px 0;
}

.modal-message {
  font-size: 14px;
  color: #666;
  margin: 0 0 24px 0;
  line-height: 1.5;
}

.modal-actions {
  display: flex;
  gap: 12px;
  justify-content: center;
}

.cancel-btn,
.confirm-btn {
  padding: 10px 24px;
  border: none;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
}

.cancel-btn {
  background: #f8f9fa;
  color: #333;
  border: 1px solid #ddd;
}

.cancel-btn:hover {
  background: #e9ecef;
}

.confirm-btn {
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
  color: white;
}

.confirm-btn:hover {
  transform: translateY(-1px);
  box-shadow: 0 2px 8px rgba(79, 172, 254, 0.3);
}

/* 响应式设计 */
@media (max-width: 768px) {
  .settings-content {
    padding: 12px;
  }
  
  .setting-item {
    padding: 14px 16px;
  }
  
  .setting-icon {
    width: 36px;
    height: 36px;
    font-size: 18px;
  }
  
  .setting-title {
    font-size: 15px;
  }
  
  .setting-subtitle {
    font-size: 12px;
  }
  
  .logout-btn {
    max-width: none;
  }
}
</style>
