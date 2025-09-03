//用户相关的api
import axios from 'axios'

// 从环境变量获取基础URL
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:8080'

// 创建axios实例
const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json'
  }
})

// 获取当前用户ID的工具函数
function getCurrentUserId() {
  return localStorage.getItem('userId')
}

// 请求拦截器
api.interceptors.request.use(
  (config) => {
    // 添加token认证信息
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    
    // 为需要用户ID的请求自动添加用户ID
    const userId = getCurrentUserId()
    if (userId) {
      // 如果是GET请求，添加到查询参数
      if (config.method === 'get') {
        config.params = config.params || {}
        if (!config.params.userId) {
          config.params.userId = userId
        }
      } else {
        // 如果是POST/PUT等请求，添加到请求体
        if (config.data && typeof config.data === 'object') {
          if (!config.data.userId) {
            config.data.userId = userId
          }
        }
      }
    }
    
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// 响应拦截器
api.interceptors.response.use(
  (response) => {
    return response.data
  },
  (error) => {
    console.error('用户API请求错误:', error)
    
    // 处理401未授权错误
    if (error.response?.status === 401) {
      localStorage.removeItem('token')
      localStorage.removeItem('userId')
      localStorage.removeItem('userType')
      localStorage.removeItem('userInfo')
      // 跳转到统一登录页
      window.location.href = 'http://localhost:9527/login'
    }
    
    return Promise.reject(error)
  }
)

// 用户认证相关API
export const authAPI = {
  // 用户登录
  login: (account, password) => {
    return api.post('/users/login', {
      account,
      password
    })
  },

  // 用户注册
  register: (userData) => {
    return api.post('/users/register', {
      account: userData.account,
      nickName: userData.nickName,
      gender: userData.gender,
      birthday: userData.birthday,
      password: userData.password
    })
  },

  // 重置密码
  resetPassword: (userId, oldPassword, newPassword) => {
    return api.post(`/users/${userId}/password/reset`, {
      oldPassword,
      newPassword
    })
  }
}

// 用户信息相关API
export const userAPI = {
  // 根据id获取用户信息
  getUserById: (userId) => {
    // 如果是测试用户ID，返回模拟数据
    if (userId === 'test_user_001') {
      return Promise.resolve({
        code: 200,
        message: '获取用户信息成功',
        data: {
          id: 'test_user_001',
          nickName: '测试用户',
          account: 'testuser@example.com',
          gender: 1,
          birthday: '1990-01-01',
          avatar: null,
          createTime: '2024-01-01T00:00:00',
          updateTime: '2024-01-01T00:00:00'
        }
      })
    }
    return api.get(`/users/${userId}`)
  },

  // 根据id部分修改用户信息
  updateUser: (userId, userData) => {
    return api.patch(`/user/${userId}`, userData, {
      headers: {
        'Content-Type': 'text/plain'
      }
    })
  },

  // 根据id部分删除用户信息
  deleteUser: (userId) => {
    return api.delete(`/users/${userId}`)
  },

  // 用户注销 (新增)
  logout: () => {
    return api.post('/users/logout', {}, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
  }
}

// 用户地址相关API
export const addressAPI = {
  // 根据id获取用户的所有地址列表
  getUserAddresses: (userId) => {
    // 如果是测试用户ID，返回模拟地址数据
    if (userId === 'test_user_001') {
      return Promise.resolve({
        code: 200,
        message: '获取地址列表成功',
        data: [
          {
            id: 'addr_001',
            receiverName: '张三',
            receiverPhone: '13800138000',
            detailedAddress: '北京市朝阳区某某街道123号',
            isDefault: true
          },
          {
            id: 'addr_002',
            receiverName: '李四',
            receiverPhone: '13900139000',
            detailedAddress: '上海市浦东新区某某路456号',
            isDefault: false
          }
        ]
      })
    }
    return api.get(`/users/${userId}/addresses`)
  },

  // 用户添加收货地址
  addAddress: (userId, addressData) => {
    return api.post(`/users/${userId}/addresses`, {
      receiverName: addressData.receiverName,
      receiverPhone: addressData.receiverPhone,
      detailedAddress: addressData.detailedAddress,
      isDefault: addressData.isDefault
    })
  },

  // 根据id更新地址
  updateAddress: (userId, addressId, addressData) => {
    return api.patch(`/users/${userId}/addresses/${addressId}`, addressData)
  },

  // 根据id删除地址
  deleteAddress: (userId, addressId) => {
    return api.delete(`/users/${userId}/addresses/${addressId}`)
  }
}

// 用户购物车相关API
export const cartAPI = {
  // 根据id获取用户的购物车内容
  getUserCart: (userId) => {
    // 如果是测试用户ID，返回模拟购物车数据
    if (userId === 'test_user_001') {
      return Promise.resolve({
        code: 200,
        message: '获取购物车成功',
        data: [
          {
            id: 'cart_001',
            dishId: 'dish_001',
            dishName: '宫保鸡丁',
            price: 28.0,
            quantity: 2,
            image: null,
            merchantName: '川菜餐厅'
          },
          {
            id: 'cart_002',
            dishId: 'dish_002',
            dishName: '红烧肉',
            price: 35.0,
            quantity: 1,
            image: null,
            merchantName: '家常菜馆'
          }
        ]
      })
    }
    return api.get(`/users/${userId}/cart`)
  },

  // 用户添加商品到购物车
  addToCart: (userId, dishId) => {
    return api.post(`/users/${userId}/cartitems`, {
      dishId,
      userId
    })
  },

  // 根据id删除购物车内容
  removeFromCart: (userId, cartId) => {
    return api.delete(`/users/${userId}/carts/${cartId}`)
  }
}

// 用户收藏相关API
export const favoriteAPI = {
  // 根据id获取用户收藏的商品列表
  getUserFavorites: (userId) => {
    return api.get(`/users/${userId}/favorites`)
  },

  // 用户添加收藏夹
  addToFavorites: (userId, dishId) => {
    return api.post(`/users/${userId}/favorites`, {
      dishId
    })
  },

  // 根据id删除收藏内容
  removeFavorite: (userId, favoriteId) => {
    return api.delete(`/users/${userId}/favorites/${favoriteId}`)
  }
}

// 用户评论相关API
export const reviewAPI = {
  // 根据id获取用户发布的所有评论
  getUserReviews: (userId) => {
    return api.get(`/users/${userId}/review`)
  },

  // 用户提交评论
  submitReview: (userId, reviewData) => {
    return api.post(`/users/${userId}/comments`, {
      orderId: reviewData.orderId,
      dishId: reviewData.dishId,
      rating: reviewData.rating,
      content: reviewData.content,
      isAnonymous: reviewData.isAnonymous
    })
  },

  // 根据id修改评论
  updateReview: (userId, reviewId, reviewData) => {
    return api.patch(`/users/${userId}/reviews/${reviewId}`, reviewData, {
      headers: {
        'Content-Type': 'text/plain'
      }
    })
  },

  // 根据id删除评论
  deleteReview: (userId, reviewId) => {
    return api.delete(`/users/${userId}/reviews/${reviewId}`)
  }
}

// 用户投诉相关API
export const complaintAPI = {
  // 根据id获取用户提交的所有投诉
  getUserComplaints: (userId) => {
    return api.get(`/users/${userId}/complaints`)
  },

  // 用户添加投诉
  submitComplaint: (userId, complaintData) => {
    return api.post(`/users/${userId}/complaints`, {
      orderId: complaintData.orderId,
      description: complaintData.description
    })
  },

  // 根据id修改投诉
  updateComplaint: (userId, complaintId, description) => {
    return api.patch(`/users/${userId}/complaints/${complaintId}`, description, {
      headers: {
        'Content-Type': 'text/plain'
      }
    })
  },

  // 根据id删除投诉
  deleteComplaint: (userId, complaintId) => {
    return api.delete(`/users/${userId}/complaints/${complaintId}`)
  }
}

// 导出默认的api实例，供其他模块使用
export default api

