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
  // 自定义筛选条件获取商家列表
  getMerchantList: (params = {}) => {
    const { merchantName, location, size, page, status } = params
    return api.get('/api/merchants', {
      params: {
        merchantName,
        location,
        size,
        page,
        status
      }
    })
  },

  // 模糊搜索，智能匹配
  getMerchantAutocomplete: (prefix, limit) => {
    return api.get('/api/merchants/autocomplete', {
      params: {
        prefix,
        limit
      }
    })
  },

  // 根据ID获取商家信息
  getMerchantById: (merchantId) => {
    return api.get(`/api/merchants/${merchantId}`)
  },

  // 根据ID修改商家信息
  updateMerchant: (merchantId, merchantData) => {
    return api.patch(`/api/merchants/${merchantId}`, {
      merchantName: merchantData.merchantName,
      status: merchantData.status,
      contactInfo: merchantData.contactInfo,
      location: merchantData.location,
      description: merchantData.description
    })
  }
}

// 商家菜品相关API
export const merchantDishAPI = {
  // 根据商家ID获取所有菜品信息
  getAllDishes: (merchantId, params = {}) => {
    const { page, size } = params
    return api.get(`/api/merchants/${merchantId}/dishes`, {
      params: {
        page,
        size
      }
    })
  },

  // 根据商家ID和菜品ID获取特定菜品详细信息
  getDishById: (merchantId, dishId) => {
    return api.get(`/api/merchants/${merchantId}/dishes/${dishId}`)
  },

  // 商家添加新菜品
  addNewDish: (merchantId, dishData) => {
    return api.post(`/api/merchants/${merchantId}/addNewDish`, dishData)
  },

  // 根据商家ID和菜品ID删除对应的菜品
  deleteDish: (merchantId, dishId) => {
    return api.delete(`/api/merchants/${merchantId}/${dishId}`)
  },

  // 根据商家ID获取菜品类目
  getDishCategories: (merchantId) => {
    return api.get(`/api/merchants/${merchantId}/dish-categories`)
  },

  // 根据商家ID和分类获取菜品
  getDishesByCategory: (merchantId, params = {}) => {
    const { categoryId, size, page } = params
    return api.get(`/api/merchants/${merchantId}/dishesByCategory`, {
      params: {
        categoryId,
        size,
        page
      }
    })
  }
}

// 商家统计相关API
export const merchantStatsAPI = {
  // 根据ID获取商家店铺数据统计信息
  getSalesStat: (merchantId) => {
    return api.get(`/api/merchants/${merchantId}/SalesStat`)
  },

  // 根据ID和日期获取商家特定日期的销售统计
  getSalesStatByDate: (merchantId, statDate) => {
    return api.get(`/api/merchants/${merchantId}/SalesStat/${statDate}`)
  }
}

// 商家订单相关API
export const merchantOrderAPI = {
  // 根据商家ID获取订单列表
  getMerchantOrders: (merchantId, params = {}) => {
    const { page, size } = params
    return api.get(`/api/merchants/${merchantId}/orders`, {
      params: {
        page,
        size
      }
    })
  },

  // 根据商家ID获取退单列表
  getMerchantRefunds: (merchantId, params = {}) => {
    const { page, size } = params
    return api.get(`/api/merchants/${merchantId}/refunds`, {
      params: {
        page,
        size
      }
    })
  }
}

// 商家申请相关API
export const merchantApplicationAPI = {
  // 商家提交申请
  submitApplication: (merchantId, applicationData) => {
    return api.post(`/api/merchants/${merchantId}/applications`, applicationData)
  },

  // 根据商家ID获取申请列表
  getApplications: (merchantId, params = {}) => {
    const { page, size } = params
    return api.get(`/api/merchants/${merchantId}/applications`, {
      params: {
        page,
        size
      }
    })
  }
}

// 导出默认的api实例，供其他模块使用
export default api
