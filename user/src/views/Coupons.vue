<template>
  <div class="coupons">
    <!-- 顶部导航 -->
    <div class="header">
      <button @click="goBack" class="back-btn">
        <i class="back-icon">←</i>
      </button>
      <h1 class="header-title">我的优惠券</h1>
    </div>

    <!-- 统计信息 -->
    <div class="stats-section">
      <div class="stat-item">
        <div class="stat-number">{{ stats.available }}</div>
        <div class="stat-label">可用</div>
      </div>
      <div class="stat-item">
        <div class="stat-number">{{ stats.used }}</div>
        <div class="stat-label">已使用</div>
      </div>
      <div class="stat-item">
        <div class="stat-number">{{ stats.expired }}</div>
        <div class="stat-label">已过期</div>
      </div>
    </div>

    <!-- 优惠券状态筛选 -->
    <div class="filter-tabs">
      <div 
        v-for="tab in filterTabs"
        :key="tab.value"
        :class="['filter-tab', { active: activeFilter === tab.value }]"
        @click="activeFilter = tab.value"
      >
        {{ tab.label }}
      </div>
    </div>

    <!-- 加载状态 -->
    <div v-if="loading" class="loading-container">
      <div class="loading-spinner"></div>
      <p>正在加载优惠券...</p>
    </div>

    <!-- 优惠券列表 -->
    <div v-else class="coupons-content">
      <div class="coupons-list">
        <div 
          v-for="coupon in filteredCoupons"
          :key="coupon.couponId"
          :class="['coupon-card', `status-${coupon.status}`]"
        >
          <div class="coupon-main">
            <div class="coupon-left">
              <div class="coupon-amount">
                <span class="amount-symbol">¥</span>
                <span class="amount-value">{{ coupon.discountValue }}</span>
              </div>
              <div class="coupon-type">{{ getCouponTypeText(coupon.couponType) }}</div>
            </div>
            
            <div class="coupon-divider"></div>
            
            <div class="coupon-right">
              <div class="coupon-info">
                <h3 class="coupon-name">{{ coupon.couponName }}</h3>
                <p class="coupon-desc">{{ coupon.description }}</p>
                <div class="coupon-condition">
                  满{{ coupon.minOrderAmount }}元可用
                </div>
              </div>
              
              <div class="coupon-footer">
                <div class="expire-time">
                  {{ getExpireText(coupon.expireTime) }}
                </div>
                <button 
                  v-if="coupon.status === 0"
                  @click="useCoupon(coupon)"
                  class="use-btn"
                >
                  立即使用
                </button>
                <span v-else-if="coupon.status === 1" class="status-text used">
                  已使用
                </span>
                <span v-else class="status-text expired">
                  已过期
                </span>
              </div>
            </div>
          </div>
          
          <!-- 已使用或过期的遮罩 -->
          <div v-if="coupon.status !== 0" class="coupon-overlay">
            <span class="overlay-text">
              {{ coupon.status === 1 ? '已使用' : '已过期' }}
            </span>
          </div>
        </div>
      </div>

      <!-- 空状态 -->
      <div v-if="filteredCoupons.length === 0" class="empty-coupons">
        <div class="empty-icon">🎫</div>
        <p class="empty-text">暂无相关优惠券</p>
        <button @click="goToCouponCenter" class="get-coupon-btn">
          去领券中心
        </button>
      </div>
    </div>

    <!-- 领券中心悬浮按钮 -->
    <div class="float-btn" @click="showCouponCenter">
      <i class="btn-icon">🎁</i>
      <span>领券中心</span>
    </div>

    <!-- 领券中心弹窗 -->
    <div v-if="showCenterModal" class="center-modal" @click="closeCenterModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3>领券中心</h3>
          <button @click="closeCenterModal" class="modal-close">✕</button>
        </div>
        <div class="modal-body">
          <div class="templates-list">
            <div 
              v-for="template in couponTemplates"
              :key="template.templateId"
              class="template-card"
            >
              <div class="template-left">
                <div class="template-amount">
                  <span class="amount-symbol">¥</span>
                  <span class="amount-value">{{ template.discountValue }}</span>
                </div>
                <div class="template-type">{{ getCouponTypeText(template.couponType) }}</div>
              </div>
              
              <div class="template-info">
                <h4 class="template-name">{{ template.couponName }}</h4>
                <p class="template-desc">{{ template.description }}</p>
                <div class="template-condition">{{ template.conditions }}</div>
                <div class="template-validity">有效期{{ template.validDays }}天</div>
              </div>
              
              <button 
                @click="claimCoupon(template)"
                :disabled="claimingIds.includes(template.templateId)"
                class="claim-btn"
              >
                <span v-if="claimingIds.includes(template.templateId)">领取中...</span>
                <span v-else>立即领取</span>
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { couponAPI } from '@/api/coupon.js'

export default {
  name: 'Coupons',
  setup() {
    const router = useRouter()

    // 响应式数据
    const loading = ref(false)
    const coupons = ref([])
    const stats = ref({
      available: 0,
      used: 0,
      expired: 0
    })
    const activeFilter = ref('all')
    const showCenterModal = ref(false)
    const couponTemplates = ref([])
    const claimingIds = ref([])

    // 筛选选项
    const filterTabs = ref([
      { label: '全部', value: 'all' },
      { label: '可用', value: 'available' },
      { label: '已使用', value: 'used' },
      { label: '已过期', value: 'expired' }
    ])

    // 计算属性
    const filteredCoupons = computed(() => {
      if (activeFilter.value === 'all') {
        return coupons.value
      }
      
      const statusMap = {
        'available': 0,
        'used': 1,
        'expired': 2
      }
      
      return coupons.value.filter(coupon => 
        coupon.status === statusMap[activeFilter.value]
      )
    })

    // 方法
    const goBack = () => {
      router.back()
    }

    const loadCoupons = async () => {
      // ...existing code...
      // 加载后输出所有优惠券详情到控制台，方便调试
      setTimeout(() => {
        console.log('[调试] 优惠券详情:', coupons.value)
      }, 0)
      loading.value = true
      try {
        // 自动获取当前用户ID（优先 localStorage，可扩展 pinia/vuex/cookie）
        let userId = localStorage.getItem('userId')
        if (!userId || userId.length < 10) {
          alert('请先登录后再查看优惠券！')
          coupons.value = []
          stats.value = { available: 0, used: 0, expired: 0 }
          loading.value = false
          return
        }
        const response = await couponAPI.getUserCoupons(userId)
        console.log('[调试] couponId列表:', response.data)
        if (response && response.code === 0 && Array.isArray(response.data)) {
          // 批量获取优惠券详情
          const detailPromises = response.data.map(couponId => couponAPI.getCouponById(couponId))
          const detailResults = await Promise.all(detailPromises)
          console.log('[调试] 优惠券详情结果:', detailResults)
          coupons.value = detailResults
          // 简单统计（可用/已用/过期）
          stats.value = {
            available: detailResults.filter(c => c.data && new Date(c.data.endTime) > new Date()).length,
            used: 0, // 后端无已用标记，暂设为0
            expired: detailResults.filter(c => c.data && new Date(c.data.endTime) <= new Date()).length
          }
        } else {
          coupons.value = []
          stats.value = { available: 0, used: 0, expired: 0 }
        }
      } catch (error) {
        console.error('加载优惠券失败:', error)
      } finally {
        loading.value = false
      }
    }

    const loadCouponTemplates = async () => {
  // 此接口后端未实现，直接移除
    }

    const getCouponTypeText = (type) => {
      const typeMap = {
        'DISCOUNT': '满减券',
        'DELIVERY_FREE': '免配送费',
        'FULL_REDUCE': '立减券'
      }
      return typeMap[type] || '优惠券'
    }

    const getExpireText = (expireTime) => {
      const expire = new Date(expireTime)
      const now = new Date()
      const diffDays = Math.ceil((expire - now) / (1000 * 60 * 60 * 24))
      
      if (diffDays < 0) {
        return '已过期'
      } else if (diffDays === 0) {
        return '今日到期'
      } else if (diffDays <= 3) {
        return `${diffDays}天后到期`
      } else {
        return expire.toLocaleDateString()
      }
    }

    const useCoupon = (coupon) => {
      // 跳转到商家浏览页面使用优惠券
      router.push({
        path: '/browse',
        query: { couponId: coupon.couponId }
      })
    }

    const goToCouponCenter = () => {
      showCenterModal.value = true
    }

    const showCouponCenter = () => {
      if (couponTemplates.value.length === 0) {
        loadCouponTemplates()
      }
      showCenterModal.value = true
    }

    const closeCenterModal = () => {
      showCenterModal.value = false
    }

    const claimCoupon = async (template) => {
      if (claimingIds.value.includes(template.templateId)) return

      claimingIds.value.push(template.templateId)

      try {
        const response = await couponAPI.claimCoupon('USER123', template.templateId)
        if (response && response.code === 200) {
          alert('优惠券领取成功！')
          // 重新加载优惠券列表
          loadCoupons()
          // 更新模板剩余数量
          const templateIndex = couponTemplates.value.findIndex(t => t.templateId === template.templateId)
          if (templateIndex !== -1) {
            couponTemplates.value[templateIndex].remainingQuantity--
          }
        } else {
          alert('领取失败，请重试')
        }
      } catch (error) {
        console.error('领取优惠券失败:', error)
        alert('领取失败，请重试')
      } finally {
        const index = claimingIds.value.indexOf(template.templateId)
        if (index > -1) {
          claimingIds.value.splice(index, 1)
        }
      }
    }

    // 生命周期
    onMounted(() => {
      loadCoupons()
    })

    return {
      loading,
      coupons,
      stats,
      activeFilter,
      filterTabs,
      showCenterModal,
      couponTemplates,
      claimingIds,
      filteredCoupons,
      goBack,
      getCouponTypeText,
      getExpireText,
      useCoupon,
      goToCouponCenter,
      showCouponCenter,
      closeCenterModal,
      claimCoupon
    }
  }
}
</script>

<style scoped>
.coupons {
  min-height: 100vh;
  background: #f8f9fa;
  padding-bottom: 80px;
}

/* 顶部导航 */
.header {
  position: relative;
  height: 60px;
  background: linear-gradient(135deg, #007BFF, #00D4FF);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
}

.back-btn {
  position: absolute;
  left: 16px;
  width: 36px;
  height: 36px;
  background: rgba(255, 255, 255, 0.2);
  border: none;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  color: white;
  font-size: 18px;
}

.header-title {
  font-size: 18px;
  font-weight: 600;
  margin: 0;
}

/* 统计信息 */
.stats-section {
  display: flex;
  background: white;
  margin: 16px;
  border-radius: 12px;
  padding: 20px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
}

.stat-item {
  flex: 1;
  text-align: center;
}

.stat-number {
  font-size: 24px;
  font-weight: 600;
  color: #007BFF;
  margin-bottom: 4px;
}

.stat-label {
  font-size: 14px;
  color: #666;
}

/* 筛选标签 */
.filter-tabs {
  display: flex;
  background: white;
  margin: 0 16px 16px;
  border-radius: 8px;
  padding: 4px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
}

.filter-tab {
  flex: 1;
  text-align: center;
  padding: 8px 12px;
  border-radius: 6px;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.3s ease;
  color: #666;
}

.filter-tab.active {
  background: #007BFF;
  color: white;
}

/* 加载状态 */
.loading-container {
  text-align: center;
  padding: 60px 20px;
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #e1e5e9;
  border-top: 4px solid #007BFF;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 16px;
}

/* 优惠券列表 */
.coupons-content {
  padding: 0 16px;
}

.coupons-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.coupon-card {
  position: relative;
  background: white;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
  transition: all 0.3s ease;
}

.coupon-card:hover {
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
}

.coupon-card.status-1,
.coupon-card.status-2 {
  opacity: 0.6;
}

.coupon-main {
  display: flex;
  align-items: center;
  padding: 16px;
}

.coupon-left {
  width: 80px;
  text-align: center;
  margin-right: 16px;
}

.coupon-amount {
  display: flex;
  align-items: baseline;
  justify-content: center;
  color: #e74c3c;
  margin-bottom: 4px;
}

.amount-symbol {
  font-size: 14px;
  font-weight: 500;
}

.amount-value {
  font-size: 24px;
  font-weight: 600;
}

.coupon-type {
  font-size: 12px;
  color: #666;
}

.coupon-divider {
  width: 1px;
  height: 60px;
  background: #e9ecef;
  margin-right: 16px;
  position: relative;
}

.coupon-divider::before,
.coupon-divider::after {
  content: '';
  position: absolute;
  width: 12px;
  height: 12px;
  background: #f8f9fa;
  border-radius: 50%;
  left: -5.5px;
}

.coupon-divider::before {
  top: -6px;
}

.coupon-divider::after {
  bottom: -6px;
}

.coupon-right {
  flex: 1;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  min-height: 60px;
}

.coupon-info {
  margin-bottom: 8px;
}

.coupon-name {
  font-size: 16px;
  font-weight: 600;
  color: #333;
  margin: 0 0 4px 0;
}

.coupon-desc {
  font-size: 12px;
  color: #666;
  margin: 0 0 4px 0;
}

.coupon-condition {
  font-size: 12px;
  color: #999;
}

.coupon-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.expire-time {
  font-size: 12px;
  color: #999;
}

.use-btn {
  padding: 6px 16px;
  background: #007BFF;
  color: white;
  border: none;
  border-radius: 16px;
  font-size: 12px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.use-btn:hover {
  background: #0056b3;
}

.status-text {
  font-size: 12px;
  padding: 4px 12px;
  border-radius: 12px;
}

.status-text.used {
  background: #28a745;
  color: white;
}

.status-text.expired {
  background: #dc3545;
  color: white;
}

.coupon-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.1);
  display: flex;
  align-items: center;
  justify-content: center;
}

.overlay-text {
  font-size: 24px;
  font-weight: 600;
  color: rgba(0, 0, 0, 0.5);
  transform: rotate(-15deg);
}

/* 空状态 */
.empty-coupons {
  text-align: center;
  padding: 60px 20px;
}

.empty-icon {
  font-size: 48px;
  margin-bottom: 16px;
}

.empty-text {
  font-size: 16px;
  color: #666;
  margin-bottom: 20px;
}

.get-coupon-btn {
  padding: 10px 24px;
  background: linear-gradient(135deg, #007BFF, #00D4FF);
  color: white;
  border: none;
  border-radius: 20px;
  font-size: 14px;
  cursor: pointer;
}

/* 悬浮按钮 */
.float-btn {
  position: fixed;
  bottom: 20px;
  right: 20px;
  background: linear-gradient(135deg, #ff6b6b, #ff8e8e);
  color: white;
  padding: 12px 16px;
  border-radius: 25px;
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  box-shadow: 0 4px 16px rgba(255, 107, 107, 0.3);
  z-index: 1000;
}

.btn-icon {
  font-size: 18px;
}

/* 领券中心弹窗 */
.center-modal {
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
  border-radius: 16px;
  max-width: 500px;
  width: 100%;
  max-height: 80vh;
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 20px;
  border-bottom: 1px solid #e9ecef;
}

.modal-header h3 {
  font-size: 16px;
  font-weight: 600;
  color: #333;
  margin: 0;
}

.modal-close {
  width: 32px;
  height: 32px;
  background: #f8f9fa;
  border: none;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  color: #666;
}

.modal-body {
  flex: 1;
  overflow-y: auto;
  padding: 16px 20px;
}

.templates-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.template-card {
  display: flex;
  align-items: center;
  padding: 16px;
  border: 1px solid #e9ecef;
  border-radius: 8px;
  gap: 16px;
}

.template-left {
  width: 60px;
  text-align: center;
}

.template-amount {
  display: flex;
  align-items: baseline;
  justify-content: center;
  color: #e74c3c;
  margin-bottom: 4px;
}

.template-left .amount-value {
  font-size: 20px;
}

.template-type {
  font-size: 10px;
  color: #666;
}

.template-info {
  flex: 1;
}

.template-name {
  font-size: 14px;
  font-weight: 600;
  color: #333;
  margin: 0 0 4px 0;
}

.template-desc {
  font-size: 12px;
  color: #666;
  margin: 0 0 4px 0;
}

.template-condition {
  font-size: 11px;
  color: #999;
  margin-bottom: 2px;
}

.template-validity {
  font-size: 11px;
  color: #007BFF;
}

.claim-btn {
  padding: 8px 16px;
  background: #28a745;
  color: white;
  border: none;
  border-radius: 16px;
  font-size: 12px;
  cursor: pointer;
  white-space: nowrap;
}

.claim-btn:disabled {
  background: #6c757d;
  cursor: not-allowed;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .coupons-content {
    padding: 0 12px;
  }
  
  .coupon-main {
    padding: 12px;
  }
  
  .coupon-left {
    width: 60px;
    margin-right: 12px;
  }
  
  .template-card {
    padding: 12px;
    gap: 12px;
  }
  
  .template-left {
    width: 50px;
  }
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}
</style>
