import { request } from '../request';

/**
 * Logs in a user with either a username or an email address.
 *
 * @param loginData The login credentials.
 * @param userType The user's role type (1: User, 2: Merchant, 3: Rider, 4: Admin).
 */
export function fetchLogin(loginData: { UserName?: string; Email?: string; PassWord: string }, userType: number) {
  return request<Api.Auth.LoginResponse>({
    url: `/api/Auth/login?userType=${userType}`,
    method: 'post',
    data: loginData
  });
}

/**
 * A convenience method for logging in with a username.
 *
 * @param userName The user's name.
 * @param password The user's password.
 */
export function fetchLoginByUsername(userName: string, password: string) {
  return fetchLogin({
    UserName: userName,
    PassWord: password
  }, 1); // Default to user type 1
}

/**
 * A convenience method for logging in with an email address.
 *
 * @param email The user's email address.
 * @param password The user's password.
 */
export function fetchLoginByEmail(email: string, password: string) {
  return fetchLogin({
    Email: email,
    PassWord: password
  }, 1); // Default to user type 1
}

/**
 * Registers a new user with email verification.
 *
 * @param registerData The registration data.
 * @param userType The user's role type (1: User, 2: Merchant, 3: Rider, 4: Admin).
 */
export function fetchRegister(registerData: Api.Auth.RegisterRequest, userType: number) {
  return request<Api.Auth.RegisterResponse>({
    url: `/api/Auth/register?userType=${userType}`,
    method: 'post',
    data: registerData
  });
}

/** Fetches the current user's information. */
export function fetchGetUserInfo() {
  return request<Api.Auth.UserInfo>({ url: '/api/Auth/getUserInfo' });
}

/**
 * Refreshes the authentication token.
 *
 * @param refreshToken The refresh token.
 */
export function fetchRefreshToken(refreshToken: string) {
  return request<Api.Auth.LoginToken>({
    url: '/api/Auth/refreshToken',
    method: 'post',
    data: {
      refreshToken
    }
  });
}

/**
 * Returns a custom backend error.
 *
 * @param code The error code.
 * @param msg The error message.
 */
export function fetchCustomBackendError(code: string, msg: string) {
  return request({ url: '/api/Auth/error', params: { code, msg } });
}
