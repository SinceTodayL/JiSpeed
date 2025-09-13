declare namespace Api {
  namespace OrderAssignment {
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

    // ========== 订单分配数据 ==========

    /** 订单分配信息 */
    export interface OrderAssignmentInfo {
      /** 分配ID */
      assignmentId: string;
      /** 订单ID */
      orderId: string;
      /** 骑手ID */
      riderId: string;
      /** 分配时间 */
      assignedAt: string;
      /** 接单状态 */
      status: 'pending' | 'accepted' | 'rejected' | 'completed' | 'cancelled';
      /** 接单时间 */
      acceptedAt?: string;
      /** 拒绝时间 */
      rejectedAt?: string;
      /** 完成时间 */
      completedAt?: string;
      /** 超时时间（分钟） */
      timeoutMinutes: number;
      /** 分配类型 */
      assignmentType: 'auto' | 'manual';
      /** 优先级 */
      priority: number;
      /** 订单信息 */
      order: OrderInfo;
      /** 骑手信息 */
      rider: RiderInfo;
    }

    /** 订单信息 */
    export interface OrderInfo {
      /** 订单ID */
      orderId: string;
      /** 订单金额 */
      orderAmount: number;
      /** 创建时间 */
      createdAt: string;
      /** 订单状态 */
      orderStatus: 'pending' | 'confirmed' | 'preparing' | 'ready' | 'delivering' | 'delivered' | 'cancelled';
      /** 收货地址 */
      deliveryAddress: {
        addressId: string;
        detailedAddress: string;
        receiverName: string;
        receiverPhone: string;
        coordinates: {
          longitude: number;
          latitude: number;
        };
      };
      /** 商家信息 */
      merchant: {
        merchantId: string;
        merchantName: string;
        merchantPhone: string;
        address: string;
        coordinates: {
          longitude: number;
          latitude: number;
        };
      };
      /** 预计送达时间 */
      estimatedDeliveryTime?: string;
      /** 特殊要求 */
      specialInstructions?: string;
    }

    /** 骑手信息 */
    export interface RiderInfo {
      /** 骑手ID */
      riderId: string;
      /** 骑手姓名 */
      riderName: string;
      /** 手机号 */
      phoneNumber: string;
      /** 当前位置 */
      currentLocation: {
        longitude: number;
        latitude: number;
        address?: string;
      };
      /** 在线状态 */
      onlineStatus: 'online' | 'offline' | 'busy';
      /** 当前任务数量 */
      currentTaskCount: number;
      /** 评分 */
      rating?: number;
    }

    // ========== 请求类型 ==========

    /** 自动分配订单请求 */
    export interface AutoAssignRequest {
      /** 订单ID */
      orderId: string;
      /** 分配策略 */
      strategy: 'nearest' | 'least_busy' | 'highest_rating' | 'balanced';
      /** 搜索半径（公里） */
      searchRadius?: number;
      /** 最大任务数限制 */
      maxTaskLimit?: number;
      /** 优先级 */
      priority?: number;
    }

    /** 手动分配订单请求 */
    export interface ManualAssignRequest {
      /** 订单ID */
      orderId: string;
      /** 骑手ID */
      riderId: string;
      /** 分配原因 */
      reason?: string;
      /** 优先级 */
      priority?: number;
    }

    /** 骑手接受订单请求 */
    export interface AcceptOrderRequest {
      /** 分配ID */
      assignmentId: string;
      /** 接受时间 */
      acceptedAt: string;
      /** 预计到达时间 */
      estimatedArrivalTime?: string;
    }

    /** 骑手拒绝订单请求 */
    export interface RejectOrderRequest {
      /** 分配ID */
      assignmentId: string;
      /** 拒绝时间 */
      rejectedAt: string;
      /** 拒绝原因 */
      reason: string;
    }

    /** 更新订单状态请求 */
    export interface UpdateOrderStatusRequest {
      /** 新状态 */
      status: 'accepted' | 'rejected' | 'completed' | 'cancelled';
      /** 更新时间 */
      updatedAt: string;
      /** 备注 */
      remarks?: string;
      /** 位置信息（完成时） */
      location?: {
        longitude: number;
        latitude: number;
        address?: string;
      };
    }

    /** 获取骑手分配列表请求 */
    export interface GetRiderAssignmentListRequest {
      /** 页码 */
      pageIndex?: number;
      /** 每页数量 */
      pageSize?: number;
      /** 状态筛选 */
      status?: 'pending' | 'accepted' | 'rejected' | 'completed' | 'cancelled';
      /** 开始日期 */
      startDate?: string;
      /** 结束日期 */
      endDate?: string;
      /** 排序字段 */
      sortBy?: 'assignedAt' | 'acceptedAt' | 'completedAt';
      /** 排序方向 */
      sortOrder?: 'asc' | 'desc';
    }

    // ========== 响应类型 ==========

    /** 自动分配订单响应 */
    export type AutoAssignResponse = ApiResponse<{
      assignmentId: string;
      riderId: string;
      assignedAt: string;
      message: string;
    }>;

    /** 手动分配订单响应 */
    export type ManualAssignResponse = ApiResponse<{
      assignmentId: string;
      riderId: string;
      assignedAt: string;
      message: string;
    }>;

    /** 骑手接受订单响应 */
    export type AcceptOrderResponse = ApiResponse<{
      assignmentId: string;
      status: string;
      acceptedAt: string;
      message: string;
    }>;

    /** 骑手拒绝订单响应 */
    export type RejectOrderResponse = ApiResponse<{
      assignmentId: string;
      status: string;
      rejectedAt: string;
      message: string;
    }>;

    /** 更新订单状态响应 */
    export type UpdateOrderStatusResponse = ApiResponse<{
      assignmentId: string;
      status: string;
      updatedAt: string;
      message: string;
    }>;

    /** 订单分配信息响应 */
    export type OrderAssignmentInfoResponse = ApiResponse<OrderAssignmentInfo>;

    /** 骑手分配列表响应 */
    export type RiderAssignmentListResponse = ApiResponse<{
      assignments: OrderAssignmentInfo[];
      total: number;
      pageIndex: number;
      pageSize: number;
    }>;
  }
}
