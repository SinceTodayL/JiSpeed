using JISpeed.Core.Entities.Common;
using JISpeed.Core.Interfaces.IRepositories;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Repositories
{
    public class AppUserRepository : IAppUserRepository
    {
        private readonly OracleDbContext _context;
        public AppUserRepository(OracleDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationUser?> FindByEmailAndTypeAsync(string email, int userType)
        {
            return await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == email&& u.UserType == userType);
        }
        
        

    }
}