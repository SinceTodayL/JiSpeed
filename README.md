## JiSpeed 济时达外卖项目前端开发

### **前端开发规范统一说明**



#### 文件夹结构规范

```bash
front-end/
├── public/                     # 静态资源目录，打包时复制
│
├── src/                        # 项目源代码目录（核心开发）
│   ├── api/                    # 所有接口封装（对接后端/Apifox）
│   ├── assets/                 # 存放图片、字体、图标等静态资源
│   ├── components/             # 公共组件目录（如表单、卡片、按钮等）
│   ├── router/                 # Vue Router 配置
│   ├── utils/                  # 工具函数与 axios 请求封装
│   ├── views/                  # 页面组件（与路由一一对应）
│   ├── App.vue                 # 根组件（全局结构容器）
│   └── main.js                 # 项目入口文件，挂载 App 并注册路由等插件
│ 
├── .env.development            # 开发环境变量配置（如 mock 接口地址）
├── .env.production             # 生产环境变量配置（部署后用）
├── .gitignore                  # Git 忽略配置
├── index.html                  # 入口 HTML 模板，Vite 注入内容的容器
├── package.json                # 项目依赖管理 + 脚本定义
├── package-lock.json           # 依赖锁定文件（自动生成）
├── README.md                   # 项目说明文档（你正在编写）
└── vite.config.js              # Vite 配置文件（可定义路径别名、代理等）
```

 

在运行前先安装 `axios` `router` 等依赖（要在当前目录下运行）

```bash
npm install vue axios vue-router
```



* 使用 `utils/request.js` 统一向后端发送数据请求，即，每一次向后端要数据的时候都要经过`request.js`
* 使用 `router/index.js` 实现界面跳传，如 `http://localhost:5173/test` 中 `/test` 是自定义的跳转逻辑

