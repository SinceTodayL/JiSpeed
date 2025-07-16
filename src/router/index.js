// src/router/index.js
import { createRouter, createWebHistory } from 'vue-router'
import home from '@/views/home.vue'

const routes = [
  {
    path: '/',
    component: home
  },
  // 商家相关路由
  {
    path: '/merchant/login',
    name: 'MerchantLogin',
    component: () => import('@/views/merchant/login.vue'),
    meta: {
      title: '商家登录'
    }
  },
  {
    path: '/merchant/main',
    component: () => import('@/views/merchant/main.vue'),
    // 使用嵌套路由来管理商家后台
    children: [
      {
        path: '', // 默认子路由
        name: 'MerchantDashboard',
        component: () => import('@/views/merchant/dashboard.vue'),
        meta: { title: '仪表盘' }
      },
      {
        path: 'menu', // -> /merchant/main/menu
        name: 'MenuManagement',
        component: () => import('@/views/merchant/menu_management.vue'),
        meta: { title: '菜单管理' }
      },
      {
        path: 'coupons', // -> /merchant/main/coupons
        name: 'CouponManagement',
        component: () => import('@/views/merchant/coupon_management.vue'),
        meta: { title: '优惠券管理' }
      }
      // 其他子路由可以在这里继续添加
    ]
  },
  // 根路径重定向到登录页
  {
    path: '/merchant',
    redirect: '/merchant/login'
  }
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
