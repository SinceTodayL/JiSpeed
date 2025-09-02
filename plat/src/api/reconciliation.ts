import { get, post, patch } from '../utils/request.js';

/**
 * 新增Reconciliation实体
 * @param reconciliation - 对账信息
 */
export function createReconciliation(reconciliation) {
  console.log('创建对账记录:', reconciliation);
  return post('/api/reconciliations', reconciliation);
}

/**
 * 根据reconId获取其实体和关联的orderID
 * @param reconId - 对账ID
 */
export function getReconciliationById(reconId) {
  console.log(`获取对账记录: ${reconId}`);
  return get(`/api/reconciliations/${reconId}`);
}

/**
 * 自定义筛选Reconciliations
 * @param params - 筛选参数
 */
export function getReconciliationList(params = {}) {
  console.log('获取对账列表', params);
  return get('/api/reconciliations', params);
}

/**
 * 管理员解决目标异常
 * @param reconId - 对账ID
 */
export function resolveReconciliation(reconId) {
  console.log(`解决对账异常: ${reconId}`);
  return patch(`/api/reconciliations/${reconId}/resolve`);
}

// 兼容旧代码
export { getReconciliationList as getReconciliations };
