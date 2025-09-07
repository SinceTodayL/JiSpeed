using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JISpeed.Core.Entities.Rider;
using JISpeed.Core.Interfaces.IRepositories.Rider;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Interfaces.IRepositories;
using JISpeed.Core.Interfaces.IRepositories.Order;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Constants;
using Microsoft.Extensions.Logging;

namespace JISpeed.Application.Services.Rider
{
    public class RiderService : IRiderService
    {
        private readonly IRiderRepository _riderRepository;
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<RiderService> _logger;

        public RiderService(
            IRiderRepository riderRepository,
            IAssignmentRepository assignmentRepository,
            IOrderRepository orderRepository,
            ILogger<RiderService> logger)
        {
            _riderRepository = riderRepository;
            _assignmentRepository = assignmentRepository;
            _orderRepository = orderRepository;
            _logger = logger;
        }

        // 创建用户实体（当ApplicationUser的UserType=3时调用）
        // <param name="applicationUser">已创建的ApplicationUser</param>
        // <param name="nickname">用户昵称，默认使用用户名</param>
        // <returns>创建的Rider实体</returns>
        public async Task<JISpeed.Core.Entities.Rider.Rider> CreateUserEntityAsync(ApplicationUser applicationUser, string? nickname = null)
        {
            try
            {
                _logger.LogInformation("开始创建用户实体, ApplicationUserId: {ApplicationUserId}", applicationUser.Id);

                // 参数验证
                if (applicationUser == null)
                {
                    throw new ValidationException("ApplicationUser不能为空");
                }

                if (applicationUser.UserType != 3)
                {
                    throw new ValidationException($"UserType必须为3，当前值: {applicationUser.UserType}");
                }

                // 检查是否已存在关联的Rider实体
                //var existingUser = await _riderRepository.GetUserByApplicationUserIdAsync(applicationUser.Id);
                // if (existingUser != null)
                // {
                //     _logger.LogWarning("用户实体已存在, ApplicationUserId: {ApplicationUserId}", applicationUser.Id);
                //     throw new BusinessException("用户实体已存在");
                // }

                // 生成用户ID和昵称
                var userId = Guid.NewGuid().ToString("N");
                var userNickname = nickname ?? applicationUser.UserName ?? "用户" + userId.Substring(0, 8);

                // 创建User实体
                var user = new JISpeed.Core.Entities.Rider.Rider
                {
                    RiderId = userId,
                    Name = userNickname,
                    PhoneNumber = "1234567890",
                    ApplicationUserId = applicationUser.Id
                };

                // 保存到数据库
                await _riderRepository.CreateAsync(user);
                await _riderRepository.SaveChangesAsync();

                _logger.LogInformation("用户实体创建成功, UserId: {UserId}, ApplicationUserId: {ApplicationUserId}",
                    user.RiderId, applicationUser.Id);

                return user;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is BusinessException))
            {
                _logger.LogError(ex, "创建用户实体时发生异常, ApplicationUserId: {ApplicationUserId}",
                    applicationUser?.Id);
                throw new BusinessException("创建用户实体失败");
            }
        }

        // 根据ID获取骑手信息
        // <param name="riderId">骑手ID</param>
        // <returns>骑手实体</returns>
        public async Task<JISpeed.Core.Entities.Rider.Rider> GetRiderByIdAsync(string riderId)
        {
            try
            {
                _logger.LogInformation("开始获取骑手信息, RiderId: {RiderId}", riderId);

                if (string.IsNullOrWhiteSpace(riderId))
                {
                    _logger.LogWarning("骑手ID不能为空");
                    throw CommonExceptions.ValidationFailed("riderId", "不能为空");
                }

                var rider = await _riderRepository.GetByIdAsync(riderId);

                if (rider == null)
                {
                    _logger.LogWarning("骑手不存在, RiderId: {RiderId}", riderId);
                    throw RiderExceptions.RiderNotFound(riderId);
                }

                _logger.LogInformation("成功获取骑手信息, RiderId: {RiderId}, Name: {Name}",
                    riderId, rider.Name);

                return rider;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取骑手信息时发生异常, RiderId: {RiderId}", riderId);
                throw CommonExceptions.GeneralError($"获取骑手信息失败: {ex.Message}");
            }
        }

        // 获取骑手列表（支持分页和搜索）
        // <param name="page">页码</param>
        // <param name="pageSize">每页大小</param>
        // <param name="searchTerm">搜索关键词</param>
        // <returns>骑手列表和分页信息</returns>
        public async Task<(IEnumerable<JISpeed.Core.Entities.Rider.Rider> Riders, int TotalCount, int TotalPages)> GetRidersAsync(
            int page, int pageSize, string? searchTerm = null)
        {
            try
            {
                _logger.LogInformation("开始获取骑手列表, Page: {Page}, PageSize: {PageSize}, SearchTerm: {SearchTerm}",
                    page, pageSize, searchTerm ?? "无");

                // 获取所有骑手
                var allRiders = await _riderRepository.GetAllAsync();

                // 应用搜索筛选
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    allRiders = allRiders.Where(r => 
                        (r.Name != null && r.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (r.PhoneNumber != null && r.PhoneNumber.Contains(searchTerm))
                    ).ToList();
                }

                // 计算分页信息
                var totalCount = allRiders.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                var skip = (page - 1) * pageSize;

                // 应用分页
                var riders = allRiders
                    .Skip(skip)
                    .Take(pageSize)
                    .ToList();

                _logger.LogInformation("成功获取骑手列表, 总数: {TotalCount}, 当前页: {Page}, 每页大小: {PageSize}",
                    totalCount, page, pageSize);

                return (riders, totalCount, totalPages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取骑手列表时发生异常");
                throw new BusinessException("获取骑手列表失败");
            }
        }
        
        // 创建骑手
        // <param name="rider">骑手实体</param>
        // <returns>创建的骑手实体</returns>
        public async Task<JISpeed.Core.Entities.Rider.Rider> CreateRiderAsync(JISpeed.Core.Entities.Rider.Rider rider)
        {
            try
            {
                _logger.LogInformation("开始创建骑手, Name: {Name}, Phone: {Phone}",
                    rider.Name, rider.PhoneNumber);

                // 参数验证
                if (rider == null)
                {
                    throw CommonExceptions.ValidationFailed("rider", "骑手信息不能为空");
                }

                if (string.IsNullOrWhiteSpace(rider.Name))
                {
                    throw CommonExceptions.ValidationFailed("name", "骑手名称不能为空");
                }

                if (string.IsNullOrWhiteSpace(rider.PhoneNumber))
                {
                    throw CommonExceptions.ValidationFailed("phoneNumber", "骑手手机号不能为空");
                }

                // 检查手机号是否已存在
                var exists = await _riderRepository.ExistsByPhoneAsync(rider.PhoneNumber);
                if (exists)
                {
                    _logger.LogWarning("骑手手机号已存在, Phone: {Phone}", rider.PhoneNumber);
                    throw RiderExceptions.RiderAlreadyExists(rider.PhoneNumber);
                }

                // 生成骑手ID
                if (string.IsNullOrEmpty(rider.RiderId))
                {
                    rider.RiderId = Guid.NewGuid().ToString("N");
                }

                // 保存骑手信息
                await _riderRepository.CreateAsync(rider);
                await _riderRepository.SaveChangesAsync();

                _logger.LogInformation("骑手创建成功, RiderId: {RiderId}, Name: {Name}",
                    rider.RiderId, rider.Name);

                return rider;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is BusinessException))
            {
                _logger.LogError(ex, "创建骑手时发生异常, Name: {Name}, Phone: {Phone}",
                    rider.Name, rider.PhoneNumber);
                throw CommonExceptions.GeneralError($"创建骑手失败: {ex.Message}");
            }
        }

        // 更新骑手信息
        // <param name="rider">更新的骑手实体</param>
        // <returns>更新后的骑手实体</returns>
        public async Task<JISpeed.Core.Entities.Rider.Rider> UpdateRiderAsync(JISpeed.Core.Entities.Rider.Rider rider)
        {
            try
            {
                _logger.LogInformation("开始更新骑手信息, RiderId: {RiderId}", rider.RiderId);

                // 参数验证
                if (rider == null || string.IsNullOrWhiteSpace(rider.RiderId))
                {
                    throw CommonExceptions.ValidationFailed("riderId", "骑手ID不能为空");
                }

                // 检查骑手是否存在
                var existingRider = await _riderRepository.GetByIdAsync(rider.RiderId);
                if (existingRider == null)
                {
                    _logger.LogWarning("骑手不存在, RiderId: {RiderId}", rider.RiderId);
                    throw RiderExceptions.RiderNotFound(rider.RiderId);
                }

                // 更新骑手信息
                await _riderRepository.UpdateAsync(rider);
                await _riderRepository.SaveChangesAsync();

                _logger.LogInformation("骑手信息更新成功, RiderId: {RiderId}", rider.RiderId);

                return rider;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "更新骑手信息时发生异常, RiderId: {RiderId}", rider.RiderId);
                throw CommonExceptions.GeneralError($"更新骑手信息失败: {ex.Message}");
            }
        }

        // 获取骑手的订单分配列表
        // <param name="riderId">骑手ID</param>
        // <param name="status">订单状态筛选（可选）</param>
        // <returns>订单分配列表</returns>
        public async Task<IEnumerable<Assignment>> GetRiderAssignmentsAsync(string riderId, int? status = null)
        {
            try
            {
                _logger.LogInformation("开始获取骑手订单分配列表, RiderId: {RiderId}, Status: {Status}",
                    riderId, status.HasValue ? status.Value.ToString() : "全部");

                if (string.IsNullOrWhiteSpace(riderId))
                {
                    throw CommonExceptions.ValidationFailed("riderId", "骑手ID不能为空");
                }

                // 检查骑手是否存在
                var riderExists = await _riderRepository.GetByIdAsync(riderId) != null;
                if (!riderExists)
                {
                    _logger.LogWarning("骑手不存在, RiderId: {RiderId}", riderId);
                    throw RiderExceptions.RiderNotFound(riderId);
                }

                // 获取订单分配列表
                var assignments = await _assignmentRepository.GetByRiderIdAsync(riderId);

                // 按照状态筛选
                if (status.HasValue)
                {
                    assignments = assignments.Where(a => a.AcceptedStatus == status.Value);
                }

                _logger.LogInformation("成功获取骑手订单分配列表, RiderId: {RiderId}, Count: {Count}",
                    riderId, assignments.Count());

                return assignments;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "获取骑手订单分配列表时发生异常, RiderId: {RiderId}", riderId);
                throw CommonExceptions.GeneralError($"获取骑手订单分配列表失败: {ex.Message}");
            }
        }

        // 更新订单分配状态（接单/拒单/确认送达）
        // <param name="riderId">骑手ID</param>
        // <param name="assignId">分配ID</param>
        // <param name="acceptedStatus">接单状态：0=待接单, 1=已接单, 2=已拒绝, 3=已完成</param>
        // <returns>更新后的订单分配</returns>
        public async Task<Assignment> UpdateAssignmentStatusAsync(string riderId, string assignId, int acceptedStatus)
        {
            try
            {
                _logger.LogInformation("开始更新订单分配状态, RiderId: {RiderId}, AssignId: {AssignId}, Status: {Status}",
                    riderId, assignId, acceptedStatus);

                // 参数验证
                if (string.IsNullOrWhiteSpace(riderId))
                {
                    throw CommonExceptions.ValidationFailed("riderId", "骑手ID不能为空");
                }

                if (string.IsNullOrWhiteSpace(assignId))
                {
                    throw CommonExceptions.ValidationFailed("assignId", "订单分配ID不能为空");
                }

                // 检查骑手是否存在
                var riderExists = await _riderRepository.GetByIdAsync(riderId) != null;
                if (!riderExists)
                {
                    _logger.LogWarning("骑手不存在, RiderId: {RiderId}", riderId);
                    throw RiderExceptions.RiderNotFound(riderId);
                }

                // 获取订单分配（包含关联的订单信息）
                var assignment = await _assignmentRepository.GetByIdAsync(assignId);
                if (assignment == null)
                {
                    _logger.LogWarning("订单分配不存在, AssignId: {AssignId}", assignId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"订单分配不存在，ID: {assignId}");
                }

                // 验证订单分配是否属于该骑手
                if (assignment.RiderId != riderId)
                {
                    _logger.LogWarning("订单分配不属于该骑手, RiderId: {RiderId}, AssignId: {AssignId}",
                        riderId, assignId);
                    throw AuthExceptions.Forbidden(riderId, $"订单分配 (ID: {assignId})");
                }

                // 验证订单状态是否允许操作
                if (acceptedStatus == 3) // 确认送达
                {
                    if (assignment.AcceptedStatus != 1) // 只允许从已接单状态确认送达
                    {
                        _logger.LogWarning("只能从已接单状态确认送达, AssignId: {AssignId}, CurrentStatus: {CurrentStatus}",
                            assignId, assignment.AcceptedStatus);
                        throw OrderExceptions.OrderStatusError(assignId, assignment.AcceptedStatus, 1);
                    }
                }
                else // 其他状态更新（接单、拒单）
                {
                    if (assignment.AcceptedStatus != 0) // 只允许从待接单状态操作
                    {
                        _logger.LogWarning("订单分配状态不允许操作, AssignId: {AssignId}, CurrentStatus: {CurrentStatus}",
                            assignId, assignment.AcceptedStatus);
                        throw OrderExceptions.OrderStatusError(assignId, assignment.AcceptedStatus, 0);
                    }
                }

                // 更新订单分配状态
                assignment.AcceptedStatus = acceptedStatus;
                assignment.AcceptedAt = DateTime.Now;
                await _assignmentRepository.UpdateAsync(assignment);
                await _assignmentRepository.SaveChangesAsync();

                // 如果是确认送达，需要同时更新订单状态
                if (acceptedStatus == 3)
                {
                    // 获取关联的订单并更新状态为已确认收货
                    if (assignment.Order != null)
                    {
                        assignment.Order.OrderStatus = (int)OrderStatus.Confirmed; // 2 = 确认收货
                        await _orderRepository.SaveChangesAsync(); // 保存订单状态更新

                        _logger.LogInformation("订单状态已更新为确认收货, OrderId: {OrderId}", assignment.Order.OrderId);
                    }
                    else
                    {
                        _logger.LogWarning("未找到关联的订单信息, AssignId: {AssignId}", assignId);
                    }
                }

                _logger.LogInformation("订单分配状态更新成功, AssignId: {AssignId}, Status: {Status}",
                    assignId, acceptedStatus);

                return assignment;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException || ex is BusinessException))
            {
                _logger.LogError(ex, "更新订单分配状态时发生异常, RiderId: {RiderId}, AssignId: {AssignId}",
                    riderId, assignId);
                throw CommonExceptions.GeneralError($"更新订单分配状态失败: {ex.Message}");
            }
        }
    }
}
