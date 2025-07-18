using Microsoft.EntityFrameworkCore;
using JISpeed.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace JISpeed.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<UUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts)
        {
        }

        // 此处可以继续加的业务表
        // public DbSet<T> Orders { get; set; }
        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);
            
            mb.Entity<UUser>(b=>{
                b.ToTable("USERS");
            
                b.Property(u => u.EmailConfirmed)
                    .HasColumnName("EMAIL_CONFIRMED")
                    .HasColumnType("NUMBER(1,0)"); // 显式指定 Oracle 类型
                    
                b.Property(u => u.PhoneNumberConfirmed)
                    .HasColumnName("PHONE_NUMBER_CONFIRMED")
                    .HasColumnType("NUMBER(1,0)");
                    
                b.Property(u => u.TwoFactorEnabled)
                    .HasColumnName("TWO_FACTOR_ENABLED")
                    .HasColumnType("NUMBER(1,0)");
                    
                b.Property(u => u.LockoutEnabled)
                    .HasColumnName("LOCKOUT_ENABLED")
                    .HasColumnType("NUMBER(1,0)");
                    
                // // 2. 日期时间类型映射
                b.Property(u => u.LockoutEnd)
                    .HasColumnName("LOCKOUT_END")
                    .HasConversion(
                        v => v.HasValue ? v.Value.DateTime : (DateTime?)null, // 转换为 DateTime
                        v => v.HasValue ? new DateTimeOffset(v.Value) : (DateTimeOffset?)null // 转换回来
                    )
                    .HasColumnType("TIMESTAMP(7)");
                    
                b.Property(u => u.SecurityStamp)
                    .HasColumnName("SECURITY_STAMP")
                    .HasMaxLength(256);
                
                // // 3. 字符串长度和类型
                b.Property(u => u.UserName)
                    .HasColumnName("USER_NAME")
                    .HasMaxLength(256)
                    .IsRequired();

                b.Property(u => u.PasswordHash)
                    .HasColumnName("PASSWORD_HASH")
                    .HasMaxLength(255);
                
                b.Property(u => u.ConcurrencyStamp)
                    .HasColumnName("CONCURRENCY_STAMP")
                    .HasMaxLength(256);
                b.Property(u => u.NormalizedUserName)
                    .HasColumnName("NORMALIZED_USER_NAME")
                    .HasMaxLength(256);
                b.Property(u => u.PhoneNumber)
                    .HasColumnName("PHONE_NUMBER")
                    .HasMaxLength(50);
                b.Property(u => u.Email)
                    .HasColumnName("EMAIL")
                    .HasMaxLength(256);
                b.Property(u => u.NormalizedEmail)
                    .HasColumnName("NORMALIZED_EMAIL")
                    .HasMaxLength(256);
                // // 4. 整数类型
                b.Property(u => u.AccessFailedCount)
                    .HasColumnName("ACCESS_FAILED_COUNT");
                b.Property(u => u.Id)
                    .HasColumnName("ID");

            });
        }
    }
}