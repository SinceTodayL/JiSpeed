<template>
  <div class="api-test-page">
    <h1>用户API测试页面</h1>
    
    <!-- 用户认证测试 -->
    <div class="test-section">
      <h2>🔐 用户认证API测试</h2>
      
      <!-- 用户登录测试 -->
      <div class="test-card">
        <h3>用户登录</h3>
        <div class="form-group">
          <input v-model="loginForm.account" placeholder="账号" />
          <input v-model="loginForm.password" type="password" placeholder="密码" />
          <button @click="testLogin" :disabled="loading">登录测试</button>
        </div>
        <div class="result" v-if="results.login">
          <strong>结果:</strong>
          <pre>{{ JSON.stringify(results.login, null, 2) }}</pre>
        </div>
      </div>

      <!-- 用户注册测试 -->
      <div class="test-card">
        <h3>用户注册</h3>
        <div class="form-group">
          <input v-model="registerForm.account" placeholder="账号" />
          <input v-model="registerForm.nickName" placeholder="昵称" />
          <select v-model="registerForm.gender">
            <option value="">选择性别</option>
            <option value="0">女</option>
            <option value="1">男</option>
          </select>
          <input v-model="registerForm.birthday" type="date" placeholder="生日" />
          <input v-model="registerForm.password" type="password" placeholder="密码" />
          <button @click="testRegister" :disabled="loading">注册测试</button>
        </div>
        <div class="result" v-if="results.register">
          <strong>结果:</strong>
          <pre>{{ JSON.stringify(results.register, null, 2) }}</pre>
        </div>
      </div>
    </div>

    <!-- 用户信息测试 -->
    <div class="test-section">
      <h2>👤 用户信息API测试</h2>
      
      <!-- 获取用户信息 -->
      <div class="test-card">
        <h3>获取用户信息</h3>
        <div class="form-group">
          <input v-model="userId" placeholder="用户ID" />
          <button @click="testGetUser" :disabled="loading">获取用户信息</button>
        </div>
        <div class="result" v-if="results.getUser">
          <strong>结果:</strong>
          <pre>{{ JSON.stringify(results.getUser, null, 2) }}</pre>
        </div>
      </div>

      <!-- 更新用户信息 -->
      <div class="test-card">
        <h3>更新用户信息</h3>
        <div class="form-group">
          <input v-model="updateUserForm.userId" placeholder="用户ID" />
          <textarea v-model="updateUserForm.data" placeholder="更新数据(JSON格式)"></textarea>
          <button @click="testUpdateUser" :disabled="loading">更新用户信息</button>
        </div>
        <div class="result" v-if="results.updateUser">
          <strong>结果:</strong>
          <pre>{{ JSON.stringify(results.updateUser, null, 2) }}</pre>
        </div>
      </div>
    </div>

    <!-- 地址管理测试 -->
    <div class="test-section">
      <h2>📍 地址管理API测试</h2>
      
      <!-- 获取用户地址 -->
      <div class="test-card">
        <h3>获取用户地址列表</h3>
        <div class="form-group">
          <input v-model="addressUserId" placeholder="用户ID" />
          <button @click="testGetAddresses" :disabled="loading">获取地址列表</button>
        </div>
        <div class="result" v-if="results.getAddresses">
          <strong>结果:</strong>
          <pre>{{ JSON.stringify(results.getAddresses, null, 2) }}</pre>
        </div>
      </div>

      <!-- 添加地址 -->
      <div class="test-card">
        <h3>添加收货地址</h3>
        <div class="form-group">
          <input v-model="addAddressForm.userId" placeholder="用户ID" />
          <input v-model="addAddressForm.receiverName" placeholder="收货人姓名" />
          <input v-model="addAddressForm.receiverPhone" placeholder="收货人电话" />
          <input v-model="addAddressForm.detailedAddress" placeholder="详细地址" />
          <select v-model="addAddressForm.isDefault">
            <option value="">是否默认地址</option>
            <option value="0">否</option>
            <option value="1">是</option>
          </select>
          <button @click="testAddAddress" :disabled="loading">添加地址</button>
        </div>
        <div class="result" v-if="results.addAddress">
          <strong>结果:</strong>
          <pre>{{ JSON.stringify(results.addAddress, null, 2) }}</pre>
        </div>
      </div>
    </div>

    <!-- 购物车测试 -->
    <div class="test-section">
      <h2>🛒 购物车API测试</h2>
      
      <!-- 获取购物车 -->
      <div class="test-card">
        <h3>获取购物车内容</h3>
        <div class="form-group">
          <input v-model="cartUserId" placeholder="用户ID" />
          <button @click="testGetCart" :disabled="loading">获取购物车</button>
        </div>
        <div class="result" v-if="results.getCart">
          <strong>结果:</strong>
          <pre>{{ JSON.stringify(results.getCart, null, 2) }}</pre>
        </div>
      </div>

      <!-- 添加到购物车 -->
      <div class="test-card">
        <h3>添加商品到购物车</h3>
        <div class="form-group">
          <input v-model="addCartForm.userId" placeholder="用户ID" />
          <input v-model="addCartForm.dishId" placeholder="商品ID" />
          <button @click="testAddToCart" :disabled="loading">添加到购物车</button>
        </div>
        <div class="result" v-if="results.addToCart">
          <strong>结果:</strong>
          <pre>{{ JSON.stringify(results.addToCart, null, 2) }}</pre>
        </div>
      </div>
    </div>

    <!-- 收藏功能测试 -->
    <div class="test-section">
      <h2>❤️ 收藏功能API测试</h2>
      
      <!-- 获取收藏列表 -->
      <div class="test-card">
        <h3>获取收藏列表</h3>
        <div class="form-group">
          <input v-model="favoriteUserId" placeholder="用户ID" />
          <button @click="testGetFavorites" :disabled="loading">获取收藏列表</button>
        </div>
        <div class="result" v-if="results.getFavorites">
          <strong>结果:</strong>
          <pre>{{ JSON.stringify(results.getFavorites, null, 2) }}</pre>
        </div>
      </div>
    </div>

    <!-- 评论功能测试 -->
    <div class="test-section">
      <h2>💬 评论功能API测试</h2>
      
      <!-- 获取用户评论 -->
      <div class="test-card">
        <h3>获取用户评论</h3>
        <div class="form-group">
          <input v-model="reviewUserId" placeholder="用户ID" />
          <button @click="testGetReviews" :disabled="loading">获取评论列表</button>
        </div>
        <div class="result" v-if="results.getReviews">
          <strong>结果:</strong>
          <pre>{{ JSON.stringify(results.getReviews, null, 2) }}</pre>
        </div>
      </div>

      <!-- 提交评论 -->
      <div class="test-card">
        <h3>提交评论</h3>
        <div class="form-group">
          <input v-model="submitReviewForm.userId" placeholder="用户ID" />
          <input v-model="submitReviewForm.orderId" placeholder="订单ID" />
          <input v-model="submitReviewForm.dishId" placeholder="商品ID" />
          <select v-model="submitReviewForm.rating">
            <option value="">选择评分</option>
            <option value="1">1星</option>
            <option value="2">2星</option>
            <option value="3">3星</option>
            <option value="4">4星</option>
            <option value="5">5星</option>
          </select>
          <textarea v-model="submitReviewForm.content" placeholder="评论内容"></textarea>
          <select v-model="submitReviewForm.isAnonymous">
            <option value="">是否匿名</option>
            <option value="0">不匿名</option>
            <option value="1">匿名</option>
          </select>
          <button @click="testSubmitReview" :disabled="loading">提交评论</button>
        </div>
        <div class="result" v-if="results.submitReview">
          <strong>结果:</strong>
          <pre>{{ JSON.stringify(results.submitReview, null, 2) }}</pre>
        </div>
      </div>
    </div>

    <!-- 投诉功能测试 -->
    <div class="test-section">
      <h2>📋 投诉功能API测试</h2>
      
      <!-- 获取投诉列表 -->
      <div class="test-card">
        <h3>获取投诉列表</h3>
        <div class="form-group">
          <input v-model="complaintUserId" placeholder="用户ID" />
          <button @click="testGetComplaints" :disabled="loading">获取投诉列表</button>
        </div>
        <div class="result" v-if="results.getComplaints">
          <strong>结果:</strong>
          <pre>{{ JSON.stringify(results.getComplaints, null, 2) }}</pre>
        </div>
      </div>

      <!-- 提交投诉 -->
      <div class="test-card">
        <h3>提交投诉</h3>
        <div class="form-group">
          <input v-model="submitComplaintForm.userId" placeholder="用户ID" />
          <input v-model="submitComplaintForm.orderId" placeholder="订单ID" />
          <textarea v-model="submitComplaintForm.description" placeholder="投诉描述"></textarea>
          <button @click="testSubmitComplaint" :disabled="loading">提交投诉</button>
        </div>
        <div class="result" v-if="results.submitComplaint">
          <strong>结果:</strong>
          <pre>{{ JSON.stringify(results.submitComplaint, null, 2) }}</pre>
        </div>
      </div>
    </div>

    <!-- 全局加载状态 -->
    <div v-if="loading" class="loading-overlay">
      <div class="loading-spinner">测试中...</div>
    </div>

    <!-- 错误信息显示 -->
    <div v-if="error" class="error-message">
      <h3>错误信息:</h3>
      <pre>{{ error }}</pre>
      <button @click="clearError">清除错误</button>
    </div>
  </div>
</template>

<script>
import { 
  authAPI, 
  userAPI, 
  addressAPI, 
  cartAPI, 
  favoriteAPI, 
  reviewAPI, 
  complaintAPI 
} from '../api/user.js'

export default {
  name: 'ApiTest',
  data() {
    return {
      loading: false,
      error: null,
      results: {},
      
      // 登录表单
      loginForm: {
        account: '123456',
        password: '123456'
      },
      
      // 注册表单
      registerForm: {
        account: 'test' + Date.now(),
        nickName: '测试用户',
        gender: '1',
        birthday: '1990-01-01',
        password: '123456'
      },
      
      // 用户ID
      userId: '6',
      
      // 更新用户信息表单
      updateUserForm: {
        userId: '1',
        data: '{"nickname":"乐乐小超人","avatarUrl":"https://example.com/avatars/lele_new.jpg","gender":"0","birthday":"2005-03-22Z","defaultAddrId":"222"}'
      },
      
      // 地址相关
      addressUserId: '6',
      addAddressForm: {
        userId: '6',
        receiverName: '张三',
        receiverPhone: '13800138000',
        detailedAddress: '北京市朝阳区测试街道123号',
        isDefault: '0'
      },
      
      // 购物车相关
      cartUserId: '6',
      addCartForm: {
        userId: '6',
        dishId: 'dish123'
      },
      
      // 收藏相关
      favoriteUserId: '6',
      
      // 评论相关
      reviewUserId: '6',
      submitReviewForm: {
        userId: '6',
        orderId: 'order123',
        dishId: 'dish123',
        rating: '5',
        content: '这道菜非常好吃！',
        isAnonymous: '0'
      },
      
      // 投诉相关
      complaintUserId: '6',
      submitComplaintForm: {
        userId: '6',
        orderId: 'order123',
        description: '食物质量有问题'
      }
    }
  },
  
  methods: {
    async executeTest(testName, apiCall) {
      this.loading = true
      this.error = null
      
      try {
        const result = await apiCall()
        this.results = { ...this.results, [testName]: result }
        console.log(`${testName} 测试成功:`, result)
      } catch (error) {
        this.error = `${testName} 测试失败: ${error.message}`
        console.error(`${testName} 测试失败:`, error)
      } finally {
        this.loading = false
      }
    },
    
    // 用户认证测试
    async testLogin() {
      await this.executeTest('login', () => 
        authAPI.login(this.loginForm.account, this.loginForm.password)
      )
    },
    
    async testRegister() {
      await this.executeTest('register', () => 
        authAPI.register(this.registerForm)
      )
    },
    
    // 用户信息测试
    async testGetUser() {
      await this.executeTest('getUser', () => 
        userAPI.getUserById(this.userId)
      )
    },
    
    async testUpdateUser() {
      await this.executeTest('updateUser', () => 
        userAPI.updateUser(this.updateUserForm.userId, this.updateUserForm.data)
      )
    },
    
    // 地址管理测试
    async testGetAddresses() {
      await this.executeTest('getAddresses', () => 
        addressAPI.getUserAddresses(this.addressUserId)
      )
    },
    
    async testAddAddress() {
      await this.executeTest('addAddress', () => 
        addressAPI.addAddress(this.addAddressForm.userId, {
          receiverName: this.addAddressForm.receiverName,
          receiverPhone: this.addAddressForm.receiverPhone,
          detailedAddress: this.addAddressForm.detailedAddress,
          isDefault: parseInt(this.addAddressForm.isDefault)
        })
      )
    },
    
    // 购物车测试
    async testGetCart() {
      await this.executeTest('getCart', () => 
        cartAPI.getUserCart(this.cartUserId)
      )
    },
    
    async testAddToCart() {
      await this.executeTest('addToCart', () => 
        cartAPI.addToCart(this.addCartForm.userId, this.addCartForm.dishId)
      )
    },
    
    // 收藏功能测试
    async testGetFavorites() {
      await this.executeTest('getFavorites', () => 
        favoriteAPI.getUserFavorites(this.favoriteUserId)
      )
    },
    
    // 评论功能测试
    async testGetReviews() {
      await this.executeTest('getReviews', () => 
        reviewAPI.getUserReviews(this.reviewUserId)
      )
    },
    
    async testSubmitReview() {
      await this.executeTest('submitReview', () => 
        reviewAPI.submitReview(this.submitReviewForm.userId, {
          orderId: this.submitReviewForm.orderId,
          dishId: this.submitReviewForm.dishId,
          rating: parseInt(this.submitReviewForm.rating),
          content: this.submitReviewForm.content,
          isAnonymous: parseInt(this.submitReviewForm.isAnonymous)
        })
      )
    },
    
    // 投诉功能测试
    async testGetComplaints() {
      await this.executeTest('getComplaints', () => 
        complaintAPI.getUserComplaints(this.complaintUserId)
      )
    },
    
    async testSubmitComplaint() {
      await this.executeTest('submitComplaint', () => 
        complaintAPI.submitComplaint(this.submitComplaintForm.userId, {
          orderId: this.submitComplaintForm.orderId,
          description: this.submitComplaintForm.description
        })
      )
    },
    
    clearError() {
      this.error = null
    }
  }
}
</script>

<style scoped>
.api-test-page {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
}

h1 {
  color: #2c3e50;
  text-align: center;
  margin-bottom: 30px;
}

h2 {
  color: #34495e;
  border-bottom: 2px solid #3498db;
  padding-bottom: 10px;
  margin-top: 40px;
}

.test-section {
  margin-bottom: 40px;
}

.test-card {
  background: #fff;
  border: 1px solid #e1e8ed;
  border-radius: 8px;
  padding: 20px;
  margin-bottom: 20px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.test-card h3 {
  color: #2c3e50;
  margin-top: 0;
  margin-bottom: 15px;
}

.form-group {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  margin-bottom: 15px;
}

.form-group input,
.form-group select,
.form-group textarea {
  padding: 8px 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
  min-width: 150px;
}

.form-group textarea {
  min-width: 300px;
  min-height: 60px;
  resize: vertical;
}

.form-group button {
  padding: 8px 16px;
  background: #3498db;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  transition: background 0.3s;
}

.form-group button:hover:not(:disabled) {
  background: #2980b9;
}

.form-group button:disabled {
  background: #bdc3c7;
  cursor: not-allowed;
}

.result {
  background: #f8f9fa;
  border: 1px solid #e9ecef;
  border-radius: 4px;
  padding: 15px;
  margin-top: 15px;
}

.result strong {
  color: #27ae60;
}

.result pre {
  background: #2c3e50;
  color: #ecf0f1;
  padding: 10px;
  border-radius: 4px;
  overflow-x: auto;
  font-size: 12px;
  margin-top: 10px;
}

.loading-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0,0,0,0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.loading-spinner {
  background: white;
  padding: 20px 40px;
  border-radius: 8px;
  font-size: 16px;
  font-weight: bold;
  color: #2c3e50;
}

.error-message {
  background: #fff5f5;
  border: 1px solid #fed7d7;
  border-radius: 8px;
  padding: 20px;
  margin-top: 20px;
}

.error-message h3 {
  color: #e53e3e;
  margin-top: 0;
}

.error-message pre {
  background: #fed7d7;
  color: #c53030;
  padding: 10px;
  border-radius: 4px;
  overflow-x: auto;
  font-size: 12px;
}

.error-message button {
  background: #e53e3e;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 4px;
  cursor: pointer;
  margin-top: 10px;
}

.error-message button:hover {
  background: #c53030;
}

@media (max-width: 768px) {
  .form-group {
    flex-direction: column;
  }
  
  .form-group input,
  .form-group select,
  .form-group textarea {
    min-width: unset;
    width: 100%;
  }
}
</style>
