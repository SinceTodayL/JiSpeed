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
        <div class="address-info" v-if="orderDetail.deliveryAddress">
          <div class="receiver-info">
            <span class="receiver-name">{{ orderDetail.deliveryAddress.receiverName || '未知收件人' }}</span>
            <span class="receiver-phone">{{ orderDetail.deliveryAddress.receiverPhone || '未知电话' }}</span>
          </div>
          <div class="address-detail">{{ orderDetail.deliveryAddress.detailedAddress || '未知地址' }}</div>
        </div>
        <div class="address-info" v-else>
          <div class="address-loading">加载地址信息中...</div>
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
            <div class="merchant-name">{{ orderDetail.merchantName || '未知商家' }}</div>
            <div class="merchant-contact">
              <span>📞 {{ orderDetail.merchantPhone || '未知电话' }}</span>
              <span>📍 {{ orderDetail.merchantAddress || '未知地址' }}</span>
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
              :alt="item.dishName || '未知商品'"
              class="item-image"
            />
            <div class="item-info">
              <div class="item-name">{{ item.dishName || '未知商品' }}</div>
              <div class="item-price">¥{{ item.price || 0 }}</div>
            </div>
            <div class="item-details">
              <div class="item-quantity">×{{ item.quantity || 1 }}</div>
              <div class="item-subtotal">¥{{ item.subtotal || ((item.price || 0) * (item.quantity || 1)) }}</div>
            </div>
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
            <span class="value">¥{{ ((orderDetail.totalAmount || orderDetail.orderAmount || 0) - (orderDetail.deliveryFee || 0)).toFixed(1) }}</span>
          </div>
          <div class="price-item">
            <span class="label">配送费</span>
            <span class="value">¥{{ (orderDetail.deliveryFee || 0).toFixed(1) }}</span>
          </div>
          <div v-if="orderDetail.discountAmount > 0" class="price-item discount">
            <span class="label">优惠金额</span>
            <span class="value">-¥{{ (orderDetail.discountAmount || 0).toFixed(1) }}</span>
          </div>
          <div class="price-item total">
            <span class="label">实付金额</span>
            <span class="value">¥{{ (orderDetail.finalAmount || orderDetail.orderAmount || 0).toFixed(1) }}</span>
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
        v-if="[1, 7, 8, 9].includes(orderDetail.orderStatus)"
        @click="confirmOrder"
        class="action-btn primary"
      >
        确认收货
      </button>
      
      <button 
        v-if="[0].includes(orderDetail.orderStatus)"
        @click="cancelOrder"
        class="action-btn secondary"
      >
        取消订单
      </button>
      
      <button 
        v-if="orderDetail.orderStatus === 2 || orderDetail.orderStatus === 9"
        @click="showReviewModal = true"
        class="action-btn secondary"
      >
        评价订单
      </button>
      
      <button 
        v-if="orderDetail.orderStatus === 2 || orderDetail.orderStatus === 9"
        @click="showComplaintModal = true"
        class="action-btn secondary"
      >
        投诉订单
      </button>
      
      <button 
        v-if="orderDetail.orderStatus === 2 || orderDetail.orderStatus === 9"
        @click="showRefundModal = true"
        class="action-btn secondary"
      >
        申请退款
      </button>

      <button 
        @click="contactService"
        class="action-btn secondary"
      >
        联系客服
      </button>
    </div>

    <!-- 评价弹窗 -->
    <div v-if="showReviewModal" class="review-modal" @click="showReviewModal = false">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3>订单评价</h3>
          <button @click="showReviewModal = false" class="close-btn">×</button>
        </div>
        <div class="modal-body">
          <!-- 评分 -->
          <div class="rating-section">
            <h4>总体评分</h4>
            <div class="star-rating">
              <span 
                v-for="star in 5" 
                :key="star" 
                class="star"
                :class="{ 'active': star <= reviewForm.rating }"
                @click="reviewForm.rating = star"
              >
                ★
              </span>
            </div>
            <div class="rating-text">{{ getRatingText(reviewForm.rating) }}</div>
          </div>

          <!-- 评价内容 -->
          <div class="review-content">
            <h4>评价内容</h4>
            <textarea 
              v-model="reviewForm.content" 
              placeholder="请分享您对商家和菜品的评价..."
              class="review-textarea"
            ></textarea>
          </div>

          <!-- 匿名评价 -->
          <div class="anonymous-section">
            <label class="checkbox-label">
              <input type="checkbox" v-model="reviewForm.isAnonymous">
              <span>匿名评价</span>
            </label>
          </div>
        </div>
        <div class="modal-footer">
          <button @click="showReviewModal = false" class="cancel-btn">取消</button>
          <button 
            @click="submitReview" 
            class="submit-btn"
          >
            提交评价
          </button>
        </div>
      </div>
    </div>

    <!-- 投诉弹窗 -->
    <div v-if="showComplaintModal" class="review-modal" @click="showComplaintModal = false">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3>订单投诉</h3>
          <button @click="showComplaintModal = false" class="close-btn">×</button>
        </div>
        <div class="modal-body">
          <!-- 投诉内容 -->
          <div class="complaint-content">
            <h4>投诉原因</h4>
            <textarea 
              v-model="complaintForm.description" 
              placeholder="请详细描述您遇到的问题，以便我们更好地为您解决..."
              class="review-textarea complaint-textarea"
            ></textarea>
          </div>

          <!-- 上传图片（可选） -->
          <div class="upload-section">
            <h4>上传凭证（可选）</h4>
            <p class="upload-hint">您可以上传相关图片作为投诉凭证</p>
            <div class="upload-area">
              <button class="upload-btn">
                <i class="upload-icon">+</i>
                <span>上传图片</span>
              </button>
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button @click="showComplaintModal = false" class="cancel-btn">取消</button>
          <button 
            @click="submitComplaint" 
            class="submit-btn complaint-btn"
          >
            提交投诉
          </button>
        </div>
      </div>
    </div>

    <!-- 退款弹窗 -->
    <div v-if="showRefundModal" class="review-modal" @click="showRefundModal = false">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3>申请退款</h3>
          <button @click="showRefundModal = false" class="close-btn">×</button>
        </div>
        <div class="modal-body">
          <!-- 退款原因 -->
          <div class="refund-reason">
            <h4>退款原因</h4>
            <select v-model="refundForm.reason" class="refund-select">
              <option value="">请选择退款原因</option>
              <option value="商品质量问题">商品质量问题</option>
              <option value="送餐太慢">送餐太慢</option>
              <option value="菜品与描述不符">菜品与描述不符</option>
              <option value="服务态度差">服务态度差</option>
              <option value="其他原因">其他原因</option>
            </select>
          </div>

          <!-- 详细说明 -->
          <div class="refund-description">
            <h4>详细说明</h4>
            <textarea 
              v-model="refundForm.description" 
              placeholder="请详细描述您的退款原因..."
              class="refund-textarea"
            ></textarea>
          </div>

          <!-- 退款金额 -->
          <div class="refund-amount">
            <h4>退款金额</h4>
            <div class="amount-info">
              <span>最大可退金额：¥{{ orderDetail?.finalAmount?.toFixed(1) || '0.0' }}</span>
              <input 
                type="number" 
                v-model="refundForm.amount" 
                :max="orderDetail?.finalAmount || 0" 
                min="0" 
                step="0.1" 
                class="amount-input"
              />
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button @click="showRefundModal = false" class="cancel-btn">取消</button>
          <button 
            @click="submitRefund" 
            class="submit-btn"
            :disabled="!refundForm.reason || !refundForm.description || !refundForm.amount"
          >
            提交退款申请
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { orderAPI } from '@/api/order.js'
import { merchantDishAPI, merchantAPI } from '@/api/merchant.js'
import { addressAPI, reviewAPI, complaintAPI } from '@/api/user.js'

export default {
  name: 'OrderDetail',
  setup() {
    const route = useRoute()
    const router = useRouter()

    // 响应式数据
    const orderDetail = ref(null)
    const loading = ref(false)
    const showReviewModal = ref(false)
    const showComplaintModal = ref(false)
    const reviewForm = ref({
      rating: 5,
      content: '',
      isAnonymous: false
    })
    const complaintForm = ref({
      description: '',
      images: []
    })

    // 方法
    const goBack = () => {
      router.back()
    }

    const loadOrderDetail = async () => {
      loading.value = true

      try {
        const orderId = route.params.orderId
        const response = await orderAPI.getOrderDetail(orderId)
        
        // 直接使用返回的数据，不检查错误码
        if (response && response.data) {
          // 处理缺失的必要字段
          const data = response.data;
          
          // 设置默认值
          data.orderStatusText = getOrderStatusText(data.orderStatus);
          data.finalAmount = data.orderAmount || data.finalAmount || 0;
          data.deliveryFee = data.deliveryFee || 0;
          data.discountAmount = data.discountAmount || 0;
          
          // 如果地址ID存在但缺少地址详情
          if (data.addressId && !data.deliveryAddress) {
            data.deliveryAddress = {
              addressId: data.addressId,
              receiverName: "收件人",
              receiverPhone: "未知电话",
              detailedAddress: "地址信息加载中..."
            };
          }
          
          // 确保订单项数组存在
          data.orderItems = data.orderDishes || data.orderItems || [];
          
          // 如果没有商家信息，设置默认值
          if (!data.merchantName) {
            data.merchantName = "未知商家";
            data.merchantPhone = "未知电话";
            data.merchantAddress = "未知地址";
          }
          
          // 设置订单详情
          orderDetail.value = data;
          
          // 异步加载额外信息
          loadAdditionalInfo(data);
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
    
    // 异步加载额外信息
    const loadAdditionalInfo = async (orderData) => {
      try {
        // 1. 获取地址信息
        console.log('加载地址信息，orderData:', orderData);
        if (orderData.addressId ) {
          try {
            // 获取用户的所有地址
            const addressResponse = await addressAPI.getUserAddresses(orderData.userId, {});
            console.log('获取用户地址成功:', addressResponse);
            if (addressResponse && addressResponse.data && Array.isArray(addressResponse.data)) {
              // 查找与订单addressId匹配的地址
              const matchedAddress = addressResponse.data.find(addr => addr.addressId === orderData.addressId);
              if (matchedAddress) {
                orderData.deliveryAddress = matchedAddress;
                if (orderDetail.value) {
                  orderDetail.value.deliveryAddress = matchedAddress;
                }
              }
            }
          } catch (e) {
            console.error('加载地址信息失败:', e);
          }
        }

        // 2. 获取商家信息
        if (orderData.merchantId ) {
          try {
            const merchantResponse = await merchantAPI.getMerchantById(orderData.merchantId);
            if (merchantResponse && merchantResponse.data) {
              const merchantData = merchantResponse.data;
              // 更新商家信息
              const merchantInfo = {
                merchantName: merchantData.merchantName || merchantData.name || '未知商家',
                merchantPhone: merchantData.phone || merchantData.contactPhone || merchantData.contactInfo || '未知电话',
                merchantAddress: merchantData.address || merchantData.location || '未知地址',
                merchantLogo: merchantData.logo || merchantData.imageUrl || null
              };
              
              // 将商家信息合并到订单数据中
              Object.assign(orderData, merchantInfo);
              
              if (orderDetail.value) {
                Object.assign(orderDetail.value, merchantInfo);
              }
            }
          } catch (e) {
            console.error('加载商家信息失败:', e);
          }
        }

        // 3. 尝试加载菜品详细信息
        if (orderData.orderItems && orderData.orderItems.length > 0) {
          try {
            const merchantId = orderData.merchantId;
            const updatedItems = await Promise.all(orderData.orderItems.map(async (item) => {
              try {
                const dishRes = await merchantDishAPI.getDishById(merchantId, item.dishId);
                if (dishRes && dishRes.data) {
                  return {
                    ...item,
                    dishName: dishRes.data.dishName || dishRes.data.name || item.dishName || '未知商品',
                    price: dishRes.data.price || dishRes.data.dishPrice || 0,
                    coverUrl: dishRes.data.coverUrl || dishRes.data.imageUrl || null,
                    subtotal: (dishRes.data.price || 0) * (item.quantity || 1)
                  };
                }
              } catch (e) {
                console.error('加载菜品详情失败:', e);
              }
              return item;
            }));
            
            if (orderDetail.value) {
              orderDetail.value.orderItems = updatedItems;
            }
          } catch (e) {
            console.error('加载菜品详情失败:', e);
          }
        }
      } catch (error) {
        console.error('加载额外信息失败:', error);
      }
    }

    const getStatusIcon = (status) => {
      const icons = {
        0: '💳', // Unpaid
        1: '✅', // Paid
        2: '�', // Confirmed
        3: '⭐', // Reviewed
        4: '🔄', // Aftersales
        5: '🛠️', // AftersalesCompleted
        6: '❌', // Cancelled
        7: '🚲', // Assigned
        8: '🛵', // InDelivery
        9: '📦'  // Delivered
      }
      return icons[status] || '📋'
    }
    
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
        8: '配送中', // InDelivery
        9: '已送达'  // Delivered
      }
      return statusTexts[status] || '未知状态'
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
        if (response && (response.code === 0 || response.code === 200 || response.data)) {
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
        if (response && (response.code === 0 || response.code === 200 || response.data)) {
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

    // 获取评分对应的文字描述
    const getRatingText = (rating) => {
      const texts = {
        1: '非常差',
        2: '较差',
        3: '一般',
        4: '满意',
        5: '非常满意'
      }
      return texts[rating] || ''
    }

    // 提交评价
    const submitReview = async () => {
      if (!reviewForm.value.content || reviewForm.value.content.length < 5) {
        alert('请输入至少5个字的评价内容')
        return
      }

      try {
        const userId = localStorage.getItem('userId')
        if (!userId) {
          alert('请先登录')
          return
        }

        const reviewData = {
          orderId: orderDetail.value.orderId,
          dishId: null, // 整单评价不指定菜品ID
          rating: reviewForm.value.rating,
          content: reviewForm.value.content,
          isAnonymous: reviewForm.value.isAnonymous ? 1 : 0
        }

        const response = await reviewAPI.submitReview(userId, reviewData)
        
        if (response) {
          alert('评价提交成功，谢谢您的反馈！')
          showReviewModal.value = false
          // 刷新订单数据
          loadOrderDetail()
        } else {
          alert('评价提交失败，请重试')
        }
      } catch (error) {
        console.error('提交评价失败:', error)
        alert('评价提交失败，请重试')
      }
    }
    
    // 提交投诉
    const submitComplaint = async () => {
      if (!complaintForm.value.description || complaintForm.value.description.length < 10) {
        alert('请详细描述您的投诉，至少10个字')
        return
      }

      try {
        const userId = localStorage.getItem('userId')
        if (!userId) {
          alert('请先登录')
          return
        }

        const complaintData = {
          orderId: orderDetail.value.orderId,
          description: complaintForm.value.description
        }

        const response = await complaintAPI.submitComplaint(userId, complaintData)
        
        if (response) {
          alert('投诉提交成功，我们会尽快处理！')
          showComplaintModal.value = false
          // 刷新订单数据
          loadOrderDetail()
        } else {
          alert('投诉提交失败，请重试')
        }
      } catch (error) {
        console.error('提交投诉失败:', error)
        alert('投诉提交失败，请重试')
      }
    }

    // 生命周期
    onMounted(() => {
      loadOrderDetail()
    })

    // 退款相关
    const showRefundModal = ref(false)
    const refundForm = ref({
      reason: '',
      description: '',
      amount: 0
    })

    // 提交退款申请
    const submitRefund = async () => {
      if (!refundForm.value.reason) {
        alert('请选择退款原因')
        return
      }
      if (!refundForm.value.description || refundForm.value.description.length < 5) {
        alert('请输入至少5个字的详细说明')
        return
      }
      if (!refundForm.value.amount || refundForm.value.amount <= 0) {
        alert('请输入有效的退款金额')
        return
      }
      if (refundForm.value.amount > (orderDetail.value?.finalAmount || 0)) {
        alert('退款金额不能超过订单金额')
        return
      }

      try {
        const userId = localStorage.getItem('userId')
        if (!userId) {
          alert('请先登录')
          return
        }

        const refundData = {
          reason: refundForm.value.reason + (refundForm.value.description ? ': ' + refundForm.value.description : ''),
          refundAmount: parseFloat(refundForm.value.amount)
        }

        const response = await orderAPI.createRefund(userId, orderDetail.value.orderId, refundData)
        if (response && (response.code === 0 || response.code === 200 || response.data)) {
          alert('退款申请已提交')
          showRefundModal.value = false
          // 重新加载订单详情
          loadOrderDetail()
        } else {
          alert('退款申请提交失败，请重试')
        }
      } catch (error) {
        console.error('提交退款申请失败:', error)
        alert('退款申请提交失败，请重试')
      }
    }

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
      contactService,
      showReviewModal,
      reviewForm,
      getRatingText,
      submitReview,
      showComplaintModal,
      complaintForm,
      submitComplaint,
      showRefundModal,
      refundForm,
      submitRefund
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
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 20px;
  padding: 20px;
}

.item-card {
  display: flex;
  flex-direction: column;
  border: 1px solid #f0f0f0;
  border-radius: 8px;
  padding: 16px;
  box-shadow: 0 2px 6px rgba(0,0,0,0.05);
}

.item-card:last-child {
  border-bottom: 1px solid #f0f0f0;
}

.item-image {
  width: 100%;
  height: 120px;
  border-radius: 8px;
  object-fit: cover;
  margin-bottom: 12px;
}

.item-info {
  width: 100%;
  padding: 4px 0;
  margin-bottom: 8px;
}

.item-name {
  font-size: 16px;
  font-weight: 600;
  color: #333;
  margin-bottom: 8px;
}

.item-price {
  font-size: 14px;
  color: #e74c3c;
}

.item-details {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  margin-top: 8px;
}

.item-quantity {
  font-size: 14px;
  color: #666;
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

.choose-address-btn {
  margin-left: 12px;
  background: #007BFF;
  color: #fff;
  border: none;
  border-radius: 16px;
  padding: 4px 12px;
  font-size: 13px;
  cursor: pointer;
}

.choose-address-btn:hover {
  background: #00D4FF;
}

/* 评价弹窗样式 */
.review-modal {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal-content {
  width: 90%;
  max-width: 500px;
  background: white;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
  animation: modalFadeIn 0.3s ease;
}

@keyframes modalFadeIn {
  from { opacity: 0; transform: translateY(20px); }
  to { opacity: 1; transform: translateY(0); }
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 20px;
  border-bottom: 1px solid #e9ecef;
}

.modal-header h3 {
  margin: 0;
  font-size: 18px;
  color: #333;
}

.close-btn {
  background: none;
  border: none;
  font-size: 24px;
  color: #999;
  cursor: pointer;
  padding: 0;
  line-height: 1;
}

.modal-body {
  padding: 20px;
}

.rating-section,
.review-content,
.anonymous-section {
  margin-bottom: 20px;
}

.rating-section h4,
.review-content h4 {
  font-size: 16px;
  font-weight: 500;
  margin: 0 0 10px 0;
  color: #444;
}

.star-rating {
  display: flex;
  gap: 8px;
  margin: 10px 0;
}

.star {
  font-size: 32px;
  color: #ddd;
  cursor: pointer;
  transition: color 0.2s ease;
}

.star:hover,
.star.active {
  color: #ffcc00;
}

.rating-text {
  font-size: 14px;
  color: #666;
  margin-top: 5px;
}

.review-textarea {
  width: 90%;
  height: 120px;
  padding: 12px 12px;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  resize: none;
  font-size: 14px;
  font-family: inherit;
  margin-top: 8px;
  color: #333;
  background-color: white;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.05);
  transition: border-color 0.3s, box-shadow 0.3s;
}

.review-textarea:focus {
  outline: none;
  border-color: #007BFF;
  box-shadow: 0 2px 10px rgba(0, 123, 255, 0.1);
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  color: #666;
  cursor: pointer;
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  padding: 16px 20px;
  border-top: 1px solid #e9ecef;
}

.cancel-btn,
.submit-btn {
  padding: 10px 20px;
  border-radius: 20px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
}

.cancel-btn {
  background: #f8f9fa;
  border: 1px solid #ddd;
  color: #666;
}

.cancel-btn:hover {
  background: #e9ecef;
}

.submit-btn {
  background: #007BFF;
  border: none;
  color: white;
}

.submit-btn:hover {
  background: #0069d9;
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0, 123, 255, 0.3);
}

.submit-btn:disabled {
  background: #b0d4ff;
  cursor: not-allowed;
  transform: none;
  box-shadow: none;
}

/* 投诉模态框样式 */
.complaint-textarea {
  min-height: 150px;
}

.upload-section {
  margin-bottom: 20px;
}

.upload-hint {
  font-size: 13px;
  color: #666;
  margin-bottom: 10px;
}

.upload-area {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  margin-top: 10px;
}

.upload-btn {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  width: 100px;
  height: 100px;
  border: 1px dashed #ddd;
  border-radius: 8px;
  background: #f9f9f9;
  cursor: pointer;
  transition: all 0.3s ease;
}

.upload-btn:hover {
  border-color: #007BFF;
  background: #f0f7ff;
}

.upload-icon {
  font-size: 24px;
  color: #999;
  margin-bottom: 8px;
}

.complaint-btn {
  background: #ff4d4f;
}

.complaint-btn:hover {
  background: #ff7875;
  box-shadow: 0 4px 8px rgba(255, 77, 79, 0.3);
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
  
  .items-list {
    grid-template-columns: repeat(2, 1fr);
    gap: 15px;
    padding: 15px;
  }
  
  .item-image {
    height: 100px;
  }
  
  .action-section {
    flex-direction: column;
  }
  
  .action-btn {
    width: 100%;
  }
}

@media (max-width: 480px) {
  .items-list {
    grid-template-columns: 1fr;
    gap: 10px;
    padding: 10px;
  }
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* 退款样式 */
.refund-reason,
.refund-description,
.refund-amount {
  margin-bottom: 16px;
  display: flex;
  flex-direction: column;
  align-items: center;
  width: 100%;
}

.refund-reason h4,
.refund-description h4,
.refund-amount h4 {
  font-size: 14px;
  margin-bottom: 8px;
  color: #333;
  align-self: flex-start;
  margin-left: 5%;
  width: 90%;
}

.refund-select {
  width: 90%;
  margin: 0 auto;
  padding: 10px;
  border: 1px solid #eee;
  border-radius: 8px;
  font-size: 14px;
  background-color: #fff;
  color: #333;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.refund-textarea {
  width: 90%;
  margin: 0 auto;
  height: 100px;
  padding: 10px;
  border: 1px solid #eee;
  border-radius: 8px;
  font-size: 14px;
  resize: none;
  color: #333;
  background-color: #fff;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.refund-textarea::placeholder {
  color: #aaa;
}

.amount-info {
  display: flex;
  flex-direction: column;
  gap: 2px;
  width: 100%;
  margin: 0 auto;
}

.amount-info span {
  font-size: 14px;
  color: #333;
  margin-bottom: 5px;
  display: inline-block;
  padding-left: 5%;
  text-align: left;
  width: 90%;
}

.amount-input {
  width: 90%;
  margin: 0 auto;
  padding: 10px;
  border: 1px solid #eee;
  border-radius: 8px;
  font-size: 14px;
  color: #333;
  background-color: #fff;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  /* 隐藏数字输入框的上下箭头 */
  appearance: textfield;
  -webkit-appearance: none;
  -moz-appearance: textfield;
  display: block;
}

/* 兼容不同浏览器，确保上下箭头被隐藏 */
.amount-input::-webkit-outer-spin-button,
.amount-input::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}
</style>
