using JISpeed.Application.Services.Common;
using JISpeed.Core.Constants;
using JISpeed.Core.Entities.Order;
using JISpeed.Core.Entities.Rider;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Interfaces.IRepositories.Order;
using JISpeed.Core.Interfaces.IRepositories.Rider;
using JISpeed.Core.Interfaces.IServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderEntity = JISpeed.Core.Entities.Order.Order;

namespace JISpeed.Application.Services.Order
{
    // 订单分配服务实现 - 处理订单自动分配和骑手接单的业务逻辑
    public class OrderAssignmentService : IOrderAssignmentService
    {
        private readonly ILogger<OrderAssignmentService> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IRiderLocationService _riderLocationService;
        private readonly IMapService _mapService;
        private readonly IRiderRepository _riderRepository;

        public OrderAssignmentService(
            ILogger<OrderAssignmentService> logger,
            IOrderRepository orderRepository,
            IAssignmentRepository assignmentRepository,
            IRiderLocationService riderLocationService,
            IMapService mapService,
            IRiderRepository riderRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _assignmentRepository = assignmentRepository;
            _riderLocationService = riderLocationService;
            _mapService = mapService;
            _riderRepository = riderRepository;
        }

        // 自动分配订单给最近的在线骑手
        public async Task<string?> AutoAssignOrderAsync(string orderId)
        {
            try
            {
                _logger.LogInformation("开始自动分配订单, OrderId: {OrderId}", orderId);

                // 获取订单详情
                var order = await _orderRepository.GetWithDetailsAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning("订单不存在, OrderId: {OrderId}", orderId);
                    throw OrderExceptions.OrderNotFound(orderId);
                }

                // 检查订单状态是否允许分配
                if (order.OrderStatus != (int)OrderStatus.Paid)
                {
                    _logger.LogWarning("订单状态不允许分配, OrderId: {OrderId}, CurrentStatus: {CurrentStatus}", 
                        orderId, order.OrderStatus);
                    throw OrderExceptions.OrderStatusError(orderId, order.OrderStatus, (int)OrderStatus.Paid);
                }

                // 检查订单是否已经分配
                if (!string.IsNullOrEmpty(order.AssignId))
                {
                    _logger.LogWarning("订单已分配, OrderId: {OrderId}, AssignId: {AssignId}", 
                        orderId, order.AssignId);
                    throw new BusinessException(ErrorCodes.OrderAlreadyAssigned, "订单已分配，无法重复分配");
                }

                // 获取在线骑手列表
                var onlineRiders = await _riderLocationService.GetOnlineRiderLocationsAsync();
                if (!onlineRiders.Any())
                {
                    _logger.LogWarning("没有在线骑手可用");
                    return null; // 返回null表示分配失败
                }

                // 计算每个骑手到订单地址的距离
                var riderDistances = new List<(string RiderId, double Distance)>();
                foreach (var riderLocation in onlineRiders)
                {
                    try
                    {
                        // 检查订单地址坐标是否有效
                        if (order.Address.Longitude == null || order.Address.Latitude == null)
                        {
                            _logger.LogWarning("订单地址坐标无效, OrderId: {OrderId}, Longitude: {Longitude}, Latitude: {Latitude}",
                                orderId, order.Address.Longitude, order.Address.Latitude);
                            riderDistances.Add((riderLocation.RiderId, double.MaxValue));
                            continue;
                        }

                        var distance = await _mapService.CalculateDistanceAsync(
                            riderLocation.Longitude,
                            riderLocation.Latitude,
                            order.Address.Longitude.Value,
                            order.Address.Latitude.Value);

                        riderDistances.Add((riderLocation.RiderId, distance));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "计算骑手距离失败, RiderId: {RiderId}", riderLocation.RiderId);
                        // 如果计算距离失败，给一个很大的距离值
                        riderDistances.Add((riderLocation.RiderId, double.MaxValue));
                    }
                }

                // 选择距离最近的骑手
                var nearestRider = riderDistances
                    .OrderBy(x => x.Distance)
                    .FirstOrDefault();

                if (nearestRider.RiderId == null)
                {
                    _logger.LogWarning("无法找到合适的骑手");
                    return null;
                }

                // 创建分配记录
                var assignment = new Assignment
                {
                    AssignId = Guid.NewGuid().ToString("N"),
                    RiderId = nearestRider.RiderId,
                    AssignedAt = DateTime.Now,
                    AcceptedStatus = 0, // 待接单状态
                    TimeOut = 5, // 5分钟超时
                    Rider = await _riderRepository.GetByIdAsync(nearestRider.RiderId) ??
                            throw new BusinessException(ErrorCodes.RiderNotFound, $"骑手不存在: {nearestRider.RiderId}"),
                    Order = order // 设置订单关联
                };

                // 保存分配记录
                await _assignmentRepository.CreateAsync(assignment);

                // 更新订单的分配ID
                order.AssignId = assignment.AssignId;
                order.OrderStatus = (int)OrderStatus.Assigned;
                await _orderRepository.SaveChangesAsync();

                _logger.LogInformation("订单自动分配成功, OrderId: {OrderId}, RiderId: {RiderId}, Distance: {Distance}m", 
                    orderId, nearestRider.RiderId, nearestRider.Distance);

                return assignment.AssignId;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException || ex is BusinessException))
            {
                _logger.LogError(ex, "自动分配订单时发生异常, OrderId: {OrderId}", orderId);
                throw new BusinessException(ErrorCodes.OrderAssignmentFailed, "订单分配失败");
            }
        }

        // 手动分配订单给指定骑手
        public async Task<bool> AssignOrderToRiderAsync(string orderId, string riderId)
        {
            try
            {
                _logger.LogInformation("开始手动分配订单, OrderId: {OrderId}, RiderId: {RiderId}", orderId, riderId);

                // 获取订单详情
                var order = await _orderRepository.GetWithDetailsAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning("订单不存在, OrderId: {OrderId}", orderId);
                    throw OrderExceptions.OrderNotFound(orderId);
                }

                // 检查订单状态
                if (order.OrderStatus != (int)OrderStatus.Paid)
                {
                    _logger.LogWarning("订单状态不允许分配, OrderId: {OrderId}, CurrentStatus: {CurrentStatus}", 
                        orderId, order.OrderStatus);
                    throw OrderExceptions.OrderStatusError(orderId, order.OrderStatus, (int)OrderStatus.Paid);
                }

                // 检查骑手是否在线
                var riderLocation = await _riderLocationService.GetRiderLatestLocationAsync(riderId);
                if (riderLocation == null || riderLocation.Status != 1)
                {
                    _logger.LogWarning("骑手不在线, RiderId: {RiderId}", riderId);
                    throw new BusinessException(ErrorCodes.RiderDeviceOffline, "骑手不在线，无法分配订单");
                }

                // 创建分配记录
                var assignment = new Assignment
                {
                    AssignId = Guid.NewGuid().ToString("N"),
                    RiderId = riderId,
                    AssignedAt = DateTime.Now,
                    AcceptedStatus = 0, // 待接单状态
                    TimeOut = 5, // 5分钟超时
                    Rider = await _riderRepository.GetByIdAsync(riderId) ??
                            throw new BusinessException(ErrorCodes.RiderNotFound, $"骑手不存在: {riderId}"),
                    Order = order // 设置订单关联
                };

                // 保存分配记录
                await _assignmentRepository.CreateAsync(assignment);

                // 更新订单
                order.AssignId = assignment.AssignId;
                order.OrderStatus = (int)OrderStatus.Assigned;
                await _orderRepository.SaveChangesAsync();

                _logger.LogInformation("订单手动分配成功, OrderId: {OrderId}, RiderId: {RiderId}", orderId, riderId);
                return true;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException || ex is BusinessException))
            {
                _logger.LogError(ex, "手动分配订单时发生异常, OrderId: {OrderId}, RiderId: {RiderId}", orderId, riderId);
                throw new BusinessException(ErrorCodes.OrderAssignmentFailed, "订单分配失败");
            }
        }

        // 骑手接受订单
        public async Task<bool> AcceptOrderAsync(string orderId, string riderId)
        {
            try
            {
                _logger.LogInformation("骑手接受订单, OrderId: {OrderId}, RiderId: {RiderId}", orderId, riderId);

                // 获取订单
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning("订单不存在, OrderId: {OrderId}", orderId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, "订单不存在");
                }

                // 检查订单是否已分配
                if (string.IsNullOrEmpty(order.AssignId))
                {
                    _logger.LogWarning("订单未分配, OrderId: {OrderId}", orderId);
                    throw new BusinessException(ErrorCodes.ResourceNotFound, "订单未分配给任何骑手");
                }

                // 用订单的AssignId获取分配记录
                var assignment = await _assignmentRepository.GetByIdAsync(order.AssignId);
                if (assignment == null)
                {
                    _logger.LogWarning("分配记录不存在, OrderId: {OrderId}, AssignId: {AssignId}", orderId, order.AssignId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, "分配记录不存在");
                }

                // 验证骑手身份
                if (assignment.RiderId != riderId)
                {
                    _logger.LogWarning("骑手身份验证失败, OrderId: {OrderId}, ExpectedRider: {ExpectedRider}, ActualRider: {ActualRider}",
                        orderId, assignment.RiderId, riderId);
                    throw new BusinessException(ErrorCodes.Forbidden, "无权操作此订单");
                }

                // 检查分配状态
                if (assignment.AcceptedStatus != 0)
                {
                    _logger.LogWarning("分配状态不允许接受, OrderId: {OrderId}, Status: {Status}",
                        orderId, assignment.AcceptedStatus);
                    throw new BusinessException(ErrorCodes.OrderStatusError, "订单状态不允许接受");
                }

                // 更新分配状态
                assignment.AcceptedStatus = 1; // 已接单
                assignment.AcceptedAt = DateTime.Now;
                await _assignmentRepository.SaveChangesAsync();

                // 更新订单状态（order已经获取过了，不需要重新获取）
                order.OrderStatus = (int)OrderStatus.InDelivery;
                await _orderRepository.SaveChangesAsync();

                _logger.LogInformation("骑手接受订单成功, OrderId: {OrderId}, RiderId: {RiderId}", orderId, riderId);
                return true;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException || ex is BusinessException))
            {
                _logger.LogError(ex, "骑手接受订单时发生异常, OrderId: {OrderId}, RiderId: {RiderId}", orderId, riderId);
                throw new BusinessException(ErrorCodes.OrderAssignmentFailed, "接受订单失败");
            }
        }

        // 骑手拒绝订单
        public async Task<bool> RejectOrderAsync(string orderId, string riderId)
        {
            try
            {
                _logger.LogInformation("骑手拒绝订单, OrderId: {OrderId}, RiderId: {RiderId}", orderId, riderId);

                // 获取订单
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning("订单不存在, OrderId: {OrderId}", orderId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, "订单不存在");
                }

                // 检查订单是否已分配
                if (string.IsNullOrEmpty(order.AssignId))
                {
                    _logger.LogWarning("订单未分配, OrderId: {OrderId}", orderId);
                    throw new BusinessException(ErrorCodes.ResourceNotFound, "订单未分配给任何骑手");
                }

                // 获取分配记录
                var assignment = await _assignmentRepository.GetByIdAsync(order.AssignId);
                if (assignment == null)
                {
                    _logger.LogWarning("分配记录不存在, OrderId: {OrderId}", orderId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, "分配记录不存在");
                }

                // 验证骑手身份
                if (assignment.RiderId != riderId)
                {
                    _logger.LogWarning("骑手身份验证失败, OrderId: {OrderId}, ExpectedRider: {ExpectedRider}, ActualRider: {ActualRider}",
                        orderId, assignment.RiderId, riderId);
                    throw new BusinessException(ErrorCodes.Forbidden, "无权操作此订单");
                }

                // 检查分配状态
                if (assignment.AcceptedStatus != 0)
                {
                    _logger.LogWarning("分配状态不允许拒绝, OrderId: {OrderId}, Status: {Status}",
                        orderId, assignment.AcceptedStatus);
                    throw new BusinessException(ErrorCodes.OrderStatusError, "订单状态不允许拒绝");
                }

                // 删除分配记录
                await _assignmentRepository.DeleteAsync(assignment.AssignId);

                // 重置订单状态
                order.AssignId = null;
                order.OrderStatus = (int)OrderStatus.Paid; // 回到已支付状态，等待重新分配
                await _orderRepository.SaveChangesAsync();

                _logger.LogInformation("骑手拒绝订单成功, OrderId: {OrderId}, RiderId: {RiderId}", orderId, riderId);
                return true;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException || ex is BusinessException))
            {
                _logger.LogError(ex, "骑手拒绝订单时发生异常, OrderId: {OrderId}, RiderId: {RiderId}", orderId, riderId);
                throw new BusinessException(ErrorCodes.OrderAssignmentFailed, "拒绝订单失败");
            }
        }

        // 更新订单状态
        public async Task<bool> UpdateOrderStatusAsync(string orderId, OrderStatus newStatus)
        {
            try
            {
                _logger.LogInformation("更新订单状态, OrderId: {OrderId}, NewStatus: {NewStatus}", orderId, newStatus);

                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning("订单不存在, OrderId: {OrderId}", orderId);
                    throw OrderExceptions.OrderNotFound(orderId);
                }

                order.OrderStatus = (int)newStatus;
                await _orderRepository.SaveChangesAsync();

                _logger.LogInformation("订单状态更新成功, OrderId: {OrderId}, NewStatus: {NewStatus}", orderId, newStatus);
                return true;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "更新订单状态时发生异常, OrderId: {OrderId}", orderId);
                throw new BusinessException(ErrorCodes.OrderStatusError, "更新订单状态失败");
            }
        }

        // 获取订单的分配信息
        public async Task<dynamic?> GetOrderAssignmentAsync(string orderId)
        {
            try
            {
                var order = await _orderRepository.GetWithDetailsAsync(orderId);
                if (order?.AssignId == null)
                {
                    return null;
                }

                var assignment = await _assignmentRepository.GetByIdAsync(order.AssignId);
                if (assignment == null)
                {
                    return null;
                }

                return new
                {
                    AssignId = assignment.AssignId,
                    RiderId = assignment.RiderId,
                    RiderName = assignment.Rider.Name,
                    AssignedAt = assignment.AssignedAt,
                    AcceptedStatus = assignment.AcceptedStatus,
                    AcceptedAt = assignment.AcceptedAt,
                    TimeOut = assignment.TimeOut
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取订单分配信息时发生异常, OrderId: {OrderId}", orderId);
                return null;
            }
        }

        // 获取骑手的当前分配
        public async Task<IEnumerable<dynamic>> GetRiderAssignmentsAsync(string riderId)
        {
            try
            {
                var assignments = await _assignmentRepository.GetByRiderIdAsync(riderId);
                
                return assignments.Select(a => new
                {
                    AssignId = a.AssignId,
                    OrderId = a.Order.OrderId,
                    OrderAmount = a.Order.OrderAmount,
                    AssignedAt = a.AssignedAt,
                    AcceptedStatus = a.AcceptedStatus,
                    AcceptedAt = a.AcceptedAt,
                    TimeOut = a.TimeOut
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取骑手分配信息时发生异常, RiderId: {RiderId}", riderId);
                return Enumerable.Empty<dynamic>();
            }
        }
    }
}