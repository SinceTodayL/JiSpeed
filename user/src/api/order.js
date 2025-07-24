import axios from 'axios'

// 创建axios实例
const api = axios.create({
  baseURL: '/api',
  timeout: 10000,
})

// 请求拦截器
api.interceptors.request.use(
  (config) => {
    // 添加认证token
    const token = localStorage.getItem('userToken')
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

// 订单API
export const orderAPI = {
  // 创建订单
  createOrder: (orderData) => {
    return api.post('/orders', orderData)
  },

  // 获取用户订单列表
  getUserOrders: (userId, params = {}) => {
    return api.get(`/users/${userId}/orders`, { params })
  },

  // 获取订单详情
  getOrderDetail: (orderId) => {
    return api.get(`/orders/${orderId}`)
  },

  // 订单支付
  payOrder: (orderId, paymentData) => {
    return api.post(`/orders/${orderId}/pay`, paymentData)
  },

  // 取消订单
  cancelOrder: (orderId, reason) => {
    return api.put(`/orders/${orderId}/cancel`, { reason })
  },

  // 确认收货
  confirmOrder: (orderId) => {
    return api.put(`/orders/${orderId}/confirm`)
  },

  // 查询支付状态
  getPaymentStatus: (orderId) => {
    return api.get(`/orders/${orderId}/payment-status`)
  }
}

// 模拟数据生成器
const generateMockOrder = (orderId, userId, status = 1) => {
  const statusTexts = {
    0: '待付款',
    1: '待接单', 
    2: '已接单',
    3: '配送中',
    4: '已完成',
    5: '已取消'
  }

  const merchantNames = ['老王家川菜馆', '小李烧烤店', '张阿姨家常菜', '麻辣香锅城', '兰州拉面馆']
  const dishNames = ['宫保鸡丁', '麻婆豆腐', '红烧肉', '酸辣土豆丝', '番茄鸡蛋']

  return {
    orderId,
    userId,
    merchantId: `MER${Math.random().toString().substr(2, 10)}`,
    merchantName: merchantNames[Math.floor(Math.random() * merchantNames.length)],
    merchantLogo: `https://picsum.photos/60/60?random=${Math.floor(Math.random() * 100)}`,
    orderStatus: status,
    orderStatusText: statusTexts[status],
    totalAmount: Number((Math.random() * 80 + 20).toFixed(1)),
    finalAmount: Number((Math.random() * 75 + 18).toFixed(1)),
    deliveryFee: 3.5,
    discountAmount: Number((Math.random() * 10).toFixed(1)),
    createAt: new Date(Date.now() - Math.random() * 7 * 24 * 60 * 60 * 1000).toISOString(),
    estimatedDeliveryTime: new Date(Date.now() + Math.random() * 2 * 60 * 60 * 1000).toISOString(),
    deliveryAddress: {
      addressId: `ADDR${Math.random().toString().substr(2, 6)}`,
      receiverName: ['张三', '李四', '王五', '赵六'][Math.floor(Math.random() * 4)],
      receiverPhone: '138' + Math.random().toString().substr(2, 8),
      detailedAddress: '北京市朝阳区xxx小区' + Math.floor(Math.random() * 10 + 1) + '号楼' + Math.floor(Math.random() * 20 + 101) + '室'
    },
    orderItems: Array.from({ length: Math.floor(Math.random() * 3) + 1 }, (_, i) => ({
      dishId: `DISH${i + 1}${Math.random().toString().substr(2, 6)}`,
      dishName: dishNames[Math.floor(Math.random() * dishNames.length)],
      coverUrl: `https://picsum.photos/80/80?random=${Math.floor(Math.random() * 100) + i}`,
      price: Number((Math.random() * 30 + 10).toFixed(1)),
      quantity: Math.floor(Math.random() * 3) + 1,
      get subtotal() { return Number((this.price * this.quantity).toFixed(1)) }
    })),
    remark: ['不要辣椒', '多放香菜', '少盐', '不要香菜', ''][Math.floor(Math.random() * 5)]
  }
}

// 模拟订单详情数据
const generateMockOrderDetail = (orderId) => {
  const order = generateMockOrder(orderId, 'USER123', 3)
  
  return {
    ...order,
    merchantPhone: '13800001111',
    merchantAddress: '北京市朝阳区建国路88号',
    paymentMethod: 'ALIPAY',
    paymentStatus: 1,
    payAt: new Date(new Date(order.createAt).getTime() + 2 * 60 * 1000).toISOString(),
    acceptAt: new Date(new Date(order.createAt).getTime() + 5 * 60 * 1000).toISOString(),
    deliveryInfo: {
      riderId: 'RIDER001',
      riderName: '李师傅',
      riderPhone: '13900139000',
      currentLocation: '距离您500米'
    },
    statusHistory: [
      {
        status: 0,
        statusText: '订单已创建',
        timestamp: order.createAt
      },
      {
        status: 1,
        statusText: '支付成功，等待商家接单',
        timestamp: new Date(new Date(order.createAt).getTime() + 2 * 60 * 1000).toISOString()
      },
      {
        status: 2,
        statusText: '商家已接单，准备中',
        timestamp: new Date(new Date(order.createAt).getTime() + 5 * 60 * 1000).toISOString()
      },
      {
        status: 3,
        statusText: '订单已出餐，配送中',
        timestamp: new Date(new Date(order.createAt).getTime() + 20 * 60 * 1000).toISOString()
      }
    ]
  }
}

// 添加mock数据拦截器
if (process.env.NODE_ENV === 'development') {
  // 拦截订单API请求，返回模拟数据
  const mockResponses = {
    'POST /orders': (config) => {
      const requestData = JSON.parse(config.data)
      const orderId = `ORDER${Date.now()}`
      return Promise.resolve({
        code: 200,
        message: '下单成功',
        data: {
          orderId,
          orderStatus: 0,
          totalAmount: requestData.totalAmount,
          finalAmount: requestData.finalAmount,
          createAt: new Date().toISOString(),
          estimatedDeliveryTime: new Date(Date.now() + 45 * 60 * 1000).toISOString(),
          paymentUrl: `https://payment.gateway.com/pay/${orderId}`
        }
      })
    },

    'GET /users/(.+)/orders': (config) => {
      const orders = Array.from({ length: 15 }, (_, i) => 
        generateMockOrder(`ORDER${Date.now() - i * 1000}`, 'USER123', Math.floor(Math.random() * 6))
      )
      
      return Promise.resolve({
        code: 200,
        message: '获取成功',
        data: {
          orders: orders.slice(0, 10),
          total: 25,
          page: 1,
          limit: 10,
          totalPages: 3
        }
      })
    },

    'GET /orders/(.+)': (config) => {
      const orderId = config.url.split('/').pop()
      return Promise.resolve({
        code: 200,
        message: '获取成功',
        data: generateMockOrderDetail(orderId)
      })
    },

    'POST /orders/(.+)/pay': () => {
      return Promise.resolve({
        code: 200,
        message: '支付请求成功',
        data: {
          paymentId: `PAY${Date.now()}`,
          paymentUrl: 'https://payment.gateway.com/pay/mock',
          qrCode: 'data:image/png;base64,mock-qr-code',
          expireTime: new Date(Date.now() + 15 * 60 * 1000).toISOString()
        }
      })
    },

    'PUT /orders/(.+)/cancel': () => {
      return Promise.resolve({
        code: 200,
        message: '订单取消成功',
        data: {
          orderId: 'ORDER123',
          orderStatus: 5,
          cancelAt: new Date().toISOString(),
          refundAmount: 73.5,
          refundStatus: 2
        }
      })
    },

    'PUT /orders/(.+)/confirm': () => {
      return Promise.resolve({
        code: 200,
        message: '确认收货成功',
        data: {
          orderId: 'ORDER123',
          orderStatus: 4,
          completeAt: new Date().toISOString()
        }
      })
    }
  }

  // 设置mock响应拦截器
  api.interceptors.response.use(
    (response) => response,
    async (error) => {
      const { config } = error
      const method = config.method.toUpperCase()
      const url = config.url.replace(config.baseURL, '')
      const mockKey = `${method} ${url}`
      
      // 查找匹配的mock响应
      for (const [pattern, handler] of Object.entries(mockResponses)) {
        const regex = new RegExp('^' + pattern.replace(/\(.+\)/g, '(.+)') + '$')
        if (regex.test(mockKey)) {
          console.log(`Mock API: ${mockKey}`)
          const mockResponse = await handler(config)
          return mockResponse
        }
      }
      
      return Promise.reject(error)
    }
  )
}

export default api
