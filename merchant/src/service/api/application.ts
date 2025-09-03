import { request } from '../request';

/**
 * 申请相关API
 */

/**
 * 提交商家入驻申请
 * @param merchantId 商家ID
 * @param applicationData 申请数据
 */
export function submitApplication(
  merchantId: string,
  applicationData: {
    companyName: string;
    applicationMaterials: string;
  }
) {
  return request<Api.Application.SubmitApplicationResponse>({
    url: `/api/merchants/${merchantId}/applications`,
    method: 'post',
    data: {
      CompanyName: applicationData.companyName,
      ApplicationMaterials: applicationData.applicationMaterials
    }
  });
}

/**
 * 获取商家的申请列表
 * @param merchantId 商家ID
 * @param params 查询参数
 */
export function fetchApplications(
  merchantId: string,
  params?: {
    auditStatus?: number;
    size?: number;
    page?: number;
  }
) {
  return request<Api.Application.ApplicationResponse[]>({
    url: `/api/merchants/${merchantId}/applications`,
    method: 'get',
    params
  });
}

/**
 * 获取申请详情
 * @param applyId 申请ID
 */
export function fetchApplicationDetail(applyId: string) {
  return request<Api.Application.ApplicationResponse>({
    url: `/api/applications/${applyId}`,
    method: 'get'
  });
}
