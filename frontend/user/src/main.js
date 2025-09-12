// src/main.js
import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import router from './router'
import naive from 'naive-ui'
import { autoLoginForTesting } from './utils/autoLogin.js'
import { initUserAuth } from './utils/urlParams.js'

const app = createApp(App)

// 初始化用户认证状态
// 优先处理统一登录跳转的URL参数，其次是自动登录测试模式
console.log('初始化用户应用...')

// 1. 处理统一登录跳转的参数
const authResult = initUserAuth()
console.log('认证初始化结果:', authResult)

// 2. 如果没有通过统一登录，且启用了测试模式的自动登录
if (!authResult.isLogin) {
  const enableAutoLogin = import.meta.env.VITE_ENABLE_AUTO_LOGIN === 'true'
  if (enableAutoLogin) {
    console.log('未检测到统一登录，启用自动登录测试模式')
    autoLoginForTesting()
  } else {
    console.log('用户未登录，且自动登录已禁用')
  }
} else {
  console.log(`用户已通过${authResult.source}方式登录，用户ID: ${authResult.userId}`)
}

app.use(router)
app.use(naive)
app.mount('#app')
