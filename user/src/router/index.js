// src/router/index.js
import { createRouter, createWebHistory } from 'vue-router'
import { authState } from '@/utils/auth.js';
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
      title: '首页 - JiSpeed 济时达外卖'
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
      title: '我的 - JiSpeed 济时达外卖',
      requiresAuth: true
    }
  },
  {
    path: '/browse',
    component: MerchantsBrowse,
    meta: {
      title: '商家浏览 - JiSpeed 济时达外卖'
    }
  },
  {
    path: '/merchant/:id',
    component: MerchantDetail,
    meta: {
      title: '商家详情 - JiSpeed 济时达外卖'
    }
  },
  {
    path: '/checkout',
    name: 'Checkout',
    component: Checkout,
    meta: {
      title: '确认订单 - JiSpeed 济时达外卖'
    }
  },
  {
    path: '/payment/:orderId',
    name: 'Payment',
    component: Payment,
    meta: {
      title: '订单支付 - JiSpeed 济时达外卖',
      requiresAuth: true
    }
  },
  {
    path: '/orders',
    name: 'Orders',
    component: Orders,
    meta: {
      title: '我的订单 - JiSpeed 济时达外卖',
      requiresAuth: true
    }
  },
  {
    path: '/orders/:orderId',
    name: 'OrderDetail',
    component: OrderDetail,
    meta: {
      title: '订单详情 - JiSpeed 济时达外卖',
      requiresAuth: true
    }
  },
  {
    path: '/addresses',
    name: 'Addresses',
    component: Addresses,
    meta: {
      title: '地址管理 - JiSpeed 济时达外卖',
      requiresAuth: true
    }
  },
  {
    path: '/cart',
    name: 'Cart',
    component: Cart,
    meta: {
      title: '购物车 - JiSpeed 济时达外卖',
      requiresAuth: true
    }
  },
  {
    path: '/coupons',
    name: 'Coupons',
    component: Coupons,
    meta: {
      title: '我的优惠券 - JiSpeed 济时达外卖',
      requiresAuth: true
    }
  },
  {
    path: '/favorites',
    name: 'Favorites',
    component: Favorites,
    meta: {
      title: '我的收藏 - JiSpeed 济时达外卖',
      requiresAuth: true
    }
  },
  {
    path: '/settings',
    name: 'Settings',
    component: Settings,
    meta: {
      title: '设置 - JiSpeed 济时达外卖',
      requiresAuth: true
    }
  },
  {
    path: '/complaints',
    name: 'Complaints',
    component: Complaints,
    meta: {
      title: '投诉与建议 - JiSpeed 济时达外卖',
      requiresAuth: true
    }
  },
  {
    path: '/reviews',
    name: 'UserReviews',
    component: UserReviews,
    meta: {
      title: '我的评价 - JiSpeed 济时达外卖',
      requiresAuth: true
    }
  },
  {
    path: '/user-complaints',
    name: 'UserComplaints',
    component: UserComplaints,
    meta: {
      title: '我的投诉 - JiSpeed 济时达外卖',
      requiresAuth: true
    }
  },
  {
    path: '/profile/edit',
    name: 'UserInfoEdit',
    component: UserInfoEdit,
    meta: {
      title: '编辑个人信息 - JiSpeed 济时达外卖',
      requiresAuth: true
    }
  }
  // add more routes
]

const router = createRouter({
  history: createWebHistory('/user/'),
  routes
})

// 简化的路由守卫 - 只设置页面标题，不做认证检查
router.beforeEach(async (to, from, next) => {
  // 等待认证状态就绪
  if (!authState.isReady) {
    await new Promise(resolve => {
      const interval = setInterval(() => {
        if (authState.isReady) {
          clearInterval(interval);
          resolve();
        }
      }, 50); // 每50ms检查一次
    });
  }

  // 设置页面标题
  if (to.meta.title) {
    document.title = to.meta.title
  }
  
  console.log('🚀 路由跳转:', from.path, '→', to.path)
  next() // 直接放行所有路由
})

export default router
