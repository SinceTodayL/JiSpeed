using System.Threading.Tasks;
using JISpeed.Core.Entities;

namespace JISpeed.Core.Repositories
{
    public interface IUserRepository
    {
        Task<UUser> GetUserByIdAsync(string id);
        Task<List<UUser>> GetAllUsersAsync();
    }
}