<template>
    <div class="test-api">
      <h2>测试接口 /api/products</h2>
  
      <button @click="fetchProducts">点击加载产品列表</button>
  
      <div class="loading" v-if="loading">加载中...</div>
  
      <div v-if="products.length > 0" class="result">
        <h3>返回数据：</h3>
        <ul>
          <li v-for="item in products" :key="item.id">
            ID: {{ item.id }}，名称: {{ item.name }}，价格: {{ item.price }}
          </li>
        </ul>
      </div>
  
      <div v-if="error" class="error">
        <h3>出错：</h3>
        <pre>{{ error }}</pre>
      </div>
    </div>
  </template>
  
  <script>
  import { getProductList } from '../api/product'
  
  export default {
    name: 'TestApi',
    data() {
      return {
        products: [],
        loading: false,
        error: null
      }
    },
    methods: {
      async fetchProducts() {
        this.loading = true
        this.error = null
        try {
          const data = await getProductList()
          this.products = data
        } catch (err) {
          this.error = err.message || '未知错误'
        } finally {
          this.loading = false
        }
      }
    },
    mounted() {
      // 页面加载后自动发起请求（你也可以注释掉这行，手动点击按钮）
      this.fetchProducts()
    }
  }
  </script>
  
  <style scoped>
  .test-api {
    padding: 20px;
  }
  ul {
    list-style: none;
    padding: 0;
  }
  li {
    margin: 6px 0;
  }
  .error {
    color: red;
  }
  </style>
  