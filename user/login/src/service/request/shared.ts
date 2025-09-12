import { useAuthStore } from '@/store/modules/auth';
import { localStg } from '@/utils/storage';
import type { RequestInstanceState } from './type';

export function getAuthorization() {
  const token = localStg.get('token');
  const Authorization = token ? `Bearer ${token}` : null;

  return Authorization;
}

/** refresh token (模拟实现，不发送真实请求) */
async function handleRefreshToken() {
  const { resetStore } = useAuthStore();

  // 模拟token刷新，不发送真实API请求
  const rToken = localStg.get('refreshToken') || '';
  
  if (rToken) {
    // 生成新的模拟token
    const newToken = 'mock-token-' + Date.now();
    const newRefreshToken = 'mock-refresh-token-' + Date.now();
    
    localStg.set('token', newToken);
    localStg.set('refreshToken', newRefreshToken);
    return true;
  }

  resetStore();
  return false;
}

export async function handleExpiredRequest(state: RequestInstanceState) {
  if (!state.refreshTokenFn) {
    state.refreshTokenFn = handleRefreshToken();
  }

  const success = await state.refreshTokenFn;

  setTimeout(() => {
    state.refreshTokenFn = null;
  }, 10000);

  return success;
}

export function showErrorMsg(state: RequestInstanceState, message: string) {
  if (!state.errMsgStack?.length) {
    state.errMsgStack = [];
  }

  const isExist = state.errMsgStack.includes(message);

  if (!isExist) {
    state.errMsgStack.push(message);

    window.$message?.error(message, {
      onLeave: () => {
        state.errMsgStack = state.errMsgStack.filter(msg => msg !== message);

        setTimeout(() => {
          state.errMsgStack = [];
        }, 5000);
      }
    });
  }
}
