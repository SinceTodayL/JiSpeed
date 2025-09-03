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
    console.error('API请求错误:', error)
    return Promise.reject(error)
  }
)

// 地址管理API
export const addressAPI = {
  // 获取用户地址列表
  getUserAddresses: (userId, params = {}) => {
    const { page, size } = params
    return api.get(`/users/${userId}/addresses`, {
      params: {
        page,
        size
      }
    })
  },

  // 添加新地址
  addAddress: (userId, addressData) => {
    return api.post(`/users/${userId}/addresses`, {
      receiverName: addressData.receiverName,
      receiverPhone: addressData.receiverPhone,
      detailedAddress: addressData.detailedAddress,
      isDefault: addressData.isDefault || 0
    })
  },

  // 更新地址
  updateAddress: (userId, addressId, addressData) => {
    return api.patch(`/users/${userId}/addresses/${addressId}`, {
      receiverName: addressData.receiverName,
      receiverPhone: addressData.receiverPhone,
      detailedAddress: addressData.detailedAddress,
      isDefault: addressData.isDefault
    })
  },

  // 删除地址
  deleteAddress: (userId, addressId) => {
    return api.delete(`/users/${userId}/addresses/${addressId}`)
  }

}

// 导出默认的api实例，供其他模块使用
export default api
