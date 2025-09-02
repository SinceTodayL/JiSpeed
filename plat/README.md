# JiSpeed 平台管理系统 (Frontend)

基于 Vue3 + TypeScript + NaiveUI + UnoCSS 的外卖配送平台管理系统前端

## 功能特性

- 🏪 **商家管理** - 商家信息管理、状态控制、申请审核
- 🚴 **骑手管理** - 骑手信息管理、绩效统计、排名查看
- 👥 **用户管理** - 用户信息查看、数据统计
- 📢 **公告管理** - 平台公告发布、管理
- 🎫 **优惠券管理** - 优惠券发放、管理
- 💰 **对账管理** - 财务对账、异常处理
- 📞 **投诉处理** - 用户投诉处理、退款审核

## 技术栈

- **框架**: Vue 3.5.17
- **构建工具**: Vite 7.0.4
- **语言**: TypeScript 5.8.3
- **UI库**: Naive UI 2.42.0
- **CSS框架**: UnoCSS 66.3.3
- **状态管理**: Pinia 3.0.3
- **路由**: Vue Router 4.5.1
- **国际化**: Vue I18n 11.1.9
- **图表**: ECharts 5.6.0

## 环境要求

- Node.js >= 20.19.0
- pnpm >= 10.5.0

## 快速开始

### 安装依赖

```bash
pnpm install
```

### 启动开发服务器

```bash
# 测试模式
pnpm dev

# 生产模式
pnpm dev:prod
```

访问地址: http://localhost:9528

### 构建

```bash
# 生产环境构建
pnpm build

# 测试环境构建
pnpm build:test
```

### 其他命令

```bash
# 代码检查和修复
pnpm lint

# 类型检查
pnpm typecheck

# 预览构建产物
pnpm preview
```

## 项目结构

```
plat/
├── packages/              # 内部工具包
│   ├── axios/            # HTTP请求封装
│   ├── color/            # 颜色工具
│   ├── hooks/            # Vue组合式API工具
│   ├── materials/        # UI组件材料
│   ├── scripts/          # 构建脚本
│   ├── utils/            # 通用工具
│   └── uno-preset/       # UnoCSS预设
├── src/
│   ├── api/              # API接口定义
│   ├── components/       # 公共组件
│   ├── layouts/          # 布局组件
│   ├── views/            # 页面组件
│   ├── router/           # 路由配置
│   ├── store/            # 状态管理
│   ├── styles/           # 样式文件
│   ├── utils/            # 工具函数
│   └── typings/          # 类型定义
├── public/               # 静态资源
└── build/                # 构建配置
```

## API 配置

项目通过 Vite 代理连接后端 API：

- **后端服务地址**: https://localhost:5091
- **代理配置**: `/api/*` → `https://localhost:5091/api/*`

确保后端服务正常运行在 5091 端口。

## 主要页面

- `/home` - 首页仪表板
- `/merchant-manage` - 商家管理
- `/rider-manage` - 骑手管理
- `/user-manage` - 用户管理
- `/complaint-handle` - 投诉处理

## 开发规范

- 使用 TypeScript 进行类型约束
- 遵循 ESLint 代码规范
- 使用 Prettier 代码格式化
- 提交前自动进行类型检查和代码检查

## 贡献指南

1. Fork 项目
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 开启 Pull Request

## 许可证

MIT License
