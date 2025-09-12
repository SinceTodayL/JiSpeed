// 处理商家图片的工具函数
/**
 * 根据商家名称返回对应的图片路径
 * @param {string} merchantName - 商家名称
 * @param {string} defaultImage - 默认图片路径，如果没有匹配到特定商家
 * @returns {string} 图片路径
 */
export function getMerchantImage(merchantName, defaultImage = '/assets/placeholder.png') {
  if (!merchantName) return defaultImage;
  
  // 转换为字符串并处理为小写
  const name = String(merchantName).toLowerCase();
  
  // 特定商家的图片映射
  if (name.startsWith('老乡鸡')) {
    return '/assets/merchants/laoxiangji.png';
  } else if (name.startsWith('星巴克')) {
    return '/assets/merchants/xingbake.png';
  } else if (name.startsWith('熊大爷')) {
    return '/assets/merchants/xiongdaye.png';
  } else if (name.endsWith('可星(同济大学店)')) {
    return '/assets/merchants/taco.png';
  } else if (name.startsWith('小段')) {
    return '/assets/merchants/xiaoduan.png';
  } else if (name.startsWith('沙县')) {
    return '/assets/merchants/shaxian.png';
  } else if (name.startsWith('烤鱼')) {
    return '/assets/merchants/kaoyu.png';
  } else if (name.startsWith('老上海')) {
    return '/assets/merchants/laoshanghai.png';
  } else if (name.startsWith('融柳')) {
    return '/assets/merchants/rongliu.png';
  }else if (name.startsWith('福荣')) {
    return '/assets/merchants/furong.png';
  }

  
  // 对于其他商家，返回默认图片
  return defaultImage;
}

/**
 * 生成随机食物图片路径
 * @returns {string} 随机食物图片路径
 */
export function getRandomFoodImage() {
  // 随机食物图片数组
  const foodImages = [
    '/assets/foods/food1.png',
    '/assets/foods/food2.png',
    '/assets/foods/food3.png',
    '/assets/foods/food4.png',
    '/assets/foods/food5.png'
  ];
  
  // 随机选择一个图片
  const randomIndex = Math.floor(Math.random() * foodImages.length);
  console.log('随机选择的食物图片:', foodImages[randomIndex]);
  return foodImages[randomIndex];
}

/**
 * 获取商家图片，如果是特定商家则返回对应图片，否则返回随机食物图片
 * @param {string} merchantName - 商家名称
 * @returns {string} 图片路径
 */
export function getMerchantOrRandomImage(merchantName) {
  const merchantImage = getMerchantImage(merchantName, null);
  
  // 如果返回null，说明不是特定商家，返回随机食物图片
  if (merchantImage === null) {
    return getRandomFoodImage();
  }
  
  return merchantImage;
}
