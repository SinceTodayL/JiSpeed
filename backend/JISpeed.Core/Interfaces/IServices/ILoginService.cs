using JISpeed.Core.Entities.Common;

namespace JISpeed.Core.Interfaces.IServices
{
    public interface ILoginService
    {
        Task<string?> GetBusinessEntityId(string applicationUserId, int userType);
        bool IsLocked(ApplicationUser user);
    }
}