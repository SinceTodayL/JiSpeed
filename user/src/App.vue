<script setup>
import { NConfigProvider, NMessageProvider, NDialogProvider } from 'naive-ui'

// 定义全局主题覆盖
/*
const themeOverrides = {
  common: {
    primaryColor: '#667eea',
    primaryColorHover: '#5a6fd8',
    primaryColorPressed: '#4d5dbb',
    primaryColorSuppl: '#667eea'
  }
}
*/
</script>

<template>
  <n-config-provider :theme-overrides="themeOverrides">
    <n-message-provider>
      <n-dialog-provider>
        <div class="app-container">
          <!-- 导航栏 -->
          <nav class="navbar">
            <div class="nav-brand">
              <router-link to="/" class="brand-link">
                🚀 JiSpeed 用户系统
              </router-link>
            </div>
            <div class="nav-links">
              <router-link to="/" class="nav-link" active-class="active">
                首页
              </router-link>
              <router-link to="/api-test" class="nav-link" active-class="active">
                API测试
              </router-link>
            </div>
          </nav>
          
          <!-- 主要内容区域 -->
          <main class="main-content">
            <router-view />
          </main>
        </div>
      </n-dialog-provider>
    </n-message-provider>
  </n-config-provider>
</template>

<style>
body {
  margin: 0;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
  background-color: #f8f9fa;
}

.app-container {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.navbar {
  background: white;
  border-bottom: 1px solid #e1e8ed;
  padding: 0 20px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  height: 60px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  position: sticky;
  top: 0;
  z-index: 100;
}

.nav-brand .brand-link {
  font-size: 1.5rem;
  font-weight: 700;
  color: #2c3e50;
  text-decoration: none;
  transition: color 0.3s ease;
}

.nav-brand .brand-link:hover {
  color: #3498db;
}

.nav-links {
  display: flex;
  gap: 20px;
}

.nav-link {
  color: #6c757d;
  text-decoration: none;
  padding: 8px 16px;
  border-radius: 6px;
  transition: all 0.3s ease;
  font-weight: 500;
}

.nav-link:hover {
  color: #3498db;
  background-color: #f8f9fa;
}

.nav-link.active {
  color: #3498db;
  background-color: #e3f2fd;
}

.main-content {
  flex: 1;
}

@media (max-width: 768px) {
  .navbar {
    padding: 0 15px;
    height: 50px;
  }
  
  .nav-brand .brand-link {
    font-size: 1.2rem;
  }
  
  .nav-links {
    gap: 10px;
  }
  
  .nav-link {
    padding: 6px 12px;
    font-size: 0.9rem;
  }
}
</style>
