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
  if (!routeStore.isInitConstantRoute) {
    await routeStore.initConstantRoute();
  }
  if (!routeStore.isInitAuthRoute) {
    await routeStore.initAuthRoute();
  }

  setupProNaiveComponents(app);

  setupI18n(app);

  setupAppVersionNotification();

  app.mount('#app');
}

setupApp();
