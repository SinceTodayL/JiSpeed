declare namespace Api {
  namespace Rider {
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

    // ========== 骑手基础信息 ==========

    /** 骑手信息数据 */
    export interface RiderInfoData {
      /** 骑手ID */
      riderId: string;
      /** 骑手姓名 */
      name: string;
      /** 手机号码 */
      phoneNumber: string;
      /** 车辆号码 */
      vehicleNumber: string;
      /** 应用用户ID */
      applicationUserId: string;
    }

    /** 获取骑手信息响应 */
    export type GetRiderInfoResponse = ApiResponse<RiderInfoData>;

    /** 更新骑手信息请求 */
    export interface UpdateInfoRequest {
      /** 骑手姓名 */
      name: string;
      /** 手机号码 */
      phoneNumber: string;
      /** 车辆号码 */
      vehicleNumber: string;
    }

    /** 更新骑手信息响应数据 */
    export interface UpdateInfoData {
      applicationUserId: string;
      name: string;
      phoneNumber: string;
      riderId: string;
      vehicleNumber: string;
    }

    /** 更新骑手信息响应 */
    export interface UpdateInfoResponse {
      code: number;
      data: UpdateInfoData;
      message: string;
      timestamp: number;
      [property: string]: any;
    }

    /** 骑手注册请求 */
    export interface RegisterRequest {
      name: string;
      phoneNumber: string;
      vehicleNumber: string;
      [property: string]: any;
    }

    /** 骑手注册响应数据 */
    export interface RegisterData {
      name: string;
      phoneNumber: string;
      riderId: string;
      vehicleNumber: string;
      [property: string]: any;
    }

    /** 骑手注册响应 */
    export type RegisterResponse = ApiResponse<RegisterData>;

    /** 骑手详细信息（包含绩效） */
    export interface DetailData {
      applicationUserId: string;
      name: string;
      phoneNumber: string;
      riderId: string;
      vehicleNumber: string;
      performance?: PerformanceData;
      currentTaskCount: number;
      todayCompletedOrders: number;
      [property: string]: any;
    }

    // ========== 订单管理 ==========

    /** 获取骑手订单列表请求参数 */
    export interface GetRiderOrderListRequest {
      /** 接单状态（0:未处理, 1:已接单, 2:已拒单, 3:已完成） */
      status?: 0 | 1 | 2 | 3;
    }

    /** 获取骑手订单列表响应 */
    export type GetRiderOrderListResponse = ApiResponse<OrderAssignmentData[]>;

    /** 订单分配数据 */
    export interface OrderAssignmentData {
      /** 分配ID */
      assignId: string;
      /** 分配时间 */
      assignedAt: string;
      /** 接单状态（0:未处理, 1:已接单, 2:已拒单, 3:已完成） */
      acceptedStatus: number;
      /** 接单时间 */
      acceptedAt: string | null;
      /** 超时时间（分钟） */
      timeOut: number | null;
      /** 订单信息 */
      order: OrderInfo;
    }

    /** 订单信息 */
    export interface OrderInfo {
      /** 订单ID */
      orderId: string;
      /** 订单金额 */
      orderAmount: number;
      /** 创建时间 */
      createAt: string;
      /** 订单状态 */
      orderStatus: number;
      /** 配送地址 */
      deliveryAddress: string;
      /** 商家名称 */
      merchantName: string;
    }

    /** 订单收货地址 */
    export interface OrderAddress {
      /** 地址ID */
      addressId: string;
      /** 详细地址 */
      detailedAddress: string;
      /** 收货人姓名 */
      receiverName: string;
      /** 收货人电话 */
      receiverPhone: string;
    }

    /** 获取特定订单分配详情响应 */
    export type AssignResponse = ApiResponse<AssignData>;

    /** 订单分配详情数据 */
    export interface AssignData {
      acceptedAt: string;
      acceptedStatus: number;
      assignedAt: string;
      assignId: string;
      order: AssignOrder;
      riderId: string;
      timeOut: number;
      [property: string]: any;
    }

    /** 分配订单信息 */
    export interface AssignOrder {
      address: AssignAddress;
      addressId: string;
      createAt: string;
      orderAmount: number;
      orderId: string;
      orderStatus: number;
      userId: string;
      [property: string]: any;
    }

    /** 分配地址信息 */
    export interface AssignAddress {
      addressId: string;
      detailedAddress: string;
      receiverName: string;
      receiverPhone: string;
      [property: string]: any;
    }

    /** 更新订单分配状态请求 */
    export interface UpdateAssignStatusRequest {
      acceptedStatus: number;
      [property: string]: any;
    }

    /** 更新订单分配响应数据 */
    export interface UpdateAssignData {
      acceptedAt: string;
      acceptedStatus: number;
      assignedAt: string;
      assignId: string;
      riderId: string;
      timeOut: null;
      [property: string]: any;
    }

    /** 更新订单分配响应 */
    export type UpdateAssignResponse = ApiResponse<UpdateAssignData>;

    // ========== 考勤管理 ==========

    /** 考勤请求 */
    export interface AttendanceRequest {
      checkDate: string;
      checkInAt: string;
      isAbsent: number;
      isLate: number;
      [property: string]: any;
    }

    /** 考勤响应 */
    export type AttendanceResponse = ApiResponse<AttendanceData>;

    /** 考勤数据 */
    export interface AttendanceData {
      attendanceId: string;
      checkDate: string;
      checkInAt: string;
      checkoutAt: null;
      isAbsent: number;
      isLate: number;
      riderId: string;
      [property: string]: any;
    }

    /** 更新考勤记录请求 */
    export interface UpdateAttendanceRecordRequest {
      checkoutAt: string;
      [property: string]: any;
    }

    /** 更新考勤记录响应 */
    export type UpdateAttendanceResponse = ApiResponse<UpdateAttendanceData>;

    /** 更新考勤数据 */
    export interface UpdateAttendanceData {
      attendanceId: string;
      checkDate: string;
      checkInAt: string;
      checkoutAt: string;
      isAbsent: number;
      isLate: number;
      riderId: string;
      [property: string]: any;
    }

    /** 获取骑手考勤记录请求 */
    export interface CheckRequest {
      /** 结束日期（可选） */
      endDate?: string;
      /** 是否缺勤筛选（可选） */
      isAbsent?: number;
      /** 是否迟到筛选（可选） */
      isLate?: number;
      /** 开始日期（可选） */
      startDate?: string;
      [property: string]: any;
    }

    /** 获取骑手考勤记录响应 */
    export type CheckResponse = ApiResponse<CheckDatum[]>;

    /** 考勤记录数据 */
    export interface CheckDatum {
      attendanceId?: string;
      checkDate?: string;
      checkInAt?: string;
      checkoutAt?: string;
      isAbsent?: number;
      isLate?: number;
      riderId?: string;
      [property: string]: any;
    }

    // ========== 排班管理 ==========

    /** 获取骑手排班列表请求 */
    export interface ScheduleRequest {
      /** 结束日期（可选） */
      endDate?: string;
      /** 开始日期（可选） */
      startDate?: string;
      [property: string]: any;
    }

    /** 获取骑手排班列表响应 */
    export type ScheduleResponse = ApiResponse<ScheduleDatum[]>;

    /** 排班数据 */
    export interface ScheduleDatum {
      scheduleId?: string;
      shiftEnd?: string;
      shiftStart?: string;
      workDate?: string;
      [property: string]: any;
    }


    // ========== 绩效管理 ==========

    /** 绩效数据 */
    export interface PerformanceData {
      statsMonth: string;
      totalOrders: number;
      onTimeRate: number;
      goodReviewRate: number;
      badReviewRate: number;
      income: number;
      [property: string]: any;
    }

    /** 根据ID和时间获取骑手特定月份的绩效详情响应 */
    export type TimeResponse = ApiResponse<TimeData>;

    /** 绩效时间数据 */
    export interface TimeData {
      badReviewRate: number;
      goodReviewRate: number;
      income: number;
      onTimeRate: number;
      riderId: string;
      statsMonth: string;
      totalOrders: number;
      [property: string]: any;
    }

    /** 获取骑手绩效趋势请求 */
    export interface PerformanceTrendRequest {
      riderId: string;
      months?: number;
      [property: string]: any;
    }

    /** 获取骑手绩效趋势响应 */
    export type PerformanceTrendResponse = ApiResponse<PerformanceTrendData[]>;

    /** 绩效趋势数据 */
    export interface PerformanceTrendData {
      statsMonth: string;
      totalOrders: number;
      onTimeRate: number;
      goodReviewRate: number;
      badReviewRate: number;
      income: number;
      [property: string]: any;
    }

    /** 获取骑手绩效排名请求 */
    export interface PerformanceRankingRequest {
      riderId: string;
      year: number;
      month: number;
      [property: string]: any;
    }

    /** 获取骑手绩效排名响应 */
    export type PerformanceRankingResponse = ApiResponse<RankingData>;

    /** 排名数据 */
    export interface RankingData {
      totalOrdersRank: number;
      onTimeRateRank: number;
      goodReviewRateRank: number;
      incomeRank: number;
      overallRank: number;
      [property: string]: any;
    }

    /** 获取绩效优秀骑手排行榜请求 */
    export interface PerformanceTopRequest {
      year: number;
      month: number;
      topCount?: number;
      [property: string]: any;
    }

    /** 获取绩效优秀骑手排行榜响应 */
    export type PerformanceTopResponse = ApiResponse<PerformanceTrendData[]>;

    /** 获取月度绩效概览请求 */
    export interface PerformanceOverviewRequest {
      year: number;
      month: number;
      [property: string]: any;
    }

    /** 获取月度绩效概览响应 */
    export type PerformanceOverviewResponse = ApiResponse<OverviewData>;

    /** 概览数据 */
    export interface OverviewData {
      TotalRiders: number;
      TotalOrders: number;
      AverageOnTimeRate: number;
      AverageGoodReviewRate: number;
      TotalIncome: number;
      AverageIncome: number;
      [property: string]: any;
    }

    /** 生成骑手月度绩效请求 */
    export interface PerformanceGenerateRequest {
      riderId: string;
      year: number;
      month: number;
      [property: string]: any;
    }

    /** 生成骑手月度绩效响应 */
    export type PerformanceGenerateResponse = ApiResponse<PerformanceData>;

    // ========== 新增骑手相关类型 ==========

    /** 获取骑手列表请求 */
    export interface GetRiderListRequest {
      /** 页码 */
      pageIndex?: number;
      /** 每页数量 */
      pageSize?: number;
      /** 骑手姓名 */
      name?: string;
      /** 手机号 */
      phoneNumber?: string;
      /** 在线状态 */
      onlineStatus?: 'online' | 'offline' | 'busy';
      /** 排序字段 */
      sortBy?: 'name' | 'createdAt' | 'rating' | 'totalOrders';
      /** 排序方向 */
      sortOrder?: 'asc' | 'desc';
    }

    /** 骑手列表项 */
    export interface RiderListItem {
      /** 骑手ID */
      riderId: string;
      /** 骑手姓名 */
      name: string;
      /** 手机号码 */
      phoneNumber: string;
      /** 车辆号码 */
      vehicleNumber: string;
      /** 在线状态 */
      onlineStatus: 'online' | 'offline' | 'busy';
      /** 当前任务数量 */
      currentTaskCount: number;
      /** 今日完成订单数 */
      todayCompletedOrders: number;
      /** 评分 */
      rating?: number;
      /** 注册时间 */
      createdAt: string;
      /** 最后活跃时间 */
      lastActiveAt?: string;
    }

    /** 骑手列表响应 */
    export type RiderListResponse = ApiResponse<{
      riders: RiderListItem[];
      total: number;
      pageIndex: number;
      pageSize: number;
    }>;

  }
}
