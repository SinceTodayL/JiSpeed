using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JISpeed.Core.Entities.Rider;

namespace JISpeed.Core.Interfaces.IRepositories.Rider
{
    // 骑手仓储接口 - 处理骑手基础信息的数据访问操作
    public interface IRiderRepositoryNew : IBaseRepository<JISpeed.Core.Entities.Rider.Rider, string>
    {
        // === 业务专用查询方法 ===

        // 根据ApplicationUserId查询骑手
        Task<JISpeed.Core.Entities.Rider.Rider?> GetByApplicationUserIdAsync(string applicationUserId);

        // 根据手机号查询骑手
        Task<JISpeed.Core.Entities.Rider.Rider?> GetByPhoneNumberAsync(string phoneNumber);

        // 根据手机号检查骑手是否存在
        Task<bool> ExistsByPhoneAsync(string phoneNumber);

        // 根据姓名搜索骑手
        Task<IEnumerable<JISpeed.Core.Entities.Rider.Rider>> SearchByNameAsync(string name);

        // 根据车牌号查询骑手
        Task<JISpeed.Core.Entities.Rider.Rider?> GetByVehicleNumberAsync(string vehicleNumber);

        // 检查车牌号是否已存在
        Task<bool> ExistsByVehicleNumberAsync(string vehicleNumber);

        // 获取有车牌号的骑手
        Task<IEnumerable<JISpeed.Core.Entities.Rider.Rider>> GetRidersWithVehicleAsync();

        // 获取没有车牌号的骑手
        Task<IEnumerable<JISpeed.Core.Entities.Rider.Rider>> GetRidersWithoutVehicleAsync();
    }
}
