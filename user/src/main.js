// src/main.js
import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import router from './router'
import naive from 'naive-ui'
import { autoLoginForTesting } from './utils/autoLogin.js'

const app = createApp(App)

// 检查是否启用自动登录（用于测试）
const enableAutoLogin = import.meta.env.VITE_ENABLE_AUTO_LOGIN === 'true'
if (enableAutoLogin) {
  console.log('自动登录已启用（测试模式）')
  autoLoginForTesting()
} else {
  console.log('自动登录已禁用')
}

app.use(router)
app.use(naive)
app.mount('#app')
