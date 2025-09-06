<template>
  <div class="merchant-detail">
    <!-- 商家头部信息 -->
    <div class="merchant-header">
      <div class="header-bg"></div>
      <div class="header-content">
        <button @click="goBack" class="back-btn">
          <i class="back-icon">←</i>
        </button>
        
        <div v-if="merchantInfo" class="merchant-info-header">
          <div class="merchant-logo">
            <img
              :src="merchantInfo.logo || '/src/assets/placeholder.png'"
              :alt="merchantInfo.merchantName"
              @error="handleImageError"
            />
          </div>
          <div class="merchant-details">
            <h1 class="merchant-name">{{ merchantInfo.merchantName }}</h1>
            <div class="merchant-rating">
              <span class="rating-stars">⭐</span>
              <span class="rating-score">{{ merchantInfo.rating || 4.5 }}</span>
              <span class="rating-text">({{ merchantInfo.reviewCount || 200 }}条评价)</span>
            </div>
            <div class="merchant-meta">
              <span class="meta-item">🚚 {{ merchantInfo.deliveryTime || 30 }}分钟</span>
              <span class="meta-item">💰 配送费¥{{ merchantInfo.deliveryFee || 3 }}</span>
              <span class="meta-item">📦 起送¥{{ merchantInfo.minOrderAmount || 20 }}</span>
            </div>
            <div class="merchant-address">
              <span class="address-icon">📍</span>
              <span>{{ merchantInfo.location || '地址信息' }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 顶部分类导航 -->
    <div class="tab-navigation">
      <div 
        :class="['tab-item', { active: activeTab === 'menu' }]" 
        @click="switchTab('menu')"
      >
        点餐
      </div>
      <div 
        :class="['tab-item', { active: activeTab === 'reviews' }]" 
        @click="switchTab('reviews')"
      >
        评价
      </div>
    </div>

    <!-- 菜品内容区域 -->
    <div v-if="activeTab === 'menu'" class="content-wrapper">
      <!-- 分类侧边栏 -->
      <div class="categories-sidebar">
        <div class="categories-list">
          <div
            v-for="category in categories"
            :key="category.categoryId"
            :class="['category-item', { active: activeCategory === category.categoryId }]"
            @click="scrollToCategory(category.categoryId)"
          >
            <div class="category-text">
              <span class="category-name">{{ category.categoryName }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- 菜品展示区域 -->
      <div class="dishes-content" ref="dishesContent">
        <!-- 搜索栏 -->
        <div class="dishes-search">
          <div class="search-input-wrapper">
            <i class="search-icon">🔍</i>
            <input
              v-model="dishSearchKeyword"
              type="text"
              class="search-input"
              placeholder="搜索店内商品"
              @input="filterDishes"
            />
            <button v-if="dishSearchKeyword" @click="clearDishSearch" class="clear-btn">✕</button>
          </div>
        </div>

        <!-- 加载状态 -->
        <div v-if="loading" class="loading-container">
          <div class="loading-spinner"></div>
          <p>正在加载菜品信息...</p>
        </div>

        <!-- 菜品分类展示 -->
        <div v-else class="dishes-container">
          <div
            v-for="category in filteredCategories"
            :key="category.categoryId"
            :id="`category-${category.categoryId}`"
            class="category-section"
          >
            <div class="category-header">
              <h2 class="category-title">{{ category.categoryName }}</h2>
              <span class="category-desc">{{ category.description || '' }}</span>
            </div>

            <div class="dishes-grid">
              <div
                v-for="dish in category.dishes"
                :key="dish.dishId"
                class="dish-card"
                @click="showDishDetail(dish)"
              >
                <div class="dish-image-container">
                  <img
                    :src="dish.coverUrl"
                    :alt="dish.dishName"
                    class="dish-image"
                    @error="handleImageError"
                  />
                  <div v-if="dish.monthlySales" class="sales-badge">
                    月售{{ dish.monthlySales }}+
                  </div>
                  <div 
                    class="favorite-btn" 
                    @click.stop="toggleFavorite($event, dish)"
                    :class="{ 'is-favorite': isFavorite(dish.dishId) }"
                  >
                    <i class="favorite-icon">{{ isFavorite(dish.dishId) ? '❤️' : '🤍' }}</i>
                  </div>
                </div>

                <div class="dish-info">
                  <h3 class="dish-name">{{ dish.dishName }}</h3>
                  <p v-if="dish.description" class="dish-desc">{{ dish.description }}</p>
                  
                  <div class="dish-rating" v-if="dish.rating">
                    <span class="rating-stars">⭐</span>
                    <span class="rating-score">{{ dish.rating }}%</span>
                    <span class="rating-text">好评</span>
                  </div>

                  <div class="dish-price-section">
                    <div class="price-info">
                      <span class="current-price">¥{{ dish.price }}</span>
                      <span v-if="dish.originPrice && dish.originPrice > dish.price" class="original-price">
                        ¥{{ dish.originPrice }}
                      </span>
                    </div>
                    
                    <div class="quantity-controls">
                      <button
                        v-if="getCartQuantity(dish.dishId) > 0"
                        @click.stop="decreaseQuantity(dish)"
                        class="quantity-btn minus"
                      >
                        −
                      </button>
                      
                      <span v-if="getCartQuantity(dish.dishId) > 0" class="quantity">
                        {{ getCartQuantity(dish.dishId) }}
                      </span>
                      
                      <button
                        @click.stop="showDishDetail(dish)"
                        class="quantity-btn plus"
                      >
                        +
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- 空状态 -->
          <div v-if="filteredCategories.length === 0" class="empty-dishes">
            <div class="empty-icon">🍽️</div>
            <p>暂无相关菜品</p>
          </div>
        </div>
      </div>
    </div>

    <!-- 评价内容区域 -->
    <div v-if="activeTab === 'reviews'" class="reviews-wrapper">
      <div class="reviews-content">
        <!-- 加载状态 -->
        <div v-if="reviewsLoading" class="loading-container">
          <div class="loading-spinner"></div>
          <p>正在加载评价信息...</p>
        </div>

        <!-- 评价列表 -->
        <div v-else-if="reviews.length > 0" class="reviews-list">
          <div v-for="review in reviews" :key="review.reviewId" class="review-item">
            <div class="review-header">
              <div class="user-info">
                <img 
                  :src="review.userAvatarUrl ? `/api/uploads/${review.userAvatarUrl}` : '/src/assets/placeholder.png'" 
                  :alt="review.userNickname || '匿名用户'" 
                  class="user-avatar"
                  @error="handleReviewImageError"
                />
                <div class="user-details">
                  <div class="user-name">{{ review.isAnonymous ? '匿名用户' : (review.userNickname || '用户') }}</div>
                  <div class="review-date">{{ formatReviewTime(review.reviewAt) }}</div>
                </div>
              </div>
              <div class="review-rating">
                <span v-for="i in 5" :key="i" :class="['star', { filled: i <= review.rating }]">★</span>
              </div>
            </div>
            <div class="review-content">
              {{ review.content }}
            </div>
          </div>
        </div>
        
        <!-- 加载更多 -->
        <div v-if="hasMoreReviews && !reviewsLoading" class="load-more-reviews">
          <button @click="loadMoreReviews" class="load-more-btn">加载更多评价</button>
        </div>

        <!-- 空状态 -->
        <div v-if="!reviewsLoading && reviews.length === 0" class="empty-reviews">
          <div class="empty-icon">⭐</div>
          <p>暂无评价</p>
        </div>
      </div>
    </div>

    <!-- 购物车悬浮按钮 -->
    <div class="cart-float" @click="showCart">
      <div class="cart-icon">🛒</div>
      <div class="cart-info">
        <div class="cart-count">
          {{ cartItems.length > 0 ? cartItems.reduce((sum, item) => sum + item.quantity, 0) : 0 }}
        </div>
        <div class="cart-total">
          ¥{{ cartItems.length > 0 ? cartItems.reduce((sum, item) => sum + item.subtotal, 0).toFixed(2) : '0.00' }}
        </div>
      </div>
    </div>

    <!-- 菜品详情弹窗 -->
    <div v-if="selectedDish" class="dish-modal" @click="closeDishDetail">
      <div class="modal-content" @click.stop>
        <button @click="closeDishDetail" class="modal-close">✕</button>
        <div class="modal-header">
          <img
            :src="selectedDish.coverUrl || '/api/placeholder/300/200'"
            :alt="selectedDish.dishName"
            class="modal-dish-image"
          />
        </div>
        <div class="modal-body">
          <h2 class="modal-dish-name">{{ selectedDish.dishName }}</h2>
          <p v-if="selectedDish.description" class="modal-dish-desc">{{ selectedDish.description }}</p>
          
          <div class="modal-dish-meta">
            <div v-if="selectedDish.rating" class="modal-rating">
              <span class="rating-stars">⭐</span>
              <span>{{ selectedDish.rating }}% 好评</span>
            </div>
            <div v-if="selectedDish.monthlySales" class="modal-sales">
              月售 {{ selectedDish.monthlySales }}+
            </div>
          </div>

          <div class="modal-price-section">
            <div class="modal-price">
              <span class="modal-current-price">¥{{ selectedDish.price }}</span>
              <span v-if="selectedDish.originPrice && selectedDish.originPrice > selectedDish.price" class="modal-original-price">
                ¥{{ selectedDish.originPrice }}
              </span>
            </div>
            
            <div class="modal-quantity-controls">
              <button
                v-if="getCartQuantity(selectedDish.dishId) > 0"
                @click="decreaseQuantity(selectedDish)"
                class="modal-quantity-btn minus"
              >
                −
              </button>
              
              <span v-if="getCartQuantity(selectedDish.dishId) > 0" class="modal-quantity">
                {{ getCartQuantity(selectedDish.dishId) }}
              </span>
              
              <button
                @click="handleAddToCart"
                class="modal-quantity-btn plus"
              >
                +
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 购物车弹窗 -->
    <div v-if="showCartModal" class="cart-modal" @click="closeCart">
      <div class="cart-modal-content" @click.stop>
        <div class="cart-header">
          <h2 class="cart-title">购物车</h2>
          <button @click="closeCart" class="cart-close">✕</button>
        </div>
        
        <div class="cart-body">
          <div v-if="cartItems.length === 0" class="empty-cart">
            <div class="empty-icon">🛒</div>
            <p>购物车是空的</p>
            <p>去挑选一些美味的商品吧</p>
          </div>
          
          <div v-else class="cart-items">
            <div 
              v-for="item in cartItems"
              :key="item.dishId"
              class="cart-item"
            >
              <div class="item-image">
                <img 
                  :src="item.coverUrl || '/api/placeholder/60/60'"
                  :alt="item.dishName"
                />
              </div>
              
              <div class="item-details">
                <div class="item-name">{{ item.dishName }}</div>
                <div class="item-price">¥{{ item.price }}</div>
              </div>
              
              <div class="item-controls">
                <button 
                  @click="decreaseQuantity({ dishId: item.dishId, merchantId: item.merchantId })"
                  class="cart-quantity-btn minus"
                >
                  −
                </button>
                <span class="cart-quantity">{{ item.quantity }}</span>
                <button
                  @click.stop="increaseQuantity({ dishId: item.dishId, merchantId: item.merchantId })"
                  class="cart-quantity-btn plus"
                >
                  +
                </button>
              </div>
              
              <div class="item-subtotal">
                ¥{{ (item.price * item.quantity).toFixed(2) }}
              </div>
            </div>
          </div>
        </div>
        
        <div v-if="cartItems.length > 0" class="cart-footer">
          <div class="cart-summary">
            <div class="summary-line">
              <span>商品总额</span>
              <span>¥{{ cartItems.reduce((sum, item) => sum + item.subtotal, 0).toFixed(2) }}</span>
            </div>
            <div class="summary-line">
              <span>配送费</span>
              <span>¥{{ deliveryFee.toFixed(2) }}</span>
            </div>
            <div class="summary-line total">
              <span>合计</span>
              <span>¥{{ (cartItems.reduce((sum, item) => sum + item.subtotal, 0) + deliveryFee).toFixed(2) }}</span>
            </div>
          </div>
          
          <button 
            @click="proceedToCheckout"
            :disabled="cartItems.reduce((sum, item) => sum + item.subtotal, 0) < minOrderAmount"
            class="checkout-btn"
          >
            <span v-if="cartItems.reduce((sum, item) => sum + item.subtotal, 0) < minOrderAmount">
              还差¥{{ (minOrderAmount - cartItems.reduce((sum, item) => sum + item.subtotal, 0)).toFixed(2) }}起送
            </span>
            <span v-else>
              去结算 ({{ cartItems.reduce((sum, item) => sum + item.quantity, 0) }})
            </span>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, reactive, onMounted, computed, nextTick } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { merchantAPI, dishAPI } from '@/api/browse.js'
import { getCartInstance } from '@/composables/useCart.js'
import { favoriteAPI } from '@/api/user.js'
import { merchantAPI as merchantAPINew } from '@/api/merchant.js'

export default {
  name: 'MerchantDetail',
  setup() {
    function handleAddToCart() {
      const userId = (typeof window !== 'undefined' && window.localStorage && window.localStorage.getItem)
        ? window.localStorage.getItem('userId') || ''
        : '';
      console.log('调试 localStorage userId:', userId);
      
      // 检查是否已在购物车中
      if (getCartQuantity(selectedDish.value.dishId) > 0) {
        // 如果已经在购物车中，增加数量
        increaseQuantity(selectedDish.value);
      } else {
        // 如果不在购物车中，添加到购物车
        const params = { 
          dishId: selectedDish.value.dishId,
          userId: userId,
          merchantId: route.params.id
        };
        console.log('addToCart 按钮传入参数:', params);
        addToCart(params);
      }
    }
  // setup 阶段调试输出
  console.log('setup 阶段 localStorage userId:', localStorage.getItem('userId'))
    const route = useRoute()
    const router = useRouter()
    
    // 使用购物车组合式函数
    const cart = getCartInstance()
    
    // 响应式数据
    const merchantInfo = ref(null)
    const categories = ref([])
    const allDishes = ref([])
    const loading = ref(false)
    const activeCategory = ref('')
    const activeTab = ref('menu') // 添加activeTab状态，默认显示菜单
    const dishSearchKeyword = ref('')
    const selectedDish = ref(null)
    const showCartModal = ref(false)
    const dishesContent = ref(null)
    const favoriteDishes = ref({}) // 存储收藏状态，key为dishId，value为收藏ID
    
    // 评价相关数据
    const reviews = ref([])
    const reviewsLoading = ref(false)
    const currentReviewPage = ref(1)
    const reviewPageSize = ref(10)
    const hasMoreReviews = ref(true)

    // 商家配送信息
    const deliveryFee = ref(3.5)
    const minOrderAmount = ref(20)

    // 计算属性
    const filteredCategories = computed(() => {
      if (!dishSearchKeyword.value) {
        return categories.value
      }
      
      return categories.value.map(category => ({
        ...category,
        dishes: category.dishes.filter(dish =>
          dish.dishName.toLowerCase().includes(dishSearchKeyword.value.toLowerCase())
        )
      })).filter(category => category.dishes.length > 0)
    })

    const totalCartItems = computed(() => {
      return cart.totalItems.value
    })

    const totalCartPrice = computed(() => {
      return cart.totalAmount.value
    })

    const cartItems = computed(() => {
      // 只返回当前商家的商品
      return cart.cartItems.value.filter(item => item.merchantId === route.params.id)
    })

    // 方法
    const fetchMerchantInfo = async () => {
      try {
        const response = await merchantAPI.getMerchantById(route.params.id)
        console.log('商家信息响应:', response)
        
        // 检查 response 是否已经是对象（已经被 axios 拦截器处理）
        if (response && typeof response === 'object' && !response.code) {
          merchantInfo.value = response
          console.log('处理后的商家信息(直接对象):', merchantInfo.value)
        }
        // 如果 response 包含 data 字段，说明 axios 拦截器没有处理它
        else if (response && response.data) {
          merchantInfo.value = response.data
          console.log('处理后的商家信息(从 response.data):', merchantInfo.value)
        } else {
          console.error('获取商家信息格式不正确:', response)
          merchantInfo.value = null
        }
      } catch (error) {
        console.error('获取商家信息失败:', error)
        merchantInfo.value = null
      }
    }
    
    // 获取评价列表
    const fetchReviews = async (isLoadMore = false) => {
      if (reviewsLoading.value) return
      
      reviewsLoading.value = true
      try {
        const response = await merchantAPINew.getMerchantReviews(
          route.params.id, 
          currentReviewPage.value, 
          reviewPageSize.value
        )
        
        console.log('评价数据响应:', response)
        
        if (response && Array.isArray(response)) {
          // 如果是加载更多，则追加数据
          if (isLoadMore) {
            reviews.value = [...reviews.value, ...response]
          } else {
            reviews.value = response
          }
          
          // 判断是否还有更多数据
          hasMoreReviews.value = response.length === reviewPageSize.value
        } else if (response && response.data && Array.isArray(response.data)) {
          // 如果是加载更多，则追加数据
          if (isLoadMore) {
            reviews.value = [...reviews.value, ...response.data]
          } else {
            reviews.value = response.data
          }
          
          // 判断是否还有更多数据
          hasMoreReviews.value = response.data.length === reviewPageSize.value
        } else {
          console.error('获取评价数据格式不正确:', response)
          if (!isLoadMore) {
            reviews.value = []
          }
          hasMoreReviews.value = false
        }
      } catch (error) {
        console.error('获取评价失败:', error)
        if (!isLoadMore) {
          reviews.value = []
        }
        hasMoreReviews.value = false
      } finally {
        reviewsLoading.value = false
      }
    }
    
    // 加载更多评价
    const loadMoreReviews = async () => {
      currentReviewPage.value++
      await fetchReviews(true)
    }
    
    // 格式化评价时间
    const formatReviewTime = (timestamp) => {
      if (!timestamp) return '未知时间'
      
      try {
        const reviewDate = new Date(timestamp)
        const now = new Date()
        
        // 计算时间差（毫秒）
        const diff = now - reviewDate
        
        // 转换为秒
        const diffSeconds = Math.floor(diff / 1000)
        
        // 小于1分钟
        if (diffSeconds < 60) {
          return '刚刚'
        }
        
        // 小于1小时
        if (diffSeconds < 3600) {
          return `${Math.floor(diffSeconds / 60)}分钟前`
        }
        
        // 小于24小时
        if (diffSeconds < 86400) {
          return `${Math.floor(diffSeconds / 3600)}小时前`
        }
        
        // 小于30天
        if (diffSeconds < 2592000) {
          return `${Math.floor(diffSeconds / 86400)}天前`
        }
        
        // 大于30天，显示具体日期
        const year = reviewDate.getFullYear()
        const month = String(reviewDate.getMonth() + 1).padStart(2, '0')
        const day = String(reviewDate.getDate()).padStart(2, '0')
        
        return `${year}-${month}-${day}`
      } catch (error) {
        console.error('格式化评价时间失败:', error)
        return '未知时间'
      }
    }
    
    // 图片加载错误处理
    const handleReviewImageError = (event) => {
      event.target.src = '/src/assets/placeholder.png'
    }
    
    // 切换标签页
    const switchTab = async (tab) => {
      activeTab.value = tab
      
      // 如果切换到评价标签，并且还没有加载过评价数据，则加载评价数据
      if (tab === 'reviews' && reviews.value.length === 0 && !reviewsLoading.value) {
        await fetchReviews()
      }
    }

    const fetchDishes = async () => {
      loading.value = true
      try {
        console.log('开始获取商家菜品，商家ID:', route.params.id)
        // 添加网络请求调试
        const response = await merchantAPI.getAllDishes(route.params.id)
        console.log('获取菜品接口响应:', response)
        
        // 检查 response 是否是数组（已经被 axios 拦截器处理）
        if (Array.isArray(response)) {
          allDishes.value = response
          console.log('获取到的菜品数据:', response)
          organizeDishesIntoCategories(response)
        } 
        // 如果 response 包含 data 字段，说明 axios 拦截器没有处理它
        else if (response && response.data) {
          allDishes.value = response.data
          console.log('获取到的菜品数据 (从 response.data):', response.data)
          organizeDishesIntoCategories(response.data)
        } else {
          console.error('获取菜品数据格式不正确:', response)
          allDishes.value = []
          dishCategories.value = []
        }
      } catch (error) {
        console.error('获取菜品信息失败:', error)
        allDishes.value = []
        dishCategories.value = []
      } finally {
        loading.value = false
      }
    }

    const fetchCategories = async () => {
      try {
        // 获取商家ID，优先用route.params.id
        const merchantId = route?.params?.id
        if (!merchantId) {
          console.error('商家ID不存在，无法获取分类');
          return [];
        }
        const response = await dishAPI.getCategories(merchantId)
        console.log('获取分类接口响应:', response)
        
        // 检查 response 是否是数组（已经被 axios 拦截器处理）
        if (Array.isArray(response)) {
          console.log('获取到的分类数据 (数组):', response)
          return response
        }
        // 如果 response 包含 data 字段，说明 axios 拦截器没有处理它
        else if (response && response.data) {
          console.log('获取到的分类数据 (从 response.data):', response.data)
          return response.data
        } else {
          console.error('获取分类数据格式不正确:', response)
          return []
        }
      } catch (error) {
        console.error('获取分类信息失败:', error)
      }
      
      // 如果无法获取分类数据，返回空数组
      return []
    }

    const organizeDishesIntoCategories = async (dishes) => {
      const categoriesData = await fetchCategories()
      // 修改normalizeId函数，更严格地处理categoryId
      const normalizeId = id => {
        if (!id) return '';
        // 将id转为字符串，去除所有空白，只保留数字部分
        return String(id).trim().replace(/\s+/g, '');
      }
      const categoryMap = new Map()
      
      // 调试输出
      console.log('原始菜品数据示例:', dishes.slice(0, 2));
      console.log('原始分类数据:', categoriesData);
      
      // 初始化分类
      categoriesData.forEach(cat => {
        const normalizedCatId = normalizeId(cat.categoryId);
        console.log(`分类ID处理: 原始=${cat.categoryId}, 处理后=${normalizedCatId}`);
        categoryMap.set(normalizedCatId, {
          ...cat,
          categoryId: normalizedCatId, // 使用处理后的ID
          dishes: [],
          dishCount: 0
        })
      })
      // 将菜品分配到对应分类
      dishes.forEach(dish => {
        const catId = normalizeId(dish.categoryId)
        if (categoryMap.has(catId)) {
          categoryMap.get(catId).dishes.push(dish)
          categoryMap.get(catId).dishCount++
        } else {
          // 没有分类的菜品也显示出来
          if (!categoryMap.has('other')) {
            categoryMap.set('other', {
              categoryId: 'other',
              categoryName: '其他',
              dishes: [],
              dishCount: 0
            })
          }
          categoryMap.get('other').dishes.push(dish)
          categoryMap.get('other').dishCount++
        }
      })
      // 过滤掉没有菜品的分类
      categories.value = Array.from(categoryMap.values()).filter(cat => cat.dishCount > 0)
      if (categories.value.length > 0) {
        activeCategory.value = categories.value[0].categoryId
      }
      // 调试输出
      console.log('最终分类数据:', categories.value)
    }

    const scrollToCategory = (categoryId) => {
      activeCategory.value = categoryId
      const element = document.getElementById(`category-${categoryId}`)
      if (element) {
        element.scrollIntoView({ behavior: 'smooth', block: 'start' })
      }
    }

    const filterDishes = () => {
      // 筛选逻辑已在计算属性中实现
    }

    const clearDishSearch = () => {
      dishSearchKeyword.value = ''
    }

    const showDishDetail = async (dish) => {
      selectedDish.value = dish
      
      // 检查菜品收藏状态
      const userId = localStorage.getItem('userId')
      if (userId) {
        try {
          // 检查收藏状态
          const response = await favoriteAPI.checkFavoriteStatus(userId, dish.dishId)
          if (response && response.data === true) {
            favoriteDishes.value[dish.dishId] = true
          } else {
            favoriteDishes.value[dish.dishId] = false
          }
          
          // 检查购物车状态
          const quantity = getCartQuantity(dish.dishId)
          if (quantity > 0) {
            console.log(`打开详情: 菜品 ${dish.dishName} 在购物车中，数量: ${quantity}`)
          }
        } catch (error) {
          console.error(`检查菜品 ${dish.dishId} 状态失败:`, error)
        }
      }
    }

    const closeDishDetail = () => {
      selectedDish.value = null
    }

    const getCartQuantity = (dishId) => {
      return cart.getItemQuantity(dishId, route.params.id)
    }

    const addToCart = async (dish) => {
      console.log('addToCart 传入参数:', dish);
      
      // 确保获取必要的参数
      const userId = dish.userId || (typeof window !== 'undefined' && window.localStorage && window.localStorage.getItem ? window.localStorage.getItem('userId') || '' : '');
      const dishId = dish.dishId;
      const merchantId = dish.merchantId || route.params.id;
      
      // 检查必要参数
      if (!userId || !dishId || !merchantId) {
        console.error('缺少必要参数:', { userId, dishId, merchantId });
        return { success: false, message: '参数错误' };
      }
      
      // 符合cart.js接口的参数结构
      const cartParams = {
        userId,
        dishId,
        merchantId
      };
      
      console.log('addToCart 最终参数:', cartParams);
      const result = await cart.addToCart(cartParams);
      console.log('addToCart 返回结果:', result);
      if (result.success) {
        // 加入成功后刷新购物车数据
        if (typeof cart.fetchCartData === 'function') {
          await cart.fetchCartData(userId);
        }
        // 可以显示成功提示
        console.log(result.message);
      } else {
        console.error(result.message);
        alert(result.message);
      }
      return result;
    }

    const removeFromCart = async (dish) => {
      // 支持弹窗和详情页都能正确传 merchantId
      const merchantId = dish.merchantId || route.params.id;
      const cartItem = cart.cartItems.value.find(
        item => item.dishId === dish.dishId && item.merchantId === merchantId
      );
      if (cartItem) {
        await cart.removeFromCart(cartItem.cartItemId);
      }
    }
    
    // 减少购物车数量
    const decreaseQuantity = async (dish) => {
      // 获取必要参数
      const userId = localStorage.getItem('userId');
      const merchantId = dish.merchantId || route.params.id;
      
      // 查找购物车中的项目
      const cartItem = cart.cartItems.value.find(
        item => item.dishId === dish.dishId && item.merchantId === merchantId
      );
      
      if (!cartItem) {
        console.error('商品不在购物车中');
        return;
      }
      
      // 如果数量为1，则直接删除
      if (cartItem.quantity <= 1) {
        await removeFromCart(dish);
        return;
      }
      
      // 减少数量
      const newQuantity = cartItem.quantity - 1;
      console.log(`减少商品数量: dishId=${dish.dishId}, cartId=${cartItem.cartItemId}, 新数量=${newQuantity}`);
      
      try {
        // 使用购物车实例的更新数量方法
        await cart.updateQuantity(userId, cartItem.cartItemId, newQuantity);
      } catch (error) {
        console.error('减少商品数量失败:', error);
      }
    }
    
    // 增加购物车数量
    const increaseQuantity = async (dish) => {
      // 获取必要参数
      const userId = localStorage.getItem('userId');
      const merchantId = dish.merchantId || route.params.id;
      
      // 查找购物车中的项目
      let cartItem = cart.cartItems.value.find(
        item => item.dishId === dish.dishId && item.merchantId === merchantId
      );
      
      // 如果不在购物车中，先添加
      if (!cartItem) {
        await addToCart(dish);
        return;
      }
      
      // 增加数量
      const newQuantity = cartItem.quantity + 1;
      console.log(`增加商品数量: dishId=${dish.dishId}, cartId=${cartItem.cartItemId}, 新数量=${newQuantity}`);
      
      try {
        // 使用购物车实例的更新数量方法
        await cart.updateQuantity(userId, cartItem.cartItemId, newQuantity);
      } catch (error) {
        console.error('增加商品数量失败:', error);
      }
    }

    const showCart = async () => {
      // 每次点开购物车都强制刷新当前用户购物车数据
      const userId = localStorage.getItem('userId');
      if (userId && typeof cart.fetchCartData === 'function') {
        await cart.fetchCartData(userId);
      }
      // 打印所有购物车项（不做商家过滤）
      console.log('全部购物车项:', cart.cartItems.value);
      showCartModal.value = true;
    }

    const closeCart = () => {
      showCartModal.value = false
    }

    const proceedToCheckout = () => {
      const currentMerchantItems = cartItems.value
      
      if (currentMerchantItems.length === 0) {
        alert('购物车为空')
        return
      }

      const totalAmount = currentMerchantItems.reduce((total, item) => total + item.subtotal, 0)
      
      if (totalAmount < minOrderAmount.value) {
        alert(`还差¥${(minOrderAmount.value - totalAmount).toFixed(2)}起送`)
        return
      }

      // 准备订单数据
      const userId = localStorage.getItem('userId')
      if (!userId) {
        alert('请先登录')
        return
      }
      
      const orderData = {
        userId: userId,
        merchantId: route.params.id,
        merchantName: merchantInfo.value?.merchantName,
        items: currentMerchantItems.map(item => ({
          dishId: item.dishId,
          dishName: item.dishName,
          price: item.price,
          quantity: item.quantity,
          subtotal: item.subtotal,
          coverUrl: item.dishImage
        })),
        totalAmount: totalAmount,
        deliveryFee: deliveryFee.value,
        finalAmount: totalAmount + deliveryFee.value
      }

      // 同时保存到localStorage作为备用
      localStorage.setItem('cartItems', JSON.stringify(orderData.items))
      localStorage.setItem('currentMerchantId', route.params.id)
      localStorage.setItem('currentMerchantName', merchantInfo.value?.merchantName || '餐厅')
      // 确保已保存userId
      if (userId) {
        localStorage.setItem('userId', userId)
      }

      // 跳转到结算页面
      router.push({
        name: 'Checkout',
        query: {
          data: JSON.stringify(orderData)
        }
      })
    }

    const goBack = () => {
      router.back()
    }

    const handleImageError = (event) => {
      event.target.src = '/src/assets/placeholder.png'
    }
    
    // 获取用户收藏列表
    const fetchUserFavorites = async () => {
      try {
        const userId = localStorage.getItem('userId')
        if (!userId) {
          console.log('用户未登录，无法获取收藏列表')
          return
        }
        
        const response = await favoriteAPI.getUserFavorites(userId)
        console.log('获取用户收藏列表:', response)
        
        // 将收藏数据转换为便于查询的格式
        const favorites = {}
        
        if (response && response.data && Array.isArray(response.data)) {
          response.data.forEach(item => {
            favorites[item.dishId] = true // 直接存储收藏状态为true
          })
        }
        
        favoriteDishes.value = favorites
        console.log('处理后的收藏数据:', favoriteDishes.value)
      } catch (error) {
        console.error('获取收藏列表失败:', error)
      }
    }
    
    // 切换收藏状态
    const toggleFavorite = async (event, dish) => {
      event.stopPropagation() // 阻止事件冒泡，避免触发点击菜品的事件
      
      const userId = localStorage.getItem('userId')
      if (!userId) {
        console.log('用户未登录，无法收藏')
        return
      }
      
      const dishId = dish.dishId
      
      try {
        // 先检查当前收藏状态
        const checkResponse = await favoriteAPI.checkFavoriteStatus(userId, dishId)
        const isFavorited = checkResponse && checkResponse.data === true
        
        if (isFavorited) {
          // 已收藏，取消收藏
          await favoriteAPI.removeFavorite(userId, dishId)
          favoriteDishes.value[dishId] = false
          console.log('取消收藏成功:', dishId)
        } else {
          // 未收藏，添加收藏
          const response = await favoriteAPI.addToFavorites(userId, dishId)
          if (response) {
            favoriteDishes.value[dishId] = true
            console.log('添加收藏成功:', response)
          }
        }
      } catch (error) {
        console.error('收藏操作失败:', error)
      }
    }
    
    // 检查菜品是否已收藏
    const isFavorite = (dishId) => {
      return favoriteDishes.value[dishId] === true
    }
    
    // 获取单个菜品的收藏状态
    const checkDishFavoriteStatus = async (dishId) => {
      try {
        const userId = localStorage.getItem('userId')
        if (!userId) return false
        
        const response = await favoriteAPI.checkFavoriteStatus(userId, dishId)
        return response && response.data === true
      } catch (error) {
        console.error('检查菜品收藏状态失败:', error)
        return false
      }
    }

    // 生命周期
    onMounted(async () => {
      // onMounted 阶段调试输出
      console.log('onMounted 阶段 localStorage userId:', localStorage.getItem('userId'))
      
      // 先获取购物车数据
      const userId = localStorage.getItem('userId')
      if (userId) {
        await cart.fetchCartData(userId)
      }
      
      // 获取商家和菜品信息
      await fetchMerchantInfo()
      await fetchDishes()
      
      // 检查收藏状态
      await fetchUserFavorites()
      
      // 使用新API检查每个菜品的收藏状态
      if (userId) {
        // 遍历所有分类和菜品
        for (const category of categories.value) {
          for (const dish of category.dishes) {
            try {
              // 检查收藏状态
              const response = await favoriteAPI.checkFavoriteStatus(userId, dish.dishId)
              // 更新收藏状态
              if (response && response.data === true) {
                favoriteDishes.value[dish.dishId] = true
              }
              
              // 检查购物车状态
              const quantity = getCartQuantity(dish.dishId)
              // 如果菜品在购物车中，触发更新
              if (quantity > 0) {
                console.log(`菜品 ${dish.dishName} 在购物车中，数量: ${quantity}`)
              }
            } catch (error) {
              console.error(`检查菜品 ${dish.dishId} 状态失败:`, error)
            }
          }
        }
      }
    })

    return {
      merchantInfo,
      categories,
      allDishes,
      loading,
      activeCategory,
      activeTab, // 添加activeTab到返回值中
      dishSearchKeyword,
      selectedDish,
      cartItems,
      showCartModal,
      dishesContent,
      deliveryFee,
      minOrderAmount,
      filteredCategories,
      totalCartItems,
      totalCartPrice,
      // 评价相关数据和方法
      reviews,
      reviewsLoading,
      hasMoreReviews,
      fetchReviews,
      loadMoreReviews,
      formatReviewTime,
      handleReviewImageError,
      switchTab,
      // 原有的方法
      fetchMerchantInfo,
      fetchDishes,
      favoriteDishes,
      toggleFavorite,
      isFavorite,
      checkDishFavoriteStatus,
      scrollToCategory,
      filterDishes,
      clearDishSearch,
      showDishDetail,
      closeDishDetail,
      getCartQuantity,
      addToCart,
      removeFromCart,
      increaseQuantity,
      decreaseQuantity,
      showCart,
      closeCart,
      proceedToCheckout,
      goBack,
      handleImageError,
      handleAddToCart
    }
  }
}
</script>

<style scoped>
.tab-navigation {
  display: flex;
  background: white;
  border-bottom: 1px solid #e1e5e9;
  margin-bottom: 1px;
  position: sticky;
  top: 0;
  z-index: 100;
}

.tab-item {
  flex: 1;
  text-align: center;
  padding: 16px 0;
  font-size: 15px;
  font-weight: 500;
  color: #666;
  cursor: pointer;
  transition: all 0.3s ease;
  position: relative;
}

.tab-item.active {
  color: #007BFF;
}

.tab-item.active::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 50%;
  transform: translateX(-50%);
  width: 30%;
  height: 3px;
  background-color: #007BFF;
  border-radius: 3px;
}

/* 评价区域样式 */
.reviews-wrapper {
  flex: 1;
  overflow-y: auto;
  background: #f8f9fa;
  position: relative;
}

.reviews-content {
  padding: 16px;
}

.reviews-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.review-item {
  background: white;
  border-radius: 8px;
  padding: 16px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
}

.review-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 12px;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 12px;
}

.user-avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  object-fit: cover;
  background-color: #f0f0f0;
}

.user-details {
  display: flex;
  flex-direction: column;
}

.user-name {
  font-weight: 500;
  color: #333;
}

.review-date {
  font-size: 12px;
  color: #999;
  margin-top: 2px;
}

.review-rating {
  display: flex;
}

.star {
  color: #e0e0e0;
  font-size: 16px;
  margin-left: 2px;
}

.star.filled {
  color: #FF9500;
}

.review-content {
  font-size: 14px;
  line-height: 1.5;
  color: #333;
  word-break: break-word;
}

.load-more-reviews {
  text-align: center;
  margin-top: 24px;
  margin-bottom: 16px;
}

.load-more-btn {
  background: white;
  border: 1px solid #e1e5e9;
  padding: 8px 24px;
  border-radius: 20px;
  font-size: 14px;
  color: #666;
  cursor: pointer;
  transition: all 0.3s ease;
}

.load-more-btn:hover {
  background: #f5f5f5;
}

.empty-reviews {
  text-align: center;
  padding: 40px 0;
  color: #999;
}

.empty-reviews .empty-icon {
  font-size: 48px;
  margin-bottom: 16px;
  color: #e0e0e0;
}

/* 商家头部样式 */
.merchant-header {
  position: relative;
  height: 200px;
  background: linear-gradient(135deg, #007BFF 0%, #00D4FF 50%, #40E0D0 100%);
  overflow: hidden;
}

.header-bg {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: url('https://picsum.photos/1200/200?random=header') center/cover;
  opacity: 0.3;
}

.header-content {
  position: relative;
  height: 100%;
  display: flex;
  align-items: center;
  padding: 20px;
  max-width: 1200px;
  margin: 0 auto;
}

.back-btn {
  position: absolute;
  top: 20px;
  left: 20px;
  width: 40px;
  height: 40px;
  background: rgba(255, 255, 255, 0.9);
  border: none;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  font-size: 18px;
  color: #333;
  transition: all 0.3s ease;
}

.back-btn:hover {
  background: white;
  transform: scale(1.1);
}

.merchant-info-header {
  display: flex;
  align-items: center;
  gap: 20px;
  width: 100%;
  margin-top: 60px;
  min-height: 120px;
}

.merchant-logo {
  width: 80px;
  height: 80px;
  border-radius: 12px;
  overflow: hidden;
  border: 3px solid white;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
}

.merchant-logo img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.merchant-details {
  flex: 1;
  color: white;
}

.merchant-name {
  font-size: 28px;
  font-weight: 700;
  margin: 0 0 8px 0;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
}

.merchant-rating {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 12px;
}

.rating-stars {
  font-size: 16px;
}

.rating-score {
  font-size: 16px;
  font-weight: 600;
}

.rating-text {
  font-size: 14px;
  opacity: 0.9;
}

.merchant-meta {
  display: flex;
  gap: 20px;
  margin-bottom: 8px;
  flex-wrap: wrap;
}

.meta-item {
  font-size: 14px;
  opacity: 0.9;
}

.merchant-address {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 14px;
  opacity: 0.9;
}

/* 内容区域样式 */
.content-wrapper {
  display: flex;
  max-width: 1200px;
  margin: 0 auto;
  min-height: calc(100vh - 200px);
}

/* 分类侧边栏样式 */
.categories-sidebar {
  width: 120px;
  background: #f5f7fa;
  border-right: 1px solid #e9ecef;
  position: sticky;
  top: 50px; /* 让分类栏位于tab-navigation下方 */
  height: calc(100vh - 20px); /* 固定高度 */
  overflow-y: auto;
  padding-bottom: 80px;
  border-radius: 0 0 8px 0;
  z-index: 10;
}

.categories-list {
  padding: 20px 8px;
}

.category-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 12px 16px;
  cursor: pointer;
  transition: all 0.3s ease;
  border-left: 3px solid transparent;
  background: white;
  color: #333;
  border-radius: 6px;
  margin: 4px 0;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
}

.category-item:hover {
  background: #f8f9fa;
  box-shadow: 0 3px 6px rgba(0, 0, 0, 0.1);
}

.category-item.active {
  background: #007BFF;
  border-left-color: #0056b3;
  color: white;
  box-shadow: 0 3px 6px rgba(0, 123, 255, 0.3);
}

.category-text {
  display: flex;
  align-items: center;
  justify-content: center;
  flex-wrap: wrap;
  padding: 4px 0;
}

.category-name {
  font-size: 14px;
  text-align: center;
  line-height: 1.2;
  margin-bottom: 0;
  font-weight: 500;
}

/* 菜品内容区域样式 */
.dishes-content {
  flex: 1;
  padding: 20px;
  background: white;
}

.dishes-search {
  margin-bottom: 24px;
  position: sticky;
  top: 50px; /* 让搜索栏位于tab-navigation下方 */
  background: white;
  z-index: 10;
  padding-bottom: 16px;
}

.search-input-wrapper {
  position: relative;
  max-width: 400px;
}

.search-icon {
  position: absolute;
  left: 12px;
  top: 50%;
  transform: translateY(-50%);
  font-size: 14px;
  color: #999;
}

.search-input {
  width: 100%;
  padding: 10px 40px 10px 36px;
  border: 2px solid #e1e5e9;
  border-radius: 20px;
  font-size: 14px;
  background-color: #fff;
  color: #333;
  transition: all 0.3s ease;
}

.search-input:focus {
  outline: none;
  border-color: #007BFF;
}

.clear-btn {
  position: absolute;
  right: 12px;
  top: 50%;
  transform: translateY(-50%);
  background: none;
  border: none;
  color: #999;
  cursor: pointer;
  font-size: 16px;
}

.loading-container {
  text-align: center;
  padding: 60px 20px;
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #e1e5e9;
  border-top: 4px solid #007BFF;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 16px;
}

.category-section {
  margin-bottom: 40px;
}

.category-header {
  margin-bottom: 16px;
  padding-bottom: 8px;
  border-bottom: 2px solid #e9ecef;
}

.category-title {
  font-size: 20px;
  font-weight: 600;
  color: #333;
  margin: 0 0 4px 0;
}

.category-desc {
  font-size: 14px;
  color: #666;
}

.dishes-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 16px;
}

/* 菜品卡片样式 */
.dish-card {
  display: flex;
  background: white;
  border: 1px solid #e9ecef;
  border-radius: 12px;
  overflow: hidden;
  transition: all 0.3s ease;
  cursor: pointer;
}

.dish-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  transform: translateY(-2px);
}

.dish-image-container {
  position: relative;
  width: 90px;
  height: 90px; /* 增加50px高度 */
  flex-shrink: 0;
}

.dish-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.sales-badge {
  position: absolute;
  bottom: 4px;
  left: 4px;
  background: rgba(0, 0, 0, 0.7);
  color: white;
  padding: 2px 6px;
  border-radius: 8px;
  font-size: 10px;
}

.favorite-btn {
  position: absolute;
  top: 4px;
  right: 4px;
  width: 30px;
  height: 30px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.8);
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.2s ease;
  z-index: 10;
}

.favorite-btn:hover {
  transform: scale(1.1);
  background: rgba(255, 255, 255, 0.95);
}

.favorite-btn.is-favorite {
  background: rgba(255, 241, 241, 0.9);
}

.favorite-icon {
  font-size: 18px;
}

.dish-info {
  flex: 1;
  padding: 12px;
  display: flex;
  flex-direction: column;
}

.dish-name {
  font-size: 16px;
  font-weight: 600;
  color: #333;
  margin: 0 0 6px 0;
  line-height: 1.3;
}

.dish-desc {
  font-size: 12px;
  color: #666;
  margin: 0 0 8px 0;
  line-height: 1.4;
  overflow: hidden;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  line-clamp: 2;
  -webkit-box-orient: vertical;
}

.dish-rating {
  display: flex;
  align-items: center;
  gap: 4px;
  margin-bottom: 12px;
}

.dish-rating .rating-stars {
  font-size: 12px;
}

.dish-rating .rating-score,
.dish-rating .rating-text {
  font-size: 12px;
  color: #f39c12;
}

.dish-price-section {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: auto;
}

.price-info {
  display: flex;
  align-items: center;
  gap: 6px;
}

.current-price {
  font-size: 16px;
  font-weight: 600;
  color: #e74c3c;
}

.original-price {
  font-size: 12px;
  color: #999;
  text-decoration: line-through;
}

.quantity-controls {
  display: flex;
  align-items: center;
  gap: 6px;
  min-width: 80px;
  justify-content: flex-end;
}

.quantity-btn {
  width: 28px;
  height: 28px;
  border: 1px solid #007BFF;
  background: white;
  color: #007BFF;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  font-size: 16px;
  font-weight: 600;
  transition: all 0.3s ease;
  flex-shrink: 0;
}

.quantity-btn:hover {
  background: #007BFF;
  color: white;
}

.quantity-btn.plus {
  background: #007BFF;
  color: white;
}

.quantity {
  font-size: 14px;
  font-weight: 600;
  min-width: 20px;
  text-align: center;
  color: #333;
  flex-shrink: 0;
}

/* 购物车悬浮按钮 */
.cart-float {
  position: fixed;
  bottom: 80px;
  right: 20px;
  background: linear-gradient(135deg, #007BFF, #00D4FF);
  border-radius: 25px;
  padding: 12px 20px;
  display: flex;
  align-items: center;
  gap: 12px;
  cursor: pointer;
  box-shadow: 0 4px 20px rgba(0, 123, 255, 0.3);
  z-index: 1000;
  transition: all 0.3s ease;
}

.cart-float:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 25px rgba(0, 123, 255, 0.4);
}

.cart-icon {
  font-size: 20px;
  color: white;
}

.cart-info {
  color: white;
}

.cart-count {
  font-size: 12px;
  font-weight: 600;
}

.cart-total {
  font-size: 14px;
  font-weight: 700;
}

/* 菜品详情弹窗样式 */
.dish-modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 2000;
  padding: 20px;
}

.modal-content {
  background: white;
  border-radius: 16px;
  overflow: hidden;
  max-width: 400px;
  width: 100%;
  max-height: 80vh;
  overflow-y: auto;
  position: relative;
}

.modal-close {
  position: absolute;
  top: 12px;
  right: 12px;
  width: 32px;
  height: 32px;
  background: rgba(0, 0, 0, 0.5);
  color: white;
  border: none;
  border-radius: 50%;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1;
}

.modal-header {
  height: 200px;
  overflow: hidden;
}

.modal-dish-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.modal-body {
  padding: 20px;
}

.modal-dish-name {
  font-size: 20px;
  font-weight: 600;
  color: #333;
  margin: 0 0 8px 0;
}

.modal-dish-desc {
  font-size: 14px;
  color: #666;
  line-height: 1.5;
  margin: 0 0 16px 0;
}

.modal-dish-meta {
  display: flex;
  gap: 16px;
  margin-bottom: 20px;
}

.modal-rating,
.modal-sales {
  font-size: 14px;
  color: #666;
}

.modal-price-section {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.modal-price {
  display: flex;
  align-items: center;
  gap: 8px;
}

.modal-current-price {
  font-size: 20px;
  font-weight: 600;
  color: #e74c3c;
}

.modal-original-price {
  font-size: 14px;
  color: #999;
  text-decoration: line-through;
}

.modal-quantity-controls {
  display: flex;
  align-items: center;
  gap: 8px;
  min-width: 100px;
  justify-content: flex-end;
}

.modal-quantity-btn {
  width: 36px;
  height: 36px;
  border: 1px solid #007BFF;
  background: white;
  color: #007BFF;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  font-size: 18px;
  font-weight: 600;
  transition: all 0.3s ease;
  flex-shrink: 0;
}

.modal-quantity-btn:hover {
  background: #007BFF;
  color: white;
}

.modal-quantity-btn.plus {
  background: #007BFF;
  color: white;
}

.modal-quantity {
  font-size: 16px;
  font-weight: 600;
  min-width: 30px;
  text-align: center;
  color: #333;
  flex-shrink: 0;
}

/* 购物车弹窗样式 */
.cart-modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: flex-end;
  justify-content: center;
  z-index: 2000;
}

.cart-modal-content {
  background: white;
  border-radius: 16px 16px 0 0;
  width: 100%;
  max-width: 500px;
  max-height: 80vh;
  display: flex;
  flex-direction: column;
  animation: slideUp 0.3s ease-out;
}

@keyframes slideUp {
  from {
    transform: translateY(100%);
  }
  to {
    transform: translateY(0);
  }
}

.cart-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 20px;
  border-bottom: 1px solid #e9ecef;
}

.cart-title {
  font-size: 18px;
  font-weight: 600;
  color: #333;
  margin: 0;
}

.cart-close {
  width: 32px;
  height: 32px;
  background: #f8f9fa;
  border: none;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  color: #666;
  font-size: 16px;
}

.cart-close:hover {
  background: #e9ecef;
}

.cart-body {
  flex: 1;
  overflow-y: auto;
  padding: 16px 20px;
}

.empty-cart {
  text-align: center;
  padding: 40px 20px;
  color: #666;
}

.empty-cart .empty-icon {
  font-size: 48px;
  margin-bottom: 16px;
}

.empty-cart p {
  margin: 8px 0;
  font-size: 14px;
}

.cart-items {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.cart-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px;
  background: #f8f9fa;
  border-radius: 12px;
}

.cart-item .item-image {
  width: 60px;
  height: 60px;
  border-radius: 8px;
  overflow: hidden;
  flex-shrink: 0;
}

.cart-item .item-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.cart-item .item-details {
  flex: 1;
}

.cart-item .item-name {
  font-size: 14px;
  font-weight: 600;
  color: #333;
  margin-bottom: 4px;
}

.cart-item .item-price {
  font-size: 12px;
  color: #e74c3c;
}

.cart-item .item-controls {
  display: flex;
  align-items: center;
  gap: 8px;
}

.cart-quantity-btn {
  width: 28px;
  height: 28px;
  border: 1px solid #007BFF;
  background: white;
  color: #007BFF;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  font-size: 14px;
  font-weight: 600;
  transition: all 0.3s ease;
}

.cart-quantity-btn:hover {
  background: #007BFF;
  color: white;
}

.cart-quantity-btn.plus {
  background: #007BFF;
  color: white;
}

.cart-quantity {
  font-size: 14px;
  font-weight: 600;
  min-width: 24px;
  text-align: center;
  color: #333;
}

.cart-item .item-subtotal {
  font-size: 14px;
  font-weight: 600;
  color: #333;
  min-width: 60px;
  text-align: right;
}

.cart-footer {
  border-top: 1px solid #e9ecef;
  padding: 16px 20px;
}

.cart-summary {
  margin-bottom: 16px;
}

.summary-line {
  display: flex;
  justify-content: space-between;
  margin-bottom: 8px;
  font-size: 14px;
}

.summary-line:last-child {
  margin-bottom: 0;
}

.summary-line span:first-child {
  color: #666;
}

.summary-line span:last-child {
  color: #333;
  font-weight: 500;
}

.summary-line.total {
  font-size: 16px;
  font-weight: 600;
  padding-top: 8px;
  border-top: 1px solid #e9ecef;
}

.summary-line.total span {
  color: #333;
}

.checkout-btn {
  width: 100%;
  padding: 14px;
  background: linear-gradient(135deg, #007BFF, #00D4FF);
  color: white;
  border: none;
  border-radius: 25px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.checkout-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 123, 255, 0.3);
}

.checkout-btn:disabled {
  background: #e9ecef;
  color: #666;
  cursor: not-allowed;
}

.empty-dishes {
  text-align: center;
  padding: 60px 20px;
}

.empty-icon {
  font-size: 48px;
  margin-bottom: 16px;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .content-wrapper {
    flex-direction: column;
  }
  
  .categories-sidebar {
    width: 100%;
    height: auto;
    max-height: none;
    position: static;
    background: white;
    border-bottom: 1px solid #e9ecef;
    border-radius: 0;
    padding-bottom: 0;
  }
  
  .categories-list {
    display: flex;
    overflow-x: auto;
    padding: 12px;
    -webkit-overflow-scrolling: touch;
    scrollbar-width: none; /* Firefox */
  }
  
  .categories-list::-webkit-scrollbar {
    display: none; /* Chrome, Safari, Edge */
  }
  
  .category-item {
    flex-direction: row;
    min-width: 80px;
    padding: 8px 12px;
    border-left: none;
    border-bottom: 3px solid transparent;
    margin: 0 4px;
    border-radius: 20px;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
  }
  
  .category-item.active {
    border-left: none;
    border-bottom: none;
    background: #007BFF;
    color: white;
    box-shadow: 0 3px 6px rgba(0, 123, 255, 0.3);
  }
  
  .category-icon {
    width: 24px;
    height: 24px;
    margin-bottom: 0;
    margin-right: 6px;
  }
  
  .category-text {
    flex-direction: row;
    flex-wrap: nowrap;
  }
  
  .dishes-grid {
    grid-template-columns: 1fr;
  }
  
  .dish-card {
    flex-direction: column;
  }
  
  .dish-image-container {
    width: 100%;
    height: 200px; /* 增加50px高度 */
  }
  
  .cart-float {
    bottom: 20px;
    right: 20px;
    left: 20px;
    justify-content: center;
  }
  
  .cart-modal-content {
    max-width: 100%;
    border-radius: 16px 16px 0 0;
  }
  
  .cart-item {
    gap: 8px;
    padding: 8px;
  }
  
  .cart-item .item-image {
    width: 50px;
    height: 50px;
  }
  
  .cart-item .item-name {
    font-size: 13px;
  }
  
  .cart-item .item-price {
    font-size: 11px;
  }
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}
</style>