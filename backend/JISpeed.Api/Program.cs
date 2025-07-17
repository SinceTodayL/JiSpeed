using JISpeed.Core.Repositories;
using JISpeed.Infrastructure.Repositories;
using JISpeed.Core.Services;
using JISpeed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. 读取连接字符串（从 appsettings.json）
//string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
string connectionString ="User Id=BACKEND_WRITER_01;Password=Database07&;Data Source=121.4.90.75:1521/XEPDB1";
if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("数据库连接字符串为空，请检查 appsettings.json 配置");
}

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
