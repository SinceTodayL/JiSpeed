using JISpeed.Application.Services.Admin;
using JISpeed.Application.Services.Common;
using JISpeed.Application.Services.Email;
using JISpeed.Application.Services.Merchant;
using JISpeed.Application.Services.Customer;
using JISpeed.Application.Services.Order;
using JISpeed.Application.Services.Platform;
using JISpeed.Application.Services.Rider;
using JISpeed.Core.Interfaces.IRepositories.Admin;
using JISpeed.Core.Interfaces.IRepositories.Common;
using JISpeed.Core.Interfaces.IRepositories.Dish;
using JISpeed.Core.Interfaces.IRepositories.Junctions;
using JISpeed.Core.Interfaces.IRepositories.Merchant;
using JISpeed.Core.Interfaces.IRepositories.Order;
using JISpeed.Core.Interfaces.IRepositories.Reconciliation;
using JISpeed.Core.Interfaces.IRepositories.Rider;
using JISpeed.Core.Interfaces.IRepositories.User;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Infrastructure.DailyServices;
using JISpeed.Infrastructure.Repositories.Admin;
using JISpeed.Infrastructure.Repositories.Common;
using JISpeed.Infrastructure.Repositories.Dish;
using JISpeed.Infrastructure.Repositories.Junctions;
using JISpeed.Infrastructure.Repositories.Merchant;
using JISpeed.Infrastructure.Repositories.Order;
using JISpeed.Infrastructure.Repositories.Reconciliation;
using JISpeed.Infrastructure.Repositories.Rider;
using JISpeed.Infrastructure.Repositories.User;
using Microsoft.Extensions.DependencyInjection;

namespace JISpeed.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRiderRepository, RiderRepository>();
            services.AddScoped<IMerchantRepository, MerchantRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
            services.AddScoped<ISalesStatRepository, SalesStatRepository>();
            services.AddScoped<IDishRepository, DishRepository>();
            services.AddScoped<IApplicationRepository, ApplicationRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderDishRepository, OrderDishRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IComplaintRepository, ComplaintRepository>();
            services.AddScoped<ISettlementRepository,SettlementRepository>();
            services.AddScoped<IPaymentRepository,PaymentRepository>();
            services.AddScoped<IAnnouncementRepository,AnnouncementRepository>();
            services.AddScoped<IReconciliationRepository, ReconciliationRepository>();
            services.AddScoped<ICouponRepository, CouponRepository>();
            services.AddScoped<IOrderLogRepository, OrderLogRepository>();
            services.AddScoped<IRefundRepository, RefundRepository>();
            // 骑手相关仓储
            services.AddScoped<IAssignmentRepository, AssignmentRepository>();
            services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            services.AddScoped<IPerformanceRepository, PerformanceRepository>();
            services.AddScoped<IRiderLocationRepository, RiderLocationRepository>();

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IRegistrationService, RegistrationService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IMerchantService, MerchantService>();
            services.AddScoped<IApplicationService, ApplicationService>();
            services.AddScoped<IDishService, DishService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ISettlementService,SettlementService>();
            services.AddScoped<IRegistrationService, RegistrationService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRiderService, RiderService>();
            services.AddScoped<IMerchantService, MerchantService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<ICouponService, CouponService>();
            
            // 定时任务
            services.AddHostedService<DailyCreator>();
            
            // 骑手相关服务
            services.AddScoped<IPerformanceService, PerformanceService>();
            services.AddScoped<IMapService, AMapService>();
            services.AddScoped<ILocationPushService, LocationPushService>();
            services.AddScoped<IRiderLocationService, RiderLocationService>();
            // 注册考勤服务
            services.AddScoped<IAttendanceService, AttendanceService>();
            
            // 平台服务
            services.AddScoped<IPlatformService, PlatformService>();

            
            return services;
        }
    }
}
