// src/router/index.js
import { createRouter, createWebHistory } from 'vue-router'
import home from '@/views/home.vue'
import ApiTest from '@/views/ApiTest.vue'

const routes = [
  {
    path: '/',
    component: home,
    meta: {
      title: '首页'
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
