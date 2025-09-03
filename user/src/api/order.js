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
    console.error('订单API请求错误:', error)
    return Promise.reject(error)
  }
)

// 订单API
export const orderAPI = {
  // 根据用户ID获取订单列表
  getUserOrders: (userId, orderStatus, size, page) => {
    return api.get(`/api/users/${userId}/orders`, {
      params: {
        orderStatus,
        size,
        page
      }
    })
  },

  // 根据OrderId获取订单详情
  getOrderById: (orderId) => {
    return api.get(`/api/orders/${orderId}`)
  },

  // 用户创建订单
  createOrder: (userId, orderData) => {
    return api.post(`/api/users/${userId}/createOrder`, {
      orderAmount: orderData.orderAmount,
      couponId: orderData.couponId,
      addressId: orderData.addressId,
      merchantId: orderData.merchantId,
      dishQuantities: orderData.dishQuantities
    })
  },

  // 发起支付请求，创建待支付实体
  createPayment: (orderId, paymentData) => {
    return api.post(`/api/orders/${orderId}/createPayment`, paymentData)
  },

  // 用户创建退单申请
  createRefund: (userId, orderId, refundData) => {
    return api.post(`/api/users/${userId}/orders/${orderId}/refunds`, refundData)
  }
}

// 订单日志API
export const orderLogAPI = {
  // 根据Logid获取OrderLog的详情
  getOrderLogById: (logId) => {
    return api.get(`/api/orderLogs/${logId}`)
  }
}

// 导出默认的api实例，供其他模块使用
export default api
