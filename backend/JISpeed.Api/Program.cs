using JISpeed.Api.Extensions;
using JISpeed.Api.Hubs;
using JISpeed.Api.Mappers;
using JISpeed.Application.Services.Common;
using JISpeed.Application.Services.Email;
using JISpeed.Application.Services.Rider;
using JISpeed.Core.Entities.Common; // 引入 ApplicationUser 和 ApplicationRole
using JISpeed.Core.Interfaces.IRepositories.Rider;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Infrastructure.Data; // 引入 OracleDbContext
using JISpeed.Infrastructure.Redis;
using JISpeed.Infrastructure.Repositories.Rider;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; // 用于配置 DbContext
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 添加 JWT 认证服务
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-256-bit-secret-key-here-123456")),
            ValidateIssuer = false, // 不验证发行人
            ValidateAudience = false, // 不验证受众
            ValidateLifetime = true // 验证 Token 有效期（关键：退出后即使 Token 存在，过期也会失效）
        };
        // 添加认证事件，输出调试信息
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                // 打印 Token 验证失败的原因（如过期、签名错误）
                Console.WriteLine($"JWT 认证失败：{context.Exception.Message}");
                return Task.CompletedTask;
            }
        };
    });

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

// 注册HttpClient，用于高德地图API
builder.Services.AddHttpClient();

// 4. 控制器和日志等默认配置
builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();  // 注册SignalR服务
builder.Services.AddSingleton<RedisService>();

// 认证配置
builder.Services.Configure<AuthenticationOptions>(options =>
{
    // 强制所有认证使用JWT方案
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
});

// 注册仓储层服务（统一封装）
builder.Services.AddRepositories();

// 注册骑手定位相关仓储
builder.Services.AddScoped<IRiderLocationRepository, RiderLocationRepository>();

// 注册：接口 -> 实现类
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
// builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRiderService, RiderService>();
builder.Services.AddScoped<IPerformanceService, PerformanceService>();
// builder.Services.AddScoped<IMerchantService, MerchantService>();
// builder.Services.AddScoped<IAdminService, AdminService>();

// 注册骑手定位相关服务
builder.Services.AddScoped<IMapService, AMapService>();
builder.Services.AddScoped<ILocationPushService, LocationPushService>();
builder.Services.AddScoped<IRiderLocationService, RiderLocationService>();

// 注册考勤服务
builder.Services.AddScoped<IAttendanceService, AttendanceService>();

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
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<LocationHub>("/locationHub");
app.MapControllers();

app.Run();
