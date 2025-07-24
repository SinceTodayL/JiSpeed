//商家相关的api
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

// 商家信息相关API
export const merchantAPI = {
  // 根据ID获取商家信息
  getMerchantById: (merchantId) => {
    return api.get(`/merchant/${merchantId}`)
  },

  // 根据商家ID获取所有菜品信息
  getAllDishes: (merchantId) => {
    return api.get(`/merchant/${merchantId}/getAllDishes`)
  },

  // 根据ID获取商家店铺数据统计信息
  getSalesStat: (merchantId) => {
    return api.get(`/merchant/${merchantId}/SalesStat`)
  }
}

// 商家优惠券相关API
export const merchantCouponAPI = {
  // 根据商家ID获取所有优惠券
  getAllCoupons: (merchantId) => {
    return api.get(`/merchant/${merchantId}/coupon`)
  },

  // 根据商家ID和优惠券ID获取优惠券信息
  getCouponById: (merchantId, couponId) => {
    return api.get(`/merchant/${merchantId}/coupon/${couponId}`)
  }
}

// 导出默认的api实例，供其他模块使用
export default api
