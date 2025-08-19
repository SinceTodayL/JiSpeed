import { createApp } from 'vue';
import './plugins/assets';
import { setupAppVersionNotification, setupDayjs, setupIconifyOffline, setupLoading, setupNProgress } from './plugins';
import { setupStore } from './store';
import { setupRouter } from './router';
import { localStg } from './utils/storage';
import { setupI18n } from './locales';
import App from './App.vue';

async function setupApp() {
  setupLoading();

  setupNProgress();

  setupIconifyOffline();

  setupDayjs();

  // 开发阶段：设置模拟登录状态，跳过登录页面
  if (!localStg.get('token')) {
    localStg.set('token', 'mock-dev-token-' + Date.now());
    localStg.set('refreshToken', 'mock-dev-refresh-token-' + Date.now());
  }
  
  // 强制设置中文语言
  localStg.set('lang', 'zh-CN');

  const app = createApp(App);

  setupStore(app);

  await setupRouter(app);

  setupI18n(app);

  setupAppVersionNotification();

  app.mount('#app');
}

setupApp();
