import { addAPIProvider } from '@iconify/vue';

/** Setup the iconify offline */
export function setupIconifyOffline() {
  const { VITE_ICONIFY_URL } = import.meta.env;

  // 设置默认的图标API提供商
  if (VITE_ICONIFY_URL) {
    addAPIProvider('', { resources: [VITE_ICONIFY_URL] });
  } else {
    // 如果没有配置自定义API提供商，使用默认的公共API
    addAPIProvider('', { resources: ['https://api.iconify.design'] });
  }

  // 确保Material Design图标集可用
  addAPIProvider('mdi', { resources: ['https://api.iconify.design'] });
}
