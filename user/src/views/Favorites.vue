<template>
  <div class="favorites-page">
    <!-- 页面头部 -->
    <div class="page-header">
      <div class="header-content">
        <button @click="goBack" class="back-btn">
          <i class="back-icon">←</i>
        </button>
        <h1 class="page-title">我的收藏</h1>
      </div>
    </div>

    <!-- 收藏内容 -->
    <div class="favorites-content">
      <!-- 加载状态 -->
      <div v-if="loading" class="loading-container">
        <div class="loading-spinner"></div>
        <p>正在加载收藏...</p>
      </div>

      <!-- 空状态 -->
      <div v-else-if="favorites.length === 0" class="empty-state">
        <div class="empty-icon">💝</div>
        <h3 class="empty-title">暂无收藏</h3>
        <p class="empty-desc">收藏喜欢的美食，方便下次查找</p>
        <button @click="goToHome" class="browse-btn">去逛逛</button>
      </div>

      <!-- 收藏列表 -->
      <div v-else class="favorites-list">
        <div 
          v-for="item in favorites" 
          :key="item.id"
          class="favorite-card"
          @click="goToMerchant(item.merchantId)"
        >
          <!-- 菜品图片 -->
          <div class="dish-image-container">
            <img 
              :src="item.image || '/api/placeholder/120/120'"
              :alt="item.dishName"
              class="dish-image"
            />
            <button 
              @click.stop="removeFavorite(item.id)"
              class="remove-favorite-btn"
              title="取消收藏"
            >
              ❤️
            </button>
          </div>

          <!-- 菜品信息 -->
          <div class="dish-info">
            <h3 class="dish-name">{{ item.dishName }}</h3>
            <p class="dish-description">{{ item.description || '暂无描述' }}</p>
            
            <div class="merchant-info">
              <span class="merchant-name">🏪 {{ item.merchantName }}</span>
            </div>
            
            <div class="dish-meta">
              <div class="price-section">
                <span class="current-price">¥{{ item.price.toFixed(2) }}</span>
                <span v-if="item.originalPrice && item.originalPrice > item.price" class="original-price">
                  ¥{{ item.originalPrice.toFixed(2) }}
                </span>
              </div>
              
              <div class="rating-section">
                <span class="rating">⭐ {{ item.rating || 4.5 }}</span>
                <span class="sales">月销{{ item.monthlySales || 100 }}+</span>
              </div>
            </div>
            
            <div class="favorite-time">
              <span class="time-text">{{ formatTime(item.favoriteTime) }}收藏</span>
            </div>
          </div>

          <!-- 操作按钮 -->
          <div class="dish-actions">
            <button 
              @click.stop="addToCart(item)"
              class="add-to-cart-btn"
            >
              加入购物车
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- 批量操作栏 -->
    <div v-if="favorites.length > 0" class="bulk-actions">
      <label class="select-all-checkbox">
        <input 
          type="checkbox" 
          v-model="isAllSelected"
          @change="toggleSelectAll"
        />
        <span class="checkmark"></span>
        <span class="select-text">全选</span>
      </label>
      
      <div class="action-buttons">
        <button 
          @click="bulkRemove"
          :disabled="selectedItems.length === 0"
          class="bulk-remove-btn"
        >
          删除选中 ({{ selectedItems.length }})
        </button>
        <button 
          @click="bulkAddToCart"
          :disabled="selectedItems.length === 0"
          class="bulk-cart-btn"
        >
          批量加购物车
        </button>
      </div>
    </div>

    <!-- 删除确认弹窗 -->
    <div v-if="showDeleteModal" class="delete-modal" @click="cancelDelete">
      <div class="modal-content" @click.stop>
        <h3 class="modal-title">确认删除</h3>
        <p class="modal-message">{{ deleteModalMessage }}</p>
        <div class="modal-actions">
          <button @click="cancelDelete" class="cancel-btn">取消</button>
          <button @click="confirmDelete" class="confirm-btn">确认删除</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { favoriteAPI } from '@/api/user.js'

export default {
  name: 'FavoritesPage',
  setup() {
    const router = useRouter()
    
    const loading = ref(false)
    const favorites = ref([])
    const showDeleteModal = ref(false)
    const deleteModalMessage = ref('')
    const pendingDeleteAction = ref(null)
    
    // 计算属性
    const selectedItems = computed(() => {
      return favorites.value.filter(item => item.selected)
    })
    
    const isAllSelected = computed({
      get() {
        return favorites.value.length > 0 && favorites.value.every(item => item.selected)
      },
      set(value) {
        favorites.value.forEach(item => {
          item.selected = value
        })
      }
    })
    
    // 获取收藏列表
    const fetchFavorites = async () => {
      loading.value = true
      try {
        const userId = (typeof localStorage !== 'undefined' && localStorage.getItem && localStorage.getItem('userId'))
          ? localStorage.getItem('userId')
          : ''
        
        const response = await favoriteAPI.getUserFavorites(userId)
        if (response.code === 200) {
          favorites.value = response.data.map(item => ({
            ...item,
            selected: false
          }))
        } else {
          favorites.value = []
        }
      } catch (error) {
        console.error('获取收藏列表失败:', error)
        favorites.value = []
      } finally {
        loading.value = false
      }
    }
    
    // 取消收藏
    const removeFavorite = (favoriteId) => {
      deleteModalMessage.value = '确定要取消收藏这个菜品吗？'
      pendingDeleteAction.value = () => deleteFavoriteItem(favoriteId)
      showDeleteModal.value = true
    }
    
    // 删除收藏项
    const deleteFavoriteItem = async (favoriteId) => {
      try {
        const userId = (typeof localStorage !== 'undefined' && localStorage.getItem && localStorage.getItem('userId'))
          ? localStorage.getItem('userId')
          : ''
        await favoriteAPI.removeFavorite(userId, favoriteId)
        
        const index = favorites.value.findIndex(item => item.id === favoriteId)
        if (index > -1) {
          favorites.value.splice(index, 1)
        }
        console.log('取消收藏成功')
      } catch (error) {
        console.error('取消收藏失败:', error)
      }
    }
    
    // 批量删除
    const bulkRemove = () => {
      if (selectedItems.value.length === 0) return
      
      deleteModalMessage.value = `确定要取消收藏所选的 ${selectedItems.value.length} 个菜品吗？`
      pendingDeleteAction.value = () => deleteBulkFavorites()
      showDeleteModal.value = true
    }
    
    // 批量删除收藏
    const deleteBulkFavorites = async () => {
      try {
        const userId = (typeof localStorage !== 'undefined' && localStorage.getItem && localStorage.getItem('userId'))
          ? localStorage.getItem('userId')
          : ''
        const itemsToDelete = selectedItems.value
        
        for (const item of itemsToDelete) {
          await favoriteAPI.removeFavorite(userId, item.id)
          const index = favorites.value.findIndex(fav => fav.id === item.id)
          if (index > -1) {
            favorites.value.splice(index, 1)
          }
        }
        console.log('批量取消收藏成功')
      } catch (error) {
        console.error('批量取消收藏失败:', error)
      }
    }
    
    // 确认删除
    const confirmDelete = async () => {
      if (pendingDeleteAction.value) {
        await pendingDeleteAction.value()
        cancelDelete()
      }
    }
    
    // 取消删除
    const cancelDelete = () => {
      showDeleteModal.value = false
      deleteModalMessage.value = ''
      pendingDeleteAction.value = null
    }
    
    // 全选/取消全选
    const toggleSelectAll = () => {
      // 计算属性会自动处理
    }
    
    // 加入购物车
    const addToCart = (item) => {
      console.log('加入购物车:', item)
      // 实际项目中调用购物车API
    }
    
    // 批量加入购物车
    const bulkAddToCart = () => {
      if (selectedItems.value.length === 0) return
      
      selectedItems.value.forEach(item => {
        addToCart(item)
      })
      console.log('批量加入购物车:', selectedItems.value.length, '件商品')
    }
    
    // 跳转到商家页面
    const goToMerchant = (merchantId) => {
      router.push(`/merchant/${merchantId}`)
    }
    
    // 格式化时间
    const formatTime = (timeString) => {
      const date = new Date(timeString)
      const now = new Date()
      const diffMs = now - date
      const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24))
      
      if (diffDays === 0) {
        return '今天'
      } else if (diffDays === 1) {
        return '昨天'
      } else if (diffDays < 30) {
        return `${diffDays}天前`
      } else {
        return `${date.getMonth() + 1}月${date.getDate()}日`
      }
    }
    
    // 返回首页
    const goToHome = () => {
      router.push('/')
    }
    
    // 返回上一页
    const goBack = () => {
      router.back()
    }
    
    onMounted(() => {
      fetchFavorites()
    })
    
    return {
      loading,
      favorites,
      showDeleteModal,
      deleteModalMessage,
      selectedItems,
      isAllSelected,
      removeFavorite,
      bulkRemove,
      confirmDelete,
      cancelDelete,
      toggleSelectAll,
      addToCart,
      bulkAddToCart,
      goToMerchant,
      formatTime,
      goToHome,
      goBack
    }
  }
}
</script>

<style scoped>
.favorites-page {
  min-height: 100vh;
  background: #f8f9fa;
  padding-bottom: 140px; /* 为底部导航和批量操作栏留空间 */
}

/* 页面头部 */
.page-header {
  background: white;
  border-bottom: 1px solid #e9ecef;
  position: sticky;
  top: 0;
  z-index: 100;
}

.header-content {
  display: flex;
  align-items: center;
  padding: 16px 20px;
  max-width: 1200px;
  margin: 0 auto;
}

.back-btn {
  width: 40px;
  height: 40px;
  background: none;
  border: none;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  font-size: 18px;
  color: #333;
  transition: all 0.3s ease;
  margin-right: 12px;
}

.back-btn:hover {
  background: #f8f9fa;
}

.page-title {
  font-size: 20px;
  font-weight: 600;
  color: #333;
  margin: 0;
}

/* 收藏内容 */
.favorites-content {
  padding: 20px;
  max-width: 1200px;
  margin: 0 auto;
}

.loading-container {
  text-align: center;
  padding: 60px 20px;
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #e1e5e9;
  border-top: 4px solid #4facfe;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 16px;
}

.empty-state {
  text-align: center;
  padding: 80px 20px;
  background: white;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.empty-icon {
  font-size: 64px;
  margin-bottom: 24px;
}

.empty-title {
  font-size: 20px;
  font-weight: 600;
  color: #333;
  margin: 0 0 12px 0;
}

.empty-desc {
  font-size: 14px;
  color: #666;
  margin: 0 0 32px 0;
}

.browse-btn {
  padding: 12px 32px;
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
  color: white;
  border: none;
  border-radius: 25px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.browse-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(79, 172, 254, 0.3);
}

/* 收藏列表 */
.favorites-list {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 20px;
}

.favorite-card {
  background: white;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  cursor: pointer;
  transition: all 0.3s ease;
  position: relative;
}

.favorite-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.15);
}

.dish-image-container {
  position: relative;
  height: 180px;
  overflow: hidden;
}

.dish-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s ease;
}

.favorite-card:hover .dish-image {
  transform: scale(1.05);
}

.remove-favorite-btn {
  position: absolute;
  top: 12px;
  right: 12px;
  width: 36px;
  height: 36px;
  background: rgba(255, 255, 255, 0.9);
  border: none;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  font-size: 18px;
  transition: all 0.3s ease;
  backdrop-filter: blur(10px);
}

.remove-favorite-btn:hover {
  background: white;
  transform: scale(1.1);
}

.dish-info {
  padding: 16px;
}

.dish-name {
  font-size: 16px;
  font-weight: 600;
  color: #333;
  margin: 0 0 8px 0;
  line-height: 1.3;
}

.dish-description {
  font-size: 14px;
  color: #666;
  margin: 0 0 12px 0;
  line-height: 1.4;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.merchant-info {
  margin-bottom: 12px;
}

.merchant-name {
  font-size: 13px;
  color: #999;
}

.dish-meta {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.price-section {
  display: flex;
  align-items: center;
  gap: 8px;
}

.current-price {
  font-size: 18px;
  font-weight: 600;
  color: #e74c3c;
}

.original-price {
  font-size: 14px;
  color: #999;
  text-decoration: line-through;
}

.rating-section {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 2px;
}

.rating {
  font-size: 13px;
  color: #ff9500;
}

.sales {
  font-size: 12px;
  color: #999;
}

.favorite-time {
  font-size: 12px;
  color: #999;
  margin-bottom: 12px;
}

.dish-actions {
  padding: 0 16px 16px;
}

.add-to-cart-btn {
  width: 100%;
  padding: 10px;
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
}

.add-to-cart-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(79, 172, 254, 0.3);
}

/* 批量操作栏 */
.bulk-actions {
  position: fixed;
  bottom: 60px;
  left: 0;
  right: 0;
  background: white;
  border-top: 1px solid #e9ecef;
  padding: 16px 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  z-index: 99;
  box-shadow: 0 -2px 8px rgba(0, 0, 0, 0.1);
}

.select-all-checkbox {
  display: flex;
  align-items: center;
  cursor: pointer;
  user-select: none;
}

.select-all-checkbox input {
  display: none;
}

.checkmark {
  width: 20px;
  height: 20px;
  border: 2px solid #ddd;
  border-radius: 4px;
  margin-right: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;
}

.select-all-checkbox input:checked + .checkmark {
  background: #4facfe;
  border-color: #4facfe;
}

.select-all-checkbox input:checked + .checkmark::after {
  content: '✓';
  color: white;
  font-size: 12px;
  font-weight: bold;
}

.select-text {
  font-size: 14px;
  color: #333;
}

.action-buttons {
  display: flex;
  gap: 12px;
}

.bulk-remove-btn,
.bulk-cart-btn {
  padding: 10px 20px;
  border: none;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
}

.bulk-remove-btn {
  background: #dc3545;
  color: white;
}

.bulk-remove-btn:disabled {
  background: #e9ecef;
  color: #6c757d;
  cursor: not-allowed;
}

.bulk-remove-btn:not(:disabled):hover {
  background: #c82333;
}

.bulk-cart-btn {
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
  color: white;
}

.bulk-cart-btn:disabled {
  background: #e9ecef;
  color: #6c757d;
  cursor: not-allowed;
}

.bulk-cart-btn:not(:disabled):hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(79, 172, 254, 0.3);
}

/* 删除确认弹窗 */
.delete-modal {
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
  border-radius: 12px;
  padding: 24px;
  max-width: 400px;
  width: 100%;
  text-align: center;
}

.modal-title {
  font-size: 18px;
  font-weight: 600;
  color: #333;
  margin: 0 0 16px 0;
}

.modal-message {
  font-size: 14px;
  color: #666;
  margin: 0 0 24px 0;
  line-height: 1.5;
}

.modal-actions {
  display: flex;
  gap: 12px;
  justify-content: center;
}

.cancel-btn,
.confirm-btn {
  padding: 10px 24px;
  border: none;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
}

.cancel-btn {
  background: #f8f9fa;
  color: #333;
  border: 1px solid #ddd;
}

.cancel-btn:hover {
  background: #e9ecef;
}

.confirm-btn {
  background: #dc3545;
  color: white;
}

.confirm-btn:hover {
  background: #c82333;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .favorites-content {
    padding: 12px;
  }
  
  .favorites-list {
    grid-template-columns: 1fr;
    gap: 16px;
  }
  
  .bulk-actions {
    padding: 12px 16px;
    flex-direction: column;
    gap: 12px;
    bottom: 60px;
  }
  
  .action-buttons {
    width: 100%;
  }
  
  .bulk-remove-btn,
  .bulk-cart-btn {
    flex: 1;
  }
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}
</style>
