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
          <h3>🧪 API 测试</h3>
          <p>测试所有用户相关的API接口</p>
          <router-link to="/api-test" class="btn-primary">
            开始测试
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
          <h3>📍 地址管理</h3>
          <p>收货地址的增删改查</p>
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
          <h4>API测试</h4>
          <p>内置完整的API测试工具，方便开发调试</p>
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
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
}

.hero-section {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 80px 20px;
  text-align: center;
}

.hero-section h1 {
  font-size: 3rem;
  margin-bottom: 1rem;
  font-weight: 700;
}

.hero-section p {
  font-size: 1.2rem;
  margin-bottom: 3rem;
  opacity: 0.9;
}

.navigation-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 2rem;
  max-width: 1000px;
  margin: 0 auto;
}

.nav-card {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 12px;
  padding: 2rem;
  text-align: center;
  border: 1px solid rgba(255, 255, 255, 0.2);
  transition: transform 0.3s ease, box-shadow 0.3s ease;
}

.nav-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
}

.nav-card h3 {
  font-size: 1.5rem;
  margin-bottom: 1rem;
  color: white;
}

.nav-card p {
  margin-bottom: 1.5rem;
  opacity: 0.9;
  font-size: 0.95rem;
}

.btn-primary,
.btn-secondary {
  padding: 12px 24px;
  border-radius: 6px;
  font-weight: 600;
  text-decoration: none;
  display: inline-block;
  transition: all 0.3s ease;
  border: none;
  cursor: pointer;
  font-size: 14px;
}

.btn-primary {
  background: #3498db;
  color: white;
}

.btn-primary:hover {
  background: #2980b9;
  transform: translateY(-2px);
}

.btn-secondary {
  background: rgba(255, 255, 255, 0.2);
  color: white;
  cursor: not-allowed;
  opacity: 0.6;
}

.features-section {
  padding: 80px 20px;
  background: #f8f9fa;
}

.features-section h2 {
  text-align: center;
  font-size: 2.5rem;
  margin-bottom: 3rem;
  color: #2c3e50;
}

.features-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 2rem;
  max-width: 1000px;
  margin: 0 auto;
}

.feature-item {
  background: white;
  padding: 2rem;
  border-radius: 12px;
  text-align: center;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s ease;
}

.feature-item:hover {
  transform: translateY(-3px);
  box-shadow: 0 8px 15px rgba(0, 0, 0, 0.15);
}

.feature-icon {
  font-size: 3rem;
  margin-bottom: 1rem;
}

.feature-item h4 {
  font-size: 1.3rem;
  margin-bottom: 1rem;
  color: #2c3e50;
}

.feature-item p {
  color: #7f8c8d;
  line-height: 1.6;
}

@media (max-width: 768px) {
  .hero-section h1 {
    font-size: 2rem;
  }
  
  .hero-section p {
    font-size: 1rem;
  }
  
  .navigation-cards {
    grid-template-columns: 1fr;
    gap: 1rem;
  }
  
  .features-grid {
    grid-template-columns: 1fr;
    gap: 1rem;
  }
  
  .features-section h2 {
    font-size: 2rem;
  }
}
</style>
