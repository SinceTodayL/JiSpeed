<template>
  <div class="home-page">
    <!-- 欢迎区域 -->
    <div class="welcome-section">
      <div class="welcome-content">
        <h1 class="welcome-title">极速外卖</h1>
        <p class="welcome-subtitle">美食随心点，快递送到家</p>
      </div>
    </div>

    <!-- 搜索栏 -->
    <div class="search-section">
      <div class="search-container">
        <div class="search-input-wrapper">
          <i class="search-icon">🔍</i>
          <input
            v-model="searchKeyword"
            @keyup.enter="handleSearch"
            type="text"
            placeholder="搜索商家或菜品"
            class="search-input"
          />
          <button @click="handleSearch" class="search-btn">搜索</button>
        </div>
      </div>
    </div>

    <!-- 商家列表 -->
    <div class="merchant-list-section">
      <!-- 筛选和排序 -->
      <div class="filter-bar">
        <div class="section-header">
          <h2 class="section-title">附近商家</h2>
          <p class="section-subtitle">为您精选优质餐厅</p>
        </div>
        <div class="sort-options">
          <button 
            v-for="option in sortOptions" 
            :key="option.value"
            @click="changeSortBy(option.value)"
            :class="['sort-btn', { active: currentSort === option.value }]"
          >
            {{ option.label }}
          </button>
        </div>
      </div>

      <!-- 商家列表 -->
      <div class="merchant-list">
        <div v-if="loading" class="loading-container">
          <div class="loading-spinner"></div>
          <p>正在加载商家信息...</p>
        </div>

        <div v-else-if="merchants.length === 0" class="empty-state">
          <div class="empty-icon">🏪</div>
          <h3>暂无商家</h3>
          <p>当前条件下没有找到商家</p>
        </div>

        <div v-else class="merchants-grid">
          <div 
            v-for="merchant in merchants" 
            :key="merchant.merchantId"
            @click="goToMerchant(merchant.merchantId)"
            class="merchant-card"
          >
            <div class="merchant-image">
              <img :src="getMerchantImage(merchant.name)" :alt="merchant.name" />
              <div v-if="merchant.isOnline" class="online-badge">营业中</div>
              <div v-else class="offline-badge">休息中</div>
            </div>

            <div class="merchant-info">
              <h3 class="merchant-name">{{ merchant.name }}</h3>
              <p class="merchant-desc">{{ merchant.description }}</p>
              
              <div class="merchant-stats">
                <div class="rating">
                  <span class="rating-stars">★</span>
                  <span class="rating-score">{{ merchant.rating || '4.5' }}</span>
                  <span class="rating-count">({{ merchant.reviewCount || '999+' }})</span>
                </div>
                
                <div class="distance">
                  <i class="location-icon">📍</i>
                  <span>{{ merchant.distance || '1.2' }}km</span>
                </div>
              </div>

              <div class="merchant-tags">
                <span 
                  v-for="tag in merchant.tags?.slice(0, 3)" 
                  :key="tag"
                  class="tag"
                >
                  {{ tag }}
                </span>
              </div>

              <div class="merchant-bottom">
                <div class="delivery-info">
                  <span class="delivery-fee">配送费 ¥{{ merchant.deliveryFee || '3' }}</span>
                  <span class="delivery-time">{{ merchant.deliveryTime || '30-45' }}分钟</span>
                </div>
                
                <div v-if="merchant.minOrderAmount" class="min-order">
                  起送 ¥{{ merchant.minOrderAmount }}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- 加载更多 -->
      <div v-if="hasMore && !loading" class="load-more">
        <button @click="loadMore" class="load-more-btn">
          加载更多
        </button>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, reactive, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { merchantAPI } from '@/api/merchant.js'
import { getMerchantOrRandomImage } from '@/utils/imageUtils.js'

export default {
  name: 'Home',
  setup() {
    const router = useRouter()
    
    // 响应式数据
    const searchKeyword = ref('')
    const loading = ref(false)
    const merchants = ref([])
    const currentSort = ref('recommended')
    const currentPage = ref(1)
    const hasMore = ref(true)
    
    // 排序选项
    const sortOptions = ref([
      { value: 'recommended', label: '推荐' },
      { value: 'rating', label: '评分' },
      { value: 'distance', label: '距离' },
      { value: 'sales', label: '销量' },
      { value: 'delivery_time', label: '配送时间' }
    ])
    
    // 获取商家列表
    const fetchMerchants = async (reset = false) => {
      if (loading.value) return
      
      loading.value = true
      try {
        const params = {
          page: reset ? 1 : currentPage.value,
          size: 10,
          sortBy: currentSort.value,
          keyword: searchKeyword.value || undefined
        }
        
        // 调用真实API
        const response = await merchantAPI.getMerchantList(params)
        console.log('获取商家列表成功:', response.data)
          if (response.code === 0) {

          if (reset) {
            merchants.value = response.data || []
            currentPage.value = 1
          } else {
            merchants.value.push(...(response.data || []))
          }
          
              hasMore.value = (response.data?.length || 0) >= params.size
          
          if (!reset) {
            currentPage.value++
              }
          }
          else
          {
              console.error('获取商家列表失败:', response.code, response.message)
        }
      } catch (error) {
        console.error('获取商家列表失败:', error)
      } finally {
        loading.value = false
      }
    }
    
    // 搜索
    const handleSearch = () => {
      fetchMerchants(true)
    }
    
    // 改变排序
    const changeSortBy = (sortBy) => {
      currentSort.value = sortBy
      fetchMerchants(true)
    }
    
    // 加载更多
    const loadMore = () => {
      fetchMerchants(false)
    }
    
    // 跳转到商家详情
    const goToMerchant = (merchantId) => {
      router.push(`/merchant/${merchantId}`)
    }
    
    // 获取商家图片
    const getMerchantImage = (merchantName) => {
      return getMerchantOrRandomImage(merchantName)
    }
    
    // 页面挂载时初始化
    onMounted(() => {
      fetchMerchants(true)
    })
    
    return {
      searchKeyword,
      loading,
      merchants,
      sortOptions,
      currentSort,
      hasMore,
      handleSearch,
      changeSortBy,
      loadMore,
      goToMerchant,
      getMerchantImage
    }
  }
}
</script>

<style scoped>
.home-page {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  min-height: 100vh;
  padding-bottom: 80px; /* 为底部导航留空间 */
}

/* 欢迎区域 */
.welcome-section {
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
  color: white;
  text-align: center;
  padding: 40px 20px 20px;
}

.welcome-content {
  max-width: 400px;
  margin: 0 auto;
}

.welcome-title {
  font-size: 32px;
  font-weight: 700;
  margin: 0 0 8px 0;
  text-shadow: 0 2px 4px rgba(0,0,0,0.2);
}

.welcome-subtitle {
  font-size: 16px;
  opacity: 0.9;
  margin: 0;
  font-weight: 300;
}

/* 搜索栏 */
.search-section {
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
  padding: 0 20px 20px;
}

.search-container {
  max-width: 400px;
  margin: 0 auto;
}

.search-input-wrapper {
  display: flex;
  align-items: center;
  background: rgba(255, 255, 255, 0.95);
  border-radius: 25px;
  padding: 8px 16px;
  box-shadow: 0 4px 15px rgba(0,0,0,0.1);
  backdrop-filter: blur(10px);
}

.search-icon {
  margin-right: 8px;
  font-size: 16px;
  color: #666;
}

.search-input {
  flex: 1;
  border: none;
  outline: none;
  font-size: 16px;
  padding: 8px 0;
  background: transparent;
  color: #333;
}

.search-input::placeholder {
  color: #999;
}

.search-btn {
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 20px;
  font-size: 14px;
  cursor: pointer;
  margin-left: 8px;
  transition: all 0.3s ease;
}

.search-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(79, 172, 254, 0.4);
}
/* 商家列表 */
.merchant-list-section {
  background: white;
  margin-top: 8px;
  border-radius: 20px 20px 0 0;
}

.filter-bar {
  padding: 20px 16px 16px;
  border-bottom: 1px solid #f0f0f0;
}

.section-header {
  margin-bottom: 16px;
}

.section-title {
  font-size: 24px;
  font-weight: 700;
  color: #333;
  margin: 0 0 4px 0;
}

.section-subtitle {
  font-size: 14px;
  color: #666;
  margin: 0;
}

.sort-options {
  display: flex;
  gap: 12px;
  flex-wrap: wrap;
}

.sort-btn {
  padding: 8px 16px;
  border: 1px solid #e0e0e0;
  background: white;
  color: #333;
  border-radius: 20px;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.sort-btn:hover {
  border-color: #4facfe;
  color: #4facfe;
}

.sort-btn.active {
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
  color: white;
  border-color: transparent;
  box-shadow: 0 2px 8px rgba(79, 172, 254, 0.3);
}

.loading-container {
  text-align: center;
  padding: 40px 16px;
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 3px solid #f3f3f3;
  border-top: 3px solid #4facfe;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 16px;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.empty-state {
  text-align: center;
  padding: 60px 16px;
}

.empty-icon {
  font-size: 48px;
  margin-bottom: 16px;
}

.merchants-grid {
  padding: 16px;
}

.merchant-card {
  background: white;
  border-radius: 12px;
  margin-bottom: 16px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
  cursor: pointer;
  transition: transform 0.3s ease, box-shadow 0.3s ease;
}

.merchant-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 16px rgba(0,0,0,0.15);
}

.merchant-image {
  position: relative;
  height: 140px;
  overflow: hidden;
}

.merchant-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.online-badge, .offline-badge {
  position: absolute;
  top: 8px;
  right: 8px;
  padding: 4px 8px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 500;
}

.online-badge {
  background: #52c41a;
  color: white;
}

.offline-badge {
  background: #ff4d4f;
  color: white;
}

.merchant-info {
  padding: 16px;
}

.merchant-name {
  font-size: 18px;
  font-weight: 600;
  margin: 0 0 4px 0;
  color: #333;
}

.merchant-desc {
  font-size: 14px;
  color: #666;
  margin: 0 0 12px 0;
}

.merchant-stats {
  display: flex;
  align-items: center;
  gap: 16px;
  margin-bottom: 12px;
}

.rating {
  display: flex;
  align-items: center;
  gap: 4px;
}

.rating-stars {
  color: #ffb400;
  font-size: 16px;
}

.rating-score {
  font-weight: 600;
  font-size: 14px;
}

.rating-count {
  font-size: 12px;
  color: #999;
}

.distance {
  display: flex;
  align-items: center;
  gap: 4px;
  font-size: 14px;
  color: #666;
}

.merchant-tags {
  display: flex;
  gap: 6px;
  margin-bottom: 12px;
}

.tag {
  padding: 2px 6px;
  background: #f0f8ff;
  color: #4facfe;
  font-size: 12px;
  border-radius: 4px;
}

.merchant-bottom {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.delivery-info {
  display: flex;
  gap: 12px;
  font-size: 12px;
  color: #666;
}

.min-order {
  font-size: 12px;
  color: #ff4d4f;
}

.load-more {
  text-align: center;
  padding: 20px;
}

.load-more-btn {
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
  color: white;
  border: none;
  padding: 12px 24px;
  border-radius: 20px;
  cursor: pointer;
  font-size: 14px;
}
</style>
