using JISpeed.Core.Entities.Common;

namespace JISpeed.Core.Interfaces.IServices
{
    public interface IAppUserService
    {
        Task<ApplicationUser?>FindUserAsync(string email,int userType);
    }
}
