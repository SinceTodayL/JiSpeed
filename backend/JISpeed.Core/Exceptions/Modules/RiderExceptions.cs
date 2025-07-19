using JISpeed.Core.Constants;
using System;

namespace JISpeed.Core.Exceptions
{
    // 骑手模块异常类
    public class RiderExceptions
    {
        // 创建骑手未找到异常
        // riderId: 骑手编号
        // 返回: 未找到异常
        public static NotFoundException RiderNotFound(string riderId)
        {
            return new NotFoundException(ErrorCodes.RiderNotFound, $"骑手 (编号: {riderId}) 未找到");
        }

        // 创建骑手未找到异常(按姓名)
        // name: 骑手姓名
        // 返回: 未找到异常
        public static NotFoundException RiderNotFoundByName(string name)
        {
            return new NotFoundException(ErrorCodes.RiderNotFound, $"骑手 (姓名: {name}) 未找到");
        }

        // 创建骑手状态错误异常
        // riderId: 骑手编号
        // currentStatus: 当前状态
        // requiredStatus: 所需状态
        // 返回: 业务异常
        public static BusinessException RiderStatusError(string riderId, string currentStatus, string requiredStatus)
        {
            return new BusinessException(ErrorCodes.RiderStatusError,
                $"骑手 (编号: {riderId}) 当前状态 ({currentStatus}) 不允许此操作，需要状态: {requiredStatus}");
        }

        // 创建骑手运力已满异常
        // riderId: 骑手编号
        // currentOrders: 当前订单数
        // 返回: 业务异常
        public static BusinessException RiderCapacityExceeded(string riderId, int currentOrders)
        {
            return new BusinessException(ErrorCodes.RiderCapacityExceeded,
                $"骑手 (编号: {riderId}) 当前已有 {currentOrders} 个订单，无法接受更多订单");
        }

        // 创建骑手不在服务区域异常
        // riderId: 骑手编号
        // location: 当前位置
        // 返回: 业务异常
        public static BusinessException RiderOutOfServiceArea(string riderId, string location)
        {
            return new BusinessException(ErrorCodes.RiderOutOfServiceArea,
                $"骑手 (编号: {riderId}) 当前位置 ({location}) 不在服务区域内");
        }

        // 创建骑手账号已存在异常
        // phoneNumber: 联系电话
        // 返回: 业务异常
        public static BusinessException RiderAlreadyExists(string phoneNumber)
        {
            return new BusinessException(ErrorCodes.RiderAlreadyExists,
                $"骑手账号 (电话: {phoneNumber}) 已存在");
        }

        // 创建骑手认证失败异常
        // riderId: 骑手编号
        // reason: 失败原因
        // 返回: 业务异常
        public static BusinessException RiderAuthenticationFailed(string riderId, string reason)
        {
            return new BusinessException(ErrorCodes.RiderAuthenticationFailed,
                $"骑手 (编号: {riderId}) 认证失败: {reason}");
        }

        // 创建骑手位置更新失败异常
        // riderId: 骑手编号
        // reason: 失败原因
        // 返回: 业务异常
        public static BusinessException RiderLocationUpdateFailed(string riderId, string reason)
        {
            return new BusinessException(ErrorCodes.RiderLocationUpdateFailed,
                $"骑手 (编号: {riderId}) 位置更新失败: {reason}");
        }

        // 创建骑手评分过低异常
        // riderId: 骑手编号
        // rating: 当前评分
        // minimumRating: 最低要求评分
        // 返回: 业务异常
        public static BusinessException RiderRatingTooLow(string riderId, decimal rating, decimal minimumRating)
        {
            return new BusinessException(ErrorCodes.RiderRatingTooLow,
                $"骑手 (编号: {riderId}) 当前评分 ({rating}) 低于系统要求 ({minimumRating})");
        }

        // 创建骑手休息中异常
        // riderId: 骑手编号
        // 返回: 业务异常
        public static BusinessException RiderOnBreak(string riderId)
        {
            return new BusinessException(ErrorCodes.RiderOnBreak,
                $"骑手 (编号: {riderId}) 当前处于休息状态，无法接单");
        }

        // 创建骑手设备离线异常
        // riderId: 骑手编号
        // 返回: 业务异常
        public static BusinessException RiderDeviceOffline(string riderId)
        {
            return new BusinessException(ErrorCodes.RiderDeviceOffline,
                $"骑手 (编号: {riderId}) 设备离线，请检查网络连接");
        }

        // 创建车辆信息错误异常
        // riderId: 骑手编号
        // vehicleNumber: 车牌号
        // reason: 错误原因
        // 返回: 业务异常
        public static BusinessException RiderVehicleError(string riderId, string vehicleNumber, string reason)
        {
            return new BusinessException(ErrorCodes.RiderDeviceOffline,
                $"骑手 (编号: {riderId}) 车辆 (车牌: {vehicleNumber}) 信息错误: {reason}");
        }
    }
}
