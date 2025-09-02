import { createApp } from 'vue';
import './plugins/assets';
import {
  setupAppVersionNotification,
  setupDayjs,
  setupIconifyOffline,
  setupLoading,
  setupNProgress,
  setupProNaiveComponents
} from './plugins';
import { setupStore } from './store';
import { useRouteStore } from './store/modules/route';
import { useAuthStore } from './store/modules/auth';
import { setupRouter } from './router';
import { setupI18n } from './locales';
import App from './App.vue';

async function setupApp() {
  setupLoading();

  setupNProgress();

  setupIconifyOffline();

  setupDayjs();

  const app = createApp(App);

  setupStore(app);

  await setupRouter(app);

  const routeStore = useRouteStore();
  const authStore = useAuthStore();

  // 骑手模块：确保路由完全初始化后再挂载应用
  console.log('=== 骑手模块启动 ===');

  if (!routeStore.isInitConstantRoute) {
    console.log('初始化常量路由...');
    await routeStore.initConstantRoute();
  }

  if (!routeStore.isInitAuthRoute) {
    console.log('初始化认证路由...');
    await routeStore.initAuthRoute();
  }

  // 初始化用户信息
  console.log('初始化用户信息...');
  await authStore.initUserInfo();

  console.log('路由初始化完成，准备挂载应用');

  setupProNaiveComponents(app);

  setupI18n(app);

  setupAppVersionNotification();

  app.mount('#app');
}

setupApp();
