using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using JISpeed.Core.Entities.User;
using JISpeed.Core.Entities.Admin;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Entities.Dish;
using JISpeed.Core.Entities.Junctions;
using JISpeed.Core.Entities.Merchant;
using JISpeed.Core.Entities.Order;
using JISpeed.Core.Entities.Reconciliation;
using JISpeed.Core.Entities.Rider;

namespace JISpeed.Infrastructure.Data
{
    public class OracleDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public OracleDbContext(DbContextOptions<OracleDbContext> options) : base(options)
        {
        }

        // User 相关实体
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

        // Admin 相关实体
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Announcement> Announcements { get; set; }

        // Common 相关实体
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<Coupon> Coupons { get; set; }

        // Dish 相关实体
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Category> Categories { get; set; }

        // Junction 相关实体
        public DbSet<DishReview> DishReviews { get; set; }
        public DbSet<OrderDish> OrderDishes { get; set; }

        // Merchant 相关实体
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<SalesStat> SalesStats { get; set; }
        public DbSet<Settlement> Settlements { get; set; }

        // Order 相关实体
        public DbSet<Order> Orders { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<OrderLog> OrderLogs { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Refund> Refunds { get; set; }
        public DbSet<Review> Reviews { get; set; }

        // Reconciliation 相关实体
        public DbSet<Reconciliation> Reconciliations { get; set; }

        // Rider 相关实体
        public DbSet<Rider> Riders { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Performance> Performances { get; set; }
        public DbSet<RiderSchedule> RiderSchedules { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<ScheduleAttendance> ScheduleAttendances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 先调用 IdentityDbContext 的 OnModelCreating 配置 Identity 相关表
            base.OnModelCreating(modelBuilder);

            // 配置 Identity 表名
            modelBuilder.Entity<ApplicationUser>().ToTable("APP_USERS");
            modelBuilder.Entity<ApplicationRole>().ToTable("APP_ROLES");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("APP_USER_ROLES");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("APP_USER_CLAIMS");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("APP_USER_LOGINS");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("APP_ROLE_CLAIMS");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("APP_USER_TOKENS");

            // 配置复合主键
            modelBuilder.Entity<Favorite>()
                .HasKey(f => new { f.UserId, f.DishId });

            modelBuilder.Entity<CartItem>()
                .HasKey(c => new { c.CartItemId, c.UserId });

            modelBuilder.Entity<OrderLog>()
                .HasKey(ol => new { ol.LogId, ol.OrderId });

            modelBuilder.Entity<Performance>()
                .HasKey(p => new { p.RiderId, p.StatsMonth });

            modelBuilder.Entity<RiderSchedule>()
                .HasKey(rs => new { rs.RiderId, rs.ScheduleId });

            modelBuilder.Entity<ScheduleAttendance>()
                .HasKey(sa => new { sa.ScheduleId, sa.AttendanceId });

            modelBuilder.Entity<SalesStat>()
                .HasKey(ss => new { ss.StatDate, ss.MerchantId });

            modelBuilder.Entity<DishReview>()
                .HasKey(dr => new { dr.DishId, dr.ReviewId });

            modelBuilder.Entity<OrderDish>()
                .HasKey(od => new { od.OrderId, od.DishId });

            // 配置实体关系
            // User 和 Address 一对多关系
            modelBuilder.Entity<User>()
                .HasMany(u => u.Addresses)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User 和 DefaultAddress 一对一关系
            modelBuilder.Entity<User>()
                .HasOne(u => u.DefaultAddress)
                .WithMany()
                .HasForeignKey(u => u.DefaultAddrId)
                .OnDelete(DeleteBehavior.SetNull);

            // User 和 ApplicationUser 一对一关系
            modelBuilder.Entity<User>()
                .HasOne(u => u.ApplicationUser)
                .WithOne(au => au.UserEntity)
                .HasForeignKey<User>(u => u.ApplicationUserId);

            // Merchant 和 ApplicationUser 一对一关系
            modelBuilder.Entity<Merchant>()
                .HasOne(m => m.ApplicationUser)
                .WithOne(au => au.MerchantEntity)
                .HasForeignKey<Merchant>(m => m.ApplicationUserId);

            // Rider 和 ApplicationUser 一对一关系
            modelBuilder.Entity<Rider>()
                .HasOne(r => r.ApplicationUser)
                .WithOne(au => au.RiderEntity)
                .HasForeignKey<Rider>(r => r.ApplicationUserId);

            // Admin 和 ApplicationUser 一对一关系
            modelBuilder.Entity<Admin>()
                .HasOne(a => a.ApplicationUser)
                .WithOne(au => au.AdminEntity)
                .HasForeignKey<Admin>(a => a.ApplicationUserId);

            // Merchant 和 SalesStat 一对多关系
            modelBuilder.Entity<Merchant>()
                .HasMany(m => m.SalesStats)
                .WithOne(ss => ss.Merchant)
                .HasForeignKey(ss => ss.MerchantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Merchant 和 Settlement 一对多关系
            modelBuilder.Entity<Merchant>()
                .HasMany(m => m.Settlements)
                .WithOne(s => s.Merchant)
                .HasForeignKey(s => s.MerchantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Merchant 和 Application 一对多关系
            modelBuilder.Entity<Merchant>()
                .HasMany(m => m.Applications)
                .WithOne(a => a.Merchant)
                .HasForeignKey(a => a.MerchantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Merchant 和 Dish 一对多关系
            modelBuilder.Entity<Merchant>()
                .HasMany(m => m.Dishes)
                .WithOne(d => d.Merchant)
                .HasForeignKey(d => d.MerchantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Category 和 Dish 一对多关系
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Dishes)
                .WithOne(d => d.Category)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Category 和 Category（自引用）一对多关系
            modelBuilder.Entity<Category>()
                .HasMany(c => c.SubCategories)
                .WithOne(c => c.ParentCategory)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order 和 User 多对一关系
            modelBuilder.Entity<User>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order 和 Refund 一对多关系
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Refunds)
                .WithOne(r => r.Order)
                .HasForeignKey(r => r.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order 和 Payment 一对多关系
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Payments)
                .WithOne(p => p.Order)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order 和 OrderLog 一对多关系
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderLogs)
                .WithOne(ol => ol.Order)
                .HasForeignKey(ol => ol.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order 和 Complaint 一对多关系
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Complaints)
                .WithOne(c => c.Order)
                .HasForeignKey(c => c.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order 和 Review 一对多关系
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Reviews)
                .WithOne(r => r.Order)
                .HasForeignKey(r => r.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order 和 Reconciliation 多对一关系
            modelBuilder.Entity<Reconciliation>()
                .HasMany(r => r.Orders)
                .WithOne(o => o.Reconciliation)
                .HasForeignKey(o => o.ReconId)
                .OnDelete(DeleteBehavior.SetNull);

            // Order 和 Coupon 多对一关系
            modelBuilder.Entity<Coupon>()
                .HasMany(c => c.Orders)
                .WithOne(o => o.Coupon)
                .HasForeignKey(o => o.CouponId)
                .OnDelete(DeleteBehavior.SetNull);

            // Order 和 Assignment 一对一关系
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Assignment)
                .WithOne(a => a.Order)
                .HasForeignKey<Order>(o => o.AssignId)
                .OnDelete(DeleteBehavior.SetNull);

            // User 和 Coupon 一对多关系
            modelBuilder.Entity<User>()
                .HasMany(u => u.Coupons)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User 和 Favorite 一对多关系
            modelBuilder.Entity<User>()
                .HasMany(u => u.Favorites)
                .WithOne(f => f.User)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User 和 CartItem 一对多关系
            modelBuilder.Entity<User>()
                .HasMany(u => u.CartItems)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User 和 Complaint 一对多关系
            modelBuilder.Entity<User>()
                .HasMany(u => u.Complaints)
                .WithOne(c => c.Complainant)
                .HasForeignKey(c => c.ComplainantId)
                .OnDelete(DeleteBehavior.Restrict);

            // User 和 Refund 一对多关系
            modelBuilder.Entity<User>()
                .HasMany(u => u.Refunds)
                .WithOne(r => r.Applicant)
                .HasForeignKey(r => r.ApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            // User 和 Review 一对多关系
            modelBuilder.Entity<User>()
                .HasMany(u => u.Reviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Dish 和 Favorite 一对多关系
            modelBuilder.Entity<Dish>()
                .HasMany(d => d.Favorites)
                .WithOne(f => f.Dish)
                .HasForeignKey(f => f.DishId)
                .OnDelete(DeleteBehavior.Cascade);

            // Dish 和 CartItem 一对多关系
            modelBuilder.Entity<Dish>()
                .HasMany(d => d.CartItems)
                .WithOne(c => c.Dish)
                .HasForeignKey(c => c.DishId)
                .OnDelete(DeleteBehavior.Cascade);

            // 多对多关系配置
            // Dish 和 Review 多对多关系 (通过 DishReview 联结表)
            modelBuilder.Entity<DishReview>()
                .HasOne(dr => dr.Dish)
                .WithMany(d => d.DishReviews)
                .HasForeignKey(dr => dr.DishId);

            modelBuilder.Entity<DishReview>()
                .HasOne(dr => dr.Review)
                .WithMany(r => r.DishReviews)
                .HasForeignKey(dr => dr.ReviewId);

            // Order 和 Dish 多对多关系 (通过 OrderDish 联结表)
            modelBuilder.Entity<OrderDish>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDishes)
                .HasForeignKey(od => od.OrderId);

            modelBuilder.Entity<OrderDish>()
                .HasOne(od => od.Dish)
                .WithMany(d => d.OrderDishes)
                .HasForeignKey(od => od.DishId);

            // Rider 和 Schedule 多对多关系 (通过 RiderSchedule 联结表)
            modelBuilder.Entity<RiderSchedule>()
                .HasOne(rs => rs.Rider)
                .WithMany(r => r.RiderSchedules)
                .HasForeignKey(rs => rs.RiderId);

            modelBuilder.Entity<RiderSchedule>()
                .HasOne(rs => rs.Schedule)
                .WithMany(s => s.RiderSchedules)
                .HasForeignKey(rs => rs.ScheduleId);

            // Schedule 和 Attendance 多对多关系 (通过 ScheduleAttendance 联结表)
            modelBuilder.Entity<ScheduleAttendance>()
                .HasOne(sa => sa.Schedule)
                .WithMany(s => s.ScheduleAttendances)
                .HasForeignKey(sa => sa.ScheduleId);

            modelBuilder.Entity<ScheduleAttendance>()
                .HasOne(sa => sa.Attendance)
                .WithMany(a => a.ScheduleAttendances)
                .HasForeignKey(sa => sa.AttendanceId);

            // Rider 和 Performance 一对多关系
            modelBuilder.Entity<Rider>()
                .HasMany(r => r.Performances)
                .WithOne(p => p.Rider)
                .HasForeignKey(p => p.RiderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Rider 和 Attendance 一对多关系
            modelBuilder.Entity<Rider>()
                .HasMany(r => r.Attendances)
                .WithOne(a => a.Rider)
                .HasForeignKey(a => a.RiderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Rider 和 Assignment 一对多关系
            modelBuilder.Entity<Rider>()
                .HasMany(r => r.Assignments)
                .WithOne(a => a.Rider)
                .HasForeignKey(a => a.RiderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Admin 和 Announcement 一对多关系
            modelBuilder.Entity<Admin>()
                .HasMany(a => a.Announcements)
                .WithOne(an => an.Admin)
                .HasForeignKey(an => an.AdminId)
                .OnDelete(DeleteBehavior.Cascade);

            // Admin 和 Application 一对多关系
            modelBuilder.Entity<Admin>()
                .HasMany(a => a.Applications)
                .WithOne(ap => ap.Admin)
                .HasForeignKey(ap => ap.AdminId)
                .OnDelete(DeleteBehavior.SetNull);

            // 配置表名称 - 修正为大写以匹配实体中的Table注解
            modelBuilder.Entity<User>().ToTable("CUSTOMUSER");
            modelBuilder.Entity<Address>().ToTable("ADDRESS");
            modelBuilder.Entity<CartItem>().ToTable("CARTITEM");
            modelBuilder.Entity<Favorite>().ToTable("FAVORITE");
            modelBuilder.Entity<Admin>().ToTable("ADMIN");
            modelBuilder.Entity<Announcement>().ToTable("ANNOUNCEMENT");
            modelBuilder.Entity<Coupon>().ToTable("COUPON");
            modelBuilder.Entity<Dish>().ToTable("DISH");
            modelBuilder.Entity<Category>().ToTable("CATEGORY");
            modelBuilder.Entity<DishReview>().ToTable("DISH_REVIEW");
            modelBuilder.Entity<OrderDish>().ToTable("ORDER_DISH");
            modelBuilder.Entity<Merchant>().ToTable("MERCHANT");
            modelBuilder.Entity<Application>().ToTable("APPLICATION");
            modelBuilder.Entity<SalesStat>().ToTable("SALESSTAT");
            modelBuilder.Entity<Settlement>().ToTable("SETTLEMENT");
            modelBuilder.Entity<Order>().ToTable("ORDERS");
            modelBuilder.Entity<Complaint>().ToTable("COMPLAINT");
            modelBuilder.Entity<OrderLog>().ToTable("ORDERLOG");
            modelBuilder.Entity<Payment>().ToTable("PAYMENT");
            modelBuilder.Entity<Refund>().ToTable("REFUND");
            modelBuilder.Entity<Review>().ToTable("REVIEW");
            modelBuilder.Entity<Reconciliation>().ToTable("RECONCILIATION");
            modelBuilder.Entity<Rider>().ToTable("RIDER");
            modelBuilder.Entity<Assignment>().ToTable("ASSIGNMENT");
            modelBuilder.Entity<Attendance>().ToTable("ATTENDANCE");
            modelBuilder.Entity<Performance>().ToTable("PERFORMANCE");
            modelBuilder.Entity<RiderSchedule>().ToTable("RIDER_SCHEDULE");
            modelBuilder.Entity<Schedule>().ToTable("SCHEDULE");
            modelBuilder.Entity<ScheduleAttendance>().ToTable("SCHEDULE_ATTENDANCE");

            // 配置 Oracle 特定的列类型
            modelBuilder.Entity<User>()
                .Property(u => u.UserId)
                .HasColumnType("VARCHAR2(450)");

            modelBuilder.Entity<Merchant>()
                .Property(m => m.MerchantId)
                .HasColumnType("VARCHAR2(450)");

            modelBuilder.Entity<Rider>()
                .Property(r => r.RiderId)
                .HasColumnType("VARCHAR2(450)");

            modelBuilder.Entity<Admin>()
                .Property(a => a.AdminId)
                .HasColumnType("VARCHAR2(450)");
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // 在保存前处理审计信息等
            var now = DateTime.UtcNow;
            
            // 遍历所有被添加或修改的实体
            // 如果需要自动添加审计信息，可以在这里处理
            
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
