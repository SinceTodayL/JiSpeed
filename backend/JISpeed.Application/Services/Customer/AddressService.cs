using JISpeed.Core.Entities.User;
using JISpeed.Core.Interfaces.IRepositories.User;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JISpeed.Application.Services.Customer
{
    public class AddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IUserRepository _userRepository;
        private readonly OracleDbContext _context;
        private readonly ILogger<AddressService> _logger;

        public AddressService(
            IAddressRepository addressRepository,
            IUserRepository userRepository,
            OracleDbContext context,
            ILogger<AddressService> logger)
        {
            _addressRepository = addressRepository;
            _userRepository = userRepository;
            _context = context;
            _logger = logger;
        }


        /// 创建地址

        /// <param name="userId">用户ID</param>
        /// <param name="receiverName">收货人姓名</param>
        /// <param name="receiverPhone">收货人电话</param>
        /// <param name="detailedAddress">详细地址</param>
        /// <param name="isDefault">是否默认地址</param>
        /// <returns>地址ID，失败返回null</returns>
        public async Task<string?> CreateAddressAsync(string userId, string receiverName, string receiverPhone, string detailedAddress, int isDefault)
        {
            try
            {
                // 验证用户是否存在
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("用户不存在，UserId: {UserId}", userId);
                    return null;
                }

                var addressId = Guid.NewGuid().ToString("N");

                // 如果设置为默认地址，先将其他地址设为非默认
                if (isDefault == 1)
                {
                    await SetAllAddressesAsNonDefaultAsync(userId);
                }

                // 使用原生 SQL 插入，因为 Address 实体构造函数受限
                var sql = @"
                    INSERT INTO ADDRESS (AddressId, UserId, ReceiverName, ReceiverPhone, DetailedAddress, IsDefault) 
                    VALUES (:addressId, :userId, :receiverName, :receiverPhone, :detailedAddress, :isDefault)";

                var parameters = new[]
                {
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":addressId", addressId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":userId", userId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":receiverName", receiverName),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":receiverPhone", receiverPhone),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":detailedAddress", detailedAddress),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":isDefault", isDefault)
                };

                var result = await _context.Database.ExecuteSqlRawAsync(sql, parameters);

                if (result > 0)
                {
                    _logger.LogInformation("创建地址成功，AddressId: {AddressId}, UserId: {UserId}", addressId, userId);
                    return addressId;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建地址失败，UserId: {UserId}", userId);
                return null;
            }
        }


        /// 更新地址信息

        /// <param name="userId">用户ID</param>
        /// <param name="addressId">地址ID</param>
        /// <param name="receiverName">收货人姓名</param>
        /// <param name="receiverPhone">收货人电话</param>
        /// <param name="detailedAddress">详细地址</param>
        /// <param name="isDefault">是否默认地址</param>
        /// <returns>更新是否成功</returns>
        public async Task<bool> UpdateAddressAsync(string userId, string addressId, string receiverName, string receiverPhone, string detailedAddress, int isDefault)
        {
            try
            {
                // 验证地址是否属于该用户
                var address = await _addressRepository.GetByIdAsync(addressId);
                if (address == null || address.UserId != userId)
                {
                    _logger.LogWarning("地址不存在或不属于该用户，AddressId: {AddressId}, UserId: {UserId}", addressId, userId);
                    return false;
                }

                // 如果设置为默认地址，先将其他地址设为非默认
                if (isDefault == 1 && address.IsDefault != 1)
                {
                    await SetAllAddressesAsNonDefaultAsync(userId);
                }

                // 更新地址信息
                var sql = @"
                    UPDATE ADDRESS 
                    SET ReceiverName = :receiverName, 
                        ReceiverPhone = :receiverPhone, 
                        DetailedAddress = :detailedAddress, 
                        IsDefault = :isDefault
                    WHERE AddressId = :addressId AND UserId = :userId";

                var parameters = new[]
                {
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":receiverName", receiverName),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":receiverPhone", receiverPhone),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":detailedAddress", detailedAddress),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":isDefault", isDefault),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":addressId", addressId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":userId", userId)
                };

                var result = await _context.Database.ExecuteSqlRawAsync(sql, parameters);

                if (result > 0)
                {
                    _logger.LogInformation("更新地址成功，AddressId: {AddressId}, UserId: {UserId}", addressId, userId);
                }

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新地址失败，AddressId: {AddressId}, UserId: {UserId}", addressId, userId);
                return false;
            }
        }


        /// 删除地址

        /// <param name="userId">用户ID</param>
        /// <param name="addressId">地址ID</param>
        /// <returns>删除是否成功</returns>
        public async Task<bool> DeleteAddressAsync(string userId, string addressId)
        {
            try
            {
                // 验证地址是否属于该用户
                var address = await _addressRepository.GetByIdAsync(addressId);
                if (address == null || address.UserId != userId)
                {
                    _logger.LogWarning("地址不存在或不属于该用户，AddressId: {AddressId}, UserId: {UserId}", addressId, userId);
                    return false;
                }

                // 使用仓储层的删除方法
                var result = await _addressRepository.DeleteAsync(addressId);
                if (result)
                {
                    await _addressRepository.SaveChangesAsync();
                    _logger.LogInformation("删除地址成功，AddressId: {AddressId}, UserId: {UserId}", addressId, userId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除地址失败，AddressId: {AddressId}, UserId: {UserId}", addressId, userId);
                return false;
            }
        }


        /// 设置默认地址

        /// <param name="userId">用户ID</param>
        /// <param name="addressId">地址ID</param>
        /// <returns>设置是否成功</returns>
        public async Task<bool> SetDefaultAddressAsync(string userId, string addressId)
        {
            try
            {
                // 验证地址是否属于该用户
                var address = await _addressRepository.GetByIdAsync(addressId);
                if (address == null || address.UserId != userId)
                {
                    _logger.LogWarning("地址不存在或不属于该用户，AddressId: {AddressId}, UserId: {UserId}", addressId, userId);
                    return false;
                }

                // 先将所有地址设为非默认
                await SetAllAddressesAsNonDefaultAsync(userId);

                // 设置指定地址为默认
                var sql = @"
                    UPDATE ADDRESS 
                    SET IsDefault = 1 
                    WHERE AddressId = :addressId AND UserId = :userId";

                var parameters = new[]
                {
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":addressId", addressId),
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":userId", userId)
                };

                var result = await _context.Database.ExecuteSqlRawAsync(sql, parameters);

                if (result > 0)
                {
                    _logger.LogInformation("设置默认地址成功，AddressId: {AddressId}, UserId: {UserId}", addressId, userId);
                }

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "设置默认地址失败，AddressId: {AddressId}, UserId: {UserId}", addressId, userId);
                return false;
            }
        }


        /// 将用户的所有地址设为非默认

        /// <param name="userId">用户ID</param>
        /// <returns>更新的记录数</returns>
        private async Task<int> SetAllAddressesAsNonDefaultAsync(string userId)
        {
            try
            {
                var sql = @"UPDATE ADDRESS SET IsDefault = 0 WHERE UserId = :userId";
                var parameters = new[]
                {
                    new Oracle.ManagedDataAccess.Client.OracleParameter(":userId", userId)
                };

                return await _context.Database.ExecuteSqlRawAsync(sql, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "设置地址为非默认失败，UserId: {UserId}", userId);
                return 0;
            }
        }


        /// 获取用户地址数量

        /// <param name="userId">用户ID</param>
        /// <returns>地址数量</returns>
        public async Task<int> GetAddressCountAsync(string userId)
        {
            try
            {
                var addresses = await _addressRepository.GetByUserIdAsync(userId);
                return addresses.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取地址数量失败，UserId: {UserId}", userId);
                return 0;
            }
        }
    }
}
