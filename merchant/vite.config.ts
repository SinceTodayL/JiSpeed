import process from 'node:process';
import { URL, fileURLToPath } from 'node:url';
import { defineConfig, loadEnv } from 'vite';
import { setupVitePlugins } from './build/plugins';
import { createViteProxy, getBuildTime } from './build/config';

export default defineConfig(configEnv => {
  const viteEnv = loadEnv(configEnv.mode, process.cwd()) as unknown as Env.ImportMeta;

  // 设置默认环境变量，如果viteEnv中没有对应值则使用默认值
  const defaultEnv = {
    // VITE_SERVICE_BASE_URL: 'https://m1.apifoxmock.com/m1/6776921-6489236-default',
    VITE_SERVICE_BASE_URL: 'http://121.4.90.75',
    VITE_OTHER_SERVICE_BASE_URL: '{}',
    VITE_SERVICE_SUCCESS_CODE: '0',
    VITE_HTTP_PROXY: 'N',
    VITE_PROXY_LOG: 'N',
    VITE_LOGIN_URL: 'http://121.4.90.75/login'
  };
  
  const customEnv = Object.assign({}, defaultEnv, viteEnv) as Env.ImportMeta;

  const buildTime = getBuildTime();

  const enableProxy = true;

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
      'import.meta.env.VITE_HTTP_PROXY': JSON.stringify(customEnv.VITE_HTTP_PROXY),
      'import.meta.env.VITE_LOGIN_URL': JSON.stringify(customEnv.VITE_LOGIN_URL)
    },

    server: {
      host: '0.0.0.0',
      port: 9520,
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
