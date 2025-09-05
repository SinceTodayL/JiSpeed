<template>
  <div class="payment-page">
    <!-- 顶部导航 -->
    <div class="header">
      <button @click="goBack" class="back-btn">
        <i class="back-icon">←</i>
      </button>
      <h1 class="header-title">订单支付</h1>
    </div>

    <!-- 订单信息 -->
    <div class="order-info">
      <div class="amount-section">
        <div class="amount-label">支付金额</div>
        <div class="amount-value">¥{{ paymentAmount.toFixed(2) }}</div>
      </div>
      
      <div class="order-details">
        <div class="detail-item">
          <span class="label">订单号</span>
          <span class="value">{{ orderId }}</span>
        </div>
        <div class="detail-item">
          <span class="label">创建时间</span>
          <span class="value">{{ formatTime(createTime) }}</span>
        </div>
      </div>
    </div>

    <!-- 支付方式选择 -->
    <div class="payment-methods">
      <div class="section-title">选择支付方式</div>
      
      <div 
        v-for="method in paymentMethods"
        :key="method.code"
        :class="['payment-method', { active: selectedMethod === method.code }]"
        @click="selectPaymentMethod(method.code)"
      >
        <div class="method-info">
          <i class="method-icon">{{ method.icon }}</i>
          <div class="method-details">
            <div class="method-name">{{ method.name }}</div>
            <div class="method-desc">{{ method.description }}</div>
          </div>
        </div>
        <i v-if="selectedMethod === method.code" class="check-icon">✓</i>
      </div>
    </div>

    <!-- 支付二维码（移动端显示） -->
    <div v-if="showQRCode && qrCodeData" class="qr-section">
      <div class="section-title">扫码支付</div>
      <div class="qr-container">
        <div class="qr-code">
          <img :src="qrCodeData" alt="支付二维码" />
        </div>
        <div class="qr-tips">
          <p>请使用{{ getPaymentMethodName(selectedMethod) }}扫描二维码完成支付</p>
          <div class="countdown">
            <span>支付剩余时间：</span>
            <span class="time">{{ formatCountdown(countdown) }}</span>
          </div>
        </div>
      </div>
    </div>

    <!-- 支付状态 -->
    <div v-if="paymentStatus" class="status-section">
      <div :class="['status-indicator', paymentStatus]">
        <i class="status-icon">
          {{ paymentStatus === 'success' ? '✓' : paymentStatus === 'failed' ? '✕' : '⏳' }}
        </i>
        <div class="status-text">
          {{ getStatusText(paymentStatus) }}
        </div>
      </div>
    </div>

    <!-- 底部按钮 -->
    <div class="action-section">
      <button 
        v-if="!paymentStatus || paymentStatus === 'failed'"
        @click="initiatePayment"
        :disabled="!selectedMethod || processing"
        class="pay-btn"
      >
        {{ processing ? '处理中...' : `立即支付 ¥${paymentAmount.toFixed(2)}` }}
      </button>
      
      <button 
        v-if="paymentStatus === 'success'"
        @click="viewOrder"
        class="view-order-btn"
      >
        查看订单
      </button>
      
      <div v-if="showQRCode" class="action-tips">
        <button @click="checkPaymentStatus" class="check-btn">检查支付状态</button>
        <button @click="cancelPayment" class="cancel-btn">取消支付</button>
      </div>
    </div>


    <!-- 安全提示 -->
    <div class="security-tips">
      <div class="tips-title">
        <i class="security-icon">🔒</i>
        <span>安全提示</span>
      </div>
      <ul class="tips-list">
        <li>请在安全的网络环境下进行支付</li>
        <li>支付密码请勿告诉他人</li>
        <li>如遇到问题，请及时联系客服</li>
      </ul>
    </div>
  </div>
  <!-- 货到付款选择弹窗 -->
  <div v-if="showSuccessModal" class="modal-overlay">
    <div class="modal-content" @click.stop>
      <div class="modal-header">
        <h3>货到付款方式已选</h3>
      </div>
      <div class="modal-body">
        <p>您已选择货到付款，请在配送员送达时支付现金。</p>
        <button class="modal-btn" @click="showSuccessModal = false">确定</button>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { orderAPI } from '@/api/order.js'

export default {
  name: 'Payment',
  setup() {
    const route = useRoute()
    const router = useRouter()

    // 响应式数据
    const orderId = ref('')
    const paymentAmount = ref(0)
    const createTime = ref('')
    const selectedMethod = ref('ALIPAY')
    const processing = ref(false)
    const paymentStatus = ref('')
    const qrCodeData = ref('')
    const countdown = ref(900) // 15分钟倒计时
    const countdownTimer = ref(null)
    // 新增：货到付款成功弹窗
    const showSuccessModal = ref(false)

    // 支付方式配置
    const paymentMethods = ref([
      {
        code: 'ALIPAY',
        name: '支付宝',
        icon: '💙',
        description: '推荐有支付宝账户的用户使用'
      },
      {
        code: 'WECHAT',
        name: '微信支付',
        icon: '💚',
        description: '推荐有微信账户的用户使用'
      },
      {
        code: 'CASH',
        name: '货到付款',
        icon: '💰',
        description: '送达后现金支付给配送员'
      }
    ])

    // 计算属性
    const showQRCode = computed(() => {
      return qrCodeData.value && ['ALIPAY', 'WECHAT'].includes(selectedMethod.value)
    })

    // 方法
    const goBack = () => {
      if (paymentStatus.value === 'success') {
        router.push('/orders')
      } else {
        router.back()
      }
    }

    const initializePayment = () => {
      orderId.value = route.params.orderId
      paymentAmount.value = parseFloat(route.query.amount || 0)
      selectedMethod.value = route.query.paymentMethod || 'ALIPAY'
      createTime.value = new Date().toISOString()

      // 启动倒计时
      startCountdown()
    }

    const selectPaymentMethod = (method) => {
  if (processing.value || paymentStatus.value === 'success') return
  selectedMethod.value = method
  // 如果切换支付方式，清除之前的二维码
  qrCodeData.value = ''
  paymentStatus.value = ''
  // 不弹窗，弹窗逻辑移至支付成功后
    }

    const getPaymentMethodName = (code) => {
      const method = paymentMethods.value.find(m => m.code === code)
      return method ? method.name : ''
    }

    const initiatePayment = async () => {
  // 支付前调试：打印支付参数
  console.log('发起支付参数 orderId:', orderId.value)
  console.log('发起支付参数 channel:', selectedMethod.value)
      if (!selectedMethod.value || processing.value) return

      processing.value = true

      try {
        // 支付前调试：查询订单详情
        try {
          const orderDetail = await orderAPI.getOrderById(orderId.value)
          console.log('支付前订单详情:', orderDetail)
        } catch (err) {
          console.error('支付前查询订单详情失败:', err)
        }

        const paymentData = {
          paymentMethod: selectedMethod.value,
          returnUrl: window.location.origin + '/payment/success',
          notifyUrl: window.location.origin + '/api/payment/notify'
        }

    const response = await orderAPI.createPayment(orderId.value, selectedMethod.value.toLowerCase())

        if (response && response.code === 0) {
          if (selectedMethod.value.toLowerCase() === 'cash') {
            // 货到付款直接成功
            paymentStatus.value = 'success'
            stopCountdown()
            showSuccessModal.value = true
          } else {
            // 在线支付显示二维码
            qrCodeData.value = response.data.qrCode
            // 开始轮询支付状态
            startPaymentStatusPolling()
          }
        } else {
          paymentStatus.value = 'failed'
          alert('支付请求失败，请重试')
        }
      } catch (error) {
  console.error('发起支付失败:', error)
  paymentStatus.value = 'failed'
  alert('支付请求失败，请重试')
      } finally {
        processing.value = false
      }
    }

    const startPaymentStatusPolling = () => {
      const pollInterval = setInterval(async () => {
        try {
          const response = await orderAPI.getPaymentStatus(orderId.value)
          if (response && response.data) {
            if (response.data.paymentStatus === 1) {
              // 支付成功
              paymentStatus.value = 'success'
              clearInterval(pollInterval)
              stopCountdown()
            } else if (response.data.paymentStatus === 3) {
              // 支付失败
              paymentStatus.value = 'failed'
              clearInterval(pollInterval)
            }
          }
        } catch (error) {
          console.error('查询支付状态失败:', error)
        }
      }, 2000) // 每2秒查询一次

      // 15分钟后停止轮询
      setTimeout(() => {
        clearInterval(pollInterval)
        if (paymentStatus.value !== 'success') {
          paymentStatus.value = 'timeout'
        }
      }, 15 * 60 * 1000)
    }

    const checkPaymentStatus = async () => {
      try {
        const response = await orderAPI.getPaymentStatus(orderId.value)
        if (response && response.data) {
          if (response.data.paymentStatus === 1) {
            paymentStatus.value = 'success'
            stopCountdown()
          } else {
            alert('支付尚未完成，请继续等待或重新扫码')
          }
        }
      } catch (error) {
        console.error('查询支付状态失败:', error)
        alert('查询失败，请稍后重试')
      }
    }

    const cancelPayment = () => {
      if (confirm('确定要取消支付吗？')) {
        stopCountdown()
        router.push('/orders')
      }
    }

    const viewOrder = () => {
      router.push(`/orders/${orderId.value}`)
    }

    const startCountdown = () => {
      countdownTimer.value = setInterval(() => {
        countdown.value--
        if (countdown.value <= 0) {
          stopCountdown()
          paymentStatus.value = 'timeout'
        }
      }, 1000)
    }

    const stopCountdown = () => {
      if (countdownTimer.value) {
        clearInterval(countdownTimer.value)
        countdownTimer.value = null
      }
    }

    const formatTime = (timeString) => {
      return new Date(timeString).toLocaleString('zh-CN')
    }

    const formatCountdown = (seconds) => {
      const minutes = Math.floor(seconds / 60)
      const remainingSeconds = seconds % 60
      return `${minutes.toString().padStart(2, '0')}:${remainingSeconds.toString().padStart(2, '0')}`
    }

    const getStatusText = (status) => {
      const statusTexts = {
        'success': '支付成功',
        'failed': '支付失败',
        'timeout': '支付超时',
        'processing': '支付处理中...'
      }
      return statusTexts[status] || ''
    }

    // 生命周期
    onMounted(() => {
      initializePayment()
    })

    onUnmounted(() => {
      stopCountdown()
    })

    return {
  orderId,
  paymentAmount,
  createTime,
  selectedMethod,
  processing,
  paymentStatus,
  qrCodeData,
  countdown,
  paymentMethods,
  showQRCode,
  goBack,
  selectPaymentMethod,
  getPaymentMethodName,
  initiatePayment,
  checkPaymentStatus,
  cancelPayment,
  viewOrder,
  formatTime,
  formatCountdown,
  getStatusText,
  showSuccessModal // 暴露弹窗变量到模板
    }
  }
}
</script>

<style scoped>
/* 货到付款成功弹窗样式 */
/* 货到付款选择弹窗样式 */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background: rgba(0, 0, 0, 0.15);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 3000;
}
.modal-content {
  background: #fff;
  border-radius: 10px;
  padding: 24px 18px;
  box-shadow: 0 2px 16px rgba(0,0,0,0.10);
  min-width: 220px;
  max-width: 80vw;
  text-align: center;
}
.modal-header h3 {
  margin: 0 0 12px 0;
  font-size: 18px;
  font-weight: 600;
  color: #4facfe;
}
.modal-body p {
  font-size: 15px;
  color: #333;
  margin-bottom: 18px;
}
.modal-btn {
  padding: 8px 24px;
  background: linear-gradient(135deg, #4facfe, #00d4ff);
  color: white;
  border: none;
  border-radius: 20px;
  font-size: 15px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
}
.modal-btn:hover {
  background: #3e95fd;
}
/* 货到付款成功弹窗样式 */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.4);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 3000;
}
.modal-content {
  background: #fff;
  border-radius: 12px;
  padding: 32px 24px;
  box-shadow: 0 4px 24px rgba(0,0,0,0.15);
  min-width: 280px;
  max-width: 90vw;
  text-align: center;
}
.modal-header h3 {
  margin: 0 0 16px 0;
  font-size: 20px;
  font-weight: 700;
  color: #007BFF;
}
.modal-body p {
  font-size: 16px;
  color: #333;
  margin-bottom: 24px;
}
.modal-btn {
  padding: 10px 32px;
  background: linear-gradient(135deg, #007BFF, #00D4FF);
  color: white;
  border: none;
  border-radius: 25px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}
.modal-btn:hover {
  background: #0056b3;
}
.payment-page {
  min-height: 100vh;
  background: #f8f9fa;
  padding-bottom: 100px;
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

/* 通用区块样式 */
.order-info,
.payment-methods,
.qr-section,
.status-section,
.security-tips {
  background: white;
  margin: 16px;
  border-radius: 12px;
  padding: 20px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
}

.section-title {
  font-size: 16px;
  font-weight: 600;
  color: #333;
  margin-bottom: 16px;
}

/* 订单信息 */
.amount-section {
  text-align: center;
  margin-bottom: 20px;
  padding: 20px;
  background: linear-gradient(135deg, #f8f9ff, #e3f2fd);
  border-radius: 12px;
}

.amount-label {
  font-size: 14px;
  color: #666;
  margin-bottom: 8px;
}

.amount-value {
  font-size: 32px;
  font-weight: 700;
  color: #e74c3c;
}

.order-details {
  border-top: 1px solid #f0f0f0;
  padding-top: 16px;
}

.detail-item {
  display: flex;
  justify-content: space-between;
  margin-bottom: 8px;
}

.detail-item:last-child {
  margin-bottom: 0;
}

.detail-item .label {
  color: #666;
  font-size: 14px;
}

.detail-item .value {
  color: #333;
  font-size: 14px;
  font-weight: 500;
}

/* 支付方式 */
.payment-method {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 16px;
  border: 1px solid #e1e5e9;
  border-radius: 8px;
  margin-bottom: 12px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.payment-method:last-child {
  margin-bottom: 0;
}

.payment-method:hover {
  border-color: #007BFF;
}

.payment-method.active {
  border-color: #007BFF;
  background: #f0f8ff;
}

.method-info {
  display: flex;
  align-items: center;
  gap: 12px;
}

.method-icon {
  font-size: 24px;
}

.method-details {
  flex: 1;
}

.method-name {
  font-size: 16px;
  font-weight: 500;
  color: #333;
  margin-bottom: 2px;
}

.method-desc {
  font-size: 12px;
  color: #666;
}

.check-icon {
  color: #007BFF;
  font-size: 18px;
  font-weight: bold;
}

/* 二维码支付 */
.qr-container {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.qr-code {
  width: 200px;
  height: 200px;
  border: 1px solid #e9ecef;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 16px;
  background: white;
}

.qr-code img {
  width: 180px;
  height: 180px;
  object-fit: contain;
}

.qr-tips {
  text-align: center;
  color: #666;
}

.qr-tips p {
  margin: 0 0 12px 0;
  font-size: 14px;
}

.countdown {
  font-size: 14px;
}

.countdown .time {
  color: #e74c3c;
  font-weight: 600;
}

/* 支付状态 */
.status-indicator {
  text-align: center;
  padding: 20px;
}

.status-icon {
  display: block;
  font-size: 48px;
  margin-bottom: 12px;
}

.status-indicator.success .status-icon {
  color: #28a745;
}

.status-indicator.failed .status-icon {
  color: #dc3545;
}

.status-indicator.timeout .status-icon {
  color: #ffc107;
}

.status-text {
  font-size: 18px;
  font-weight: 600;
  color: #333;
}

/* 操作按钮 */
.action-section {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  background: white;
  padding: 16px;
  border-top: 1px solid #e9ecef;
  box-shadow: 0 -2px 8px rgba(0, 0, 0, 0.1);
}

.pay-btn,
.view-order-btn {
  width: 100%;
  padding: 14px;
  background: linear-gradient(135deg, #007BFF, #00D4FF);
  color: white;
  border: none;
  border-radius: 25px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.pay-btn:hover:not(:disabled),
.view-order-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 123, 255, 0.3);
}

.pay-btn:disabled {
  background: #ccc;
  cursor: not-allowed;
  transform: none;
  box-shadow: none;
}

.view-order-btn {
  background: linear-gradient(135deg, #28a745, #20c997);
}

.view-order-btn:hover {
  box-shadow: 0 4px 12px rgba(40, 167, 69, 0.3);
}

.action-tips {
  display: flex;
  gap: 12px;
  margin-top: 12px;
}

.check-btn,
.cancel-btn {
  flex: 1;
  padding: 10px;
  border: 1px solid #007BFF;
  border-radius: 20px;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.check-btn {
  background: #007BFF;
  color: white;
}

.check-btn:hover {
  background: #0056b3;
}

.cancel-btn {
  background: white;
  color: #007BFF;
}

.cancel-btn:hover {
  background: #f8f9fa;
}

/* 安全提示 */
.tips-title {
  display: flex;
  align-items: center;
  margin-bottom: 12px;
  color: #333;
}

.security-icon {
  margin-right: 8px;
  font-size: 16px;
}

.tips-list {
  margin: 0;
  padding-left: 20px;
  color: #666;
  font-size: 14px;
  line-height: 1.6;
}

.tips-list li {
  margin-bottom: 4px;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .order-info,
  .payment-methods,
  .qr-section,
  .status-section,
  .security-tips {
    margin: 8px;
    padding: 16px;
  }
  
  .amount-value {
    font-size: 28px;
  }
  
  .qr-code {
    width: 160px;
    height: 160px;
  }
  
  .qr-code img {
    width: 140px;
    height: 140px;
  }
}
</style>
