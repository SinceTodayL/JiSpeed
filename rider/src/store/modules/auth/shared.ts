import { localStg } from '@/utils/storage';

/** Get token */
export function getToken() {
  return localStg.get('token') || '';
}

/** Get user ID */
export function getUserId() {
  return localStg.get('userId') || '';
}

/** Set auth storage */
export function setAuthStorage(token: string, userId: string, userType: number) {
  localStg.set('token', token);
  localStg.set('userId', userId);
  localStg.set('userType', userType);
}

/** Clear auth storage */
export function clearAuthStorage() {
  localStg.remove('token');
  localStg.remove('refreshToken');
  localStg.remove('userId');
  localStg.remove('userType');
}
