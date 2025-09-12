/**
 * URL参数处理工具
 * 用于接收从统一登录界面跳转时传递的用户ID和Token
 */

/**
 * 从URL中获取查询参数
 * @param {string} param - 参数名
 * @returns {string|null} 参数值
 */
export function getUrlParam(param) {
  const urlParams = new URLSearchParams(window.location.search)
  return urlParams.get(param)
}

/**
 * 从URL中获取所有查询参数
 * @returns {Object} 包含所有参数的对象
 */
export function getAllUrlParams() {
  const urlParams = new URLSearchParams(window.location.search)
  const params = {}
  
  for (const [key, value] of urlParams) {
    params[key] = value
  }
  
  return params
}

/**
 * 清除URL中的查询参数（保持历史记录干净）
 */
export function clearUrlParams() {
  const url = new URL(window.location.href)
  url.search = ''
  window.history.replaceState({}, document.title, url.toString())
}

/**
 * 处理登录跳转参数
 * 从URL中获取id和token，存储到localStorage并清除URL参数
 * @returns {Object} 包含userId和token的对象
 */
export function handleLoginParams() {
  const userId = getUrlParam('id') || getUrlParam('userId');
  const token = getUrlParam('token')
  
  console.log('从URL获取登录参数:', { userId, token })
  
  if (userId && token) {
    // 存储到localStorage
    localStorage.setItem('userId', userId)
    localStorage.setItem('token', token)
    localStorage.setItem('userType', '1') // 用户类型为1
    
    // 设置用户基本信息
    const userInfo = {
      userId: userId,
      userName: `user_${userId}`,
      userType: 1,
      isLogin: true,
      loginTime: new Date().toISOString()
    }
    
    localStorage.setItem('userInfo', JSON.stringify(userInfo))
    
    console.log('✅ 登录信息已成功存储到localStorage:', userInfo)
    console.log('✅ localStorage验证 - token:', localStorage.getItem('token'))
    console.log('✅ localStorage验证 - userId:', localStorage.getItem('userId'))
    console.log('✅ localStorage验证 - userType:', localStorage.getItem('userType'))
    
    // 清除URL参数
    clearUrlParams()
    
    return { userId, token, success: true }
  }
  
  return { userId: null, token: null, success: false }
}

/**
 * 检查用户登录状态
 * @returns {boolean} 是否已登录
 */
export function checkLoginStatus() {
  const token = localStorage.getItem('token')
  const userId = localStorage.getItem('userId')
  const userType = localStorage.getItem('userType')
  
  return !!(token && userId && userType === '1')
}

/**
 * 获取当前用户信息
 * @returns {Object|null} 用户信息对象或null
 */
export function getCurrentUser() {
  try {
    const userInfo = localStorage.getItem('userInfo')
    return userInfo ? JSON.parse(userInfo) : null
  } catch (error) {
    console.error('获取用户信息失败:', error)
    return null
  }
}

/**
 * 清除用户登录信息
 */
export function clearUserAuth() {
  localStorage.removeItem('token')
  localStorage.removeItem('userId')
  localStorage.removeItem('userType')
  localStorage.removeItem('userInfo')
  
  console.log('用户登录信息已清除')
}

/**
 * 初始化用户认证状态
 * 在应用启动时调用，检查URL参数或localStorage中的登录状态
 * @returns {Object} 认证状态信息
 */
export function initUserAuth() {
  console.log('初始化用户认证状态...')
  
  // 首先尝试从URL参数获取登录信息
  const urlResult = handleLoginParams()
  
  if (urlResult.success) {
    console.log('从URL参数成功获取登录信息')
    return {
      isLogin: true,
      source: 'url',
      userId: urlResult.userId,
      token: urlResult.token
    }
  }
  
  // 如果URL中没有参数，检查localStorage
  const isLogin = checkLoginStatus()
  if (isLogin) {
    const userInfo = getCurrentUser()
    console.log('从localStorage恢复登录状态:', userInfo)
    return {
      isLogin: true,
      source: 'localStorage',
      userId: userInfo?.userId,
      token: localStorage.getItem('token')
    }
  }
  
  console.log('用户未登录')
  return {
    isLogin: false,
    source: 'none',
    userId: null,
    token: null
  }
}
