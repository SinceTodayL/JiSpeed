// 购物车相关API
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
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
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
    console.error('购物车API请求错误:', error)
    return Promise.reject(error)
  }
)

// 购物车API接口
export const cartAPI = {
  // 根据id获取用户的购物车内容
  getUserCart: (userId, params = {}) => {
    const { page, size } = params
    return api.get(`/api/users/${userId}/cart`, {
      params: {
        page,
        size
      }
    })
  },

  // 用户添加商品到购物车
    addToCart: (userId, dishId,merchantId) => {
      console.log('addToCart 参数:', userId, dishId,merchantId)
      return api.post(`/api/users/${userId}/cart`, {
        dishId,
        merchantId
      })
    },

  // 根据id删除购物车内容
  removeFromCart: (userId, cartId) => {
    return api.delete(`/api/users/${userId}/cart/${cartId}`)
  }
}

// 导出默认的api实例，供其他模块使用
export default api
