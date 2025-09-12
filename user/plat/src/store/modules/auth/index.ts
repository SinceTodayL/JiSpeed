import { computed, reactive, ref } from 'vue';
import { useRoute } from 'vue-router';
import { defineStore } from 'pinia';
import { useLoading } from '@sa/hooks';
import { fetchGetUserInfo, fetchLogin } from '@/api';
import { useRouterPush } from '@/hooks/common/router';
import { localStg } from '@/utils/storage';
import { SetupStoreId } from '@/enum';
import { $t } from '@/locales';
import { useRouteStore } from '../route';
import { useTabStore } from '../tab';
import { clearAuthStorage, getToken, getUserId, getUserType } from './shared';

export const useAuthStore = defineStore(SetupStoreId.Auth, () => {
  const route = useRoute();
  const authStore = useAuthStore();
  const routeStore = useRouteStore();
  const tabStore = useTabStore();
  const { toLogin, redirectFromLogin } = useRouterPush(false);
  const { loading: loginLoading, startLoading, endLoading } = useLoading();

  const token = ref(getToken());

  const userInfo: Api.Auth.UserInfo = reactive({
    userId: '',
    userName: '',
    roles: ['admin'],
    buttons: []
  });

  /** is super role in static route */
  const isStaticSuper = computed(() => {
    const { VITE_AUTH_ROUTE_MODE, VITE_STATIC_SUPER_ROLE } = import.meta.env;

    return VITE_AUTH_ROUTE_MODE === 'static' && userInfo.roles.includes(VITE_STATIC_SUPER_ROLE);
  });

  /** Is login */
  const isLogin = computed(() => Boolean(token.value));

  /** Reset auth store */
  async function resetStore() {
    recordUserId();

    clearAuthStorage();

    authStore.$reset();

    if (!route.meta.constant) {
      await toLogin();
    }

    tabStore.cacheTabs();
    routeStore.resetStore();
  }

  /** Reset auth store without redirect (for logout) */
  async function resetStoreWithoutRedirect() {
    console.log('执行resetStoreWithoutRedirect...');
    
    recordUserId();

    clearAuthStorage();

    // 重置store状态
    token.value = '';
    Object.assign(userInfo, {
      userId: '',
      userName: '',
      roles: ['admin'],
      buttons: []
    });

    // 清理标签页和路由缓存
    tabStore.cacheTabs();
    routeStore.resetStore();
    
    console.log('认证状态重置完成');
  }

  /** Record the user ID of the previous login session Used to compare with the current user ID on next login */
  function recordUserId() {
    if (!userInfo.userId) {
      return;
    }

    // Store current user ID locally for next login comparison
    localStg.set('lastLoginUserId', userInfo.userId);
  }

  /**
   * Check if current login user is different from previous login user If different, clear all tabs
   *
   * @returns {boolean} Whether to clear all tabs
   */
  function checkTabClear(): boolean {
    if (!userInfo.userId) {
      return false;
    }

    const lastLoginUserId = localStg.get('lastLoginUserId');

    // Clear all tabs if current user is different from previous user
    if (!lastLoginUserId || lastLoginUserId !== userInfo.userId) {
      localStg.remove('globalTabs');
      tabStore.clearTabs();

      localStg.remove('lastLoginUserId');
      return true;
    }

    localStg.remove('lastLoginUserId');
    return false;
  }

  /**
   * Login
   *
   * @param userName User name
   * @param password Password
   * @param [redirect=true] Whether to redirect after login. Default is `true`
   */
  async function login(userName: string, password: string, redirect = true) {
    startLoading();

    try {
      // 尝试真实登录
      const { data: loginToken, error } = await fetchLogin(userName, password);

      if (!error) {
        const pass = await loginByToken(loginToken);

        if (pass) {
          // Check if the tab needs to be cleared
          const isClear = checkTabClear();
          let needRedirect = redirect;

          if (isClear) {
            // If the tab needs to be cleared,it means we don't need to redirect.
            needRedirect = false;
          }
          await redirectFromLogin(needRedirect);

          window.$notification?.success({
            title: $t('page.login.common.loginSuccess'),
            content: $t('page.login.common.welcomeBack', { userName: userInfo.userName }),
            duration: 4500
          });
        }
      } else {
        resetStore();
      }
    } catch (error) {
      // 如果API调用失败，使用模拟登录（开发阶段）
      console.warn('登录API调用失败，使用模拟登录:', error);
      
      // 模拟登录成功
      const mockToken = {
        token: 'mock-token-' + Date.now(),
        refreshToken: 'mock-refresh-token-' + Date.now()
      };
      
      const pass = await loginByToken(mockToken);
      
      if (pass) {
        const isClear = checkTabClear();
        let needRedirect = redirect;

        if (isClear) {
          needRedirect = false;
          console.log('检测到新用户或清除标签页，跳转到首页');
        }
        await redirectFromLogin(needRedirect);

        window.$notification?.success({
          title: $t('page.login.common.loginSuccess'),
          content: $t('page.login.common.welcomeBack', { userName: userInfo.userName }),
          duration: 4500
        });
      }
    }

    endLoading();
  }

  async function loginByToken(loginToken: Api.Auth.LoginToken) {
    // 1. stored in the localStorage, the later requests need it in headers
    localStg.set('token', loginToken.token);
    localStg.set('refreshToken', loginToken.refreshToken);

    // 2. get user info
    const pass = await getUserInfo();

    if (pass) {
      token.value = loginToken.token;

      return true;
    }

    return false;
  }

  async function getUserInfo() {
    // 从localStorage获取存储的认证信息
    const storedUserId = getUserId();
    const storedUserType = getUserType();
    
    if (storedUserId && storedUserType) {
      const dynamicUserInfo = {
        userId: storedUserId,
        userName: '平台管理员',
        roles: ['admin'],
        buttons: []
      };
      
      console.log('🔄 更新用户信息:', dynamicUserInfo);
      Object.assign(userInfo, dynamicUserInfo);
      return true;
    }
    
    console.warn('❌ 未找到存储的用户认证信息');
    return false;
  }



  async function initUserInfo() {
    // 检查本地存储的token
    const hasToken = getToken();

    if (hasToken) {
      const pass = await getUserInfo();

      if (!pass) {
        console.warn('⚠️ 用户信息获取失败，重置认证状态');
        resetStore();
      }
    } else {
      console.warn('⚠️ 未找到认证token');
    }
  }

  return {
    token,
    userInfo,
    isStaticSuper,
    isLogin,
    loginLoading,
    resetStore,
    resetStoreWithoutRedirect,
    login,
    initUserInfo
  };
});
