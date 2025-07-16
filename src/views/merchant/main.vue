<template>
  <div class="merchant-layout">
    <n-layout has-sider style="min-height: 100vh">
      <!-- 侧边栏 -->
      <n-layout-sider
        bordered
        collapse-mode="width"
        :collapsed-width="64"
        :width="240"
        :collapsed="collapsed"
        show-trigger
        @collapse="collapsed = true"
        @expand="collapsed = false"
      >
        <div class="logo-container">
          <span>{{ collapsed ? '商家' : '商家管理系统' }}</span>
        </div>
        
        <n-menu
          :collapsed="collapsed"
          :collapsed-width="64"
          :collapsed-icon-size="22"
          :options="menuOptions"
          :value="activeKey"
          @update:value="handleMenuSelect"
        />
      </n-layout-sider>

      <n-layout>
        <!-- 顶部导航栏 -->
        <n-layout-header bordered class="header">
          <n-breadcrumb>
            <n-breadcrumb-item 
              v-for="item in breadcrumbItems" 
              :key="item.path"
              @click="router.push(item.path)"
              :style="{ cursor: 'pointer' }"
            >
              {{ item.meta.title }}
            </n-breadcrumb-item>
          </n-breadcrumb>
          
        </n-layout-header>

        <!-- 主要内容区域 -->
        <n-layout-content class="content">
          <router-view />
        </n-layout-content>
      </n-layout>
    </n-layout>
  </div>
</template>

<script setup>
import { ref, computed, h } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { NIcon } from 'naive-ui'

const router = useRouter()
const route = useRoute()

// 侧边栏折叠状态
const collapsed = ref(false)

// 当前激活的菜单项，根据当前路由动态计算
const activeKey = computed(() => route.name)

// 面包屑导航
const breadcrumbItems = computed(() => route.matched.filter(item => item.meta && item.meta.title))

// 创建图标的辅助函数
const renderIcon = (svg) => {
  return () => h(NIcon, null, { default: () => h('div', { innerHTML: svg, style: 'font-size: 18px;' }) })
}

// 简化后的菜单选项
const menuOptions = ref([
  {
    label: '仪表盘',
    key: 'MerchantDashboard',
    icon: renderIcon('<svg viewBox="0 0 24 24"><path fill="currentColor" d="M13 3v6h8V3h-8zm-2 6V3H3v6h8zM3 21h8v-6H3v6zm10-6v6h8v-6h-8z"/></svg>')
  },
  {
    label: '菜单管理',
    key: 'MenuManagement',
    icon: renderIcon('<svg viewBox="0 0 24 24"><path fill="currentColor" d="M3 11h8V3H3v8zm0 10h8v-8H3v8zm10 0h8v-8h-8v8zm0-18v8h8V3h-8z"/></svg>')
  },
  {
    label: '优惠券管理',
    key: 'CouponManagement',
    icon: renderIcon('<svg viewBox="0 0 24 24"><path fill="currentColor" d="M21.41 11.58l-9-9C12.05 2.22 11.55 2 11 2H4c-1.1 0-2 .9-2 2v7c0 .55.22 1.05.59 1.42l9 9c.36.36.86.58 1.41.58c.55 0 1.05-.22 1.41-.59l7-7c.37-.36.59-.86.59-1.41c0-.55-.23-1.05-.59-1.42zM5.5 7C4.67 7 4 6.33 4 5.5S4.67 4 5.5 4S7 4.67 7 5.5S6.33 7 5.5 7z"/></svg>')
  }
])

// 用户头像下拉菜单选项
const userOptions = ref([
  { label: '退出登录', key: 'logout' }
])

// 菜单点击事件
const handleMenuSelect = (key) => {
  router.push({ name: key })
}

// 用户菜单点击事件
const handleUserMenuSelect = (key) => {
  if (key === 'logout') {
    router.push('/merchant/login')
  }
}
</script>

<style scoped>
.merchant-layout {
  width: 100%;
}
.logo-container {
  height: 64px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 18px;
  font-weight: bold;
  color: #18a058;
  white-space: nowrap;
}
.header {
  height: 64px;
  padding: 0 24px;
  display: flex;
  align-items: center;
  justify-content: space-between;
}
.header-right {
  display: flex;
  align-items: center;
}
.user-avatar {
  display: flex;
  align-items: center;
  cursor: pointer;
}
.username {
  margin-left: 8px;
  font-weight: bold;
}
.content {
  padding: 24px;
  background-color: #f0f2f5;
}
</style>
