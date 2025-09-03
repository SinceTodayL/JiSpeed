<template>
  <div class="merchants-browse">
    <!-- 背景装饰 -->
    <div class="background-decoration">
      <div class="decoration-circle circle-1"></div>
      <div class="decoration-circle circle-2"></div>
      <div class="decoration-circle circle-3"></div>
      <div class="decoration-wave wave-1"></div>
      <div class="decoration-wave wave-2"></div>
      <div class="decoration-dots"></div>
    </div>

    <!-- 搜索栏 -->
    <div class="search-section">
      <div class="search-container">
        <div class="search-input-wrapper">
          <i class="search-icon">🔍</i>
          <input
            v-model="searchKeyword"
            type="text"
            class="search-input"
            placeholder="搜索商家或菜品"
            @keyup.enter="handleSearch"
          />
          <button v-if="searchKeyword" @click="clearSearch" class="clear-btn">✕</button>
        </div>
        <button @click="handleSearch" class="search-btn">搜索</button>
      </div>
    </div>

    <!-- 分类筛选 -->
    <div class="filter-section">
      <div class="filter-tabs">
        <button
          v-for="filter in filterOptions"
          :key="filter.value"
          :class="['filter-tab', { active: activeFilter === filter.value }]"
          @click="changeFilter(filter.value)"
        >
          {{ filter.label }}
        </button>
      </div>
    </div>

    <!-- 商家列表 -->
    <div class="merchants-container">
      <div v-if="loading" class="loading-container">
        <div class="loading-spinner"></div>
        <p>正在加载商家信息...</p>
      </div>

      <div v-else-if="merchants.length === 0" class="empty-container">
        <div class="empty-icon">🏪</div>
        <p>暂无商家信息</p>
      </div>

      <div v-else class="merchants-grid">
        <div
          v-for="merchant in merchants"
          :key="merchant.merchantId"
          class="merchant-card"
          @click="goToMerchant(merchant.merchantId)"
        >
          <!-- 商家封面图 -->
          <div class="merchant-cover">
            <img
              :src="merchant.coverImage || '/api/placeholder/400/200'"
              :alt="merchant.merchantName"
              class="cover-image"
              @error="handleImageError"
            />
            <div class="merchant-status">
              <span v-if="merchant.status === 1" class="status-open">营业中</span>
              <span v-else class="status-closed">休息中</span>
            </div>
          </div>

          <!-- 商家信息 -->
          <div class="merchant-info">
            <div class="merchant-header">
              <h3 class="merchant-name">{{ merchant.merchantName }}</h3>
              <div class="merchant-rating">
                <span class="rating-stars">⭐</span>
                <span class="rating-score">{{ merchant.rating || 4.5 }}</span>
              </div>
            </div>

            <div class="merchant-meta">
              <div class="meta-item">
                <span class="meta-icon">🚚</span>
                <span class="meta-text">{{ merchant.deliveryTime || 30 }}分钟</span>
              </div>
              <div class="meta-item">
                <span class="meta-icon">💰</span>
                <span class="meta-text">配送费¥{{ merchant.deliveryFee || 3 }}</span>
              </div>
              <div class="meta-item">
                <span class="meta-icon">📦</span>
                <span class="meta-text">起送¥{{ merchant.minOrderAmount || 20 }}</span>
              </div>
            </div>

            <!-- 商家标签 -->
            <div v-if="merchant.tags && merchant.tags.length" class="merchant-tags">
              <span
                v-for="tag in merchant.tags.slice(0, 3)"
                :key="tag"
                class="tag"
              >
                {{ tag }}
              </span>
            </div>

            <!-- 特色菜品预览 -->
            <div v-if="merchant.featuredDishes && merchant.featuredDishes.length" class="featured-dishes">
              <div class="featured-title">招牌菜：</div>
              <div class="dishes-preview">
                <div
                  v-for="dish in merchant.featuredDishes.slice(0, 3)"
                  :key="dish.dishId"
                  class="dish-preview"
                >
                  <img
                    :src="dish.coverUrl || '/api/placeholder/60/60'"
                    :alt="dish.dishName"
                    class="dish-image"
                    @error="handleImageError"
                  />
                  <div class="dish-info">
                    <div class="dish-name">{{ dish.dishName }}</div>
                    <div class="dish-price">¥{{ dish.price }}</div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- 分页组件 -->
      <div v-if="totalPages > 1" class="pagination">
        <button
          :disabled="currentPage <= 1"
          @click="changePage(currentPage - 1)"
          class="page-btn"
        >
          上一页
        </button>
        
        <div class="page-numbers">
          <button
            v-for="page in visiblePages"
            :key="page"
            :class="['page-number', { active: page === currentPage }]"
            @click="changePage(page)"
          >
            {{ page }}
          </button>
        </div>
        
        <button
          :disabled="currentPage >= totalPages"
          @click="changePage(currentPage + 1)"
          class="page-btn"
        >
          下一页
        </button>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, reactive, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { merchantAPI } from '@/api/browse.js'

export default {
  name: 'MerchantsBrowse',
  setup() {
    const router = useRouter()
    
    // 响应式数据
    const merchants = ref([])
    const loading = ref(false)
    const searchKeyword = ref('')
    const activeFilter = ref('all')
    const currentPage = ref(1)
    const totalPages = ref(1)
    const pageSize = ref(12)
    const total = ref(0)

    // 筛选选项
    const filterOptions = ref([
      { label: '全部', value: 'all' },
      { label: '评分最高', value: 'rating' },
      { label: '销量最高', value: 'sales' },
      { label: '距离最近', value: 'distance' },
      { label: '配送最快', value: 'deliveryTime' }
    ])

    // 计算可见页码
    const visiblePages = computed(() => {
      const pages = []
      const start = Math.max(1, currentPage.value - 2)
      const end = Math.min(totalPages.value, start + 4)
      
      for (let i = start; i <= end; i++) {
        pages.push(i)
      }
      return pages
    })

    // 方法
    const fetchMerchants = async () => {
      loading.value = true
      try {
        const params = {
          page: currentPage.value,
          limit: pageSize.value,
          keyword: searchKeyword.value,
          sortBy: activeFilter.value
        }
        
        const response = await merchantAPI.getAllMerchants(params)
        
        if (response && response.data) {
          merchants.value = response.data.merchants || []
          total.value = response.data.total || 0
          totalPages.value = Math.ceil(total.value / pageSize.value)
        } else {
          merchants.value = []
          total.value = 0
          totalPages.value = 1
        }
      } catch (error) {
        console.error('获取商家列表失败:', error)
        merchants.value = []
        total.value = 0
        totalPages.value = 1
      } finally {
        loading.value = false
      }
    }

    const handleSearch = async () => {
      currentPage.value = 1
      await fetchMerchants()
    }

    const clearSearch = () => {
      searchKeyword.value = ''
      handleSearch()
    }

    const changeFilter = async (filterValue) => {
      activeFilter.value = filterValue
      currentPage.value = 1
      await fetchMerchants()
    }

    const changePage = async (page) => {
      if (page >= 1 && page <= totalPages.value) {
        currentPage.value = page
        await fetchMerchants()
        // 滚动到顶部
        window.scrollTo({ top: 0, behavior: 'smooth' })
      }
    }

    const goToMerchant = (merchantId) => {
      router.push(`/merchant/${merchantId}`)
    }

    const handleImageError = (event) => {
      event.target.src = '/api/placeholder/400/200'
    }

    // 生命周期
    onMounted(() => {
      fetchMerchants()
    })

    return {
      merchants,
      loading,
      searchKeyword,
      activeFilter,
      currentPage,
      totalPages,
      total,
      filterOptions,
      visiblePages,
      fetchMerchants,
      handleSearch,
      clearSearch,
      changeFilter,
      changePage,
      goToMerchant,
      handleImageError
    }
  }
}
</script>

<style scoped>
.merchants-browse {
  min-height: 100vh;
  background: linear-gradient(135deg, #f0f8ff 0%, #e8f4fd 30%, #f8fbff 70%, #e3f2fd 100%);
  padding: 20px;
  position: relative;
  overflow-x: hidden;
}

/* 背景装饰样式 */
.background-decoration {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
  z-index: 0;
  overflow: hidden;
}

.decoration-circle {
  position: absolute;
  border-radius: 50%;
  background: linear-gradient(45deg, rgba(0, 123, 255, 0.08), rgba(0, 212, 255, 0.12));
  animation: float 8s ease-in-out infinite;
}

.circle-1 {
  width: 200px;
  height: 200px;
  top: 10%;
  right: 15%;
  animation-delay: 0s;
}

.circle-2 {
  width: 150px;
  height: 150px;
  bottom: 20%;
  left: 10%;
  animation-delay: 2s;
}

.circle-3 {
  width: 100px;
  height: 100px;
  top: 60%;
  right: 5%;
  animation-delay: 4s;
}

.decoration-wave {
  position: absolute;
  width: 100%;
  height: 120px;
  background: linear-gradient(90deg, 
    transparent 0%, 
    rgba(0, 123, 255, 0.03) 25%, 
    rgba(0, 212, 255, 0.05) 50%, 
    rgba(64, 224, 208, 0.03) 75%, 
    transparent 100%);
  border-radius: 50%;
  transform: rotate(-15deg);
}

.wave-1 {
  top: 25%;
  left: -20%;
  animation: wave 12s ease-in-out infinite;
}

.wave-2 {
  bottom: 30%;
  right: -20%;
  animation: wave 15s ease-in-out infinite reverse;
}

.decoration-dots {
  position: absolute;
  top: 40%;
  left: 20%;
  width: 60px;
  height: 60px;
  background-image: radial-gradient(circle, rgba(0, 123, 255, 0.1) 2px, transparent 2px);
  background-size: 20px 20px;
  animation: dots 10s linear infinite;
}

@keyframes float {
  0%, 100% { 
    transform: translateY(0px) rotate(0deg); 
    opacity: 0.7;
  }
  50% { 
    transform: translateY(-30px) rotate(5deg); 
    opacity: 1;
  }
}

@keyframes wave {
  0%, 100% { 
    transform: rotate(-15deg) translateX(0px); 
  }
  50% { 
    transform: rotate(-15deg) translateX(50px); 
  }
}

@keyframes dots {
  0% { 
    transform: rotate(0deg); 
  }
  100% { 
    transform: rotate(360deg); 
  }
}

/* 搜索栏样式 */
.search-section {
  margin-bottom: 20px;
  position: relative;
  z-index: 1;
}

.search-container {
  max-width: 800px;
  margin: 0 auto;
  display: flex;
  gap: 12px;
}

.search-input-wrapper {
  flex: 1;
  position: relative;
  display: flex;
  align-items: center;
}

.search-icon {
  position: absolute;
  left: 16px;
  font-size: 16px;
  color: #999;
  z-index: 1;
}

.search-input {
  width: 100%;
  padding: 14px 50px 14px 48px;
  border: 2px solid rgba(225, 229, 233, 0.8);
  border-radius: 25px;
  font-size: 16px;
  color: #333;
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
  transition: all 0.3s ease;
  box-sizing: border-box;
}

.search-input:focus {
  outline: none;
  border-color: #007BFF;
  box-shadow: 0 0 0 3px rgba(0, 123, 255, 0.1);
  background: rgba(255, 255, 255, 0.98);
}

.clear-btn {
  position: absolute;
  right: 16px;
  background: none;
  border: none;
  font-size: 18px;
  color: #999;
  cursor: pointer;
  padding: 0;
  z-index: 1;
}

.search-btn {
  padding: 14px 24px;
  background: linear-gradient(135deg, #007BFF, #00D4FF);
  color: white;
  border: none;
  border-radius: 25px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  min-width: 80px;
}

.search-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 15px rgba(0, 123, 255, 0.3);
}

/* 筛选标签样式 */
.filter-section {
  margin-bottom: 24px;
  position: relative;
  z-index: 1;
}

.filter-tabs {
  display: flex;
  gap: 12px;
  justify-content: center;
  flex-wrap: wrap;
}

.filter-tab {
  padding: 10px 20px;
  background: rgba(255, 255, 255, 0.9);
  backdrop-filter: blur(8px);
  border: 2px solid rgba(225, 229, 233, 0.6);
  border-radius: 20px;
  font-size: 14px;
  color: #666;
  cursor: pointer;
  transition: all 0.3s ease;
}

.filter-tab:hover {
  border-color: #007BFF;
  color: #007BFF;
  background: rgba(255, 255, 255, 0.95);
}

.filter-tab.active {
  background: linear-gradient(135deg, #007BFF, #00D4FF);
  border-color: #007BFF;
  color: white;
}

/* 商家网格样式 */
.merchants-container {
  max-width: 1200px;
  margin: 0 auto;
  position: relative;
  z-index: 1;
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

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.empty-container {
  text-align: center;
  padding: 60px 20px;
}

.empty-icon {
  font-size: 48px;
  margin-bottom: 16px;
}

.merchants-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 24px;
  margin-bottom: 40px;
}

/* 商家卡片样式 */
.merchant-card {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 16px;
  overflow: hidden;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
  transition: all 0.3s ease;
  cursor: pointer;
}

.merchant-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 25px rgba(0, 123, 255, 0.15);
  background: rgba(255, 255, 255, 0.98);
}

.merchant-cover {
  position: relative;
  height: 180px;
  overflow: hidden;
}

.cover-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s ease;
}

.merchant-card:hover .cover-image {
  transform: scale(1.05);
}

.merchant-status {
  position: absolute;
  top: 12px;
  right: 12px;
}

.status-open {
  background: #28a745;
  color: white;
  padding: 4px 8px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 500;
}

.status-closed {
  background: #dc3545;
  color: white;
  padding: 4px 8px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 500;
}

.merchant-info {
  padding: 16px;
}

.merchant-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.merchant-name {
  font-size: 18px;
  font-weight: 600;
  color: #333;
  margin: 0;
  flex: 1;
  margin-right: 8px;
}

.merchant-rating {
  display: flex;
  align-items: center;
  gap: 4px;
}

.rating-stars {
  font-size: 14px;
}

.rating-score {
  font-size: 14px;
  font-weight: 600;
  color: #333;
}

.merchant-meta {
  display: flex;
  gap: 16px;
  margin-bottom: 12px;
  flex-wrap: wrap;
}

.meta-item {
  display: flex;
  align-items: center;
  gap: 4px;
}

.meta-icon {
  font-size: 12px;
}

.meta-text {
  font-size: 12px;
  color: #666;
}

.merchant-tags {
  display: flex;
  gap: 6px;
  margin-bottom: 12px;
  flex-wrap: wrap;
}

.tag {
  background: #f0f8ff;
  color: #007BFF;
  padding: 2px 8px;
  border-radius: 12px;
  font-size: 12px;
}

.featured-dishes {
  border-top: 1px solid #f0f0f0;
  padding-top: 12px;
}

.featured-title {
  font-size: 14px;
  font-weight: 600;
  color: #333;
  margin-bottom: 8px;
}

.dishes-preview {
  display: flex;
  gap: 8px;
  overflow-x: auto;
  scrollbar-width: none;
  -ms-overflow-style: none;
}

.dishes-preview::-webkit-scrollbar {
  display: none;
}

.dish-preview {
  display: flex;
  align-items: center;
  gap: 8px;
  min-width: 120px;
  padding: 6px;
  background: #f8f9fa;
  border-radius: 8px;
}

.dish-image {
  width: 36px;
  height: 36px;
  border-radius: 6px;
  object-fit: cover;
}

.dish-info {
  flex: 1;
  min-width: 0;
}

.dish-name {
  font-size: 12px;
  color: #333;
  margin-bottom: 2px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.dish-price {
  font-size: 12px;
  font-weight: 600;
  color: #e74c3c;
}

/* 分页样式 */
.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 8px;
  margin-top: 40px;
}

.page-btn {
  padding: 8px 16px;
  background: rgba(255, 255, 255, 0.9);
  backdrop-filter: blur(8px);
  border: 2px solid rgba(225, 229, 233, 0.6);
  border-radius: 8px;
  color: #666;
  cursor: pointer;
  transition: all 0.3s ease;
}

.page-btn:hover:not(:disabled) {
  border-color: #007BFF;
  color: #007BFF;
  background: rgba(255, 255, 255, 0.95);
}

.page-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.page-numbers {
  display: flex;
  gap: 4px;
}

.page-number {
  width: 36px;
  height: 36px;
  background: rgba(255, 255, 255, 0.9);
  backdrop-filter: blur(8px);
  border: 2px solid rgba(225, 229, 233, 0.6);
  border-radius: 8px;
  color: #666;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.page-number:hover {
  border-color: #007BFF;
  color: #007BFF;
  background: rgba(255, 255, 255, 0.95);
}

.page-number.active {
  background: linear-gradient(135deg, #007BFF, #00D4FF);
  border-color: #007BFF;
  color: white;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .merchants-browse {
    padding: 16px;
  }
  
  .search-container {
    flex-direction: column;
  }
  
  .merchants-grid {
    grid-template-columns: 1fr;
    gap: 16px;
  }
  
  .merchant-meta {
    flex-direction: column;
    gap: 8px;
  }
  
  .filter-tabs {
    justify-content: flex-start;
    overflow-x: auto;
    padding-bottom: 8px;
  }
  
  .pagination {
    flex-wrap: wrap;
  }

  /* 移动端背景装饰适配 */
  .circle-1 {
    width: 120px;
    height: 120px;
    top: 5%;
    right: 5%;
  }

  .circle-2 {
    width: 100px;
    height: 100px;
    bottom: 15%;
    left: 5%;
  }

  .circle-3 {
    width: 80px;
    height: 80px;
    top: 50%;
    right: 2%;
  }

  .decoration-dots {
    top: 30%;
    left: 10%;
    width: 40px;
    height: 40px;
    background-size: 15px 15px;
  }

  .wave-1 {
    left: -30%;
  }

  .wave-2 {
    right: -30%;
  }
}
</style>
