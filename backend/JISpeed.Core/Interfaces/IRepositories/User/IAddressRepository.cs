using JISpeed.Core.Entities.User;

namespace JISpeed.Core.Interfaces.IRepositories.User
{
    // 地址仓储接口
    public interface IAddressRepository
    {
        // 根据地址ID获取地址信息
        // <param name="addressId">地址ID</param>
        // <returns>地址实体，如果不存在则返回null</returns>
        Task<Address?> GetByIdAsync(string addressId);

        // 根据地址ID获取地址详细信息（包含关联数据）
        // <param name="addressId">地址ID</param>
        // <returns>包含关联数据的地址实体，如果不存在则返回null</returns>
        Task<Address?> GetWithDetailsAsync(string addressId);

        // 根据用户ID获取地址列表
        // <param name="userId">用户ID</param>
        // <returns>地址列表</returns>
        Task<List<Address>> GetByUserIdAsync(string userId);

        // 根据用户ID获取默认地址
        // <param name="userId">用户ID</param>
        // <returns>默认地址，如果不存在则返回null</returns>
        Task<Address?> GetDefaultByUserIdAsync(string userId);

        // 获取所有地址列表
        // <returns>地址列表</returns>
        Task<List<Address>> GetAllAsync();

        // 检查地址是否存在
        // <param name="addressId">地址ID</param>
        // <returns>地址是否存在</returns>
        Task<bool> ExistsAsync(string addressId);

        // 创建新地址
        // <param name="address">地址实体</param>
        // <returns>创建的地址实体</returns>
        Task<Address> CreateAsync(Address address);

        // 更新地址信息
        // <param name="address">地址实体</param>
        // <returns>更新的地址实体</returns>
        Task<Address> UpdateAsync(Address address);

        // 删除地址
        // <param name="addressId">地址ID</param>
        // <returns>是否删除成功</returns>
        Task<bool> DeleteAsync(string addressId);

        // 保存更改
        // <returns>保存的记录数</returns>
        Task<int> SaveChangesAsync();
    }
}
