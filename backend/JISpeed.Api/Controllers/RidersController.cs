using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JISpeed.Api.Common;
using JISpeed.Api.DTOS;
using JISpeed.Api.Mappers;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Core.Entities.Rider;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Constants;

namespace JISpeed.Api.Controllers
{
    // 骑手API控制器
    [ApiController]
    [Route("api/[controller]")]
    public class RidersController : ControllerBase
    {
        private readonly IRiderService _riderService;
        private readonly ILogger<RidersController> _logger;

        public RidersController(IRiderService riderService, ILogger<RidersController> logger)
        {
            _riderService = riderService;
            _logger = logger;
        }

        // 获取骑手信息
        // <param name="riderId">骑手ID</param>
        // <returns>骑手信息</returns>
        [HttpGet("{riderId}")]
        [ProducesResponseType(typeof(ApiResponse<RiderDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<RiderDTO>>> GetRider(string riderId)
        {
            try
            {
                _logger.LogInformation("收到获取骑手信息请求, RiderId: {RiderId}", riderId);

                var rider = await _riderService.GetRiderByIdAsync(riderId);
                var riderDto = RiderMapper.ToRiderDTO(rider);

                return Ok(ApiResponse<RiderDTO>.Success(riderDto));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败, RiderId: {RiderId}", riderId);
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "骑手不存在, RiderId: {RiderId}", riderId);
                return NotFound(ApiResponse<object>.Fail(
                    ex.ErrorCode,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取骑手信息时发生异常, RiderId: {RiderId}", riderId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "获取骑手信息失败"));
            }
        }

        // 创建骑手
        // <param name="createRiderDto">创建骑手请求</param>
        // <returns>创建的骑手信息</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<RiderDTO>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<RiderDTO>>> CreateRider([FromBody] CreateRiderDTO createRiderDto)
        {
            try
            {
                _logger.LogInformation("收到创建骑手请求, Name: {Name}, Phone: {Phone}",
                    createRiderDto.Name, createRiderDto.PhoneNumber);

                // 参数验证
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.ValidationFailed,
                        "参数验证失败",
                        ModelState.ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        )));
                }

                // 转换DTO为实体
                var rider = RiderMapper.ToRiderEntity(createRiderDto);

                // 调用服务创建骑手
                var createdRider = await _riderService.CreateRiderAsync(rider);

                // 转换结果为DTO
                var riderDto = RiderMapper.ToRiderDTO(createdRider);

                return CreatedAtAction(nameof(GetRider), new { riderId = riderDto.RiderId },
                    ApiResponse<RiderDTO>.Success(riderDto, "骑手创建成功"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败, Name: {Name}, Phone: {Phone}",
                    createRiderDto.Name, createRiderDto.PhoneNumber);
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning(ex, "业务规则验证失败, Name: {Name}, Phone: {Phone}",
                    createRiderDto.Name, createRiderDto.PhoneNumber);
                return BadRequest(ApiResponse<object>.Fail(
                    ex.ErrorCode,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建骑手时发生异常, Name: {Name}, Phone: {Phone}",
                    createRiderDto.Name, createRiderDto.PhoneNumber);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "创建骑手失败"));
            }
        }

        // 更新骑手信息
        // <param name="riderId">骑手ID</param>
        // <param name="updateRiderDto">更新骑手请求</param>
        // <returns>更新后的骑手信息</returns>
        [HttpPatch("{riderId}")]
        [ProducesResponseType(typeof(ApiResponse<RiderDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<RiderDTO>>> UpdateRider(
            string riderId,
            [FromBody] UpdateRiderDTO updateRiderDto)
        {
            try
            {
                _logger.LogInformation("收到更新骑手请求, RiderId: {RiderId}", riderId);

                // 参数验证
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.ValidationFailed,
                        "参数验证失败",
                        ModelState.ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        )));
                }

                // 获取现有骑手
                var existingRider = await _riderService.GetRiderByIdAsync(riderId);

                // 更新骑手信息
                RiderMapper.UpdateRiderFromDTO(existingRider, updateRiderDto);

                // 调用服务更新骑手
                var updatedRider = await _riderService.UpdateRiderAsync(existingRider);

                // 转换结果为DTO
                var riderDto = RiderMapper.ToRiderDTO(updatedRider);

                return Ok(ApiResponse<RiderDTO>.Success(riderDto, "骑手信息更新成功"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败, RiderId: {RiderId}", riderId);
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "骑手不存在, RiderId: {RiderId}", riderId);
                return NotFound(ApiResponse<object>.Fail(
                    ex.ErrorCode,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning(ex, "业务规则验证失败, RiderId: {RiderId}", riderId);
                return BadRequest(ApiResponse<object>.Fail(
                    ex.ErrorCode,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新骑手信息时发生异常, RiderId: {RiderId}", riderId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "更新骑手信息失败"));
            }
        }

        // 获取骑手的订单分配列表
        // <param name="riderId">骑手ID</param>
        // <param name="status">订单状态筛选（可选）</param>
        // <returns>订单分配列表</returns>
        [HttpGet("{riderId}/assignments")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssignmentDTO>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<AssignmentDTO>>>> GetRiderAssignments(
            string riderId,
            [FromQuery] int? status = null)
        {
            try
            {
                _logger.LogInformation("收到获取骑手订单分配列表请求, RiderId: {RiderId}, Status: {Status}",
                    riderId, status.HasValue ? status.Value.ToString() : "全部");

                var assignments = await _riderService.GetRiderAssignmentsAsync(riderId, status);
                var assignmentDtos = RiderMapper.ToAssignmentDTOs(assignments);

                return Ok(ApiResponse<IEnumerable<AssignmentDTO>>.Success(assignmentDtos));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败, RiderId: {RiderId}", riderId);
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "骑手不存在, RiderId: {RiderId}", riderId);
                return NotFound(ApiResponse<object>.Fail(
                    ex.ErrorCode,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取骑手订单分配列表时发生异常, RiderId: {RiderId}", riderId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "获取骑手订单分配列表失败"));
            }
        }

        // 更新订单分配状态
        // <param name="riderId">骑手ID</param>
        // <param name="assignId">分配ID</param>
        // <param name="updateStatusDto">更新状态请求</param>
        // <returns>更新后的订单分配</returns>
        [HttpPatch("{riderId}/assignments/{assignId}")]
        [ProducesResponseType(typeof(ApiResponse<AssignmentDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<AssignmentDTO>>> UpdateAssignmentStatus(
            string riderId,
            string assignId,
            [FromBody] UpdateAssignmentStatusDTO updateStatusDto)
        {
            try
            {
                _logger.LogInformation("收到更新订单分配状态请求, RiderId: {RiderId}, AssignId: {AssignId}, Status: {Status}",
                    riderId, assignId, updateStatusDto.AcceptedStatus);

                // 参数验证
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.Fail(
                        ErrorCodes.ValidationFailed,
                        "参数验证失败",
                        ModelState.ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        )));
                }

                // 调用服务更新订单分配状态
                var assignment = await _riderService.UpdateAssignmentStatusAsync(
                    riderId, assignId, updateStatusDto.AcceptedStatus);

                // 转换结果为DTO
                var assignmentDto = RiderMapper.ToAssignmentDTO(assignment);

                // 根据状态返回不同的消息
                string message = updateStatusDto.AcceptedStatus switch
                {
                    1 => "接单成功",
                    2 => "拒单成功",
                    3 => "完成订单成功",
                    _ => "状态更新成功"
                };

                return Ok(ApiResponse<AssignmentDTO>.Success(assignmentDto, message));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败, RiderId: {RiderId}, AssignId: {AssignId}",
                    riderId, assignId);
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "资源不存在, RiderId: {RiderId}, AssignId: {AssignId}",
                    riderId, assignId);
                return NotFound(ApiResponse<object>.Fail(
                    ex.ErrorCode,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning(ex, "业务规则验证失败, RiderId: {RiderId}, AssignId: {AssignId}",
                    riderId, assignId);
                return BadRequest(ApiResponse<object>.Fail(
                    ex.ErrorCode,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新订单分配状态时发生异常, RiderId: {RiderId}, AssignId: {AssignId}",
                    riderId, assignId);
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "更新订单分配状态失败"));
            }
        }
    }
}
