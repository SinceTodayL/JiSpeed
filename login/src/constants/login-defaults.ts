/**
 * 登录默认配置
 * 每个角色的默认账号和密码设置
 */

export interface DefaultCredentials {
  loginKey: string;
  password: string;
}

export interface RoleDefaultCredentials {
  user: DefaultCredentials;
  rider: DefaultCredentials;
  merchant: DefaultCredentials;
  admin: DefaultCredentials;
}

// 默认值配置
export const PRODUCTION_DEFAULTS: RoleDefaultCredentials = {
  user: {
    loginKey: '',
    password: ''
  },
  rider: {
    loginKey: '2351716@tongji.edu.cn',
    password: '123456'
  },
  merchant: {
    loginKey: 'test_1226',
    password: '123456'
  },
  admin: {
    loginKey: 'jispeed_plat',
    password: '123456'
  }
};

// 开发环境默认值（用于测试）
export const DEVELOPMENT_DEFAULTS: RoleDefaultCredentials = {
  user: {
    loginKey: 'TongJi',
    password: '6547265472'
  },
  rider: {
    loginKey: 'rider001',
    password: 'rider123456'
  },
  merchant: {
    loginKey: 'merchant001',
    password: 'merchant123456'
  },
  admin: {
    loginKey: 'admin',
    password: 'admin123456'
  }
};

// 根据环境自动选择默认值
export const getDefaultCredentials = (): RoleDefaultCredentials => {
  const isDevelopment = import.meta.env.DEV;
  return isDevelopment ? DEVELOPMENT_DEFAULTS : PRODUCTION_DEFAULTS;
};

// 角色映射配置
export const ROLE_CONFIG = {
  user: {
    userType: 1,
    frontendUrl: 'VITE_USER_FRONTEND_URL',
    displayName: '用户'
  },
  merchant: {
    userType: 2,
    frontendUrl: 'VITE_MERCHANT_FRONTEND_URL',
    displayName: '商户'
  },
  rider: {
    userType: 3,
    frontendUrl: 'VITE_RIDER_FRONTEND_URL',
    displayName: '骑手'
  },
  admin: {
    userType: 4,
    frontendUrl: 'VITE_ADMIN_FRONTEND_URL',
    displayName: '管理员'
  }
} as const;

/**
 * 自定义默认值配置说明：
 * 
 * 1. 修改开发环境默认值：
 *    直接修改 DEVELOPMENT_DEFAULTS 对象中的值
 * 
 * 2. 修改生产环境默认值：
 *    直接修改 PRODUCTION_DEFAULTS 对象中的值
 * 
 * 3. 添加新角色：
 *    在接口和配置对象中添加新的角色类型
 * 
 * 4. 环境检测：
 *    系统会自动根据 import.meta.env.DEV 判断环境
 */
