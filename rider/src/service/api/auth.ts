import { request } from '../request';

/**
 * Login
 *
 * @param userName User name
 * @param password Password
 */
export function fetchLogin(userName: string, password: string) {
  return request<Api.Auth.LoginResponse>({
    url: '/api/Auth/login?userType=3', // 骑手登录，userType=3
    method: 'post',
    data: { UserName: userName, PassWord: password } // 使用正确的字段名
  });
}

/** Get user info */
export function fetchGetUserInfo() {
  return request({
    url: '/api/Auth/getUserInfo', // 使用正确的API路径
    method: 'get'
  });
}

/**
 * Refresh token
 *
 * @param refreshToken Refresh token
 */
export function fetchRefreshToken(refreshToken: string) {
  return request<Api.Auth.LoginToken>({
    url: '/api/Auth/refreshToken', // 使用正确的API路径
    method: 'post',
    data: {
      refreshToken
    }
  });
}

/**
 * return custom backend error
 *
 * @param code error code
 * @param msg error message
 */
export function fetchCustomBackendError(code: string, msg: string) {
  return request({ url: '/api/auth/error', params: { code, msg } });
}
