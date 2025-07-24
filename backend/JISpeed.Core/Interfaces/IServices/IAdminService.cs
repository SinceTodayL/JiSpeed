using JISpeed.Core.Entities.Admin;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Interfaces.IRepositories;

namespace JISpeed.Core.Interfaces.IServices
{
    public interface IAdminService
    {
        /// 创建用户实体（当ApplicationUser的UserType=4时调用）
        
        /// <param name="applicationUser">已创建的ApplicationUser</param>
        /// <param name="nickname">用户昵称，默认使用用户名</param>
        /// <returns>创建的Admin实体</returns>
        Task<Admin> CreateUserEntityAsync(ApplicationUser applicationUser, string? nickname = null);

    }
}