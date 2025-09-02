import { get, post } from '../utils/request.js';

/** 结算单基础信息 */
export interface SettlementDto {
  settleId: string;
  periodStart: string;
  periodEnd: string;
  settledAt?: string;
}

/** 结算单详情信息 */
export interface SettlementDetailDto {
  settleId: string;
  periodStart: string;
  periodEnd: string;
  grossAmount: number;
  commissionFee: number;
  netAmount: number;
  settledAt?: string;
}

/** 扩展的结算单信息（用于前端显示） */
export interface SettlementItem extends SettlementDetailDto {
  merchantId?: string;
  merchantName?: string;
  statusText?: string;
  commissionRate?: number;
}

/** 结算查询参数 */
export interface SettlementQuery {
  startDate?: string;
  endDate?: string;
  isSettled?: boolean;
  page?: number;
  size?: number;
}

/** 结算列表响应 */
export interface SettlementListResponse {
  settlements: SettlementItem[];
  total: number;
}

/**
 * 获取结算单列表
 * GET /api/settlements
 */
export function getSettlements(params: SettlementQuery = {}): Promise<SettlementListResponse> {
  // 构建查询参数，过滤掉undefined值
  const queryParams: Record<string, any> = {};
  if (params.startDate) queryParams.startDate = params.startDate;
  if (params.endDate) queryParams.endDate = params.endDate;
  if (params.isSettled !== undefined) queryParams.isSettled = params.isSettled;
  if (params.page) queryParams.page = params.page;
  if (params.size) queryParams.size = params.size;
  else queryParams.size = 50;
  
  return get('/api/settlements', queryParams).then((response: any) => {
    // 处理后端包装的响应格式
    const responseData = response.data || response;
    const settlements: SettlementItem[] = responseData.map((item: SettlementDto) => ({
      ...item,
      grossAmount: 0, // 基础列表不包含这些字段，需要通过详情接口获取
      commissionFee: 0,
      netAmount: 0,
      statusText: item.settledAt ? '已结算' : '待结算'
    }));
    
    return {
      settlements,
      total: settlements.length
    };
  });
}

/**
 * 获取结算单详情
 * GET /api/settlements/{settlementId}
 */
export function getSettlementDetail(settleId: string): Promise<SettlementItem> {
  if (!settleId) {
    return Promise.reject(new Error('结算单ID不能为空'));
  }
  
  return get(`/api/settlements/${settleId}`).then((response: any) => {
    // 处理后端包装的响应格式
    const responseData = response.data || response;
    const result: SettlementItem = {
      ...responseData,
      statusText: responseData.settledAt ? '已结算' : '待结算',
      commissionRate: responseData.grossAmount > 0 ? (responseData.commissionFee / responseData.grossAmount * 100) : 0
    };
    
    return result;
  });
}

/**
 * 根据商家ID获取结算单列表
 * GET /api/settlements/merchants/{merchantId}
 */
export function getSettlementsByMerchant(merchantId: string, params: SettlementQuery = {}): Promise<SettlementListResponse> {
  if (!merchantId) {
    return Promise.reject(new Error('商家ID不能为空'));
  }
  
  // 构建查询参数，过滤掉undefined值
  const queryParams: Record<string, any> = {};
  if (params.startDate) queryParams.startDate = params.startDate;
  if (params.endDate) queryParams.endDate = params.endDate;
  if (params.isSettled !== undefined) queryParams.isSettled = params.isSettled;
  if (params.page) queryParams.page = params.page;
  if (params.size) queryParams.size = params.size;
  else queryParams.size = 50;
  
  return get(`/api/settlements/merchants/${merchantId}`, queryParams).then((response: any) => {
    // 处理后端包装的响应格式
    const responseData = response.data || response;
    const settlements: SettlementItem[] = responseData.map((item: SettlementDto) => ({
      ...item,
      merchantId,
      grossAmount: 0, // 基础列表不包含这些字段，需要通过详情接口获取
      commissionFee: 0,
      netAmount: 0,
      statusText: item.settledAt ? '已结算' : '待结算'
    }));
    
    return {
      settlements,
      total: settlements.length
    };
  });
}


