import { computed, reactive, ref } from 'vue';
import { useRoute } from 'vue-router';
import { defineStore } from 'pinia';
import { useLoading } from '@sa/hooks';
import { useRouterPush } from '@/hooks/common/router';
import { localStg } from '@/utils/storage';
import { SetupStoreId } from '@/enum';
import { $t } from '@/locales';
import { useRouteStore } from '../route';
import { useTabStore } from '../tab';
import { useMerchantStore } from '../merchant';
import { fetchLogin } from '@/service/api/auth';
import { getToken, clearAuthStorage } from './shared';

export const useAuthStore = defineStore(SetupStoreId.Auth, () => {
  const route = useRoute();
  const authStore = useAuthStore();
  const routeStore = useRouteStore();
  const tabStore = useTabStore();
  const merchantStore = useMerchantStore();
  const { toLogin, redirectFromLogin } = useRouterPush(false);
  const { loading: loginLoading, startLoading, endLoading } = useLoading();

  const token = ref(getToken());

  const userInfo: Api.Auth.UserInfo = reactive({
    userId: '',
    userName: '',
    roles: [],
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
   * Login - 调用后端API进行登录验证
   *
   * @param loginKey User name or email
   * @param password Password
   * @param userType User role type (1: User, 2: Merchant, 3: Rider, 4: Admin)
   * @param [redirect=true] Whether to redirect after login. Default is `true`
   */
  async function login(loginKey: string, password: string, userType: number, redirect = true) {
    startLoading();

    try {
      // 判断是邮箱还是用户名
      const isEmail = loginKey.includes('@');
      const loginData: { UserName?: string; Email?: string; PassWord: string } = {
        PassWord: password
      };

      if (isEmail) {
        loginData.Email = loginKey;
      } else {
        loginData.UserName = loginKey;
      }

      const { data: response, error } = await fetchLogin(loginData, userType);

      // 检查是否有错误
      if (error) {
        window.$notification?.error({
          title: '登录失败',
          content: error.message || '登录失败，请稍后重试',
          duration: 4500
        });
        return;
      }

      // 检查后端响应
      if (response && response.code === 0) {
        // 登录成功，由于后端不返回token，我们基于登录成功创建会话
        // 创建基本的用户信息
        const basicUserInfo: Api.Auth.UserInfo = {
          userId: `user-${Date.now()}`, // 临时ID，实际应该从后端获取
          userName: loginKey, // 使用登录时的用户名
          roles: userType === 1 ? ['user'] : userType === 2 ? ['merchant'] : userType === 3 ? ['rider'] : ['admin'],
          buttons: [],
          merchantId: userType === 2 ? 'temp-merchant-id' : undefined
        };

        // 创建会话token（由于后端不提供JWT，我们创建一个会话标识）
        const sessionToken = `session-${Date.now()}-${Math.random().toString(36).substring(2)}`;
        
        // 存储会话信息
        localStg.set('token', sessionToken);
        localStg.set('refreshToken', `refresh-${sessionToken}`);
        token.value = sessionToken;

        // 更新用户信息
        Object.assign(userInfo, basicUserInfo);

        // 如果是商家登录，更新merchant store
        if (basicUserInfo.merchantId) {
          merchantStore.setMerchantId(basicUserInfo.merchantId);
        }

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
          content: $t('page.login.common.welcomeBack', { userName: basicUserInfo.userName }),
          duration: 4500
        });
      } else {
        // 登录失败，显示后端返回的错误信息
        window.$notification?.error({
          title: '登录失败',
          content: response?.message || '登录失败，请检查用户名和密码',
          duration: 4500
        });
      }
    } catch (error: any) {
      // 网络错误或其他错误
      console.error('Login error:', error);
      window.$notification?.error({
        title: '登录失败',
        content: error.message || '网络错误，请稍后重试',
        duration: 4500
      });
    }

    endLoading();
  }

  // loginByToken函数已不再需要，因为我们在login函数中直接处理会话创建

  async function getUserInfo() {
    // 由于后端目前没有获取用户信息的API，
    // 这里只需要确认当前已有用户信息即可
    if (userInfo.userId) {
      return true;
    }
    
    // 如果没有用户信息，说明会话无效
    return false;
  }

  async function initUserInfo() {
    const hasToken = getToken();

    if (hasToken) {
      // 如果有token但没有用户信息，说明需要重新登录
      const hasUserInfo = await getUserInfo();
      if (!hasUserInfo) {
        // 清除无效的token
        await resetStore();
      }
    }
  }

  return {
    token,
    userInfo,
    isStaticSuper,
    isLogin,
    loginLoading,
    resetStore,
    login,
    initUserInfo
  };
});
