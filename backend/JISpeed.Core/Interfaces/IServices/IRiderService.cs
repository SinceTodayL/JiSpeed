using JISpeed.Core.Entities.Rider;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Interfaces.IRepositories;

namespace JISpeed.Core.Interfaces.IServices
{
    public interface IRiderService
    {
        /// 创建用户实体（当ApplicationUser的UserType=3时调用）
        
        /// <param name="applicationUser">已创建的ApplicationUser</param>
        /// <param name="nickname">用户昵称，默认使用用户名</param>
        /// <returns>创建的Rider实体</returns>
        Task<Rider> CreateUserEntityAsync(ApplicationUser applicationUser, string? nickname = null);

    }
}