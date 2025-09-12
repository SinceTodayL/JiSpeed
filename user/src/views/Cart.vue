<template>
  <div class="cart-page">
    <!-- 页面头部 -->
    <div class="cart-header">
      <div class="header-content">
        <h1 class="page-title">购物车</h1>
        <div class="cart-count">
          <span v-if="cartItems.length > 0">({{ totalItems }}件商品)</span>
        </div>
      </div>
    </div>

    <!-- 购物车内容 -->
    <div class="cart-content">
      <!-- 加载状态 -->
      <div v-if="loading" class="loading-container">
        <div class="loading-spinner"></div>
        <p>正在加载购物车...</p>
      </div>

      <!-- 空购物车 -->
      <div v-else-if="cartItems.length === 0" class="empty-cart">
        <div class="empty-icon">🛒</div>
        <h2 class="empty-title">购物车是空的</h2>
        <p class="empty-desc">去挑选一些美味的商品吧</p>
        <button @click="goToMerchants" class="browse-btn">
          去逛逛
        </button>
      </div>

      <!-- 购物车列表 -->
      <div v-else class="cart-list">
        <!-- 全选操作 -->
        <div class="select-all-section">
          <label class="select-all-checkbox">
            <input 
              type="checkbox" 
              v-model="isAllSelected" 
              @change="toggleSelectAll"
            />
            <span class="checkmark"></span>
            <span class="select-text">全选</span>
          </label>
          <button 
            @click="deleteSelected"
            :disabled="selectedItems.length === 0"
            class="delete-selected-btn"
          >
            删除选中 ({{ selectedItems.length }})
          </button>
        </div>

        <!-- 按商家分组显示 -->
        <div 
          v-for="merchantGroup in groupedCartItems"
          :key="merchantGroup.merchantId"
          class="merchant-group"
        >
          <!-- 商家信息 -->
          <div class="merchant-header">
            <label class="merchant-checkbox">
              <input 
                type="checkbox" 
                v-model="merchantGroup.selected"
                @change="toggleMerchantSelect(merchantGroup)"
              />
              <span class="checkmark"></span>
            </label>
            <div class="merchant-info">
              <h3 class="merchant-name">🏪 {{ merchantGroup.merchantName }}</h3>
              <span class="merchant-status">{{ merchantGroup.items.length }}件商品</span>
            </div>
          </div>

          <!-- 商品列表 -->
          <div class="items-list">
            <div 
              v-for="item in merchantGroup.items"
              :key="item.cartItemId"
              :class="['cart-item', { 'unavailable': !item.isAvailable }]"
            >
              <!-- 选择框 -->
              <label class="item-checkbox">
                <input 
                  type="checkbox" 
                  v-model="item.selected"
                  @change="updateItemSelection"
                  :disabled="!item.isAvailable"
                />
                <span class="checkmark"></span>
              </label>

              <!-- 商品图片 -->
              <div class="item-image-container">
                <img 
                  :src="item.image || item.coverUrl || '/assets/placeholder.png'"
                  :alt="item.dishName"
                  class="item-image"
                  @error="handleImageError"
                />
                <div v-if="!item.isAvailable" class="unavailable-mask">
                  <span>暂时缺货</span>
                </div>
              </div>

              <!-- 商品信息 -->
              <div class="item-details">
                <h4 class="item-name">{{ item.dishName }}</h4>
                <div class="item-price">
                  <span class="current-price">¥{{ item.price.toFixed(2) }}</span>
                </div>
                <div class="item-meta">
                  <span class="added-time">{{ formatTime(item.addedAt) }}加入</span>
                </div>
              </div>

              <!-- 数量控制 -->
              <div class="quantity-controls">
                <button 
                  @click="decreaseQuantity(item)"
                  :disabled="item.quantity <= 1 || !item.isAvailable"
                  class="quantity-btn minus"
                >
                  −
                </button>
                <span class="quantity">{{ Number(item.quantity) }}</span>
                <button 
                  @click="increaseQuantity(item)"
                  :disabled="!item.isAvailable"
                  class="quantity-btn plus"
                >
                  +
                </button>
              </div>

              <!-- 小计 -->
              <div class="item-subtotal">
                <span class="subtotal-amount">¥{{ (Number(item.price) * Number(item.quantity)).toFixed(2) }}</span>
              </div>

              <!-- 删除按钮 -->
              <button 
                @click="removeItem(item)"
                class="delete-item-btn"
                title="删除"
              >
                ✕
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 底部结算栏 -->
    <div v-if="cartItems.length > 0" class="cart-footer">
      <div class="summary-info">
        <div class="summary-line">
          <span>已选商品 ({{ selectedItems.length }}件)</span>
          <span class="amount">¥{{ selectedTotalAmount.toFixed(2) }}</span>
        </div>
        <div v-if="estimatedDeliveryFee > 0" class="summary-line delivery-fee">
          <span>预估配送费</span>
          <span class="amount">¥{{ estimatedDeliveryFee.toFixed(2) }}</span>
        </div>
      </div>
      
      <div class="checkout-section">
        <div class="total-amount">
          <span class="total-label">合计</span>
          <span class="total-price">¥{{ finalAmount.toFixed(2) }}</span>
        </div>
        <button 
          @click="proceedToCheckout"
          :disabled="selectedItems.length === 0"
          class="checkout-btn"
        >
          去结算 ({{ selectedItems.reduce((total, item) => total + Number(item.quantity), 0) }})
        </button>
      </div>
    </div>

    <!-- 删除确认弹窗 -->
    <div v-if="showDeleteModal" class="delete-modal" @click="closeDeleteModal">
      <div class="modal-content" @click.stop>
        <h3 class="modal-title">确认删除</h3>
        <p class="modal-message">
          {{ deleteModalMessage }}
        </p>
        <div class="modal-actions">
          <button @click="closeDeleteModal" class="cancel-btn">取消</button>
          <button @click="confirmDelete" class="confirm-btn">确认删除</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, reactive, onMounted, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import { getCartInstance } from '@/composables/useCart.js'

export default {
  name: 'CartPage',
  setup() {
    const router = useRouter()
    
    // 使用购物车组合式函数
    const cart = getCartInstance()
    
    // 响应式数据
    const loading = ref(false)
    const cartItems = ref([])
    const showDeleteModal = ref(false)
    const deleteModalMessage = ref('')
    const pendingDeleteAction = ref(null)

    // 获取当前用户ID（从localStorage获取测试用户ID）
    const currentUserId = ref((typeof localStorage !== 'undefined' && localStorage.getItem && localStorage.getItem('userId'))
      ? localStorage.getItem('userId')
      : '')

    // 计算属性
    const totalItems = computed(() => {
      return cartItems.value.reduce((total, item) => total + Number(item.quantity), 0)
    })

    const selectedItems = computed(() => {
      return cartItems.value.filter(item => item.selected && item.isAvailable)
    })

    const selectedTotalAmount = computed(() => {
      return selectedItems.value.reduce((total, item) => total + item.subtotal, 0)
    })

    const estimatedDeliveryFee = computed(() => {
      // 如果有选中商品，计算配送费
      if (selectedItems.value.length > 0) {
        // 按商家分组计算配送费
        const merchantGroups = groupSelectedItemsByMerchant()
        return merchantGroups.length * 5 // 每个商家5元配送费
      }
      return 0
    })

    const finalAmount = computed(() => {
      return selectedTotalAmount.value + estimatedDeliveryFee.value
    })

    const isAllSelected = computed({
      get() {
        const availableItems = cartItems.value.filter(item => item.isAvailable)
        return availableItems.length > 0 && availableItems.every(item => item.selected)
      },
      set(value) {
        cartItems.value.forEach(item => {
          if (item.isAvailable) {
            item.selected = value
          }
        })
      }
    })

    const groupedCartItems = computed(() => {
      const groups = {}
      
      cartItems.value.forEach(item => {
        if (!groups[item.merchantId]) {
          groups[item.merchantId] = {
            merchantId: item.merchantId,
            merchantName: item.merchantName,
            items: [],
            selected: false
          }
        }
        groups[item.merchantId].items.push(item)
      })

      // 计算每个商家组的选中状态
      Object.values(groups).forEach(group => {
        const availableItems = group.items.filter(item => item.isAvailable)
        group.selected = availableItems.length > 0 && availableItems.every(item => item.selected)
      })

      return Object.values(groups)
    })

    // 方法
    // 使用 useCart.js 中的方法获取购物车数据
    const fetchCartData = async () => {
      loading.value = true
      console.log('当前用户ID:', currentUserId.value)
      try {
        // 使用 cart 实例的 fetchCartData 方法
        await cart.fetchCartData(currentUserId.value)
        
        // 从 cart 实例获取购物车数据
        cartItems.value = cart.cartItems.value
        console.log('购物车数据:', cartItems.value)
      } catch (error) {
        console.error('获取购物车数据失败:', error)
        cartItems.value = []
      } finally {
        loading.value = false
      }
    }

    const toggleSelectAll = () => {
      // 计算属性会自动处理选中状态
    }

    const toggleMerchantSelect = (merchantGroup) => {
      merchantGroup.items.forEach(item => {
        if (item.isAvailable) {
          item.selected = merchantGroup.selected
        }
      })
    }

    const updateItemSelection = () => {
      // 选中状态变化会自动更新计算属性
    }

    // 增加数量
    const increaseQuantity = async (item) => {
      if (!item.isAvailable) return
      
      try {
        console.log(`增加商品数量: cartId=${item.cartItemId}, 新数量=${item.quantity + 1}`)
        await cart.updateQuantity(currentUserId.value, item.cartItemId, item.quantity + 1)
        // 更新本地购物车数据
        await fetchCartData()
      } catch (error) {
        console.error('增加商品数量失败:', error)
      }
    }

    // 减少数量
    const decreaseQuantity = async (item) => {
      if (!item.isAvailable) return
      
      try {
        // 如果数量为1，则直接删除
        if (item.quantity <= 1) {
          console.log(`商品数量为1，直接删除: cartId=${item.cartItemId}`)
          await cart.removeFromCart(currentUserId.value, item.cartItemId)
          return
        }
        
        // 否则减少数量
        console.log(`减少商品数量: cartId=${item.cartItemId}, 新数量=${item.quantity - 1}`)
        await cart.updateQuantity(currentUserId.value, item.cartItemId, item.quantity - 1)
        // 更新本地购物车数据
        await fetchCartData()
      } catch (error) {
        console.error('减少商品数量失败:', error)
      }
    }

    const removeItem = (item) => {
      deleteModalMessage.value = `确定要删除"${item.dishName}"吗？`
      pendingDeleteAction.value = () => cart.removeFromCart(currentUserId.value, item.cartItemId).then(() => fetchCartData())
      showDeleteModal.value = true
    }

    const deleteSelected = () => {
      if (selectedItems.value.length === 0) return
      
      deleteModalMessage.value = `确定要删除所选的 ${selectedItems.value.length} 件商品吗？`
      pendingDeleteAction.value = () => deleteMultipleItems()
      showDeleteModal.value = true
    }

    // 批量删除
    const deleteMultipleItems = async () => {
      try {
        const itemsToDelete = selectedItems.value
        for (const item of itemsToDelete) {
          await cart.removeFromCart(currentUserId.value, item.cartItemId)
        }
        await fetchCartData()
      } catch (error) {
        console.error('批量删除商品失败:', error)
      }
    }

    const confirmDelete = async () => {
      if (pendingDeleteAction.value) {
        await pendingDeleteAction.value()
        closeDeleteModal()
      }
    }

    const closeDeleteModal = () => {
      showDeleteModal.value = false
      deleteModalMessage.value = ''
      pendingDeleteAction.value = null
    }

    const proceedToCheckout = () => {
      if (selectedItems.value.length === 0) return

      // 准备结算数据
      const checkoutData = {
        items: selectedItems.value.map(item => ({
          dishId: item.dishId,
          dishName: item.dishName,
          price: item.price,
          quantity: item.quantity,
          subtotal: item.subtotal,
          coverUrl: item.image || item.coverUrl || '',
          merchantId: item.merchantId,
          merchantName: item.merchantName
        })),
        totalAmount: selectedTotalAmount.value,
        deliveryFee: estimatedDeliveryFee.value,
        finalAmount: finalAmount.value
      }

      // 跳转到结算页面
      router.push({
        name: 'Checkout',
        query: {
          data: JSON.stringify(checkoutData)
        }
      })
    }

    const groupSelectedItemsByMerchant = () => {
      const groups = {}
      
      selectedItems.value.forEach(item => {
        if (!groups[item.merchantId]) {
          groups[item.merchantId] = {
            merchantId: item.merchantId,
            merchantName: item.merchantName,
            items: []
          }
        }
        groups[item.merchantId].items.push(item)
      })

      return Object.values(groups)
    }

    const formatTime = (timeString) => {
      const date = new Date(timeString)
      const now = new Date()
      const diffMs = now - date
      const diffMins = Math.floor(diffMs / (1000 * 60))
      const diffHours = Math.floor(diffMs / (1000 * 60 * 60))
      const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24))

      if (diffMins < 60) {
        return `${diffMins}分钟前`
      } else if (diffHours < 24) {
        return `${diffHours}小时前`
      } else if (diffDays < 7) {
        return `${diffDays}天前`
      } else {
        return date.toLocaleDateString()
      }
    }

    const goBack = () => {
      router.back()
    }

    const goToMerchants = () => {
      router.push('/browse')
    }

    const handleImageError = (event) => {
      event.target.src = '/assets/placeholder.png'
    }

    // 生命周期
    onMounted(() => {
      console.log('购物车组件已挂载，开始获取数据')
      fetchCartData()
    })

    return {
      loading,
      cartItems,
      showDeleteModal,
      deleteModalMessage,
      totalItems,
      selectedItems,
      selectedTotalAmount,
      estimatedDeliveryFee,
      finalAmount,
      isAllSelected,
      groupedCartItems,
      toggleSelectAll,
      toggleMerchantSelect,
      updateItemSelection,
      increaseQuantity,
      decreaseQuantity,
      removeItem,
      deleteSelected,
      confirmDelete,
      closeDeleteModal,
      proceedToCheckout,
      formatTime,
      goBack,
      goToMerchants,
      handleImageError
    }
  }
}
</script>

<style scoped>
.cart-page {
  min-height: 100vh;
  background: #f8f9fa;
  display: flex;
  flex-direction: column;
}

/* 页面头部 */
.cart-header {
  background: white;
  border-bottom: 1px solid #e9ecef;
  position: sticky;
  top: 0;
  z-index: 100;
}

.header-content {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 16px 20px;
  max-width: 1200px;
  margin: 0 auto;
}

.page-title {
  font-size: 20px;
  font-weight: 600;
  color: #333;
  margin: 0;
  flex: 1;
}

.cart-count {
  font-size: 14px;
  color: #666;
}

/* 购物车内容 */
.cart-content {
  flex: 1;
  max-width: 1200px;
  margin: 0 auto;
  width: 100%;
  padding: 20px;
}

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

/* 空购物车 */
.empty-cart {
  text-align: center;
  padding: 80px 20px;
  background: white;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.empty-icon {
  font-size: 64px;
  margin-bottom: 24px;
}

.empty-title {
  font-size: 24px;
  font-weight: 600;
  color: #333;
  margin: 0 0 12px 0;
}

.empty-desc {
  font-size: 16px;
  color: #666;
  margin: 0 0 32px 0;
}

.browse-btn {
  padding: 12px 32px;
  background: linear-gradient(135deg, #007BFF, #00D4FF);
  color: white;
  border: none;
  border-radius: 25px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.browse-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 123, 255, 0.3);
}

/* 全选操作 */
.select-all-section {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 20px;
  background: white;
  border-radius: 12px;
  margin-bottom: 16px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.select-all-checkbox,
.merchant-checkbox,
.item-checkbox {
  display: flex;
  align-items: center;
  cursor: pointer;
  user-select: none;
}

.select-all-checkbox input,
.merchant-checkbox input,
.item-checkbox input {
  display: none;
}

.checkmark {
  width: 20px;
  height: 20px;
  border: 2px solid #ddd;
  border-radius: 4px;
  margin-right: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;
}

.select-all-checkbox input:checked + .checkmark,
.merchant-checkbox input:checked + .checkmark,
.item-checkbox input:checked + .checkmark {
  background: #007BFF;
  border-color: #007BFF;
}

.select-all-checkbox input:checked + .checkmark::after,
.merchant-checkbox input:checked + .checkmark::after,
.item-checkbox input:checked + .checkmark::after {
  content: '✓';
  color: white;
  font-size: 12px;
  font-weight: bold;
}

.select-text {
  font-size: 16px;
  color: #333;
}

.delete-selected-btn {
  padding: 8px 16px;
  background: #dc3545;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.delete-selected-btn:disabled {
  background: #e9ecef;
  color: #6c757d;
  cursor: not-allowed;
}

.delete-selected-btn:not(:disabled):hover {
  background: #c82333;
}

/* 商家分组 */
.merchant-group {
  background: white;
  border-radius: 12px;
  margin-bottom: 16px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.merchant-header {
  display: flex;
  align-items: center;
  padding: 16px 20px;
  background: #f8f9fa;
  border-bottom: 1px solid #e9ecef;
}

.merchant-info {
  flex: 1;
  margin-left: 8px;
}

.merchant-name {
  font-size: 16px;
  font-weight: 600;
  color: #333;
  margin: 0 0 4px 0;
}

.merchant-status {
  font-size: 14px;
  color: #666;
}

/* 购物车商品项 */
.cart-item {
  display: flex;
  align-items: center;
  padding: 16px 20px;
  border-bottom: 1px solid #f0f0f0;
  transition: all 0.3s ease;
}

.cart-item:last-child {
  border-bottom: none;
}

.cart-item.unavailable {
  opacity: 0.6;
  background: #f8f9fa;
}

.item-checkbox {
  margin-right: 12px;
}

.item-image-container {
  position: relative;
  width: 80px;
  height: 80px;
  margin-right: 12px;
  border-radius: 8px;
  overflow: hidden;
}

.item-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.unavailable-mask {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 12px;
}

.item-details {
  flex: 1;
  margin-right: 12px;
}

.item-name {
  font-size: 16px;
  font-weight: 600;
  color: #333;
  margin: 0 0 8px 0;
  line-height: 1.3;
}

.item-price {
  margin-bottom: 4px;
}

.current-price {
  font-size: 16px;
  font-weight: 600;
  color: #e74c3c;
}

.item-meta {
  font-size: 12px;
  color: #999;
}

.quantity-controls {
  display: flex;
  align-items: center;
  margin-right: 16px;
}

.quantity-btn {
  width: 32px;
  height: 32px;
  border: 1px solid #ddd;
  background: white;
  color: #333;
  border-radius: 6px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  font-size: 16px;
  font-weight: 600;
  transition: all 0.3s ease;
}

.quantity-btn:disabled {
  background: #f8f9fa;
  color: #999;
  cursor: not-allowed;
}

.quantity-btn:not(:disabled):hover {
  background: #007BFF;
  color: white;
  border-color: #007BFF;
}

.quantity {
  margin: 0 12px;
  font-size: 16px;
  font-weight: 600;
  min-width: 24px;
  text-align: center;
  color: #333;
}

.item-subtotal {
  margin-right: 16px;
  min-width: 80px;
  text-align: right;
}

.subtotal-amount {
  font-size: 16px;
  font-weight: 600;
  color: #333;
}

.delete-item-btn {
  width: 32px;
  height: 32px;
  background: none;
  border: none;
  color: #999;
  cursor: pointer;
  font-size: 16px;
  transition: all 0.3s ease;
  border-radius: 6px;
}

.delete-item-btn:hover {
  background: #fee;
  color: #e74c3c;
}

/* 底部结算栏 */
.cart-footer {
  background: white;
  border-top: 1px solid #e9ecef;
  padding: 16px 20px;
  position: sticky;
  bottom: 0;
  z-index: 100;
}

.cart-footer .summary-info {
  max-width: 1200px;
  margin: 0 auto 12px;
}

.summary-line {
  display: flex;
  justify-content: space-between;
  margin-bottom: 8px;
  font-size: 14px;
}

.summary-line:last-child {
  margin-bottom: 0;
}

.summary-line span:first-child {
  color: #666;
}

.summary-line .amount {
  color: #333;
  font-weight: 500;
}

.summary-line.delivery-fee .amount {
  color: #f39c12;
}

.checkout-section {
  display: flex;
  justify-content: space-between;
  align-items: center;
  max-width: 1200px;
  margin: 0 auto;
}

.total-amount {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
}

.total-label {
  font-size: 14px;
  color: #666;
  margin-bottom: 4px;
}

.total-price {
  font-size: 20px;
  font-weight: 700;
  color: #e74c3c;
}

.checkout-btn {
  padding: 12px 32px;
  background: linear-gradient(135deg, #007BFF, #00D4FF);
  color: white;
  border: none;
  border-radius: 25px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.checkout-btn:disabled {
  background: #e9ecef;
  color: #6c757d;
  cursor: not-allowed;
}

.checkout-btn:not(:disabled):hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 123, 255, 0.3);
}

/* 删除确认弹窗 */
.delete-modal {
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
}

.cancel-btn:hover {
  background: #e9ecef;
}

.confirm-btn {
  background: #dc3545;
  color: white;
}

.confirm-btn:hover {
  background: #c82333;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .cart-content {
    padding: 12px;
  }

  .cart-item {
    padding: 12px 16px;
    flex-wrap: wrap;
  }

  .item-details {
    margin-right: 0;
    margin-bottom: 12px;
    flex-basis: 100%;
  }

  .quantity-controls {
    margin-right: 0;
    margin-bottom: 8px;
  }

  .item-subtotal {
    margin-right: 0;
    text-align: left;
  }

  .checkout-section {
    flex-direction: column;
    gap: 16px;
    align-items: stretch;
  }

  .checkout-btn {
    width: 100%;
  }

  .merchant-header,
  .cart-item,
  .select-all-section {
    padding: 12px 16px;
  }
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}
</style>
