using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JISpeed.Core.Entities.Rider;
using JISpeed.Core.Interfaces.IRepositories;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Constants;
using Microsoft.Extensions.Logging;

namespace JISpeed.Application.Services
{
    // 骑手服务实现
    public class RiderService : IRiderService
    {
        private readonly IRiderRepository _riderRepository;
        private readonly ILogger<RiderService> _logger;

        public RiderService(IRiderRepository riderRepository, ILogger<RiderService> logger)
        {
            _riderRepository = riderRepository;
            _logger = logger;
        }

        // 根据ID获取骑手信息
        // <param name="riderId">骑手ID</param>
        // <returns>骑手实体</returns>
        public async Task<Rider> GetRiderByIdAsync(string riderId)
        {
            try
            {
                _logger.LogInformation("开始获取骑手信息, RiderId: {RiderId}", riderId);

                if (string.IsNullOrWhiteSpace(riderId))
                {
                    _logger.LogWarning("骑手ID参数为空");
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

        // 创建新骑手
        // <param name="rider">骑手实体</param>
        // <returns>创建的骑手实体</returns>
        public async Task<Rider> CreateRiderAsync(Rider rider)
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
                await _riderRepository.AddAsync(rider);

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
        public async Task<Rider> UpdateRiderAsync(Rider rider)
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
                var assignments = await _riderRepository.GetAssignmentsAsync(riderId);

                // 根据状态筛选
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

        // 更新订单分配状态（接单/拒单）
        // <param name="riderId">骑手ID</param>
        // <param name="assignId">分配ID</param>
        // <param name="acceptedStatus">接单状态</param>
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

                // 获取订单分配
                var assignment = await _riderRepository.GetAssignmentByIdAsync(assignId);
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

                // 验证订单状态是否允许更新
                if (assignment.AcceptedStatus != 0) // 假设0表示未处理
                {
                    _logger.LogWarning("订单分配状态不允许更新, AssignId: {AssignId}, CurrentStatus: {CurrentStatus}",
                        assignId, assignment.AcceptedStatus);
                    throw OrderExceptions.OrderStatusError(assignId, assignment.AcceptedStatus, 0);
                }

                // 更新订单分配状态
                assignment.AcceptedStatus = acceptedStatus;
                assignment.AcceptedAt = DateTime.Now;

                await _riderRepository.UpdateAssignmentAsync(assignment);

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
