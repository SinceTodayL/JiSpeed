declare namespace Api {
  namespace Attendance {
    // ========== 基础响应结构 ==========

    /** 通用API响应结构 */
    export interface ApiResponse<T = any> {
      /** 状态码 */
      code: number;
      /** 响应数据 */
      data: T;
      /** 响应消息 */
      message: string;
      /** 时间戳 */
      timestamp: number;
    }

    // ========== 考勤记录数据 ==========

    /** 考勤记录数据 */
    export interface AttendanceRecord {
      /** 考勤ID */
      attendanceId: string;
      /** 骑手ID */
      riderId: string;
      /** 考勤日期 */
      checkDate: string;
      /** 签到时间 */
      checkInAt: string | null;
      /** 签退时间 */
      checkoutAt: string | null;
      /** 是否迟到 */
      isLate: number;
      /** 是否缺勤 */
      isAbsent: number;
    }

    /** 考勤详情数据 */
    export interface AttendanceDetail extends AttendanceRecord {
      /** 骑手姓名 */
      riderName: string;
      /** 骑手手机号 */
      riderPhone: string;
      /** 备注 */
      remarks?: string;
    }

    // ========== 考勤记录查询请求 ==========

    /** 获取所有考勤记录请求 */
    export interface GetAllAttendanceRequest {
      /** 页码 */
      pageIndex?: number;
      /** 每页数量 */
      pageSize?: number;
      /** 开始日期 */
      startDate?: string;
      /** 结束日期 */
      endDate?: string;
      /** 骑手ID */
      riderId?: string;
      /** 是否迟到筛选 */
      isLate?: number;
      /** 是否缺勤筛选 */
      isAbsent?: number;
    }

    /** 获取骑手考勤记录请求 */
    export interface GetRiderAttendanceRequest {
      /** 页码 */
      pageIndex?: number;
      /** 每页数量 */
      pageSize?: number;
      /** 开始日期 */
      startDate?: string;
      /** 结束日期 */
      endDate?: string;
      /** 是否迟到筛选 */
      isLate?: number;
      /** 是否缺勤筛选 */
      isAbsent?: number;
    }

    /** 按日期范围查询考勤记录请求 */
    export interface GetAttendanceByDateRangeRequest {
      /** 开始日期 */
      startDate: string;
      /** 结束日期 */
      endDate: string;
      /** 页码 */
      pageIndex?: number;
      /** 每页数量 */
      pageSize?: number;
      /** 骑手ID */
      riderId?: string;
    }

    /** 获取骑手指定日期范围内的考勤记录请求 */
    export interface GetRiderAttendanceByDateRangeRequest {
      /** 开始日期 */
      startDate: string;
      /** 结束日期 */
      endDate: string;
      /** 页码 */
      pageIndex?: number;
      /** 每页数量 */
      pageSize?: number;
    }

    /** 获取迟到记录请求 */
    export interface GetLateAttendanceRequest {
      /** 页码 */
      pageIndex?: number;
      /** 每页数量 */
      pageSize?: number;
      /** 开始日期 */
      startDate?: string;
      /** 结束日期 */
      endDate?: string;
      /** 骑手ID */
      riderId?: string;
    }

    /** 获取缺勤记录请求 */
    export interface GetAbsentAttendanceRequest {
      /** 页码 */
      pageIndex?: number;
      /** 每页数量 */
      pageSize?: number;
      /** 开始日期 */
      startDate?: string;
      /** 结束日期 */
      endDate?: string;
      /** 骑手ID */
      riderId?: string;
    }

    /** 获取未签退记录请求 */
    export interface GetUncheckedOutRequest {
      /** 页码 */
      pageIndex?: number;
      /** 每页数量 */
      pageSize?: number;
      /** 日期 */
      date?: string;
      /** 骑手ID */
      riderId?: string;
    }

    /** 获取今日所有考勤记录请求 */
    export interface GetTodayAllAttendanceRequest {
      /** 页码 */
      pageIndex?: number;
      /** 每页数量 */
      pageSize?: number;
      /** 骑手ID */
      riderId?: string;
    }

    // ========== 考勤统计请求 ==========

    /** 获取迟到统计请求 */
    export interface GetLateStatsRequest {
      /** 开始日期 */
      startDate?: string;
      /** 结束日期 */
      endDate?: string;
      /** 骑手ID */
      riderId?: string;
    }

    /** 获取缺勤统计请求 */
    export interface GetAbsentStatsRequest {
      /** 开始日期 */
      startDate?: string;
      /** 结束日期 */
      endDate?: string;
      /** 骑手ID */
      riderId?: string;
    }

    /** 获取骑手考勤统计请求 */
    export interface GetRiderStatsRequest {
      /** 开始日期 */
      startDate?: string;
      /** 结束日期 */
      endDate?: string;
    }

    /** 计算工作时长请求 */
    export interface CalculateWorkHoursRequest {
      /** 骑手ID */
      riderId: string;
      /** 开始日期 */
      startDate: string;
      /** 结束日期 */
      endDate: string;
    }

    /** 获取考勤报表请求 */
    export interface GetAttendanceReportRequest {
      /** 开始日期 */
      startDate: string;
      /** 结束日期 */
      endDate: string;
      /** 骑手ID */
      riderId?: string;
      /** 报表类型 */
      reportType?: 'daily' | 'weekly' | 'monthly';
    }

    // ========== 考勤操作请求 ==========

    /** 签到请求 */
    export interface CheckInRequest {
      /** 考勤日期 */
      checkDate: string;
      /** 签到时间 */
      checkInAt: string;
      /** 是否迟到 */
      isLate: number;
      /** 是否缺勤 */
      isAbsent: number;
    }

    /** 签退请求 */
    export interface CheckOutRequest {
      /** 签退时间 */
      checkoutAt: string;
      /** 位置信息 */
      location?: {
        longitude: number;
        latitude: number;
        address?: string;
      };
      /** 备注 */
      remarks?: string;
    }

    /** 创建考勤记录请求 */
    export interface CreateAttendanceRequest {
      /** 骑手ID */
      riderId: string;
      /** 考勤日期 */
      checkDate: string;
      /** 签到时间 */
      checkInAt: string;
      /** 签退时间 */
      checkoutAt?: string;
      /** 是否迟到 */
      isLate: number;
      /** 是否缺勤 */
      isAbsent: number;
      /** 备注 */
      remarks?: string;
    }

    /** 标记缺勤请求 */
    export interface MarkAbsentRequest {
      /** 考勤日期 */
      checkDate: string;
      /** 缺勤原因 */
      absentReason: string;
      /** 备注 */
      remarks?: string;
    }

    // ========== 考勤统计响应 ==========

    /** 迟到统计响应 */
    export interface LateStatsData {
      /** 总迟到次数 */
      totalLateCount: number;
      /** 迟到率 */
      lateRate: number;
      /** 平均迟到时间（分钟） */
      averageLateMinutes: number;
      /** 最常迟到时间段 */
      mostLateTimeSlot: string;
    }

    /** 缺勤统计响应 */
    export interface AbsentStatsData {
      /** 总缺勤次数 */
      totalAbsentCount: number;
      /** 缺勤率 */
      absentRate: number;
      /** 连续缺勤天数 */
      consecutiveAbsentDays: number;
    }

    /** 骑手考勤统计响应 */
    export interface RiderStatsData {
      /** 总出勤天数 */
      totalWorkDays: number;
      /** 总工作时长（分钟） */
      totalWorkHours: number;
      /** 平均工作时长（分钟） */
      averageWorkHours: number;
      /** 迟到次数 */
      lateCount: number;
      /** 缺勤次数 */
      absentCount: number;
      /** 出勤率 */
      attendanceRate: number;
    }

    /** 工作时长响应 */
    export interface WorkHoursData {
      /** 总工作时长（分钟） */
      totalWorkHours: number;
      /** 总工作时长（小时） */
      totalWorkHoursInHours: number;
      /** 平均每日工作时长（分钟） */
      averageDailyWorkHours: number;
      /** 工作时长详情 */
      workHoursDetails: Array<{
        date: string;
        workHours: number;
        checkInAt: string;
        checkoutAt: string;
      }>;
    }

    /** 考勤报表响应 */
    export interface AttendanceReportData {
      /** 报表标题 */
      title: string;
      /** 报表期间 */
      period: string;
      /** 总出勤人数 */
      totalRiders: number;
      /** 总出勤天数 */
      totalWorkDays: number;
      /** 平均出勤率 */
      averageAttendanceRate: number;
      /** 迟到统计 */
      lateStats: LateStatsData;
      /** 缺勤统计 */
      absentStats: AbsentStatsData;
      /** 详细数据 */
      details: AttendanceRecord[];
    }

    // ========== 响应类型 ==========

    /** 考勤记录响应 */
    export type AttendanceResponse = ApiResponse<AttendanceRecord>;

    /** 考勤详情响应 */
    export type AttendanceDetailResponse = ApiResponse<AttendanceDetail>;

    /** 考勤记录列表响应 */
    export type AttendanceListResponse = ApiResponse<{
      records: AttendanceRecord[];
      total: number;
      pageIndex: number;
      pageSize: number;
    }>;

    /** 迟到统计响应 */
    export type LateStatsResponse = ApiResponse<LateStatsData>;

    /** 缺勤统计响应 */
    export type AbsentStatsResponse = ApiResponse<AbsentStatsData>;

    /** 骑手考勤统计响应 */
    export type RiderStatsResponse = ApiResponse<RiderStatsData>;

    /** 工作时长响应 */
    export type WorkHoursResponse = ApiResponse<WorkHoursData>;

    /** 考勤报表响应 */
    export type AttendanceReportResponse = ApiResponse<AttendanceReportData>;

    /** 签到响应 */
    export type CheckInResponse = ApiResponse<AttendanceRecord>;

    /** 签退响应 */
    export type CheckOutResponse = ApiResponse<AttendanceRecord>;

    /** 创建考勤记录响应 */
    export type CreateAttendanceResponse = ApiResponse<AttendanceRecord>;

    /** 标记缺勤响应 */
    export type MarkAbsentResponse = ApiResponse<AttendanceRecord>;
  }
}
