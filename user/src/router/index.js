// src/router/index.js
import { createRouter, createWebHistory } from 'vue-router'
import home from '@/views/home.vue'
import Login from '@/views/Login.vue'
import Register from '@/views/Register.vue'
import MerchantsBrowse from '@/views/MerchantsBrowse.vue'
import MerchantDetail from '@/views/MerchantDetail.vue'
import Checkout from '@/views/Checkout.vue'
import Payment from '@/views/Payment.vue'
import Orders from '@/views/Orders.vue'
import OrderDetail from '@/views/OrderDetail.vue'
import Coupons from '@/views/Coupons.vue'
import AddressManagement from '@/views/AddressManagement.vue'
import Cart from '@/views/Cart.vue'

const routes = [
  {
    path: '/',
    component: home,
    meta: {
      title: '首页'
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
    path: '/coupons',
    name: 'Coupons',
    component: Coupons,
    meta: {
      title: '我的优惠券 - 急速外卖'
    }
  },
  {
    path: '/payment/:orderId',
    name: 'Payment',
    component: Payment,
    meta: {
      title: '订单支付 - 急速外卖'
    }
  },
  {
    path: '/orders',
    name: 'Orders',
    component: Orders,
    meta: {
      title: '我的订单 - 急速外卖'
    }
  },
  {
    path: '/orders/:orderId',
    name: 'OrderDetail',
    component: OrderDetail,
    meta: {
      title: '订单详情 - 急速外卖'
    }
  },
  {
    path: '/addresses',
    name: 'AddressManagement',
    component: AddressManagement,
    meta: {
      title: '地址管理 - 急速外卖'
    }
  },
  {
    path: '/cart',
    name: 'Cart',
    component: Cart,
    meta: {
      title: '购物车 - 急速外卖'
    }
  }
  // add more routes
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// 路由守卫
router.beforeEach((to, from, next) => {
  // 设置页面标题
  if (to.meta.title) {
    document.title = to.meta.title
  }
  next();
})

export default router
