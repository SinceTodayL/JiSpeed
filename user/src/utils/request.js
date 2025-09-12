/*
   request.js ― 全局 Axios 封装
 */

import axios from 'axios'

// 从环境变量获取基础URL，如果没有设置则使用空字符串（开发环境兼容）
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || ''

const request = axios.create({
  baseURL: API_BASE_URL,  // 使用环境变量，确保生产环境正常工作
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json'
  }
})


/* ============ 请求拦截器 ============ */
request.interceptors.request.use(
  config => {
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  error => Promise.reject(error)
)

/* ============ 响应拦截器 ============ */
/**
 * 统一响应结构            :contentReference[oaicite:0]{index=0}
 * {
 *   code: 0,                  // 业务状态码：0=成功，!=0=失败
 *   message: '操作成功',       // 人类可读信息
 *   data: {...},              // 业务数据
 *   timestamp: 1678886400000  // 服务器 UTC 时间戳 (ms)
 * }
 */
request.interceptors.response.use(
  response => {
    const { code, message, data, timestamp } = response.data

    /* --- 方便调试：打印后端时间戳 --- */
    if (import.meta.env.DEV && timestamp) {
      // eslint-disable-next-line no-console
      console.log('[API timestamp]', new Date(timestamp).toLocaleString())
    }

    /* ---------- 正常成功 ---------- */
    if (code === 0) return response.data  // 返回完整的响应对象

    /* ---------- 认证 / 授权相关 ---------- */
    if (code === 2001) {                     // Token 失效
      window.$message?.warning?.('登录状态可能已过期，部分功能可能受限')
      return Promise.reject(response.data)
    }
    if (code === 2002) {                     // 权限不足            :contentReference[oaicite:2]{index=2}
      window.$message?.error?.('当前账号权限不足')
      return Promise.reject(response.data)
    }

    /* ---------- 客户端请求 / 业务规则错误 ---------- */
    if (code >= 1000 && code < 4000) {       // 1xxx & 3xxx          :contentReference[oaicite:3]{index=3}
      window.$message?.error?.(message || '请求或业务处理失败')
      return Promise.reject(response.data)
    }

    /* ---------- 系统 / 服务内部错误 ---------- */
    if (code >= 5000) {                      // 5xxx                 :contentReference[oaicite:4]{index=4}
      window.$message?.error?.('系统繁忙，请稍后再试')
      return Promise.reject(response.data)
    }

    /* ---------- 兜底未知错误 ---------- */
    window.$message?.error?.(message || '未知错误')
    return Promise.reject(response.data)
  },
  error => {
    window.$message?.error?.('网络异常')
    return Promise.reject(error)
  }
)

export default request
