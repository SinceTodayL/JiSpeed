// src/router/index.js
import { createRouter, createWebHistory } from 'vue-router'
import home from '@/views/home.vue'

const routes = [
  {
    path: '/',
    component: home
  },
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
