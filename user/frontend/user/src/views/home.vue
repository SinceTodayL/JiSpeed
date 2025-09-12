<template>
    <h1> JiSpeed 主界面 </h1>
    <n-button @click="goToMerchant"> 跳转商家界面 </n-button>
</template>

<script setup>
// import { useRouter } from 'vue-router'
// const router = useRouter() 

function goToMerchant(){
    window.location.href = 'http://localhost:9527';
}
</script>