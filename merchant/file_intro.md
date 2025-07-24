# Project File Structure Introduction

This document provides a detailed overview of the directories and files within this project.

## Root Directory

| File/Directory | Description |
| --- | --- |
| `.git/` | Git directory for version control. |
| `node_modules/` | Stores project dependencies. |
| `.gitignore` | Specifies files and folders to be ignored by Git. |
| `pnpm-lock.yaml` | Lockfile for pnpm, ensuring consistent dependency versions. |
| `uno.config.ts` | Configuration file for UnoCSS, an atomic CSS framework. |
| `pnpm-workspace.yaml` | Defines the workspace for pnpm, managing multiple packages. |
| `package.json` | Project metadata and dependency list. |
| `src/` | **Main source code directory.** This is where most of the development happens. |
| `vite.config.ts` | Configuration file for Vite, the build tool. |
| `tsconfig.json` | TypeScript configuration file. |
| `public/` | Static assets that are not processed by the build. |
| `packages/` | Contains local packages used within the project. |
| `eslint.config.js` | ESLint configuration for code linting. |
| `index.html` | The main HTML entry point for the application. |
| `Soybean.md` / `Soybean.en_US.md` | Project documentation. |
| `LICENSE` | Project's license file. |
| `CHANGELOG.md` / `CHANGELOG.zh_CN.md` | Project's changelog. |

## `packages/` Directory

This directory contains local packages that are part of the monorepo, managed by pnpm workspaces.

| Package | Description |
| --- | --- |
| `alova/` | A lightweight request strategy library. It seems to be used for data fetching and caching. |
| `axios/` | A promise-based HTTP client for the browser and Node.js. This is likely a customized wrapper around Axios. |
| `color/` | A utility package for handling colors. It likely provides functions for color manipulation and conversion. |
| `hooks/` | Contains custom Vue Composition API hooks, promoting reusable logic across components. |
| `materials/` | Seems to contain reusable UI components or materials for building pages. |
| `ofetch/` | A lightweight fetching library, probably used as an alternative to `axios` or `alova` in some cases. |
| `scripts/` | Contains various scripts for automating tasks like changelog generation, releases, etc. |
| `uno-preset/` | A preset for UnoCSS, defining custom rules and shortcuts for the project's styling. |
| `utils/` | A collection of utility functions that can be used throughout the application. |

## `src/` Directory

This is the heart of the application, containing all the frontend source code.

### Root of `src/`

| File/Directory | Description |
| --- | --- |
| `App.vue` | The root Vue component of the application. |
| `main.ts` | The entry point of the application. It initializes Vue and other plugins. |
| `assets/` | Contains static assets like images and icons that are processed by Vite. |
| `components/` | Contains reusable Vue components. |
| `constants/` | Contains constant values used throughout the application. |
| `enum/` | Contains enum definitions. |
| `hooks/` | Contains application-specific custom hooks. |
| `layouts/` | Contains different page layouts (e.g., base layout, blank layout). |
| `locales/` | Contains files for internationalization (i18n). |
| `plugins/` | Contains application plugins. |
| `router/` | Contains the routing configuration. |
| `service/` | Contains services for API requests. |
| `store/` | Contains the state management setup (Pinia). |
| `styles/` | Contains global styles and CSS variables. |
| `theme/` | Contains theme configuration and settings. |
| `typings/` | Contains global TypeScript type definitions. |
| `utils/` | Contains utility functions specific to the main application. |
| `views/` | Contains the pages of the application. Each subdirectory usually represents a route. |

Now, let's dive deeper into each of these directories.

### `src/assets/`

This directory stores all static assets that are part of the source code and will be processed by Vite.

*   `imgs/`: Contains images used in the application.
*   `svg-icon/`: Contains SVG files used as icons.

### `src/components/`

This directory is for globally reusable Vue components.

*   `common/`: Basic, general-purpose components that can be used anywhere in the application.
    *   `app-provider.vue`: Provides global services like dialogs, messages, and notifications.
    *   `dark-mode-container.vue`: A container that handles dark mode styling.
    *   `exception-base.vue`: A base component for displaying exception pages (e.g., 404, 500).
    *   `full-screen.vue`: A component to toggle full-screen mode.
    *   `lang-switch.vue`: A component to switch the application's language.
    *   `menu-toggler.vue`: A component to toggle the sidebar menu.
    *   `pin-toggler.vue`: A component to pin or unpin elements.
    *   `reload-button.vue`: A button to refresh the current page.
    *   `system-logo.vue`: The system's logo component.
    *   `theme-schema-switch.vue`: A switch for changing the color scheme (e.g., light, dark, auto).
*   `custom/`: More complex or specialized components.
    *   `better-scroll.vue`: A wrapper around the BetterScroll library for smooth scrolling.
    *   `button-icon.vue`: A button component with an integrated icon.
    *   `count-to.vue`: A component that animates a number counting up to a target value.
    *   `look-forward.vue`: A placeholder or "coming soon" component.
    *   `soybean-avatar.vue`: A custom avatar component.
    * `svg-icon.vue`: A component for displaying SVG icons from the `assets/svg-icon` directory.
    * `wave-bg.vue`: A component for a wave-like background effect.
*   `advanced/`: Components that are highly complex or have very specific use cases.
    *   `table-column-setting.vue`: A component for customizing table columns.
    *   `table-header-operation.vue`: A component for operations in a table header.

### `src/constants/`

This directory holds constant values that are used across the application.

*   `app.ts`: Application-level constants, such as local storage keys.
*   `common.ts`: Common constants that don't fit into other categories.
*   `reg.ts`: Regular expressions for validation.

### `src/enum/`

This directory contains TypeScript enums.

*   `index.ts`: Exports all enums. It's likely that enums are defined in other files within this directory and aggregated here.

### `src/hooks/`

This directory contains custom Vue Composition API hooks for reusable logic. This is different from the `packages/hooks` directory, which contains more generic, cross-project hooks.

*   `business/`: Hooks related to specific business logic.
    *   `auth.ts`: Hooks for handling authentication logic.
    *   `captcha.ts`: A hook for managing captcha logic (e.g., sending and validating codes).
*   `common/`: General-purpose hooks for this application.
    *   `echarts.ts`: A hook for working with ECharts.
    *   `form.ts`: A hook to simplify form handling.
    *   `icon.ts`: A hook for managing icons.
    *   `router.ts`: A hook providing access to router-related functions.
    *   `table.ts`: A hook for managing table state and logic.

### `src/layouts/`

This directory defines the overall structure of pages in the application.

*   `base-layout/`: The main layout for the application, which likely includes a header, sider, footer, and content area.
*   `blank-layout/`: A minimal layout, often used for pages like login or 404.
*   `context/`: Provides context for the layouts, possibly for sharing state between layout components.
*   `modules/`: Contains the building blocks of the layouts, such as the global header, sider, tabs, etc. Each of these is a complex component in itself.

### `src/locales/`

This directory manages internationalization (i18n).

*   `langs/`: Contains the translation files for different languages (e.g., `en-us.ts`, `zh-cn.ts`).
*   `index.ts`: Initializes and configures the i18n instance.
*   `dayjs.ts`: Configures the locale for the `dayjs` library.
*   `naive.ts`: Configures the locale for the `naive-ui` component library.
*   `locale.ts`: Defines the available locales and the default locale.

### `src/plugins/`

This directory is for integrating and configuring third-party libraries as Vue plugins.

*   `index.ts`: Exports a function that installs all plugins.
*   `app.ts`: A core plugin for application-level setup.
*   `assets.ts`: A plugin to handle static assets.
*   `dayjs.ts`: A plugin to integrate the `dayjs` library.
*   `iconify.ts`: A plugin to integrate the `Iconify` library for icons.
*   `loading.ts`: A plugin to show a loading indicator during page transitions or API calls.
*   `nprogress.ts`: A plugin to show a progress bar at the top of the page during navigation.

### `src/router/`

This directory handles all aspects of routing.

*   `index.ts`: Creates and configures the Vue Router instance.
*   `elegant/`: Contains the logic for the "elegant-router," which seems to be a custom solution for generating routes.
    *   `imports.ts`: Handles dynamic imports of view components.
    *   `routes.ts`: Defines the static part of the routes.
    *   `transform.ts`: Transforms the route configuration into a format that Vue Router can understand.
*   `routes/`: Defines the application's routes.
    *   `index.ts`: Aggregates and exports all routes.
    *   `builtin.ts`: Defines built-in routes, such as for 404 pages or the login page.
*   `guard/`: Implements navigation guards.
    *   `index.ts`: Applies all navigation guards.
    *   `progress.ts`: A guard to control the NProgress loading bar.
    *   `route.ts`: The main guard that handles authentication and permissions.
    *   `title.ts`: A guard that updates the page title based on the current route.

### `src/service/`

This directory is responsible for communication with the backend API.

*   `api/`: Defines the API endpoints. Each file typically corresponds to a module on the backend.
    *   `auth.ts`: API calls related to authentication.
    *   `route.ts`: API calls for fetching routes from the backend.
    *   `index.ts`: Exports all API modules.
*   `request/`: Contains the core logic for making HTTP requests.
    *   `index.ts`: The main request function, likely a wrapper around a library like Axios or Alova.
    *   `shared.ts`: Shared configuration and utilities for requests.
    *   `type.ts`: TypeScript types for the request and response objects.

### `src/store/`

This directory contains the application's state management, using Pinia.

*   `index.ts`: Creates the Pinia instance and exports functions for using stores.
*   `modules/`: Each file represents a separate store module.
    *   `app/`: Stores general application state.
    *   `auth/`: Stores authentication-related state, such as tokens and user information.
    *   `route/`: Stores the dynamically generated routes.
    *   `tab/`: Stores the state of the tab navigation.
    *   `theme/`: Stores the current theme and layout settings.
*   `plugins/`: Contains Pinia plugins, for example, for persisting state to local storage.

### `src/styles/`

This directory contains global CSS and Scss files.

*   `css/`: Contains plain CSS files.
    *   `global.css`: Global styles that apply to the entire application.
    *   `nprogress.css`: Styles for the NProgress loading bar.
    *   `reset.css`: CSS reset to ensure consistent styling across browsers.
    *   `transition.css`: Styles for Vue transitions.
*   `scss/`: Contains Scss files for more advanced styling.
    *   `global.scss`: Global Scss styles.
    *   `scrollbar.scss`: Styles for custom scrollbars.

### `src/theme/`

This directory is for configuring the application's theme.

*   `settings.ts`: Defines the default theme settings, such as layout mode, color scheme, and other visual options.
*   `vars.ts`: Defines theme variables, likely for both light and dark modes.

### `src/typings/`

This directory contains global TypeScript declaration files (`.d.ts`). These are used to provide type information for modules that don't have their own types, or to declare global types.

*   This directory includes type definitions for various parts of the application, such as `api.d.ts`, `router.d.ts`, `storage.d.ts`, and for libraries like `naive-ui.d.ts`.
*   `elegant-router.d.ts`: Type definitions for the custom elegant-router.
*   `components.d.ts`: Auto-generated file with type definitions for components.

### `src/utils/`

This directory contains utility functions specific to the main application.

*   `agent.ts`: Utilities for detecting the user agent (e.g., browser, OS).
*   `common.ts`: Common utility functions.
*   `icon.ts`: Utility functions related to icons.
*   `service.ts`: Utility functions for interacting with services.
*   `storage.ts`: A wrapper around `localStorage` and `sessionStorage` for convenient and type-safe access.

### `src/views/`

This is where the application's pages are located. Each subdirectory typically represents a feature or a section of the application, and contains the `.vue` files for the pages.

*   `_builtin/`: Contains built-in pages that are part of the application's core functionality.
    *   `403/`, `404/`, `500/`: Exception pages.
    *   `iframe-page/`: A page for embedding content in an iframe.
    *   `login/`: The user login page, with modules for different login methods.
*   `home/`: The dashboard or home page.
*   `order-manage/`: A page for managing orders, which you have recently added.

This concludes the overview of the project structure. I hope this detailed documentation is helpful for your future development! 