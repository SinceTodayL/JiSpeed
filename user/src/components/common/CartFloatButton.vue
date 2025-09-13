<template>
  <div 
    v-if="cartItemCount > 0" 
    class="cart-float-button"
    @click="goToCart"
  >
    <div class="cart-icon">
      <span class="icon">🛒</span>
      <div class="cart-badge">{{ cartItemCount }}</div>
    </div>
    <div class="cart-info">
      <div class="cart-count">{{ cartItemCount }}件</div>
      <div class="cart-total">¥{{ cartTotalAmount.toFixed(2) }}</div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { cartAPI } from '@/api/cart.js'

export default {
  name: 'CartFloatButton',
  setup() {
    const router = useRouter()
    const cartData = ref(null)
    const updateTimer = ref(null)

    // 模拟用户ID
    const currentUserId = ref('USER001')

    // 计算属性
    const cartItemCount = computed(() => {
      if (!cartData.value || !cartData.value.items) return 0
      return cartData.value.items.reduce((total, item) => total + item.quantity, 0)
    })

    const cartTotalAmount = computed(() => {
      if (!cartData.value || !cartData.value.items) return 0
      return cartData.value.items
        .filter(item => item.isAvailable)
        .reduce((total, item) => total + item.subtotal, 0)
    })

    // 方法
    const fetchCartData = async () => {
      try {
        const response = await cartAPI.getUserCart(currentUserId.value)
        if (response && response.data) {
          cartData.value = response.data
        }
      } catch (error) {
        console.error('获取购物车数据失败:', error)
      }
    }

    const goToCart = () => {
      router.push('/cart')
    }

    const startAutoUpdate = () => {
      // 每30秒更新一次购物车数据
      updateTimer.value = setInterval(() => {
        fetchCartData()
      }, 30000)
    }

    const stopAutoUpdate = () => {
      if (updateTimer.value) {
        clearInterval(updateTimer.value)
        updateTimer.value = null
      }
    }

    // 生命周期
    onMounted(() => {
      fetchCartData()
      startAutoUpdate()
    })

    onUnmounted(() => {
      stopAutoUpdate()
    })

    return {
      cartItemCount,
      cartTotalAmount,
      goToCart
    }
  }
}
</script>

<style scoped>
.cart-float-button {
  position: fixed;
  bottom: 80px;
  right: 20px;
  background: linear-gradient(135deg, #007BFF 0%, #00D4FF 50%, #40E0D0 100%);
  border-radius: 25px;
  padding: 12px 20px;
  display: flex;
  align-items: center;
  gap: 12px;
  cursor: pointer;
  box-shadow: 0 4px 20px rgba(0, 123, 255, 0.3);
  z-index: 1000;
  transition: all 0.3s ease;
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.cart-float-button:hover {
  transform: translateY(-3px);
  box-shadow: 0 8px 30px rgba(0, 123, 255, 0.4);
}

.cart-icon {
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
}

.icon {
  font-size: 20px;
  color: white;
}

.cart-badge {
  position: absolute;
  top: -8px;
  right: -8px;
  background: #e74c3c;
  color: white;
  border-radius: 50%;
  min-width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  font-weight: 600;
  border: 2px solid white;
}

.cart-info {
  color: white;
  line-height: 1.2;
}

.cart-count {
  font-size: 12px;
  font-weight: 500;
  opacity: 0.9;
}

.cart-total {
  font-size: 14px;
  font-weight: 700;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .cart-float-button {
    bottom: 20px;
    right: 20px;
    padding: 10px 16px;
  }

  .icon {
    font-size: 18px;
  }

  .cart-info {
    font-size: 12px;
  }

  .cart-total {
    font-size: 13px;
  }
}
</style>
