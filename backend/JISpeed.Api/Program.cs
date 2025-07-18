using JISpeed.Core.Services;
using JISpeed.Infrastructure.Data;
using JISpeed.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. 读取连接字符串（从 appsettings.json）
//string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
string connectionString ="User Id=BACKEND_WRITER_01;Password=Database07&;Data Source=121.4.90.75:1521/XEPDB1";
// 2. 注册 Oracle 数据库上下文
builder.Services.AddDbContext<AppDbContext>(options => options.UseOracle(connectionString)
    .LogTo(Console.WriteLine)
    .EnableSensitiveDataLogging());
    
// 3. 注册依赖注入（仓储、服务）
// 注册Identity服务（它内部会注册PasswordHasher等）
builder.Services.AddIdentity<UUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// 注册 Data Protection（解决 DataProtectorTokenProvider 问题）
builder.Services.AddDataProtection();

// 注册 TimeProvider（解决 TimeProvider 无法解析的问题）
builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);

// 仓储和服务注册
//builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<IUserService, UserService>();

// 4. 控制器和日志等默认配置
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // 如果你需要 OpenAPI / Swagger（Apifox 不必需）

var app = builder.Build();

// 5. 中间件配置
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
