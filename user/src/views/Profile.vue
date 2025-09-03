<template>
  <div class="profile-page">
    <!-- 用户信息头部 -->
    <div class="profile-header">
      <div class="user-info">
        <div class="avatar-section">
          <img 
            :src="userInfo.avatarUrl || '/default-avatar.jpg'" 
            :alt="userInfo.nickname" 
            class="user-avatar"
          />
          <div class="level-badge">Lv.{{ userInfo.level || 1 }}</div>
        </div>
        <div class="user-details">
          <h2 class="user-name">{{ userInfo.nickname || '用户' }}</h2>
          <p class="user-id">ID: {{ userInfo.userId || 'USER001' }}</p>
          <div class="user-stats">
            <div class="stat-item">
              <span class="stat-number">{{ userInfo.stats?.totalOrders || 0 }}</span>
              <span class="stat-label">订单</span>
            </div>
            <div class="stat-item">
              <span class="stat-number">{{ userInfo.stats?.favoriteCount || 0 }}</span>
              <span class="stat-label">收藏</span>
            </div>
            <div class="stat-item">
              <span class="stat-number">{{ userInfo.stats?.availableCouponCount || 0 }}</span>
              <span class="stat-label">优惠券</span>
            </div>
          </div>
        </div>
        <button @click="editProfile" class="edit-btn">
          <i class="edit-icon">✏️</i>
        </button>
      </div>
    </div>

    <!-- 功能菜单 -->
    <div class="menu-section">
      <!-- 订单相关 -->
      <div class="menu-group">
        <h3 class="group-title">订单管理</h3>
        <div class="menu-items">
          <div @click="goToOrders()" class="menu-item">
            <div class="menu-icon">📋</div>
            <div class="menu-content">
              <span class="menu-title">全部订单</span>
              <span class="menu-subtitle">查看所有订单记录</span>
            </div>
            <div class="menu-arrow">></div>
          </div>
          
          <div @click="goToOrders('pending')" class="menu-item">
            <div class="menu-icon">⏳</div>
            <div class="menu-content">
              <span class="menu-title">待付款</span>
              <span class="menu-subtitle">等待支付的订单</span>
            </div>
            <div v-if="pendingOrderCount > 0" class="menu-badge">{{ pendingOrderCount }}</div>
            <div class="menu-arrow">></div>
          </div>
          
          <div @click="goToOrders('shipped')" class="menu-item">
            <div class="menu-icon">🚚</div>
            <div class="menu-content">
              <span class="menu-title">配送中</span>
              <span class="menu-subtitle">正在配送的订单</span>
            </div>
            <div v-if="shippingOrderCount > 0" class="menu-badge">{{ shippingOrderCount }}</div>
            <div class="menu-arrow">></div>
          </div>
        </div>
      </div>

      <!-- 个人信息 -->
      <div class="menu-group">
        <h3 class="group-title">个人信息</h3>
        <div class="menu-items">
          <div @click="goToAddresses" class="menu-item">
            <div class="menu-icon">📍</div>
            <div class="menu-content">
              <span class="menu-title">地址管理</span>
              <span class="menu-subtitle">管理收货地址</span>
            </div>
            <div class="menu-arrow">></div>
          </div>
          
          <div @click="goToFavorites" class="menu-item">
            <div class="menu-icon">❤️</div>
            <div class="menu-content">
              <span class="menu-title">我的收藏</span>
              <span class="menu-subtitle">收藏的商家和菜品</span>
            </div>
            <div class="menu-arrow">></div>
          </div>
          
          <div @click="goToCoupons" class="menu-item">
            <div class="menu-icon">🎫</div>
            <div class="menu-content">
              <span class="menu-title">优惠券</span>
              <span class="menu-subtitle">我的优惠券</span>
            </div>
            <div v-if="userInfo.stats?.availableCouponCount > 0" class="menu-badge">
              {{ userInfo.stats.availableCouponCount }}
            </div>
            <div class="menu-arrow">></div>
          </div>
        </div>
      </div>

      <!-- 客服与反馈 -->
      <div class="menu-group">
        <h3 class="group-title">客服与反馈</h3>
        <div class="menu-items">
          <div @click="goToReviews" class="menu-item">
            <div class="menu-icon">⭐</div>
            <div class="menu-content">
              <span class="menu-title">我的评价</span>
              <span class="menu-subtitle">查看已发表的评价</span>
            </div>
            <div class="menu-arrow">></div>
          </div>
          
          <div @click="goToComplaints" class="menu-item">
            <div class="menu-icon">📞</div>
            <div class="menu-content">
              <span class="menu-title">投诉建议</span>
              <span class="menu-subtitle">投诉和建议记录</span>
            </div>
            <div class="menu-arrow">></div>
          </div>
          
          <div @click="contactCustomerService" class="menu-item">
            <div class="menu-icon">💬</div>
            <div class="menu-content">
              <span class="menu-title">联系客服</span>
              <span class="menu-subtitle">在线客服支持</span>
            </div>
            <div class="menu-arrow">></div>
          </div>
        </div>
      </div>

      <!-- 设置 -->
      <div class="menu-group">
        <h3 class="group-title">设置</h3>
        <div class="menu-items">
          <div @click="goToSettings" class="menu-item">
            <div class="menu-icon">⚙️</div>
            <div class="menu-content">
              <span class="menu-title">账户设置</span>
              <span class="menu-subtitle">修改密码、安全设置</span>
            </div>
            <div class="menu-arrow">></div>
          </div>
          
          <div @click="showLogoutConfirm" class="menu-item logout">
            <div class="menu-icon">🚪</div>
            <div class="menu-content">
              <span class="menu-title">退出登录</span>
              <span class="menu-subtitle">安全退出账户</span>
            </div>
            <div class="menu-arrow">></div>
          </div>
        </div>
      </div>
    </div>

    <!-- 退出登录确认弹窗 -->
    <div v-if="showLogoutDialog" class="modal-overlay" @click="hideLogoutConfirm">
      <div class="modal-content" @click.stop>
        <h3>确认退出</h3>
        <p>确定要退出登录吗？</p>
        <div class="modal-actions">
          <button @click="hideLogoutConfirm" class="cancel-btn">取消</button>
          <button @click="handleLogout" class="confirm-btn">确认退出</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, reactive, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { userAPI } from '@/api/user.js'
import { getCurrentUser, clearUserAuth } from '@/utils/urlParams.js'

export default {
  name: 'Profile',
  setup() {
    const router = useRouter()
    
    // 响应式数据
    const userInfo = ref({})
    const loading = ref(false)
    const showLogoutDialog = ref(false)
    
    // 计算属性
    const pendingOrderCount = computed(() => {
      // 这里应该从订单API获取待付款订单数量
      return 2
    })
    
    const shippingOrderCount = computed(() => {
      // 这里应该从订单API获取配送中订单数量
      return 1
    })
    
    // 获取用户信息
    const fetchUserInfo = async () => {
      loading.value = true
      try {
        // 优先从统一登录获取的用户信息开始
        const localUserInfo = getCurrentUser()
        const userId = localStorage.getItem('userId')
        
        console.log('Profile - 本地用户信息:', localUserInfo)
        console.log('Profile - 用户ID:', userId)
        
        if (!userId) {
          console.error('未找到用户ID，跳转到登录页')
          router.push('/login')
          return
        }
        
        // 尝试从API获取完整的用户信息
        try {
          const response = await userAPI.getUserById(userId)
          
          if (response.code === 200 || response.code === 0) {
            // 合并API数据、本地数据和统计数据
            userInfo.value = {
              ...response.data,
              ...localUserInfo, // 覆盖本地登录信息
              userId: userId,
              stats: {
                totalOrders: response.data?.totalOrders || 0,
                favoriteCount: response.data?.favoriteCount || 0,
                cartItemCount: response.data?.cartItemCount || 0,
                availableCouponCount: response.data?.availableCouponCount || 0,
                addressCount: response.data?.addressCount || 0
              }
            }
            console.log('Profile - API用户信息获取成功:', userInfo.value)
          } else {
            throw new Error(response.message || 'API响应失败')
          }
        } catch (apiError) {
          console.warn('API获取用户信息失败，使用本地信息和模拟数据:', apiError)
          // 使用本地信息和模拟数据
          userInfo.value = {
            ...localUserInfo,
            userId: userId,
            nickname: localUserInfo?.userName || `用户${userId.slice(-4)}`,
            avatarUrl: '/default-avatar.jpg',
            level: 1,
            stats: {
              totalOrders: 15,
              favoriteCount: 8,
              cartItemCount: 3,
              availableCouponCount: 5,
              addressCount: 2
            }
          }
          console.log('Profile - 使用降级用户信息:', userInfo.value)
        }
      } catch (error) {
        console.error('获取用户信息失败:', error)
        // 最终降级方案
        const userId = localStorage.getItem('userId') || 'unknown'
        userInfo.value = {
          userId: userId,
          nickname: `用户${userId.slice(-4)}`,
          avatarUrl: '/default-avatar.jpg',
          level: 1,
          stats: {
            totalOrders: 0,
            favoriteCount: 0,
            cartItemCount: 0,
            availableCouponCount: 0,
            addressCount: 0
          }
        }
      } finally {
        loading.value = false
      }
    }
    
    // 模拟用户数据
    const getMockUserInfo = () => {
      return {
        id: 'test_user_001',
        userId: 'test_user_001',
        nickName: '测试用户',
        nickname: '测试用户', // 兼容模板中的字段名
        account: 'testuser@example.com',
        avatarUrl: '/default-avatar.jpg',
        level: 3,
        stats: {
          totalOrders: 15,
          favoriteCount: 8,
          cartItemCount: 3,
          availableCouponCount: 5,
          addressCount: 2
        }
      }
    }
    
    // 导航方法
    const editProfile = () => {
      router.push('/profile/edit')
    }
    
    const goToOrders = (status = '') => {
      const query = status ? { status } : {}
      router.push({ path: '/orders', query })
    }
    
    const goToAddresses = () => {
      router.push('/addresses')
    }
    
    const goToFavorites = () => {
      router.push('/favorites')
    }
    
    const goToCoupons = () => {
      router.push('/coupons')
    }
    
    const goToReviews = () => {
      router.push('/reviews')
    }
    
    const goToComplaints = () => {
      router.push('/complaints')
    }
    
    const goToSettings = () => {
      router.push('/settings')
    }
    
    const contactCustomerService = () => {
      // 这里可以打开客服聊天窗口或跳转到客服页面
      alert('客服功能开发中...')
    }
    
    // 退出登录相关
    const showLogoutConfirm = () => {
      showLogoutDialog.value = true
    }
    
    const hideLogoutConfirm = () => {
      showLogoutDialog.value = false
    }
    
    const handleLogout = async () => {
      try {
        // 调用登出API
        const userId = localStorage.getItem('userId')
        if (userId) {
          await userAPI.logout(userId)
        }
      } catch (error) {
        console.error('退出登录API调用失败:', error)
        // 即使API失败也继续清除本地数据
      } finally {
        // 清除所有用户认证信息
        clearUserAuth()
        
        // 跳转到统一登录页面
        window.location.href = 'http://localhost:9527/login'
        
        hideLogoutConfirm()
      }
    }
    
    // 页面挂载时获取用户信息
    onMounted(() => {
      fetchUserInfo()
    })
    
    return {
      userInfo,
      loading,
      showLogoutDialog,
      pendingOrderCount,
      shippingOrderCount,
      editProfile,
      goToOrders,
      goToAddresses,
      goToFavorites,
      goToCoupons,
      goToReviews,
      goToComplaints,
      goToSettings,
      contactCustomerService,
      showLogoutConfirm,
      hideLogoutConfirm,
      handleLogout
    }
  }
}
</script>

<style scoped>
.profile-page {
  background: #f5f7fa;
  min-height: 100vh;
  padding-bottom: 80px; /* 为底部导航留空间 */
}

/* 用户信息头部 */
.profile-header {
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
  padding: 20px 16px 24px;
  color: white;
}

.user-info {
  display: flex;
  align-items: center;
  position: relative;
}

.avatar-section {
  position: relative;
  margin-right: 16px;
}

.user-avatar {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  border: 3px solid rgba(255,255,255,0.3);
  object-fit: cover;
}

.level-badge {
  position: absolute;
  bottom: -5px;
  right: -5px;
  background: #ff6b6b;
  color: white;
  padding: 2px 6px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 600;
}

.user-details {
  flex: 1;
}

.user-name {
  font-size: 20px;
  font-weight: 600;
  margin: 0 0 4px 0;
}

.user-id {
  font-size: 14px;
  opacity: 0.8;
  margin: 0 0 12px 0;
}

.user-stats {
  display: flex;
  gap: 20px;
}

.stat-item {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.stat-number {
  font-size: 16px;
  font-weight: 600;
}

.stat-label {
  font-size: 12px;
  opacity: 0.8;
}

.edit-btn {
  background: rgba(255,255,255,0.2);
  border: none;
  color: white;
  padding: 8px 12px;
  border-radius: 20px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.edit-btn:hover {
  background: rgba(255,255,255,0.3);
}

/* 功能菜单 */
.menu-section {
  padding: 16px;
}

.menu-group {
  background: white;
  border-radius: 12px;
  margin-bottom: 16px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0,0,0,0.05);
}

.group-title {
  padding: 16px 16px 8px;
  margin: 0;
  font-size: 16px;
  font-weight: 600;
  color: #333;
}

.menu-items {
  padding: 0 16px 16px;
}

.menu-item {
  display: flex;
  align-items: center;
  padding: 12px 0;
  border-bottom: 1px solid #f0f0f0;
  cursor: pointer;
  transition: all 0.3s ease;
  position: relative;
}

.menu-item:last-child {
  border-bottom: none;
}

.menu-item:hover {
  background: #f8f9ff;
  margin: 0 -16px;
  padding-left: 16px;
  padding-right: 16px;
}

.menu-item.logout:hover {
  background: #fff5f5;
}

.menu-icon {
  font-size: 20px;
  margin-right: 12px;
  width: 24px;
  text-align: center;
}

.menu-content {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.menu-title {
  font-size: 16px;
  color: #333;
  margin-bottom: 2px;
}

.menu-subtitle {
  font-size: 12px;
  color: #999;
}

.menu-badge {
  background: #ff4d4f;
  color: white;
  padding: 2px 6px;
  border-radius: 10px;
  font-size: 12px;
  font-weight: 500;
  margin-right: 8px;
}

.menu-arrow {
  color: #ccc;
  font-size: 14px;
}

/* 退出登录确认弹窗 */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0,0,0,0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: 12px;
  padding: 24px;
  margin: 20px;
  max-width: 300px;
  width: 100%;
  text-align: center;
}

.modal-content h3 {
  margin: 0 0 12px 0;
  font-size: 18px;
  color: #333;
}

.modal-content p {
  margin: 0 0 24px 0;
  color: #666;
  font-size: 14px;
}

.modal-actions {
  display: flex;
  gap: 12px;
}

.cancel-btn, .confirm-btn {
  flex: 1;
  padding: 12px;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-size: 14px;
  transition: all 0.3s ease;
}

.cancel-btn {
  background: #f5f5f5;
  color: #666;
}

.cancel-btn:hover {
  background: #e8e8e8;
}

.confirm-btn {
  background: #ff4d4f;
  color: white;
}

.confirm-btn:hover {
  background: #ff7875;
}
</style>
