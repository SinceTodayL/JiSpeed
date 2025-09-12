// src/main.js
import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import router from './router'
import naive from 'naive-ui'
import { autoLoginForTesting } from './utils/autoLogin.js'
import { initUserAuth } from './utils/urlParams.js'
import { setAuthReady } from './utils/auth.js'

// 初始化用户认证，处理URL参数或从localStorage恢复
const authInfo = initUserAuth();

// 如果localStorage中没有认证信息，并且自动登录被启用，则执行自动登录
if (!authInfo.isLogin) {
  const enableAutoLogin = import.meta.env.VITE_ENABLE_AUTO_LOGIN === 'true';
  if (enableAutoLogin) {
    autoLoginForTesting();
  }
}

// 标记认证过程已完成
setAuthReady();

const app = createApp(App)

app.use(router)
app.use(naive)
app.mount('#app')
