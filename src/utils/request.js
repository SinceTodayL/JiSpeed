import axios from 'axios'

const request = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  timeout: 10000
})

// 请求拦截器：自动带上 token
request.interceptors.request.use(config => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
}, error => Promise.reject(error))

// 响应拦截器：统一处理 code / message / 异常
request.interceptors.response.use(
  response => {
    const { code, message, data } = response.data

    if (code === 0) {
      return data //成功，直接返回 data 部分
    } else {
      //失败
      window.$message?.error?.(message)
      alert(message)  // 临时测试，后续删除
      return Promise.reject(response.data)
    }
  },
  error => {
    //网络错误等非业务问题
    window.$message?.error?.('网络异常')
    return Promise.reject(error)
  }
)

export default request
