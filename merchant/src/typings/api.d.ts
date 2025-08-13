/**
 * Namespace Api
 *
 * All backend api type
 */
declare namespace Api {
  namespace Common {
    /** common params of paginating */
    interface PaginatingCommonParams {
      /** current page number */
      current: number;
      /** page size */
      size: number;
      /** total count */
      total: number;
    }

    /** common params of paginating query list data */
    interface PaginatingQueryRecord<T = any> extends PaginatingCommonParams {
      records: T[];
    }

    /** common search params of table */
    type CommonSearchParams = Pick<Common.PaginatingCommonParams, 'current' | 'size'>;

    /**
     * enable status
     *
     * - "1": enabled
     * - "2": disabled
     */
    type EnableStatus = '1' | '2';

    /** common record */
    type CommonRecord<T = any> = {
      /** record id */
      id: number;
      /** record creator */
      createBy: string;
      /** record create time */
      createTime: string;
      /** record updater */
      updateBy: string;
      /** record update time */
      updateTime: string;
      /** record status */
      status: EnableStatus | null;
    } & T;
  }

  /**
   * namespace Auth
   *
   * backend api module: "auth"
   */
  namespace Auth {
    interface LoginToken {
      token: string;
      refreshToken: string;
    }

    interface UserInfo {
      userId: string;
      userName: string;
      roles: string[];
      buttons: string[];
      merchantId?: string; // 商家ID，登录后获取
    }
  }

  /**
   * namespace Route
   *
   * backend api module: "route"
   */
  namespace Route {
    type ElegantConstRoute = import('@elegant-router/types').ElegantConstRoute;

    interface MenuRoute extends ElegantConstRoute {
      id: string;
    }

    interface UserRoute {
      routes: MenuRoute[];
      home: import('@elegant-router/types').LastLevelRouteKey;
    }
  }

  /**
   * namespace Goods
   *
   * backend api module: "goods"
   */
  namespace Goods {
    /** Dish item - matches backend DishesDto structure */
    interface DishItem {
      /** 菜品ID - maps to DishId */
      dishId: string;
      /** 分类ID - maps to CategoryId */
      categoryId: string;
      /** 菜品名称 - maps to DishName */
      dishName: string;
      /** 价格 - maps to Price */
      price: number;
      /** 原价 - maps to OriginPrice */
      originPrice: number;
      /** 封面图片URL - maps to CoverUrl */
      coverUrl: string;
      /** 月销量 - maps to MonthlySales */
      monthlySales: number | null;
      /** 评分 - maps to Rating */
      rating: number | null;
      /** 是否在售 1:在售 0:停售 - maps to OnSale */
      onSale: number | null;
      /** 商家ID - maps to MerchantId */
      merchantId: string | null;
      /** 库存数量 - NOT in backend DTO, frontend only */
      quantity?: number;
      /** 评论数量 - maps to ReviewQuantity */
      reviewQuantity?: number | null;
      /** 分类名称 - maps to CategoryName */
      categoryName?: string | null;
    }

    /** Create dish request - matches backend CreateDishesDto */
    interface CreateDishRequest {
      /** 分类ID - required */
      categoryId: string;
      /** 菜品名称 - required */
      dishName: string;
      /** 价格 - optional */
      price?: number;
      /** 原价 - required */
      originPrice: number;
      /** 封面图片URL - optional */
      coverUrl?: string;
    }

    /** Update dish request - matches backend UpdateDishesDto */
    interface UpdateDishRequest {
      /** 分类ID - optional */
      categoryId?: string;
      /** 菜品名称 - optional */
      dishName?: string;
      /** 价格 - optional */
      price?: number;
      /** 原价 - optional */
      originPrice?: number;
      /** 封面图片URL - optional */
      coverUrl?: string;
      /** 是否在售 - optional */
      onSale?: number;
    }

    /** 菜品列表响应 */
    interface DishResponse {
      /** 状态码 */
      code: number;
      /** 消息 */
      message: string;
      /** 菜品数据 */
      data: DishItem[];
      /** 时间戳 */
      timestamp: number;
    }
  }

  /**
   * backend api module: "merchant"
   */
  namespace Merchant {
    /** 商家基本信息 */
    interface MerchantInfo {
      /** 商家ID */
      merchantId: string;
      /** 商家名称 */
      merchantName: string;
      /** 状态 (1: 正常, 0: 停用) */
      status: number;
      /** 联系方式 */
      contactInfo: string;
      /** 地址 */
      location: string;
    }

    /** 销售统计项 */
    interface SalesStatItem {
      /** 统计日期 */
      statDate: string;
      /** 销售数量 */
      salesQty: number;
      /** 销售金额 */
      salesAmount: number;
      /** 商家ID */
      merchantId: string;
    }

    /** 商家信息响应 */
    interface MerchantInfoResponse {
      /** 响应码 */
      code: number;
      /** 响应信息 */
      message: string;
      /** 商家信息 */
      data: MerchantInfo;
    }

    /** 销售统计响应 */
    interface SalesStatResponse {
      /** 响应码 */
      code: number;
      /** 响应信息 */
      message: string;
      /** 销售统计数据 */
      data: SalesStatItem;
    }
  }

  /**
   * backend api module: "coupon"
   */
  namespace Coupon {
    /** 优惠券数据项 */
    interface CouponItem {
      /** 优惠券ID */
      couponId: string;
      /** 面值 */
      faceValue: number;
      /** 使用门槛 */
      threshold: number;
      /** 总数量 */
      totalQty: number;
      /** 已发放数量 */
      issuedQty: number;
      /** 开始时间 */
      startTime: string;
      /** 结束时间 */
      endTime: string;
    }

    /** 优惠券响应 */
    interface CouponResponse {
      /** 响应码 */
      code: number;
      /** 响应信息 */
      message: string;
      /** 优惠券数据 */
      data: CouponItem[];
    }
  }

  /**
   * backend api module: "order"
   */
  namespace Order {
    /** 订单数据项 */
    interface OrderItem {
      /** 订单ID */
      orderId: string;
      /** 用户ID */
      userId: string;
      /** 地址ID */
      addressId: string;
      /** 订单金额 */
      orderAmount: number;
      /** 创建时间 */
      createAt: string;
      /** 订单状态 */
      orderStatus: number;
      /** 对账ID */
      reconId: string;
      /** 优惠券ID */
      couponId: string;
      /** 分配ID */
      assignid: string;
    }

    /** 订单响应 */
    interface OrderResponse {
      /** 响应码 */
      code: number;
      /** 响应信息 */
      message: string;
      /** 订单数据 */
      data: OrderItem[];
    }

    /** 订单状态映射 */
    interface OrderStatusMap {
      [key: number]: string;
    }
  }

  /**
   * backend api module: "announcement"
   */
  namespace Announcement {
    /** Announcement data item - matches backend AnnouncementResponseDto */
    interface AnnouncementItem {
      /** Announcement ID - maps to AnnounceId */
      announceId: string;
      /** Title - maps to Title */
      title: string;
      /** Content - maps to Content (nullable) */
      content: string | null;
      /** Target role - maps to TargetRole (nullable) */
      targetRole: string | null;
      /** Start time - maps to StartAt (DateTime as ISO string) */
      startAt: string;
      /** End time - maps to EndAt (DateTime as ISO string) */
      endAt: string;
    }

    /** Announcements response */
    interface AnnouncementResponse {
      /** Response code */
      code: number;
      /** Response message */
      message: string;
      /** Announcements data */
      data: AnnouncementItem[];
    }
  }

  /**
   * backend api module: "review"
   */
  namespace Review {
    /** 评论数据项 */
    interface ReviewItem {
      /** 评论ID */
      reviewId: string;
      /** 订单ID */
      orderId: string;
      /** 用户ID */
      userId: string;
      /** 评分 (1-5星) */
      rating: number;
      /** 评论内容 */
      content: string;
      /** 是否匿名 1:匿名 2:非匿名 */
      isAnonymous: number;
      /** 评论时间 */
      reviewAt: string;
      /** 评论状态 1:待审核 2:已通过 3:已拒绝 */
      status?: number;
    }

    /** 菜品评论响应 */
    interface DishReviewResponse {
      /** 响应码 */
      code: number;
      /** 响应信息 */
      message: string;
      /** 评论数据 */
      data: ReviewItem[];
    }

    /** 新增评论请求 */
    interface CreateReviewRequest {
      /** 菜品ID */
      dishId: string;
      /** 订单ID */
      orderId: string;
      /** 用户ID */
      userId: string;
      /** 评分 */
      rating: number;
      /** 评论内容 */
      content: string;
      /** 是否匿名 */
      isAnonymous: number;
    }

    /** 更新评论请求 */
    interface UpdateReviewRequest {
      /** 评论ID */
      reviewId: string;
      /** 评分 */
      rating?: number;
      /** 评论内容 */
      content?: string;
      /** 是否匿名 */
      isAnonymous?: number;
      /** 评论状态 */
      status?: number;
    }
  }
}
