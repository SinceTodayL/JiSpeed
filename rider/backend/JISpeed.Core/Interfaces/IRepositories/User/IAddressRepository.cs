using JISpeed.Core.Entities.User;

namespace JISpeed.Core.Interfaces.IRepositories.User
{
    // 地址仓储接口
    public interface IAddressRepository : IBaseRepository<Address, string>
    {
        // === 业务专用查询方法 ===

        // 根据用户ID获取地址列表
        // <param name="userId">用户ID</param>
        // <returns>地址列表</returns>
        Task<List<Address>> GetByUserIdAsync(string userId);


        /// 根据用户ID获取地址列表（分页）

        /// <param name="userId">用户ID</param>
        /// <param name="page">页码</param>
        /// <param name="size">每页大小</param>
        /// <returns>地址列表</returns>
        Task<List<Address>> GetByUserIdAsync(string userId, int page, int size);


        /// 根据用户ID获取地址数量

        /// <param name="userId">用户ID</param>
        /// <returns>地址数量</returns>
        Task<int> GetCountByUserIdAsync(string userId);

        // 根据用户ID获取默认地址
        // <param name="userId">用户ID</param>
        // <returns>默认地址，如果不存在则返回null</returns>
        Task<Address?> GetDefaultByUserIdAsync(string userId);
    }
}
