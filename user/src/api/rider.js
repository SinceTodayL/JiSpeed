//骑手相关的api
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

// 骑手基本信息相关API
export const riderAPI = {
  // 根据id获取骑手信息
  getRiderById: (riderId) => {
    return api.get(`/api/riders/${riderId}`)
  },

  // 骑手注册
  registerRider: (riderData) => {
    return api.post('/api/riders', {
      name: riderData.name,
      phoneNumber: riderData.phoneNumber,
      vehicleNumber: riderData.vehicleNumber
    })
  },

  // 更新骑手信息
  updateRider: (riderId, riderData) => {
    return api.patch(`/api/riders/${riderId}`, {
      name: riderData.name,
      phoneNumber: riderData.phoneNumber,
      vehicleNumber: riderData.vehicleNumber
    })
  }
}

// 骑手订单分配相关API
export const riderAssignmentAPI = {
  // 根据id获取骑手订单列表
  getRiderAssignments: (riderId, status = null) => {
    const params = status !== null ? { status } : {}
    return api.get(`/api/riders/${riderId}/assignments`, { params })
  },

  // 根据骑手和订单id获取特定订单分配的详情
  getAssignmentDetail: (riderId, assignId) => {
    return api.get(`/api/riders/${riderId}/assignments/${assignId}`)
  },

  // 更新订单分配状态（接单/拒单）
  updateAssignmentStatus: (riderId, assignId, acceptedStatus) => {
    return api.patch(`/api/riders/${riderId}/assignments/${assignId}`, {
      acceptedStatus
    })
  }
}

// 骑手绩效相关API
export const riderPerformanceAPI = {
  // 根据id和时间获取骑手特定月份的绩效详情
  getRiderPerformance: (riderId, year, month) => {
    return api.get(`/api/riders/${riderId}/performances/${year}/${month}`)
  }
}

// 骑手考勤相关API
export const riderAttendanceAPI = {
  // 根据id获取骑手考勤记录
  getRiderAttendances: (riderId, params = {}) => {
    const queryParams = {}
    if (params.startDate) queryParams.startDate = params.startDate
    if (params.endDate) queryParams.endDate = params.endDate
    if (params.isLate !== undefined) queryParams.isLate = params.isLate
    if (params.isAbsent !== undefined) queryParams.isAbsent = params.isAbsent
    
    return api.get(`/api/riders/${riderId}/attendances`, { params: queryParams })
  },

  // 创建考勤记录（签到）
  checkIn: (riderId, attendanceData) => {
    return api.post(`/api/riders/${riderId}/attendances`, {
      checkDate: attendanceData.checkDate,
      checkInAt: attendanceData.checkInAt,
      isLate: attendanceData.isLate,
      isAbsent: attendanceData.isAbsent
    })
  },

  // 更新考勤记录（签退）
  checkOut: (riderId, attendanceId, checkoutAt) => {
    return api.patch(`/api/riders/${riderId}/attendances/${attendanceId}`, {
      checkoutAt
    })
  }
}

// 骑手排班相关API
export const riderScheduleAPI = {
  // 根据id获取骑手排班列表
  getRiderSchedules: (riderId, params = {}) => {
    const queryParams = {}
    if (params.startDate) queryParams.startDate = params.startDate
    if (params.endDate) queryParams.endDate = params.endDate
    
    return api.get(`/api/riders/${riderId}/schedules`, { params: queryParams })
  }
}

// 导出默认的api实例，供其他模块使用
export default api
