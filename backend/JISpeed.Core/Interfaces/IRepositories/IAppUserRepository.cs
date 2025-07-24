using JISpeed.Core.Entities.Common;

namespace JISpeed.Core.Interfaces.IRepositories
{
    public interface IAppUserRepository
    {
        Task<ApplicationUser?>FindByEmailAndTypeAsync(string email,int userType);
    }
}