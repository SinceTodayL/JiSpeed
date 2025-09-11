using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JISpeed.Core.Entities.Rider;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Interfaces.IRepositories;

namespace JISpeed.Core.Interfaces.IServices
{
    // 骑手服务接口 - 定义骑手模块的业务逻辑操作
    public interface IRiderService
    {
        // 创建用户实体（当ApplicationUser的UserType=3时调用）
        // <param name="applicationUser">已创建的ApplicationUser</param>
        // <param name="nickname">用户昵称，默认使用用户名</param>
        // <returns>创建的Rider实体</returns>
        Task<Rider> CreateUserEntityAsync(ApplicationUser applicationUser, string? nickname = null);

        // 根据ID获取骑手信息
        // <param name="riderId">骑手ID</param>
        // <returns>骑手实体</returns>
        Task<Rider> GetRiderByIdAsync(string riderId);

        // 获取骑手列表（支持分页和搜索）
        // <param name="page">页码</param>
        // <param name="pageSize">每页大小</param>
        // <param name="searchTerm">搜索关键词</param>
        // <returns>骑手列表和分页信息</returns>
        Task<(IEnumerable<JISpeed.Core.Entities.Rider.Rider> Riders, int TotalCount, int TotalPages)> GetRidersAsync(
            int page, int pageSize, string? searchTerm = null);
            
        // 创建新骑手
        // <param name="rider">骑手实体</param>
        // <returns>创建的骑手实体</returns>
        Task<Rider> CreateRiderAsync(Rider rider);

        // 更新骑手信息
        // <param name="rider">更新的骑手实体</param>
        // <returns>更新后的骑手实体</returns>
        Task<Rider> UpdateRiderAsync(Rider rider);

        // 获取骑手的订单分配列表
        // <param name="riderId">骑手ID</param>
        // <param name="status">订单状态筛选（可选）</param>
        // <returns>订单分配列表</returns>
        Task<IEnumerable<Assignment>> GetRiderAssignmentsAsync(string riderId, int? status = null);

        // 更新订单分配状态（接单/拒单）
        // <param name="riderId">骑手ID</param>
        // <param name="assignId">分配ID</param>
        // <param name="acceptedStatus">接单状态</param>
        // <returns>更新后的订单分配</returns>
        Task<Assignment> UpdateAssignmentStatusAsync(string riderId, string assignId, int acceptedStatus);

        // 骑手确认送达
        // <param name="riderId">骑手ID</param>
        // <param name="orderId">订单ID</param>
        // <param name="deliveredAt">送达时间</param>
        // <param name="deliveryNote">送达备注</param>
        // <returns>确认送达结果</returns>
        Task<bool> ConfirmDeliveryAsync(string riderId, string orderId, DateTime? deliveredAt = null, string? deliveryNote = null);
    }
}
