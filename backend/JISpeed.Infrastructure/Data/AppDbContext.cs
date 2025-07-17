using Microsoft.EntityFrameworkCore;
using JISpeed.Core.Entities;

namespace JISpeed.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        public DbSet<UUser> Users { get; set; }

    }
}