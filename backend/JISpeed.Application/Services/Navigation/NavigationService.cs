using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JISpeed.Core.DTOs;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Core.Interfaces.IRepositories.Order;
using JISpeed.Core.Interfaces.IRepositories.Rider;
using JISpeed.Core.Interfaces.IRepositories.Merchant;
using JISpeed.Core.Interfaces.IRepositories.User;
using JISpeed.Core.Entities.Order;
using JISpeed.Core.Entities.Rider;
using Microsoft.Extensions.Logging;

namespace JISpeed.Application.Services.Navigation
{
    // 导航服务实现类
    public class NavigationService : INavigationService
    {
        private readonly IMapService _mapService;
        private readonly IOrderRepository _orderRepository;
        private readonly IRiderLocationRepository _riderLocationRepository;
        private readonly IMerchantRepository _merchantRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly ILogger<NavigationService> _logger;

        public NavigationService(
            IMapService mapService,
            IOrderRepository orderRepository,
            IRiderLocationRepository riderLocationRepository,
            IMerchantRepository merchantRepository,
            IAddressRepository addressRepository,
            IAssignmentRepository assignmentRepository,
            ILogger<NavigationService> logger)
        {
            _mapService = mapService ?? throw new ArgumentNullException(nameof(mapService));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _riderLocationRepository = riderLocationRepository ?? throw new ArgumentNullException(nameof(riderLocationRepository));
            _merchantRepository = merchantRepository ?? throw new ArgumentNullException(nameof(merchantRepository));
            _addressRepository = addressRepository ?? throw new ArgumentNullException(nameof(addressRepository));
            _assignmentRepository = assignmentRepository ?? throw new ArgumentNullException(nameof(assignmentRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // 根据订单ID获取导航路线（骑手接单后调用）
        // <param name="orderId">订单ID</param>
        // <param name="riderId">骑手ID</param>
        // <returns>导航路线信息</returns>
        public async Task<NavigationRouteInfo> GetOrderNavigationRouteAsync(string orderId, string riderId)
        {
            try
            {
                _logger.LogInformation("开始获取订单导航路线, OrderId: {OrderId}, RiderId: {RiderId}", orderId, riderId);

                // 1. 获取订单信息，包含商家和用户地址
                var order = await _orderRepository.GetWithDetailsAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning("订单不存在, OrderId: {OrderId}", orderId);
                    throw new InvalidOperationException("订单不存在");
                }

                // 2. 验证骑手是否已接单 - 使用现有的Assignment实体关系
                if (string.IsNullOrEmpty(order.AssignId))
                {
                    _logger.LogWarning("订单未分配, OrderId: {OrderId}", orderId);
                    throw new InvalidOperationException("订单未分配");
                }

                var assignment = await _assignmentRepository.GetByIdAsync(order.AssignId);
                if (assignment == null || assignment.RiderId != riderId)
                {
                    _logger.LogWarning("骑手未接此订单或订单未分配, OrderId: {OrderId}, RiderId: {RiderId}", orderId, riderId);
                    throw new InvalidOperationException("骑手未接此订单或订单未分配");
                }

                // 3. 获取骑手当前位置 - 使用现有的GetLatestLocationAsync方法
                var riderLocation = await _riderLocationRepository.GetLatestLocationAsync(riderId);
                if (riderLocation == null)
                {
                    _logger.LogWarning("无法获取骑手位置信息, RiderId: {RiderId}", riderId);
                    throw new InvalidOperationException("无法获取骑手位置信息");
                }

                // 4. 获取商家位置（取餐点）
                var merchant = await _merchantRepository.GetByIdAsync(order.MerchantId);
                if (merchant == null)
                {
                    _logger.LogWarning("商家信息不存在, MerchantId: {MerchantId}", order.MerchantId);
                    throw new InvalidOperationException("商家信息不存在");
                }

                // 5. 获取用户收货地址位置（送餐点）
                var userAddress = await _addressRepository.GetByIdAsync(order.AddressId);
                if (userAddress == null || !userAddress.Longitude.HasValue || !userAddress.Latitude.HasValue)
                {
                    _logger.LogWarning("用户地址信息不完整, AddressId: {AddressId}", order.AddressId);
                    throw new InvalidOperationException("用户地址信息不完整");
                }

                // 6. 生成导航路线：骑手位置 -> 商家位置 -> 用户位置
                var routeInfo = await GenerateMultiPointRouteAsync(
                    riderLocation.Longitude, riderLocation.Latitude,
                    merchant.Longitude, merchant.Latitude,
                    userAddress.Longitude.Value, userAddress.Latitude.Value
                );

                // 7. 设置订单相关信息
                routeInfo.RouteId = Guid.NewGuid().ToString();

                _logger.LogInformation("成功生成订单导航路线, OrderId: {OrderId}, RouteId: {RouteId}", orderId, routeInfo.RouteId);

                return routeInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取订单导航路线失败, OrderId: {OrderId}, RiderId: {RiderId}", orderId, riderId);
                throw new InvalidOperationException($"获取订单导航路线失败: {ex.Message}", ex);
            }
        }

        // 获取实时导航更新（基于骑手实际位置）
        // <param name="orderId">订单ID</param>
        // <param name="riderId">骑手ID</param>
        // <returns>实时导航更新信息</returns>
        public async Task<NavigationUpdateInfo> GetOrderNavigationUpdatesAsync(string orderId, string riderId)
        {
            try
            {
                _logger.LogInformation("开始获取订单导航更新, OrderId: {OrderId}, RiderId: {RiderId}", orderId, riderId);

                // 1. 获取骑手当前位置
                var riderLocation = await _riderLocationRepository.GetLatestLocationAsync(riderId);
                if (riderLocation == null)
                {
                    throw new InvalidOperationException("无法获取骑手位置");
                }

                // 2. 获取订单导航路线
                var routeInfo = await GetOrderNavigationRouteAsync(orderId, riderId);

                // 3. 计算剩余距离和时间
                var remainingInfo = await CalculateRemainingInfoAsync(
                    riderLocation.Longitude, riderLocation.Latitude, routeInfo);

                // 4. 构建导航更新信息
                var updateInfo = new NavigationUpdateInfo
                {
                    RouteId = routeInfo.RouteId,
                    CurrentLongitude = riderLocation.Longitude,
                    CurrentLatitude = riderLocation.Latitude,
                    CurrentSpeed = (double)(riderLocation.Speed ?? 0),
                    RemainingDistance = remainingInfo.RemainingDistance,
                    RemainingTime = remainingInfo.RemainingTime,
                    NextInstruction = remainingInfo.NextInstruction,
                    Timestamp = DateTime.Now
                };

                return updateInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取订单导航更新失败, OrderId: {OrderId}, RiderId: {RiderId}", orderId, riderId);
                throw new InvalidOperationException($"获取订单导航更新失败: {ex.Message}", ex);
            }
        }

        // 获取实时导航路径（基础路径规划）
        public async Task<NavigationRouteInfo> GetRealTimeNavigationAsync(
            decimal startLongitude, decimal startLatitude,
            decimal endLongitude, decimal endLatitude,
            string mode = "driving")
        {
            try
            {
                // 调用现有的地图服务获取导航路径
                var routeInfo = await _mapService.GetNavigationRouteAsync(
                    startLongitude, startLatitude, endLongitude, endLatitude, mode);

                return routeInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取导航路径失败");
                throw new InvalidOperationException($"获取导航路径失败: {ex.Message}", ex);
            }
        }

        // 获取导航路线信息（基础导航路线）
        public async Task<NavigationRouteInfo> GetNavigationRouteAsync(NavigationRouteRequest request)
        {
            try
            {
                // 调用现有的地图服务获取导航路径
                var routeInfo = await _mapService.GetNavigationRouteAsync(
                    request.StartLongitude, request.StartLatitude,
                    request.EndLongitude, request.EndLatitude, request.Mode);

                // 生成唯一的路线ID
                routeInfo.RouteId = Guid.NewGuid().ToString();

                return routeInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取导航路线失败");
                throw new InvalidOperationException($"获取导航路线失败: {ex.Message}", ex);
            }
        }

        // 获取附近的服务点（加油站、餐厅等）
        public async Task<IEnumerable<dynamic>> GetNearbyServicesAsync(
            decimal longitude, decimal latitude,
            string serviceType = "all", int radius = 2000)
        {
            try
            {
                // 调用现有的地图服务获取附近POI
                var pois = await _mapService.GetNearbyPOIsAsync(longitude, latitude, radius);

                // 根据服务类型过滤
                if (serviceType != "all")
                {
                    // TODO: 实现按类型过滤POI的逻辑
                }

                return pois;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取附近服务点失败");
                throw new InvalidOperationException($"获取附近服务点失败: {ex.Message}", ex);
            }
        }

        // 生成多点导航路线（骑手->商家->用户）
        private async Task<NavigationRouteInfo> GenerateMultiPointRouteAsync(
            decimal riderLng, decimal riderLat,
            decimal merchantLng, decimal merchantLat,
            decimal userLng, decimal userLat)
        {
            // 第一段：骑手到商家（取餐）
            var routeToMerchant = await _mapService.GetNavigationRouteAsync(
                riderLng, riderLat, merchantLng, merchantLat, "driving");

            // 第二段：商家到用户（送餐）
            var routeToUser = await _mapService.GetNavigationRouteAsync(
                merchantLng, merchantLat, userLng, userLat, "driving");

            // 合并路线信息
            var combinedRoute = new NavigationRouteInfo
            {
                RouteId = Guid.NewGuid().ToString(),
                TotalDistance = routeToMerchant.TotalDistance + routeToUser.TotalDistance,
                EstimatedDuration = routeToMerchant.EstimatedDuration + routeToUser.EstimatedDuration,
                Mode = "driving",
                Steps = new List<RouteStep>()
            };

            // 骑手到商家的步骤
            combinedRoute.Steps.AddRange(routeToMerchant.Steps);

            // 中间过渡步骤
            combinedRoute.Steps.Add(new RouteStep
            {
                Instruction = "到达商家，准备取餐",
                Distance = 0,
                Duration = 0,
                RoadName = "商家位置",
                TurnType = "arrive",
                Polyline = new List<Coordinate> { new Coordinate { Longitude = merchantLng, Latitude = merchantLat } }
            });

            // 商家到用户的步骤
            combinedRoute.Steps.AddRange(routeToUser.Steps);

            // 合并Polyline坐标点
            combinedRoute.Polyline = CombinePolylines(routeToMerchant.Polyline, routeToUser.Polyline);

            return combinedRoute;
        }

        // 辅助方法
        private string CombinePolylines(string polyline1, string polyline2)
        {
            if (string.IsNullOrEmpty(polyline1)) return polyline2;
            if (string.IsNullOrEmpty(polyline2)) return polyline1;

            return $"{polyline1};{polyline2}";
        }

        // 计算剩余距离、时间和下一个导航指令
        private async Task<RemainingInfo> CalculateRemainingInfoAsync(
            decimal currentLng, decimal currentLat, NavigationRouteInfo routeInfo)
        {
            var remainingInfo = new RemainingInfo();

            try
            {
                if (routeInfo.Steps != null && routeInfo.Steps.Any())
                {
                    // 找到商家位置步骤
                    var merchantStep = routeInfo.Steps.FirstOrDefault(s => s.RoadName == "商家位置");
                    if (merchantStep != null && merchantStep.Polyline.Any())
                    {
                        // 获取商家坐标
                        var merchantCoord = merchantStep.Polyline.First();

                        // 计算到商家的距离
                        var distanceToMerchant = await _mapService.CalculateDistanceAsync(
                            currentLng, currentLat, merchantCoord.Longitude, merchantCoord.Latitude);

                        if (distanceToMerchant <= 100) // 100米范围内
                        {
                            // 骑手在商家位置
                            remainingInfo.CurrentWaypoint = "pickup";

                            // 找到最后一个步骤（用户位置）
                            var lastStep = routeInfo.Steps.Last();
                            if (lastStep.Polyline.Any())
                            {
                                var userCoord = lastStep.Polyline.Last();
                                var distanceToUser = await _mapService.CalculateDistanceAsync(
                                    currentLng, currentLat, userCoord.Longitude, userCoord.Latitude);

                                // 使用高德API重新计算到用户的时间
                                var routeToUser = await _mapService.GetNavigationRouteAsync(
                                    currentLng, currentLat, userCoord.Longitude, userCoord.Latitude, "driving");

                                remainingInfo.RemainingDistance = distanceToUser;
                                remainingInfo.RemainingTime = routeToUser.EstimatedDuration; // 使用API返回的准确时间
                                remainingInfo.NextInstruction = "已到达商家，准备取餐后前往用户位置";
                            }
                        }
                        else
                        {
                            // 骑手在前往商家的途中
                            // 使用高德API重新计算到商家的时间
                            var routeToMerchant = await _mapService.GetNavigationRouteAsync(
                                currentLng, currentLat, merchantCoord.Longitude, merchantCoord.Latitude, "driving");

                            remainingInfo.RemainingDistance = distanceToMerchant;
                            remainingInfo.RemainingTime = routeToMerchant.EstimatedDuration; // 使用API返回的准确时间
                            remainingInfo.NextInstruction = "正在前往商家取餐";
                            remainingInfo.CurrentWaypoint = "en_route";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "计算剩余信息失败");
                // 返回默认值
                remainingInfo.RemainingDistance = routeInfo.TotalDistance;
                remainingInfo.RemainingTime = routeInfo.EstimatedDuration;
                remainingInfo.NextInstruction = "正在导航中";
                remainingInfo.CurrentWaypoint = "en_route";
            }

            return remainingInfo;
        }
    }

    // 剩余信息类
    public class RemainingInfo
    {
        public double RemainingDistance { get; set; }
        public int RemainingTime { get; set; }
        public string NextInstruction { get; set; } = string.Empty;
        public string CurrentWaypoint { get; set; } = string.Empty;
    }
}