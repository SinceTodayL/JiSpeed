<template>
  <div class="order-detail">
    <!-- 顶部导航 -->
    <div class="header">
      <button @click="goBack" class="back-btn">
        <i class="back-icon">←</i>
      </button>
      <h1 class="header-title">订单详情</h1>
    </div>

    <!-- 加载状态 -->
    <div v-if="loading" class="loading-container">
      <div class="loading-spinner"></div>
      <p>正在加载订单详情...</p>
    </div>

    <!-- 订单内容 -->
    <div v-else-if="orderDetail" class="order-content">
      <!-- 订单状态 -->
      <div class="status-section">
        <div class="status-indicator">
          <i class="status-icon">{{ getStatusIcon(orderDetail.orderStatus) }}</i>
          <div class="status-info">
            <div class="status-text">{{ orderDetail.orderStatusText }}</div>
            <div v-if="orderDetail.estimatedDeliveryTime" class="status-desc">
              预计{{ formatTime(orderDetail.estimatedDeliveryTime) }}送达
            </div>
          </div>
        </div>
        
        <!-- 配送信息 -->
        <div v-if="orderDetail.deliveryInfo && orderDetail.orderStatus === 3" class="delivery-info">
          <div class="delivery-rider">
            <span class="rider-name">{{ orderDetail.deliveryInfo.riderName }}</span>
            <a :href="`tel:${orderDetail.deliveryInfo.riderPhone}`" class="rider-phone">
              📞 {{ orderDetail.deliveryInfo.riderPhone }}
            </a>
          </div>
          <div class="delivery-location">{{ orderDetail.deliveryInfo.currentLocation }}</div>
        </div>
      </div>

      <!-- 配送地址 -->
      <div class="address-section">
        <div class="section-title">
          <i class="address-icon">📍</i>
          <span>配送地址</span>
        </div>
        <div class="address-info">
          <div class="receiver-info">
            <span class="receiver-name">{{ orderDetail.deliveryAddress.receiverName }}</span>
            <span class="receiver-phone">{{ orderDetail.deliveryAddress.receiverPhone }}</span>
          </div>
          <div class="address-detail">{{ orderDetail.deliveryAddress.detailedAddress }}</div>
        </div>
      </div>

      <!-- 商家信息 -->
      <div class="merchant-section">
        <div class="section-title">
          <i class="merchant-icon">🏪</i>
          <span>商家信息</span>
        </div>
        <div class="merchant-info">
          <div class="merchant-details">
            <div class="merchant-name">{{ orderDetail.merchantName }}</div>
            <div class="merchant-contact">
              <span>📞 {{ orderDetail.merchantPhone }}</span>
              <span>📍 {{ orderDetail.merchantAddress }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- 商品清单 -->
      <div class="items-section">
        <div class="section-title">
          <i class="items-icon">🛒</i>
          <span>商品清单</span>
        </div>
        <div class="items-list">
          <div 
            v-for="item in orderDetail.orderItems"
            :key="item.dishId"
            class="item-card"
          >
            <img 
              :src="item.coverUrl || '/api/placeholder/80/80'"
              :alt="item.dishName"
              class="item-image"
            />
            <div class="item-info">
              <div class="item-name">{{ item.dishName }}</div>
              <div class="item-price">¥{{ item.price }}</div>
            </div>
            <div class="item-quantity">×{{ item.quantity }}</div>
            <div class="item-subtotal">¥{{ item.subtotal }}</div>
          </div>
        </div>
      </div>

      <!-- 费用详情 -->
      <div class="price-section">
        <div class="section-title">
          <i class="price-icon">💰</i>
          <span>费用详情</span>
        </div>
        <div class="price-details">
          <div class="price-item">
            <span class="label">商品金额</span>
            <span class="value">¥{{ (orderDetail.totalAmount - orderDetail.deliveryFee).toFixed(1) }}</span>
          </div>
          <div class="price-item">
            <span class="label">配送费</span>
            <span class="value">¥{{ orderDetail.deliveryFee.toFixed(1) }}</span>
          </div>
          <div v-if="orderDetail.discountAmount > 0" class="price-item discount">
            <span class="label">优惠金额</span>
            <span class="value">-¥{{ orderDetail.discountAmount.toFixed(1) }}</span>
          </div>
          <div class="price-item total">
            <span class="label">实付金额</span>
            <span class="value">¥{{ orderDetail.finalAmount.toFixed(1) }}</span>
          </div>
        </div>
      </div>

      <!-- 订单信息 -->
      <div class="order-info-section">
        <div class="section-title">
          <i class="info-icon">📄</i>
          <span>订单信息</span>
        </div>
        <div class="order-info-details">
          <div class="info-item">
            <span class="label">订单号</span>
            <span class="value">{{ orderDetail.orderId }}</span>
          </div>
          <div class="info-item">
            <span class="label">下单时间</span>
            <span class="value">{{ formatTime(orderDetail.createAt) }}</span>
          </div>
          <div v-if="orderDetail.payAt" class="info-item">
            <span class="label">支付时间</span>
            <span class="value">{{ formatTime(orderDetail.payAt) }}</span>
          </div>
          <div class="info-item">
            <span class="label">支付方式</span>
            <span class="value">{{ getPaymentMethodText(orderDetail.paymentMethod) }}</span>
          </div>
          <div v-if="orderDetail.remark" class="info-item">
            <span class="label">备注</span>
            <span class="value">{{ orderDetail.remark }}</span>
          </div>
        </div>
      </div>

      <!-- 订单状态历史 -->
      <div v-if="orderDetail.statusHistory" class="history-section">
        <div class="section-title">
          <i class="history-icon">📊</i>
          <span>订单跟踪</span>
        </div>
        <div class="status-timeline">
          <div 
            v-for="(history, index) in orderDetail.statusHistory"
            :key="index"
            :class="['timeline-item', { active: index === 0 }]"
          >
            <div class="timeline-dot"></div>
            <div class="timeline-content">
              <div class="timeline-title">{{ history.statusText }}</div>
              <div class="timeline-time">{{ formatTime(history.timestamp) }}</div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 底部操作按钮 -->
    <div v-if="orderDetail" class="action-section">
      <button 
        v-if="orderDetail.orderStatus === 0"
        @click="payOrder"
        class="action-btn primary"
      >
        立即支付 ¥{{ orderDetail.finalAmount.toFixed(1) }}
      </button>
      
      <button 
        v-if="orderDetail.orderStatus === 3"
        @click="confirmOrder"
        class="action-btn primary"
      >
        确认收货
      </button>
      
      <button 
        v-if="[0, 1].includes(orderDetail.orderStatus)"
        @click="cancelOrder"
        class="action-btn secondary"
      >
        取消订单
      </button>
      
      <button 
        v-if="orderDetail.orderStatus === 4"
        @click="reviewOrder"
        class="action-btn secondary"
      >
        评价订单
      </button>

      <button 
        @click="contactService"
        class="action-btn secondary"
      >
        联系客服
      </button>
    </div>
  </div>
</template>

<script>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { orderAPI } from '@/api/order.js'

export default {
  name: 'OrderDetail',
  setup() {
    const route = useRoute()
    const router = useRouter()

    // 响应式数据
    const orderDetail = ref(null)
    const loading = ref(false)

    // 方法
    const goBack = () => {
      router.back()
    }

    const loadOrderDetail = async () => {
      loading.value = true

      try {
        const orderId = route.params.orderId
        const response = await orderAPI.getOrderDetail(orderId)
        
        if (response && response.code === 200) {
          orderDetail.value = response.data
        } else {
          alert('订单详情加载失败')
          router.back()
        }
      } catch (error) {
        console.error('加载订单详情失败:', error)
        alert('订单详情加载失败')
        router.back()
      } finally {
        loading.value = false
      }
    }

    const getStatusIcon = (status) => {
      const icons = {
        0: '💳',
        1: '⏳',
        2: '👨‍🍳',
        3: '🚴‍♂️',
        4: '✅',
        5: '❌'
      }
      return icons[status] || '📋'
    }

    const getPaymentMethodText = (method) => {
      const methods = {
        'ALIPAY': '支付宝',
        'WECHAT': '微信支付',
        'CASH': '货到付款'
      }
      return methods[method] || method
    }

    const formatTime = (timeString) => {
      return new Date(timeString).toLocaleString('zh-CN')
    }

    const payOrder = () => {
      router.push({
        name: 'Payment',
        params: { orderId: orderDetail.value.orderId },
        query: { 
          amount: orderDetail.value.finalAmount,
          paymentMethod: orderDetail.value.paymentMethod
        }
      })
    }

    const confirmOrder = async () => {
      if (!confirm('确认已收到商品？')) return

      try {
        const response = await orderAPI.confirmOrder(orderDetail.value.orderId)
        if (response && response.code === 200) {
          alert('确认收货成功')
          // 重新加载订单详情
          loadOrderDetail()
        } else {
          alert('确认收货失败，请重试')
        }
      } catch (error) {
        console.error('确认收货失败:', error)
        alert('确认收货失败，请重试')
      }
    }

    const cancelOrder = async () => {
      const reason = prompt('请输入取消原因（可选）:') || '用户主动取消'
      if (reason === null) return

      try {
        const response = await orderAPI.cancelOrder(orderDetail.value.orderId, reason)
        if (response && response.code === 200) {
          alert('订单取消成功')
          // 重新加载订单详情
          loadOrderDetail()
        } else {
          alert('取消订单失败，请重试')
        }
      } catch (error) {
        console.error('取消订单失败:', error)
        alert('取消订单失败，请重试')
      }
    }

    const reviewOrder = () => {
      router.push(`/orders/${orderDetail.value.orderId}/review`)
    }

    const contactService = () => {
      // 联系客服功能
      alert('客服电话：400-123-4567')
    }

    // 生命周期
    onMounted(() => {
      loadOrderDetail()
    })

    return {
      orderDetail,
      loading,
      goBack,
      getStatusIcon,
      getPaymentMethodText,
      formatTime,
      payOrder,
      confirmOrder,
      cancelOrder,
      reviewOrder,
      contactService
    }
  }
}
</script>

<style scoped>
.order-detail {
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

/* 通用区块样式 */
.status-section,
.address-section,
.merchant-section,
.items-section,
.price-section,
.order-info-section,
.history-section {
  background: white;
  margin: 8px 16px;
  border-radius: 12px;
  padding: 16px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
}

.section-title {
  display: flex;
  align-items: center;
  margin-bottom: 16px;
  font-size: 16px;
  font-weight: 600;
  color: #333;
}

.section-title i {
  margin-right: 8px;
  font-size: 18px;
}

/* 订单状态 */
.status-indicator {
  display: flex;
  align-items: center;
  gap: 16px;
  padding: 20px 0;
  text-align: center;
}

.status-icon {
  font-size: 48px;
}

.status-info {
  flex: 1;
  text-align: left;
}

.status-text {
  font-size: 20px;
  font-weight: 600;
  color: #333;
  margin-bottom: 4px;
}

.status-desc {
  font-size: 14px;
  color: #666;
}

.delivery-info {
  border-top: 1px solid #f0f0f0;
  padding-top: 16px;
  margin-top: 16px;
}

.delivery-rider {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.rider-name {
  font-weight: 600;
  color: #333;
}

.rider-phone {
  color: #007BFF;
  text-decoration: none;
  font-size: 14px;
}

.delivery-location {
  color: #666;
  font-size: 14px;
}

/* 地址信息 */
.address-info {
  padding: 12px;
  background: #f8f9fa;
  border-radius: 8px;
}

.receiver-info {
  display: flex;
  gap: 16px;
  margin-bottom: 8px;
}

.receiver-name {
  font-weight: 600;
  color: #333;
}

.receiver-phone {
  color: #666;
  font-size: 14px;
}

.address-detail {
  color: #666;
  font-size: 14px;
  line-height: 1.4;
}

/* 商家信息 */
.merchant-info {
  display: flex;
  align-items: center;
  gap: 12px;
}

.merchant-name {
  font-size: 16px;
  font-weight: 600;
  color: #333;
  margin-bottom: 6px;
}

.merchant-contact {
  display: flex;
  flex-direction: column;
  gap: 4px;
  font-size: 12px;
  color: #666;
}

/* 商品列表 */
.items-list {
  border: 1px solid #f0f0f0;
  border-radius: 8px;
  overflow: hidden;
}

.item-card {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px;
  border-bottom: 1px solid #f0f0f0;
}

.item-card:last-child {
  border-bottom: none;
}

.item-image {
  width: 80px;
  height: 80px;
  border-radius: 8px;
  object-fit: cover;
}

.item-info {
  flex: 1;
}

.item-name {
  font-size: 14px;
  font-weight: 500;
  color: #333;
  margin-bottom: 4px;
}

.item-price {
  font-size: 12px;
  color: #e74c3c;
}

.item-quantity {
  font-size: 14px;
  color: #666;
  margin-right: 16px;
}

.item-subtotal {
  font-size: 14px;
  font-weight: 600;
  color: #333;
}

/* 价格详情 */
.price-details {
  border: 1px solid #f0f0f0;
  border-radius: 8px;
  padding: 16px;
}

.price-item {
  display: flex;
  justify-content: space-between;
  margin-bottom: 8px;
  font-size: 14px;
}

.price-item:last-child {
  margin-bottom: 0;
}

.price-item .label {
  color: #666;
}

.price-item .value {
  color: #333;
}

.price-item.discount .value {
  color: #e74c3c;
}

.price-item.total {
  border-top: 1px solid #e9ecef;
  padding-top: 8px;
  margin-top: 8px;
  font-size: 16px;
  font-weight: 600;
}

.price-item.total .label {
  color: #333;
}

.price-item.total .value {
  color: #e74c3c;
}

/* 订单信息 */
.order-info-details {
  border: 1px solid #f0f0f0;
  border-radius: 8px;
  padding: 16px;
}

.info-item {
  display: flex;
  justify-content: space-between;
  margin-bottom: 12px;
  font-size: 14px;
}

.info-item:last-child {
  margin-bottom: 0;
}

.info-item .label {
  color: #666;
  min-width: 80px;
}

.info-item .value {
  color: #333;
  text-align: right;
  flex: 1;
}

/* 状态时间线 */
.status-timeline {
  padding-left: 20px;
}

.timeline-item {
  position: relative;
  padding-bottom: 20px;
}

.timeline-item:last-child {
  padding-bottom: 0;
}

.timeline-item::before {
  content: '';
  position: absolute;
  left: -27px;
  top: 8px;
  bottom: -12px;
  width: 2px;
  background: #e9ecef;
}

.timeline-item:last-child::before {
  display: none;
}

.timeline-dot {
  position: absolute;
  left: -32px;
  top: 4px;
  width: 12px;
  height: 12px;
  border-radius: 50%;
  background: #e9ecef;
}

.timeline-item.active .timeline-dot {
  background: #007BFF;
}

.timeline-content {
  padding-left: 8px;
}

.timeline-title {
  font-size: 14px;
  font-weight: 500;
  color: #333;
  margin-bottom: 2px;
}

.timeline-item.active .timeline-title {
  color: #007BFF;
}

.timeline-time {
  font-size: 12px;
  color: #666;
}

/* 底部操作区域 */
.action-section {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  background: white;
  padding: 16px;
  border-top: 1px solid #e9ecef;
  display: flex;
  gap: 12px;
  box-shadow: 0 -2px 8px rgba(0, 0, 0, 0.1);
}

.action-btn {
  flex: 1;
  padding: 12px;
  border: 1px solid;
  border-radius: 25px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.action-btn.primary {
  background: linear-gradient(135deg, #007BFF, #00D4FF);
  border-color: #007BFF;
  color: white;
}

.action-btn.primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 123, 255, 0.3);
}

.action-btn.secondary {
  background: white;
  border-color: #e1e5e9;
  color: #666;
}

.action-btn.secondary:hover {
  border-color: #007BFF;
  color: #007BFF;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .status-section,
  .address-section,
  .merchant-section,
  .items-section,
  .price-section,
  .order-info-section,
  .history-section {
    margin: 8px;
    padding: 12px;
  }
  
  .status-indicator {
    flex-direction: column;
    text-align: center;
    gap: 12px;
  }
  
  .status-info {
    text-align: center;
  }
  
  .action-section {
    flex-direction: column;
  }
  
  .action-btn {
    width: 100%;
  }
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}
</style>
