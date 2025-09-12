import process from 'node:process';
import { URL, fileURLToPath } from 'node:url';
import { defineConfig, loadEnv } from 'vite';
import { setupVitePlugins } from './build/plugins';
import { createViteProxy, getBuildTime } from './build/config';

export default defineConfig(configEnv => {
  const viteEnv = loadEnv(configEnv.mode, process.cwd()) as unknown as Env.ImportMeta;

  // 设置默认环境变量，如果viteEnv中没有对应值则使用默认值
  const defaultEnv = {
    VITE_SERVICE_BASE_URL: 'https://localhost:5091',  // 指向JiSpeed后端API地址
    VITE_OTHER_SERVICE_BASE_URL: '{}',
    VITE_SERVICE_SUCCESS_CODE: '0000',
    VITE_HTTP_PROXY: 'N',  // 关闭代理
    VITE_PROXY_LOG: 'Y'   // 开启代理日志
  };
  
  const customEnv = Object.assign({}, defaultEnv, viteEnv) as Env.ImportMeta;

  const buildTime = getBuildTime();

  const enableProxy = configEnv.command === 'serve' && customEnv.VITE_HTTP_PROXY === 'Y';

  return {
    base: customEnv.VITE_BASE_URL,
    resolve: {
      alias: {
        '~': fileURLToPath(new URL('./', import.meta.url)),
        '@': fileURLToPath(new URL('./src', import.meta.url))
      }
    },
    css: {
      preprocessorOptions: {
        scss: {
          api: 'modern-compiler',
          additionalData: `@use "@/styles/scss/global.scss" as *;`
        }
      }
    },
    plugins: setupVitePlugins(customEnv, buildTime),
    define: {
      BUILD_TIME: JSON.stringify(buildTime),
      // 将自定义环境变量注入到运行时
      'import.meta.env.VITE_SERVICE_BASE_URL': JSON.stringify(customEnv.VITE_SERVICE_BASE_URL),
      'import.meta.env.VITE_SERVICE_SUCCESS_CODE': JSON.stringify(customEnv.VITE_SERVICE_SUCCESS_CODE),
      'import.meta.env.VITE_OTHER_SERVICE_BASE_URL': JSON.stringify(customEnv.VITE_OTHER_SERVICE_BASE_URL),
      'import.meta.env.VITE_HTTP_PROXY': JSON.stringify(customEnv.VITE_HTTP_PROXY)
    },
    server: {
      host: '0.0.0.0',
      port: 9527,
      open: true,
      proxy: createViteProxy(customEnv, enableProxy)
    },
    preview: {
      port: 9725
    },
    build: {
      reportCompressedSize: false,
      sourcemap: customEnv.VITE_SOURCE_MAP === 'Y',
      commonjsOptions: {
        ignoreTryCatch: false
      }
    }
  };
});
