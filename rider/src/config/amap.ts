export const amapConfig = {
  apiKey: '2bc67deced375a36fb9db0937fb48d56',
  version: '2.0',
  securityJsCode: '',
  plugins: ['AMap.Scale', 'AMap.ToolBar', 'AMap.Driving', 'AMap.Geolocation', 'AMap.Marker', 'AMap.InfoWindow']
} as const;

// 高德地图加载器
export class AMapLoader {
  private static instance: any = null;
  private static loading = false;
  private static callbacks: Array<(AMap: any) => void> = [];

  static async load(): Promise<any> {
    if (this.instance) {
      return this.instance;
    }

    if (this.loading) {
      return new Promise((resolve) => {
        this.callbacks.push(resolve);
      });
    }

    this.loading = true;

    return new Promise((resolve, reject) => {
      // 检查是否已经加载
      if (window.AMap) {
        this.instance = window.AMap;
        this.loading = false;
        resolve(this.instance);
        return;
      }

      // 动态加载高德地图脚本
      const script = document.createElement('script');
      script.type = 'text/javascript';
      script.async = true;
      script.src = `https://webapi.amap.com/maps?v=${amapConfig.version}&key=${amapConfig.apiKey}&plugin=${amapConfig.plugins.join(',')}`;

      script.onload = () => {
        this.instance = window.AMap;
        this.loading = false;
        resolve(this.instance);

        // 执行等待中的回调
        this.callbacks.forEach(callback => callback(this.instance));
        this.callbacks = [];
      };

      script.onerror = () => {
        this.loading = false;
        reject(new Error('Failed to load AMap'));
      };

      document.head.appendChild(script);
    });
  }
}

// 扩展Window接口
declare global {
  interface Window {
    AMap: any;
  }
}
