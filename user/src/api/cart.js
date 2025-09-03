// 购物车相关API
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
    console.error('购物车API请求错误:', error)
    return Promise.reject(error)
  }
)

// 购物车API接口
export const cartAPI = {
  // 获取用户购物车内容
  getUserCart: (userId) => {
    return api.get(`/users/${userId}/cart`)
  },

  // 添加商品到购物车
  addToCart: (userId, cartItemData) => {
    return api.post(`/users/${userId}/cartitems`, {
      dishId: cartItemData.dishId,
      merchantId: cartItemData.merchantId,
      quantity: cartItemData.quantity || 1
    })
  },

  // 更新购物车商品数量
  updateCartItem: (userId, cartItemId, quantity) => {
    return api.patch(`/users/${userId}/cartitems/${cartItemId}`, {
      quantity
    })
  },

  // 删除购物车商品
  removeFromCart: (userId, cartItemId) => {
    return api.delete(`/users/${userId}/cartitems/${cartItemId}`)
  },

  // 清空购物车
  clearCart: (userId) => {
    return api.delete(`/users/${userId}/cart`)
  },

  // 批量删除购物车商品
  removeMultipleItems: (userId, cartItemIds) => {
    return api.delete(`/users/${userId}/cartitems/batch`, {
      data: { cartItemIds }
    })
  },

  // 获取购物车统计信息
  getCartSummary: (userId) => {
    return api.get(`/users/${userId}/cart/summary`)
  },

  // 将购物车商品移至收藏夹
  moveToFavorites: (userId, cartItemId) => {
    return api.post(`/users/${userId}/cartitems/${cartItemId}/move-to-favorites`)
  },

  // 批量选择购物车商品
  selectCartItems: (userId, cartItemIds, selected = true) => {
    return api.patch(`/users/${userId}/cartitems/select`, {
      cartItemIds,
      selected
    })
  }
}

// 模拟数据生成器（用于开发测试）
export const mockCartAPI = {
  // 生成模拟购物车数据
  generateMockCartData: (userId) => {
    return {
      code: 0,
      message: '获取购物车成功',
      data: {
        userId,
        items: [
          {
            cartItemId: 'CART001',
            dishId: 'DISH001',
            dishName: '宫保鸡丁',
            dishImage: 'https://picsum.photos/120/120?random=1',
            price: 26.8,
            quantity: 2,
            merchantId: 'MERCHANT001',
            merchantName: '川味小厨',
            subtotal: 53.6,
            addedAt: new Date().toISOString(),
            isAvailable: true
          },
          {
            cartItemId: 'CART002',
            dishId: 'DISH002',
            dishName: '麻婆豆腐',
            dishImage: 'https://picsum.photos/120/120?random=2',
            price: 18.9,
            quantity: 1,
            merchantId: 'MERCHANT001',
            merchantName: '川味小厨',
            subtotal: 18.9,
            addedAt: new Date().toISOString(),
            isAvailable: true
          },
          {
            cartItemId: 'CART003',
            dishId: 'DISH003',
            dishName: '红烧肉',
            dishImage: 'https://picsum.photos/120/120?random=3',
            price: 35.5,
            quantity: 1,
            merchantId: 'MERCHANT002',
            merchantName: '江南菜馆',
            subtotal: 35.5,
            addedAt: new Date().toISOString(),
            isAvailable: false // 商品不可用
          }
        ],
        totalItems: 4,
        totalAmount: 108.0,
        estimatedDeliveryFee: 8.5,
        finalAmount: 116.5
      },
      timestamp: Date.now()
    }
  },

  // 模拟添加到购物车
  addToCart: (userId, cartItemData) => {
    return Promise.resolve({
      code: 0,
      message: '添加到购物车成功',
      data: {
        cartItemId: `CART${Date.now()}`,
        ...cartItemData,
        quantity: cartItemData.quantity || 1,
        addedAt: new Date().toISOString()
      },
      timestamp: Date.now()
    })
  },

  // 模拟更新购物车商品
  updateCartItem: (userId, cartItemId, quantity) => {
    return Promise.resolve({
      code: 0,
      message: '购物车更新成功',
      data: {
        cartItemId,
        quantity,
        updatedAt: new Date().toISOString()
      },
      timestamp: Date.now()
    })
  },

  // 模拟删除购物车商品
  removeFromCart: (userId, cartItemId) => {
    return Promise.resolve({
      code: 0,
      message: '商品已从购物车删除',
      data: {
        cartItemId,
        removedAt: new Date().toISOString()
      },
      timestamp: Date.now()
    })
  }
}

export default cartAPI
