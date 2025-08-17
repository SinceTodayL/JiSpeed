// 自动登录工具
export const autoLoginForTesting = () => {
  // 检查是否已经有登录信息
  const existingToken = localStorage.getItem('token')
  const existingUserId = localStorage.getItem('userId')
  
  if (existingToken && existingUserId) {
    console.log('已存在登录信息，无需自动登录')
    return {
      isLoggedIn: true,
      userId: existingUserId,
      token: existingToken
    }
  }
  
  // 为测试创建模拟登录信息
  const mockLoginData = {
    userId: 'test_user_001',
    token: 'mock_token_for_testing_' + Date.now(),
    userInfo: {
      id: 'test_user_001',
      nickName: '测试用户',
      account: 'testuser@example.com',
      gender: 1,
      birthday: '1990-01-01',
      avatar: null
    }
  }
  
  // 保存到localStorage
  localStorage.setItem('token', mockLoginData.token)
  localStorage.setItem('userId', mockLoginData.userId)
  localStorage.setItem('userInfo', JSON.stringify(mockLoginData.userInfo))
  
  console.log('自动登录成功 (测试模式)', mockLoginData)
  
  return {
    isLoggedIn: true,
    userId: mockLoginData.userId,
    token: mockLoginData.token,
    userInfo: mockLoginData.userInfo
  }
}

// 检查登录状态
export const checkLoginStatus = () => {
  const token = localStorage.getItem('token')
  const userId = localStorage.getItem('userId')
  
  return {
    isLoggedIn: !!(token && userId),
    token,
    userId
  }
}

// 清除登录信息
export const clearLoginInfo = () => {
  localStorage.removeItem('token')
  localStorage.removeItem('userId')
  localStorage.removeItem('userInfo')
}

// 获取当前用户信息
export const getCurrentUser = () => {
  try {
    const userInfo = localStorage.getItem('userInfo')
    return userInfo ? JSON.parse(userInfo) : null
  } catch (error) {
    console.error('获取用户信息失败:', error)
    return null
  }
}
