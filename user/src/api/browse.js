//外卖浏览相关的api
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

// 商家信息相关API（已有接口）
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
  },

  // ===== 以下为缺失接口，需要后端实现 =====
  
  // 获取所有商家列表（分页）- 缺失接口
  getAllMerchants: (params = {}) => {
    const { page = 1, limit = 20, keyword = '', sortBy = 'rating' } = params
    return api.get('/merchants', {
      params: {
        page,
        limit,
        keyword,
        sortBy
      }
    })
  },

  // 搜索商家 - 缺失接口
  searchMerchants: (keyword, page = 1, limit = 20) => {
    return api.get('/merchants/search', {
      params: {
        keyword,
        page,
        limit
      }
    })
  }
}

// 商家优惠券相关API（已有接口）
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

// 菜品相关API
export const dishAPI = {
  // ===== 以下为缺失接口，需要后端实现 =====
  
  // 获取菜品分类列表 - 缺失接口
  getCategories: () => {
    return api.get('/categories')
  },

  // 根据分类获取商家菜品 - 缺失接口
  getDishesByCategory: (merchantId, categoryId) => {
    return api.get(`/merchant/${merchantId}/dishes/category/${categoryId}`)
  },

  // 获取菜品详情 - 缺失接口
  getDishDetail: (dishId) => {
    return api.get(`/dishes/${dishId}`)
  },

  // 获取推荐菜品 - 缺失接口
  getRecommendedDishes: (limit = 10) => {
    return api.get('/dishes/recommended', {
      params: { limit }
    })
  },

  // 获取商家热门菜品 - 缺失接口
  getPopularDishes: (merchantId, limit = 5) => {
    return api.get(`/merchant/${merchantId}/dishes/popular`, {
      params: { limit }
    })
  }
}

// 搜索相关API
export const searchAPI = {
  // ===== 以下为缺失接口，需要后端实现 =====
  
  // 综合搜索 - 缺失接口
  search: (keyword, type = 'all', page = 1, limit = 20) => {
    return api.get('/search', {
      params: {
        keyword,
        type,
        page,
        limit
      }
    })
  }
}

// 导出默认的api实例，供其他模块使用
export default api
