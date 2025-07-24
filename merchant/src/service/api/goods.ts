/**
 * 获取商家所有菜品
 * @param merchantId 商家ID (暂时不使用，因为使用固定的Mock地址)
 */
export function fetchGetAllDishes(merchantId: string) {
  // 直接使用您的完整Mock API地址
  return fetch('https://m1.apifoxmock.com/m2/6776921-6489236-default/325430434')
    .then(response => {
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      
      return response.json();
    })
    .then(data => {
      // 检查响应格式是否正确
      if (data.code === 0) {
        return data.data; // 返回data数组
      } else {
        throw new Error(data.message || '请求失败');
      }
    });
} 