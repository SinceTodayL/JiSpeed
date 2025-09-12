<template>
  <div class="user-complaints-page">
    <!-- 页面头部 -->
    <div class="page-header">
      <div class="header-content">
        <button @click="goBack" class="back-btn">
          <i class="back-icon">←</i>
        </button>
        <h1 class="page-title">我的投诉</h1>
      </div>
    </div>

    <!-- 投诉内容 -->
    <div class="complaints-content">
      <!-- 加载状态 -->
      <div v-if="loading" class="loading-container">
        <div class="loading-spinner"></div>
        <p>正在加载投诉记录...</p>
      </div>

      <!-- 空状态 -->
      <div v-else-if="complaints.length === 0" class="empty-state">
        <div class="empty-icon">📝</div>
        <h3 class="empty-title">暂无投诉</h3>
        <p class="empty-desc">您还没有提交任何投诉</p>
        <button @click="goToSubmitComplaint" class="submit-btn">功能开发中</button>
      </div>

      <!-- 投诉列表 -->
      <div v-else class="complaints-list">
        <div 
          v-for="complaint in complaints" 
          :key="complaint.complaintId"
          class="complaint-card"
        >
          <!-- 投诉内容 -->
          <div class="complaint-content">
            <div class="complaint-header">
              <div class="complaint-status" :class="getStatusClass(complaint.status)">
                {{ getStatusText(complaint.status) }}
              </div>
              <span class="complaint-time">{{ formatTime(complaint.createdAt) }}</span>
            </div>
            
            <p class="complaint-text">
              {{ complaint.description }}
            </p>
            
            <div class="complaint-meta">
              <div class="order-link-container">
                <span class="order-link" @click.stop="goToOrderDetail(complaint.orderId)">查看订单 ></span>
              </div>
            </div>
          </div>
          
          <!-- 订单信息 -->
          <div v-if="complaint.orderInfo" class="order-info">
            <div class="order-header">
              <span class="order-id">订单号: {{ complaint.orderId || '未知' }}</span>
              <span class="order-status">{{ getOrderStatusText(complaint.orderInfo.orderStatus) }}</span>
            </div>
            
            <div class="order-items">
              <div class="merchant-name">{{ complaint.merchantName || '未知商家' }}</div>
              <div class="order-price">¥{{ complaint.orderInfo.orderAmount?.toFixed(2) || '0.00' }}</div>
            </div>
            
            <div class="order-time">
              下单时间: {{ formatTime(complaint.orderInfo.createAt) }}
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { complaintAPI } from '@/api/user.js'
import { orderAPI } from '@/api/order.js'
import { merchantAPI } from '@/api/merchant.js'

export default {
  name: 'UserComplaints',
  setup() {
    const router = useRouter()
    
    const loading = ref(false)
    const complaints = ref([])
    
    // 获取用户所有投诉
    const fetchUserComplaints = async () => {
      loading.value = true
      try {
        const userId = localStorage.getItem('userId')
        if (!userId) {
          alert('请先登录')
          router.push('/login')
          return
        }
        
        const response = await complaintAPI.getUserComplaints(userId)
        
        if (response && response.data && Array.isArray(response.data)) {
          // 保存投诉数据
          complaints.value = response.data

          // 为每条投诉加载对应的订单信息
          await Promise.all(
            complaints.value.map(async (complaint) => {
              try {
                if (complaint.orderId) {
                  const orderResponse = await orderAPI.getOrderDetail(complaint.orderId)
                  if (orderResponse && orderResponse.data) {
                    complaint.orderInfo = {
                      ...orderResponse.data,
                      itemCount: orderResponse.data.orderItems?.length || 0
                    }
                    
                    // 获取商家信息
                    if (orderResponse.data.merchantId) {
                      try {
                        const merchantResponse = await merchantAPI.getMerchantById(orderResponse.data.merchantId)
                        if (merchantResponse && merchantResponse.data) {
                          complaint.merchantName = merchantResponse.data.merchantName
                        }
                      } catch (err) {
                        console.error(`获取商家信息失败 (${orderResponse.data.merchantId}):`, err)
                      }
                    }
                  }
                }
              } catch (error) {
                console.error(`加载订单信息失败 (${complaint.orderId}):`, error)
              }
            })
          )
          
          console.log('加载的投诉数据:', complaints.value)
        } else {
          complaints.value = []
        }
      } catch (error) {
        console.error('获取投诉列表失败:', error)
        complaints.value = []
      } finally {
        loading.value = false
      }
    }
    
    // 跳转到订单详情
    const goToOrderDetail = (orderId) => {
      if (!orderId) {
        console.error('订单ID不存在')
        return
      }
      console.log('跳转到订单详情页:', orderId)
      router.push(`/orders/${orderId}`)
    }
    
    // 格式化时间
    const formatTime = (timeString) => {
      if (!timeString) return '未知时间'
      
      const date = new Date(timeString)
      const now = new Date()
      const diffMs = now - date
      const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24))
      
      if (diffDays === 0) {
        return '今天 ' + date.getHours().toString().padStart(2, '0') + ':' + 
               date.getMinutes().toString().padStart(2, '0')
      } else if (diffDays === 1) {
        return '昨天 ' + date.getHours().toString().padStart(2, '0') + ':' + 
               date.getMinutes().toString().padStart(2, '0')
      } else if (diffDays < 30) {
        return `${diffDays}天前`
      } else {
        return `${date.getFullYear()}-${(date.getMonth() + 1).toString().padStart(2, '0')}-${date.getDate().toString().padStart(2, '0')}`
      }
    }
    
    // 获取投诉状态文本
    const getStatusText = (status) => {
      const statusTexts = {
        '0': '待处理', 
        '1': '已处理',
        '2': '已关闭'
      }
      return statusTexts[status] || '未知状态'
    }
    
    // 获取投诉状态类名
    const getStatusClass = (status) => {
      const statusClasses = {
        '0': 'pending', 
        '1': 'processed',
        '2': 'closed'
      }
      return statusClasses[status] || ''
    }
    
    // 获取订单状态文本
    const getOrderStatusText = (status) => {
      const statusTexts = {
        0: '待付款', // Unpaid
        1: '已支付', // Paid
        2: '已确认收货', // Confirmed
        3: '已评价', // Reviewed
        4: '售后中', // Aftersales
        5: '售后结束', // AftersalesCompleted
        6: '已取消', // Cancelled
        7: '已派单', // Assigned
        8: '配送中' // InDelivery
      }
      return statusTexts[status] || '未知状态'
    }
    
    // 返回上一页
    const goBack = () => {
      router.back()
    }
    
    // 跳转到提交投诉页面
    const goToSubmitComplaint = () => {
      // 创建一个功能表示器，可以在将来实现
      alert('提交投诉功能正在开发中')
    }
    
    onMounted(() => {
      fetchUserComplaints()
    })
    
    return {
      loading,
      complaints,
      goToOrderDetail,
      formatTime,
      getStatusText,
      getStatusClass,
      getOrderStatusText,
      goBack,
      goToSubmitComplaint
    }
  }
}
</script>

<style scoped>
.user-complaints-page {
  min-height: 100vh;
  background: #f8f9fa;
  padding-bottom: 60px;
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
}

.back-btn {
  width: 40px;
  height: 40px;
  background: none;
  border: none;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  font-size: 20px;
  color: #333;
  margin-right: 10px;
  transition: background 0.3s;
}

.back-btn:hover {
  background: #f0f0f0;
}

.page-title {
  font-size: 18px;
  margin: 0 0 0 10px;
  font-weight: 600;
  color: #333;
  flex-grow: 1;
  text-align: center;
  padding-right: 40px; /* 为了视觉平衡，与返回按钮对称 */
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

/* 空状态 */
.empty-state {
  text-align: center;
  padding: 60px 20px;
  color: #666;
}

.empty-icon {
  font-size: 48px;
  margin-bottom: 16px;
}

.empty-title {
  font-size: 18px;
  margin-bottom: 8px;
  color: #333;
}

.empty-desc {
  font-size: 14px;
  margin-bottom: 24px;
  color: #999;
}

.submit-btn {
  background: #007bff;
  color: white;
  border: none;
  padding: 10px 24px;
  border-radius: 20px;
  font-size: 14px;
  cursor: pointer;
  transition: background 0.3s;
}

.submit-btn:hover {
  background: #0069d9;
}

/* 投诉列表 */
.complaints-list {
  padding: 16px;
  max-width: 800px;
  margin: 0 auto;
}

.complaint-card {
  background: white;
  border-radius: 10px;
  overflow: hidden;
  margin-bottom: 16px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
}

.complaint-content {
  padding: 20px;
}

.complaint-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.complaint-status {
  font-size: 12px;
  padding: 3px 8px;
  border-radius: 12px;
  font-weight: 500;
}

.complaint-status.pending {
  background: #fff3cd;
  color: #856404;
}

.complaint-status.processed {
  background: #d4edda;
  color: #155724;
}

.complaint-status.closed {
  background: #e2e3e5;
  color: #383d41;
}

.complaint-time {
  font-size: 12px;
  color: #999;
}

.complaint-text {
  font-size: 14px;
  line-height: 1.6;
  color: #333;
  margin-bottom: 12px;
}

.complaint-meta {
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin-top: 10px;
}

.order-link-container {
  width: 100%;
  text-align: right;
}

.order-link {
  font-size: 13px;
  color: #007bff;
  cursor: pointer;
  padding: 5px 0;
  display: inline-block;
}

.order-info {
  padding: 16px 20px;
  background: #f9f9f9;
}

.order-header {
  display: flex;
  justify-content: space-between;
  margin-bottom: 10px;
}

.order-id {
  font-size: 14px;
  font-weight: 500;
  color: #333;
}

.order-status {
  font-size: 12px;
  color: #007bff;
}

.order-items {
  display: flex;
  justify-content: space-between;
  margin-bottom: 8px;
}

.item-count {
  font-size: 13px;
  color: #666;
}

.merchant-name {
  font-size: 14px;
  color: #333;
  font-weight: 500;
}

.order-price {
  font-size: 14px;
  font-weight: 500;
  color: #333;
}

.order-time {
  font-size: 12px;
  color: #999;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* 响应式设计 */
@media (max-width: 768px) {
  .complaints-list {
    padding: 12px;
  }
  
  .complaint-card {
    border-radius: 8px;
  }
  
  .complaint-content {
    padding: 16px;
  }
  
  .order-info {
    padding: 12px 16px;
  }
}
</style>
