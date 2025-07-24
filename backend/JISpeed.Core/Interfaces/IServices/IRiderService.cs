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
    }
}
