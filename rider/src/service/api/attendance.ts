import { request } from '@/service/request';

/**
 * 考勤管理API
 */

// 获取考勤记录列表
export function getAttendanceRecords(params: {
  riderId: string;
  startDate: string;
  endDate: string;
}) {
  return request<Api.Attendance.AttendanceListResponse>({
    url: `/api/attendance/rider/${params.riderId}/daterange`,
    method: 'get',
    params: {
      startDate: params.startDate,
      endDate: params.endDate
    }
  });
}

// 获取骑手今日考勤记录
export function getTodayAttendance(riderId: string) {
  return request<Api.Attendance.AttendanceResponse>({
    url: `/api/attendance/today/${riderId}`,
    method: 'get'
  });
}

