using JISpeed.Application.Services.Common;
using JISpeed.Application.Services.Email;
using JISpeed.Application.Services.Merchant;
using JISpeed.Core.Interfaces.IRepositories.Admin;
using JISpeed.Core.Interfaces.IRepositories.Common;
using JISpeed.Core.Interfaces.IRepositories.Dish;
using JISpeed.Core.Interfaces.IRepositories.Merchant;
using JISpeed.Core.Interfaces.IRepositories.Rider;
using JISpeed.Core.Interfaces.IRepositories.User;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Infrastructure.Repositories.Admin;
using JISpeed.Infrastructure.Repositories.Common;
using JISpeed.Infrastructure.Repositories.Dish;
using JISpeed.Infrastructure.Repositories.Merchant;
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

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IRegistrationService, RegistrationService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IMerchantService, MerchantService>();
            services.AddScoped<IApplicationService, ApplicationService>();
            services.AddScoped<IDishService, DishService>();
            services.AddScoped<ILoginService,LoginService>();
            
            return services;
        }
    }
}