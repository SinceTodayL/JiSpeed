using JISpeed.Core.Entities.Common;

namespace JISpeed.Core.Interfaces.IServices
{
    
    public interface IRegisterService
    {
        Task<bool> PreRegisterUserAsync(ApplicationUser request,string passwordHash);
        Task<bool> RegisterUserAsync(string Id, string token);

    }
}