import { request } from '../request';

/* ========== GET ========== */

/** 1. 根据id获取骑手信息 */
export function getRiderInfo(params: Api.Rider.Request) {
  return request<Api.Rider.InfoData>({
    url: `/api/riders/${params.riderId}`,
    method: 'get'
  });
}

/** 2. 根据id获取骑手订单列表 */
export function getRiderOrderList(riderId: string, params: Api.Rider.OrderListRequest) {
  return request<Api.Rider.Datum[]>({
    url: `/api/riders/${riderId}/assignments`,
    method: 'get',
    params
  });
}

/** 3. 根据id和时间获取骑手特定月份的绩效详情 */
export function getRiderPerformance(params: Api.Rider.TimeRequest) {
  return request<Api.Rider.TimeResponse>({
    url: `/api/riders/${params.riderId}/performance`,
    method: 'get',
    params
  });
}

/** 4. 根据id获取骑手考勤记录 */
export function getRiderCheckRecords(riderId: string, params: Api.Rider.CheckRequest) {
  return request<Api.Rider.CheckResponse>({
    url: `/api/riders/${riderId}/checks`,
    method: 'get',
    params
  });
}

/** 5. 根据id获取骑手排班列表 */
export function getRiderSchedule(riderId: string, params: Api.Rider.ScheduleRequest) {
  return request<Api.Rider.ScheduleResponse>({
    url: `/api/riders/${riderId}/schedules`,
    method: 'get',
    params
  });
}

/** 6. 根据骑手和订单id获取特定订单分配的详情 */
export function getRiderAssignDetail(params: Api.Rider.AssignRequest) {
  return request<Api.Rider.AssignResponse>({
    url: `/api/riders/${params.riderId}/assigns/${params.assignId}`,
    method: 'get'
  });
}

/* ========== POST ========== */

/** 1. 骑手注册 */
export function registerRider(data: Api.Rider.RegisterRequest) {
  return request<Api.Rider.RegisterResponse>({
    url: `/riders/register`,
    method: 'post',
    data
  });
}

/** 2. 创建考勤记录（签到） */
export function createAttendanceRecord(riderId: string, data: Api.Rider.AttendanceRequest) {
  return request<Api.Rider.AttendanceResponse>({
    url: `/riders/${riderId}/attendance`,
    method: 'post',
    data
  });
}

/* ========== PATCH ========== */

/** 1. 更新骑手信息 */
export function updateRiderInfo(riderId: string, data: Api.Rider.UpdateInfoRequest) {
  return request<Api.Rider.UpdateInfoData>({
    url: `/api/riders/${riderId}`,
    method: 'patch',
    data
  });
}

/** 2. 更新订单分配状态 */
export function updateAssignStatus(riderId: string, assignId: string, data: Api.Rider.UpdateAssignStatusRequest) {
  return request<Api.Rider.UpdateAssignData>({
    url: `/api/riders/${riderId}/assignments/${assignId}`,
    method: 'patch',
    data
  });
}

/** 3. 更新考勤记录（签退） */
export function updateAttendanceRecord(
  riderId: string,
  attendanceId: string,
  data: Api.Rider.UpdateAttendanceRecordRequest
) {
  return request<Api.Rider.UpdateAttendanceResponse>({
    url: `/riders/${riderId}/attendance/${attendanceId}`,
    method: 'patch',
    data
  });
}
