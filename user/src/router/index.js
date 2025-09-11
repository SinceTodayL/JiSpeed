// src/router/index.js
import { createRouter, createWebHistory } from 'vue-router'
import HomeNew from '@/views/HomeNew.vue'  // 新主页 - 商家浏览页面
import Profile from '@/views/Profile.vue'  // 新增个人中心页面
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
import UserReviews from '@/views/UserReviews.vue'
import UserComplaints from '@/views/UserComplaints.vue'
import UserInfoEdit from '@/views/UserInfoEdit.vue'

const routes = [
  {
    path: '/',
  component: HomeNew,
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
  },
  {
    path: '/reviews',
    name: 'UserReviews',
    component: UserReviews,
    meta: {
      title: '我的评价 - 急速外卖',
      requiresAuth: true
    }
  },
  {
    path: '/user-complaints',
    name: 'UserComplaints',
    component: UserComplaints,
    meta: {
      title: '我的投诉 - 急速外卖',
      requiresAuth: true
    }
  }
  // add more routes
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// 路由守卫 - 检查认证状态和用户类型
router.beforeEach((to, from, next) => {
  // 设置页面标题
  if (to.meta.title) {
    document.title = to.meta.title
  }
  
  // 检查是否需要认证
  if (to.meta.requiresAuth) {
    const token = localStorage.getItem('token')
    const userType = localStorage.getItem('userType')
    
    if (!token) {
      // 未登录，跳转到统一登录页面
      console.log('用户未登录，跳转到统一登录页面')
      window.location.href = 'http://localhost:9527/login'
      return
    }
    
    // 检查用户类型，确保只有用户(userType=1)可以访问用户端
    if (userType !== '1') {
      console.log('用户类型不匹配，当前用户类型:', userType, '要求用户类型: 1')
      alert('访问权限不足，您不是普通用户')
      // 跳转到对应的前端页面
      if (userType === '2') {
        window.location.href = 'http://localhost:9520' // 商家端
      } else if (userType === '3') {
        // 骑手端URL（如果有的话）
        alert('骑手端页面尚未配置')
      } else if (userType === '4') {
        // 管理员端URL（如果有的话）
        alert('管理员端页面尚未配置')
      } else {
        window.location.href = 'http://localhost:9527/login'
      }
      return
    }
  }
  
  next()
})

export default router
