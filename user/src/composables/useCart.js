// 购物车状态管理组合式函数
import { ref, computed, watch } from 'vue'
import { cartAPI, mockCartAPI } from '@/api/cart.js'

// 全局购物车状态
const cartItems = ref([])
const loading = ref(false)
const currentUserId = ref('USER001') // 模拟用户ID

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
  const fetchCartData = async () => {
    loading.value = true
    try {
      // 使用模拟数据进行开发测试
      const response = await mockCartAPI.generateMockCartData(currentUserId.value)
      
      if (response.code === 0) {
        cartItems.value = response.data.items.map(item => ({
          ...item,
          selected: item.isAvailable // 默认选中可用商品
        }))
      } else {
        console.error('获取购物车失败:', response.message)
      }
    } catch (error) {
      console.error('获取购物车数据失败:', error)
      // 如果API失败，使用空数据
      cartItems.value = []
    } finally {
      loading.value = false
    }
  }

  // 添加商品到购物车
  const addToCart = async (dishData) => {
    try {
      const cartItemData = {
        dishId: dishData.dishId,
        dishName: dishData.dishName,
        dishImage: dishData.coverUrl,
        price: dishData.price,
        merchantId: dishData.merchantId,
        merchantName: dishData.merchantName,
        quantity: 1
      }

      const response = await mockCartAPI.addToCart(currentUserId.value, cartItemData)
      
      if (response.code === 0) {
        // 检查是否已存在该商品
        const existingItemIndex = cartItems.value.findIndex(
          item => item.dishId === dishData.dishId && item.merchantId === dishData.merchantId
        )

        if (existingItemIndex > -1) {
          // 更新数量
          cartItems.value[existingItemIndex].quantity += 1
          cartItems.value[existingItemIndex].subtotal = 
            cartItems.value[existingItemIndex].price * cartItems.value[existingItemIndex].quantity
        } else {
          // 添加新商品
          cartItems.value.push({
            ...response.data,
            subtotal: response.data.price * response.data.quantity,
            selected: true,
            isAvailable: true
          })
        }

        return { success: true, message: '已添加到购物车' }
      } else {
        return { success: false, message: response.message }
      }
    } catch (error) {
      console.error('添加到购物车失败:', error)
      return { success: false, message: '添加失败，请重试' }
    }
  }

  // 更新商品数量
  const updateQuantity = async (cartItemId, newQuantity) => {
    try {
      if (newQuantity <= 0) {
        return await removeFromCart(cartItemId)
      }

      const response = await mockCartAPI.updateCartItem(currentUserId.value, cartItemId, newQuantity)
      
      if (response.code === 0) {
        const itemIndex = cartItems.value.findIndex(item => item.cartItemId === cartItemId)
        if (itemIndex > -1) {
          cartItems.value[itemIndex].quantity = newQuantity
          cartItems.value[itemIndex].subtotal = cartItems.value[itemIndex].price * newQuantity
        }
        return { success: true }
      } else {
        return { success: false, message: response.message }
      }
    } catch (error) {
      console.error('更新数量失败:', error)
      return { success: false, message: '更新失败，请重试' }
    }
  }

  // 从购物车删除商品
  const removeFromCart = async (cartItemId) => {
    try {
      const response = await mockCartAPI.removeFromCart(currentUserId.value, cartItemId)
      
      if (response.code === 0) {
        const itemIndex = cartItems.value.findIndex(item => item.cartItemId === cartItemId)
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
    currentUserId,
    
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
