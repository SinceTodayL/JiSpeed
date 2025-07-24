/**
 * 获取商家基本信息
 * @param merchantId 商家ID
 */
export function fetchMerchantInfo(merchantId: string) {
  // 使用正确的商家信息 Mock API 地址
  return fetch('https://m1.apifoxmock.com/m2/6776921-6489236-default/325542820')
    .then(response => {
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      
      return response.json();
    })
    .then(data => {
      // 检查响应格式：{ code: 0, message: "...", data: { merchantId, merchantName, ... } }
      if (data.code === 0 && data.data && typeof data.data === 'object') {
        return data.data; // 返回商家信息对象
      } else {
        throw new Error(data.message || '商家信息获取失败');
      }
    });
}

/**
 * 获取商家销售统计信息
 * @param merchantId 商家ID
 */
export function fetchMerchantSalesStats(merchantId: string) {
  // 使用正确的销售统计 Mock API 地址
  return fetch('https://m1.apifoxmock.com/m2/6776921-6489236-default/325543104')
    .then(response => {
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      
      return response.json();
    })
    .then(data => {
      // 检查响应格式：{ code: 0, message: "...", data: [{ statDate, salesQty, salesAmount, merchantId }, ...] }
      if (data.code === 0 && data.data && Array.isArray(data.data)) {
        return data.data; // 返回销售统计数组
      } else {
        throw new Error(data.message || '销售统计获取失败');
      }
    });
} 