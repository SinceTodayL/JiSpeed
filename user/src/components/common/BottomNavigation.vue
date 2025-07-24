<template>
  <div class="bottom-navigation">
    <div class="nav-item" @click="goHome" :class="{ active: currentRoute === '/' }">
      <div class="nav-icon">🏠</div>
      <span class="nav-text">首页</span>
    </div>
    
    <div class="nav-item" @click="goBrowse" :class="{ active: currentRoute === '/browse' }">
      <div class="nav-icon">🍽️</div>
      <span class="nav-text">美食</span>
    </div>
    
    <div class="nav-item" @click="goCart" :class="{ active: currentRoute === '/cart' }">
      <div class="nav-icon">
        <span class="icon">🛒</span>
        <div v-if="cartItemCount > 0" class="cart-badge">{{ cartItemCount }}</div>
      </div>
      <span class="nav-text">购物车</span>
    </div>
    
    <div class="nav-item" @click="goOrders" :class="{ active: currentRoute === '/orders' }">
      <div class="nav-icon">📋</div>
      <span class="nav-text">订单</span>
    </div>
    
    <div class="nav-item" @click="goProfile" :class="{ active: currentRoute === '/profile' }">
      <div class="nav-icon">👤</div>
      <span class="nav-text">我的</span>
    </div>
  </div>
</template>

<script>
import { computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { getCartInstance } from '@/composables/useCart.js'

export default {
  name: 'BottomNavigation',
  setup() {
    const router = useRouter()
    const route = useRoute()
    const cart = getCartInstance()

    const currentRoute = computed(() => route.path)
    const cartItemCount = computed(() => cart.totalItems.value)

    const goHome = () => {
      router.push('/')
    }

    const goBrowse = () => {
      router.push('/browse')
    }

    const goCart = () => {
      router.push('/cart')
    }

    const goOrders = () => {
      router.push('/orders')
    }

    const goProfile = () => {
      // 暂时跳转到地址管理页面
      router.push('/addresses')
    }

    return {
      currentRoute,
      cartItemCount,
      goHome,
      goBrowse,
      goCart,
      goOrders,
      goProfile
    }
  }
}
</script>

<style scoped>
.bottom-navigation {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  height: 60px;
  background: white;
  border-top: 1px solid #e9ecef;
  display: flex;
  justify-content: space-around;
  align-items: center;
  z-index: 1000;
  padding: 0 10px;
  box-shadow: 0 -2px 8px rgba(0, 0, 0, 0.1);
}

.nav-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.3s ease;
  padding: 8px 12px;
  border-radius: 8px;
  flex: 1;
  max-width: 80px;
}

.nav-item:hover {
  background: #f8f9fa;
}

.nav-item.active {
  color: #007BFF;
}

.nav-item.active .nav-icon {
  transform: scale(1.1);
}

.nav-icon {
  position: relative;
  font-size: 20px;
  margin-bottom: 4px;
  transition: all 0.3s ease;
}

.nav-text {
  font-size: 12px;
  line-height: 1;
}

.cart-badge {
  position: absolute;
  top: -6px;
  right: -6px;
  background: #e74c3c;
  color: white;
  border-radius: 50%;
  min-width: 16px;
  height: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 10px;
  font-weight: 600;
  border: 2px solid white;
}

/* 为页面内容留出底部导航的空间 */
body {
  padding-bottom: 60px;
}
</style>
