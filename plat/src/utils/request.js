// 简单的请求工具，用于API调用
// 使用Vite代理，避免CORS跨域问题
const baseURL = ''; // 使用相对路径，通过Vite代理转发

/**
 * 通用请求函数
 * @param {Object} config - 请求配置
 */
function request(config) {
  const { url, method = 'GET', data, params } = config;
  
  let fullUrl = `${baseURL}${url}`;
  
  // 处理查询参数
  if (params && Object.keys(params).length > 0) {
    const searchParams = new URLSearchParams(params);
    fullUrl += `?${searchParams.toString()}`;
  }
  
  const fetchOptions = {
    method: method.toUpperCase(),
    headers: {
      'Content-Type': 'application/json',
    },
  };
  
  // 处理请求体
  if (data && (method.toUpperCase() === 'POST' || method.toUpperCase() === 'PUT' || method.toUpperCase() === 'PATCH')) {
    fetchOptions.body = JSON.stringify(data);
  }
  
  return fetch(fullUrl, fetchOptions)
    .then(response => {
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      return response.json();
    })
    .catch(error => {
      console.error('Request failed:', error);
      throw error;
    });
}

/**
 * GET请求
 * @param {string} url - 请求URL
 * @param {Object} params - 查询参数
 */
export function get(url, params) {
  const newParams = { ...(params || {}), _t: Date.now() }; // 防缓存
  return request({ url, method: 'GET', params: newParams });
}

/**
 * POST请求
 * @param {string} url - 请求URL
 * @param {Object} data - 请求体数据
 */
export function post(url, data) {
  return request({ url, method: 'POST', data });
}

/**
 * PUT请求
 * @param {string} url - 请求URL
 * @param {Object} data - 请求体数据
 */
export function put(url, data) {
  return request({ url, method: 'PUT', data });
}

/**
 * PATCH请求
 * @param {string} url - 请求URL
 * @param {Object} data - 请求体数据
 */
export function patch(url, data) {
  return request({ url, method: 'PATCH', data });
}

/**
 * DELETE请求
 * @param {string} url - 请求URL
 */
export function del(url) {
  return request({ url, method: 'DELETE' });
}

export default request;
