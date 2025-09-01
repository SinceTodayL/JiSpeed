import { get, patch } from '../utils/request.js';

/**
 * 自定义筛选条件获取商家列表 - 使用真实Apifox接口
 * @param params - 筛选参数
 */
export function fetchMerchantList(params = {}) {
  console.log('获取商家列表', params);
  return get('/api/merchants', params);
}

/** Get merchant info by merchant ID */
export function fetchMerchantInfo(merchantId) {
  console.log(`获取商家信息: ${merchantId}`);
  return get(`/api/merchants/${merchantId}`);
}

/** Get merchant sales statistics by merchant ID */
export function fetchMerchantSalesStats(merchantId) {
  console.log(`获取商家销售统计: ${merchantId}`);
  return get(`/api/merchants/${merchantId}/sales-stats`);
}

/** Get dish reviews by merchant ID and dish ID */
export function fetchDishReviews(merchantId, dishId) {
  console.log(`获取菜品评价: ${merchantId}/${dishId}`);
  return get(`/api/merchants/${merchantId}/dishes/${dishId}/reviews`);
}

/**
 * 更新商家信息（使用PATCH /api/merchants/{merchantId}）
 * @param merchantId - 商家ID
 * @param updateData - 更新数据 {MerchantName, Status, ContactInfo, Location, Description}
 */
export function updateMerchantInfo(merchantId, updateData) {
  console.log(`更新商家信息，ID: ${merchantId}`, updateData);
  return patch(`/api/merchants/${merchantId}`, updateData);
}

/**
 * 封禁商家
 * @param merchantId - 商家ID
 * @param reason - 封禁原因
 */
export function banMerchant(merchantId, reason = '') {
  console.log(`封禁商家，ID: ${merchantId}，原因: ${reason}`);
  return updateMerchantInfo(merchantId, {
    Status: 0, // 0表示停用/封禁
    Description: reason ? `封禁原因: ${reason}` : '管理员封禁'
  });
}

/**
 * 解封商家
 * @param merchantId - 商家ID
 */
export function unbanMerchant(merchantId) {
  console.log(`解封商家，ID: ${merchantId}`);
  return updateMerchantInfo(merchantId, {
    Status: 1, // 1表示正常
    Description: '已解封'
  });
}

/**
 * 更新商家状态（向后兼容）
 * @param merchantId - 商家ID
 * @param status - 状态 (0: 停用, 1: 启用)
 */
export function updateMerchantStatus(merchantId, status) {
  console.log(`更新商家状态: ${merchantId} -> ${status}`);
  return updateMerchantInfo(merchantId, { Status: status });
}

/**
 * 获取商家详细统计信息
 * @param merchantId - 商家ID
 * @param params - 查询参数（时间范围等）
 */
export function fetchMerchantDetailStats(merchantId, params = {}) {
  console.log(`获取商家详细统计: ${merchantId}`, params);
  // 这个接口可能需要后端提供
  return get(`/api/merchants/${merchantId}/detail-stats`, params);
}

// 工具: 格式化商家状态
export function formatMerchantStatus(status) {
  switch (status) {
    case 1:
      return '正常';
    case 0:
      return '已封禁';
    case 2:
      return '暂停';
    default:
      return '未知';
  }
}

/**
 * 管理员根据管理员ID和自定义筛选条件获取事务申请
 * @param adminId - 管理员ID
 * @param params - 筛选参数
 */
export function fetchApplications(adminId = '6f7af74d972c481c91f19596e07aae3a', params = {}) {
  console.log(`获取申请列表，管理员ID: ${adminId}`, params);
  return get(`/api/admin/${adminId}/applications`, params);
}

/**
 * 根据申请ID获取申请状态
 * @param applicationId - 申请ID
 */
export function fetchApplicationStatus(applicationId) {
  console.log(`获取申请状态，ID: ${applicationId}`);
  return get(`/api/applications/${applicationId}`);
}

/**
 * 管理员同意/拒绝申请
 * @param adminId - 管理员ID
 * @param applyId - 申请ID
 * @param auditData - 审核数据 {decision: 'approve'|'reject', reason?: string}
 */
export function auditApplication(adminId, applyId, auditData) {
  console.log(`审核申请，管理员ID: ${adminId}, 申请ID: ${applyId}`, auditData);
  return patch(`/api/admin/${adminId}/audit/${applyId}`, auditData);
}

// 工具: 获取状态对应的标签类型
export function getMerchantStatusType(status) {
  switch (status) {
    case 1:
      return 'success'; // 正常 - 绿色
    case 0:
      return 'error';   // 封禁 - 红色
    case 2:
      return 'warning'; // 暂停 - 黄色
    default:
      return 'default'; // 未知 - 灰色
  }
}

// 工具: 格式化申请状态
export function formatApplicationStatus(status) {
  const statusMap = {
    0: '待审核',
    1: '已同意',
    2: '已拒绝',
    'pending': '待审核',
    'approved': '已同意',
    'rejected': '已拒绝'
  };
  return statusMap[status] || '未知状态';
}

// 工具: 获取申请状态标签类型
export function getApplicationStatusType(status) {
  switch (status) {
    case 0:
    case 'pending':
      return 'warning'; // 待审核 - 黄色
    case 1:
    case 'approved':
      return 'success'; // 已同意 - 绿色
    case 2:
    case 'rejected':
      return 'error';   // 已拒绝 - 红色
    default:
      return 'default'; // 未知 - 灰色
  }
}