<template>
  <div class="home-page">
    <!-- 右上角地址选择器 -->
    <div class="top-right-address">
      <div class="address-selector" @click="handleAddressClick">
        <div class="address-content">
          <i class="address-icon">📍</i>
          <div class="address-text">
            <div class="address-label">当前地址</div>
            <div class="address-name">
              {{ currentAddress?.detailedAddress || '请选择地址' }}
            </div>
          </div>
          <i class="dropdown-icon">▼</i>
        </div>
      </div>
    </div>

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
              <img class="cover-image" :src="getMerchantImageByIndex(index)" alt="商家图片" />
              <div class="merchant-status">
                <span v-if="merchant.status === 0" class="status-review">审核中</span>
                <span v-else-if="merchant.status === 1" class="status-open">营业中</span>
                <span v-else-if="merchant.status === 2" class="status-closed">休息中</span>
                <span v-else-if="merchant.status === 3" class="status-banned">已封禁</span>
                <span v-else class="status-unknown">未知状态</span>
              </div>
            </div>

            <!-- 商家信息 -->
            <div class="merchant-info">
              <div class="merchant-header">
                <h3 class="merchant-name">{{ merchant.merchantName || merchant.name || '商家名称缺失' }}</h3>
                <div v-if="merchant.rating" class="merchant-rating">
                  <span class="rating-stars">⭐</span>
                  <span class="rating-score">{{ merchant.rating }}</span>
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
              
              <!-- 商家地址和联系方式 -->
              <div class="merchant-contact-info">
                <div v-if="merchant.location || merchant.address" class="merchant-location">
                  <i class="location-icon">📍</i>
                  <span class="location-text">{{ merchant.location || merchant.address }}</span>
                </div>
                <div v-if="merchant.contactInfo || merchant.phone" class="merchant-phone">
                  <i class="phone-icon">📞</i>
                  <span class="phone-text">{{ merchant.contactInfo || merchant.phone }}</span>
                </div>
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
                    <img :src="dish.image || getRandomFoodImage()" class="dish-image" :alt="dish.name" />
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

    <!-- 地址选择弹窗 -->
    <div v-if="showAddressModal" class="address-modal" @click="closeAddressModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3 class="modal-title">选择当前地址</h3>
          <button @click="closeAddressModal" class="close-btn">✕</button>
        </div>
        
        <div class="modal-body">
          <!-- 加载状态 -->
          <div v-if="addressLoading" class="loading-container">
            <div class="loading-spinner"></div>
            <p>正在加载地址...</p>
          </div>
          
          <!-- 地址列表 -->
          <div v-else-if="userAddresses.length > 0" class="address-list">
            <div 
              v-for="address in userAddresses" 
              :key="address.addressId"
              class="address-item"
              :class="{ active: currentAddress?.addressId === address.addressId }"
              @click="selectAddress(address)"
            >
              <div class="address-info">
                <div class="address-header">
                  <span class="receiver-name">{{ address.receiverName }}</span>
                  <span class="receiver-phone">{{ address.receiverPhone }}</span>
                  <span v-if="address.isDefault" class="default-badge">默认</span>
                </div>
                <div class="address-detail">{{ address.detailedAddress }}</div>
              </div>
              <div class="select-icon" v-if="currentAddress?.addressId === address.addressId">✓</div>
            </div>
          </div>
          
          <!-- 空状态 -->
          <div v-else class="empty-addresses">
            <div class="empty-icon">📍</div>
            <p>暂无收货地址</p>
            <button @click="goToAddresses" class="add-address-btn">添加地址</button>
          </div>
        </div>
        
        <div class="modal-footer">
          <button @click="goToAddresses" class="manage-btn">管理地址</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, reactive, onMounted, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import { merchantAPI } from '@/api/merchant.js'
import { addressAPI } from '@/api/user.js'
import { getMerchantImage, getRandomFoodImage, getMerchantOrRandomImage } from '@/utils/imageUtils.js'

export default {
  setup() {
    const router = useRouter()
    
    // 响应式数据
    const searchKeyword = ref('')
    const loading = ref(false)
    const merchants = ref([])
    const currentSort = ref('recommended')
    const currentPage = ref(1)
    const hasMore = ref(true)
    
    // 添加缺失的变量
    const userLocation = ref({ addressId: '' })
    const merchantImages = [
      'https://picsum.photos/id/1011/400/200',
      'https://picsum.photos/id/1012/400/200',
      'https://picsum.photos/id/1015/400/200',
      'https://picsum.photos/id/1025/400/200',
      'https://picsum.photos/id/1035/400/200',
      'https://picsum.photos/id/1041/400/200',
      'https://picsum.photos/id/1043/400/200',
      'https://picsum.photos/id/1050/400/200',
      'https://picsum.photos/id/1062/400/200',
      'https://picsum.photos/id/1069/400/200',
      'https://picsum.photos/id/1074/400/200',
      'https://picsum.photos/id/1080/400/200',
      'https://picsum.photos/id/1084/400/200'
    ]
    
    // 排序选项与tag枚举值的映射
    const sortOptions = [
      { label: '推荐', value: 'recommend' },
      { label: '评分', value: 'rating', tag: 0 },
      { label: '距离', value: 'distance', tag: 1 },
      { label: '销量', value: 'sales', tag: 2 },
      { label: '配送时间', value: 'delivery_time', tag: 3 }
    ]

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
            // 移除默认评分、默认评价数、默认距离
            deliveryFee: Math.floor(Math.random() * 5) + 1, // 随机配送费 1-5元
            deliveryTime: `${Math.floor(Math.random() * 30) + 15}-${Math.floor(Math.random() * 30) + 30}`, // 随机配送时间
            minOrderAmount: Math.floor(Math.random() * 15) + 15, // 随机起送价 15-30元
            status: item.status !== undefined ? item.status : 1, // 保留原始状态值
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
              deliveryFee: Math.floor(Math.random() * 5) + 1,
              deliveryTime: `${Math.floor(Math.random() * 30) + 15}-${Math.floor(Math.random() * 30) + 30}`,
              minOrderAmount: Math.floor(Math.random() * 15) + 15,
              status: 1, // 营业中状态
              isOnline: true,
              tags: ['川菜', '香锅', '麻辣'],
              phone: '13800001111',
              address: '北京市朝阳区建国路88号'
            },
            {
              merchantId: 'M002',
              name: '新店审核中',
              description: '新开业店铺，正在审核中',
              imageUrl: '/images/merchant2.jpg',
              deliveryFee: Math.floor(Math.random() * 5) + 1,
              deliveryTime: `${Math.floor(Math.random() * 30) + 15}-${Math.floor(Math.random() * 30) + 30}`,
              minOrderAmount: Math.floor(Math.random() * 15) + 15,
              status: 0, // 审核中状态
              isOnline: false,
              tags: ['新店', '特色小吃'],
              phone: '13800002222',
              address: '北京市海淀区中关村大街1号'
            },
            {
              merchantId: 'M003',
              name: '暂停营业店',
              description: '店铺暂时休息中，稍后恢复营业',
              imageUrl: '/images/merchant3.jpg',
              deliveryFee: Math.floor(Math.random() * 5) + 1,
              deliveryTime: `${Math.floor(Math.random() * 30) + 15}-${Math.floor(Math.random() * 30) + 30}`,
              minOrderAmount: Math.floor(Math.random() * 15) + 15,
              status: 2, // 休息中状态
              isOnline: false,
              tags: ['西餐', '牛排', '休息中'],
              phone: '13800003333',
              address: '北京市朝阳区三里屯10号'
            },
            {
              merchantId: 'M004',
              name: '违规封禁店',
              description: '因违规操作被系统封禁',
              imageUrl: '/images/merchant4.jpg',
              deliveryFee: Math.floor(Math.random() * 5) + 1,
              deliveryTime: `${Math.floor(Math.random() * 30) + 15}-${Math.floor(Math.random() * 30) + 30}`,
              minOrderAmount: Math.floor(Math.random() * 15) + 15,
              status: 3, // 封禁状态
              isOnline: false,
              tags: ['已封禁'],
              phone: '13800004444',
              address: '北京市东城区王府井大街88号'
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
    const changeSortBy = async (sortBy) => {
      currentSort.value = sortBy
      
      // 查找对应的tag值
      const option = sortOptions.find(opt => opt.value === sortBy)
      
      if (option && option.tag !== undefined) {
        // 调用真实的byTag接口
        await fetchMerchantsByTag(option.tag)
      } else {
        // 推荐或其他情况，使用原有接口
        await fetchMerchants(true)
      }
    }

    // 新增：通过Tag筛选商家的方法
    const fetchMerchantsByTag = async (tag) => {
      try {
        loading.value = true
        const params = {
          tag: tag,
          page: 1,
          size: 20,
          addressId: currentAddress.value?.addressId || localStorage.getItem('userAddressId') || '1'
        }
        
        const response = await merchantAPI.getMerchantListByTag(params)
        
        if (response && response.code === 0 && response.data) {
          // 获取商家ID列表
          const merchantIds = response.data.map(item => item.merchantId)
          
          if (merchantIds.length > 0) {
            // 并发获取所有商家的详细信息
            const merchantDetails = await Promise.all(
              merchantIds.map(async (merchantId) => {
                try {
                  const detailResponse = await merchantAPI.getMerchantById(merchantId)
                  if (detailResponse && detailResponse.code === 0 && detailResponse.data) {
                    return detailResponse.data
                  }
                  return null
                } catch (error) {
                  console.error(`获取商家 ${merchantId} 详情失败:`, error)
                  return null
                }
              })
            )
            
            // 过滤掉获取失败的商家，并格式化数据
            const validMerchants = merchantDetails
              .filter(merchant => merchant !== null)
              .map((merchant) => ({
                merchantId: merchant.merchantId,
                merchantName: merchant.merchantName || `商家-${merchant.merchantId}`,
                name: merchant.merchantName || `商家-${merchant.merchantId}`,
                description: merchant.description || `商家ID: ${merchant.merchantId}`,
                // 移除默认评分
                // 随机生成配送相关信息
                deliveryFee: Math.floor(Math.random() * 5) + 1, // 随机配送费 1-5元
                deliveryTime: `${Math.floor(Math.random() * 30) + 15}-${Math.floor(Math.random() * 30) + 30}`, // 随机配送时间
                minOrderAmount: Math.floor(Math.random() * 15) + 15, // 随机起送价 15-30元
                status: merchant.status || 1,
                isOnline: (merchant.status || 1) === 1,
                tags: ['精选商家'],
                contactInfo: merchant.contactInfo || '',
                location: merchant.location || '',
                salesQty: 0, // 如果有销量数据可以添加
                timestamp: merchant.timestamp || Date.now()
              }))
            
            merchants.value = validMerchants
          } else {
            merchants.value = []
          }
        } else {
          // 如果 byTag 接口失败，降级使用前端排序
          await sortMerchantsLocally(getTagNameByValue(tag))
        }
      } catch (error) {
        console.error('获取筛选商家列表失败:', error)
        // 降级：使用前端排序
        await sortMerchantsLocally(getTagNameByValue(tag))
      } finally {
        loading.value = false
      }
    }
    
    // 新增：根据tag值获取排序类型名称
    const getTagNameByValue = (tag) => {
      const tagMap = {
        0: 'rating',
        1: 'distance', 
        2: 'sales',
        3: 'delivery_time'
      }
      return tagMap[tag] || 'recommend'
    }

    // 新增：前端排序方法（降级方案）
    const sortMerchantsLocally = async (sortBy) => {
      // 如果当前没有商家数据，先获取一次
      if (merchants.value.length === 0) {
        await fetchMerchants(true)
      }
      
      // 前端排序
      const sorted = [...merchants.value]
      
      switch (sortBy) {
        case 'rating':
          // 仅对有评分的商家进行排序，没有评分的排在后面
          sorted.sort((a, b) => {
            if (a.rating && b.rating) return b.rating - a.rating
            if (a.rating) return -1
            if (b.rating) return 1
            return 0
          })
          break
        case 'distance':
          // 由于已移除默认距离，这里按照商家ID排序作为替代
          sorted.sort((a, b) => a.merchantId.localeCompare(b.merchantId))
          break
        case 'sales':
          sorted.sort((a, b) => (b.salesQty || 0) - (a.salesQty || 0))
          break
        case 'delivery_time':
          // 使用配送时间的最小值进行排序
          sorted.sort((a, b) => {
            const getMinTime = (timeStr) => {
              const match = String(timeStr).match(/^(\d+)/)
              return match ? parseInt(match[1]) : 999
            }
            return getMinTime(a.deliveryTime) - getMinTime(b.deliveryTime)
          })
          break
        default:
          // 推荐排序保持原序
          break
      }
      
      merchants.value = sorted
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
      fetchUserAddresses()
      fetchMerchants(true)
    })
    
    // 商家图片处理函数
    const getMerchantImageByIndex = (index) => {
      // 如果有商家数据，使用商家名称获取图片
      if (merchants.value && merchants.value[index]) {
        return getMerchantOrRandomImage(merchants.value[index].merchantName || merchants.value[index].name);
      }
      // 否则返回随机食物图片
      return getRandomFoodImage();
    }
    
    // 新增：地址相关数据
    const showAddressModal = ref(false)
    const userAddresses = ref([])
    const currentAddress = ref(null)
    const addressLoading = ref(false)
    
    // 获取用户地址列表
    const fetchUserAddresses = async () => {
      try {
        addressLoading.value = true
        const userId = localStorage.getItem('userId')
        
        if (!userId) {
          userAddresses.value = []
          return
        }
        
        const response = await addressAPI.getUserAddresses(userId)
        
        if (response && response.code === 0 && response.data) {
          const addressList = Array.isArray(response.data) ? response.data : []
          userAddresses.value = addressList
          
          // 设置当前地址
          if (addressList.length > 0) {
            // 优先使用保存的地址
            const savedAddress = localStorage.getItem('currentSelectedAddress')
            let selectedAddress = null
            
            if (savedAddress) {
              try {
                const parsed = JSON.parse(savedAddress)
                selectedAddress = addressList.find(addr => addr.addressId === parsed.addressId)
              } catch (e) {
                console.error('解析保存地址失败:', e)
              }
            }
            
            // 如果没有保存的地址，使用默认地址
            if (!selectedAddress) {
              selectedAddress = addressList.find(addr => addr.isDefault === 1 || addr.isDefault === true)
            }
            
            // 如果没有默认地址，使用第一个
            if (!selectedAddress) {
              selectedAddress = addressList[0]
            }
            
            if (selectedAddress) {
              currentAddress.value = selectedAddress
              localStorage.setItem('currentSelectedAddress', JSON.stringify(selectedAddress))
              localStorage.setItem('userAddressId', selectedAddress.addressId)
            }
          }
          
        } else {
          userAddresses.value = []
        }
      } catch (error) {
        console.error('获取地址列表失败:', error)
        userAddresses.value = []
      } finally {
        addressLoading.value = false
      }
    }
    
    // 选择地址
    const selectAddress = (address) => {
      currentAddress.value = address
      localStorage.setItem('currentSelectedAddress', JSON.stringify(address))
      localStorage.setItem('userAddressId', address.addressId)
      closeAddressModal()
      
      // 地址变更后重新加载商家列表
      fetchMerchants(true)
    }
    
    // 关闭地址选择弹窗
    const closeAddressModal = () => {
      showAddressModal.value = false
    }
    
    // 跳转到地址管理页面
    const goToAddresses = () => {
      closeAddressModal()
      router.push('/addresses')
    }
    
    // 修改地址选择器点击事件
    const handleAddressClick = () => {
      showAddressModal.value = true
      
      // 如果没有地址数据，重新获取
      if (!userAddresses.value || userAddresses.value.length === 0) {
        fetchUserAddresses()
      }
    }

    // 监听路由变化，从地址管理页面返回时刷新地址列表
    watch(() => router.currentRoute.value.path, (newPath) => {
      if (newPath === '/' || newPath === '/home') {
        // 延迟一下，确保地址管理页面的操作已完成
        setTimeout(() => {
          fetchUserAddresses()
        }, 100)
      }
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
      getMerchantImageByIndex,
      getRandomFoodImage,
      showAddressModal,
      userAddresses,
      currentAddress,
      addressLoading,
      fetchUserAddresses,
      selectAddress,
      closeAddressModal,
      goToAddresses,
      handleAddressClick,
      fetchMerchantsByTag,
      sortMerchantsLocally // 新增
    }
  }
}
</script>

<style scoped>
.home-page {
  background: linear-gradient(135deg, #4facfe 0%, #764ba2 100%);
  min-height: 100vh;
  padding-bottom: 80px;
  position: relative;
}

/* 右上角地址选择器 */
.top-right-address {
  position: fixed;
  top: 20px;
  right: 20px;
  z-index: 1000;
}

.top-right-address .address-selector {
  cursor: pointer;
}

.top-right-address .address-content {
  display: flex;
  align-items: center;
  background: rgba(255, 255, 255, 0.95);
  border-radius: 25px;
  padding: 8px 16px;
  transition: all 0.3s;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  backdrop-filter: blur(10px);
  min-width: 180px;
  max-width: 280px;
}

.top-right-address .address-content:hover {
  background: rgba(255, 255, 255, 1);
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.2);
}

.top-right-address .address-icon {
  font-size: 16px;
  margin-right: 8px;
  color: #4facfe;
}

.top-right-address .address-text {
  flex: 1;
  text-align: left;
  color: #333;
}

.top-right-address .address-label {
  font-size: 11px;
  opacity: 0.7;
  margin-bottom: 2px;
  color: #666;
}

.top-right-address .address-name {
  font-size: 13px;
  font-weight: 500;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  max-width: 180px;
  color: #333;
}

.top-right-address .dropdown-icon {
  font-size: 10px;
  opacity: 0.7;
  margin-left: 8px;
  transition: transform 0.3s;
  color: #666;
}

.top-right-address .address-content:hover .dropdown-icon {
  transform: rotate(180deg);
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

.status-review {
  background: #ffc107;
  color: #333;
  padding: 4px 10px;
  border-radius: 14px;
  font-size: 13px;
  font-weight: 600;
  box-shadow: 0 2px 8px rgba(255,193,7,0.10);
}

.status-banned {
  background: #6c757d;
  color: white;
  padding: 4px 10px;
  border-radius: 14px;
  font-size: 13px;
  font-weight: 600;
  box-shadow: 0 2px 8px rgba(108,117,125,0.10);
}

.status-unknown {
  background: #17a2b8;
  color: white;
  padding: 4px 10px;
  border-radius: 14px;
  font-size: 13px;
  font-weight: 600;
  box-shadow: 0 2px 8px rgba(23,162,184,0.10);
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

.merchant-contact-info {
  margin-bottom: 10px;
  width: 100%;
  padding-top: 4px;
}

.merchant-location, .merchant-phone {
  display: flex;
  align-items: center;
  gap: 6px;
  margin-bottom: 4px;
  font-size: 13px;
  color: #666;
  width: 100%;
}

.location-icon, .phone-icon {
  font-size: 14px;
  color: #4facfe;
  flex-shrink: 0;
}

.location-text, .phone-text {
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  max-width: calc(100% - 20px);
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

/* 地址选择弹窗样式 */
.address-modal {
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
  max-width: 500px;
  width: 100%;
  max-height: 70vh;
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px;
  border-bottom: 1px solid #f0f0f0;
}

.modal-title {
  font-size: 18px;
  font-weight: 600;
  color: #333;
  margin: 0;
}

.close-btn {
  width: 32px;
  height: 32px;
  background: none;
  border: none;
  font-size: 18px;
  color: #999;
  cursor: pointer;
  border-radius: 50%;
  transition: all 0.3s;
}

.close-btn:hover {
  background: #f8f9fa;
  color: #333;
}

.modal-body {
  flex: 1;
  overflow-y: auto;
  padding: 20px;
}

.loading-container {
  text-align: center;
  padding: 40px 20px;
}

.loading-spinner {
  width: 32px;
  height: 32px;
  border: 3px solid #e1e5e9;
  border-top: 3px solid #4facfe;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 16px;
}

.address-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.address-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 16px;
  border: 2px solid #f0f0f0;
  border-radius: 12px;
  cursor: pointer;
  transition: all 0.3s;
}

.address-item:hover {
  border-color: #4facfe;
}

.address-item.active {
  border-color: #4facfe;
  background: rgba(79, 172, 254, 0.05);
}

.address-info {
  flex: 1;
}

.address-header {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 6px;
}

.receiver-name {
  font-size: 16px;
  font-weight: 600;
  color: #333;
}

.receiver-phone {
  font-size: 14px;
  color: #666;
}

.default-badge {
  padding: 2px 8px;
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
  color: white;
  font-size: 12px;
  border-radius: 10px;
}

.address-detail {
  font-size: 14px;
  color: #666;
  line-height: 1.4;
}

.select-icon {
  width: 24px;
  height: 24px;
  background: #4facfe;
  color: white;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 16px;
  font-weight: bold;
}

.empty-addresses {
  text-align: center;
  padding: 40px 20px;
}

.empty-icon {
  font-size: 48px;
  margin-bottom: 16px;
}

.add-address-btn {
  padding: 10px 24px;
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
  color: white;
  border: none;
  border-radius: 20px;
  font-size: 14px;
  cursor: pointer;
  margin-top: 16px;
}

.modal-footer {
  border-top: 1px solid #f0f0f0;
  padding: 16px 20px;
  text-align: center;
}

.manage-btn {
  padding: 10px 24px;
  background: #f8f9fa;
  color: #333;
  border: 1px solid #ddd;
  border-radius: 20px;
  font-size: 14px;
  cursor: pointer;
}

.manage-btn:hover {
  background: #e9ecef;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* 响应式设计 */
@media (max-width: 768px) {
  .merchants-grid {
    grid-template-columns: 1fr;
    gap: 12px;
  }
  .merchant-meta {
    flex-direction: column;
    gap: 8px;
  }
  
  .top-right-address {
    top: 10px;
    right: 10px;
  }
  
  .top-right-address .address-content {
    min-width: 150px;
    max-width: 200px;
    padding: 6px 12px;
  }
  
  .top-right-address .address-name {
    max-width: 120px;
    font-size: 12px;
  }
  
  .modal-content {
    margin: 10px;
    max-width: none;
  }
}
</style>
