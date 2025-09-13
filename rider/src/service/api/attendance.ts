import { request } from '../request';

// ========== 考勤记录查询 ==========

/**
 * 获取今日考勤记录
 */
export function getTodayAttendance(riderId: string) {
  return request<Api.Attendance.AttendanceResponse>({
    url: `/api/Attendance/today/${riderId}`,
    method: 'get'
  });
}

/**
 * 获取某个考勤记录详情
 */
export function getAttendanceDetail(attendanceId: string) {
  return request<Api.Attendance.AttendanceDetailResponse>({
    url: `/api/Attendance/${attendanceId}`,
    method: 'get'
  });
}

/**
 * 获取所有考勤记录
 */
export function getAllAttendanceRecords(params: Api.Attendance.GetAllAttendanceRequest = {}) {
  return request<Api.Attendance.AttendanceListResponse>({
    url: '/api/Attendance',
    method: 'get',
    params
  });
}

/**
 * 获取骑手考勤记录
 */
export function getRiderAttendanceRecordsList(riderId: string, params: Api.Attendance.GetRiderAttendanceRequest = {}) {
  return request<Api.Attendance.AttendanceListResponse>({
    url: `/api/Attendance/rider/${riderId}`,
    method: 'get',
    params
  });
}

/**
 * 按日期范围查询考勤记录
 */
export function getAttendanceByDateRange(params: Api.Attendance.GetAttendanceByDateRangeRequest) {
  return request<Api.Attendance.AttendanceListResponse>({
    url: '/api/Attendance/daterange',
    method: 'get',
    params
  });
}

/**
 * 获取骑手指定日期范围内的考勤记录
 */
export function getRiderAttendanceByDateRange(riderId: string, params: Api.Attendance.GetRiderAttendanceByDateRangeRequest) {
  return request<Api.Attendance.AttendanceListResponse>({
    url: `/api/Attendance/rider/${riderId}/daterange`,
    method: 'get',
    params
  });
}

/**
 * 获取迟到记录
 */
export function getLateAttendanceRecords(params: Api.Attendance.GetLateAttendanceRequest = {}) {
  return request<Api.Attendance.AttendanceListResponse>({
    url: '/api/Attendance/late',
    method: 'get',
    params
  });
}

/**
 * 获取缺勤记录
 */
export function getAbsentAttendanceRecords(params: Api.Attendance.GetAbsentAttendanceRequest = {}) {
  return request<Api.Attendance.AttendanceListResponse>({
    url: '/api/Attendance/absent',
    method: 'get',
    params
  });
}

/**
 * 获取未签退记录
 */
export function getUncheckedOutRecords(params: Api.Attendance.GetUncheckedOutRequest = {}) {
  return request<Api.Attendance.AttendanceListResponse>({
    url: '/api/Attendance/not-checked-out',
    method: 'get',
    params
  });
}

/**
 * 获取今日所有考勤记录
 */
export function getTodayAllAttendanceRecords(params: Api.Attendance.GetTodayAllAttendanceRequest = {}) {
  return request<Api.Attendance.AttendanceListResponse>({
    url: '/api/Attendance/today',
    method: 'get',
    params
  });
}

// ========== 考勤统计 ==========

/**
 * 获取迟到统计
 */
export function getLateAttendanceStats(params: Api.Attendance.GetLateStatsRequest = {}) {
  return request<Api.Attendance.LateStatsResponse>({
    url: '/api/Attendance/stats/late',
    method: 'get',
    params
  });
}

/**
 * 获取缺勤统计
 */
export function getAbsentAttendanceStats(params: Api.Attendance.GetAbsentStatsRequest = {}) {
  return request<Api.Attendance.AbsentStatsResponse>({
    url: '/api/Attendance/stats/absent',
    method: 'get',
    params
  });
}

/**
 * 获取骑手考勤统计
 */
export function getRiderAttendanceStats(riderId: string, params: Api.Attendance.GetRiderStatsRequest = {}) {
  return request<Api.Attendance.RiderStatsResponse>({
    url: `/api/Attendance/stats/rider/${riderId}`,
    method: 'get',
    params
  });
}

/**
 * 计算工作时长
 */
export function calculateWorkHours(riderId: string, checkDate: string) {
  return request<Api.Attendance.WorkHoursResponse>({
    url: `/api/Attendance/working-hours/${riderId}`,
    method: 'get',
    params: { checkDate }
  });
}

/**
 * 获取考勤报表
 */
export function getAttendanceReport(params: Api.Attendance.GetAttendanceReportRequest) {
  return request<Api.Attendance.AttendanceReportResponse>({
    url: '/api/Attendance/report',
    method: 'get',
    params
  });
}

// ========== 考勤操作 ==========

/**
 * 骑手签退
 */
export function riderCheckOutAttendance(riderId: string) {
  return request<Api.Attendance.CheckOutResponse>({
    url: `/api/Attendance/checkout/${riderId}`,
    method: 'post'
  });
}

/**
 * 创建考勤记录
 */
export function createAttendanceRecord(data: Api.Attendance.CreateAttendanceRequest) {
  return request<Api.Attendance.CreateAttendanceResponse>({
    url: '/api/Attendance',
    method: 'post',
    data
  });
}

/**
 * 标记缺勤
 */
export function markAbsent(data: Api.Attendance.MarkAbsentRequest) {
  return request<Api.Attendance.MarkAbsentResponse>({
    url: `/api/Attendance/mark-absent`,
    method: 'post',
    data
  });
}

/**
 * 骑手签到
 */
export function riderCheckInAttendance(riderId: string) {
  return request<Api.Attendance.CheckInResponse>({
    url: `/api/Attendance/checkin/${riderId}`,
    method: 'post'
  });
}
