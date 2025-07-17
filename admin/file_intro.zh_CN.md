# 项目文件结构介绍

本文档详细概述了该项目中的目录和文件。

## 根目录

| 文件/目录 | 描述 |
| --- | --- |
| `.git/` | 用于版本控制的 Git 目录。 |
| `node_modules/` | 存储项目依赖项。 |
| `.gitignore` | 指定 Git 要忽略的文件和文件夹。 |
| `pnpm-lock.yaml` | pnpm 的锁定文件，确保依赖版本的一致性。 |
| `uno.config.ts` | UnoCSS（一个原子化 CSS 框架）的配置文件。 |
| `pnpm-workspace.yaml` | 为 pnpm 定义工作区，用于管理多个包。 |
| `package.json` | 项目元数据和依赖项列表。 |
| `src/` | **主要源代码目录。** 大部分的开发工作都在这里进行。 |
| `vite.config.ts` | Vite（构建工具）的配置文件。 |
| `tsconfig.json` | TypeScript 配置文件。 |
| `public/` | 未经构建处理的静态资源。 |
| `packages/` | 包含项目内部使用的本地包。 |
| `eslint.config.js` | ESLint 配置文件，用于代码检查。 |
| `index.html` | 应用程序的主要 HTML 入口点。 |
| `Soybean.md` / `Soybean.en_US.md` | 项目文档。 |
| `LICENSE` | 项目的许可证文件。 |
| `CHANGELOG.md` / `CHANGELOG.zh_CN.md` | 项目的更新日志。 |

## `packages/` 目录

该目录包含作为 monorepo 一部分的本地包，由 pnpm工作区管理。

| 包 | 描述 |
| --- | --- |
| `alova/` | 一个轻量级的请求策略库。似乎用于数据获取和缓存。 |
| `axios/` | 一个基于 Promise 的 HTTP 客户端，适用于浏览器和 Node.js。这可能是一个定制的 Axios 包装器。 |
| `color/` | 一个用于处理颜色的实用工具包。它可能提供颜色操作和转换的功能。 |
| `hooks/` | 包含自定义的 Vue Composition API 钩子，促进组件间的可复用逻辑。 |
| `materials/` | 似乎包含可复用的 UI 组件或用于构建页面的素材。 |
| `ofetch/` | 一个轻量级的请求库，在某些情况下可能作为 `axios` 或 `alova` 的替代品。 |
| `scripts/` | 包含各种用于自动化任务的脚本，如生成更新日志、发布等。 |
| `uno-preset/` | UnoCSS 的预设，为项目的样式定义了自定义规则和快捷方式。 |
| `utils/` | 可在整个应用程序中使用的实用功能集合。 |

## `src/` 目录

这是应用程序的核心，包含所有前端源代码。

### `src/` 的根目录

| 文件/目录 | 描述 |
| --- | --- |
| `App.vue` | 应用程序的根 Vue 组件。 |
| `main.ts` | 应用程序的入口点。它初始化 Vue 和其他插件。 |
| `assets/` | 包含由 Vite 处理的静态资源，如图像和图标。 |
| `components/` | 包含可复用的 Vue 组件。 |
| `constants/` | 包含在整个应用程序中使用的常量值。 |
| `enum/` | 包含枚举定义。 |
| `hooks/` | 包含特定于应用程序的自定义钩子。 |
| `layouts/` | 包含不同的页面布局（例如，基础布局、空白布局）。 |
| `locales/` | 包含用于国际化 (i18n) 的文件。 |
| `plugins/` | 包含应用程序插件。 |
| `router/` | 包含路由配置。 |
| `service/` | 包含用于 API 请求的服务。 |
| `store/` | 包含状态管理设置 (Pinia)。 |
| `styles/` | 包含全局样式和 CSS 变量。 |
| `theme/` | 包含主题配置和设置。 |
| `typings/` | 包含全局 TypeScript 类型定义。 |
| `utils/` | 包含特定于主应用程序的实用功能。 |
| `views/` | 包含应用程序的页面。每个子目录通常代表一个路由。 |

现在，让我们更深入地了解这些目录。

### `src/assets/`

该目录存储所有作为源代码一部分并将由 Vite 处理的静态资源。

*   `imgs/`: 包含应用程序中使用的图像。
*   `svg-icon/`: 包含用作图标的 SVG 文件。

### `src/components/`

该目录用于全局可复用的 Vue 组件。

*   `common/`: 可在应用程序中任何地方使用的基本通用组件。
    *   `app-provider.vue`: 提供对话框、消息和通知等全局服务。
    *   `dark-mode-container.vue`: 处理暗黑模式样式的容器。
    *   `exception-base.vue`: 用于显示异常页面（例如 404、500）的基础组件。
    *   `full-screen.vue`: 用于切换全屏模式的组件。
    *   `lang-switch.vue`: 用于切换应用程序语言的组件。
    *   `menu-toggler.vue`: 用于切换侧边栏菜单的组件。
    *   `pin-toggler.vue`: 用于固定或取消固定元素的组件。
    *   `reload-button.vue`: 用于刷新当前页面的按钮。
    *   `system-logo.vue`: 系统的徽标组件。
    *   `theme-schema-switch.vue`: 用于更改配色方案（例如，浅色、深色、自动）的开关。
*   `custom/`: 更复杂或专门的组件。
    *   `better-scroll.vue`: BetterScroll 库的包装器，用于平滑滚动。
    *   `button-icon.vue`: 带有集成图标的按钮组件。
    *   `count-to.vue`: 一个动画组件，使数字向上计数到目标值。
    *   `look-forward.vue`: 占位符或“即将推出”组件。
    *   `soybean-avatar.vue`: 自定义头像组件。
    * `svg-icon.vue`: 用于显示 `assets/svg-icon` 目录中 SVG 图标的组件。
    * `wave-bg.vue`: 用于波浪状背景效果的组件。
*   `advanced/`: 非常复杂或具有非常特定用例的组件。
    *   `table-column-setting.vue`: 用于自定义表格列的组件。
    *   `table-header-operation.vue`: 用于表格头部操作的组件。

### `src/constants/`

该目录包含在整个应用程序中使用的常量值。

*   `app.ts`: 应用程序级别的常量，例如本地存储键。
*   `common.ts`: 不属于其他类别的通用常量。
*   `reg.ts`: 用于验证的正则表达式。

### `src/enum/`

该目录包含 TypeScript 枚举。

*   `index.ts`: 导出所有枚举。很可能枚举是在此目录中的其他文件中定义并在此处聚合的。

### `src/hooks/`

此目录包含用于可重用逻辑的自定义Vue Composition API钩子。这与`packages/hooks`目录不同，后者包含更通用的跨项目钩子。

*   `business/`: 与特定业务逻辑相关的钩子。
    *   `auth.ts`: 用于处理身份验证逻辑的钩子。
    *   `captcha.ts`: 用于管理验证码逻辑（例如，发送和验证代码）的钩子。
*   `common/`: 此应用程序的通用钩子。
    *   `echarts.ts`: 用于使用ECharts的钩子。
    *   `form.ts`: 用于简化表单处理的钩子。
    *   `icon.ts`: 用于管理图标的钩子。
    *   `router.ts`: 提供对与路由器相关功能的访问的钩子。
    *   `table.ts`: 用于管理表格状态和逻辑的钩子。

### `src/layouts/`

该目录定义了应用程序中页面的整体结构。

*   `base-layout/`: 应用程序的主要布局，可能包括页眉、侧边栏、页脚和内容区域。
*   `blank-layout/`: 一个最小化的布局，通常用于登录或404等页面。
*   `context/`: 为布局提供上下文，可能用于在布局组件之间共享状态。
*   `modules/`: 包含布局的构建块，例如全局页眉、侧边栏、选项卡等。每个都是一个复杂的组件。

### `src/locales/`

该目录管理国际化（i18n）。

*   `langs/`: 包含不同语言的翻译文件（例如`en-us.ts`，`zh-cn.ts`）。
*   `index.ts`: 初始化和配置i18n实例。
*   `dayjs.ts`: 为`dayjs`库配置区域设置。
*   `naive.ts`: 为`naive-ui`组件库配置区域设置。
*   `locale.ts`: 定义可用的区域设置和默认区域设置。

### `src/plugins/`

该目录用于将第三方库集成为Vue插件并进行配置。

*   `index.ts`: 导出一个安装所有插件的函数。
*   `app.ts`: 用于应用程序级设置的核心插件。
*   `assets.ts`: 用于处理静态资源的插件。
*   `dayjs.ts`: 用于集成`dayjs`库的插件。
*   `iconify.ts`: 用于集成`Iconify`图标库的插件。
*   `loading.ts`: 用于在页面转换或API调用期间显示加载指示器的插件。
*   `nprogress.ts`: 用于在导航期间在页面顶部显示进度条的插件。

### `src/router/`

该目录处理路由的各个方面。

*   `index.ts`: 创建和配置Vue Router实例。
*   `elegant/`: 包含“elegant-router”的逻辑，这似乎是用于生成路由的自定义解决方案。
    *   `imports.ts`: 处理视图组件的动态导入。
    *   `routes.ts`: 定义路由的静态部分。
    *   `transform.ts`: 将路由配置转换为Vue Router可以理解的格式。
*   `routes/`: 定义应用程序的路由。
    *   `index.ts`: 聚合和导出所有路由。
    *   `builtin.ts`: 定义内置路由，例如404页面或登录页面。
*   `guard/`: 实现导航守卫。
    *   `index.ts`: 应用所有导航守卫。
    *   `progress.ts`: 控制NProgress加载条的守卫。
    *   `route.ts`: 处理身份验证和权限的主要守卫。
    *   `title.ts`: 根据当前路由更新页面标题的守卫。

### `src/service/`

该目录负责与后端API的通信。

*   `api/`: 定义API端点。每个文件通常对应于后端的一个模块。
    *   `auth.ts`: 与身份验证相关的API调用。
    *   `route.ts`: 用于从后端获取路由的API调用。
    *   `index.ts`: 导出所有API模块。
*   `request/`: 包含发出HTTP请求的核心逻辑。
    *   `index.ts`: 主要的请求函数，可能是像Axios或Alova这样的库的包装器。
    *   `shared.ts`: 请求的共享配置和实用程序。
    *   `type.ts`: 请求和响应对象的TypeScript类型。

### `src/store/`

该目录包含使用Pinia的应用程序状态管理。

*   `index.ts`: 创建Pinia实例并导出用于使用存储的函数。
*   `modules/`: 每个文件代表一个单独的存储模块。
    *   `app/`: 存储常规应用程序状态。
    *   `auth/`: 存储与身份验证相关的状态，例如令牌和用户信息。
    *   `route/`: 存储动态生成的路由。
    *   `tab/`: 存储选项卡导航的状态。
    *   `theme/`: 存储当前主题和布局设置。
*   `plugins/`: 包含Pinia插件，例如用于将状态持久化到本地存储。

### `src/styles/`

该目录包含全局CSS和Scss文件。

*   `css/`: 包含纯CSS文件。
    *   `global.css`: 适用于整个应用程序的全局样式。
    *   `nprogress.css`: NProgress加载条的样式。
    *   `reset.css`: CSS重置，以确保跨浏览器样式一致。
    *   `transition.css`: Vue过渡的样式。
*   `scss/`: 包含用于更高级样式的Scss文件。
    *   `global.scss`: 全局Scss样式。
    *   `scrollbar.scss`: 自定义滚动条的样式。

### `src/theme/`

该目录用于配置应用程序的主题。

*   `settings.ts`: 定义默认主题设置，例如布局模式、配色方案和其他视觉选项。
*   `vars.ts`: 定义主题变量，可能用于浅色和深色模式。

### `src/typings/`

该目录包含全局TypeScript声明文件（`.d.ts`）。这些文件用于为没有自己类型的模块提供类型信息，或声明全局类型。

*   该目录包括应用程序各个部分的类型定义，例如`api.d.ts`、`router.d.ts`、`storage.d.ts`，以及像`naive-ui.d.ts`这样的库。
*   `elegant-router.d.ts`: 自定义elegant-router的类型定义。
*   `components.d.ts`: 自动生成的包含组件类型定义的文件。

### `src/utils/`

该目录包含特定于主应用程序的实用程序功能。

*   `agent.ts`: 用于检测用户代理（例如浏览器、操作系统）的实用程序。
*   `common.ts`: 通用实用程序功能。
*   `icon.ts`: 与图标相关的实用程序功能。
*   `service.ts`: 用于与服务交互的实用程序功能。
*   `storage.ts`: `localStorage`和`sessionStorage`的包装器，用于方便和类型安全的访问。

### `src/views/`

这是应用程序页面的位置。每个子目录通常代表应用程序的一个功能或部分，并包含页面的`.vue`文件。

*   `_builtin/`: 包含作为应用程序核心功能一部分的内置页面。
    *   `403/`、`404/`、`500/`: 异常页面。
    *   `iframe-page/`: 用于在iframe中嵌入内容的页面。
    *   `login/`: 用户登录页面，包含不同登录方法的模块。
*   `home/`: 仪表板或主页。
*   `order-manage/`: 您最近添加的用于管理订单的页面。

项目结构概述到此结束。希望这份详细的文档对您未来的开发有所帮助！ 