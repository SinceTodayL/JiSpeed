// src/router/index.js
import { createRouter, createWebHistory } from 'vue-router'
import home from '@/views/home.vue'
import ApiTest from '@/views/ApiTest.vue'
import Login from '@/views/Login.vue'
import Register from '@/views/Register.vue'

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
    path: '/api-test',
    component: ApiTest,
    meta: {
      title: 'API测试页面'
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
