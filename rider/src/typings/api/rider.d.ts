declare namespace Api {
  export namespace Rider {
    /* GET */
    // 1.根据id获取骑手信息
    // Path参数
    export interface Request {
      /**
       * 骑手ID
       */
      riderId: string;
      [property: string]: any;
    }

    // 返回响应
    export interface InfoResponse {
      code: number;
      data: InfoData;
      message: string;
      timestamp: number;
      [property: string]: any;
    }

    export interface InfoData {
      applicationUserId: string;
      name: string;
      phoneNumber: string;
      riderId: string;
      vehicleNumber: string;
      [property: string]: any;
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

    // 2.根据id获取骑手订单列表
    // Path参数：Request
    // Query参数
    export interface OrderListRequest {
      /**
       * 接单状态（0:未处理, 1:已接单, 2:已拒单, 3:已完成）
       */
      status?: number;
      [property: string]: any;
    }

    // 返回响应
    export interface OrderListResponse {
      code: number;
      data: Datum[];
      message: string;
      timestamp: number;
      [property: string]: any;
    }

    export interface Datum {
      acceptedAt?: string;
      acceptedStatus?: number;
      assignedAt?: string;
      assignId?: string;
      order?: Order;
      riderId?: string;
      timeOut?: number;
      [property: string]: any;
    }

    export interface Order {
      address: Address;
      createAt: string;
      orderAmount: number;
      orderId: string;
      orderStatus: number;
      [property: string]: any;
    }

    export interface Address {
      addressId: string;
      detailedAddress: string;
      receiverName: string;
      receiverPhone: string;
      [property: string]: any;
    }

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

    /* PATCH */
    // 1.更新骑手信息
    // Path参数：Request
    // Body参数
    export interface UpdateInfoRequest {
      name: string;
      phoneNumber: string;
      vehicleNumber: string;
      [property: string]: any;
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
  }
}
