//外卖浏览相关的api
import api from '@/utils/request.js'

// 商家信息相关API
export const merchantAPI = {
  // 根据ID获取商家信息
  getMerchantById: (merchantId) => {
    return api.get(`/api/merchants/${merchantId}`)
  },

  // 根据商家ID获取所有菜品信息
  getAllDishes: (merchantId, params = {}) => {
    const { page, size } = params
    console.log(`请求商家菜品, URL: /api/merchants/${merchantId}/dishes`)
    return api.get(`/api/merchants/${merchantId}/dishes`, {
      params: {
        page,
        size
      }
    })
  },

  // 根据ID获取商家店铺数据统计信息
  getSalesStat: (merchantId) => {
    return api.get(`/api/merchants/${merchantId}/SalesStat`)
  },

  // 自定义筛选条件获取商家列表
  getAllMerchants: (params = {}) => {
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
  searchMerchants: (prefix, limit) => {
    return api.get('/api/merchants/autocomplete', {
      params: {
        prefix,
        limit
      }
    })
  }
}

// 菜品相关API
export const dishAPI = {
  // 根据商家ID和菜品ID获取特定菜品详细信息
  getDishDetail: (merchantId, dishId) => {
    return api.get(`/api/merchants/${merchantId}/dishes/${dishId}`)
  },

  // 根据商家ID获取菜品类目
  getCategories: (merchantId) => {
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

// 导出默认的api实例，供其他模块使用
export default api
