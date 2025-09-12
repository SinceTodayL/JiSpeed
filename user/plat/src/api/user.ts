import { get } from '../utils/request.js';

// 格式化性别显示
export function formatGender(gender) {
  switch(gender) {
    case 1: return '男';
    case 2: return '女'; 
    case 3: return '未知';
    default: return '未知';
  }
}

// 格式化用户等级
export function formatUserLevel(level) {
  return `LV${level}`;
}

/** Get user info by user ID */
export function fetchUserInfo(userId) {
  return get(`/api/users/${userId}`);
}

/** Get user complaints by user ID */
export function fetchUserComplaints(userId) {
  return get(`/api/users/${userId}/complaints`);
}

/** Get user list - 使用真实Apifox接口 */
export function fetchUserList(params = {}) {
  return get('/api/users', params);
}