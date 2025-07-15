// src/router/index.js
import { createRouter, createWebHistory } from 'vue-router'
import TestApi from '../views/TestApi.vue' // 你之前写好的测试组件

const routes = [
  {
    path: '/test',
    component: TestApi
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
