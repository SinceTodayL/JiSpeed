import { get, patch } from '../utils/request.js';

/** Get user complaints by user ID */
export function fetchUserComplaints(userId) {
  return get(`/api/users/${userId}/complaints`);
}

/**
 * 获取投诉列表 - 自定义筛选
 * @param params - 查询参数 {userId, merchantId, status, adminId, size, page}
 */
export function fetchComplaintList(params = {}) {
  console.log('获取投诉列表，筛选参数:', params);
  return get('/api/complaints', params);
}

/**
 * 获取单个投诉详情
 * @param complaintId - 投诉ID
 */
export function fetchComplaintDetail(complaintId) {
  console.log('获取投诉详情，ID:', complaintId);
  return get(`/api/complaints/${complaintId}`);
}

/**
 * 管理员审批投诉申请
 * @param adminId - 管理员ID
 * @param complaintId - 投诉ID
 * @param auditData - 审批数据
 */
export function auditComplaint(adminId, complaintId, auditData) {
  console.log(`管理员 ${adminId} 审批投诉 ${complaintId}:`, auditData);
  // 注意：后端API路径中admin和complaints之间缺少斜杠
  return patch(`/api/admin/${adminId}complaints/${complaintId}/audit`, auditData);
}

/**
 * 管理员处理退款请求
 * @param adminId - 管理员ID
 * @param refundId - 退款ID
 * @param auditData - 审批数据
 */
export function auditRefund(adminId, refundId, auditData) {
  console.log(`管理员 ${adminId} 处理退款 ${refundId}:`, auditData);
  return patch(`/api/admins/${adminId}/refunds/${refundId}/audit`, auditData);
}

/**
 * 获取退款申请列表
 * @param params - 查询参数
 */
export function fetchRefundList(params = {}) {
  console.log('获取退款申请列表，筛选参数:', params);
  return get('/api/refunds', params);
}