// 骑手相关API
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
    console.error('骑手API请求错误:', error)
    return Promise.reject(error)
  }
)

// 骑手相关API
export const riderAPI = {
  // 根据id获取骑手信息
  getRiderById: (riderId) => {
    return api.get(`/api/riders/${riderId}`)
  },

  // 根据id获取骑手订单列表
  getRiderAssignments: (riderId, params = {}) => {
    const { page, size } = params
    return api.get(`/api/riders/${riderId}/assignments`, {
      params: {
        page,
        size
      }
    })
  },

  // 根据id和时间获取骑手特定月份的绩效详情
  getRiderPerformance: (riderId, year, month) => {
    return api.get(`/api/riders/${riderId}/performances/${year}/${month}`)
  },

  // 根据id获取骑手考勤记录
  getRiderAttendance: (riderId, params = {}) => {
    const { page, size } = params
    return api.get(`/api/riders/${riderId}/attendances`, {
      params: {
        page,
        size
      }
    })
  }
}

// 导出默认的api实例，供其他模块使用
export default api