//用户相关的api
import axios from 'axios'

// 从环境变量获取基础URL
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL

// 创建axios实例
const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json'
  }
})

// 请求拦截器
api.interceptors.request.use(
  (config) => {
    // 在这里可以添加token等认证信息
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
    console.error('API请求错误:', error)
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
  }
}

// 用户地址相关API
export const addressAPI = {
  // 根据id获取用户的所有地址列表
  getUserAddresses: (userId) => {
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

