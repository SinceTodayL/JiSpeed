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
    // 添加认证token
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
    console.error('优惠券API请求错误:', error)
    return Promise.reject(error)
  }
)

// 优惠券API
export const couponAPI = {
  // 返回user的couponId列表
  getUserCoupons: (userId, params = {}) => {
    const { isActive, amount, size, page } = params
    return api.get(`/api/users/${userId}/coupons`, {
      params: {
        isActive,
        amount,
        size,
        page
      }
    })
  },

  // 根据主键获取coupon详情
  getCouponById: (couponId) => {
    return api.get(`/api/coupons/${couponId}`)
  }
}

// 导出默认的api实例，供其他模块使用
export default api
