import { localStg } from '@/utils/storage';

/** Get token */
export function getToken() {
  return localStg.get('token') || '';
}

/** Get user ID */
export function getUserId() {
  return localStg.get('userId') || '';
}

/** Get user type */
export function getUserType() {
  return localStg.get('userType') || 0;
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
