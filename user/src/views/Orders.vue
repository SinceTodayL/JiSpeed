<template>
  <div class="orders-page">
    <!-- 顶部导航 -->
    <div class="header">
      <button @click="goBack" class="back-btn">
        <i class="back-icon">←</i>
      </button>
      <h1 class="header-title">我的订单</h1>
    </div>

    <!-- 订单状态筛选 -->
    <div class="status-filter">
      <div 
        v-for="status in orderStatuses"
        :key="status.value"
        :class="['filter-item', { active: activeStatus === status.value }]"
        @click="filterByStatus(status.value)"
      >
        <span class="filter-text">{{ status.label }}</span>
        <span v-if="status.count > 0" class="filter-count">{{ status.count }}</span>
      </div>
    </div>

    <!-- 订单列表 -->
    <div class="orders-container">
      <!-- 加载状态 -->
      <div v-if="loading" class="loading-container">
        <div class="loading-spinner"></div>
        <p>正在加载订单...</p>
      </div>

      <!-- 订单列表 -->
      <div v-else-if="orders.length > 0" class="orders-list">
        <div 
          v-for="order in orders"
          :key="order.orderId"
          class="order-card"
          @click="viewOrderDetail(order.orderId)"
        >
          <!-- 订单头部 -->
          <div class="order-header">
            <div class="merchant-info center">
              <img 
                :src="order.merchantLogo || '/api/placeholder/32/32'"
                :alt="order.merchantName"
                class="merchant-logo"
              />
              <span class="merchant-name">{{ order.merchantName }}</span>
            </div>
            <div class="order-status">
              <span :class="['status-text', getStatusClass(order.orderStatus)]">
                {{ order.orderStatusText }}
              </span>
            </div>
          </div>

          <!-- 订单商品 -->
          <div class="order-items">
            <div 
              v-for="(item, index) in order.orderItems"
              :key="item.dishId"
              class="order-item"
            >
              <img 
                :src="item.coverUrl || '/api/placeholder/60/60'"
                :alt="item.dishName"
                class="item-image"
              />
              <div class="item-info">
                <div class="item-name">{{ item.dishName }}</div>
                <div class="item-meta">
                  <span class="item-price">¥{{ item.price }}</span>
                  <span class="item-quantity">×{{ item.quantity }}</span>
                </div>
              </div>
            </div>
            <!-- 更多商品提示 -->
            <div v-if="(order.orderDishes && order.orderDishes.length > 2)" class="more-items">
              还有{{ order.orderDishes.length - 2 }}件商品
            </div>
          </div>

          <!-- 订单底部 -->
          <div class="order-footer">
            <div class="order-info">
              <div class="order-time">{{ formatTime(order.createAt) }}</div>
              <div class="order-amount">
                <div class="order-amount center">
                  共{{ order.itemsCount }}件商品 实付¥{{ order.finalAmount }}
                </div>
              </div>
            </div>
            
            <div class="order-actions">
              <button 
                v-if="order.orderStatus === 0"
                @click.stop="payOrder(order)"
                class="action-btn primary"
              >
                立即支付
              </button>
              
              <button 
                v-if="order.orderStatus === 3"
                @click.stop="confirmOrder(order.orderId)"
                class="action-btn primary"
              >
                确认收货
              </button>
              
              <button 
                v-if="[0, 1].includes(order.orderStatus)"
                @click.stop="cancelOrder(order.orderId)"
                class="action-btn secondary"
              >
                取消订单
              </button>
              
              <button 
                v-if="order.orderStatus === 4"
                @click.stop="reviewOrder(order.orderId)"
                class="action-btn secondary"
              >
                评价
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- 空状态 -->
      <div v-else class="empty-state">
        <div class="empty-icon">📋</div>
        <p class="empty-text">暂无订单</p>
        <button @click="goShopping" class="shop-btn">去下单</button>
      </div>
    </div>

    <!-- 分页加载 -->
    <div v-if="hasMore && !loading" class="load-more">
      <button @click="loadMore" class="load-more-btn">加载更多</button>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { orderAPI } from '../api/order'
import { merchantAPI, merchantDishAPI } from '../api/merchant'
import { ref, computed, onMounted } from 'vue'

const router = useRouter()
console.log('所有路由路径:', router.getRoutes().map(r => r.path))
const allPaths = router.getRoutes().map(r => r.path)
console.log('所有路由路径:', allPaths)
const merchantPath = allPaths.find(p => p && p.includes('merchant')) || '/merchants'

const orders = ref([])
const loading = ref(false)
const activeStatus = ref('')
const currentPage = ref(1)
const totalPages = ref(1)
const orderStatuses = ref([
  { value: '', label: '全部', count: 0 },
  { value: 0, label: '待付款', count: 0 },
  { value: 1, label: '待接单', count: 0 },
  { value: 2, label: '制作中', count: 0 },
  { value: 3, label: '配送中', count: 0 },
  { value: 4, label: '已完成', count: 0 },
  { value: 5, label: '已取消', count: 0 }
])
const hasMore = computed(() => currentPage.value < totalPages.value)

const goBack = () => {
  router.back()
}

const loadOrders = async (status = '', page = 1, append = false) => {
  loading.value = true
  try {
    const userId = localStorage.getItem('userId') || 'USER123'
    console.log('[调试] 当前 userId:', userId)
    const orderStatus = status
    const size = 10
    console.log('[调试] 请求参数:', { userId, orderStatus, size, page })
    const response = await orderAPI.getUserOrders(userId, orderStatus, size, page)
    console.log('[调试] 订单API响应:', response)
    if (response) {
      console.log('[调试] 订单API响应data:', response.data)
    }
    if (response && (response.code === 200 || response.code === 0)) {
      // 如果 data 是数组，打印每个元素类型
      if (Array.isArray(response.data)) {
        console.log('[调试] data 为数组，元素类型:', response.data.map(item => typeof item))
      } else {
        console.log('[调试] data 非数组，类型:', typeof response.data)
      }
      const orderIds = Array.isArray(response.data) ? response.data : [];
      console.log('[调试] 订单API响应订单ID数组:', orderIds);
      const detailPromises = orderIds.map(id => 
        orderAPI.getOrderById(id).catch(err => {
          console.error('[调试] 订单详情获取失败:', id, err);
          return null;
        })
      );
      const detailResults = await Promise.all(detailPromises);
      console.log('[调试] 订单详情接口返回:', detailResults)
      const newOrders = await Promise.all(
        detailResults
          .filter(res => res && (res.code === 200 || res.code === 0) && typeof res.data === 'object' && res.data)
          .map(async res => {
            const order = res.data;
            order.orderId = order.orderId || order.id || order.orderID || '';
            order.createAt = order.createAt || order.createTime || order.createdAt || '';
            order.orderItems = order.orderDishes || [];
            order.itemsCount = Array.isArray(order.orderItems) ? order.orderItems.length : 0;
            order.finalAmount = order.orderAmount || order.finalAmount || order.amount || order.totalAmount || 0;

            // 获取商家详情
            let merchantInfo = {};
            try {
              const merchantRes = await merchantAPI.getMerchantById(order.merchantId);
              if (merchantRes && merchantRes.code === 0 && merchantRes.data) {
                merchantInfo = merchantRes.data;
                order.merchantLogo = merchantInfo.logo || merchantInfo.merchantLogo || '';
                order.merchantName = merchantInfo.merchantName || merchantInfo.name || order.merchantId;
              } else {
                order.merchantLogo = '';
                order.merchantName = order.merchantId;
              }
            } catch (e) {
              order.merchantLogo = '';
              order.merchantName = order.merchantId;
            }

            // 获取菜品详情（只查前2个）
            order.orderItems = await Promise.all(
              (order.orderItems || []).slice(0, 2).map(async item => {
                let dishInfo = {};
                try {
                  const dishRes = await merchantDishAPI.getDishById(order.merchantId, item.dishId);
                  if (dishRes && dishRes.code === 0 && dishRes.data) {
                    dishInfo = dishRes.data;
                  }
                } catch (e) {}
                return {
                  dishId: item.dishId,
                  quantity: item.quantity,
                  dishName: dishInfo.dishName || dishInfo.name || item.dishId,
                  coverUrl: dishInfo.coverUrl || dishInfo.imageUrl || '',
                  price: dishInfo.price || dishInfo.dishPrice || '',
                };
              })
            );

            // 统计商品总数
            order.itemsCount = Array.isArray(order.orderDishes) ? order.orderDishes.reduce((sum, d) => sum + (d.quantity || 0), 0) : 0;
            return order;
          })
      );
      console.log('[调试] 映射后的订单详情:', newOrders);
      newOrders.forEach(order => {
        console.log('[调试] 订单ID:', order.orderId, '商品列表:', order.orderItems, '金额:', order.finalAmount);
      });
      orders.value = append ? [...orders.value, ...newOrders] : newOrders;
      console.log('[调试] 最终 orders.value:', orders.value)
      totalPages.value = newOrders.length < 10 ? page : page + 1;
      currentPage.value = page;
      updateStatusCounts();
    } else {
      console.warn('[调试] 订单API响应 code 非 200/0，未处理数据')
    }
  } catch (error) {
    console.error('[调试] 加载订单失败:', error)
  } finally {
    loading.value = false
  }
}

const updateStatusCounts = () => {
  const arr = Array.isArray(orders.value) ? orders.value : [];
  const statusCounts = {
    '': arr.length,
    0: arr.filter(o => o.orderStatus === 0).length,
    1: arr.filter(o => o.orderStatus === 1).length,
    2: arr.filter(o => o.orderStatus === 2).length,
    3: arr.filter(o => o.orderStatus === 3).length,
    4: arr.filter(o => o.orderStatus === 4).length,
    5: arr.filter(o => o.orderStatus === 5).length
  }
  orderStatuses.value.forEach(status => {
    status.count = statusCounts[status.value] || 0
  })
}

const filterByStatus = (status) => {
  if (activeStatus.value === status) return
  activeStatus.value = status
  currentPage.value = 1
  loadOrders(status, 1)
}

const loadMore = () => {
  if (hasMore.value && !loading.value) {
    loadOrders(activeStatus.value, currentPage.value + 1, true)
  }
}

const viewOrderDetail = (orderId) => {
  router.push(`/orders/${orderId}`)
}

const payOrder = (order) => {
  router.push({
    name: 'Payment',
    params: { orderId: order.orderId },
    query: { amount: order.finalAmount }
  })
}

const confirmOrder = (orderId) => {
  // TODO: 实现确认收货逻辑
  console.log('确认收货:', orderId)
}

const cancelOrder = (orderId) => {
  // TODO: 实现取消订单逻辑
  console.log('取消订单:', orderId)
}

const reviewOrder = (orderId) => {
  // TODO: 实现订单评价逻辑
  console.log('评价订单:', orderId)
}

const goShopping = () => {
  router.push(merchantPath)
}

const formatTime = (time) => {
  if (!time) return ''
  const date = new Date(time)
  const now = Date.now()
  const diff = now - date.getTime()
  if (diff < 60 * 60 * 1000) {
    return `${Math.floor(diff / (60 * 1000))}分钟前`
  } else if (diff < 24 * 60 * 60 * 1000) {
    return `${Math.floor(diff / (60 * 60 * 1000))}小时前`
  } else {
    return date.toLocaleDateString('zh-CN') + ' ' + date.toLocaleTimeString('zh-CN', {
      hour: '2-digit',
      minute: '2-digit'
    })
  }
}

const getStatusClass = (status) => {
  const statusClasses = {
    0: 'pending',
    1: 'waiting',
    2: 'processing',
    3: 'shipping',
    4: 'completed',
    5: 'cancelled'
  }
  return statusClasses[status] || ''
}

const formatOrder = (order) => {
  return JSON.stringify(order, null, 2)
}

onMounted(() => {
  loadOrders()
})
</script>

<style scoped>

.orders-page {
  min-height: 100vh;
  background: linear-gradient(135deg, #e3f0ff 0%, #ffffff 100%);
}

.order-card {
  background: linear-gradient(135deg, #f8fbff 0%, #e3f0ff 100%);
  border-radius: 32px;
  box-shadow: 0 2px 12px rgba(0, 123, 255, 0.08);
  margin-bottom: 24px;
  padding: 20px 24px 16px 24px;
  transition: box-shadow 0.3s;
}
.order-card:hover {
  box-shadow: 0 6px 24px rgba(0, 123, 255, 0.18);
  transform: translateY(-2px);
}

.order-header {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-bottom: 16px;
  padding-bottom: 12px;
  border-bottom: 1px solid #e3f0ff;
}

.merchant-info {
  display: flex;
  align-items: center;
  gap: 12px;
  justify-content: center;
}
.center {
  justify-content: center;
  align-items: center;
  text-align: center;
  display: flex;
}

.merchant-logo {
  width: 40px;
  height: 40px;
  border-radius: 8px;
  object-fit: cover;
  background: #e3f0ff;
  border: 2px solid #007BFF;
}

.merchant-name {
  font-size: 18px;
  font-weight: 700;
  color: #007BFF;
}

.order-status {
  display: flex;
  align-items: center;
}

.status-text {
  font-size: 13px;
  font-weight: 600;
  padding: 4px 12px;
  border-radius: 12px;
  background: linear-gradient(90deg, #e3f0ff 0%, #ffffff 100%);
  color: #007BFF;
}

.order-items {
  margin-bottom: 16px;
  background: linear-gradient(90deg, #f8fbff 0%, #e3f0ff 100%);
  border-radius: 10px;
  padding: 12px 0;
}

.order-item {
  display: flex;
  align-items: center;
  gap: 16px;
  margin-bottom: 12px;
  padding: 10px 0;
  border-bottom: 1px solid #e3f0ff;
}
.order-item:last-child {
  border-bottom: none;
}

.item-image {
  width: 70px;
  height: 70px;
  border-radius: 10px;
  object-fit: cover;
  background: #e3f0ff;
}

.item-info {
  flex: 1;
}
.item-name {
  font-size: 16px;
  font-weight: 600;
  color: #007BFF;
  margin-bottom: 6px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.item-meta {
  display: flex;
  align-items: center;
  gap: 16px;
}
.item-price {
  font-size: 15px;
  color: #e74c3c;
  font-weight: 700;
}
.item-quantity {
  font-size: 13px;
  color: #666;
}
.more-items {
  text-align: center;
  color: #007BFF;
  font-size: 13px;
  padding: 8px;
  background: #e3f0ff;
  border-radius: 8px;
  margin-top: 8px;
}

.order-footer {
  display: flex;
  justify-content: center;
  align-items: center;
  margin-top: 16px;
  padding-top: 16px;
  border-top: 1px solid #e3f0ff;
  background: linear-gradient(90deg, #e3f0ff 0%, #f8fbff 100%);
  border-radius: 0 0 32px 32px;
}
.order-info {
  flex: 1;
}
.order-time {
  font-size: 13px;
  color: #007BFF;
  margin-bottom: 6px;
}
.order-amount {
  font-size: 18px;
  color: #007BFF;
  font-weight: 700;
  margin: 0 auto;
}
.order-actions {
  display: flex;
  gap: 10px;
}
.action-btn {
  padding: 7px 18px;
  border: 1px solid #007BFF;
  border-radius: 18px;
  font-size: 13px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}
.action-btn.primary {
  background: linear-gradient(90deg, #007BFF 0%, #00D4FF 100%);
  border-color: #007BFF;
  color: white;
}
.action-btn.primary:hover {
  background: #0056b3;
  border-color: #0056b3;
}
.action-btn.secondary {
  background: white;
  border-color: #e1e5e9;
  color: #007BFF;
}
.action-btn.secondary:hover {
  border-color: #007BFF;
  color: #0056b3;
}

/* 顶部导航 */
.header {
  position: relative;
  height: 60px;
  background: linear-gradient(135deg, #007BFF, #00D4FF);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
}

.back-btn {
  position: absolute;
  left: 16px;
  width: 36px;
  height: 36px;
  background: rgba(255, 255, 255, 0.2);
  border: none;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  color: white;
  font-size: 18px;
}

.header-title {
  font-size: 18px;
  font-weight: 600;
  margin: 0;
}

/* 状态筛选 */
.status-filter {
  display: flex;
  background: white;
  padding: 12px 16px;
  border-bottom: 1px solid #e9ecef;
  overflow-x: auto;
  gap: 8px;
}

.filter-item {
  display: flex;
  align-items: center;
  padding: 8px 16px;
  border-radius: 20px;
  background: #f8f9fa;
  cursor: pointer;
  transition: all 0.3s ease;
  white-space: nowrap;
  min-width: fit-content;
}

.order-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  transform: translateY(-2px);
}

/* 订单头部 */
.order-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
  padding-bottom: 12px;
  border-bottom: 1px solid #f0f0f0;
}

.merchant-info {
  display: flex;
  align-items: center;
  gap: 8px;
}

.merchant-logo {
  width: 32px;
  height: 32px;
  border-radius: 6px;
  object-fit: cover;
}

.merchant-name {
  font-size: 16px;
  font-weight: 600;
  color: #333;
}

.order-status {
  display: flex;
  align-items: center;
}

.status-text {
  font-size: 12px;
  font-weight: 500;
  padding: 4px 8px;
  border-radius: 12px;
}

.status-text.pending {
  background: #fff3cd;
  color: #856404;
}

.status-text.waiting {
  background: #cce5ff;
  color: #004085;
}

.status-text.processing {
  background: #d4edda;
  color: #155724;
}

.status-text.shipping {
  background: #d1ecf1;
  color: #0c5460;
}

.status-text.completed {
  background: #d4edda;
  color: #155724;
}

.status-text.cancelled {
  background: #f8d7da;
  color: #721c24;
}

/* 订单商品 */
.order-items {
  margin-bottom: 12px;
}

.order-item {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 8px;
}

.order-item:last-child {
  margin-bottom: 0;
}

.item-image {
  width: 60px;
  height: 60px;
  border-radius: 8px;
  object-fit: cover;
}

.item-info {
  flex: 1;
}

.item-name {
  font-size: 14px;
  font-weight: 500;
  color: #333;
  margin-bottom: 4px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.item-meta {
  display: flex;
  align-items: center;
  gap: 12px;
}

.item-price {
  font-size: 14px;
  color: #e74c3c;
  font-weight: 600;
}

.item-quantity {
  font-size: 12px;
  color: #666;
}

.more-items {
  text-align: center;
  color: #666;
  font-size: 12px;
  padding: 8px;
  background: #f8f9fa;
  border-radius: 6px;
  margin-top: 8px;
}

/* 订单底部 */
.order-footer {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  margin-top: 12px;
  padding-top: 12px;
  border-top: 1px solid #f0f0f0;
}

.order-info {
  flex: 1;
}

.order-time {
  font-size: 12px;
  color: #666;
  margin-bottom: 4px;
}

.order-amount {
  font-size: 14px;
  color: #333;
  font-weight: 600;
}

.order-actions {
  display: flex;
  gap: 8px;
}

.action-btn {
  padding: 6px 16px;
  border: 1px solid;
  border-radius: 16px;
  font-size: 12px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
}

.action-btn.primary {
  background: #007BFF;
  border-color: #007BFF;
  color: white;
}

.action-btn.primary:hover {
  background: #0056b3;
  border-color: #0056b3;
}

.action-btn.secondary {
  background: white;
  border-color: #e1e5e9;
  color: #666;
}

.action-btn.secondary:hover {
  border-color: #007BFF;
  color: #007BFF;
}

/* 空状态 */
.empty-state {
  text-align: center;
  padding: 80px 20px;
}

.empty-icon {
  font-size: 64px;
  margin-bottom: 16px;
  opacity: 0.5;
}

.empty-text {
  font-size: 16px;
  color: #666;
  margin-bottom: 24px;
}

.shop-btn {
  padding: 12px 32px;
  background: linear-gradient(135deg, #007BFF, #00D4FF);
  color: white;
  border: none;
  border-radius: 25px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.shop-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 123, 255, 0.3);
}

/* 加载更多 */
.load-more {
  text-align: center;
  padding: 20px;
}

.load-more-btn {
  padding: 10px 24px;
  background: white;
  border: 1px solid #e1e5e9;
  border-radius: 20px;
  color: #666;
  cursor: pointer;
  font-size: 14px;
  transition: all 0.3s ease;
}

.load-more-btn:hover {
  border-color: #007BFF;
  color: #007BFF;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .order-footer {
    flex-direction: column;
    align-items: stretch;
    gap: 12px;
  }
  
  .order-actions {
    justify-content: flex-end;
  }
  
  .status-filter {
    padding: 8px 12px;
  }
  
  .filter-item {
    padding: 6px 12px;
    font-size: 12px;
  }
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}
</style>
