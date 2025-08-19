import { get } from '../utils/request.js';

/**
 * 获取月度绩效概览 - 调用Apifox真实接口
 * @param year - 年份
 * @param month - 月份
 */
export function getPerformanceOverview(year, month) {
  const m = String(month).padStart(2, '0');
  console.log(`调用Apifox API获取绩效概览: /api/riders/performances/overview/${year}/${m}`);
  
  // 直接调用Apifox接口，不使用任何模拟数据
  return get(`/api/riders/performances/overview/${year}/${m}`);
}

/**
 * 获取最近几个月的订单趋势数据
 * @param months - 要获取的月份数量
 */
export function getOrderTrends(months = 6) {
  // 调用后端真实接口获取订单趋势
  return get('/api/riders/order-trends', { months });
}

/** Get rider info by ID */
export function fetchRiderInfo(riderId) {
  console.log(`获取骑手信息: ${riderId}`);
  return get(`/api/riders/${riderId}`);
}

/** Get rider performance ranking */
export function fetchRiderPerformanceRanking(riderId, year, month) {
  console.log(`获取骑手绩效排名: ${riderId}, ${year}年${month}月`);
  const monthStr = String(month).padStart(2, '0');
  return get(`/api/riders/${riderId}/performances/ranking/${year}/${monthStr}`);
}

/** Get rider performance by rider ID, year and month */
export function fetchRiderPerformance(riderId, year, month) {
  return get(`/api/riders/${riderId}/performances/${year}/${month}`);
}

/** Get all riders list */
export function fetchRidersList(params = {}) {
  console.log('获取骑手列表', params);
  return get('/api/riders', params);
}

/** Get top performing riders */
export async function fetchTopPerformingRiders(year, month) {
  const y = Number(year);
  const mNum = Number(month);
  const mStr = String(mNum).padStart(2, '0');

  // 调用Apifox接口
  return get(`/api/riders/performances/top-performers/${y}/${mStr}`);
}

// 工具: 格式化百分比
export function formatPercentage(value) {
  if (typeof value !== 'number') return '-';
  return `${(value * 100).toFixed(2)}%`;
}

// 工具: 格式化收入，保留两位小数并加千分位
export function formatIncome(value) {
  if (typeof value !== 'number') return '-';
  return value.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
}