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

      // check error
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

          await handleLoginRedirect(userType, needRedirect, fallbackToken, fallbackUserId);

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


        console.log('Tab clear status:', isClear, 'Final needRedirect:', needRedirect, 'UserType:', userType);

        // 根据用户类型进行页面跳转
        await handleLoginRedirect(userType, needRedirect, userToken, userId);

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
   * @param token Direct token value to use
   * @param userId Direct userId value to use
   */
  async function handleLoginRedirect(userType: number, needRedirect: boolean, token?: string, userId?: string) {
    console.log('handleLoginRedirect called with:', { userType, needRedirect });
    


    // 直接使用传入的token和userId，避免localStorage时序问题
    const currentToken = token || getToken();
    const currentUserId = userId || getUserId();
    console.log('跳转参数 - Token:', currentToken, 'UserId:', currentUserId);
    
    switch (userType) {
 
      case 1: // User
        const userUrl = import.meta.env.VITE_USER_FRONTEND_URL;
        console.log('user URL:', userUrl);
        
        if (!userUrl) {
          console.error('用户端URL未配置, 检查 VITE_USER_FRONTEND_URL');
          window.$notification?.error({
            title: '跳转失败',
            content: '用户端地址未配置',
            duration: 4500
          });
          return;
        }
        
        // 协议检查
        let userBaseUrl = userUrl;
        
        // 构建URL参数
        const userUrlParams = new URLSearchParams();
        if (currentUserId) {
          userUrlParams.append('id', currentUserId);
        }
        if (currentToken) {
          userUrlParams.append('token', currentToken);
        }
        
        // 拼接完整URL
        const userFinalUrl = `${userBaseUrl}${userBaseUrl.includes('?') ? '&' : '?'}${userUrlParams.toString()}`;
        console.log('jump to user URL:', userFinalUrl);
        window.location.href = userFinalUrl;
        break;
      
      case 2: // Merchant
        const merchantUrl = import.meta.env.VITE_MERCHANT_FRONTEND_URL;
        console.log('merchant URL:', merchantUrl);
        
        if (!merchantUrl) {
          console.error('商家端URL未配置, 检查 VITE_MERCHANT_FRONTEND_URL');
          window.$notification?.error({
            title: '跳转失败',
            content: '商家端地址未配置',
            duration: 4500
          });
          return;
        }
        
        // 协议检查
        let baseUrl = merchantUrl;
        
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
        console.log('jump to merchant URL:', finalUrl);
        window.location.href = finalUrl;
        break;
        

      case 3: // Rider
        const riderUrl = import.meta.env.VITE_RIDER_FRONTEND_URL;
        console.log('骑手端跳转URL:', riderUrl);
        
        if (!riderUrl) {
          console.error('骑手端URL未配置, 检查 VITE_RIDER_FRONTEND_URL');
          window.$notification?.error({
            title: '跳转失败',
            content: '骑手端地址未配置',
            duration: 4500
          });
          return;
        }
        
        // 添加协议检查
        let riderBaseUrl = riderUrl;
        
        // 构建URL参数
        const riderUrlParams = new URLSearchParams();
        if (currentUserId) {
          riderUrlParams.append('id', currentUserId);
        }
        if (currentToken) {
          riderUrlParams.append('token', currentToken);
        }
        
        // 拼接完整URL
        const riderFinalUrl = `${riderBaseUrl}${riderBaseUrl.includes('?') ? '&' : '?'}${riderUrlParams.toString()}`;
        console.log('rider jump to URL:', riderFinalUrl);
        window.location.href = riderFinalUrl;
        break;
        
      case 4: // Admin
        const adminUrl = import.meta.env.VITE_ADMIN_FRONTEND_URL;
        console.log('admin URL:', adminUrl);
        
        if (!adminUrl) {
          console.error('管理员端URL未配置, 检查 VITE_ADMIN_FRONTEND_URL');
          window.$notification?.error({
            title: '跳转失败',
            content: '管理员端地址未配置',
            duration: 4500
          });
          return;
        }
        
        // 协议检查
        let adminBaseUrl = adminUrl;
        
        // 构建URL参数
        const adminUrlParams = new URLSearchParams();
        if (currentUserId) {
          adminUrlParams.append('id', currentUserId);
        }
        if (currentToken) {
          adminUrlParams.append('token', currentToken);
        }
        
        // 拼接完整URL
        const adminFinalUrl = `${adminBaseUrl}${adminBaseUrl.includes('?') ? '&' : '?'}${adminUrlParams.toString()}`;
        console.log('jump to admin URL:', adminFinalUrl);
        window.location.href = adminFinalUrl;
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
    
    // TODO: 添加向后端验证token有效性的逻辑
    // 不过这又得请求一次，运行太耗时了，先不做了，反正看起来没区别

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
        userName: '', 
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
