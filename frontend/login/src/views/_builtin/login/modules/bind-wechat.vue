<script setup lang="ts">
defineOptions({
  name: 'BindWechat'
});

interface Props {
  selectedRole?: 'user' | 'rider' | 'merchant' | 'admin';
}

// 接收但不使用 selectedRole prop 以保持兼容性
defineProps<Props>();
</script>

<template>
  <div></div>
</template>

<style scoped></style>
