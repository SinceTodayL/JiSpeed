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

// 优惠券API
export const couponAPI = {
  // 获取用户所有优惠券
  getUserCoupons: (userId) => {
    return api.get(`/users/${userId}/coupons`)
  },

  // 获取可用优惠券（基于订单金额）
  getAvailableCoupons: (userId, orderAmount) => {
    return api.get(`/users/${userId}/coupons/available`, { 
      params: { orderAmount } 
    })
  },

  // 使用优惠券
  useCoupon: (couponId, orderId) => {
    return api.put(`/coupons/${couponId}/use`, { orderId })
  },

  // 领取优惠券
  claimCoupon: (userId, couponTemplateId) => {
    return api.post(`/users/${userId}/coupons/claim`, { 
      couponTemplateId 
    })
  },

  // 获取可领取的优惠券模板
  getAvailableCouponTemplates: () => {
    return api.get('/coupon-templates/available')
  }
}

// 模拟数据生成器
const generateMockCoupon = (id, status = 0) => {
  const types = ['DISCOUNT', 'DELIVERY_FREE', 'FULL_REDUCE']
  const type = types[Math.floor(Math.random() * types.length)]
  
  const couponData = {
    couponId: id,
    couponName: '',
    couponType: type,
    status, // 0-未使用, 1-已使用, 2-已过期
    minOrderAmount: 0,
    discountValue: 0,
    maxDiscountAmount: null,
    expireTime: new Date(Date.now() + Math.random() * 30 * 24 * 60 * 60 * 1000).toISOString(),
    createTime: new Date(Date.now() - Math.random() * 7 * 24 * 60 * 60 * 1000).toISOString(),
    usedTime: status === 1 ? new Date().toISOString() : null,
    description: ''
  }

  switch (type) {
    case 'DISCOUNT':
      couponData.couponName = '满减优惠券'
      couponData.minOrderAmount = [20, 30, 50, 80][Math.floor(Math.random() * 4)]
      couponData.discountValue = [3, 5, 10, 15][Math.floor(Math.random() * 4)]
      couponData.description = `满${couponData.minOrderAmount}元减${couponData.discountValue}元`
      break
    case 'DELIVERY_FREE':
      couponData.couponName = '免配送费券'
      couponData.minOrderAmount = [15, 20, 25][Math.floor(Math.random() * 3)]
      couponData.discountValue = 3.5
      couponData.description = `满${couponData.minOrderAmount}元免配送费`
      break
    case 'FULL_REDUCE':
      couponData.couponName = '满额立减券'
      couponData.minOrderAmount = [50, 80, 100][Math.floor(Math.random() * 3)]
      couponData.discountValue = [8, 12, 20][Math.floor(Math.random() * 3)]
      couponData.maxDiscountAmount = couponData.discountValue
      couponData.description = `满${couponData.minOrderAmount}元立减${couponData.discountValue}元`
      break
  }

  return couponData
}

const generateMockCouponTemplate = (id) => {
  const types = ['DISCOUNT', 'DELIVERY_FREE', 'FULL_REDUCE']
  const type = types[Math.floor(Math.random() * types.length)]
  
  const template = {
    templateId: id,
    couponName: '',
    couponType: type,
    minOrderAmount: 0,
    discountValue: 0,
    maxDiscountAmount: null,
    validDays: [7, 15, 30][Math.floor(Math.random() * 3)],
    totalQuantity: 1000,
    remainingQuantity: Math.floor(Math.random() * 800) + 100,
    description: '',
    conditions: ''
  }

  switch (type) {
    case 'DISCOUNT':
      template.couponName = '新用户专享券'
      template.minOrderAmount = 25
      template.discountValue = 5
      template.description = '新用户首单立减5元'
      template.conditions = '仅限新用户首单使用'
      break
    case 'DELIVERY_FREE':
      template.couponName = '免配送费券'
      template.minOrderAmount = 20
      template.discountValue = 3.5
      template.description = '满20元免配送费'
      template.conditions = '全平台通用'
      break
    case 'FULL_REDUCE':
      template.couponName = '满减大礼包'
      template.minOrderAmount = 60
      template.discountValue = 12
      template.maxDiscountAmount = 12
      template.description = '满60元立减12元'
      template.conditions = '部分商家可用'
      break
  }

  return template
}

// 添加mock数据拦截器
if (process.env.NODE_ENV === 'development') {
  const mockResponses = {
    'GET /users/(.+)/coupons': () => {
      const coupons = Array.from({ length: 12 }, (_, i) => 
        generateMockCoupon(`COUPON${Date.now() - i * 1000}`, Math.floor(Math.random() * 3))
      )
      
      return Promise.resolve({
        code: 200,
        message: '获取成功',
        data: {
          coupons: coupons,
          total: coupons.length,
          available: coupons.filter(c => c.status === 0).length,
          used: coupons.filter(c => c.status === 1).length,
          expired: coupons.filter(c => c.status === 2).length
        }
      })
    },

    'GET /users/(.+)/coupons/available': (config) => {
      const orderAmount = parseFloat(config.params?.orderAmount || 0)
      const allCoupons = Array.from({ length: 8 }, (_, i) => 
        generateMockCoupon(`AVAILABLE${Date.now() - i * 1000}`, 0)
      )
      
      // 筛选可用优惠券
      const availableCoupons = allCoupons.filter(coupon => 
        coupon.status === 0 && 
        orderAmount >= coupon.minOrderAmount &&
        new Date(coupon.expireTime) > new Date()
      )
      
      return Promise.resolve({
        code: 200,
        message: '获取成功',
        data: {
          coupons: availableCoupons,
          orderAmount: orderAmount
        }
      })
    },

    'GET /coupon-templates/available': () => {
      const templates = Array.from({ length: 6 }, (_, i) => 
        generateMockCouponTemplate(`TEMPLATE${i + 1}`)
      )
      
      return Promise.resolve({
        code: 200,
        message: '获取成功',
        data: {
          templates: templates
        }
      })
    },

    'POST /users/(.+)/coupons/claim': (config) => {
      const requestData = JSON.parse(config.data)
      return Promise.resolve({
        code: 200,
        message: '领取成功',
        data: {
          couponId: `COUPON${Date.now()}`,
          templateId: requestData.couponTemplateId,
          expireTime: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString()
        }
      })
    },

    'PUT /coupons/(.+)/use': (config) => {
      const requestData = JSON.parse(config.data)
      return Promise.resolve({
        code: 200,
        message: '优惠券使用成功',
        data: {
          couponId: config.url.split('/')[2],
          orderId: requestData.orderId,
          usedTime: new Date().toISOString()
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
