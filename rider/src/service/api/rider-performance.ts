import { request } from '../request';

/* ========== 骑手绩效管理 API ========== */

/** 根据id和时间获取骑手特定月份的绩效详情 */
export function getRiderPerformance(params: Api.Rider.TimeRequest) {
  return request<Api.Rider.TimeResponse>({
    url: `/api/riders/${params.riderId}/performances/${params.year}/${params.month}`,
    method: 'get'
  });
}

/** 获取骑手绩效趋势 */
export function getRiderPerformanceTrend(params: Api.Rider.PerformanceTrendRequest) {
  return request<Api.Rider.PerformanceTrendResponse>({
    url: `/api/riders/${params.riderId}/performances/trend`,
    method: 'get',
    params
  });
}

/** 获取骑手绩效排名 */
export function getRiderPerformanceRanking(params: Api.Rider.PerformanceRankingRequest) {
  return request<Api.Rider.PerformanceRankingResponse>({
    url: `/api/riders/${params.riderId}/performances/ranking/${params.year}/${params.month}`,
    method: 'get'
  });
}

/** 获取绩效优秀骑手排行榜 */
export function getTopPerformers(params: Api.Rider.PerformanceTopRequest) {
  return request<Api.Rider.PerformanceTopResponse>({
    url: `/api/riders/performances/top-performers/${params.year}/${params.month}`,
    method: 'get',
    params
  });
}

/** 获取月度绩效概览 */
export function getMonthlyPerformanceOverview(params: Api.Rider.PerformanceOverviewRequest) {
  return request<Api.Rider.PerformanceOverviewResponse>({
    url: `/api/riders/performances/overview/${params.year}/${params.month}`,
    method: 'get'
  });
}

/** 生成骑手月度绩效 */
export function generateRiderPerformance(params: Api.Rider.PerformanceGenerateRequest) {
  return request<Api.Rider.PerformanceGenerateResponse>({
    url: `/api/riders/${params.riderId}/performances/generate/${params.year}/${params.month}`,
    method: 'post'
  });
}
