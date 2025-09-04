// 购物车状态管理组合式函数
import { ref, computed, watch } from 'vue'
import api from '@/api/cart.js'

// 全局购物车状态
const cartItems = ref([])
const loading = ref(false)

// 购物车计算属性
const totalItems = computed(() => {
  return cartItems.value.reduce((total, item) => total + item.quantity, 0)
})

const totalAmount = computed(() => {
  return cartItems.value
    .filter(item => item.isAvailable)
    .reduce((total, item) => total + item.subtotal, 0)
})

const selectedItems = computed(() => {
  return cartItems.value.filter(item => item.selected && item.isAvailable)
})

const selectedTotalAmount = computed(() => {
  return selectedItems.value.reduce((total, item) => total + item.subtotal, 0)
})

// 购物车操作函数
export function useCart() {
  
  // 获取购物车数据
  const fetchCartData = async (userId) => {
    if (!userId) return
    
    loading.value = true
    try {
      const response = await cartAPI.getUserCart(userId)
      
      if (response.code === 0) {
        cartItems.value = response.data.map(item => ({
          ...item,
          selected: true // 默认选中所有商品
        }))
      } else {
        console.error('获取购物车失败:', response.message)
      }
    } catch (error) {
      console.error('获取购物车数据失败:', error)
      cartItems.value = []
    } finally {
      loading.value = false
    }
  }

  // 添加商品到购物车
  const addToCart = async (dish) => {
    // 兼容对象参数，解构 userId 和 dishId，并确保为字符串且去除空格
    const userId = (dish.userId !== undefined && dish.userId !== null) ? String(dish.userId).trim() : '';
    const dishId = (dish.dishId !== undefined && dish.dishId !== null) ? String(dish.dishId).trim() : '';
    console.log('useCart.addToCart 参数:', dish)
    if (!userId || !dishId) {
      console.error('useCart.addToCart 参数错误:', dish)
      return { success: false, message: '参数错误' }
    }
    try {
      // 按后端接口要求，POST /users/{userId}/cartitems，body为{dishId, userId}
  const response = await api.post(`/api/users/${userId}/cartitems`, {
        dishId,
        userId
      })
      console.log('cartAPI.addToCart 响应:', response)
      if (response.data && response.data.code === 0) {
        // 重新获取购物车数据
        await fetchCartData(userId)
        return { success: true, message: '已添加到购物车' }
      } else {
        console.error('添加到购物车失败，后端返回:', response)
        return { success: false, message: response.data?.message || '添加失败' }
      }
    } catch (error) {
      console.error('添加到购物车异常:', error)
      return { success: false, message: '添加失败，请重试' }
    }
  }

  // 更新商品数量 - API文档中没有此接口，暂时保留客户端逻辑
  const updateQuantity = async (cartItemId, newQuantity) => {
    try {
      if (newQuantity <= 0) {
        return await removeFromCart(cartItemId)
      }

      // 更新本地状态
      const itemIndex = cartItems.value.findIndex(item => item.cartItemId === cartItemId)
      if (itemIndex > -1) {
        cartItems.value[itemIndex].quantity = newQuantity
        cartItems.value[itemIndex].subtotal = cartItems.value[itemIndex].price * newQuantity
      }
      return { success: true }
    } catch (error) {
      console.error('更新数量失败:', error)
      return { success: false, message: '更新失败，请重试' }
    }
  }

  // 从购物车删除商品
  const removeFromCart = async (userId, cartId) => {
    if (!userId || !cartId) return { success: false, message: '参数错误' }
    
    try {
      const response = await cartAPI.removeFromCart(userId, cartId)
      
      if (response.code === 0) {
        const itemIndex = cartItems.value.findIndex(item => item.cartId === cartId)
        if (itemIndex > -1) {
          cartItems.value.splice(itemIndex, 1)
        }
        return { success: true, message: '已从购物车删除' }
      } else {
        return { success: false, message: response.message }
      }
    } catch (error) {
      console.error('删除失败:', error)
      return { success: false, message: '删除失败，请重试' }
    }
  }

  // 清空购物车
  const clearCart = async () => {
    try {
      cartItems.value = []
      return { success: true, message: '购物车已清空' }
    } catch (error) {
      console.error('清空购物车失败:', error)
      return { success: false, message: '清空失败，请重试' }
    }
  }

  // 切换商品选中状态
  const toggleItemSelection = (cartItemId) => {
    const itemIndex = cartItems.value.findIndex(item => item.cartItemId === cartItemId)
    if (itemIndex > -1) {
      cartItems.value[itemIndex].selected = !cartItems.value[itemIndex].selected
    }
  }

  // 全选/取消全选
  const toggleSelectAll = (selected = true) => {
    cartItems.value.forEach(item => {
      if (item.isAvailable) {
        item.selected = selected
      }
    })
  }

  // 获取指定商品在购物车中的数量
  const getItemQuantity = (dishId, merchantId) => {
    const item = cartItems.value.find(
      item => item.dishId === dishId && item.merchantId === merchantId
    )
    return item ? item.quantity : 0
  }

  // 检查商品是否在购物车中
  const isInCart = (dishId, merchantId) => {
    return cartItems.value.some(
      item => item.dishId === dishId && item.merchantId === merchantId
    )
  }

  // 按商家分组购物车商品
  const getGroupedCartItems = () => {
    const groups = {}
    
    cartItems.value.forEach(item => {
      if (!groups[item.merchantId]) {
        groups[item.merchantId] = {
          merchantId: item.merchantId,
          merchantName: item.merchantName,
          items: []
        }
      }
      groups[item.merchantId].items.push(item)
    })

    return Object.values(groups)
  }

  // 获取购物车摘要信息
  const getCartSummary = () => {
    const availableItems = cartItems.value.filter(item => item.isAvailable)
    const selectedAvailableItems = availableItems.filter(item => item.selected)
    
    return {
      totalItems: totalItems.value,
      availableItems: availableItems.length,
      selectedItems: selectedAvailableItems.length,
      totalAmount: totalAmount.value,
      selectedTotalAmount: selectedTotalAmount.value,
      merchantCount: new Set(availableItems.map(item => item.merchantId)).size
    }
  }

  return {
    // 状态
    cartItems,
    loading,
    
    // 计算属性
    totalItems,
    totalAmount,
    selectedItems,
    selectedTotalAmount,
    
    // 方法
    fetchCartData,
    addToCart,
    updateQuantity,
    removeFromCart,
    clearCart,
    toggleItemSelection,
    toggleSelectAll,
    getItemQuantity,
    isInCart,
    getGroupedCartItems,
    getCartSummary
  }
}

// 导出单例实例，确保全局状态一致性
let cartInstance = null

export function getCartInstance() {
  if (!cartInstance) {
    cartInstance = useCart()
  }
  return cartInstance
}
