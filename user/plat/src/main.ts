import { createApp } from 'vue';
import './plugins/assets';
import { setupAppVersionNotification, setupDayjs, setupIconifyOffline, setupLoading, setupNProgress } from './plugins';
import { setupStore } from './store';
import { setupRouter } from './router';
import { setupI18n } from './locales';
import { setAuthStorage } from './store/modules/auth/shared';
import App from './App.vue';

/**
 * 从URL参数中解析并存储认证信息
 */
function parseAndStoreAuthFromURL() {
  const urlParams = new URLSearchParams(window.location.search);
  const userId = urlParams.get('id') || urlParams.get('userId');
  const token = urlParams.get('token');
  
  console.log('解析URL参数:', { userId, token });
  
  if (userId && token) {
    // 存储认证信息到 AuthStorage
    setAuthStorage(token, userId, 4); // userType 设为 4 表示管理员
    
    console.log('URL参数存储认证信息:', { userId, token });
    
    // 可选：清理URL参数，避免敏感信息在地址栏显示
    const cleanUrl = window.location.origin + window.location.pathname;
    window.history.replaceState({}, document.title, cleanUrl);
    
    return true;
  }
  
  console.log('URL中未找到认证参数');
  return false;
}

async function setupApp() {
  setupLoading();

  setupNProgress();

  setupIconifyOffline();

  setupDayjs();

  const app = createApp(App);

  setupStore(app);

  // 解析URL参数中的认证信息并存储（在store初始化后）
  const hasAuthFromURL = parseAndStoreAuthFromURL();
  
  // 如果从URL获取了认证信息，在store初始化后立即处理
  if (hasAuthFromURL) {
    console.log('✅ 从URL获取到认证信息，已存储到localStorage');
  } else {
    console.log('⚠️ URL中未找到认证参数，使用模拟登录状态（开发阶段）');
    setAuthStorage('mock-dev-token-' + Date.now(), 'platform-admin-001', 4);
  }

  await setupRouter(app);

  setupI18n(app);

  setupAppVersionNotification();

  app.mount('#app');
}

setupApp();
