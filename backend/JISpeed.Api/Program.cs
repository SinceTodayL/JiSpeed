using JISpeed.Core.Entities.Common; // 引入 ApplicationUser 和 ApplicationRole
using JISpeed.Infrastructure.Data; // 引入 OracleDbContext
using Microsoft.EntityFrameworkCore; // 用于配置 DbContext
using Microsoft.AspNetCore.Identity;


var builder = WebApplication.CreateBuilder(args);

// 1. 读取连接字符串（从 appsettings.json）
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// 2. 注册 Oracle 数据库上下文
builder.Services.AddDbContext<OracleDbContext>(options => options.UseOracle(connectionString));
    
// 3. 注册依赖注入（仓储、服务）
// 注册Identity服务（它内部会注册PasswordHasher等）
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<OracleDbContext>()
    .AddDefaultTokenProviders();


// 4. 控制器和日志等默认配置
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); 

var app = builder.Build();

// 5. 中间件配置
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
