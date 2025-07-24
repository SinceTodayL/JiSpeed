<template>
  <div class="home">
    <div class="hero-section">
      <h1>用户模块前端应用</h1>
      <p>欢迎使用用户管理系统</p>
      
      <div class="navigation-cards">
        <div class="nav-card">
          <h3>🔐 用户登录</h3>
          <p>登录到您的账户</p>
          <router-link to="/login" class="btn-primary">
            立即登录
          </router-link>
        </div>

        <div class="nav-card">
          <h3>📝 用户注册</h3>
          <p>创建新的用户账户</p>
          <router-link to="/register" class="btn-primary">
            立即注册
          </router-link>
        </div>
        
        <div class="nav-card">
          <h3>🍽️ 外卖浏览</h3>
          <p>浏览商家和美食菜品</p>
          <router-link to="/browse" class="btn-primary">
            开始浏览
          </router-link>
        </div>
        
        <div class="nav-card">
          <h3>📋 我的订单</h3>
          <p>查看和管理您的订单</p>
          <router-link to="/orders" class="btn-primary">
            查看订单
          </router-link>
        </div>
        
        <div class="nav-card">
          <h3>🎫 我的优惠券</h3>
          <p>查看和使用优惠券</p>
          <router-link to="/coupons" class="btn-primary">
            查看优惠券
          </router-link>
        </div>
        
        <div class="nav-card">
          <h3>📍 地址管理</h3>
          <p>管理收货地址</p>
          <router-link to="/addresses" class="btn-primary">
            管理地址
          </router-link>
        </div>
        
        <div class="nav-card">
          <h3>👤 用户管理</h3>
          <p>用户信息管理和设置</p>
          <button class="btn-secondary" disabled>
            开发中...
          </button>
        </div>
        
        <div class="nav-card">
          <h3>🛒 购物车</h3>
          <p>购物车商品管理</p>
          <button class="btn-secondary" disabled>
            开发中...
          </button>
        </div>
      </div>
    </div>
    
    <div class="features-section">
      <h2>功能特性</h2>
      <div class="features-grid">
        <div class="feature-item">
          <div class="feature-icon">🔐</div>
          <h4>用户认证</h4>
          <p>支持用户注册、登录、密码重置等功能</p>
        </div>
        
        <div class="feature-item">
          <div class="feature-icon">📱</div>
          <h4>响应式设计</h4>
          <p>适配各种设备屏幕，提供优质的用户体验</p>
        </div>
        
        <div class="feature-item">
          <div class="feature-icon">⚡</div>
          <h4>高性能</h4>
          <p>基于Vue3和Vite构建，快速响应用户操作</p>
        </div>
        
        <div class="feature-item">
          <div class="feature-icon">🔧</div>
          <h4>完整功能</h4>
          <p>订单管理、优惠券系统、地址管理等完整功能</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'Home',
  mounted() {
    console.log('首页已加载')
  }
}
</script>

<style scoped>
.home {
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 20px;
}

.hero-section {
  text-align: center;
  padding: 60px 20px;
  color: white;
}

.hero-section h1 {
  font-size: 3rem;
  margin-bottom: 20px;
  text-shadow: 2px 2px 4px rgba(0,0,0,0.3);
}

.hero-section p {
  font-size: 1.2rem;
  margin-bottom: 40px;
  opacity: 0.9;
}

.navigation-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 20px;
  max-width: 1200px;
  margin: 0 auto;
}

.nav-card {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 15px;
  padding: 30px;
  text-align: center;
  transition: all 0.3s ease;
  color: white;
}

.nav-card:hover {
  transform: translateY(-5px);
  background: rgba(255, 255, 255, 0.15);
  box-shadow: 0 10px 30px rgba(0,0,0,0.2);
}

.nav-card h3 {
  font-size: 1.5rem;
  margin-bottom: 15px;
}

.nav-card p {
  margin-bottom: 20px;
  opacity: 0.9;
}

.btn-primary {
  display: inline-block;
  background: linear-gradient(45deg, #ff6b6b, #ee5a24);
  color: white;
  padding: 12px 30px;
  border-radius: 25px;
  text-decoration: none;
  font-weight: bold;
  transition: all 0.3s ease;
  border: none;
  cursor: pointer;
}

.btn-primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 5px 15px rgba(255, 107, 107, 0.4);
}

.btn-secondary {
  display: inline-block;
  background: rgba(255, 255, 255, 0.2);
  color: white;
  padding: 12px 30px;
  border-radius: 25px;
  text-decoration: none;
  font-weight: bold;
  border: 1px solid rgba(255, 255, 255, 0.3);
  cursor: not-allowed;
  opacity: 0.7;
}

.features-section {
  max-width: 1200px;
  margin: 80px auto 0;
  padding: 60px 20px;
  text-align: center;
  color: white;
}

.features-section h2 {
  font-size: 2.5rem;
  margin-bottom: 50px;
  text-shadow: 2px 2px 4px rgba(0,0,0,0.3);
}

.features-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 30px;
}

.feature-item {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 15px;
  padding: 30px;
  transition: all 0.3s ease;
}

.feature-item:hover {
  transform: translateY(-5px);
  background: rgba(255, 255, 255, 0.15);
}

.feature-icon {
  font-size: 3rem;
  margin-bottom: 20px;
}

.feature-item h4 {
  font-size: 1.3rem;
  margin-bottom: 15px;
}

.feature-item p {
  opacity: 0.9;
  line-height: 1.6;
}

@media (max-width: 768px) {
  .hero-section h1 {
    font-size: 2rem;
  }
  
  .navigation-cards {
    grid-template-columns: 1fr;
    gap: 15px;
  }
  
  .nav-card {
    padding: 20px;
  }
  
  .features-grid {
    grid-template-columns: 1fr;
  }
}
</style>
