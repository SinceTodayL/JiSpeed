<template>
  <div class="bottom-navigation">
    <div 
      v-for="tab in tabs" 
      :key="tab.name"
      @click="navigateTo(tab)"
      :class="['nav-item', { active: isActive(tab.path) }]"
    >
      <div class="nav-icon">
        <span class="icon">{{ tab.icon }}</span>
        <div v-if="tab.badge && tab.badge > 0" class="nav-badge">{{ tab.badge }}</div>
      </div>
      <span class="nav-label">{{ tab.label }}</span>
    </div>
  </div>
</template>

<script>
import { computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useCart } from '@/composables/useCart.js'

export default {
  name: 'BottomNavigation',
  setup() {
    const router = useRouter()
    const route = useRoute()
    const { totalItems } = useCart()
    
    // 导航标签配置
    const tabs = computed(() => [
      {
        name: 'home',
        path: '/',
        icon: '🏠',
        label: '首页',
        badge: 0
      },
      {
        name: 'cart',
        path: '/cart',
        icon: '🛒',
        label: '购物车',
        badge: totalItems.value
      },
      {
        name: 'profile',
        path: '/profile',
        icon: '👤',
        label: '我的',
        badge: 0
      }
    ])
    
    // 判断当前路由是否激活
    const isActive = (path) => {
      if (path === '/') {
        return route.path === '/' || route.path === '/home'
      }
      return route.path.startsWith(path)
    }
    
    // 导航到指定页面
    const navigateTo = (tab) => {
      if (!isActive(tab.path)) {
        router.push(tab.path)
      }
    }
    
    return {
      tabs,
      isActive,
      navigateTo
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
  background: white;
  border-top: 1px solid #e8e8e8;
  display: flex;
  height: 60px;
  z-index: 1000;
  box-shadow: 0 -2px 10px rgba(0,0,0,0.1);
}

.nav-item {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.3s ease;
  position: relative;
  padding: 8px 4px;
}

.nav-item:hover {
  background: #f8f9ff;
}

.nav-item.active {
  color: #4facfe;
}

.nav-item.active .nav-icon .icon {
  transform: scale(1.1);
}

.nav-icon {
  position: relative;
  margin-bottom: 2px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.icon {
  font-size: 20px;
  transition: transform 0.3s ease;
}

.nav-badge {
  position: absolute;
  top: -8px;
  right: -8px;
  background: #ff4d4f;
  color: white;
  font-size: 10px;
  font-weight: 600;
  padding: 2px 5px;
  border-radius: 10px;
  min-width: 16px;
  height: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 1px 3px rgba(0,0,0,0.3);
}

.nav-label {
  font-size: 12px;
  font-weight: 500;
  line-height: 1;
}

/* 适配iPhone X等有底部安全区域的设备 */
@supports (bottom: env(safe-area-inset-bottom)) {
  .bottom-navigation {
    padding-bottom: env(safe-area-inset-bottom);
    height: calc(60px + env(safe-area-inset-bottom));
  }
}
</style>
