declare namespace Api {
  /** Common response structure */
  interface Response<T = any> {
    code: number;
    message: string | null;
    data: T;
    timestamp?: number;
  }

  /** User API */
  namespace User {
    /** User info */
    interface UserInfo {
      userId: string;
      account: string;
      status: number;
      createdAt: string;
      deletedAt: string | null;
      lastLoginAt: string;
      lastLoginIp: string;
      nickName: string;
      avatarUrl: string;
      gender: number;
      birthday: string;
      level: number;
      defaultAddrId: string;
    }

    /** User complaint */
    interface Complaint {
      complaintId: string;
      orderId: string;
      complainantId: string;
      role: number;
      description: string;
      status: string;
      createdAt: string;
    }
  }

  /** Merchant API */
  namespace Merchant {
    /** Merchant info */
    interface MerchantInfo {
      merchantId: string;
      merchantName: string;
      status: number;
      contactInfo: string;
      location: string;
    }

    /** Sales statistics */
    interface SalesStats {
      statDate: string;
      salesQty: number;
      salesAmount: number;
      merchantId: string;
    }

    /** Dish review */
    interface DishReview {
      reviewId: string;
      orderId: string;
      user: string;
      rating: number;
      content: string;
      isAnonymous: number;
      reviewAt: string;
    }
  }

  /** Rider API */
  namespace Rider {
    /** Rider info */
    interface RiderInfo {
      riderId: string;
      name: string;
      phoneNumber: string;
      vehicleNumber: string;
      applicationUserId: string;
      status?: number;
    }

    /** Rider performance */
    interface Performance {
      riderId: string;
      statsMonth: string;
      totalOrders: number;
      onTimeRate: number;
      goodReviewRate: number;
      badReviewRate: number;
      income: number;
    }
  }

  /** Order API */
  namespace Order {
    /** Order info */
    interface OrderInfo {
      orderId: string;
      userId: string;
      merchantId: string;
      orderStatus: number;
      totalAmount: number;
      createdAt: string;
      assignId?: string;
      merchantName?: string;
      userName?: string;
      address?: {
        addressId: string;
        longitude?: number;
        latitude?: number;
        detailAddress: string;
      };
    }

    /** Order assignment info */
    interface AssignmentInfo {
      assignmentId: string;
      orderId: string;
      riderId: string;
      assignedAt: string;
      assignmentStatus: number;
      remark?: string;
    }

    /** Auto assignment result */
    interface AutoAssignmentResult {
      isSuccess: boolean;
      assignmentId?: string;
      riderId?: string;
      errorMessage?: string;
    }

    /** Order assignment request */
    interface AssignmentRequest {
      orderId: string;
      riderId: string;
    }

    /** Order status update request */
    interface StatusUpdateRequest {
      orderId: string;
      newStatus: number;
      remark?: string;
    }
  }
}