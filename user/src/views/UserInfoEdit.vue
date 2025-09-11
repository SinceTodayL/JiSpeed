<template>
  <div class="user-info-page">
    <!-- 页面头部 -->
    <div class="page-header">
      <div class="header-content">
        <button @click="goBack" class="back-btn">
          <i class="back-icon">←</i>
        </button>
        <h1 class="page-title">个人信息</h1>
        <button @click="saveChanges" class="save-btn" :disabled="saving">
          {{ saving ? '保存中...' : '保存' }}
        </button>
      </div>
    </div>

    <!-- 页面内容 -->
    <div class="page-content">
      <!-- 加载状态 -->
      <div v-if="loading" class="loading-container">
        <div class="loading-spinner"></div>
        <p>加载中...</p>
      </div>

      <div v-else class="info-form">
        <!-- 头像部分 -->
        <div class="avatar-section">
          <div class="avatar-container">
            <img :src="userInfo.avatarUrl ? userInfo.avatarUrl : '/default-avatar.jpg'" 
                 alt="用户头像" 
                 class="user-avatar" />
            <div class="avatar-overlay">
              <span class="edit-icon">📷</span>
              <span class="edit-text">修改头像</span>
            </div>
          </div>
        </div>

        <!-- 基本信息 -->
        <div class="form-section">
          <div class="form-group">
            <label class="form-label">用户名</label>
            <div class="form-value">{{ userInfo.userName }}</div>
          </div>

          <div class="form-group">
            <label class="form-label">昵称</label>
            <input
              v-model="formData.nickname"
              type="text"
              class="form-input"
              placeholder="请输入昵称"
            />
          </div>

          <div class="form-group">
            <label class="form-label">性别</label>
            <div class="radio-group">
              <label class="radio-label">
                <input
                  type="radio"
                  name="gender"
                  :value="1"
                  v-model="formData.gender"
                />
                <span>男</span>
              </label>
              <label class="radio-label">
                <input
                  type="radio"
                  name="gender"
                  :value="2"
                  v-model="formData.gender"
                />
                <span>女</span>
              </label>
              <label class="radio-label">
                <input
                  type="radio"
                  name="gender"
                  :value="3"
                  v-model="formData.gender"
                />
                <span>保密</span>
              </label>
            </div>
          </div>

          <div class="form-group">
            <label class="form-label">生日</label>
            <input
              v-model="formData.birthday"
              type="date"
              class="form-input"
              placeholder="请选择生日"
            />
          </div>

          <div class="form-group">
            <label class="form-label">手机号</label>
            <div class="form-value">{{ userInfo.phoneNumber || '未绑定' }}</div>
          </div>

          <div class="form-group">
            <label class="form-label">邮箱</label>
            <div class="form-value">{{ userInfo.email || '未绑定' }}</div>
          </div>
        </div>

        <!-- 账户信息 -->
        <div class="form-section">
          <h3 class="section-title">账户信息</h3>
          
          <div class="form-group">
            <label class="form-label">用户ID</label>
            <div class="form-value user-id">{{ userInfo.userId }}</div>
          </div>

          <div class="form-group">
            <label class="form-label">会员等级</label>
            <div class="form-value">Lv.{{ userInfo.level || 0 }}</div>
          </div>

          <div class="form-group">
            <label class="form-label">订单数量</label>
            <div class="form-value">{{ userInfo.stats?.totalOrders || 0 }}</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { userAPI } from '@/api/user.js'

export default {
  name: 'UserInfoEdit',
  setup() {
    const router = useRouter()
    const userInfo = ref({})
    const loading = ref(true)
    const saving = ref(false)
    const formData = reactive({
      nickname: '',
      gender: 3,
      birthday: ''
    })

    // 获取用户信息
    const fetchUserInfo = async () => {
      loading.value = true
      try {
        const userId = localStorage.getItem('userId')
        if (!userId) {
          alert('请先登录')
          router.push('/login')
          return
        }

        const response = await userAPI.getUserById(userId)
        if (response && (response.code === 0 || response.code === 200)) {
          userInfo.value = response.data
          
          // 初始化表单数据
          formData.nickname = response.data.nickname || ''
          formData.gender = response.data.gender || 3
          
          // 格式化生日日期 (如果有)
          if (response.data.birthday) {
            // 处理不同的日期格式，确保它是YYYY-MM-DD格式
            const date = new Date(response.data.birthday)
            if (!isNaN(date.getTime())) {
              formData.birthday = date.toISOString().split('T')[0]
            }
          } else {
            formData.birthday = ''
          }
          
          console.log('获取到的用户信息:', response.data)
        } else {
          throw new Error(response?.message || '获取用户信息失败')
        }
      } catch (error) {
        console.error('获取用户信息出错:', error)
        alert('获取用户信息失败，请重试')
      } finally {
        loading.value = false
      }
    }

    // 保存修改
    const saveChanges = async () => {
      if (saving.value) return
      
      saving.value = true
      try {
        const userId = localStorage.getItem('userId')
        if (!userId) {
          alert('请先登录')
          router.push('/login')
          return
        }

        // 验证表单数据
        if (!formData.nickname.trim()) {
          alert('昵称不能为空')
          saving.value = false
          return
        }

        // 构建更新数据
        const updateData = {
          nickname: formData.nickname,
          gender: formData.gender,
          birthday: formData.birthday || null
        }

        console.log('更新用户信息:', updateData)
        const response = await userAPI.updateUser(userId, updateData)
        
        if (response && (response.code === 0 || response.code === 200)) {
          alert('个人信息更新成功')
          // 更新本地显示
          userInfo.value = response.data
          
          // 如果需要，更新本地存储的信息
          const localUserInfo = JSON.parse(localStorage.getItem('userInfo') || '{}')
          localUserInfo.nickname = response.data.nickname
          localStorage.setItem('userInfo', JSON.stringify(localUserInfo))
          
          // 返回上一页
          router.back()
        } else {
          throw new Error(response?.message || '更新用户信息失败')
        }
      } catch (error) {
        console.error('更新用户信息出错:', error)
        alert('更新用户信息失败，请重试')
      } finally {
        saving.value = false
      }
    }

    // 返回上一页
    const goBack = () => {
      router.back()
    }

    onMounted(() => {
      fetchUserInfo()
    })

    return {
      userInfo,
      formData,
      loading,
      saving,
      saveChanges,
      goBack
    }
  }
}
</script>

<style scoped>
.user-info-page {
  min-height: 100vh;
  background: #f5f7fa;
  padding-bottom: 20px;
}

/* 页面头部 */
.page-header {
  background: white;
  border-bottom: 1px solid #e9ecef;
  position: sticky;
  top: 0;
  z-index: 100;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
}

.header-content {
  display: flex;
  align-items: center;
  padding: 16px 20px;
  max-width: 1200px;
  margin: 0 auto;
  position: relative;
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
  font-size: 20px;
  color: #333;
}

.page-title {
  flex-grow: 1;
  text-align: center;
  font-size: 18px;
  margin: 0;
  font-weight: 600;
  color: #333;
}

.save-btn {
  padding: 8px 16px;
  background: #007bff;
  color: white;
  border: none;
  border-radius: 20px;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.save-btn:hover {
  background: #0056b3;
}

.save-btn:disabled {
  background: #cccccc;
  cursor: not-allowed;
}

/* 加载状态 */
.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 60px 0;
  color: #666;
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 3px solid #f3f3f3;
  border-top: 3px solid #007bff;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 16px;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* 页面内容 */
.page-content {
  max-width: 800px;
  margin: 0 auto;
  padding: 20px;
}

.info-form {
  background: white;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
}

/* 头像部分 */
.avatar-section {
  padding: 32px 0;
  display: flex;
  justify-content: center;
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
}

.avatar-container {
  position: relative;
  width: 100px;
  height: 100px;
  border-radius: 50%;
  overflow: hidden;
  cursor: pointer;
}

.user-avatar {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.avatar-overlay {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  background: rgba(0, 0, 0, 0.6);
  padding: 8px 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: opacity 0.3s;
}

.avatar-container:hover .avatar-overlay {
  opacity: 1;
}

.edit-icon {
  font-size: 16px;
  color: white;
}

.edit-text {
  font-size: 12px;
  color: white;
}

/* 表单部分 */
.form-section {
  padding: 20px;
  border-bottom: 1px solid #f0f0f0;
}

.form-section:last-child {
  border-bottom: none;
}

.section-title {
  margin: 0 0 16px 0;
  font-size: 16px;
  font-weight: 600;
  color: #333;
}

.form-group {
  margin-bottom: 16px;
  display: flex;
  align-items: center;
}

.form-group:last-child {
  margin-bottom: 0;
}

.form-label {
  width: 80px;
  font-size: 14px;
  color: #666;
}

.form-value {
  flex: 1;
  font-size: 14px;
  color: #333;
}

.user-id {
  color: #999;
  font-size: 13px;
}

.form-input {
  flex: 1;
  padding: 10px 12px;
  border: 1px solid #e1e5e9;
  border-radius: 6px;
  font-size: 14px;
  color: #333;
  transition: border-color 0.3s;
  width: 100%;
  box-sizing: border-box;
}

.form-input:focus {
  border-color: #007bff;
  outline: none;
}

.radio-group {
  display: flex;
  gap: 16px;
}

.radio-label {
  display: flex;
  align-items: center;
  cursor: pointer;
}

.radio-label input {
  margin-right: 4px;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .form-group {
    flex-direction: column;
    align-items: flex-start;
  }

  .form-label {
    width: 100%;
    margin-bottom: 8px;
  }
}
</style>
