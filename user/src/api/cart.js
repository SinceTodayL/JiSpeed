// 购物车相关API
import api from '@/utils/request.js'

// 购物车API接口
export const cartAPI = {
  // 根据id获取用户的购物车内容
  getUserCart: (userId, params = {}) => {
    const { page, size } = params
    return api.get(`/api/users/${userId}/cart`, {
      params: {
        page,
        size
      }
    })
  },

  // 用户添加商品到购物车
  addToCart: (userId, dishId, merchantId) => {
    console.log('addToCart 参数:', userId, dishId, merchantId)
    return api.post(`/api/users/${userId}/cart`, {
      dishId,
      merchantId
    })
  },

  // 根据id删除购物车内容
  removeFromCart: (userId, cartId) => {
    return api.delete(`/api/users/${userId}/cart/${cartId}`)
  },
  
  // 修改购物车商品数量
  updateCartItemQuantity: (userId, cartId, quantity) => {
    console.log('updateCartItemQuantity 参数:', userId, cartId, quantity)
    return api.patch(`/api/users/${userId}/cart/${cartId}`, null, {
      params: {
        quantity
      }
    })
  }
}

// 导出默认的api实例，供其他模块使用
export default api
