<template>
  <div class="order-confirm">
    <!-- 顶部导航 -->
    <div class="header">
      <button @click="goBack" class="back-btn">
        <i class="back-icon">←</i>
      </button>
      <h1 class="header-title">确认订单</h1>
    </div>

    <!-- 加载状态 -->
    <div v-if="!orderItems.length && !merchantInfo.merchantName" class="loading-container">
      <div class="loading-spinner"></div>
      <p>正在加载订单信息...</p>
    </div>

    <!-- 主要内容 -->
    <div v-else>
      <!-- 配送地址 -->
    <div class="address-section">
      <div class="section-title">
        <i class="address-icon">📍</i>
        <span>配送地址</span>
      </div>
      
      <div v-if="selectedAddress" class="address-card" @click="showAddressSelector">
        <div class="address-info">
          <div class="receiver-info">
            <span class="receiver-name">{{ selectedAddress.receiverName }}</span>
            <span class="receiver-phone">{{ selectedAddress.receiverPhone }}</span>
          </div>
          <div class="address-detail">{{ selectedAddress.detailedAddress }}</div>
        </div>
        <i class="arrow-icon">→</i>
      </div>
      
      <div v-else class="empty-address" @click="showAddressSelector">
        <span>请选择配送地址</span>
        <i class="arrow-icon">→</i>
      </div>
    </div>

    <!-- 商家信息 -->
    <div class="merchant-section">
      <div class="merchant-info">
        <img 
          :src="merchantLogo" 
          :alt="merchantInfo.merchantName"
          class="merchant-logo"
        />
        <div class="merchant-details">
          <h3 class="merchant-name">{{ merchantInfo.merchantName }}</h3>
          <div class="merchant-meta">
            <span>🚚 {{ merchantInfo.deliveryTime || 30 }}分钟</span>
            <span>💰 配送费¥{{ merchantInfo.deliveryFee || 3.5 }}</span>
          </div>
        </div>
      </div>
    </div>

    <!-- 商品列表 -->
    <div class="items-section">
      <div class="section-title">
        <i class="items-icon">🛒</i>
        <span>商品清单</span>
      </div>
      
      <div class="items-list">
        <div 
          v-for="item in orderItems" 
          :key="item.dishId"
          class="item-card"
        >
          <img 
            :src="item.coverUrl || '/assets/placeholder.png'"
            :alt="item.dishName"
            class="item-image"
          />
          <div class="item-info">
            <h4 class="item-name">{{ item.dishName }}</h4>
            <div class="item-price">¥{{ item.price }}</div>
          </div>
          <div class="item-quantity">×{{ item.quantity }}</div>
          <div class="item-subtotal">¥{{ (item.price * item.quantity).toFixed(1) }}</div>
        </div>
      </div>
    </div>

    <!-- 优惠券 -->
    <div class="coupon-section">
      <div class="coupon-selector" @click="showCouponSelector">
        <div class="coupon-info">
          <i class="coupon-icon">🎫</i>
          <span v-if="selectedCoupon">{{ selectedCoupon.description }}</span>
          <span v-else>选择优惠券</span>
        </div>
        <div class="coupon-value">
          <span v-if="selectedCoupon" class="discount">-¥{{ selectedCoupon.faceValue }}</span>
          <i class="arrow-icon">→</i>
        </div>
      </div>
    </div>

    <!-- 配送信息 -->
    <div class="delivery-section">
      <div class="delivery-item">
        <span class="label">配送费</span>
        <span class="value">¥{{ deliveryFee.toFixed(1) }}</span>
      </div>
      <div class="delivery-item">
        <span class="label">预计送达</span>
        <span class="value">{{ estimatedDeliveryTime }}</span>
      </div>
    </div>

    <!-- 备注 -->
    <div class="remark-section">
      <div class="section-title">
        <i class="remark-icon">📝</i>
        <span>订单备注</span>
      </div>
      <textarea 
        v-model="remark"
        class="remark-input"
        placeholder="如有特殊要求请在此备注（选填）"
        maxlength="100"
      ></textarea>
    </div>

    <!-- 支付方式 -->
    <div class="payment-section">
      <div class="section-title">
        <i class="payment-icon">💳</i>
        <span>支付方式</span>
      </div>
      
      <div class="payment-methods">
        <div 
          v-for="method in paymentMethods"
          :key="method.code"
          :class="['payment-method', { active: selectedPaymentMethod === method.code }]"
          @click="selectedPaymentMethod = method.code"
        >
          <i class="method-icon">{{ method.icon }}</i>
          <span class="method-name">{{ method.name }}</span>
          <i v-if="selectedPaymentMethod === method.code" class="check-icon">✓</i>
        </div>
      </div>
    </div>

    <!-- 价格详情 -->
    <div class="price-section">
      <div class="price-item">
        <span class="label">商品金额</span>
        <span class="value">¥{{ subtotal.toFixed(1) }}</span>
      </div>
      <div class="price-item">
        <span class="label">配送费</span>
        <span class="value">¥{{ deliveryFee.toFixed(1) }}</span>
      </div>
      <div v-if="discountAmount > 0" class="price-item discount">
        <span class="label">优惠金额</span>
        <span class="value">-¥{{ discountAmount.toFixed(1) }}</span>
      </div>
      <div class="price-item total">
        <span class="label">实付金额</span>
        <span class="value">¥{{ finalAmount.toFixed(1) }}</span>
      </div>
    </div>

    <!-- 底部提交按钮 -->
    <div class="submit-section">
      <div class="total-info">
        <div class="total-label">实付金额</div>
        <div class="total-amount">¥{{ finalAmount.toFixed(1) }}</div>
      </div>
      <button 
        @click="submitOrder"
        :disabled="!canSubmit || submitting"
        class="submit-btn"
      >
        {{ submitting ? '提交中...' : '提交订单' }}
      </button>
    </div>
    </div>

    <!-- 地址选择弹窗 -->
    <div v-if="showAddressModal" class="modal-overlay" @click="closeAddressModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3>选择配送地址</h3>
          <button @click="closeAddressModal" class="modal-close">✕</button>
        </div>
        <div class="modal-body">
          <div 
            v-for="address in addresses"
            :key="address.addressId"
            :class="['address-option', { selected: selectedAddress?.addressId === address.addressId }]"
            @click="selectAddress(address)"
          >
            <div class="address-content">
              <div class="receiver-info">
                <span class="receiver-name">{{ address.receiverName }}</span>
                <span class="receiver-phone">{{ address.receiverPhone }}</span>
                <span v-if="address.isDefault" class="default-tag">默认</span>
              </div>
              <div class="address-detail">{{ address.detailedAddress }}</div>
            </div>
            <i v-if="selectedAddress?.addressId === address.addressId" class="check-icon">✓</i>
          </div>
          
          <div class="add-address" @click="addNewAddress">
            <i class="add-icon">+</i>
            <span>添加新地址</span>
          </div>
          
          <div class="manage-address" @click="manageAddresses">
            <i class="manage-icon">⚙️</i>
            <span>管理地址</span>
          </div>
        </div>
      </div>
    </div>

    <!-- 优惠券选择弹窗 -->
    <div v-if="showCouponModal" class="modal-overlay" @click="closeCouponModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3>选择优惠券</h3>
          <button @click="closeCouponModal" class="modal-close">✕</button>
        </div>
        <div class="modal-body">
          <div 
            v-for="coupon in availableCoupons"
            :key="coupon.couponId"
            :class="['coupon-option', { 
              selected: selectedCoupon?.couponId === coupon.couponId,
              disabled: subtotal < coupon.threshold 
            }]"
            @click="selectCoupon(coupon)"
          >
            <div class="coupon-content">
              <div class="coupon-value">¥{{ coupon.faceValue }}</div>
              <div class="coupon-desc">
                <div class="coupon-title">{{ coupon.description }}</div>
                <div class="coupon-condition">满¥{{ coupon.threshold }}可用</div>
              </div>
            </div>
            <i v-if="selectedCoupon?.couponId === coupon.couponId" class="check-icon">✓</i>
          </div>
          
          <div class="no-coupon-option" @click="selectCoupon(null)">
            <span>不使用优惠券</span>
            <i v-if="!selectedCoupon" class="check-icon">✓</i>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { orderAPI, orderLogAPI } from '@/api/order.js'
import { merchantAPI } from '@/api/browse.js'
import { couponAPI } from '@/api/coupon.js'
import { addressAPI } from '@/api/user.js'
import { getMerchantOrRandomImage } from '@/utils/imageUtils.js'

export default {
  name: 'OrderConfirm',
  setup() {
    const route = useRoute()
    const router = useRouter()

    // 响应式数据
    const merchantInfo = ref({})
    const orderItems = ref([])
    const addresses = ref([])
    const selectedAddress = ref(null)
    const availableCoupons = ref([])
    const selectedCoupon = ref(null)
    const remark = ref('')
    const selectedPaymentMethod = ref('ALIPAY')
    const submitting = ref(false)
    const showAddressModal = ref(false)
    const showCouponModal = ref(false)

    // 支付方式
    const paymentMethods = ref([
      { code: 'ALIPAY', name: '支付宝', icon: '💙' },
      { code: 'WECHAT', name: '微信支付', icon: '💚' },
      { code: 'CASH', name: '货到付款', icon: '💰' }
    ])

    // 计算属性
    const subtotal = computed(() => {
      return orderItems.value.reduce((sum, item) => sum + (item.price * item.quantity), 0)
    })

    const deliveryFee = computed(() => {
      return merchantInfo.value.deliveryFee || 3.5
    })

    const discountAmount = computed(() => {
      if (!selectedCoupon.value) return 0
      if (subtotal.value < selectedCoupon.value.threshold) return 0
      return selectedCoupon.value.faceValue
    })

    const finalAmount = computed(() => {
      return Math.max(0, subtotal.value + deliveryFee.value - discountAmount.value)
    })

    const estimatedDeliveryTime = computed(() => {
      const deliveryMinutes = merchantInfo.value.deliveryTime || 30
      const estimatedTime = new Date(Date.now() + deliveryMinutes * 60 * 1000)
      return estimatedTime.toLocaleTimeString('zh-CN', { 
        hour: '2-digit', 
        minute: '2-digit' 
      })
    })

    const canSubmit = computed(() => {
      return selectedAddress.value && orderItems.value.length > 0 && !submitting.value
    })

    // 商家Logo计算属性
    const merchantLogo = computed(() => {
      return getMerchantOrRandomImage(merchantInfo.value?.merchantName)
    })

    // 方法
    const goBack = () => {
      router.back()
    }

    const initializeOrder = async () => {
      console.log('初始化前 merchantInfo:', merchantInfo.value)
      console.log('初始化前 merchantId:', merchantInfo.value.merchantId)
      try {
        // 检查用户是否登录
        const userId = localStorage.getItem('userId')
        if (!userId) {
          console.error('未找到用户ID，无法初始化订单')
          alert('请先登录')
          router.push('/login')
          return
        }
        
        let orderData = null
        
        // 优先从路由query参数获取数据
        if (route.query.data) {
          try {
            orderData = JSON.parse(route.query.data)
            console.log('从路由获取到订单数据:', orderData)
          } catch (e) {
            console.error('解析路由数据失败:', e)
          }
        }
        
        // 如果有路由数据，使用路由数据
        if (orderData) {
          orderItems.value = orderData.items || []
          merchantInfo.value = {
            merchantId: orderData.merchantId,
            merchantName: orderData.merchantName,
            logo: '/assets/placeholder.png',
            deliveryTime: 30,
            deliveryFee: orderData.deliveryFee || 3.5,
            minOrderAmount: 20
          }
          console.log('设置商品列表:', orderItems.value)
          console.log('设置商家信息:', merchantInfo.value)
        } else {
          // 否则从localStorage获取订单数据
          const cartItems = JSON.parse(localStorage.getItem('cartItems') || '[]')
          const merchantId = route.params.merchantId || localStorage.getItem('currentMerchantId')
          const merchantName = localStorage.getItem('currentMerchantName')
          
          console.log('从localStorage获取数据:', { cartItems, merchantId, merchantName })
          
          if (cartItems.length === 0) {
            console.log('购物车为空，跳转到商家浏览页面')
            router.push('/browse')
            return
          }

          orderItems.value = cartItems
          
          // 获取商家信息
          if (merchantId) {
            const response = await merchantAPI.getMerchantById(merchantId)
            if (response && response.data) {
              merchantInfo.value = response.data
            } else {
              merchantInfo.value = null
            }
          }
        }

        // 获取用户地址列表
        await loadAddresses()
        
        // 获取可用优惠券
        await loadCoupons()

      } catch (error) {
        console.error('初始化订单失败:', error)
      }

      // 兜底 merchantId 赋值
      if (!merchantInfo.value.merchantId) {
        merchantInfo.value.merchantId = route.params.merchantId || localStorage.getItem('currentMerchantId')
        console.log('兜底赋值 merchantId:', merchantInfo.value.merchantId)
      }
    }

    const loadAddresses = async () => {
      try {
        const userId = localStorage.getItem('userId')
        // console.log('loadAddresses userId:', userId)
        if (!userId) {
          // console.error('未找到用户ID，无法加载地址')
          addresses.value = []
          return
        }
        const response = await addressAPI.getUserAddresses(userId, {})
        // console.log('getUserAddresses 返回类型:', typeof response)
        // console.log('getUserAddresses 返回内容:', response)
        // console.log('getUserAddresses 返回所有字段:', Object.keys(response))
        // console.log('getUserAddresses 返回 JSON:', JSON.stringify(response))
        if (response && response.code === 0) {
          addresses.value = response.data
          // 选择默认地址
          const defaultAddress = addresses.value.find(addr => addr.isDefault === 1)
          if (defaultAddress && !selectedAddress.value) {
            selectedAddress.value = defaultAddress
          }
        } else {
          addresses.value = []
        }
      } catch (error) {
        // console.error('加载地址失败:', error)
        addresses.value = []
      }
    }

    const loadCoupons = async () => {
      try {
        const userId = localStorage.getItem('userId')
        if (!userId) {
          availableCoupons.value = []
          return
        }
        // 第一步：获取所有优惠券ID
        const response = await couponAPI.getUserCoupons(userId)
        console.log('用户优惠券ID列表 response:', response)
        if (response && (response.code === 0 || response.code === 200) && Array.isArray(response.data)) {
          // 第二步：批量获取优惠券详情
          const couponDetails = await Promise.all(
            response.data.map(couponId => couponAPI.getCouponById(couponId))
          )
          console.log('优惠券详情列表:', couponDetails)
          // 组装前端展示数据
          availableCoupons.value = couponDetails.map(coupon => ({
            couponId: coupon.data.couponId,
            description: `满${coupon.data.threshold}减${coupon.data.faceValue}`,
            faceValue: coupon.data.faceValue,
            threshold: coupon.data.threshold,
            startTime: coupon.data.startTime,
            endTime: coupon.data.endTime
          }))
        } else {
          availableCoupons.value = []
        }
      } catch (error) {
        console.error('loadCoupons 异常:', error)
        availableCoupons.value = []
      }
    }

    const showAddressSelector = () => {
      showAddressModal.value = true
    }

    const closeAddressModal = () => {
      showAddressModal.value = false
    }

    const selectAddress = (address) => {
      selectedAddress.value = address
      showAddressModal.value = false
    }

    const addNewAddress = () => {
      // 跳转到地址管理页面
      router.push('/addresses')
    }

    const manageAddresses = () => {
      // 跳转到地址管理页面
      router.push('/addresses')
    }

    const showCouponSelector = async () => {
      console.log('showCouponSelector 被调用')
      await loadCoupons()
      showCouponModal.value = true
    }

    const closeCouponModal = () => {
      showCouponModal.value = false
    }

    const selectCoupon = (coupon) => {
      if (coupon && subtotal.value < coupon.threshold) {
        return // 不满足使用条件
      }
      selectedCoupon.value = coupon
      showCouponModal.value = false
    }

    const submitOrder = async () => {
      console.log('提交订单前 merchantInfo:', merchantInfo.value)
      console.log('提交订单前 merchantId:', merchantInfo.value.merchantId)
      if (!canSubmit.value) return

      submitting.value = true
      try {
        const userId = localStorage.getItem('userId')
        if (!userId) {
          throw new Error('未找到用户ID，无法提交订单')
        }

        // 修正 createOrder 调用
        // 打印所有关键参数
        console.log('提交订单参数 userId:', userId)
        console.log('提交订单参数 orderAmount:', subtotal.value + deliveryFee.value)
        console.log('提交订单参数 couponId:', selectedCoupon.value?.couponId)
        console.log('提交订单参数 addressId:', selectedAddress.value?.addressId)
        console.log('提交订单参数 merchantId:', merchantInfo.value?.merchantId)
        console.log('提交订单参数 dishQuantities:', orderItems.value.map(item => ({ dishId: item.dishId, quantity: item.quantity })))

        const response = await orderAPI.createOrder(userId, {
          orderAmount: subtotal.value + deliveryFee.value,
          couponId: selectedCoupon.value?.couponId,
          addressId: selectedAddress.value.addressId,
          merchantId: merchantInfo.value.merchantId,
          dishQuantities: orderItems.value.map(item => ({ dishId: item.dishId, quantity: item.quantity })),
        })
        // 打印完整 response
        console.log('createOrder response:', response)
        console.log('下单返回的 response.data:', response.data)

        if (response && response.code === 0) {
          // 清空购物车
          localStorage.removeItem('cartItems')

          // 新增：通过日志ID查订单ID
          try {
            const logId = response.data
            const logRes = await orderLogAPI.getOrderLogById(logId)
            console.log('orderLogAPI.getOrderLogById 返回:', logRes)
            if (logRes && logRes.code === 0 && logRes.data && logRes.data.orderId) {
              // 跳转到支付页面，使用真实 orderId
              router.push({
                name: 'Payment',
                params: { orderId: logRes.data.orderId },
                query: {
                  amount: finalAmount.value,
                  paymentMethod: selectedPaymentMethod.value
                }
              })
            } else {
              alert('未能获取订单ID，请重试')
            }
          } catch (err) {
            console.error('获取订单ID失败:', err)
            alert('未能获取订单ID，请重试')
          }
        } else {
          alert('下单失败，请重试')
        }
      } catch (error) {
        console.error('提交订单失败:', error)
        console.error('orderItems.value:', orderItems.value)
        const dishQuantities = orderItems.value.map(item => ({ dishId: item.dishId, quantity: item.quantity }))
        console.error('dishQuantities:', dishQuantities)
        alert('下单失败，请重试')
      } finally {
        submitting.value = false
      }
    }

    // 生命周期
    onMounted(() => {
      initializeOrder()
    })

    return {
      merchantInfo,
      orderItems,
      addresses,
      selectedAddress,
      availableCoupons,
      selectedCoupon,
      remark,
      selectedPaymentMethod,
      submitting,
      showAddressModal,
      showCouponModal,
      paymentMethods,
      subtotal,
      deliveryFee,
      discountAmount,
      finalAmount,
      estimatedDeliveryTime,
      canSubmit,
      merchantLogo,
      goBack,
      showAddressSelector,
      closeAddressModal,
      selectAddress,
      addNewAddress,
      manageAddresses,
      showCouponSelector,
      closeCouponModal,
      selectCoupon,
      submitOrder
    }
  }
}
</script>

<style scoped>
.order-confirm {
  min-height: 100vh;
  background: #f8f9fa;
  padding-bottom: 120px; /* 增加底部padding，确保内容不被导航栏遮挡 */
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
  padding: 80px 20px;
  color: #666;
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

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* 通用区块样式 */
.address-section,
.merchant-section,
.items-section,
.coupon-section,
.delivery-section,
.remark-section,
.payment-section,
.price-section {
  background: white;
  margin: 8px 16px;
  border-radius: 12px;
  padding: 16px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
}

.section-title {
  display: flex;
  align-items: center;
  margin-bottom: 12px;
  font-size: 16px;
  font-weight: 600;
  color: #333;
}

.section-title i {
  margin-right: 8px;
  font-size: 18px;
}

/* 地址选择 */
.address-card {
  display: flex;
  align-items: center;
  padding: 12px;
  background: #f8f9fa;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.address-card:hover {
  background: #e9ecef;
}

.address-info {
  flex: 1;
}

.receiver-info {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 6px;
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

.empty-address {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 12px;
  border: 2px dashed #ddd;
  border-radius: 8px;
  cursor: pointer;
  color: #999;
}

.arrow-icon {
  color: #999;
  font-size: 14px;
}

/* 商家信息 */
.merchant-info {
  display: flex;
  align-items: center;
  gap: 12px;
}

.merchant-logo {
  width: 40px;
  height: 40px;
  border-radius: 8px;
  object-fit: cover;
}

.merchant-name {
  font-size: 16px;
  font-weight: 600;
  margin: 0 0 4px 0;
  color: #333;
}

.merchant-meta {
  display: flex;
  gap: 16px;
  font-size: 12px;
  color: #666;
}

/* 商品列表 */

.item-card {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 8px 0;
}

.item-card:not(:last-child) {
  border-bottom: 1px solid #f0f0f0;
  padding-bottom: 12px;
  margin-bottom: 12px;
}

.item-image {
  width: 60px;
  height: 60px;
  border-radius: 8px;
  object-fit: cover;
}

.item-info {
  flex: 1;
}

.item-name {
  font-size: 14px;
  font-weight: 500;
  margin: 0 0 4px 0;
  color: #333;
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

/* 优惠券选择 */
.coupon-selector {
  display: flex;
  align-items: center;
  justify-content: space-between;
  cursor: pointer;
  padding: 8px 0;
}

.coupon-info {
  display: flex;
  align-items: center;
  color: #333;
}

.coupon-info i {
  margin-right: 8px;
  font-size: 16px;
}

.coupon-value {
  display: flex;
  align-items: center;
  gap: 8px;
}

.discount {
  color: #e74c3c;
  font-weight: 600;
}

/* 配送信息 */
.delivery-item {
  display: flex;
  justify-content: space-between;
  margin-bottom: 8px;
}

.delivery-item:last-child {
  margin-bottom: 0;
}

.delivery-item .label {
  color: #666;
}

.delivery-item .value {
  color: #333;
  font-weight: 500;
}

/* 备注 */
.remark-input {
  width: 100%;
  min-height: 80px;
  padding: 12px;
  border: 1px solid #e1e5e9;
  border-radius: 8px;
  font-size: 14px;
  resize: vertical;
  font-family: inherit;
}

.remark-input:focus {
  outline: none;
  border-color: #007BFF;
}

/* 支付方式 */
.payment-methods {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.payment-method {
  display: flex;
  align-items: center;
  padding: 12px;
  border: 1px solid #e1e5e9;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.payment-method:hover {
  border-color: #007BFF;
}

.payment-method.active {
  border-color: #007BFF;
  background: #f0f8ff;
}

.method-icon {
  margin-right: 12px;
  font-size: 18px;
}

.method-name {
  flex: 1;
  font-size: 14px;
  color: #333;
}

.check-icon {
  color: #007BFF;
  font-size: 16px;
}

/* 价格详情 */
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

/* 提交区域 */
.submit-section {
  position: sticky;
  bottom: 0;
  background: white;
  padding: 16px;
  border-top: 1px solid #e9ecef;
  display: flex;
  align-items: center;
  gap: 16px;
  box-shadow: 0 -2px 8px rgba(0, 0, 0, 0.1);
  margin-bottom: 60px; /* 为底部导航留出空间 */
  z-index: 10;
}

.total-info {
  flex: 1;
}

.total-label {
  font-size: 12px;
  color: #666;
  margin-bottom: 2px;
}

.total-amount {
  font-size: 18px;
  font-weight: 700;
  color: #e74c3c;
}

.submit-btn {
  padding: 12px 32px;
  background: linear-gradient(135deg, #007BFF, #00D4FF);
  color: white;
  border: none;
  border-radius: 25px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  position: relative;
  z-index: 15; /* 确保按钮在最上层 */
}

.submit-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 123, 255, 0.3);
}

.submit-btn:disabled {
  background: #ccc;
  cursor: not-allowed;
  transform: none;
  box-shadow: none;
}

/* 弹窗样式 */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: flex-end;
  z-index: 2000;
}

.modal-content {
  width: 100%;
  max-height: 70vh;
  background: white;
  border-radius: 16px 16px 0 0;
  overflow: hidden;
  animation: slideUp 0.3s ease;
}

@keyframes slideUp {
  from { transform: translateY(100%); }
  to { transform: translateY(0); }
}

.modal-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 16px 20px;
  border-bottom: 1px solid #e9ecef;
}

.modal-header h3 {
  margin: 0;
  font-size: 16px;
  font-weight: 600;
  color: #333;
}

.modal-close {
  width: 32px;
  height: 32px;
  background: none;
  border: none;
  font-size: 18px;
  cursor: pointer;
  color: #666;
}

.modal-body {
  max-height: 50vh;
  overflow-y: auto;
  padding: 16px 20px;
}

/* 地址选项 */
.address-option {
  display: flex;
  align-items: center;
  padding: 12px 0;
  border-bottom: 1px solid #f0f0f0;
  cursor: pointer;
}

.address-option:last-child {
  border-bottom: none;
}

.address-option.selected {
  color: #007BFF;
}

.address-content {
  flex: 1;
}

.default-tag {
  background: #007BFF;
  color: white;
  font-size: 10px;
  padding: 2px 6px;
  border-radius: 4px;
  margin-left: 8px;
}

.add-address,
.manage-address {
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 16px;
  border: 2px dashed #ddd;
  border-radius: 8px;
  cursor: pointer;
  color: #666;
  margin-top: 12px;
  transition: all 0.3s ease;
}

.add-address:hover,
.manage-address:hover {
  border-color: #007BFF;
  color: #007BFF;
}

.manage-address {
  border-color: #28a745;
  color: #28a745;
}

.manage-address:hover {
  border-color: #1e7e34;
  color: #1e7e34;
}

.add-icon,
.manage-icon {
  margin-right: 8px;
  font-size: 18px;
}

/* 优惠券选项 */
.coupon-option {
  display: flex;
  align-items: center;
  padding: 12px;
  border: 1px solid #e1e5e9;
  border-radius: 8px;
  margin-bottom: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.coupon-option:hover:not(.disabled) {
  border-color: #007BFF;
}

.coupon-option.selected {
  border-color: #007BFF;
  background: #f0f8ff;
}

.coupon-option.disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.coupon-content {
  display: flex;
  align-items: center;
  flex: 1;
  gap: 12px;
}

.coupon-value {
  font-size: 18px;
  font-weight: 700;
  color: #e74c3c;
  min-width: 60px;
}

.coupon-desc {
  flex: 1;
}

.coupon-title {
  font-size: 14px;
  font-weight: 500;
  color: #333;
  margin-bottom: 2px;
}

.coupon-condition {
  font-size: 12px;
  color: #666;
}

.no-coupon-option {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 12px 0;
  border-top: 1px solid #e9ecef;
  cursor: pointer;
  color: #666;
  margin-top: 8px;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .order-confirm {
    padding-bottom: 140px; /* 在移动设备上增加更多底部空间 */
  }
  
  .submit-section {
    flex-direction: column;
    align-items: stretch;
    gap: 12px;
    margin-bottom: 70px; /* 为底部导航留出更多空间 */
  }
  
  .total-info {
    text-align: center;
  }
  
  .submit-btn {
    width: 100%;
  }
}

/* 适配iPhone底部安全区域 */
@supports (padding-bottom: env(safe-area-inset-bottom)) {
  .order-confirm {
    padding-bottom: calc(120px + env(safe-area-inset-bottom));
  }
  
  .submit-section {
    margin-bottom: calc(60px + env(safe-area-inset-bottom));
  }
  
  @media (max-width: 768px) {
    .order-confirm {
      padding-bottom: calc(140px + env(safe-area-inset-bottom));
    }
    
    .submit-section {
      margin-bottom: calc(70px + env(safe-area-inset-bottom));
    }
  }
}
</style>
