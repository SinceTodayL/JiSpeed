using System.Threading.Tasks;

namespace JISpeed.Core.Services
{
    public interface IUserService
    {
        Task<bool> LoginAsync(string id, string password);
    }
}