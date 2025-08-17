//商家相关的api
import axios from 'axios'

// 从环境变量获取基础URL
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:8080'

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
    console.error('商家API请求错误:', error)
    return Promise.reject(error)
  }
)

// 商家信息相关API
export const merchantAPI = {
  // 获取商家列表 (新增，基于后端API文档)
  getMerchantList: (params = {}) => {
    return api.get('/api/merchants', { params })
  },

  // 商家自动完成搜索 (新增，基于后端API文档)
  getMerchantAutocomplete: (params = {}) => {
    return api.get('/api/merchants/autocomplete', { params })
  },

  // 根据ID获取商家信息 (更新路径以匹配后端API)
  getMerchantById: (merchantId) => {
    return api.get(`/api/merchants/${merchantId}`)
  },

  // 根据商家ID获取所有菜品信息 (更新路径)
  getAllDishes: (merchantId, params = {}) => {
    return api.get(`/api/merchants/${merchantId}/dishes`, { params })
  },

  // 获取商家和菜品的评论 (新增，基于后端API文档)
  getMerchantDishReviews: (merchantId, dishId, params = {}) => {
    return api.get(`/api/merchants/${merchantId}/dishes/${dishId}/reviews`, { params })
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

// 模拟数据API (用于开发测试)
export const mockMerchantAPI = {
  // 生成模拟商家列表数据
  generateMockMerchantList: (params = {}) => {
    const { page = 1, size = 10, keyword = '', sortBy = 'recommended' } = params
    
    const allMerchants = [
      {
        merchantId: 'M001',
        name: '麻辣香锅店',
        description: '正宗四川口味，香辣过瘾',
        imageUrl: '/images/merchant1.jpg',
        rating: 4.6,
        reviewCount: 1256,
        distance: 0.8,
        deliveryFee: 3,
        deliveryTime: '25-35',
        minOrderAmount: 20,
        isOnline: true,
        tags: ['川菜', '香锅', '麻辣'],
        categoryId: 'chinese'
      },
      {
        merchantId: 'M002', 
        name: '日式料理屋',
        description: '新鲜刺身，地道日料',
        imageUrl: '/images/merchant2.jpg',
        rating: 4.8,
        reviewCount: 892,
        distance: 1.2,
        deliveryFee: 5,
        deliveryTime: '40-50',
        minOrderAmount: 50,
        isOnline: true,
        tags: ['日料', '刺身', '寿司'],
        categoryId: 'japanese'
      },
      {
        merchantId: 'M003',
        name: '汉堡王',
        description: '美式快餐，经典汉堡',
        imageUrl: '/images/merchant3.jpg',
        rating: 4.3,
        reviewCount: 2156,
        distance: 2.1,
        deliveryFee: 4,
        deliveryTime: '20-30',
        minOrderAmount: 25,
        isOnline: false,
        tags: ['快餐', '汉堡', '薯条'],
        categoryId: 'fast-food'
      },
      {
        merchantId: 'M004',
        name: '星巴克咖啡',
        description: '精品咖啡，舒适环境',
        imageUrl: '/images/merchant4.jpg',
        rating: 4.5,
        reviewCount: 1687,
        distance: 0.6,
        deliveryFee: 6,
        deliveryTime: '15-25',
        minOrderAmount: 30,
        isOnline: true,
        tags: ['咖啡', '饮品', '甜品'],
        categoryId: 'drinks'
      },
      {
        merchantId: 'M005',
        name: '海底捞火锅',
        description: '服务贴心，口味正宗',
        imageUrl: '/images/merchant5.jpg',
        rating: 4.7,
        reviewCount: 3421,
        distance: 1.8,
        deliveryFee: 8,
        deliveryTime: '50-70',
        minOrderAmount: 80,
        isOnline: true,
        tags: ['火锅', '聚餐', '川味'],
        categoryId: 'hotpot'
      },
      {
        merchantId: 'M006',
        name: '烧烤一条街',
        description: '深夜美食，烧烤天堂',
        imageUrl: '/images/merchant6.jpg',
        rating: 4.4,
        reviewCount: 987,
        distance: 1.5,
        deliveryFee: 4,
        deliveryTime: '35-45',
        minOrderAmount: 35,
        isOnline: true,
        tags: ['烧烤', '夜宵', '啤酒'],
        categoryId: 'barbecue'
      }
    ]
    
    // 模拟筛选
    let filteredMerchants = allMerchants
    
    if (keyword) {
      filteredMerchants = allMerchants.filter(merchant => 
        merchant.name.includes(keyword) || 
        merchant.description.includes(keyword) ||
        merchant.tags.some(tag => tag.includes(keyword))
      )
    }
    
    // 模拟排序
    if (sortBy === 'rating') {
      filteredMerchants.sort((a, b) => b.rating - a.rating)
    } else if (sortBy === 'distance') {
      filteredMerchants.sort((a, b) => a.distance - b.distance)
    } else if (sortBy === 'sales') {
      filteredMerchants.sort((a, b) => b.reviewCount - a.reviewCount)
    }
    
    // 模拟分页
    const startIndex = (page - 1) * size
    const endIndex = startIndex + size
    const paginatedMerchants = filteredMerchants.slice(startIndex, endIndex)
    
    return Promise.resolve({
      code: 0,
      message: '商家信息获取成功',
      data: paginatedMerchants,
      total: filteredMerchants.length,
      page,
      size
    })
  },

  // 生成模拟商家详情数据
  generateMockMerchantDetail: (merchantId) => {
    const merchantDetails = {
      'M001': {
        merchantId: 'M001',
        name: '麻辣香锅店',
        description: '正宗四川口味，香辣过瘾',
        imageUrl: '/images/merchant1.jpg',
        rating: 4.6,
        reviewCount: 1256,
        distance: 0.8,
        deliveryFee: 3,
        deliveryTime: '25-35',
        minOrderAmount: 20,
        isOnline: true,
        tags: ['川菜', '香锅', '麻辣'],
        address: '北京市朝阳区某某街道123号',
        phone: '010-12345678',
        businessHours: '10:00-22:00',
        announcement: '店铺公告：本店提供优质服务，欢迎品尝！'
      }
    }
    
    return Promise.resolve({
      code: 0,
      message: '商家信息获取成功',
      data: merchantDetails[merchantId] || null
    })
  }
}

// 导出默认的api实例，供其他模块使用
export default api
