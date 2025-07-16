using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

//引用所有实体类的命名空间
using JISpeed.Core.Entities.User;
using JISpeed.Core.Entities.Order;
using JISpeed.Core.Entities.Dish;
using JISpeed.Core.Entities.Admin;
using JISpeed.Core.Entities.Merchant;
using JISpeed.Core.Entities.Common;
using JISpeed.Core.Entities.Reconciliation;
using JISpeed.Core.Entities.Rider;
using JISpeed.Core.Entities.Junctions;

namespace JISpeed.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //--- DbSet 属性，对应数据库中的表 ---

        //User 模块
        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Address> Addresses { get; set; }

        //Order 模块
        public DbSet<Refund> Refunds { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<OrderLog> OrderLogs { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Order> Orders { get; set; } 

        //Admin 模块
        public DbSet<Admin> Admins { get; set; }

        //Merchant 模块
        public DbSet<Application> Applications { get; set; }
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<SalesStat> SalesStats { get; set; }
        public DbSet<Settlement> Settlements { get; set; }

        //Dish 模块
        public DbSet<Category> Categories { get; set; }
        public DbSet<Dish> Dishes { get; set; }

        //Common 模块
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Coupon> Coupons { get; set; }

        //Reconciliation 模块
        public DbSet<Reconciliation> Reconciliations { get; set; }

        //Junctions 模块
        public DbSet<DishReview> DishReviews { get; set; }
        public DbSet<OrderDish> OrderDishes { get; set; }

        //Rider 模块
        public DbSet<Rider> Riders { get; set; } 
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Performance> Performances { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<RiderSchedule> RiderSchedules { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<ScheduleAttendance> ScheduleAttendances { get; set; }



        //--- 其他 DbSet 属性 ---
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //--- User 模块关系配置 ---
            //UserProfile 一对一关系
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserProfile)
                .WithOne(up => up.User)
                .HasForeignKey<UserProfile>(up => up.UserId);

            //User 与 Address 的一对多关系
            modelBuilder.Entity<User>()
                .HasMany(u => u.Addresses)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId);

            //UserProfile 与默认地址的关系
            modelBuilder.Entity<UserProfile>()
                .HasOne(up => up.DefaultAddress)
                .WithMany()
                .HasForeignKey(up => up.DefaultAddrId)
                .IsRequired(false);

            //User 与 Favorite 的一对多关系
            modelBuilder.Entity<User>()
                .HasMany(u => u.Favorites)
                .WithOne(f => f.User)
                .HasForeignKey(f => f.UserId);

            //User 与 CartItem 的一对多关系
            modelBuilder.Entity<User>()
                .HasMany(u => u.CartItems)
                .WithOne(ci => ci.User)
                .HasForeignKey(ci => ci.UserId);

            //User 与 Complaint 的一对多关系
            modelBuilder.Entity<User>()
                .HasMany(u => u.Complaints)
                .WithOne(c => c.Complainant)
                .HasForeignKey(c => c.ComplainantId);

            //User 与 Review 的一对多关系
            modelBuilder.Entity<User>()
                .HasMany(u => u.Reviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId);

            //User 与 Order 的一对多关系
            modelBuilder.Entity<User>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId);

            //User 与 Refund 的一对多关系
            modelBuilder.Entity<User>()
                .HasMany(u => u.Refunds)
                .WithOne(r => r.Applicant)
                .HasForeignKey(r => r.ApplicationId);

            //为 User.Account 添加唯一索引 (已在 User 实体中使用 [Index] 属性)
            modelBuilder.Entity<User>().HasIndex(u => u.Account).IsUnique();

            //--- CartItem 复合主键和关系 ---
            modelBuilder.Entity<CartItem>()
                .HasKey(ci => new { ci.CartItemId, ci.UserId, ci.MerchantId, ci.DishId });

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.User)
                .WithMany(u => u.CartItems)
                .HasForeignKey(ci => ci.UserId);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Merchant)
                .WithMany(m => m.CartItems)
                .HasForeignKey(ci => ci.MerchantId);

            // CartItem 到 Dish 的关系 - 修正为使用正常的导航属性
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Dish)
                .WithMany() // 假设Dish没有反向导航属性到CartItem
                .HasForeignKey(ci => ci.DishId);


            //--- Merchant 模块关系 ---
            modelBuilder.Entity<Merchant>()
                .HasMany(m => m.Applications)
                .WithOne(a => a.Merchant)
                .HasForeignKey(a => a.MerchantId);

            modelBuilder.Entity<Application>()
                .HasOne(a => a.Admin)
                .WithMany(ad => ad.Applications)
                .HasForeignKey(a => a.AdminId)
                .IsRequired(false);

            modelBuilder.Entity<Merchant>()
                .HasMany(m => m.Dishes)
                .WithOne(d => d.Merchant)
                .HasForeignKey(d => d.MerchantId);

            modelBuilder.Entity<Merchant>()
                .HasMany(m => m.SalesStats)
                .WithOne(ss => ss.Merchant)
                .HasForeignKey(ss => ss.MerchantId);

            modelBuilder.Entity<Merchant>()
                .HasMany(m => m.Settlements)
                .WithOne(s => s.Merchant)
                .HasForeignKey(s => s.MerchantId);

            //Category 自引用关系配置
            modelBuilder.Entity<Category>()
                .HasMany(c => c.SubCategories)
                .WithOne(c => c.ParentCategory)
                .HasForeignKey(c => c.ParentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Dish>()
                .HasOne(d => d.Category)
                .WithMany(c => c.Dishes)
                .HasForeignKey(d => d.CategoryId);

            modelBuilder.Entity<Dish>()
                .HasOne(d => d.Merchant)
                .WithMany(m => m.Dishes)
                .HasForeignKey(d => d.MerchantId);

            //--- Order 模块关系 ---
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Address)
                .WithMany()
                .HasForeignKey(o => o.AddressId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Reconciliation)
                .WithMany(r => r.Orders)
                .HasForeignKey(o => o.ReconId)
                .IsRequired(false);
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Coupon)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CouponId)
                .IsRequired(false);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Assignment)
                .WithOne(a => a.Order)
                .HasForeignKey<Assignment>(a => a.AssignId)
                .IsRequired(false);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.Refunds)
                .WithOne(r => r.Order)
                .HasForeignKey(r => r.OrderId);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.Payments)
                .WithOne(p => p.Order)
                .HasForeignKey(p => p.OrderId);

            //OrderLog 复合主键
            modelBuilder.Entity<OrderLog>().HasKey(ol => new { ol.LogId, ol.OrderId });
            modelBuilder.Entity<OrderLog>()
                .HasOne(ol => ol.Order)
                .WithMany(o => o.OrderLogs)
                .HasForeignKey(ol => ol.OrderId);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.Complaints)
                .WithOne(c => c.Order)
                .HasForeignKey(c => c.OrderId);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.Reviews)
                .WithOne(r => r.Order)
                .HasForeignKey(r => r.OrderId);


            //--- Reconciliation 模块关系 ---
            modelBuilder.Entity<Reconciliation>()
                .HasMany(r => r.Orders)
                .WithOne(o => o.Reconciliation)
                .HasForeignKey(o => o.ReconId)
                .IsRequired(false);


            // --- Coupon 模块关系 ---
            // Coupon 可以被多个订单使用，已在 Order 实体配置


            //--- Junctions (多对多联结表) ---
            //DishReview (dish_review)
            modelBuilder.Entity<DishReview>().HasKey(dr => new { dr.DishId, dr.ReviewId });

            modelBuilder.Entity<DishReview>()
                .HasOne(dr => dr.Dish)
                .WithMany(d => d.DishReviews)
                .HasForeignKey(dr => dr.DishId);

            modelBuilder.Entity<DishReview>()
                .HasOne(dr => dr.Review)
                .WithMany(r => r.DishReviews)
                .HasForeignKey(dr => dr.ReviewId);

            //OrderDish (order_dish)
            modelBuilder.Entity<OrderDish>().HasKey(od => new { od.OrderId, od.DishId });

            modelBuilder.Entity<OrderDish>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDishes)
                .HasForeignKey(od => od.OrderId);

            modelBuilder.Entity<OrderDish>()
                .HasOne(od => od.Dish)
                .WithMany(d => d.OrderDishes)
                .HasForeignKey(od => od.DishId);

            //--- Rider 模块关系 ---
            //骑手与配送任务的一对多关系
            modelBuilder.Entity<Rider>()
                .HasMany(r => r.Assignments)
                .WithOne(a => a.Rider)
                .HasForeignKey(a => a.RiderId);

            //Performance 复合主键：骑手ID和统计月份
            modelBuilder.Entity<Performance>().HasKey(p => new { p.RiderId, p.StatsMonth });
            //骑手与绩效数据的一对多关系
            modelBuilder.Entity<Rider>()
                .HasMany(r => r.Performances)
                .WithOne(p => p.Rider)
                .HasForeignKey(p => p.RiderId);

            //骑手与考勤记录的一对多关系
            modelBuilder.Entity<Rider>()
                .HasMany(r => r.Attendances)
                .WithOne(a => a.Rider)
                .HasForeignKey(a => a.RiderId);

            //RiderSchedule 复合主键：骑手ID和排班ID
            modelBuilder.Entity<RiderSchedule>().HasKey(rs => new { rs.RiderId, rs.ScheduleId });
            //骑手与排班关系的多对多联结表配置
            modelBuilder.Entity<Rider>()
                .HasMany(r => r.RiderSchedules)
                .WithOne(rs => rs.Rider)
                .HasForeignKey(rs => rs.RiderId);

            //排班与骑手关系的多对多联结表配置（另一侧）
            modelBuilder.Entity<RiderSchedule>()
                .HasOne(rs => rs.Schedule)
                .WithMany(s => s.RiderSchedules)
                .HasForeignKey(rs => rs.ScheduleId);

            //ScheduleAttendance 复合主键：排班ID和考勤ID
            modelBuilder.Entity<ScheduleAttendance>().HasKey(sa => new { sa.ScheduleId, sa.AttendanceId });
            //排班与考勤记录的多对多联结表配置
            modelBuilder.Entity<ScheduleAttendance>()
                .HasOne(sa => sa.Schedule)
                .WithMany(s => s.ScheduleAttendances)
                .HasForeignKey(sa => sa.ScheduleId);

            //考勤记录与排班的多对多联结表配置（另一侧）
            modelBuilder.Entity<ScheduleAttendance>()
                .HasOne(sa => sa.Attendance)
                .WithMany(a => a.ScheduleAttendances)
                .HasForeignKey(sa => sa.AttendanceId);
        }
    }
}