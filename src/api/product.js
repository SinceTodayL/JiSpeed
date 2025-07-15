// 测试文件，开发时删除
// src/api/product.js
import request from '../utils/request'

// 获取产品列表
export function getProductList(params) {
  return request.get('/api/products', { params })
}
