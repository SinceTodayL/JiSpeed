using System.Threading.Tasks;
using JISpeed.Core.Entities;
    
namespace JISpeed.Core.Services
{
    public interface IUserService
    {
        Task<bool> LoginAsync(string id, string password);
        //Task<List<UUser>> GetAllUsersAsync();
    }
}