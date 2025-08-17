// src/router/index.js
import { createRouter, createWebHistory } from 'vue-router'
import Home from '@/views/Home.vue'  // 主页 - 商家浏览页面
import Profile from '@/views/Profile.vue'  // 新增个人中心页面
import Login from '@/views/Login.vue'
import Register from '@/views/Register.vue'
import MerchantsBrowse from '@/views/MerchantsBrowse.vue'
import MerchantDetail from '@/views/MerchantDetail.vue'
import Checkout from '@/views/Checkout.vue'
import Payment from '@/views/Payment.vue'
import Orders from '@/views/Orders.vue'
import OrderDetail from '@/views/OrderDetail.vue'
import Coupons from '@/views/Coupons.vue'
import Cart from '@/views/Cart.vue'
import Addresses from '@/views/Addresses.vue'
import Favorites from '@/views/Favorites.vue'
import Settings from '@/views/Settings.vue'
import Complaints from '@/views/Complaints.vue'

const routes = [
  {
    path: '/',
    component: Home,
    meta: {
      title: '首页 - 急速外卖'
    }
  },
  {
    path: '/home',
    redirect: '/'
  },
  {
    path: '/profile',
    component: Profile,
    meta: {
      title: '我的 - 急速外卖',
      requiresAuth: true
    }
  },
  {
    path: '/login',
    component: Login,
    meta: {
      title: '用户登录 - 急速'
    }
  },
  {
    path: '/register',
    component: Register,
    meta: {
      title: '用户注册 - 急速'
    }
  },
  {
    path: '/browse',
    component: MerchantsBrowse,
    meta: {
      title: '商家浏览 - 急速外卖'
    }
  },
  {
    path: '/merchant/:id',
    component: MerchantDetail,
    meta: {
      title: '商家详情 - 急速外卖'
    }
  },
  {
    path: '/checkout',
    name: 'Checkout',
    component: Checkout,
    meta: {
      title: '确认订单 - 急速外卖'
    }
  },
  {
    path: '/payment/:orderId',
    name: 'Payment',
    component: Payment,
    meta: {
      title: '订单支付 - 急速外卖',
      requiresAuth: true
    }
  },
  {
    path: '/orders',
    name: 'Orders',
    component: Orders,
    meta: {
      title: '我的订单 - 急速外卖',
      requiresAuth: true
    }
  },
  {
    path: '/orders/:orderId',
    name: 'OrderDetail',
    component: OrderDetail,
    meta: {
      title: '订单详情 - 急速外卖',
      requiresAuth: true
    }
  },
  {
    path: '/addresses',
    name: 'Addresses',
    component: Addresses,
    meta: {
      title: '地址管理 - 急速外卖',
      requiresAuth: true
    }
  },
  {
    path: '/cart',
    name: 'Cart',
    component: Cart,
    meta: {
      title: '购物车 - 急速外卖',
      requiresAuth: true
    }
  },
  {
    path: '/coupons',
    name: 'Coupons',
    component: Coupons,
    meta: {
      title: '我的优惠券 - 急速外卖',
      requiresAuth: true
    }
  },
  {
    path: '/favorites',
    name: 'Favorites',
    component: Favorites,
    meta: {
      title: '我的收藏 - 急速外卖',
      requiresAuth: true
    }
  },
  {
    path: '/settings',
    name: 'Settings',
    component: Settings,
    meta: {
      title: '设置 - 急速外卖',
      requiresAuth: true
    }
  },
  {
    path: '/complaints',
    name: 'Complaints',
    component: Complaints,
    meta: {
      title: '投诉与建议 - 急速外卖',
      requiresAuth: true
    }
  }
  // add more routes
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// 路由守卫 - 检查认证状态
router.beforeEach((to, from, next) => {
  // 设置页面标题
  if (to.meta.title) {
    document.title = to.meta.title
  }
  
  // 检查是否需要认证
  if (to.meta.requiresAuth) {
    const token = localStorage.getItem('token')
    if (!token) {
      // 未登录，跳转到登录页
      next({
        path: '/login',
        query: { redirect: to.fullPath }
      })
      return
    }
  }
  
  next()
})

export default router
