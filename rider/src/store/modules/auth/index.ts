import { computed, reactive, ref } from 'vue';
import { defineStore } from 'pinia';
import { useLoading } from '@sa/hooks';
import { getRiderInfo } from '@/service/api/rider';
import { localStg } from '@/utils/storage';
import { SetupStoreId } from '@/enum';
import { useRouteStore } from '../route';
import { useTabStore } from '../tab';
import { clearAuthStorage, getToken, getUserId } from './shared';

export const useAuthStore = defineStore(SetupStoreId.Auth, () => {
  const routeStore = useRouteStore();
  const tabStore = useTabStore();
  const { loading: loginLoading } = useLoading();

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
   * 初始化用户信息（从本地存储获取）
   */
  async function initUserInfo() {
    const hasToken = getToken();
    const hasUserId = getUserId();

    if (hasToken && hasUserId) {
      // 从本地存储获取用户ID并设置到 userInfo
      const userIdString = String(hasUserId);
      userInfo.userId = userIdString;
      token.value = String(hasToken);

      // 先创建基础骑手用户信息
      const riderUserInfo: Api.Auth.UserInfo = {
        userId: userIdString,
        userName: '骑手', // 临时显示"骑手"，后续从API获取真实姓名
        roles: ['rider'],
        buttons: []
      };

      // 更新用户信息
      Object.assign(userInfo, riderUserInfo);

      console.log('骑手用户信息已初始化:', riderUserInfo);

      // 尝试从API获取骑手的真实姓名
      try {
        const { data } = await getRiderInfo(userIdString);
        if (data && data.name) {
          // 更新用户名为真实姓名
          userInfo.userName = data.name;
          console.log('已获取骑手真实姓名:', data.name);
        }
      } catch (error) {
        console.warn('获取骑手真实姓名失败，使用默认名称:', error);
        // 如果API调用失败，保持默认的"骑手"名称
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
    initUserInfo
  };
});
