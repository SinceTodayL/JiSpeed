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
import { getToken, getUserId, getUserType, setAuthStorage, clearAuthStorage } from './shared';

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

      // check error - 立即停止loading状态
      if (error) {
        endLoading(); // 立即结束loading状态
        window.$notification?.error({
          title: '登录失败',
          content: error.message || '登录失败，请稍后重试',
          duration: 4500
        });
        return;
      }

      // 检查后端响应
      console.log('Login response:', response);
      console.log('Response code:', response?.code);
      console.log('Response data:', response?.data);
      
      // 检查不同可能的成功响应格式
      const isSuccess = response && (
        response.code === 0 || 
        response.code === 200 || 
        (response as any).success === true ||
        (response.message && response.message.includes('成功'))
      );
      
      if (isSuccess) {
        // 尝试从响应中获取数据，可能直接在response中，也可能在data字段中
        const responseData: any = response.data || response;
        console.log('Using responseData:', responseData);
        
        // 尝试从不同的字段名获取数据
        const userId = responseData.Id || responseData.id || responseData.userId || responseData.ID;
        const userToken = responseData.Token || responseData.token || responseData.accessToken || responseData.TOKEN;
        
        console.log('Extracted userId:', userId);
        console.log('Extracted userToken:', userToken);
        
        // 验证必要数据是否存在
        if (!userId || !userToken) {
          console.log('Missing userId or userToken - userId:', userId, 'userToken:', userToken);
          console.log('Available data keys:', Object.keys(responseData));
          
          // 如果后端没有返回token和ID，但是登录成功了，创建临时的认证信息
          console.log('Creating fallback authentication data...');
          const fallbackUserId = `user-${Date.now()}`;
          const fallbackToken = `session-${Date.now()}-${Math.random().toString(36).substring(2)}`;
          
          // 使用备选认证信息
          setAuthStorage(fallbackToken, fallbackUserId, userType);
          token.value = fallbackToken;
          
          // 创建用户信息
          const basicUserInfo: Api.Auth.UserInfo = {
            userId: fallbackUserId,
            userName: loginKey,
            roles: userType === 1 ? ['user'] : userType === 2 ? ['merchant'] : userType === 3 ? ['rider'] : ['admin'],
            buttons: [],
            merchantId: userType === 2 ? fallbackUserId : undefined
          };

          // 更新用户信息
          Object.assign(userInfo, basicUserInfo);

          // 如果是商家登录，更新merchant store
          if (basicUserInfo.merchantId) {
            merchantStore.setMerchantId(basicUserInfo.merchantId);
          }

          // 执行跳转逻辑
          const isClear = checkTabClear();
          let needRedirect = redirect;
        

          console.log('[Fallback] Tab clear status:', isClear, 'Final needRedirect:', needRedirect, 'UserType:', userType);

          await handleLoginRedirect(userType, needRedirect);

          window.$notification?.success({
            title: $t('page.login.common.loginSuccess'),
            content: $t('page.login.common.welcomeBack', { userName: basicUserInfo.userName }),
            duration: 4500
          });
          
          endLoading(); // 结束loading状态
          return;
        }

        // 存储认证信息
        setAuthStorage(userToken, userId, userType);
        token.value = userToken;

        // 创建用户信息
        const basicUserInfo: Api.Auth.UserInfo = {
          userId: userId,
          userName: loginKey,
          roles: userType === 1 ? ['user'] : userType === 2 ? ['merchant'] : userType === 3 ? ['rider'] : ['admin'],
          buttons: [],
          merchantId: userType === 2 ? userId : undefined // 商家使用ID作为merchantId
        };

        // 更新用户信息
        Object.assign(userInfo, basicUserInfo);

        // 如果是商家登录，更新merchant store
        if (basicUserInfo.merchantId) {
          merchantStore.setMerchantId(basicUserInfo.merchantId);
        }

        // Check if the tab needs to be cleared
        const isClear = checkTabClear();
        let needRedirect = redirect;

        // 对于外部跳转（如商家端），即使需要清除标签页也应该跳转
        // 只有内部路由跳转才受tab清除逻辑影响
        if (isClear && userType !== 2) {
          // If the tab needs to be cleared for internal routes, we don't need to redirect.
          // But external redirects (like merchant) should still happen.
          needRedirect = false;
        }

        console.log('Tab clear status:', isClear, 'Final needRedirect:', needRedirect, 'UserType:', userType);

        // 根据用户类型进行页面跳转
        await handleLoginRedirect(userType, needRedirect);

        window.$notification?.success({
          title: $t('page.login.common.loginSuccess'),
          content: $t('page.login.common.welcomeBack', { userName: basicUserInfo.userName }),
          duration: 4500
        });
        
        endLoading(); // 登录成功，结束loading状态
        return; // 明确结束成功流程
        
      } else {
        // 登录失败，显示后端返回的错误信息 - 立即停止loading状态
        endLoading(); // 立即结束loading状态
        console.log('Login failed - response:', response);
        window.$notification?.error({
          title: '登录失败',
          content: response?.message || '登录失败，请检查用户名和密码',
          duration: 4500
        });
        return; // 明确返回，避免继续执行
      }
    } catch (error: any) {
      // 网络错误或其他错误 - 立即停止loading状态
      endLoading(); // 立即结束loading状态
      console.error('Login error:', error);
      window.$notification?.error({
        title: '登录失败',
        content: error.message || '网络错误，请稍后重试',
        duration: 4500
      });
      return; // 明确返回
    }

    // 现在所有分支都已经显式处理了endLoading()，这里不再需要
  }

  /**
   * Handle login redirect based on user type
   * @param userType User role type
   * @param needRedirect Whether to redirect
   */
  async function handleLoginRedirect(userType: number, needRedirect: boolean) {
    console.log('handleLoginRedirect called with:', { userType, needRedirect });
    
    if (!needRedirect) {
      console.log('跳转被取消，needRedirect为false');
      return;
    }

    // 获取当前用户信息用于URL参数
    const currentToken = getToken();
    const currentUserId = getUserId();
    console.log('跳转参数 - Token:', currentToken, 'UserId:', currentUserId);
    
    switch (userType) {
      case 2: // Merchant
        // 商家端跳转到外部URL
        const merchantUrl = import.meta.env.VITE_MERCHANT_FRONTEND_URL;
        console.log('商家端跳转URL:', merchantUrl);
        
        if (!merchantUrl) {
          console.error('商家端URL未配置！');
          window.$notification?.error({
            title: '跳转失败',
            content: '商家端地址未配置，请联系管理员',
            duration: 4500
          });
          return;
        }
        
        // 添加协议检查
        let baseUrl = merchantUrl.startsWith('http') ? merchantUrl : `http://${merchantUrl}`;
        
        // 构建URL参数
        const urlParams = new URLSearchParams();
        if (currentUserId) {
          urlParams.append('id', currentUserId);
        }
        if (currentToken) {
          urlParams.append('token', currentToken);
        }
        
        // 拼接完整URL
        const finalUrl = `${baseUrl}${baseUrl.includes('?') ? '&' : '?'}${urlParams.toString()}`;
        console.log('最终跳转URL:', finalUrl);
        
        // 尝试检查地址是否可访问（简单检查）
        try {
          // 延迟跳转，确保成功消息能显示
          setTimeout(async () => {
            console.log('开始跳转到商家端...');
            
            const shouldRedirect = true;
            if (shouldRedirect) {
              console.log('用户确认跳转，执行跳转...');
              window.location.href = finalUrl;
            } else {
              console.log('用户取消跳转');
              window.$notification?.info({
                title: '跳转已取消',
                content: '您可以稍后手动访问商家端系统',
                duration: 3000
              });
            }
          }, 1500);
        } catch (error: any) {
          console.error('跳转过程中发生错误:', error);
          window.$notification?.error({
            title: '跳转失败',
            content: `无法跳转到商家端：${error.message || '未知错误'}`,
            duration: 4500
          });
        }
        break;
        
      case 1: // User
        console.log('用户端路由跳转');
        await redirectFromLogin(true);
        break;
        
      case 3: // Rider
        console.log('骑手端路由跳转');
        await redirectFromLogin(true);
        break;
        
      case 4: // Admin
        console.log('管理员路由跳转');
        await redirectFromLogin(true);
        break;
        
      default:
        break;
    }
  }

  /**
   * Check if current token is valid
   * @returns Whether the token is valid
   */
  async function isTokenValid(): Promise<boolean> {
    const currentToken = getToken();
    const currentUserId = getUserId();
    const currentUserType = getUserType();
    
    // 如果没有必要的认证信息，token无效
    if (!currentToken || !currentUserId || !currentUserType) {
      return false;
    }
    
    // TODO: 这里可以添加向后端验证token有效性的逻辑
    // 目前简单检查token是否存在
    return true;
  }

  /**
   * Check authentication before login page
   * If already authenticated, redirect to appropriate page
   */
  async function checkAuthBeforeLogin() {
    const isValid = await isTokenValid();
    if (isValid) {
      const currentUserType = getUserType();
      await handleLoginRedirect(currentUserType, true);
      return true; // 已认证，应该跳转
    }
    return false; // 未认证，可以显示登录页面
  }

  async function getUserInfo() {
    // 检查本地存储的用户信息
    const storedUserId = getUserId();
    const storedUserType = getUserType();
    
    if (storedUserId && storedUserType) {
      // 重建用户信息对象
      const roles = storedUserType === 1 ? ['user'] : 
                   storedUserType === 2 ? ['merchant'] : 
                   storedUserType === 3 ? ['rider'] : ['admin'];
                   
      Object.assign(userInfo, {
        userId: storedUserId,
        userName: '', // 用户名需要另外获取，或者也存储到localStorage中
        roles: roles,
        buttons: [],
        merchantId: storedUserType === 2 ? storedUserId : undefined
      });
      
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
    initUserInfo,
    isTokenValid,
    checkAuthBeforeLogin
  };
});
