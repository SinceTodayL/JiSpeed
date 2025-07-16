using JISpeed.Core.Repositories;
using JISpeed.Infrastructure.Repositories;
using JISpeed.Core.Services;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. 读取连接字符串（从 appsettings.json）
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. 注册 Oracle 数据库上下文
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(connectionString));

// 3. 注册依赖注入（仓储、服务）
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// 4. 控制器和日志等默认配置
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // 如果你需要 OpenAPI / Swagger（Apifox 不必需）

var app = builder.Build();

// 5. 中间件配置
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
