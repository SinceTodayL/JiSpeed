// import { AMapLoader } from '@/config/amap';

/**
 * 高德地图API服务
 */

// ========== 输入提示 ==========

/**
 * 输入提示请求参数
 */
export interface InputTipsRequest {
  keywords: string; // 关键词
  city?: string; // 城市
  location?: string; // 当前位置 "经度,纬度"
  citylimit?: boolean; // 是否限制城市
  datatype?: string; // 数据类型
}

/**
 * 输入提示结果
 */
export interface InputTipsResult {
  id: string;
  name: string;
  district: string;
  adcode: string;
  location: string; // "经度,纬度"
  address: string;
  typecode: string;
}

/**
 * 输入提示 - 使用JSONP方式调用高德地图REST API
 */
export async function getInputTips(params: InputTipsRequest): Promise<{ data: { tips: InputTipsResult[] } }> {
  return new Promise((resolve, reject) => {
    // 构建请求URL
    const baseUrl = 'https://restapi.amap.com/v3/assistant/inputtips';
    const queryParams = new URLSearchParams({
      key: '2bc67deced375a36fb9db0937fb48d56',
      keywords: params.keywords,
      city: params.city || '',
      citylimit: params.citylimit ? 'true' : 'false',
      datatype: 'all',
      output: 'json'
    });

    const url = `${baseUrl}?${queryParams.toString()}`;

    // 创建JSONP请求
    const callbackName = `amap_callback_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;

    // 创建script标签
    const script = document.createElement('script');
    script.src = `${url}&callback=${callbackName}`;
    script.async = true;

    // 设置全局回调函数
    (window as any)[callbackName] = (data: any) => {
      try {
        if (data && data.status === '1' && data.tips) {
          const tips = data.tips.map((tip: any) => ({
            id: tip.id || `${tip.name}_${tip.district}`,
            name: tip.name || '',
            district: tip.district || '',
            adcode: tip.adcode || '',
            location: tip.location || '',
            address: tip.address || '',
            typecode: tip.typecode || ''
          }));
          resolve({ data: { tips } });
        } else {
          resolve({ data: { tips: [] } });
        }
      } catch (error) {
        reject(error);
      } finally {
        // 清理
        document.head.removeChild(script);
        (window as any)[callbackName] = undefined;
      }
    };

    // 错误处理
    script.onerror = () => {
      document.head.removeChild(script);
      (window as any)[callbackName] = undefined;
      reject(new Error('网络请求失败'));
    };

    // 添加到页面
    document.head.appendChild(script);

    // 设置超时
    setTimeout(() => {
      if (document.head.contains(script)) {
        document.head.removeChild(script);
        (window as any)[callbackName] = undefined;
        resolve({ data: { tips: [] } });
      }
    }, 5000);
  });
}
