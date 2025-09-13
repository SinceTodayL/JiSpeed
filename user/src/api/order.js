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
  // 自动修复：订单列表接口路径改为  /api/orders，userId 作为 query 参数
  getUserOrders: (userId, orderStatus, size, page) => {
    // 修正为 RESTful 路径 /api/users/{userId}/orders
    console.log('订单API请求参数:', { userId, orderStatus, size, page });
    const fullUrl = `${API_BASE_URL}/api/users/${userId}/orders?orderStatus=${orderStatus}&size=${size}&page=${page}`;
    console.log('订单API完整URL:', fullUrl);
    return api.get(`/api/users/${userId}/orders`, {
      params: {
        orderStatus,
        size,
        page
      }
    })
  },

  // 根据OrderId获取订单详情
  getOrderDetail: (orderId) => {
    return api.get(`/api/orders/${orderId}`)
  },
  
  // 替代 getOrderDetail 的方法（为了兼容性保留两者）
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
    // 支付接口只需要 query 参数 channel，无需 body
    return api.post(`/api/orders/${orderId}/createPayment?channel=${paymentData}`)
  },

  // 获取支付详情
  getPaymentStatus: (payId) => {
    return api.get(`/api/payments/${payId}`)
  },
  
  // 确认支付
  confirmPayment: (payId) => {
    return api.patch(`/api/payments/${payId}/pay`)
  },

  // 取消订单
  cancelOrder: (orderId, reason) => {
    return api.patch(`/api/orders/${orderId}/cancel`, { reason })
  },

  // 确认收货
  confirmOrder: (orderId) => {
    return api.patch(`/api/orders/${orderId}/confirm`)
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
