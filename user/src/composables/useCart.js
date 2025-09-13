// 购物车状态管理组合式函数
import { ref, computed, watch } from 'vue'
import { cartAPI } from '@/api/cart.js'
import { dishAPI, merchantAPI } from '@/api/browse.js'

// 全局购物车状态
const cartItems = ref([])
const loading = ref(false)

// 购物车计算属性
const totalItems = computed(() => {
  return cartItems.value.reduce((total, item) => total + Number(item.quantity), 0)
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
  // 严格调用后端API获取购物车数据
  const fetchCartData = async (userId) => {
    if (!userId) return
    loading.value = true
    try {
      const response = await cartAPI.getUserCart(userId)
      console.log('获取购物车原始数据:', response)
      
      // 检查 response 是否已经是数组（已经被 axios 拦截器处理）
      const cartData = Array.isArray(response) 
        ? response 
        : ((response && response.data && Array.isArray(response.data)) ? response.data : [])
      
      console.log('处理后的购物车数据源:', cartData)
      
      if (cartData.length > 0) {
        // 清空当前购物车
        cartItems.value = []
        
        // 创建一个存放获取菜品详情Promise的数组
        const dishPromises = []
        
        // 遍历购物车项目，为每个项目获取菜品详情
        for (const item of cartData) {
          // 创建一个基本的购物车项目对象
          const cartItem = {
            cartItemId: item.cartId,
            dishId: item.dishId,
            merchantId: item.merchantId,
            userId: item.userId,
            addedAt: item.addedAt,
            // 默认值
            dishName: '加载中...',
            price: 0,
            quantity: Number(item.quantity) || 1, // 使用后端返回的数量
            image: '',
            coverUrl: '',
            merchantName: '加载中...',
            isAvailable: true,
            selected: true,
            subtotal: 0
          }
          
          // 添加到购物车
          cartItems.value.push(cartItem)
          
          // 创建获取菜品详情的Promise
          const dishPromise = async () => {
            try {
              console.log(`开始获取菜品详情: merchantId=${item.merchantId}, dishId=${item.dishId}`);
              // 获取菜品详情
              const dishResponse = await dishAPI.getDishDetail(item.merchantId, item.dishId)
              console.log('菜品详情响应:', dishResponse);
              
              // 检查 dishResponse 是否已经是对象（已经被 axios 拦截器处理）
              const dishData = (dishResponse && typeof dishResponse === 'object' && !dishResponse.code) 
                ? dishResponse 
                : ((dishResponse && dishResponse.code === 0 && dishResponse.data) ? dishResponse.data : null)
              
              if (dishData) {
                // 更新购物车项
                const index = cartItems.value.findIndex(i => i.cartItemId === item.cartId)
                if (index !== -1) {
                  cartItems.value[index].dishName = dishData.dishName || '未知菜品'
                  cartItems.value[index].price = parseFloat(dishData.price) || 0
                  cartItems.value[index].coverUrl = dishData.coverUrl || ''
                  cartItems.value[index].image = dishData.coverUrl || ''
                  cartItems.value[index].subtotal = parseFloat(dishData.price) * (item.quantity || 1)
                  // 使用API返回的数量
                  cartItems.value[index].quantity = Number(item.quantity) || 1
                }
              } else {
                console.warn(`菜品详情获取失败, dishId=${item.dishId}:`, dishResponse);
                // 找到对应的购物车项，设置默认值
                const index = cartItems.value.findIndex(i => i.cartItemId === item.cartId)
                if (index !== -1) {
                  cartItems.value[index].dishName = '获取菜品信息失败'
                  cartItems.value[index].quantity = item.quantity || 1
                }
              }
              
              console.log(`开始获取商家信息: merchantId=${item.merchantId}`);
              // 获取商家信息
              const merchantResponse = await merchantAPI.getMerchantById(item.merchantId)
              console.log('商家信息响应:', merchantResponse);
              
              // 检查 merchantResponse 是否已经是对象（已经被 axios 拦截器处理）
              const merchantData = (merchantResponse && typeof merchantResponse === 'object' && !merchantResponse.code) 
                ? merchantResponse 
                : ((merchantResponse && (merchantResponse.code === 0 || merchantResponse.code === 200) && merchantResponse.data) ? merchantResponse.data : null)
              
              if (merchantData) {
                // 更新购物车项
                const index = cartItems.value.findIndex(i => i.cartItemId === item.cartId)
                if (index !== -1) {
                  cartItems.value[index].merchantName = merchantData.merchantName || '未知商家'
                }
              } else {
                console.warn(`商家信息获取失败, merchantId=${item.merchantId}:`, merchantResponse);
                // 找到对应的购物车项，设置默认值
                const index = cartItems.value.findIndex(i => i.cartItemId === item.cartId)
                if (index !== -1) {
                  cartItems.value[index].merchantName = '获取商家信息失败'
                }
              }
            } catch (error) {
              console.error('获取菜品或商家详情失败:', error)
            }
          }
          
          dishPromises.push(dishPromise())
        }
        
        // 等待所有菜品详情获取完成
        await Promise.all(dishPromises)
        
        console.log('处理后的购物车数据:', cartItems.value)
      } else {
        cartItems.value = []
        console.error('获取购物车失败:', response.message)
      }
    } catch (error) {
      cartItems.value = []
      console.error('获取购物车数据失败:', error)
    } finally {
      loading.value = false
    }
  }

  // 添加商品到购物车
  // 严格调用后端API添加商品到购物车
  const addToCart = async ({ userId, dishId, merchantId }) => {
    if (!userId || !dishId || !merchantId) {
      return { success: false, message: '参数错误' }
    }
    try {
      const response = await cartAPI.addToCart(userId, dishId, merchantId)
      console.log('添加到购物车响应:', response);
      
      // 请求成功，直接刷新购物车数据
      await fetchCartData(userId)
      return { success: true, message: '已添加到购物车' }
    } catch (error) {
      console.error('添加到购物车失败:', error)
      return { success: false, message: '添加失败，请重试' }
    }
  }

  // 更新商品数量
  const updateQuantity = async (userId, cartItemId, newQuantity) => {
    if (!userId || !cartItemId || newQuantity === undefined) {
      return { success: false, message: '参数错误' }
    }
    
    // 如果数量为0或负数，则删除购物车项
    if (newQuantity <= 0) {
      console.log(`数量为${newQuantity}，调用删除接口`);
      return removeFromCart(userId, cartItemId);
    }
    
    try {
      const response = await cartAPI.updateCartItemQuantity(userId, cartItemId, newQuantity)
      console.log('更新购物车数量响应:', response);
      
      // 请求成功，刷新购物车数据
      await fetchCartData(userId)
      return { success: true, message: '数量已更新' }
    } catch (error) {
      console.error('更新购物车数量失败:', error)
      return { success: false, message: '更新数量失败，请重试' }
    }
  }

  // 严格调用后端API删除购物车商品
  const removeFromCart = async (userId, cartId) => {
    if (!userId || !cartId) return { success: false, message: '参数错误' }
    try {
      const response = await cartAPI.removeFromCart(userId, cartId)
      console.log('从购物车删除响应:', response);
      
      // 请求成功，直接刷新购物车数据
      await fetchCartData(userId)
      return { success: true, message: '已从购物车删除' }
    } catch (error) {
      console.error('从购物车删除失败:', error)
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
