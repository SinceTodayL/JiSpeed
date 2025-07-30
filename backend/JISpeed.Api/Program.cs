using JISpeed.Core.Entities.Common; // 引入 ApplicationUser 和 ApplicationRole
using JISpeed.Infrastructure.Data; // 引入 OracleDbContext
using Microsoft.EntityFrameworkCore; // 用于配置 DbContext
using Microsoft.AspNetCore.Identity;
using JISpeed.Api.Extensions;
using JISpeed.Api.Mappers;
using JISpeed.Application.Services.Common;
using JISpeed.Application.Services.Email;
using JISpeed.Application.Services.Customer;
using JISpeed.Application.Services.Merchant;
using JISpeed.Application.Services.Rider;
using JISpeed.Application.Services.Admin;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Infrastructure.Redis;

var builder = WebApplication.CreateBuilder(args);

// 1. 读取连接字符串（从 appsettings.json）
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Database connection string 'DefaultConnection' is missing.");
}
// 2. 注册 Oracle 数据库上下文
builder.Services.AddDbContext<OracleDbContext>(options => options.UseOracle(connectionString));

// 3. 注册依赖注入（仓储、服务）
// 注册Identity服务（它内部会注册PasswordHasher等）
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<OracleDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddAutoMapper((cfg) => { }, typeof(MerchantProfile).Assembly);
// 4. 控制器和日志等默认配置
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<RedisService>();
// 注册仓储层服务（统一封装）
builder.Services.AddRepositories();
// 注册：接口 -> 实现类
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRiderService, RiderService>();
builder.Services.AddScoped<IMerchantService, MerchantService>();
builder.Services.AddScoped<IAdminService, AdminService>();
// 5. 添加 Swagger
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 6. 中间件配置 - 全局异常处理中间件应该在管道的最前面
app.UseGlobalExceptionHandling();

// 7. 开发环境配置
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 8. 其他中间件
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
