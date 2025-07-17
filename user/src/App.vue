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
        <router-view />
      </n-dialog-provider>
    </n-message-provider>
  </n-config-provider>
</template>

<style>
body {
  margin: 0;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
}
</style>
