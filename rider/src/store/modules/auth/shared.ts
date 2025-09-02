import { localStg } from '@/utils/storage';

/** Get token */
export function getToken() {
  return localStg.get('token') || '';
}

/** Clear auth storage */
export function clearAuthStorage() {
  localStg.remove('token');
  localStg.remove('refreshToken');

  // 清理所有可能的认证相关存储
  sessionStorage.clear();

  // 清理cookies中的认证信息
  const cookies = document.cookie.split(";");
  for (const cookie of cookies) {
    const eqPos = cookie.indexOf("=");
    const name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
    document.cookie = `${name}=;expires=Thu, 01 Jan 1970 00:00:00 GMT;path=/`;
  }

  // 强制清理localStorage中可能存在的认证信息（包括带前缀的）
  const storagePrefix = import.meta.env.VITE_STORAGE_PREFIX || '';
  if (storagePrefix) {
    localStorage.removeItem(`${storagePrefix}token`);
    localStorage.removeItem(`${storagePrefix}refreshToken`);
  }

  // 也清理不带前缀的版本
  localStorage.removeItem('token');
  localStorage.removeItem('refreshToken');
}
