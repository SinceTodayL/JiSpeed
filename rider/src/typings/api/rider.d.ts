declare namespace Api {
  namespace Rider {
    /** 获取骑手信息请求参数 */
    export interface GetRiderInfoRequest {
      /** 骑手ID */
      riderId: string;
    }

    /** 获取骑手信息响应 */
    export interface GetRiderInfoResponse {
      /** 状态码 */
      code: number;
      /** 响应数据 */
      data: RiderInfoData;
      /** 响应消息 */
      message: string;
      /** 时间戳 */
      timestamp: number;
    }

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

    // 骑手详细信息（包含绩效）
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

    // 绩效数据
    export interface PerformanceData {
      statsMonth: string;
      totalOrders: number;
      onTimeRate: number;
      goodReviewRate: number;
      badReviewRate: number;
      income: number;
      [property: string]: any;
    }

    /** 获取骑手订单列表请求参数 */
    export interface GetRiderOrderListRequest {
      /** 接单状态（0:未处理, 1:已接单, 2:已拒单, 3:已完成） */
      status?: 0 | 1 | 2 | 3;
    }

    /** 获取骑手订单列表响应 */
    export interface GetRiderOrderListResponse {
      /** 状态码 */
      code: number;
      /** 响应数据 */
      data: OrderAssignmentData[];
      /** 响应消息 */
      message: string;
      /** 时间戳 */
      timestamp: number;
    }

    /** 订单分配数据 */
    export interface OrderAssignmentData {
      /** 分配ID */
      assignId: string;
      /** 骑手ID */
      riderId: string;
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
      /** 收货地址信息 */
      address: OrderAddress;
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

    // 兼容性：保留旧的接口名称
    /** @deprecated 请使用 GetRiderOrderListRequest */
    export type OrderListRequest = GetRiderOrderListRequest;

    /** @deprecated 请使用 GetRiderOrderListResponse */
    export type OrderListResponse = GetRiderOrderListResponse;

    /** @deprecated 请使用 OrderAssignmentData */
    export type Datum = OrderAssignmentData;

    /** @deprecated 请使用 OrderInfo */
    export type Order = OrderInfo;

    /** @deprecated 请使用 OrderAddress */
    export type Address = OrderAddress;

    // 3.根据id和时间获取骑手特定月份的绩效详情
    // Path参数
    export interface TimeRequest {
      month: number;
      riderId: string;
      year: number;
      [property: string]: any;
    }

    // 返回响应
    export interface TimeResponse {
      code: number;
      data: TimeData;
      message: string;
      timestamp: number;
      [property: string]: any;
    }

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

    // 4.根据id获取骑手考勤记录
    // Path参数：Request
    // Queay参数
    export interface CheckRequest {
      /**
       * 结束日期（可选）
       */
      endDate?: string;
      /**
       * 是否缺勤筛选（可选）
       */
      isAbsent?: number;
      /**
       * 是否迟到筛选（可选）
       */
      isLate?: number;
      /**
       * 开始日期（可选）
       */
      startDate?: string;
      [property: string]: any;
    }

    // 返回响应
    export interface CheckResponse {
      code: number;
      data: CheckDatum[];
      message: string;
      timestamp: number;
      [property: string]: any;
    }

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

    // 5.根据id获取骑手排班列表
    // Path参数：Request
    // Queay参数
    export interface ScheduleRequest {
      /**
       * 结束日期（可选）
       */
      endDate?: string;
      /**
       * 开始日期（可选）
       */
      startDate?: string;
      [property: string]: any;
    }

    // 返回响应
    export interface ScheduleResponse {
      code: number;
      data: ScheduleDatum[];
      message: string;
      timestamp: number;
      [property: string]: any;
    }

    export interface ScheduleDatum {
      scheduleId?: string;
      shiftEnd?: string;
      shiftStart?: string;
      workDate?: string;
      [property: string]: any;
    }
    // 6.根据骑手和订单id获取特定订单分配的详情
    // Path参数
    export interface AssignRequest {
      assignId: string;
      riderId: string;
      [property: string]: any;
    }

    // 返回响应
    export interface AssignResponse {
      code: number;
      data: AssignData;
      message: string;
      timestamp: number;
      [property: string]: any;
    }

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

    export interface AssignAddress {
      addressId: string;
      detailedAddress: string;
      receiverName: string;
      receiverPhone: string;
      [property: string]: any;
    }

    // 7.获取骑手最新位置
    export interface LocationLatestResponse {
      code: number;
      data: LocationData;
      message: string;
      timestamp: number;
      [property: string]: any;
    }

    // 8.获取骑手历史轨迹
    export interface LocationHistoryRequest {
      riderId: string;
      startTime?: string;
      endTime?: string;
      pageIndex?: number;
      pageSize?: number;
      [property: string]: any;
    }

    export interface LocationHistoryResponse {
      code: number;
      data: LocationData[];
      message: string;
      timestamp: number;
      [property: string]: any;
    }

    // 9.获取指定区域内的骑手
    export interface LocationAreaRequest {
      minLongitude: number;
      maxLongitude: number;
      minLatitude: number;
      maxLatitude: number;
      status?: number;
      [property: string]: any;
    }

    export interface LocationAreaResponse {
      code: number;
      data: LocationData[];
      message: string;
      timestamp: number;
      [property: string]: any;
    }

    // 10.获取所有在线骑手位置
    export interface LocationOnlineRequest {
      pageIndex?: number;
      pageSize?: number;
      [property: string]: any;
    }

    export interface LocationOnlineResponse {
      code: number;
      data: LocationData[];
      message: string;
      timestamp: number;
      [property: string]: any;
    }

    // 11.计算骑手到指定点的距离
    export interface LocationDistanceRequest {
      riderId: string;
      targetLongitude: number;
      targetLatitude: number;
      [property: string]: any;
    }

    export interface LocationDistanceResponse {
      code: number;
      data: DistanceData;
      message: string;
      timestamp: number;
      [property: string]: any;
    }

    export interface DistanceData {
      riderId: string;
      targetLongitude: number;
      targetLatitude: number;
      distance: number;
      [property: string]: any;
    }

    // 12.获取骑手当前位置的地址信息
    export interface LocationAddressResponse {
      code: number;
      data: AddressData;
      message: string;
      timestamp: number;
      [property: string]: any;
    }

    export interface AddressData {
      riderId: string;
      address: string;
      [property: string]: any;
    }

    // 13.获取骑手绩效趋势
    export interface PerformanceTrendRequest {
      riderId: string;
      months?: number;
      [property: string]: any;
    }

    export interface PerformanceTrendResponse {
      code: number;
      data: PerformanceTrendData[];
      message: string;
      timestamp: number;
      [property: string]: any;
    }

    export interface PerformanceTrendData {
      statsMonth: string;
      totalOrders: number;
      onTimeRate: number;
      goodReviewRate: number;
      badReviewRate: number;
      income: number;
      [property: string]: any;
    }

    // 14.获取骑手绩效排名
    export interface PerformanceRankingRequest {
      riderId: string;
      year: number;
      month: number;
      [property: string]: any;
    }

    export interface PerformanceRankingResponse {
      code: number;
      data: RankingData;
      message: string;
      timestamp: number;
      [property: string]: any;
    }

    export interface RankingData {
      totalOrdersRank: number;
      onTimeRateRank: number;
      goodReviewRateRank: number;
      incomeRank: number;
      overallRank: number;
      [property: string]: any;
    }

    // 15.获取绩效优秀骑手排行榜
    export interface PerformanceTopRequest {
      year: number;
      month: number;
      topCount?: number;
      [property: string]: any;
    }

    export interface PerformanceTopResponse {
      code: number;
      data: PerformanceTrendData[];
      message: string;
      timestamp: number;
      [property: string]: any;
    }

    // 16.获取月度绩效概览
    export interface PerformanceOverviewRequest {
      year: number;
      month: number;
      [property: string]: any;
    }

    export interface PerformanceOverviewResponse {
      code: number;
      data: OverviewData;
      message: string;
      timestamp: number;
      [property: string]: any;
    }

    export interface OverviewData {
      TotalRiders: number;
      TotalOrders: number;
      AverageOnTimeRate: number;
      AverageGoodReviewRate: number;
      TotalIncome: number;
      AverageIncome: number;
      [property: string]: any;
    }

    // 位置数据通用接口
    export interface LocationData {
      locationId: string;
      riderId: string;
      longitude: number;
      latitude: number;
      locationTime: string;
      accuracy: number;
      speed: number;
      direction: number;
      locationType: string;
      status: number;
      [property: string]: any;
    }

    /* POST */
    // 1.骑手注册
    // Body参数
    export interface RegisterRequest {
      name: string;
      phoneNumber: string;
      vehicleNumber: string;
      [property: string]: any;
    }

    // 返回响应
    export interface RegisterResponse {
      code: number;
      data: RegisterData;
      message: string;
      timestamp: number;
      [property: string]: any;
    }

    export interface RegisterData {
      name: string;
      phoneNumber: string;
      riderId: string;
      vehicleNumber: string;
      [property: string]: any;
    }

    // 2.创建考勤记录（签到）
    // Path参数：Request
    // Body参数
    export interface AttendanceRequest {
      checkDate: string;
      checkInAt: string;
      isAbsent: number;
      isLate: number;
      [property: string]: any;
    }

    // 返回响应
    export interface AttendanceResponse {
      code: number;
      data: AttendanceData;
      message: string;
      timestamp: number;
      [property: string]: any;
    }

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

    // 3.更新骑手位置
    export interface LocationUpdateRequest {
      riderId: string;
      longitude: number;
      latitude: number;
      accuracy?: number;
      speed?: number;
      direction?: number;
      locationType?: string;
      [property: string]: any;
    }

    export interface LocationUpdateResponse {
      code: number;
      data: LocationData;
      message: string;
      timestamp: number;
      [property: string]: any;
    }

    // 4.生成骑手月度绩效
    export interface PerformanceGenerateRequest {
      riderId: string;
      year: number;
      month: number;
      [property: string]: any;
    }

    export interface PerformanceGenerateResponse {
      code: number;
      data: PerformanceData;
      message: string;
      timestamp: number;
      [property: string]: any;
    }

    /** 更新骑手信息 */
    export interface UpdateInfoRequest {
      /** 骑手姓名 */
      name: string;
      /** 手机号码 */
      phoneNumber: string;
      /** 车辆号码 */
      vehicleNumber: string;
    }

    // 返回响应
    export interface UpdateInfoResponse {
      code: number;
      data: UpdateInfoData;
      message: string;
      timestamp: number;
      [property: string]: any;
    }

    export interface UpdateInfoData {
      applicationUserId: string;
      name: string;
      phoneNumber: string;
      riderId: string;
      vehicleNumber: string;
      [property: string]: any;
    }

    // 2. 更新订单分配状态
    // Path参数
    export interface UpdateAssignRequest {
      assignId: string;
      riderId: string;
      [property: string]: any;
    }
    // Body参数
    export interface UpdateAssignStatusRequest {
      acceptedStatus: number;
      [property: string]: any;
    }

    // 返回响应
    export interface UpdateAssignResponse {
      code: number;
      data: UpdateAssignData;
      message: string;
      timestamp: number;
      [property: string]: any;
    }

    export interface UpdateAssignData {
      acceptedAt: string;
      acceptedStatus: number;
      assignedAt: string;
      assignId: string;
      riderId: string;
      timeOut: null;
      [property: string]: any;
    }

    // 3.更新考勤记录（签退）
    // Path参数
    export interface UpdateAttendanceRequest {
      attendanceId: string;
      riderId: string;
      [property: string]: any;
    }
    // Body参数
    export interface UpdateAttendanceRecordRequest {
      checkoutAt: string;
      [property: string]: any;
    }

    // 返回响应
    export interface UpdateAttendanceResponse {
      code: number;
      data: UpdateAttendanceData;
      message: string;
      timestamp: number;
      [property: string]: any;
    }

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

    // 4.更新骑手在线状态
    export interface LocationStatusRequest {
      status: number;
      [property: string]: any;
    }

    export interface LocationStatusResponse {
      code: number;
      data: Record<string, never>;
      message: string;
      timestamp: number;
      [property: string]: any;
    }
  }
}
