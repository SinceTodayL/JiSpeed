using System.Threading.Tasks;
using JISpeed.Core.Entities;
    
namespace JISpeed.Core.Services
{
    public interface IUserService
    {
        // 登录
        Task<bool> LoginAsync(string id, string password);
        
        // 测试登录
        Task<List<UUser>> GetAllUsersAsync();
        
        //注册
        Task<bool> RegiAsync(string id, string password);
    }
}