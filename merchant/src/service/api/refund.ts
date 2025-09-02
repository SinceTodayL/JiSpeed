import { request } from '../request';

/**
 * 退款相关API
 */

/**
 * 获取商家拥有的退款申请ID列表
 * @param merchantId 商家ID
 * @param params 查询参数
 */
export function fetchMerchantRefunds(
  merchantId: string,
  params?: {
    size?: number;
    page?: number;
    auditStatus?: number; // 1, 2, 3
  }
) {
  const queryParams = new URLSearchParams();
  
  if (params?.size) queryParams.append('size', params.size.toString());
  if (params?.page) queryParams.append('page', params.page.toString());
  if (params?.auditStatus !== undefined) queryParams.append('auditStatus', params.auditStatus.toString());
  
  const queryString = queryParams.toString();
  const url = `/api/merchants/${merchantId}/refunds${queryString ? `?${queryString}` : ''}`;
  
  return request<string[]>({
    url,
    method: 'get'
  });
}

/**
 * 商家同意退款申请
 * @param merchantId 商家ID
 * @param refundId 退款申请ID
 */
export function approveRefund(merchantId: string, refundId: string) {
  return request<Api.Refund.RefundOperationResponse>({
    url: `/api/merchants/${merchantId}/refunds/${refundId}/approve`,
    method: 'patch',
    data: {}
  });
}

/**
 * 商家拒绝退款申请
 * @param merchantId 商家ID
 * @param refundId 退款申请ID
 */
export function rejectRefund(merchantId: string, refundId: string) {
  return request<Api.Refund.RefundOperationResponse>({
    url: `/api/merchants/${merchantId}/refunds/${refundId}/reject`,
    method: 'patch'
  });
}

/**
 * 退款状态映射
 */
export const REFUND_STATUS_MAP: Record<number, string> = {
  0: '默认状态',        // Default
  1: '商家拒绝',        // Rejected
  2: '商家退款',        // Refunded
  3: '等待管理员处理',   // DefaultForAdmin
  4: '管理员拒绝退款',   // RejectedForAdmin
  5: '管理员同意退款'    // RefundedForAdmin
};

/**
 * 根据退款ID获取退款详情
 * @param refundId 退款申请ID
 */
export function fetchRefundDetail(refundId: string) {
  return request<Api.Refund.RefundItem>({
    url: `/api/refunds/${refundId}`,
    method: 'get'
  });
}

/**
 * 批量获取退款详情
 * @param refundIds 退款申请ID数组
 */
export async function fetchRefundDetailsBatch(refundIds: string[]): Promise<Api.Refund.RefundItem[]> {
  try {
    console.log('=== 开始批量获取退款详情 ===');
    console.log('退款ID列表:', refundIds);
    
    const promises = refundIds.map(refundId => 
      fetchRefundDetail(refundId).then(response => {
        console.log(`退款${refundId}获取成功:`, response);
        return response?.data || null;
      }).catch(error => {
        console.warn(`获取退款详情失败 ${refundId}:`, error);
        return null;
      })
    );
    
    const results = await Promise.all(promises);
    const validResults = results.filter((result): result is Api.Refund.RefundItem => result !== null);
    
    console.log('批量获取退款详情结果:', validResults);
    return validResults;
  } catch (error) {
    console.error('批量获取退款详情失败:', error);
    return [];
  }
}

/**
 * 获取退款状态文本
 * @param status 退款状态码
 */
export function getRefundStatusText(status: number): string {
  return REFUND_STATUS_MAP[status] || '未知状态';
}
