# 用户端统一登录接入文档

## 📖 概述

用户端已成功接入统一登录系统，支持从统一登录页面跳转并自动处理用户认证信息。

## 🚀 配置信息

### 1. 服务端口配置
- **用户端端口**: 3000
- **统一登录端口**: 9527
- **用户端访问地址**: http://localhost:3000
- **统一登录地址**: http://localhost:9527/login

### 2. 环境变量配置
在 `login/vite.config.ts` 中已配置：
```typescript
VITE_USER_FRONTEND_URL: 'http://localhost:3000'
```

## 🔧 技术实现

### 1. URL参数处理 (`src/utils/urlParams.js`)
- `handleLoginParams()`: 处理登录跳转参数
- `initUserAuth()`: 初始化用户认证状态
- `getCurrentUser()`: 获取当前用户信息
- `clearUserAuth()`: 清除用户认证信息

### 2. 应用启动处理 (`src/main.js`)
```javascript
// 优先处理统一登录跳转的URL参数
const authResult = initUserAuth()
```

### 3. 路由守卫 (`src/router/index.js`)
- 检查用户登录状态
- 验证用户类型（userType=1）
- 拦截非法访问

### 4. API请求增强 (`src/api/user.js`)
- 自动携带用户ID参数
- 自动添加认证头
- 401错误自动跳转统一登录

## 📋 登录流程

### 1. 统一登录跳转
当用户在统一登录页面选择"用户"角色并登录成功后，系统会跳转到：
```
http://localhost:3000/?id=USER123&token=eyJhbGciOiJIUzI1NiIs...
```

### 2. 参数自动处理
用户端会自动：
1. 提取URL中的 `id` 和 `token` 参数
2. 存储到 localStorage 中
3. 清除URL参数保持干净
4. 初始化用户状态

### 3. 持久化存储
存储的信息包括：
- `token`: JWT令牌
- `userId`: 用户ID
- `userType`: 用户类型（固定为1）
- `userInfo`: 用户基本信息JSON

## 🔐 权限控制

### 1. 用户类型验证
只允许 `userType=1` 的用户访问用户端：
- userType=2 → 跳转商家端
- userType=3 → 跳转骑手端（待配置）
- userType=4 → 跳转管理端（待配置）

### 2. 路由保护
需要认证的路由添加 `requiresAuth: true` 元数据：
```javascript
{
  path: '/profile',
  component: Profile,
  meta: {
    requiresAuth: true
  }
}
```

## 🔄 退出登录

### 1. 统一退出
所有退出操作都会：
1. 调用后端登出API（如果可用）
2. 清除本地认证信息
3. 跳转到统一登录页面

### 2. 退出入口
- Profile页面的退出按钮
- Settings页面的退出按钮

## 🧪 测试验证

### 1. 测试页面
访问 `login-test.html` 查看登录状态和测试链接。

### 2. 测试步骤
1. 启动统一登录服务：`cd login && npm run dev`
2. 启动用户端服务：`cd user && npm run dev`
3. 访问 http://localhost:9527/login
4. 选择"用户"角色登录
5. 验证自动跳转到用户端
6. 检查用户信息是否正确显示

### 3. 验证点
- [ ] URL参数正确提取
- [ ] localStorage正确存储
- [ ] URL参数自动清除
- [ ] 用户信息正确显示
- [ ] 需要认证的页面正常访问
- [ ] 退出登录正常跳转

## 📝 注意事项

### 1. 兼容性处理
- 保留了测试模式的自动登录功能
- 优先使用统一登录，降级使用测试登录
- API调用失败时使用模拟数据

### 2. 安全考虑
- URL参数处理后立即清除
- 认证信息存储在localStorage
- 自动跳转处理401错误

### 3. 开发建议
- 所有API调用都会自动携带userId
- 用户信息优先从统一登录获取
- 错误处理统一跳转到统一登录页面

## 🔗 相关文件

### 核心文件
- `src/utils/urlParams.js` - URL参数处理
- `src/main.js` - 应用启动配置
- `src/router/index.js` - 路由守卫
- `src/api/user.js` - API增强
- `vite.config.js` - 开发服务器配置

### 页面文件
- `src/views/Profile.vue` - 个人中心
- `src/views/Settings.vue` - 设置页面

### 配置文件
- `login/vite.config.ts` - 统一登录配置

## 🎯 总结

用户端已完全接入统一登录系统，实现了：
- ✅ 无缝登录跳转
- ✅ 自动参数处理
- ✅ 权限验证
- ✅ 统一退出
- ✅ 错误处理

整个登录流程对用户透明，确保了良好的用户体验和系统安全性。
