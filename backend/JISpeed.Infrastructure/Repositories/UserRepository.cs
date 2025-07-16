using System.Threading.Tasks;
using JISpeed.Core.Entities;
using JISpeed.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using JISpeed.Infrastructure.Data;

namespace JISpeed.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            // 假设User映射到数据库表Users
            return await _context.Users.SingleOrDefaultAsync(u => u.Id == id);
        }
    }
}