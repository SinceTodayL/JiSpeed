/**
 * URL参数处理工具
 * 用于从URL中读取id和token等参数，并在系统中使用
 */

/**
 * 从URL中获取查询参数
 * @param paramName 参数名称
 * @returns 参数值或null
 */
export function getUrlParam(paramName: string): string | null {
  // 优先从当前路由获取
  if (typeof window !== 'undefined') {
    const urlParams = new URLSearchParams(window.location.search);
    const value = urlParams.get(paramName);
    if (value) {
      console.log(`从URL获取参数 ${paramName}:`, value);
      return value;
    }
  }
  
  // 从hash中获取（适用于hash路由）
  if (typeof window !== 'undefined' && window.location.hash) {
    const hashParams = window.location.hash.split('?')[1];
    if (hashParams) {
      const params = new URLSearchParams(hashParams);
      const value = params.get(paramName);
      if (value) {
        console.log(`从Hash获取参数 ${paramName}:`, value);
        return value;
      }
    }
  }
  
  return null;
}

/**
 * 从URL中获取用户ID
 * @returns 用户ID或默认值
 */
export function getUserIdFromUrl(): string {
  const urlId = getUrlParam('id');
  const defaultId = 'admin-001'; // 保留默认值作为后备
  
  if (urlId) {
    console.log('🎯 使用URL中的用户ID:', urlId);
    return urlId;
  } else {
    console.warn('⚠️ URL中未找到id参数，使用默认用户ID:', defaultId);
    return defaultId;
  }
}

/**
 * 从URL中获取Token
 * @returns Token或null
 */
export function getTokenFromUrl(): string | null {
  const urlToken = getUrlParam('token');
  
  if (urlToken) {
    console.log('🎯 使用URL中的Token:', urlToken.substring(0, 20) + '...');
    return urlToken;
  } else {
    console.warn('⚠️ URL中未找到token参数');
    return null;
  }
}

/**
 * 解析JWT Token获取用户信息
 * @param token JWT Token
 * @returns 解析后的payload或null
 */
export function parseJwtToken(token: string): any {
  try {
    const base64Payload = token.split('.')[1];
    const payload = atob(base64Payload);
    const parsed = JSON.parse(payload);
    console.log('📄 解析JWT Token:', parsed);
    return parsed;
  } catch (error) {
    console.error('❌ JWT Token解析失败:', error);
    return null;
  }
}

/**
 * 检查Token是否过期
 * @param token JWT Token
 * @returns true表示过期，false表示有效
 */
export function isTokenExpired(token: string): boolean {
  try {
    const payload = parseJwtToken(token);
    if (!payload || !payload.exp) {
      return false; // 如果没有过期时间，认为未过期
    }
    
    const currentTime = Math.floor(Date.now() / 1000);
    const expired = currentTime >= payload.exp;
    
    if (expired) {
      console.warn('⏰ Token已过期');
    } else {
      console.log('✅ Token有效，过期时间:', new Date(payload.exp * 1000));
    }
    
    return expired;
  } catch (error) {
    console.error('❌ Token验证失败:', error);
    return false;
  }
}

/**
 * 获取当前用户的完整信息（从URL和localStorage综合获取）
 * @returns 用户信息对象
 */
export function getCurrentUserInfo() {
  const urlId = getUserIdFromUrl();
  const urlToken = getTokenFromUrl();
  
  // 如果URL中有Token，尝试解析用户信息
  let tokenInfo = null;
  if (urlToken) {
    tokenInfo = parseJwtToken(urlToken);
  }
  
  return {
    userId: urlId,
    token: urlToken,
    tokenInfo: tokenInfo,
    userName: tokenInfo?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || 'JiSpeed平台管理员'
  };
}
