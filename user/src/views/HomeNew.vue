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
      <div class="merchants-container">
        <!-- 筛选和排序 -->
        <div class="filter-bar">
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

        <!-- 商家网格卡片布局 -->
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
            v-for="(merchant, index) in merchants"
            :key="merchant.merchantId"
            class="merchant-card"
            @click="goToMerchant(merchant.merchantId)"
          >
            <!-- 商家封面图 -->
            <div class="merchant-cover">
              <img class="cover-image" :src="getMerchantImage(index)" alt="商家图片" />
              <div class="merchant-status">
                <span v-if="merchant.status === 1" class="status-open">营业中</span>
                <span v-else class="status-closed">休息中</span>
              </div>
            </div>

            <!-- 商家信息 -->
            <div class="merchant-info">
              <div class="merchant-header">
                <h3 class="merchant-name">{{ merchant.merchantName || merchant.name || '商家名称缺失' }}</h3>
                <div class="merchant-rating">
                  <span class="rating-stars">⭐</span>
                  <span class="rating-score">{{ merchant.rating || 4.5 }}</span>
                </div>
              </div>
              <div class="merchant-meta">
                <div class="meta-item">
                  <span class="meta-icon">🚚</span>
                  <span class="meta-text">{{ merchant.deliveryTime || '30-45' }}分钟</span>
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
              <!-- 特色菜品预览（如有） -->
              <div v-if="merchant.featuredDishes && merchant.featuredDishes.length" class="featured-dishes">
                <div class="featured-title">招牌菜：</div>
                <div class="dishes-preview">
                  <div
                    v-for="dish in merchant.featuredDishes"
                    :key="dish.dishId || dish.name"
                    class="dish-preview"
                  >
                    <img :src="dish.image || '/default-dish.jpg'" class="dish-image" :alt="dish.name" />
                    <div class="dish-info">
                      <div class="dish-name">{{ dish.name }}</div>
                      <div class="dish-price">¥{{ dish.price }}</div>
                    </div>
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
  </div>
</template>

<script>
import { ref, reactive, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { merchantAPI } from '@/api/merchant.js'

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
          merchantName: searchKeyword.value || undefined,
          location: undefined, // 可根据实际需求补充地理位置参数
          status: undefined // 可根据实际需求补充商家状态参数
        }
        // 调用真实API
        const response = await merchantAPI.getMerchantList(params)
        console.log('商家接口返回数据:', response)
        if (response.code === 0) {
          console.log('获取商家列表成功:', response.data)
          // 字段映射，保证模板字段一致
          const mappedList = (response.data || []).map(item => ({
            merchantId: item.merchantId,
            name: item.merchantName || '',
            description: item.description || '',
            imageUrl: '/default-merchant.jpg', // 默认图片
            rating: 4.5, // 默认评分
            reviewCount: 999, // 默认评价数
            distance: '1.2', // 默认距离
            deliveryFee: 3, // 默认配送费
            deliveryTime: '30-45', // 默认配送时间
            minOrderAmount: '', // 默认起送价
            isOnline: item.status === 1,
            tags: [], // 默认标签
            phone: item.contactInfo || '',
            address: item.location || ''
          }))
          if (reset) {
            merchants.value = mappedList
            currentPage.value = 1
          } else {
            merchants.value.push(...mappedList)
          }
          hasMore.value = (response.data?.length || 0) >= params.size
          if (!reset) {
            currentPage.value++
          }
        } else {
          console.error('获取商家列表失败:', response.code, response.message)
        }
      } catch (error) {
        console.error('获取商家列表失败:', error)
        if (reset || merchants.value.length === 0) {
          // 使用硬编码的降级数据
          merchants.value = [
            {
              merchantId: 'M001',
              name: '麻辣香锅店',
              description: '正宗四川口味，香辣过瘾',
              imageUrl: '/images/merchant1.jpg',
              rating: 4.6,
              reviewCount: 1256,
              distance: 0.8,
              deliveryFee: 3,
              deliveryTime: '25-35',
              minOrderAmount: 20,
              isOnline: true,
              tags: ['川菜', '香锅', '麻辣'],
              phone: '13800001111',
              address: '北京市朝阳区建国路88号'
            }
          ]
        }
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
    
    // 页面挂载时初始化
    onMounted(() => {
      fetchMerchants(true)
    })
    
    const imageList = [
      'https://picsum.photos/400/200?random=1',
      'https://picsum.photos/400/200?random=2',
      'https://picsum.photos/400/200?random=3',
      'https://picsum.photos/400/200?random=4',
      'https://picsum.photos/400/200?random=5',
      // ...更多图片
    ]
    
    // 轮询分配图片
    const getMerchantImage = (index) => {
      return imageList[index % imageList.length]
    }
    
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
  background: linear-gradient(135deg, #4facfe 0%, #764ba2 100%);
  min-height: 100vh;
  padding-bottom: 80px;
}

/* 欢迎区域 */
.welcome-section {
  background: transparent;
  color: white;
  text-align: center;
  padding: 32px 20px 12px;
}

.welcome-title {
  font-size: 36px;
  font-weight: 700;
  margin-bottom: 8px;
  text-shadow: 0 2px 8px rgba(0,0,0,0.18);
}

.welcome-subtitle {
  font-size: 16px;
  opacity: 0.92;
  font-weight: 400;
}

/* 搜索栏 */
.search-section {
  background: transparent;
  padding: 0 20px 12px;
}

.search-container {
  max-width: 420px;
  margin: 0 auto;
}

.search-input-wrapper {
  display: flex;
  align-items: center;
  background: rgba(255,255,255,0.98);
  border-radius: 24px;
  padding: 8px 18px;
  box-shadow: 0 2px 12px rgba(79,172,254,0.08);
}

.search-icon {
  margin-right: 8px;
  font-size: 18px;
  color: #4facfe;
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

.search-btn {
  background: linear-gradient(135deg, #4facfe 0%, #764ba2 100%);
  color: white;
  border: none;
  padding: 8px 18px;
  border-radius: 18px;
  font-size: 15px;
  cursor: pointer;
  margin-left: 8px;
  transition: all 0.3s;
}

.search-btn:hover {
  opacity: 0.92;
}

/* 筛选栏 */
.filter-bar {
  display: flex;
  flex-direction: column;
  align-items: center; /* 居中 */
  gap: 12px;
  margin-bottom: 24px;
  margin-top: 12px;
  padding: 24px 0 0 0;
}

.sort-options {
  display: flex;
  gap: 18px;
  margin-top: 8px;
  justify-content: center;
}

.sort-btn {
  background: #fff;
  color: #764ba2;
  border: none;
  border-radius: 18px;
  padding: 10px 28px;
  font-size: 17px;
  font-weight: 500;
  cursor: pointer;
  box-shadow: 0 4px 18px rgba(76,175,254,0.13);
  transition: background 0.2s, color 0.2s, box-shadow 0.2s;
  letter-spacing: 1px;
}

.sort-btn.active,
.sort-btn:hover {
  background: linear-gradient(135deg, #4facfe 0%, #764ba2 100%);
  color: #fff;
  box-shadow: 0 8px 32px rgba(76,175,254,0.18);
}

/* 商家网格布局 */
.merchants-container {
  display: flex;
  flex-direction: column;
  align-items: center;
}

/* 商家网格居中且最大宽度 */
.merchants-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr); /* 固定三列 */
  gap: 22px;
  margin-bottom: 40px;
  padding: 12px;
  max-width: 1400px;
  margin-left: auto;
  margin-right: auto;
}

.merchant-card {
  background: rgba(255,255,255,0.98);
  border-radius: 18px;
  overflow: hidden;
  box-shadow: 0 4px 18px rgba(76,175,254,0.10);
  transition: all 0.3s;
  cursor: pointer;
  border: none;
}

.merchant-card:hover {
  transform: translateY(-4px) scale(1.02);
  box-shadow: 0 8px 32px rgba(76,175,254,0.18);
}

.merchant-cover {
  position: relative;
  height: 160px;
  overflow: hidden;
}

.cover-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s;
}

.merchant-card:hover .cover-image {
  transform: scale(1.06);
}

.merchant-status {
  position: absolute;
  top: 12px;
  right: 12px;
}

.status-open {
  background: #28a745;
  color: white;
  padding: 4px 10px;
  border-radius: 14px;
  font-size: 13px;
  font-weight: 600;
  box-shadow: 0 2px 8px rgba(40,167,69,0.10);
}

.status-closed {
  background: #e74c3c;
  color: white;
  padding: 4px 10px;
  border-radius: 14px;
  font-size: 13px;
  font-weight: 600;
  box-shadow: 0 2px 8px rgba(231,76,60,0.10);
}

.merchant-info {
  padding: 16px;
}

.merchant-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 10px;
}

.merchant-name {
  font-size: 19px;
  font-weight: 700;
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
  font-size: 15px;
}

.rating-score {
  font-size: 15px;
  font-weight: 600;
  color: #333;
}

.merchant-meta {
  display: flex;
  gap: 14px;
  margin-bottom: 10px;
  flex-wrap: wrap;
}

.meta-item {
  display: flex;
  align-items: center;
  gap: 4px;
}

.meta-icon {
  font-size: 13px;
}

.meta-text {
  font-size: 13px;
  color: #666;
}

.merchant-tags {
  display: flex;
  gap: 6px;
  margin-bottom: 10px;
  flex-wrap: wrap;
}

.tag {
  background: #f0f8ff;
  color: #4facfe;
  padding: 2px 8px;
  border-radius: 12px;
  font-size: 12px;
}

.featured-dishes {
  border-top: 1px solid #f0f0f0;
  padding-top: 10px;
}

.featured-title {
  font-size: 14px;
  font-weight: 600;
  color: #333;
  margin-bottom: 6px;
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
  min-width: 110px;
  padding: 6px;
  background: #f8f9fa;
  border-radius: 8px;
}

.dish-image {
  width: 34px;
  height: 34px;
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

.load-more {
  text-align: center;
  padding: 20px;
}

.load-more-btn {
  background: linear-gradient(135deg, #4facfe 0%, #764ba2 100%);
  color: white;
  border: none;
  padding: 12px 28px;
  border-radius: 22px;
  cursor: pointer;
  font-size: 15px;
  font-weight: 500;
}

@media (max-width: 768px) {
  .merchants-grid {
    grid-template-columns: 1fr;
    gap: 12px;
  }
  .merchant-meta {
    flex-direction: column;
    gap: 8px;
  }
}
</style>
